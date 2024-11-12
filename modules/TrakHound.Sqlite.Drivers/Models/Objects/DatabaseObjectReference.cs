// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Entities;

namespace TrakHound.Sqlite.Drivers.Models
{
    public class DatabaseObjectReference : IDatabaseEntity<ITrakHoundObjectReferenceEntity>
    {
        public string RequestedId { get; set; }

        public string Uuid { get; set; }

        public string ObjectUuid { get; set; }

        public string TargetUuid { get; set; }

        public string SourceUuid { get; set; }

        public long Created { get; set; }


        public ITrakHoundObjectReferenceEntity ToEntity()
        {
            return new TrakHoundObjectReferenceEntity
            {
                ObjectUuid = ObjectUuid,
                TargetUuid = TargetUuid,
                SourceUuid = SourceUuid,
                Created = Created
            };
        }
    }
}
