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
    public class ObjectObservationDriver : 
        SqliteEntityDriver<ITrakHoundObjectObservationEntity, DatabaseObjectObservation>,
        IObjectObservationQueryDriver,
        IObjectObservationAggregateDriver
    {
        private const int _defaultTTL = 24; // 24 Hours
        private const int _expirationInterval = 10000;

        private const string _dataTableName = "[DATA]";
        private const int _windowSeconds = 3600; // 1 Hour
        private readonly string _entityDirectory = Path.Combine("entities", "trakhound_objects_observation");

        private readonly int _ttl; // Seconds
        private CancellationTokenSource _stop;


        class ValueModel
        {
            public int DataType { get; set; }
            public long ValueInteger { get; set; }
            public double ValueFloat { get; set; }
            public string ValueText { get; set; }
        }


        public ObjectObservationDriver() { }

        public ObjectObservationDriver(ITrakHoundDriverConfiguration configuration) : base(configuration) 
        {
            TableColumnList = new List<string> {
                "[timestamp]",
                "[data_type]",
                "[value_integer]",
                "[value_float]",
                "[value_text]",
                "[batch_id]",
                "[sequence]",
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


        internal string Prepare(IEnumerable<ITrakHoundObjectObservationEntity> entities)
        {
            var now = UnixDateTime.Now;

            var builder = new StringBuilder();
            foreach (var entity in entities)
            {
                object integerValue = NullValue;
                object floatValue = NullValue;
                object textValue = NullValue;

                var dataType = (TrakHoundObservationDataType)entity.DataType;
                switch (dataType)
                {
                    case TrakHoundObservationDataType.Boolean: integerValue = entity.Value; break;
                    case TrakHoundObservationDataType.Byte: integerValue = entity.Value; break;
                    case TrakHoundObservationDataType.Int16: integerValue = entity.Value; break;
                    case TrakHoundObservationDataType.Int32: integerValue = entity.Value; break;
                    case TrakHoundObservationDataType.Int64: integerValue = entity.Value; break;
                    case TrakHoundObservationDataType.Float: floatValue = entity.Value; break;
                    case TrakHoundObservationDataType.Double: floatValue = entity.Value; break;
                    case TrakHoundObservationDataType.Decimal: floatValue = entity.Value; break;
                    case TrakHoundObservationDataType.Timestamp: integerValue = entity.Value; break;
                    case TrakHoundObservationDataType.Duration: integerValue = entity.Value; break;
                    case TrakHoundObservationDataType.String: textValue = entity.Value; break;
                    case TrakHoundObservationDataType.Reference: textValue = entity.Value; break;
                    case TrakHoundObservationDataType.Vocabulary: textValue = entity.Value; break;
                }

                if (textValue != NullValue) textValue = $"'{textValue}'";

                builder.Append($"insert into {_dataTableName}");
                builder.Append(" (");
                builder.Append("\"timestamp\", ");
                builder.Append("\"data_type\", ");
                builder.Append("\"value_integer\", ");
                builder.Append("\"value_float\", ");
                builder.Append("\"value_text\", ");
                builder.Append("\"batch_id\", ");
                builder.Append("\"sequence\", ");
                builder.Append("\"source_uuid\", ");
                builder.Append("\"created\"");
                builder.Append(") ");
                builder.Append("values");
                builder.Append(" (");
                builder.Append($"{entity.Timestamp}, ");
                builder.Append($"{entity.DataType}, ");
                builder.Append($"{integerValue}, ");
                builder.Append($"{floatValue}, ");
                builder.Append($"{textValue}, ");
                builder.Append($"{entity.BatchId}, ");
                builder.Append($"{entity.Sequence}, ");
                builder.Append($"'{entity.SourceUuid}', ");
                builder.Append($"{entity.Created}");
                builder.Append(");");
            }
            return builder.ToString();
        }

        protected async override Task<bool> OnPublish(IEnumerable<ITrakHoundObjectObservationEntity> entities)
        {
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
                    columns.Add("data_type", typeof(int));
                    columns.Add("value_integer", typeof(long));
                    columns.Add("value_float", typeof(double));
                    columns.Add("value_text", typeof(string));
                    columns.Add("batch_id", typeof(long));
                    columns.Add("sequence", typeof(long));
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
                    return _client.ExecuteNonQuery(connectionString, queries);
                }
            }

            return false;
        }


        public async Task<TrakHoundResponse<ITrakHoundObjectObservationEntity>> Query(IEnumerable<TrakHoundRangeQuery> queries, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending)
        {
            Func<IEnumerable<string>, Task<IEnumerable<DatabaseObjectObservation>>> readFunction = async (ids) =>
            {
                var results = new List<DatabaseObjectObservation>();

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

                                var dbEntities = _client.ReadList<DatabaseObjectObservation>(connectionString, dbQuery);
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

        public async Task<TrakHoundResponse<ITrakHoundObjectObservationEntity>> Query(IEnumerable<TrakHoundRangeQuery> queries, IEnumerable<TrakHoundConditionGroupQuery> conditionQueries, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending)
        {
            return TrakHoundResponse<ITrakHoundObjectObservationEntity>.RouteNotConfigured(Id, null);
        }


        public async Task<TrakHoundResponse<ITrakHoundObjectObservationEntity>> Last(IEnumerable<TrakHoundTimeQuery> queries)
        {
            Func<IEnumerable<string>, Task<IEnumerable<DatabaseObjectObservation>>> readFunction = async (ids) =>
            {
                var results = new List<DatabaseObjectObservation>();

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

                                var dbEntities = _client.ReadList<DatabaseObjectObservation>(connectionString, dbQuery);
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

        public async Task<TrakHoundResponse<ITrakHoundObjectObservationEntity>> Last(IEnumerable<TrakHoundTimeQuery> queries, IEnumerable<TrakHoundConditionGroupQuery> conditionQueries)
        {
            return TrakHoundResponse<ITrakHoundObjectObservationEntity>.RouteNotConfigured(Id, null);
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


        public async Task<TrakHoundResponse<TrakHoundAggregate>> Aggregate(IEnumerable<TrakHoundRangeQuery> queries, TrakHoundAggregateType aggregateType)
        {
            Func<IEnumerable<string>, Task<IEnumerable<DatabaseAggregate>>> readFunction = async (ids) =>
            {
                var results = new List<DatabaseAggregate>();

                foreach (var query in queries)
                {
                    var objectUuid = query.Target;
                    var values = new List<double>();

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
                                var dbQuery = $"select [data_type], [value_integer], [value_float], [value_text] from {_dataTableName} where [timestamp] >= {query.From} and [timestamp] < {query.To};";

                                // Create Connection String
                                var connectionString = _client.GetReadConnectionString(timestampFile);

                                var dbValues = _client.ReadList<ValueModel>(connectionString, dbQuery);
                                if (!dbValues.IsNullOrEmpty())
                                {
                                    foreach (var dbValue in dbValues)
                                    {
                                        var dataTypeValue = GetDataTypeValue(dbValue);
                                        if (dataTypeValue.HasValue) values.Add(dataTypeValue.Value);
                                    }
                                }
                            }
                        }
                    }

                    var aggregateValue = ProcessAggregate(aggregateType, values);
                    if (aggregateValue.HasValue)
                    {
                        var result = new DatabaseAggregate();
                        result.RequestedId = objectUuid;
                        result.Value = aggregateValue.Value;
                        results.Add(result);
                    }
                }

                return results;
            };

            return await ProcessResponse(this, queries.Select(o => o.Target).Distinct(), readFunction);
        }

        public async Task<TrakHoundResponse<TrakHoundAggregateWindow>> AggregateWindow(IEnumerable<TrakHoundRangeQuery> queries, TrakHoundAggregateType aggregateType, long window)
        {
            Func<IEnumerable<string>, Task<IEnumerable<DatabaseAggregateWindow>>> readFunction = async (ids) =>
            {
                var results = new List<DatabaseAggregateWindow>();
                var windowSeconds = (int)(window / 1000000000); // Convert ns to seconds

                foreach (var query in queries)
                {
                    var objectUuid = query.Target;

                    var directory = Path.Combine(_baseDirectory, _entityDirectory, objectUuid);
                    var timestampFiles = Directory.GetFiles(directory);
                    if (!timestampFiles.IsNullOrEmpty())
                    {
                        var timeSegments = TimeSegment.GetSegments(query.From, query.To, windowSeconds);
                        foreach (var timeSegment in timeSegments)
                        {
                            var values = new List<double>();
                            var from = timeSegment.From.ToUnixTime();
                            var to = timeSegment.To.ToUnixTime();

                            foreach (var timestampFile in timestampFiles.OrderBy(o => o))
                            {
                                var bottomTimestamp = Path.GetFileNameWithoutExtension(timestampFile).ToLong();
                                var topTimestamp = bottomTimestamp + ((long)_windowSeconds * 1000000000);

                                if (IsBetween(bottomTimestamp, topTimestamp, from, to))
                                {
                                    // Set Query
                                    var dbQuery = $"select [data_type], [value_integer], [value_float], [value_text] from {_dataTableName} where [timestamp] >= {from} and [timestamp] < {to};";

                                    // Create Connection String
                                    var connectionString = _client.GetReadConnectionString(timestampFile);

                                    var dbValues = _client.ReadList<ValueModel>(connectionString, dbQuery);
                                    if (!dbValues.IsNullOrEmpty())
                                    {
                                        foreach (var dbValue in dbValues)
                                        {
                                            var dataTypeValue = GetDataTypeValue(dbValue);
                                            if (dataTypeValue.HasValue) values.Add(dataTypeValue.Value);
                                        }
                                    }
                                }
                            }

                            var aggregateValue = ProcessAggregate(aggregateType, values);
                            if (aggregateValue.HasValue)
                            {
                                var result = new DatabaseAggregateWindow();
                                result.RequestedId = objectUuid;
                                result.Start = from;
                                result.End = to;
                                result.Value = aggregateValue.Value;
                                results.Add(result);
                            }
                        }
                    }
                }

                return results;
            };

            return await ProcessResponse(this, queries.Select(o => o.Target).Distinct(), readFunction);
        }

        private static double? ProcessAggregate(TrakHoundAggregateType aggregateType, IEnumerable<double> values)
        {
            if (!values.IsNullOrEmpty())
            {
                switch (aggregateType)
                {
                    case TrakHoundAggregateType.Mean: return values.Average();
                    case TrakHoundAggregateType.Median: return GetMedian(values.ToArray());
                    case TrakHoundAggregateType.Max: return values.Max();
                    case TrakHoundAggregateType.Min: return values.Min();
                    case TrakHoundAggregateType.Sum: return values.Sum();
                }
            }

            return null;
        }

        private static double? GetDataTypeValue(ValueModel value)
        {
            if (value != null)
            {
                switch ((TrakHoundObservationDataType)value.DataType)
                {
                    case TrakHoundObservationDataType.Boolean: return value.ValueInteger;
                    case TrakHoundObservationDataType.Byte: return value.ValueInteger;
                    case TrakHoundObservationDataType.Int16: return value.ValueInteger;
                    case TrakHoundObservationDataType.Int32: return value.ValueInteger;
                    case TrakHoundObservationDataType.Int64: return value.ValueInteger;
                    case TrakHoundObservationDataType.Float: return value.ValueFloat;
                    case TrakHoundObservationDataType.Double: return value.ValueFloat;
                    case TrakHoundObservationDataType.Decimal: return value.ValueFloat;
                }
            }
            
            return null;
        }

        private static double GetMedian(double[] sourceNumbers)
        {
            //make sure the list is sorted, but use a new array
            double[] sortedPNumbers = (double[])sourceNumbers.Clone();
            Array.Sort(sortedPNumbers);

            //get the median
            int size = sortedPNumbers.Length;
            int mid = size / 2;
            double median = (size % 2 != 0) ? sortedPNumbers[mid] : (sortedPNumbers[mid] + sortedPNumbers[mid - 1]) / 2;
            return median;
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
