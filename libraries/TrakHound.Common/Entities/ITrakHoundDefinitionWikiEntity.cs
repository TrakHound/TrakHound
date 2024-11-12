// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Entities
{
    /// <summary>
    /// Used to describe a Wiki entry for a TrakHound Definition
    /// </summary>
    public interface ITrakHoundDefinitionWikiEntity : ITrakHoundEntity, ITrakHoundSourcedEntity
    {
        string DefinitionUuid { get; set; }

        /// <summary>
        /// The identifier for the Section (ex. Main, History, Setup, etc.)
        /// </summary>
        string Section { get; set; }

        /// <summary>
        /// The text formatted in Markdown that describes the Entry
        /// </summary>
        string Text { get; set; }

        /// <summary>
        /// The ID of the Source of the entry
        /// </summary>
        string SourceUuid { get; set; }
    }
}
