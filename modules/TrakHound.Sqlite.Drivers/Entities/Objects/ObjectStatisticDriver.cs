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
    public class ObjectStatisticDriver : 
        SqliteEntityDriver<ITrakHoundObjectStatisticEntity, DatabaseObjectStatistic>, 
        IObjectStatisticQueryDriver
    {
        public ObjectStatisticDriver() { }

        public ObjectStatisticDriver(ITrakHoundDriverConfiguration configuration) : base(configuration) 
        {
            EntityName = "trakhound_objects_statistic";
            TableColumnList = new List<string> {
                "[uuid]",
                "[object_uuid]",
                "[time_range_start]",
                "[time_range_span]",
                "[data_type]",
                "[value]",
                "[timestamp]",
                "[source_uuid]",
                "[created]"
            };
        }

        class PublishItem
        {
            public string Uuid { get; set; }
            public string ObjectUuid { get; set; }
            public long TimeRangeStart { get; set; }
            public long TimeRangeSpan { get; set; }
            public int DataType { get; set; }
            public double Value { get; set; }
            public long Timestamp { get; set; }             
            public string SourceUuid { get; set; }
            public long Created { get; set; }      
        }

        protected async override Task<bool> OnPublish(IEnumerable<ITrakHoundObjectStatisticEntity> entities)
        {
            var items = new List<PublishItem>();
            foreach (var entity in entities)
            {
                var item = new PublishItem();
                item.Uuid = entity.Uuid;
                item.ObjectUuid = entity.ObjectUuid;
                item.TimeRangeStart = entity.TimeRangeStart;
                item.TimeRangeSpan = entity.TimeRangeEnd - entity.TimeRangeStart;
                item.DataType = entity.DataType;
                item.Value = entity.Value.ToDouble();
                item.Timestamp = entity.Timestamp;
                item.SourceUuid = entity.SourceUuid;
                item.Created = entity.Created;
                items.Add(item);
            }

            _client.Insert(GetWriteConnectionString(), items, TableName, new string[] { "object_uuid", "time_range_start", "time_range_span" });

            return true;
        }


        public async Task<TrakHoundResponse<ITrakHoundObjectStatisticEntity>> Query(IEnumerable<TrakHoundTimeRangeQuery> queries, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending)
        {
            Func<IEnumerable<string>, Task<IEnumerable<DatabaseObjectStatistic>>> readFunction = async (ids) =>
            {
                var sqlQuery = "";

                // Add Queries
                sqlQuery += "drop table if exists _queries;";
                sqlQuery += "create temp table _queries([target] text, [from] integer, [to] integer);";
                foreach (var query in queries)
                {
                    sqlQuery += $"insert into _queries ([target], [from], [to]) values ('{query.Target}', {query.From}, {query.To});";
                }

                sqlQuery += $"select {TableColumns} from {TableName} inner join _queries on [object_uuid] = [target] and [time_range_start] >= [from] and ([time_range_start] + [time_range_span]) <= [to]";

                var dbEntities = _client.ReadList<DatabaseObjectStatistic>(GetReadConnectionString(), sqlQuery);
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

        public async Task<TrakHoundResponse<TrakHoundTimeRangeSpan>> Spans(IEnumerable<TrakHoundRangeQuery> queries)
        {
            Func<IEnumerable<string>, Task<IEnumerable<DatabaseTimeRangeSpan>>> readFunction = async (ids) =>
            {
                var sqlQuery = "";

                // Add Queries
                sqlQuery += "drop table if exists _queries;";
                sqlQuery += "create temp table _queries([target] text, [from] integer, [to] integer);";
                foreach (var query in queries)
                {
                    sqlQuery += $"insert into _queries ([target], [from], [to]) values ('{query.Target}', {query.From}, {query.To});";
                }

                sqlQuery += $"select [object_uuid] as [requested_id], [time_range_span] as [span] from {TableName} inner join _queries on [object_uuid] = [target] and [time_range_start] >= [from] and ([time_range_start] + [time_range_span]) <= [to] group by [object_uuid], [span]";

                return _client.ReadList<DatabaseTimeRangeSpan>(GetReadConnectionString(), sqlQuery);
            };

            return await ProcessResponse(this, queries.Select(o => o.Target).Distinct(), readFunction);
        }

        public async Task<TrakHoundResponse<TrakHoundCount>> Count(IEnumerable<TrakHoundTimeRangeQuery> queries)
        {
            return TrakHoundResponse<TrakHoundCount>.RouteNotConfigured(Id, null);
        }
    }
}
