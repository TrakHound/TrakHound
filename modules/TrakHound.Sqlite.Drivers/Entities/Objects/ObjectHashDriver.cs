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
    public class ObjectHashDriver : SqliteEntityDriver<ITrakHoundObjectHashEntity, DatabaseObjectHash>, IObjectHashQueryDriver
    {
        public ObjectHashDriver() { }

        public ObjectHashDriver(ITrakHoundDriverConfiguration configuration) : base(configuration) 
        {
            EntityName = "trakhound_objects_hash";
            TableColumnList = new List<string> {
                "[uuid]",
                "[object_uuid]",
                "[key]",
                "[value]",
                "[source_uuid]",
                "[created]"
            };
        }

        class PublishItem
        {
            public string Uuid { get; set; }
            public string ObjectUuid { get; set; }
            public string Key { get; set; }
            public string Value { get; set; }
            public string SourceUuid { get; set; }
            public long Created { get; set; }      
        }


        protected async override Task<bool> OnPublish(IEnumerable<ITrakHoundObjectHashEntity> entities)
        {
            var items = new List<PublishItem>();
            foreach (var entity in entities)
            {
                var item = new PublishItem();
                item.Uuid = entity.Uuid;
                item.ObjectUuid = entity.ObjectUuid;
                item.Key = entity.Key;
                item.Value = entity.Value;
                item.SourceUuid = entity.SourceUuid;
                item.Created = entity.Created;
                items.Add(item);
            }

            _client.Insert(items, TableName, new string[] { "object_uuid", "key" });

            return true;
        }


        public async Task<TrakHoundResponse<ITrakHoundObjectHashEntity>> QueryByObjectUuid(IEnumerable<string> objectUuids, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending)
        {
            Func<IEnumerable<string>, Task<IEnumerable<DatabaseObjectHash>>> readFunction = async (ids) =>
            {
                var conditions = new List<string>();
                foreach (var objectUuid in objectUuids)
                {
                    conditions.Add($"[object_uuid] = '{objectUuid}'");
                }
                var condition = string.Join(" or ", conditions);

                var query = $"select {TableColumns} from {TableName} where {condition};";
                var dbEntities = _client.ReadList<DatabaseObjectHash>(query);
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

        public async Task<TrakHoundResponse<ITrakHoundObjectHashEntity>> QueryByObjectUuid(IEnumerable<string> objectUuids, IEnumerable<string> keys)
        {
            Func<IEnumerable<string>, Task<IEnumerable<DatabaseObjectHash>>> readFunction = async (ids) =>
            {
                var conditions = new List<string>();
                foreach (var objectUuid in objectUuids)
                {
                    foreach (var key in keys)
                    {
                        conditions.Add($"[object_uuid] = '{objectUuid}' and [key] = '{key}'");
                    }
                }
                var condition = string.Join(" or ", conditions);

                var query = $"select {TableColumns} from {TableName} where {condition};";
                var dbEntities = _client.ReadList<DatabaseObjectHash>(query);
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
    }
}
