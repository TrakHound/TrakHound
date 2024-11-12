// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;

namespace TrakHound.Entities
{
    public struct TrakHoundObjectBlobEntity : ITrakHoundObjectBlobEntity
    {
        private string _uuid = null;
        public string Uuid
        {
            get
            {
                if (_uuid == null) _uuid = GenerateUuid(ObjectUuid);
                return _uuid;
            }
        }

        public string ObjectUuid { get; set; }

        public string BlobId { get; set; }

        public string ContentType { get; set; }

        public string Filename { get; set; }

        public long Size { get; set; }

        public string SourceUuid { get; set; }

        public long Created { get; set; }


        public byte Category => TrakHoundEntityCategoryId.Objects;

        public byte Class => TrakHoundObjectsEntityClassId.Blob;

        private byte[] _hash = null;
        public byte[] Hash
        {
            get
            {
                if (_hash == null) _hash = GenerateHash(this);
                return _hash;
            }
        }

        public bool IsValid
        {
            get
            {
                return !string.IsNullOrEmpty(ObjectUuid) &&
                       !string.IsNullOrEmpty(BlobId) &&
                       !string.IsNullOrEmpty(ContentType) &&
                       Size > 0;
            }
        }


        public TrakHoundObjectBlobEntity()
        {
            ObjectUuid = null;
            BlobId = null;
            ContentType = null;
            Filename = null;
            Size = 0;
            SourceUuid = null;
            Created = UnixDateTime.Now;
        }

        public TrakHoundObjectBlobEntity(ITrakHoundObjectBlobEntity entity)
        {
            ObjectUuid = null;
            BlobId = null;
            ContentType = null;
            Filename = null;
            Size = 0;
            SourceUuid = null;
            Created = UnixDateTime.Now;

            if (entity != null)
            {
                ObjectUuid = entity.ObjectUuid;
                BlobId = entity.BlobId;
                ContentType = entity.ContentType;
                Filename = entity.Filename;
                Size = entity.Size;
                SourceUuid = entity.SourceUuid;
                Created = entity.Created;
            }
        }

        public TrakHoundObjectBlobEntity(string objectUuid, string blobId, string contentType, long size, string filename = null, string sourceUuid = null, long created = 0)
        {
            ObjectUuid = objectUuid;
            BlobId = blobId;
            ContentType = contentType;
            Filename = filename;
            Size = size;
            SourceUuid = sourceUuid;
            Created = created > 0 ? created : UnixDateTime.Now;
        }


        public static string GenerateUuid(ITrakHoundObjectBlobEntity entity)
        {
            if (entity != null)
            {
                return GenerateUuid(entity.ObjectUuid);
            }

            return null;
        }

        public static string GenerateUuid(string objectUuid)
        {
            if (!string.IsNullOrEmpty(objectUuid))
            {
                return $"{objectUuid}:blob".ToSHA256Hash();
            }

            return null;
        }


        public static byte[] GenerateHash(ITrakHoundObjectBlobEntity entity)
        {
            if (entity != null)
            {
                return $"{entity.Uuid}:{entity.ObjectUuid}:{entity.BlobId}:{entity.ContentType}:{entity.Filename}:{entity.Size}:{entity.SourceUuid}:{entity.Created}".ToSHA256HashBytes();
            }

            return null;
        }


        public override string ToString() => ToJson();

        public string ToJson() => ToArray().ToJson();

        public static ITrakHoundObjectBlobEntity FromJson(string json) => FromArray(Json.Convert<object[]>(json));


        public static ITrakHoundObjectBlobEntity FromArray(object[] obj)
        {
            if (obj != null && obj.Length >= 7)
            {
                return new TrakHoundObjectBlobEntity
                {
                    ObjectUuid = obj[0]?.ToString(),
                    BlobId = obj[1]?.ToString(),
                    ContentType = obj[2]?.ToString(),
                    Size = obj[3].ToLong(),
                    Filename = obj[4]?.ToString(),
                    SourceUuid = obj[5]?.ToString(),
                    Created = obj[6].ToLong(),
                };
            }

            return new TrakHoundObjectBlobEntity { };
        }

        public static IEnumerable<ITrakHoundObjectBlobEntity> FromArray(IEnumerable<object[]> objs)
        {
            if (!objs.IsNullOrEmpty())
            {
                var x = new List<ITrakHoundObjectBlobEntity>();
                foreach (var obj in objs)
                {
                    var y = FromArray(obj);
                    if (y.IsValid) x.Add(y);
                }

                return x;
            }

            return Enumerable.Empty<ITrakHoundObjectBlobEntity>();
        }

        public static object[] ToArray(ITrakHoundObjectBlobEntity obj) => new object[] {
            obj.ObjectUuid,
            obj.BlobId,
            obj.ContentType,
            obj.Size,
            obj.Filename,
            obj.SourceUuid,
            obj.Created
        };

        public object[] ToArray() => new object[] {
            ObjectUuid,
            BlobId,
            ContentType,
            Size,
            Filename,
            SourceUuid,
            Created
        };

        public void SetSource(string sourceUuid, bool overwrite = false)
        {
            if (string.IsNullOrEmpty(SourceUuid) || overwrite) SourceUuid = sourceUuid;
        }
    }
}
