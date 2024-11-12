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
    public class ObjectMetadataDriver : SqliteEntityDriver<ITrakHoundObjectMetadataEntity, DatabaseObjectMetadata>, IObjectMetadataQueryDriver
    {
        public ObjectMetadataDriver() { }

        public ObjectMetadataDriver(ITrakHoundDriverConfiguration configuration) : base(configuration) 
        {
            EntityName = "trakhound_objects_metadata";
            TableColumnList = new List<string> {
                "[uuid]",
                "[entity_uuid]",
                "[name]",
                "[definition_uuid]",
                "[value]",
                "[value_definition_uuid]",
                "[source_uuid]",
                "[created]"
            };
        }

        class PublishItem
        {
            public string Uuid { get; set; }
            public string EntityUuid { get; set; }
            public string Name { get; set; }
            public string DefinitionUuid { get; set; }
            public string Value { get; set; }
            public string ValueDefinitionUuid { get; set; }
            public string SourceUuid { get; set; }
            public long Created { get; set; }      
        }


        protected async override Task<bool> OnPublish(IEnumerable<ITrakHoundObjectMetadataEntity> entities)
        {
            var items = new List<PublishItem>();
            foreach (var entity in entities)
            {
                var item = new PublishItem();
                item.Uuid = entity.Uuid;
                item.EntityUuid = entity.EntityUuid;
                item.Name = entity.Name;
                item.DefinitionUuid = entity.DefinitionUuid;
                item.Value = entity.Value;
                item.ValueDefinitionUuid = entity.ValueDefinitionUuid;
                item.SourceUuid = entity.SourceUuid;
                item.Created = entity.Created;
                items.Add(item);
            }

            _client.Insert(items, TableName, new string[] { "uuid" });

            return true;
        }


        public async Task<TrakHoundResponse<ITrakHoundObjectMetadataEntity>> Query(IEnumerable<string> entityUuids)
        {
            Func<IEnumerable<string>, Task<IEnumerable<DatabaseObjectMetadata>>> readFunction = async (ids) =>
            {
                var conditions = new List<string>();
                foreach (var entityUuid in entityUuids)
                {
                    conditions.Add($"[entity_uuid] = '{entityUuid}'");
                }
                var condition = string.Join(" or ", conditions);

                var sqlQuery = $"select {TableColumns} from {TableName} where {condition};";
                var dbEntities = _client.ReadList<DatabaseObjectMetadata>(sqlQuery);
                if (!dbEntities.IsNullOrEmpty())
                {
                    foreach (var dbEntity in dbEntities)
                    {
                        dbEntity.RequestedId = dbEntity.EntityUuid;
                    }
                }

                return dbEntities;
            };

            return await ProcessResponse(this, entityUuids, readFunction, QueryType.Object);
        }

        public async Task<TrakHoundResponse<ITrakHoundObjectMetadataEntity>> Query(IEnumerable<string> entityUuids, string name, TrakHoundMetadataQueryType queryType, string query)
        {
            Func<IEnumerable<string>, Task<IEnumerable<DatabaseObjectMetadata>>> readFunction = async (ids) =>
            {
                var conditions = new List<string>();
                foreach (var entityUuid in entityUuids)
                {
                    conditions.Add($"[entity_uuid] = '{entityUuid}'");
                }
                var condition = string.Join(" or ", conditions);

                string valueCondition = null;
                switch (queryType)
                {
                    case TrakHoundMetadataQueryType.Equal: valueCondition = $"lower([value]) = '{query?.ToLower()}'"; break;
                    case TrakHoundMetadataQueryType.Like: valueCondition = $"lower([value]) like '{query?.ToLower()}%'"; break;
                }

                var sqlQuery = $"select {TableColumns} from {TableName} where ({condition}) and lower([name]) = '{name?.ToLower()}' and {valueCondition};";
                var dbEntities = _client.ReadList<DatabaseObjectMetadata>(sqlQuery);
                if (!dbEntities.IsNullOrEmpty())
                {
                    foreach (var dbEntity in dbEntities)
                    {
                        dbEntity.RequestedId = dbEntity.EntityUuid;
                    }
                }

                return dbEntities;
            };

            return await ProcessResponse(this, entityUuids, readFunction, QueryType.Object);
        }

        public async Task<TrakHoundResponse<ITrakHoundObjectMetadataEntity>> Query(string name, TrakHoundMetadataQueryType queryType, string query)
        {
            Func<IEnumerable<string>, Task<IEnumerable<DatabaseObjectMetadata>>> readFunction = async (ids) =>
            {
                string valueCondition = null;
                switch (queryType)
                {
                    case TrakHoundMetadataQueryType.Equal: valueCondition = $"lower([value]) = '{query?.ToLower()}'"; break;
                    case TrakHoundMetadataQueryType.Like: valueCondition = $"lower([value]) like '{query?.ToLower()}%'"; break;
                }

                var sqlQuery = $"select {TableColumns} from {TableName} where lower([name]) = '{name?.ToLower()}' and {valueCondition};";
                var dbEntities = _client.ReadList<DatabaseObjectMetadata>(sqlQuery);
                if (!dbEntities.IsNullOrEmpty())
                {
                    foreach (var dbEntity in dbEntities)
                    {
                        dbEntity.RequestedId = name;
                    }
                }

                return dbEntities;
            };

            return await ProcessResponse(this, name, readFunction, QueryType.Object);
        }
    }
}
