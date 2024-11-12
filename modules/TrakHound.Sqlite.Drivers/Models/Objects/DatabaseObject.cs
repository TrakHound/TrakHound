// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Entities;

namespace TrakHound.Sqlite.Drivers.Models
{
    public class DatabaseObject : IDatabaseEntity<ITrakHoundObjectEntity>
    {
        public string RequestedId { get; set; }

        public string Uuid { get; set; }

        public string Namespace { get; set; }

        public string Path { get; set; }

        public string Name { get; set; }

        public string ContentType { get; set; }

        public byte Priority { get; set; }

        public string DefinitionUuid { get; set; }

        public string SourceUuid { get; set; }

        public long Created { get; set; }


        public ITrakHoundObjectEntity ToEntity()
        {
            return new TrakHoundObjectEntity
            {
                Namespace = Namespace,
                Path = Path,
                ContentType = ContentType,
                Priority = Priority,
                DefinitionUuid = DefinitionUuid,
                SourceUuid = SourceUuid,
                Created = Created
            };
        }
    }
}
