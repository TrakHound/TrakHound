// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;

namespace TrakHound.Entities
{
    public struct TrakHoundObjectReferenceEntity : ITrakHoundObjectReferenceEntity
    {
        private string _uuid = null;
        public string Uuid
        {
            get
            {
                if (_uuid == null) _uuid = GenerateUuid(this);
                return _uuid;
            }
        }

        public string ObjectUuid { get; set; }

        public string TargetUuid { get; set; }

        public string SourceUuid { get; set; }

        public long Created { get; set; }


        public byte Category => TrakHoundEntityCategoryId.Objects;

        public byte Class => TrakHoundObjectsEntityClassId.Reference;

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
                       !string.IsNullOrEmpty(TargetUuid);
            }
        }


        public TrakHoundObjectReferenceEntity()
        {
            ObjectUuid = null;
            TargetUuid = null;
            SourceUuid = null;
            Created = UnixDateTime.Now;
        }

        public TrakHoundObjectReferenceEntity(ITrakHoundObjectReferenceEntity entity)
        {
            ObjectUuid = null;
            TargetUuid = null;
            SourceUuid = null;
            Created = UnixDateTime.Now;

            if (entity != null)
            {
                ObjectUuid = entity.ObjectUuid;
                TargetUuid = entity.TargetUuid;
                SourceUuid = entity.SourceUuid;
                Created = entity.Created;
            }
        }

        public TrakHoundObjectReferenceEntity(string objectUuid, string target, string sourceUuid = null, long created = 0)
        {
            ObjectUuid = objectUuid;
            TargetUuid = target;
            SourceUuid = sourceUuid;
            Created = created > 0 ? created : UnixDateTime.Now;
        }


        public static string GenerateUuid(ITrakHoundObjectReferenceEntity entity)
        {
            if (entity != null)
            {
                return GenerateUuid(entity.ObjectUuid);
            }

            return null;
        }

        public static string GenerateUuid(string objectUuid)
        {
            return $"{objectUuid}:reference".ToSHA256Hash();
        }


        public static byte[] GenerateHash(ITrakHoundObjectReferenceEntity entity)
        {
            if (entity != null)
            {
                return $"{entity.ObjectUuid}:{entity.TargetUuid}:{entity.SourceUuid}:{entity.Created}".ToSHA256HashBytes();
            }

            return null;
        }


        public override string ToString() => ToJson();

        public string ToJson() => ToArray().ToJson();

        public static ITrakHoundObjectReferenceEntity FromJson(string json) => FromArray(Json.Convert<object[]>(json));


        public static ITrakHoundObjectReferenceEntity FromArray(object[] entity)
        {
            if (entity != null && entity.Length >= 4)
            {
                return new TrakHoundObjectReferenceEntity
                {
                    ObjectUuid = entity[0]?.ToString(),
                    TargetUuid = entity[1]?.ToString(),
                    SourceUuid = entity[2]?.ToString(),
                    Created = entity[3].ToLong(),
                };
            }

            return new TrakHoundObjectReferenceEntity { };
        }

        public static IEnumerable<ITrakHoundObjectReferenceEntity> FromArray(IEnumerable<object[]> entities)
        {
            if (!entities.IsNullOrEmpty())
            {
                var x = new List<ITrakHoundObjectReferenceEntity>();
                foreach (var entity in entities)
                {
                    var y = FromArray(entity);
                    if (y.IsValid) x.Add(y);
                }

                return x;
            }

            return Enumerable.Empty<ITrakHoundObjectReferenceEntity>();
        }

        public static object[] ToArray(ITrakHoundObjectReferenceEntity entity) => new object[] {
            entity.ObjectUuid,
            entity.TargetUuid,
            entity.SourceUuid,
            entity.Created
        };

        public object[] ToArray() => new object[] {
            ObjectUuid,
            TargetUuid,
            SourceUuid,
            Created
        };

        public void SetSource(string sourceUuid, bool overwrite = false)
        {
            if (string.IsNullOrEmpty(SourceUuid) || overwrite) SourceUuid = sourceUuid;
        }
    }
}
