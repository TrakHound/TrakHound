// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Entities
{
    /// <summary>
    /// Used to associate a Definition with a TrakHound Object Entity
    /// </summary>
    public interface ITrakHoundObjectVocabularyEntity : ITrakHoundEntity, ITrakHoundSourcedEntity
    {
        /// <summary>
        /// The UUID of the Object that this Entity is associated with
        /// </summary>
        string ObjectUuid { get; set; }

        /// <summary>
        /// The DefinitionUuid associated with the Entity
        /// </summary>
        string DefinitionUuid { get; set; }

        /// <summary>
        /// The UUID of the Source of the Entity
        /// </summary>
        string SourceUuid { get; set; }
    }
}
