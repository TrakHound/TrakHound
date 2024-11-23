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
    public class SourceMetadataDriver : SqliteEntityDriver<ITrakHoundSourceMetadataEntity, DatabaseSourceMetadata>, ISourceMetadataQueryDriver
    {
        public SourceMetadataDriver() { }

        public SourceMetadataDriver(ITrakHoundDriverConfiguration configuration) : base(configuration) 
        {
            EntityName = "trakhound_sources_metadata";
            TableColumnList = new List<string> {
                "[uuid]",
                "[source_uuid]",
                "[name]",
                "[value]",
                "[created]"
            };
        }

        class PublishItem
        {
            public string Uuid { get; set; }
            public string SourceUuid { get; set; }
            public string Name { get; set; }
            public string Value { get; set; }
            public long Created { get; set; }      
        }


        protected async override Task<bool> OnPublish(IEnumerable<ITrakHoundSourceMetadataEntity> entities)
        {
            var items = new List<PublishItem>();
            foreach (var entity in entities)
            {
                var item = new PublishItem();
                item.Uuid = entity.Uuid;
                item.SourceUuid = entity.SourceUuid;
                item.Name = entity.Name;
                item.Value = entity.Value;
                item.Created = entity.Created;
                items.Add(item);
            }

            _client.Insert(GetWriteConnectionString(), items, TableName, new string[] { "uuid" });

            return true;
        }


        public async Task<TrakHoundResponse<ITrakHoundSourceMetadataEntity>> Query(IEnumerable<string> entityUuids)
        {
            Func<IEnumerable<string>, Task<IEnumerable<DatabaseSourceMetadata>>> readFunction = async (ids) =>
            {
                var conditions = new List<string>();
                foreach (var entityUuid in entityUuids)
                {
                    conditions.Add($"[source_uuid] = '{entityUuid}'");
                }
                var condition = string.Join(" or ", conditions);

                var query = $"select {TableColumns} from {TableName} where {condition};";
                var dbEntities = _client.ReadList<DatabaseSourceMetadata>(GetReadConnectionString(), query);
                if (!dbEntities.IsNullOrEmpty())
                {
                    foreach (var dbEntity in dbEntities)
                    {
                        dbEntity.RequestedId = dbEntity.SourceUuid;
                    }
                }

                return dbEntities;
            };

            return await ProcessResponse(this, entityUuids, readFunction, QueryType.Object);
        }
    }
}
