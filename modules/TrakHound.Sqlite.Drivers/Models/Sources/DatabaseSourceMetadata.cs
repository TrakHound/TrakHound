// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Entities;

namespace TrakHound.Sqlite.Drivers.Models
{
    public class DatabaseSourceMetadata : IDatabaseEntity<ITrakHoundSourceMetadataEntity>
    {
        public string RequestedId { get; set; }

        public string Uuid { get; set; }

        public string SourceUuid { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }

        public long Created { get; set; }


        public ITrakHoundSourceMetadataEntity ToEntity()
        {
            return new TrakHoundSourceMetadataEntity
            {
                SourceUuid = SourceUuid,
                Name = Name,
                Value = Value,
                Created = Created
            };
        }
    }
}
