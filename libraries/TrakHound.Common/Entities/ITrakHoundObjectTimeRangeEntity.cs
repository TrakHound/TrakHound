// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;

namespace TrakHound.Entities
{
    /// <summary>
    /// Used to associate a Range of Time to a TrakHound Object Entity
    /// </summary>
    public interface ITrakHoundObjectTimeRangeEntity : ITrakHoundEntity, ITrakHoundSourcedEntity
    {
        /// <summary>
        /// The UUID of the Object that the TimeRange Entry is assigned to
        /// </summary>
        string ObjectUuid { get; set; }

        /// <summary>
        /// The timestamp in Unix Ticks when this TimeRange Entry begins
        /// </summary>
        long Start { get; set; }

        /// <summary>
        /// The timestamp in Unix Ticks when this TimeRange Entry ends
        /// </summary>
        long End { get; set; }

        /// <summary>
        /// The duration of the TimeRange Entry
        /// </summary>
        TimeSpan Duration { get; }

        /// <summary>
        /// The UUID of the Source that created this TimeRange Entry
        /// </summary>
        string SourceUuid { get; set; }
    }
}
