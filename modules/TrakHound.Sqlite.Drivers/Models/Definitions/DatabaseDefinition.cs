// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Entities;

namespace TrakHound.Sqlite.Drivers.Models
{
    public class DatabaseDefinition : IDatabaseEntity<ITrakHoundDefinitionEntity>
    {
        public string RequestedId { get; set; }

        public string Uuid { get; set; }

        public string Id { get; set; }

        public string Type { get; set; }

        public string ParentUuid { get; set; }

        public string SourceUuid { get; set; }

        public long Created { get; set; }


        public ITrakHoundDefinitionEntity ToEntity()
        {
            return new TrakHoundDefinitionEntity
            {
                Id = Id,
                ParentUuid = ParentUuid,
                SourceUuid = SourceUuid,
                Created = Created
            };
        }
    }
}
