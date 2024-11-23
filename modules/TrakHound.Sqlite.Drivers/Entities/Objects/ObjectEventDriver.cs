// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrakHound.Drivers;
using TrakHound.Drivers.Entities;
using TrakHound.Entities;
using TrakHound.Sqlite.Drivers.Models;

namespace TrakHound.Sqlite.Drivers
{
    public class ObjectEventDriver : 
        SqliteEntityDriver<ITrakHoundObjectEventEntity, DatabaseObjectEvent>, 
        IObjectEventLatestDriver
        //IObjectEventQueryDriver
    {
        public ObjectEventDriver() { }

        public ObjectEventDriver(ITrakHoundDriverConfiguration configuration) : base(configuration) 
        {
            EntityName = "trakhound_objects_event";
            TableColumnList = new List<string> {
                "[uuid]",
                "[object_uuid]",
                "[target_uuid]",
                "[timestamp]",
                "[source_uuid]",
                "[created]"
            };
        }

        class PublishItem
        {
            public string Uuid { get; set; }
            public string ObjectUuid { get; set; }
            public string TargetUuid { get; set; }
            public long Timestamp { get; set; }             
            public string SourceUuid { get; set; }
            public long Created { get; set; }      
        }


        protected async override Task<bool> OnPublish(IEnumerable<ITrakHoundObjectEventEntity> entities)
        {
            var items = new List<PublishItem>();
            foreach (var entity in entities)
            {
                var item = new PublishItem();
                item.Uuid = entity.Uuid;
                item.ObjectUuid = entity.ObjectUuid;
                item.TargetUuid = entity.TargetUuid;
                item.Timestamp = entity.Timestamp;
                item.SourceUuid = entity.SourceUuid;
                item.Created = entity.Created;
                items.Add(item);
            }

            _client.Insert(GetWriteConnectionString(), items, $"[{EntityName}_latest]", new string[] { "object_uuid" });
            _client.Insert(GetWriteConnectionString(), items, TableName, new string[] { "object_uuid", "timestamp" });

            return true;
        }


        public async Task<TrakHoundResponse<ITrakHoundObjectEventEntity>> Latest(IEnumerable<string> objectUuids)
        {
            Func<IEnumerable<string>, Task<IEnumerable<DatabaseObjectEvent>>> readFunction = async (ids) =>
            {
                var conditions = new List<string>();
                foreach (var objectUuid in objectUuids)
                {
                    conditions.Add($"[object_uuid] = '{objectUuid}'");
                }
                var condition = string.Join(" or ", conditions);

                var query = $"select {TableColumns} from [{EntityName}_latest] where {condition};";
                var dbEntities = _client.ReadList<DatabaseObjectEvent>(query);
                if (!dbEntities.IsNullOrEmpty())
                {
                    foreach (var dbEntity in dbEntities)
                    {
                        dbEntity.RequestedId = dbEntity.ObjectUuid;
                    }
                }

                return dbEntities;
            };

            return await ProcessResponse(this, objectUuids, readFunction, QueryType.Object);
        }

        public async Task<TrakHoundResponse<ITrakHoundObjectEventEntity>> Query(IEnumerable<TrakHoundRangeQuery> queries, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending)
        {
            Func<IEnumerable<string>, Task<IEnumerable<DatabaseObjectEvent>>> readFunction = async (ids) =>
            {
                var conditions = new List<string>();
                foreach (var query in queries)
                {
                    conditions.Add($"([object_uuid] = '{query.Target}' and [timestamp] >= {query.From} and [timestamp] < {query.To})");
                }
                var condition = string.Join(" or ", conditions);

                var order = sortOrder == SortOrder.Ascending ? "asc" : "desc";

                var dbQuery = $"select {TableColumns} from {TableName} where {condition} order by [timestamp] {order} limit {take} offset {skip};";
                var dbEntities = _client.ReadList<DatabaseObjectEvent>(GetReadConnectionString(), dbQuery);
                if (!dbEntities.IsNullOrEmpty())
                {
                    foreach (var dbEntity in dbEntities)
                    {
                        dbEntity.RequestedId = dbEntity.ObjectUuid;
                    }
                }

                return dbEntities;
            };

            return await ProcessResponse(this, queries.Select(o => o.Target).Distinct(), readFunction, QueryType.Object);
        }

        public async Task<TrakHoundResponse<ITrakHoundObjectEventEntity>> Query(IEnumerable<TrakHoundRangeQuery> queries, IEnumerable<TrakHoundConditionGroupQuery> conditionQueries, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending)
        {
            return TrakHoundResponse<ITrakHoundObjectEventEntity>.RouteNotConfigured(Id, null);
        }


        public async Task<TrakHoundResponse<ITrakHoundObjectEventEntity>> Last(IEnumerable<TrakHoundTimeQuery> queries)
        {
            return TrakHoundResponse<ITrakHoundObjectEventEntity>.RouteNotConfigured(Id, null);
        }

        public async Task<TrakHoundResponse<ITrakHoundObjectEventEntity>> Last(IEnumerable<TrakHoundTimeQuery> queries, IEnumerable<TrakHoundConditionGroupQuery> conditionQueries)
        {
            return TrakHoundResponse<ITrakHoundObjectEventEntity>.RouteNotConfigured(Id, null);
        }


        public async Task<TrakHoundResponse<TrakHoundCount>> Count(IEnumerable<TrakHoundRangeQuery> queries)
        {
            return TrakHoundResponse<TrakHoundCount>.RouteNotConfigured(Id, null);
        }
    }
}
