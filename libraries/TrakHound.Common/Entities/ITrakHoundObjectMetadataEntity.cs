// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Entities
{
    public interface ITrakHoundObjectMetadataEntity : ITrakHoundEntity, ITrakHoundSourcedEntity
    {
        string EntityUuid { get; set; }

        string Name { get; set; }

        string DefinitionUuid { get; set; }

        string Value { get; set; }

        string ValueDefinitionUuid { get; set; }

        string SourceUuid { get; set; }


        string Type { get; }

        string ValueType { get; }
    }
}
