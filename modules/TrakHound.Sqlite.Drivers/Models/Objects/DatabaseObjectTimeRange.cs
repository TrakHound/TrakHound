// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Entities;

namespace TrakHound.Sqlite.Drivers.Models
{
    public class DatabaseObjectTimeRange : IDatabaseEntity<ITrakHoundObjectTimeRangeEntity>
    {
        public string RequestedId { get; set; }

        public string Uuid { get; set; }

        public string ObjectUuid { get; set; }

        public long Start { get; set; }

        public long End { get; set; }

        public string SourceUuid { get; set; }

        public long Created { get; set; }


        public ITrakHoundObjectTimeRangeEntity ToEntity()
        {
            return new TrakHoundObjectTimeRangeEntity
            {
                ObjectUuid = ObjectUuid,
                Start = Start,
                End = End,
                SourceUuid = SourceUuid,
                Created = Created
            };
        }
    }
}
