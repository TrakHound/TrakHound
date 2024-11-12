// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Entities
{
    /// <summary>
    /// Used to associate a Number with a TrakHound Object Entity
    /// </summary>
    public interface ITrakHoundObjectNumberEntity : ITrakHoundEntity, ITrakHoundSourcedEntity
    {
        /// <summary>
        /// The UUID of the Object that this Entity is associated with
        /// </summary>
        string ObjectUuid { get; set; }

        int DataType { get; set; }

        /// <summary>
        /// The message associate with this Number Entity
        /// </summary>
        string Value { get; set; }

        /// <summary>
        /// The ID of the Source of the Number Entity
        /// </summary>
        string SourceUuid { get; set; }
    }
}
