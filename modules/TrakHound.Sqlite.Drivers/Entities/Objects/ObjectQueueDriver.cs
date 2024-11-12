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
    public class ObjectQueueDriver : SqliteEntityDriver<ITrakHoundObjectQueueEntity, DatabaseObjectQueue>,
        IObjectQueuePullDriver,
        IObjectQueueQueryDriver
    {
        public ObjectQueueDriver() { }

        public ObjectQueueDriver(ITrakHoundDriverConfiguration configuration) : base(configuration) 
        {
            EntityName = "trakhound_objects_queue";
            TableColumnList = new List<string> {
                "[queue_uuid]",
                "[index]",
                "[member_uuid]",
                "[timestamp]",
                "[source_uuid]",
                "[created]"
            };
        }

        class PublishItem
        {
            public string QueueUuid { get; set; }
            public long Index { get; set; }
            public string MemberUuid { get; set; }
            public long Timestamp { get; set; }
            public string SourceUuid { get; set; }
            public long Created { get; set; }      
        }


        protected async override Task<bool> OnPublish(IEnumerable<ITrakHoundObjectQueueEntity> entities)
        {
            var queueUuids = entities.Select(o => o.QueueUuid).Distinct();
            foreach (var queueUuid in queueUuids)
            {
                var publishEntities = new Dictionary<string, ITrakHoundObjectQueueEntity>();

                // Read Existing Entities
                var query = $"select {TableColumns} from {TableName} where [queue_uuid] = '{queueUuid}';";
                var existingEntities = _client.ReadList<DatabaseObjectQueue>(query);
                if (!existingEntities.IsNullOrEmpty())
                {
                    foreach (var entity in existingEntities)
                    {
                        var entityUuid = TrakHoundObjectQueueEntity.GenerateUuid(entity.QueueUuid, entity.MemberUuid);
                        publishEntities.Remove(entityUuid);
                        publishEntities.Add(entityUuid, entity.ToEntity());
                    }
                }

                // Set Last Index
                int lastIndex = 0;
                if (!existingEntities.IsNullOrEmpty()) lastIndex = existingEntities.Max(o => o.Index);

                // Set Indexes
                var queueEntities = entities.Where(o => o.QueueUuid == queueUuid).OrderBy(o => o.Index);
                foreach (var entity in queueEntities)
                {
                    if (entity.Index > 0)
                    {
                        foreach (var publishEntity in publishEntities.Values)
                        {
                            if (publishEntity.Index >= entity.Index) publishEntity.Index += 1;
                        }

                        publishEntities.Remove(entity.Uuid);
                        publishEntities.Add(entity.Uuid, entity);
                        lastIndex++;
                    }
                    else
                    {
                        entity.Index = lastIndex + 1;
                        lastIndex = entity.Index;
                        publishEntities.Remove(entity.Uuid);
                        publishEntities.Add(entity.Uuid, entity);
                    }
                }

                // Write Entities
                var items = new List<PublishItem>();
                foreach (var entity in publishEntities.Values)
                {
                    var item = new PublishItem();
                    item.QueueUuid = entity.QueueUuid;
                    item.Index = entity.Index;
                    item.MemberUuid = entity.MemberUuid;
                    item.Timestamp = entity.Timestamp;
                    item.SourceUuid = entity.SourceUuid;
                    item.Created = entity.Created;
                    items.Add(item);
                }

                _client.Insert(items, TableName, new string[] { "queue_uuid", "member_uuid" });
            }

            return true;
        }


        public async Task<TrakHoundResponse<ITrakHoundObjectQueueEntity>> Pull(string queueUuid, int count)
        {
            Func<IEnumerable<string>, Task<IEnumerable<DatabaseObjectQueue>>> readFunction = async (ids) =>
            {
                var query = $"select {TableColumns} from {TableName} where [queue_uuid] = '{queueUuid}' limit {count};";
                var pulledEntities = _client.ReadList<DatabaseObjectQueue>(query);
                if (!pulledEntities.IsNullOrEmpty())
                {
                    var lastPulledIndex = pulledEntities.Max(o => o.Index);

                    // Remove Pulled Entities
                    query = $"delete from {TableName} where [queue_uuid] = '{queueUuid}' and [index] <= {lastPulledIndex};";
                    _client.ExecuteNonQuery(query);

                    foreach (var entity in pulledEntities) entity.RequestedId = entity.QueueUuid;

                    var firstPulledIndex = pulledEntities.Min(o => o.Index);

                    query = $"select {TableColumns} from {TableName} where [queue_uuid] = '{queueUuid}' and [index] > {firstPulledIndex};";
                    var updateEntities = _client.ReadList<DatabaseObjectQueue>(query);
                    if (!updateEntities.IsNullOrEmpty())
                    {
                        // Update Index
                        var nextIndex = firstPulledIndex;
                        foreach (var entity in updateEntities.OrderBy(o => o.Index))
                        {
                            entity.Index = nextIndex;
                            nextIndex++;
                        }

                        // Write Entities
                        var items = new List<PublishItem>();
                        foreach (var entity in updateEntities)
                        {
                            var item = new PublishItem();
                            item.QueueUuid = entity.QueueUuid;
                            item.Index = entity.Index;
                            item.MemberUuid = entity.MemberUuid;
                            item.Timestamp = entity.Timestamp;
                            item.SourceUuid = entity.SourceUuid;
                            item.Created = entity.Created;
                            items.Add(item);
                        }

                        _client.Insert(items, TableName, new string[] { "queue_uuid", "member_uuid" });
                    }
                }

                return pulledEntities;
            };

            return await ProcessResponse(this, queueUuid, readFunction, QueryType.Object);
        }


        public async Task<TrakHoundResponse<ITrakHoundObjectQueueEntity>> QueryByQueue(IEnumerable<string> queueUuids, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending)
        {
            Func<IEnumerable<string>, Task<IEnumerable<DatabaseObjectQueue>>> readFunction = async (ids) =>
            {
                var conditions = new List<string>();
                foreach (var queueUuid in queueUuids)
                {
                    conditions.Add($"[queue_uuid] = '{queueUuid}'");
                }
                var condition = string.Join(" or ", conditions);

                var query = $"select {TableColumns} from {TableName} where {condition};";
                var dbEntities = _client.ReadList<DatabaseObjectQueue>(query);
                if (!dbEntities.IsNullOrEmpty())
                {
                    foreach (var dbEntity in dbEntities)
                    {
                        dbEntity.RequestedId = dbEntity.QueueUuid;
                    }
                }

                return dbEntities;
            };

            return await ProcessResponse(this, queueUuids, readFunction, QueryType.Object);
        }

        public async Task<TrakHoundResponse<ITrakHoundObjectQueueEntity>> QueryByMember(IEnumerable<string> memberUuids, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending)
        {
            Func<IEnumerable<string>, Task<IEnumerable<DatabaseObjectQueue>>> readFunction = async (ids) =>
            {
                var conditions = new List<string>();
                foreach (var memberUuid in memberUuids)
                {
                    conditions.Add($"[member_uuid] = '{memberUuid}'");
                }
                var condition = string.Join(" or ", conditions);

                var query = $"select {TableColumns} from {TableName} where {condition};";
                var dbEntities = _client.ReadList<DatabaseObjectQueue>(query);
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
    }
}
