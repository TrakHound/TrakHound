// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Entities
{
    public interface ITrakHoundObjectEntity : ITrakHoundEntity, ITrakHoundSourcedEntity
    {

        string Path { get; set; }

        string Name { get; }

        string Namespace { get; set; }

        string ParentUuid { get; }

        string ContentType { get; set; }

        string DefinitionUuid { get; set; }

        byte Priority { get; set; }

        string SourceUuid { get; set; }


        string GetAbsolutePath();
    }
}
