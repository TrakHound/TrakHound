// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Entities
{
    public interface ITrakHoundDefinitionEntity : ITrakHoundEntity, ITrakHoundSourcedEntity
    {
        string Id { get; set; }

        string Type { get; }

        string ParentUuid { get; set; }

        string SourceUuid { get; set; }
    }
}
