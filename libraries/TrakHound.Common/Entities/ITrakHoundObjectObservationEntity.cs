// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Entities
{
    /// <summary>
    /// Observations are used to record a Value at a given point in time. This Value may be static or may be constantly changing. 
    /// </summary>
    public interface ITrakHoundObjectObservationEntity : ITrakHoundEntity, ITrakHoundSourcedEntity
    {
        string ObjectUuid { get; }

        ulong BatchId { get; set; }

        ulong Sequence { get; set; }

        int DataType { get; set; }

        string Value { get; set; }

        string SourceUuid { get; set; }

        long Timestamp { get; set; }
    }
}

