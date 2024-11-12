// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Entities
{
    /// <summary>
    /// Used to associate a Duration with a TrakHound Object Entity
    /// </summary>
    public interface ITrakHoundObjectDurationEntity : ITrakHoundEntity, ITrakHoundSourcedEntity
    {
        /// <summary>
        /// The UUID of the Object that this Entity is associated with
        /// </summary>
        string ObjectUuid { get; set; }

        /// <summary>
        /// The Duration in UNIX Nanoseconds associated with this Duration Entity
        /// </summary>
        ulong Value { get; set; }

        /// <summary>
        /// The UUID of the Source of the Duration Entity
        /// </summary>
        string SourceUuid { get; set; }
    }
}
