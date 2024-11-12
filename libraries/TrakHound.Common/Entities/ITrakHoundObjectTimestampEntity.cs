// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Entities
{
    /// <summary>
    /// Used to associate a Timestamp with a TrakHound Object Entity
    /// </summary>
    public interface ITrakHoundObjectTimestampEntity : ITrakHoundEntity, ITrakHoundSourcedEntity
    {
        /// <summary>
        /// The UUID of the Object that this Entity is associated with
        /// </summary>
        string ObjectUuid { get; set; }

        /// <summary>
        /// The UNIX Timestamp in Nanoseconds associated with this Timestamp Entity
        /// </summary>
        long Value { get; set; }

        /// <summary>
        /// The UUID of the Source of the Timestamp Entity
        /// </summary>
        string SourceUuid { get; set; }
    }
}
