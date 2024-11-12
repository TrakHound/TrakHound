// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Entities;

namespace TrakHound.Sqlite.Drivers.Models
{
    public class DatabaseObjectMetadata : IDatabaseEntity<ITrakHoundObjectMetadataEntity>
    {
        public string RequestedId { get; set; }

        public string Uuid { get; set; }

        public string EntityUuid { get; set; }

        public string Name { get; set; }

        public string DefinitionUuid { get; set; }

        public string Value { get; set; }

        public string ValueDefinitionUuid { get; set; }

        public string SourceUuid { get; set; }

        public long Created { get; set; }


        public ITrakHoundObjectMetadataEntity ToEntity()
        {
            return new TrakHoundObjectMetadataEntity
            {
                EntityUuid = EntityUuid,
                Name = Name,
                DefinitionUuid = DefinitionUuid,
                Value = Value,
                ValueDefinitionUuid = ValueDefinitionUuid,
                SourceUuid = SourceUuid,
                Created = Created
            };
        }
    }
}
