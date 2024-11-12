// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Entities;

namespace TrakHound.Sqlite.Drivers.Models
{
    public class DatabaseObjectAssignment : IDatabaseEntity<ITrakHoundObjectAssignmentEntity>
    {
        public string RequestedId { get; set; }

        public string Uuid { get; set; }

        public string AssigneeUuid { get; set; }

        public string MemberUuid { get; set; }

        public string AddSourceUuid { get; set; }

        public long AddTimestamp { get; set; }

        public string RemoveSourceUuid { get; set; }

        public long RemoveTimestamp { get; set; }


        public ITrakHoundObjectAssignmentEntity ToEntity()
        {
            return new TrakHoundObjectAssignmentEntity
            {
                AssigneeUuid = AssigneeUuid,
                MemberUuid = MemberUuid,
                AddSourceUuid = AddSourceUuid,
                AddTimestamp = AddTimestamp,
                RemoveSourceUuid = RemoveSourceUuid,
                RemoveTimestamp = RemoveTimestamp
            };
        }
    }
}
