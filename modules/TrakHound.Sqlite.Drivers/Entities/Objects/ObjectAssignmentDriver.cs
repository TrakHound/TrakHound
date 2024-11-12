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
    public class ObjectAssignmentDriver : 
        SqliteEntityDriver<ITrakHoundObjectAssignmentEntity, DatabaseObjectAssignment>, 
        IObjectAssignmentCurrentDriver,
        IObjectAssignmentQueryDriver
    {
        public const string CurrentTableName = "[trakhound_objects_assignment_current]";


        public IEnumerable<string> CurrentTableColumnList;
        public string CurrentTableColumns => string.Join(", ", CurrentTableColumnList);


        public ObjectAssignmentDriver() { }

        public ObjectAssignmentDriver(ITrakHoundDriverConfiguration configuration) : base(configuration) 
        {
            EntityName = "trakhound_objects_assignment";
            TableColumnList = new List<string> {
                "[uuid]",
                "[assignee_uuid]",
                "[member_uuid]",
                "[add_timestamp]",
                "[add_source_uuid]",
                "[remove_timestamp]",
                "[remove_source_uuid]",
                "[created]"
            };

            CurrentTableColumnList = new List<string> {
                "[uuid]",
                "[assignee_uuid]",
                "[member_uuid]",
                "[add_source_uuid]",
                "[add_timestamp]",
                "[created]"
            };
        }

        class CurrentPublishItem
        {
            public string Uuid { get; set; }
            public string AssigneeUuid { get; set; }
            public string MemberUuid { get; set; }
            public long AddTimestamp { get; set; }
            public string AddSourceUuid { get; set; }
            public long Created { get; set; }
        }

        class PublishItem
        {
            public string Uuid { get; set; }
            public string AssigneeUuid { get; set; }
            public string MemberUuid { get; set; }
            public long AddTimestamp { get; set; }             
            public string AddSourceUuid { get; set; }
            public long RemoveTimestamp { get; set; }
            public string RemoveSourceUuid { get; set; }
            public long Created { get; set; }      
        }


        protected async override Task<bool> OnPublish(IEnumerable<ITrakHoundObjectAssignmentEntity> entities)
        {
            var addEntities = entities.Where(o => o.RemoveTimestamp < 1);
            var removeEntities = entities.Where(o => o.RemoveTimestamp > 1);

            // Add Current
            if (!addEntities.IsNullOrEmpty())
            {
                var currentItems = new List<CurrentPublishItem>();
                foreach (var entity in addEntities)
                {
                    var item = new CurrentPublishItem();
                    item.Uuid = entity.Uuid;
                    item.AssigneeUuid = entity.AssigneeUuid;
                    item.MemberUuid = entity.MemberUuid;
                    item.AddTimestamp = entity.AddTimestamp;
                    item.AddSourceUuid = entity.AddSourceUuid;
                    item.Created = entity.Created;
                    currentItems.Add(item);
                }
                _client.Insert(currentItems, CurrentTableName, new string[] { "uuid" });
            }

            // Remove Current
            if (!removeEntities.IsNullOrEmpty())
            {
                var conditions = new List<string>();
                foreach (var entity in removeEntities)
                {
                    conditions.Add($"[uuid] = '{entity.Uuid}'");
                }
                var condition = string.Join(" or ", conditions);

                var query = $"delete from {CurrentTableName} where {condition};";
                _client.ExecuteNonQuery(query);
            }

            var items = new List<PublishItem>();
            foreach (var entity in entities)
            {
                var item = new PublishItem();
                item.Uuid = entity.Uuid;
                item.AssigneeUuid = entity.AssigneeUuid;
                item.MemberUuid = entity.MemberUuid;
                item.AddTimestamp = entity.AddTimestamp;
                item.AddSourceUuid = entity.AddSourceUuid;
                item.RemoveTimestamp = entity.RemoveTimestamp;
                item.RemoveSourceUuid = entity.RemoveSourceUuid;
                item.Created = entity.Created;
                items.Add(item);
            }
            _client.Insert(items, TableName, new string[] { "uuid" });

            return true;
        }


        public async Task<TrakHoundResponse<ITrakHoundObjectAssignmentEntity>> CurrentByAssignee(IEnumerable<string> assigneeUuids)
        {
            Func<IEnumerable<string>, Task<IEnumerable<DatabaseObjectAssignment>>> readFunction = async (ids) =>
            {
                var conditions = new List<string>();
                foreach (var objectUuid in assigneeUuids)
                {
                    conditions.Add($"[assignee_uuid] = '{objectUuid}'");
                }
                var condition = string.Join(" or ", conditions);

                var query = $"select {CurrentTableColumns} from {CurrentTableName} where {condition};";
                var dbEntities = _client.ReadList<DatabaseObjectAssignment>(query);
                if (!dbEntities.IsNullOrEmpty())
                {
                    foreach (var dbEntity in dbEntities)
                    {
                        dbEntity.RequestedId = dbEntity.AssigneeUuid;
                    }
                }

                return dbEntities;
            };

            return await ProcessResponse(this, assigneeUuids, readFunction, QueryType.Object);
        }

        public async Task<TrakHoundResponse<ITrakHoundObjectAssignmentEntity>> CurrentByMember(IEnumerable<string> memberUuids)
        {
            Func<IEnumerable<string>, Task<IEnumerable<DatabaseObjectAssignment>>> readFunction = async (ids) =>
            {
                var conditions = new List<string>();
                foreach (var objectUuid in memberUuids)
                {
                    conditions.Add($"[member_uuid] = '{objectUuid}'");
                }
                var condition = string.Join(" or ", conditions);

                var query = $"select {CurrentTableColumns} from {CurrentTableName} where {condition};";
                var dbEntities = _client.ReadList<DatabaseObjectAssignment>(query);
                if (!dbEntities.IsNullOrEmpty())
                {
                    foreach (var dbEntity in dbEntities)
                    {
                        dbEntity.RequestedId = dbEntity.MemberUuid;
                    }
                }

                return dbEntities;
            };

            return await ProcessResponse(this, memberUuids, readFunction, QueryType.Object);
        }


        public async Task<TrakHoundResponse<ITrakHoundObjectAssignmentEntity>> QueryByAssignee(IEnumerable<TrakHoundRangeQuery> queries, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending)
        {
            Func<IEnumerable<string>, Task<IEnumerable<DatabaseObjectAssignment>>> readFunction = async (ids) =>
            {
                var sqlQuery = "";

                // Add Queries
                sqlQuery += "drop table if exists _queries;";
                sqlQuery += "create temp table _queries([target] text, [from] integer, [to] integer);";
                foreach (var query in queries)
                {
                    sqlQuery += $"insert into _queries ([target], [from], [to]) values ('{query.Target}', {query.From}, {query.To});";
                }

                sqlQuery += $"select {TableColumns} from {TableName}";
                sqlQuery += " inner join _queries on [assignee_uuid] = [target] and ";
                sqlQuery += " (([add_timestamp] >= [from] and [add_timestamp] < [to] and [remove_timestamp] < 1) or ";
                sqlQuery += " ([add_timestamp] <= [from] and [from] = [to] and [remove_timestamp] < 1) or ";
                sqlQuery += " ([add_timestamp] >= [from] and [add_timestamp] < [to]) or ";
                sqlQuery += " ([add_timestamp] <= [from] and [remove_timestamp] >= [to]) or ";
                sqlQuery += " ([add_timestamp] >= [from] and [remove_timestamp] > [from] and [remove_timestamp] < [to]))";

                var order = sortOrder == SortOrder.Ascending ? "asc" : "desc";

                sqlQuery = $"{sqlQuery} order by [add_timestamp] {order} limit {take} offset {skip};";

                var dbEntities = _client.ReadList<DatabaseObjectAssignment>(sqlQuery);
                if (!dbEntities.IsNullOrEmpty())
                {
                    foreach (var dbEntity in dbEntities)
                    {
                        dbEntity.RequestedId = dbEntity.AssigneeUuid;
                    }
                }

                return dbEntities;
            };

            return await ProcessResponse(this, queries.Select(o => o.Target).Distinct(), readFunction, QueryType.Object);
        }

        public async Task<TrakHoundResponse<ITrakHoundObjectAssignmentEntity>> QueryByMember(IEnumerable<TrakHoundRangeQuery> queries, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending)
        {
            Func<IEnumerable<string>, Task<IEnumerable<DatabaseObjectAssignment>>> readFunction = async (ids) =>
            {
                var sqlQuery = "";

                // Add Queries
                sqlQuery += "drop table if exists _queries;";
                sqlQuery += "create temp table _queries([target] text, [from] integer, [to] integer);";
                foreach (var query in queries)
                {
                    sqlQuery += $"insert into _queries ([target], [from], [to]) values ('{query.Target}', {query.From}, {query.To});";
                }

                sqlQuery += $"select {TableColumns} from {TableName}";
                sqlQuery += " inner join _queries on [member_uuid] = [target] and ";
                sqlQuery += " (([add_timestamp] >= [from] and [add_timestamp] < [to] and [remove_timestamp] < 1) or ";
                sqlQuery += " ([add_timestamp] <= [from] and [from] = [to] and [remove_timestamp] < 1) or ";
                sqlQuery += " ([add_timestamp] >= [from] and [add_timestamp] < [to]) or ";
                sqlQuery += " ([add_timestamp] <= [from] and [remove_timestamp] >= [to]) or ";
                sqlQuery += " ([add_timestamp] >= [from] and [remove_timestamp] > [from] and [remove_timestamp] < [to]))";

                var order = sortOrder == SortOrder.Ascending ? "asc" : "desc";

                sqlQuery = $"{sqlQuery} order by [add_timestamp] {order} limit {take} offset {skip};";

                var dbEntities = _client.ReadList<DatabaseObjectAssignment>(sqlQuery);
                if (!dbEntities.IsNullOrEmpty())
                {
                    foreach (var dbEntity in dbEntities)
                    {
                        dbEntity.RequestedId = dbEntity.MemberUuid;
                    }
                }

                return dbEntities;
            };

            return await ProcessResponse(this, queries.Select(o => o.Target).Distinct(), readFunction, QueryType.Object);
        }


        public async Task<TrakHoundResponse<TrakHoundCount>> CountByAssignee(IEnumerable<TrakHoundRangeQuery> queries)
        {
            return TrakHoundResponse<TrakHoundCount>.RouteNotConfigured(Id, null);
        }

        public async Task<TrakHoundResponse<TrakHoundCount>> CountByMember(IEnumerable<TrakHoundRangeQuery> queries)
        {
            return TrakHoundResponse<TrakHoundCount>.RouteNotConfigured(Id, null);
        }
    }
}
