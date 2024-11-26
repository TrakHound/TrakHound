// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TrakHound.Drivers;
using TrakHound.Drivers.Entities;
using TrakHound.Entities;
using TrakHound.Sqlite.Drivers.Models;
using TrakHound.Utilities;

namespace TrakHound.Sqlite.Drivers
{
    public class ObjectEventDriver :
        SqliteEntityDriver<ITrakHoundObjectEventEntity, DatabaseObjectEvent>,
        IObjectEventQueryDriver
    {
        private const int _defaultTTL = 24; // 24 Hours
        private const int _expirationInterval = 10000;

        private const string _dataTableName = "[DATA]";
        private const int _windowSeconds = 3600; // 1 Hour
        private readonly string _entityDirectory = Path.Combine("entities", "trakhound_objects_event");

        private readonly int _ttl; // Seconds
        private CancellationTokenSource _stop;


        class ValueModel
        {
            public int DataType { get; set; }
            public long ValueInteger { get; set; }
            public double ValueFloat { get; set; }
            public string ValueText { get; set; }
        }


        public ObjectEventDriver() { }

        public ObjectEventDriver(ITrakHoundDriverConfiguration configuration) : base(configuration)
        {
            TableColumnList = new List<string> {
                "[timestamp]",
                "[target_uuid]",
                "[source_uuid]",
                "[created]"
            };

            _ttl = configuration.ParameterExists("ttl") ? configuration.GetParameter<int>("ttl") : _defaultTTL;

            _stop = new CancellationTokenSource();
            _ = Task.Run(() => TTLWorker(_stop.Token));
        }

        protected override void OnDisposed()
        {
            if (_stop != null) _stop.Cancel();
        }

        private async Task TTLWorker(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var expiredKeys = new HashSet<string>();
                    var now = UnixDateTime.Now;

                    var baseDirectory = Path.Combine(_baseDirectory, _entityDirectory);

                    var objectDirectories = Directory.GetDirectories(baseDirectory);
                    if (!objectDirectories.IsNullOrEmpty())
                    {
                        foreach (var objectDirectory in objectDirectories)
                        {
                            var entityFiles = Directory.GetFiles(objectDirectory);
                            if (!entityFiles.IsNullOrEmpty())
                            {
                                foreach (var entityFile in entityFiles)
                                {
                                    var timestamp = Path.GetFileNameWithoutExtension(entityFile).ToLong();
                                    long diff = now - timestamp;
                                    long ttl = (long)_ttl * 3600000000000;

                                    if (diff > ttl) // Convert TTL (seconds) to Nanoseconds
                                    {
                                        try
                                        {
                                            File.Delete(entityFile);
                                        }
                                        catch { }
                                    }
                                }
                            }
                        }
                    }

                    await Task.Delay(_expirationInterval);
                }
            }
            catch { }
        }


        internal string Prepare(IEnumerable<ITrakHoundObjectEventEntity> entities)
        {
            var now = UnixDateTime.Now;

            var builder = new StringBuilder();
            foreach (var entity in entities)
            {
                builder.Append($"insert into {_dataTableName}");
                builder.Append(" (");
                builder.Append("\"timestamp\", ");
                builder.Append("\"target_uuid\", ");
                builder.Append("\"source_uuid\", ");
                builder.Append("\"created\"");
                builder.Append(") ");
                builder.Append("values");
                builder.Append(" (");
                builder.Append($"{entity.Timestamp}, ");
                builder.Append($"'{entity.TargetUuid}', ");
                builder.Append($"'{entity.SourceUuid}', ");
                builder.Append($"{entity.Created}");
                builder.Append(");");
            }
            return builder.ToString();
        }

        protected async override Task<bool> OnPublish(IEnumerable<ITrakHoundObjectEventEntity> entities)
        {
            var success = false;

            var objectUuids = entities.Select(o => o.ObjectUuid).Distinct();
            foreach (var objectUuid in objectUuids)
            {
                var objectEntities = entities.Where(o => o.ObjectUuid == objectUuid);
                var windows = objectEntities.Select(o => TimeSegment.GetSegmentBottom(o.Timestamp, _windowSeconds)).Distinct();
                foreach (var window in windows)
                {
                    var windowEntities = objectEntities.Where(o => TimeSegment.GetSegmentBottom(o.Timestamp, _windowSeconds) == window);

                    var queries = new List<string>();

                    var columns = new Dictionary<string, Type>();
                    columns.Add("timestamp", typeof(long));
                    columns.Add("target_uuid", typeof(string));
                    columns.Add("source_uuid", typeof(string));
                    columns.Add("created", typeof(long));

                    // Create Table (if not exists)
                    queries.Add(SqliteClient.CreateTableQuery(_dataTableName, columns, new string[] { "timestamp" }));

                    // Create Inserts
                    queries.Add(Prepare(windowEntities));

                    // Create Connection String
                    var filename = $"{window.ToUnixTime()}.sqlite";
                    var dataSource = Path.Combine(_baseDirectory, _entityDirectory, objectUuid, filename);
                    var connectionString = _client.GetWriteConnectionString(dataSource);

                    var dataSourceDirectory = Path.Combine(_baseDirectory, _entityDirectory, objectUuid);
                    if (!Directory.Exists(dataSourceDirectory)) Directory.CreateDirectory(dataSourceDirectory);

                    // Execute Queries
                    success = _client.ExecuteNonQuery(connectionString, queries);
                    if (!success) return false;
                }
            }

            return success;
        }


        public async Task<TrakHoundResponse<ITrakHoundObjectEventEntity>> Query(IEnumerable<TrakHoundRangeQuery> queries, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending)
        {
            Func<IEnumerable<string>, Task<IEnumerable<DatabaseObjectEvent>>> readFunction = async (ids) =>
            {
                var results = new List<DatabaseObjectEvent>();

                foreach (var query in queries)
                {
                    var objectUuid = query.Target;

                    var directory = Path.Combine(_baseDirectory, _entityDirectory, objectUuid);
                    var timestampFiles = Directory.GetFiles(directory);
                    if (!timestampFiles.IsNullOrEmpty())
                    {
                        foreach (var timestampFile in timestampFiles.OrderBy(o => o))
                        {
                            var bottomTimestamp = Path.GetFileNameWithoutExtension(timestampFile).ToLong();
                            var topTimestamp = bottomTimestamp + ((long)_windowSeconds * 1000000000);

                            if (IsBetween(bottomTimestamp, topTimestamp, query.From, query.To))
                            {
                                // Set Query
                                var dbQuery = $"select {TableColumns} from {_dataTableName} where [timestamp] >= {query.From} and [timestamp] < {query.To};";

                                // Create Connection String
                                var connectionString = _client.GetReadConnectionString(timestampFile);

                                var dbEntities = _client.ReadList<DatabaseObjectEvent>(connectionString, dbQuery);
                                if (!dbEntities.IsNullOrEmpty())
                                {
                                    foreach (var dbEntity in dbEntities)
                                    {
                                        dbEntity.RequestedId = objectUuid;
                                        dbEntity.ObjectUuid = objectUuid;
                                        results.Add(dbEntity);
                                    }
                                }
                            }
                        }
                    }
                }

                var lSkip = skip > 0 ? (int)skip : 0;
                var lTake = take > 0 ? (int)take : 0;

                switch (sortOrder)
                {
                    default: return results.OrderBy(o => o.Timestamp).Skip(lSkip).Take(lTake);
                    case SortOrder.Descending: return results.OrderByDescending(o => o.Timestamp).Skip(lSkip).Take(lTake);
                }
            };

            return await ProcessResponse(this, queries.Select(o => o.Target).Distinct(), readFunction, QueryType.Object);
        }


        public async Task<TrakHoundResponse<ITrakHoundObjectEventEntity>> Last(IEnumerable<TrakHoundTimeQuery> queries)
        {
            Func<IEnumerable<string>, Task<IEnumerable<DatabaseObjectEvent>>> readFunction = async (ids) =>
            {
                var results = new List<DatabaseObjectEvent>();

                foreach (var query in queries)
                {
                    var objectUuid = query.Target;

                    var directory = Path.Combine(_baseDirectory, _entityDirectory, objectUuid);
                    var timestampFiles = Directory.GetFiles(directory);
                    if (!timestampFiles.IsNullOrEmpty())
                    {
                        foreach (var timestampFile in timestampFiles.OrderByDescending(o => o))
                        {
                            var bottomTimestamp = Path.GetFileNameWithoutExtension(timestampFile).ToLong();
                            if (bottomTimestamp <= query.Timestamp)
                            {
                                // Set Query
                                var dbQuery = $"select {TableColumns} from {_dataTableName} where [timestamp] <= {query.Timestamp} order by [timestamp] desc limit 1;";

                                // Create Connection String
                                var connectionString = _client.GetReadConnectionString(timestampFile);

                                var dbEntities = _client.ReadList<DatabaseObjectEvent>(connectionString, dbQuery);
                                if (!dbEntities.IsNullOrEmpty())
                                {
                                    foreach (var dbEntity in dbEntities)
                                    {
                                        dbEntity.RequestedId = objectUuid;
                                        dbEntity.ObjectUuid = objectUuid;
                                        results.Add(dbEntity);
                                    }
                                }

                                break;
                            }
                        }
                    }
                }

                return results;
            };

            return await ProcessResponse(this, queries.Select(o => o.Target).Distinct(), readFunction, QueryType.Object);
        }


        public async Task<TrakHoundResponse<TrakHoundCount>> Count(IEnumerable<TrakHoundRangeQuery> queries)
        {
            Func<IEnumerable<string>, Task<IEnumerable<DatabaseEntityCount>>> readFunction = async (ids) =>
            {
                var results = new List<DatabaseEntityCount>();

                foreach (var query in queries)
                {
                    var objectUuid = query.Target;
                    var count = 0;

                    var directory = Path.Combine(_baseDirectory, _entityDirectory, objectUuid);
                    var timestampFiles = Directory.GetFiles(directory);
                    if (!timestampFiles.IsNullOrEmpty())
                    {
                        foreach (var timestampFile in timestampFiles.OrderBy(o => o))
                        {
                            var bottomTimestamp = Path.GetFileNameWithoutExtension(timestampFile).ToLong();
                            var topTimestamp = bottomTimestamp + ((long)_windowSeconds * 1000000000);

                            if (IsBetween(bottomTimestamp, topTimestamp, query.From, query.To))
                            {
                                // Set Query
                                var dbQuery = $"select count(*) from {_dataTableName} where [timestamp] >= {query.From} and [timestamp] < {query.To};";

                                // Create Connection String
                                var connectionString = _client.GetReadConnectionString(timestampFile);

                                var dbCount = _client.ReadString(connectionString, dbQuery).ToInt();
                                if (dbCount > 0)
                                {
                                    count += dbCount;
                                }
                            }
                        }
                    }

                    var entityCount = new DatabaseEntityCount();
                    entityCount.RequestedId = objectUuid;
                    entityCount.Count = count;
                    results.Add(entityCount);
                }

                return results;
            };

            return await ProcessResponse(this, queries.Select(o => o.Target).Distinct(), readFunction);
        }


        private static bool IsBetween(long fileFrom, long fileTo, long from, long to)
        {
            if (fileTo > fileFrom && to > from)
            {
                if (fileFrom >= from && fileFrom < to && fileTo < 1) return true;
                if (fileFrom >= from && fileFrom < to) return true;
                if (fileFrom <= from && fileTo >= to) return true;
                if (fileFrom >= from && fileTo > from && fileTo < to) return true;
            }

            return false;
        }
    }
}
