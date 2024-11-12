// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Entities;

namespace TrakHound.Sqlite.Drivers.Models
{
    public class DatabaseObjectStatistic : IDatabaseEntity<ITrakHoundObjectStatisticEntity>
    {
        public string RequestedId { get; set; }

        public string Uuid { get; set; }

        public string ObjectUuid { get; set; }

        public long TimeRangeStart { get; set; }

        public long TimeRangeSpan { get; set; }

        public int DataType { get; set; }

        public double Value { get; set; }

        public long Timestamp { get; set; }

        public string SourceUuid { get; set; }

        public long Created { get; set; }


        public ITrakHoundObjectStatisticEntity ToEntity()
        {
            var timeRangeEnd = TimeRangeStart + TimeRangeSpan;

            return new TrakHoundObjectStatisticEntity
            {
                ObjectUuid = ObjectUuid,
                TimeRangeStart = TimeRangeStart,
                TimeRangeEnd = timeRangeEnd,
                DataType = DataType,
                Value = Value.ToString(),
                Timestamp = Timestamp,
                SourceUuid = SourceUuid,
                Created = Created
            };
        }
    }
}

