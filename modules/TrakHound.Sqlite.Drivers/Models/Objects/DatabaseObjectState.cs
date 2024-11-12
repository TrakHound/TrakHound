// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Entities;

namespace TrakHound.Sqlite.Drivers.Models
{
    public class DatabaseObjectState : IDatabaseEntity<ITrakHoundObjectStateEntity>
    {
        public string RequestedId { get; set; }

        public string Uuid { get; set; }

        public string ObjectUuid { get; set; }

        public string DefinitionUuid { get; set; }

        public int TTL { get; set; }

        public string SourceUuid { get; set; }

        public long Timestamp { get; set; }


        public ITrakHoundObjectStateEntity ToEntity()
        {
            return new TrakHoundObjectStateEntity
            {
                ObjectUuid = ObjectUuid,
                DefinitionUuid = DefinitionUuid,
                TTL = TTL,
                SourceUuid = SourceUuid,
                Timestamp = Timestamp
            };
        }
    }
}
