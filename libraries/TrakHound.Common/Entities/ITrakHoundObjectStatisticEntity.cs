// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Entities
{
    /// <summary>
    /// A Statistic is used to record a Value over a range of time.
    /// </summary>
    public interface ITrakHoundObjectStatisticEntity : ITrakHoundEntity, ITrakHoundSourcedEntity
    {
        string ObjectUuid { get; set; }

        long TimeRangeStart { get; set; }

        long TimeRangeEnd { get; set; }

        int DataType { get; set; }

        string Value { get; set; }

        string SourceUuid { get; set; }

        long Timestamp { get; set; }
    }
}

