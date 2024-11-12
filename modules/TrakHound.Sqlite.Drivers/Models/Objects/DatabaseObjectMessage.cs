// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Entities;

namespace TrakHound.Sqlite.Drivers.Models
{
    public class DatabaseObjectMessage : IDatabaseEntity<ITrakHoundObjectMessageEntity>
    {
        public string RequestedId { get; set; }

        public string Uuid { get; set; }

        public string ObjectUuid { get; set; }

        public string BrokerId { get; set; }

        public string Topic { get; set; }

        public string ContentType { get; set; }

        public bool Retain { get; set; }

        public int Qos { get; set; }

        public string SourceUuid { get; set; }

        public long Created { get; set; }


        public ITrakHoundObjectMessageEntity ToEntity()
        {
            return new TrakHoundObjectMessageEntity
            {
                ObjectUuid = ObjectUuid,
                BrokerId = BrokerId,
                Topic = Topic,
                ContentType = ContentType,
                Retain = Retain,
                Qos = Qos,
                SourceUuid = SourceUuid,
                Created = Created
            };
        }
    }
}
