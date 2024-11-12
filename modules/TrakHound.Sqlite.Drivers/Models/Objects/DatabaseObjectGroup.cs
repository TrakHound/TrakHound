// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Entities;

namespace TrakHound.Sqlite.Drivers.Models
{
    public class DatabaseObjectGroup : IDatabaseEntity<ITrakHoundObjectGroupEntity>
    {
        public string RequestedId { get; set; }

        public string Uuid { get; set; }

        public string GroupUuid { get; set; }

        public string MemberUuid { get; set; }

        public string SourceUuid { get; set; }

        public long Created { get; set; }


        public ITrakHoundObjectGroupEntity ToEntity()
        {
            return new TrakHoundObjectGroupEntity(GroupUuid, MemberUuid, SourceUuid, Created);
        }
    }
}
