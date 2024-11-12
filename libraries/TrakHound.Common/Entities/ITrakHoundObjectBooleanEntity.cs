// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Entities
{
    /// <summary>
    /// Used to associate a Boolean with a TrakHound Object Entity
    /// </summary>
    public interface ITrakHoundObjectBooleanEntity : ITrakHoundEntity, ITrakHoundSourcedEntity
    {
        /// <summary>
        /// The UUID of the Object that this Entity is associated with
        /// </summary>
        string ObjectUuid { get; set; }

        /// <summary>
        /// The Value associated with this Boolean Entity
        /// </summary>
        bool Value { get; set; }

        /// <summary>
        /// The UUID of the Source of the Boolean Entity
        /// </summary>
        string SourceUuid { get; set; }
    }
}
