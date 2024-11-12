// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Entities;

namespace TrakHound.Sqlite.Drivers.Models
{
    public class DatabaseSource : IDatabaseEntity<ITrakHoundSourceEntity>
    {
        public string RequestedId { get; set; }

        public string Type { get; set; }

        public string Sender { get; set; }

        public string ParentUuid { get; set; }

        public long Created { get; set; }


        public ITrakHoundSourceEntity ToEntity()
        {
            return new TrakHoundSourceEntity
            {
                Type = Type,
                Sender = Sender,
                ParentUuid = ParentUuid,
                Created = Created
            };
        }
    }
}
