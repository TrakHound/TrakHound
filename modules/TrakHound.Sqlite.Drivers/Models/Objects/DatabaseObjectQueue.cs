// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Entities;

namespace TrakHound.Sqlite.Drivers.Models
{
    public class DatabaseObjectQueue : IDatabaseEntity<ITrakHoundObjectQueueEntity>
    {
        public string RequestedId { get; set; }

        public string Uuid { get; set; }

        public string QueueUuid { get; set; }

        public int Index { get; set; }

        public string MemberUuid { get; set; }

        public long Timestamp { get; set; }

        public string SourceUuid { get; set; }

        public long Created { get; set; }


        public ITrakHoundObjectQueueEntity ToEntity()
        {
            return new TrakHoundObjectQueueEntity
            {
                QueueUuid = QueueUuid,
                Index = Index,
                MemberUuid = MemberUuid,
                Timestamp = Timestamp,
                SourceUuid = SourceUuid,
                Created = Created
            };
        }
    }
}
