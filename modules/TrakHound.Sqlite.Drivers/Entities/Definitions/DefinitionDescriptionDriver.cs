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
    public class DefinitionDescriptionDriver : SqliteEntityDriver<ITrakHoundDefinitionDescriptionEntity, DatabaseDefinitionDescription>, IDefinitionDescriptionQueryDriver
    {
        public DefinitionDescriptionDriver() { }

        public DefinitionDescriptionDriver(ITrakHoundDriverConfiguration configuration) : base(configuration) 
        {
            EntityName = "trakhound_definitions_description";
            TableColumnList = new List<string> {
                "[uuid]",
                "[definition_uuid]",
                "[language_code]",
                "[text]",
                "[source_uuid]",
                "[created]"
            };
        }

        class PublishItem
        {
            public string Uuid { get; set; }
            public string DefinitionUuid { get; set; }
            public string LanguageCode { get; set; }
            public string Text { get; set; }
            public string SourceUuid { get; set; }
            public long Created { get; set; }      
        }


        protected async override Task<bool> OnPublish(IEnumerable<ITrakHoundDefinitionDescriptionEntity> entities)
        {
            var items = new List<PublishItem>();
            foreach (var entity in entities)
            {
                var item = new PublishItem();
                item.Uuid = entity.Uuid;
                item.DefinitionUuid = entity.DefinitionUuid;
                item.LanguageCode = entity.LanguageCode;
                item.Text = entity.Text;
                item.Created = entity.Created;
                item.SourceUuid = entity.SourceUuid;
                items.Add(item);
            }

            _client.Insert(GetWriteConnectionString(), items, TableName, new string[] { "uuid" });

            return true;
        }


        public async Task<TrakHoundResponse<ITrakHoundDefinitionDescriptionEntity>> Query(IEnumerable<string> entityUuids)
        {
            Func<IEnumerable<string>, Task<IEnumerable<DatabaseDefinitionDescription>>> readFunction = async (ids) =>
            {
                var conditions = new List<string>();
                foreach (var entityUuid in entityUuids)
                {
                    conditions.Add($"[definition_uuid] = '{entityUuid}'");
                }
                var condition = string.Join(" or ", conditions);

                var query = $"select {TableColumns} from {TableName} where {condition};";
                var dbEntities = _client.ReadList<DatabaseDefinitionDescription>(GetReadConnectionString(), query);
                if (!dbEntities.IsNullOrEmpty())
                {
                    foreach (var dbEntity in dbEntities)
                    {
                        dbEntity.RequestedId = dbEntity.DefinitionUuid;
                    }
                }

                return dbEntities;
            };

            return await ProcessResponse(this, entityUuids, readFunction, QueryType.Object);
        }
    }
}
