// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Entities
{
    public interface ITrakHoundSourceMetadataEntity : ITrakHoundEntity
    {
        string SourceUuid { get; set; }

        string Name { get; set; }

        string Value { get; set; }
    }
}
