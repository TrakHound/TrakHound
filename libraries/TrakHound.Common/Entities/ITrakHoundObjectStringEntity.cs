// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Entities
{
    /// <summary>
    /// Used to associate a String with a TrakHound Object Entity
    /// </summary>
    public interface ITrakHoundObjectStringEntity : ITrakHoundEntity, ITrakHoundSourcedEntity
    {
        /// <summary>
        /// The UUID of the Object that this Entity is associated with
        /// </summary>
        string ObjectUuid { get; set; }

        /// <summary>
        /// The message associate with this String Entity
        /// </summary>
        string Value { get; set; }

        /// <summary>
        /// The Uuid of the Transaction that created the Entity
        /// </summary>
        string SourceUuid { get; set; }
    }
}
