// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Entities
{
    public interface ITrakHoundObjectBlobEntity : ITrakHoundEntity, ITrakHoundSourcedEntity
    {
        string ObjectUuid { get; set; }

        string BlobId { get; set; }

        string ContentType { get; set; }

        long Size { get; set; }

        string Filename { get; set; }

        string SourceUuid { get; set; }
    }
}
