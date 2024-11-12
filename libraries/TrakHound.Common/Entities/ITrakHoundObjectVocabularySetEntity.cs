// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Entities
{
    /// <summary>
    /// Entity used to record a set of vocabulary organized by Key
    /// </summary>
    public interface ITrakHoundObjectVocabularySetEntity : ITrakHoundEntity, ITrakHoundSourcedEntity
    {
        /// <summary>
        /// The UUID of the Object that this Entity is associated with
        /// </summary>
        string ObjectUuid { get; set; }

        string DefinitionUuid { get; set; }

        string SourceUuid { get; set; }
    }
}

