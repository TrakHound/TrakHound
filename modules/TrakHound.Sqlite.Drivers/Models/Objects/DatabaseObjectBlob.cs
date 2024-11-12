// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Entities;

namespace TrakHound.Sqlite.Drivers.Models
{
    public class DatabaseObjectBlob : IDatabaseEntity<ITrakHoundObjectBlobEntity>
    {
        public string RequestedId { get; set; }

        public string Uuid { get; set; }

        public string ObjectUuid { get; set; }

        public string BlobId { get; set; }

        public string ContentType { get; set; }

        public long Size { get; set; }

        public string Filename { get; set; }

        public string SourceUuid { get; set; }

        public long Created { get; set; }


        public ITrakHoundObjectBlobEntity ToEntity()
        {
            return new TrakHoundObjectBlobEntity
            {
                ObjectUuid = ObjectUuid,
                BlobId = BlobId,
                ContentType = ContentType,
                Size = Size,
                Filename = Filename,
                SourceUuid = SourceUuid,
                Created = Created
            };
        }
    }
}
