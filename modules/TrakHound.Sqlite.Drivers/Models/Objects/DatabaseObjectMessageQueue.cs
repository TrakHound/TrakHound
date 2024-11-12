// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Entities;

namespace TrakHound.Sqlite.Drivers.Models
{
    public class DatabaseObjectMessageQueue : IDatabaseEntity<ITrakHoundObjectMessageQueueEntity>
    {
        public string RequestedId { get; set; }

        public string Uuid { get; set; }

        public string ObjectUuid { get; set; }

        public string QueueId { get; set; }

        public string ContentType { get; set; }

        public string SourceUuid { get; set; }

        public long Created { get; set; }


        public ITrakHoundObjectMessageQueueEntity ToEntity()
        {
            return new TrakHoundObjectMessageQueueEntity
            {
                ObjectUuid = ObjectUuid,
                QueueId = QueueId,
                ContentType = ContentType,
                SourceUuid = SourceUuid,
                Created = Created
            };
        }
    }
}
