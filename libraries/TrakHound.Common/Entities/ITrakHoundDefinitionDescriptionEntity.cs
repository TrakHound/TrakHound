// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Entities
{
    public interface ITrakHoundDefinitionDescriptionEntity : ITrakHoundEntity, ITrakHoundSourcedEntity
    {
        string DefinitionUuid { get; set; }

        string LanguageCode { get; }

        string Text { get; set; }

        string SourceUuid { get; set; }
    }
}
