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
    public class ObjectGroupDriver : SqliteEntityDriver<ITrakHoundObjectGroupEntity, DatabaseObjectGroup>, IObjectGroupQueryDriver
    {
        public ObjectGroupDriver() { }

        public ObjectGroupDriver(ITrakHoundDriverConfiguration configuration) : base(configuration) 
        {
            EntityName = "trakhound_objects_group";
            TableColumnList = new List<string> {
                "[uuid]",
                "[group_uuid]",
                "[member_uuid]",
                "[source_uuid]",
                "[created]"
            };
        }

        class PublishItem
        {
            public string Uuid { get; set; }
            public string GroupUuid { get; set; }
            public string MemberUuid { get; set; }
            public string SourceUuid { get; set; }
            public long Created { get; set; }      
        }


        protected async override Task<bool> OnPublish(IEnumerable<ITrakHoundObjectGroupEntity> entities)
        {
            var items = new List<PublishItem>();
            foreach (var entity in entities)
            {
                var item = new PublishItem();
                item.Uuid = entity.Uuid;
                item.GroupUuid = entity.GroupUuid;
                item.MemberUuid = entity.MemberUuid;
                item.SourceUuid = entity.SourceUuid;
                item.Created = entity.Created;
                items.Add(item);
            }

            _client.Insert(GetWriteConnectionString(), items, TableName, new string[] { "uuid" });

            return true;
        }


        public async Task<TrakHoundResponse<ITrakHoundObjectGroupEntity>> QueryByGroup(IEnumerable<string> objectUuids)
        {
            Func<IEnumerable<string>, Task<IEnumerable<DatabaseObjectGroup>>> readFunction = async (ids) =>
            {
                var conditions = new List<string>();
                foreach (var objectUuid in objectUuids)
                {
                    conditions.Add($"[group_uuid] = '{objectUuid}'");
                }
                var condition = string.Join(" or ", conditions);

                var query = $"select {TableColumns} from {TableName} where {condition};";
                var dbEntities = _client.ReadList<DatabaseObjectGroup>(GetReadConnectionString(), query);
                if (!dbEntities.IsNullOrEmpty())
                {
                    foreach (var dbEntity in dbEntities)
                    {
                        dbEntity.RequestedId = dbEntity.GroupUuid;
                    }
                }

                return dbEntities;
            };

            return await ProcessResponse(this, objectUuids, readFunction, QueryType.Object);
        }

        public async Task<TrakHoundResponse<ITrakHoundObjectGroupEntity>> QueryByMember(IEnumerable<string> objectUuids)
        {
            Func<IEnumerable<string>, Task<IEnumerable<DatabaseObjectGroup>>> readFunction = async (ids) =>
            {
                var conditions = new List<string>();
                foreach (var objectUuid in objectUuids)
                {
                    conditions.Add($"[member_uuid] = '{objectUuid}'");
                }
                var condition = string.Join(" or ", conditions);

                var query = $"select {TableColumns} from {TableName} where {condition};";
                var dbEntities = _client.ReadList<DatabaseObjectGroup>(GetReadConnectionString(), query);
                if (!dbEntities.IsNullOrEmpty())
                {
                    foreach (var dbEntity in dbEntities)
                    {
                        dbEntity.RequestedId = dbEntity.MemberUuid;
                    }
                }

                return dbEntities;
            };

            return await ProcessResponse(this, objectUuids, readFunction, QueryType.Object);
        }
    }
}
