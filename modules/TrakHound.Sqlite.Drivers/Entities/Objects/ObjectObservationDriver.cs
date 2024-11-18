// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Drivers;
using TrakHound.Drivers.Entities;
using TrakHound.Entities;
using TrakHound.Sqlite.Drivers.Models;

namespace TrakHound.Sqlite.Drivers
{
    public class ObjectObservationDriver : 
        SqliteEntityDriver<ITrakHoundObjectObservationEntity, DatabaseObjectObservation>, 
        IObjectObservationLatestDriver
    {
        public ObjectObservationDriver() { }

        public ObjectObservationDriver(ITrakHoundDriverConfiguration configuration) : base(configuration) 
        {
            EntityName = "trakhound_objects_observation_latest";
            TableColumnList = new List<string> {
                "[object_uuid]",
                "[uuid]",
                "[data_type]",
                "[value]",
                "[batch_id]",
                "[sequence]",
                "[timestamp]",
                "[source_uuid]",
                "[created]"
            };
        }

        class PublishItem
        {
            public string ObjectUuid { get; set; }
            public string Uuid { get; set; }
            public int DataType { get; set; }
            public string Value { get; set; }
            public ulong BatchId { get; set; }             
            public ulong Sequence { get; set; }             
            public long Timestamp { get; set; }             
            public string SourceUuid { get; set; }
            public long Created { get; set; }      
        }


        protected async override Task<bool> OnPublish(IEnumerable<ITrakHoundObjectObservationEntity> entities)
        {
            var items = new List<PublishItem>();
            foreach (var entity in entities)
            {
                var item = new PublishItem();
                item.Uuid = entity.Uuid;
                item.ObjectUuid = entity.ObjectUuid;
                item.DataType = entity.DataType;
                item.Value = entity.Value;
                item.BatchId = entity.BatchId;
                item.Sequence = entity.Sequence;
                item.Timestamp = entity.Timestamp;
                item.SourceUuid = entity.SourceUuid;
                item.Created = entity.Created;
                items.Add(item);
            }

            _client.Insert(items, TableName, new string[] { "object_uuid" });

            return true;
        }

        //protected async override Task<bool> OnPublish(IEnumerable<ITrakHoundObjectObservationEntity> entities)
        //{
        //    var items = new List<PublishItem>();
        //    foreach (var entity in entities)
        //    {
        //        var item = new PublishItem();
        //        item.Uuid = entity.Uuid;
        //        item.ObjectUuid = entity.ObjectUuid;
        //        item.DataType = entity.DataType;
        //        item.Value = entity.Value;
        //        item.BatchId = entity.BatchId;
        //        item.Sequence = entity.Sequence;
        //        item.Timestamp = entity.Timestamp;
        //        item.SourceUuid = entity.SourceUuid;
        //        item.Created = entity.Created;
        //        items.Add(item);
        //    }

        //    _client.Insert(items, $"[{EntityName}_latest]", new string[] { "object_uuid" });
        //    _client.Insert(items, TableName, new string[] { "object_uuid", "timestamp" });

        //    return true;
        //}


        public async Task<TrakHoundResponse<ITrakHoundObjectObservationEntity>> Latest(IEnumerable<string> objectUuids)
        {
            Func<IEnumerable<string>, Task<IEnumerable<DatabaseObjectObservation>>> readFunction = async (ids) =>
            {
                var conditions = new List<string>();
                foreach (var objectUuid in objectUuids)
                {
                    conditions.Add($"[object_uuid] = '{objectUuid}'");
                }
                var condition = string.Join(" or ", conditions);

                var query = $"select {TableColumns} from [{EntityName}_latest] where {condition};";
                var dbEntities = _client.ReadList<DatabaseObjectObservation>(query);
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

        //public async Task<TrakHoundResponse<ITrakHoundObjectObservationEntity>> Query(IEnumerable<TrakHoundRangeQuery> queries, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending)
        //{
        //    Func<IEnumerable<string>, Task<IEnumerable<DatabaseObjectObservation>>> readFunction = async (ids) =>
        //    {
        //        var conditions = new List<string>();
        //        foreach (var query in queries)
        //        {
        //            conditions.Add($"([object_uuid] = '{query.Target}' and [timestamp] >= {query.From} and [timestamp] < {query.To})");
        //        }
        //        var condition = string.Join(" or ", conditions);

        //        var order = sortOrder == SortOrder.Ascending ? "asc" : "desc";

        //        var dbQuery = $"select {TableColumns} from {TableName} where {condition} order by [timestamp] {order} limit {take} offset {skip};";
        //        var dbEntities = _client.ReadList<DatabaseObjectObservation>(dbQuery);
        //        if (!dbEntities.IsNullOrEmpty())
        //        {
        //            foreach (var dbEntity in dbEntities)
        //            {
        //                dbEntity.RequestedId = dbEntity.ObjectUuid;
        //            }
        //        }

        //        return dbEntities;
        //    };

        //    return await ProcessResponse(this, queries.Select(o => o.Target).Distinct(), readFunction, QueryType.Object);
        //}

        //public async Task<TrakHoundResponse<ITrakHoundObjectObservationEntity>> Query(IEnumerable<TrakHoundRangeQuery> queries, IEnumerable<TrakHoundConditionGroupQuery> conditionQueries, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending)
        //{
        //    return TrakHoundResponse<ITrakHoundObjectObservationEntity>.RouteNotConfigured(Id, null);
        //}


        //public async Task<TrakHoundResponse<ITrakHoundObjectObservationEntity>> Last(IEnumerable<TrakHoundTimeQuery> queries)
        //{
        //    return TrakHoundResponse<ITrakHoundObjectObservationEntity>.RouteNotConfigured(Id, null);
        //}

        //public async Task<TrakHoundResponse<ITrakHoundObjectObservationEntity>> Last(IEnumerable<TrakHoundTimeQuery> queries, IEnumerable<TrakHoundConditionGroupQuery> conditionQueries)
        //{
        //    return TrakHoundResponse<ITrakHoundObjectObservationEntity>.RouteNotConfigured(Id, null);
        //}


        //public async Task<TrakHoundResponse<TrakHoundCount>> Count(IEnumerable<TrakHoundRangeQuery> queries)
        //{
        //    return TrakHoundResponse<TrakHoundCount>.RouteNotConfigured(Id, null);
        //}
    }
}
