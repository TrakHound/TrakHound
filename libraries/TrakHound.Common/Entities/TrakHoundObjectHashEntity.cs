// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;

namespace TrakHound.Entities
{
    public struct TrakHoundObjectHashEntity : ITrakHoundObjectHashEntity
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

        public string Key { get; set; }

        public string Value { get; set; }

        public string SourceUuid { get; set; }

        public long Created { get; set; }


        public byte Category => TrakHoundEntityCategoryId.Objects;

        public byte Class => TrakHoundObjectsEntityClassId.Hash;

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
                       !string.IsNullOrEmpty(Key) &&
                       !string.IsNullOrEmpty(Value);
            }
        }


        public TrakHoundObjectHashEntity()
        {
            ObjectUuid = null;
            Key = null;
            Value = null;
            SourceUuid = null;
            Created = UnixDateTime.Now;
        }

        public TrakHoundObjectHashEntity(ITrakHoundObjectHashEntity entity)
        {
            ObjectUuid = null;
            Key = null;
            Value = null;
            SourceUuid = null;
            Created = UnixDateTime.Now;

            if (entity != null)
            {
                ObjectUuid = entity.ObjectUuid;
                Key = entity.Key;
                Value = entity.Value;
                SourceUuid = entity.SourceUuid;
                Created = entity.Created;
            }
        }

        public TrakHoundObjectHashEntity(string objectUuid, string key, string value, string sourceUuid = null, long created = 0)
        {
            ObjectUuid = objectUuid;
            Key = key;
            Value = value;
            SourceUuid = sourceUuid;
            Created = created > 0 ? created : UnixDateTime.Now;
        }


        public static string GenerateUuid(ITrakHoundObjectHashEntity entity)
        {
            if (entity != null)
            {
                return GenerateUuid(entity.ObjectUuid, entity.Key);
            }

            return null;
        }

        public static string GenerateUuid(string objectUuid, string key)
        {
            return $"{objectUuid}:{key}".ToSHA256Hash();
        }


        public static byte[] GenerateHash(ITrakHoundObjectHashEntity entity)
        {
            if (entity != null)
            {
                return $"{entity.ObjectUuid}:{entity.Key}:{entity.Value}:{entity.SourceUuid}:{entity.Created}".ToSHA256HashBytes();
            }

            return null;
        }


        public override string ToString() => ToJson();

        public string ToJson() => ToArray().ToJson();

        public static ITrakHoundObjectHashEntity FromJson(string json) => FromArray(Json.Convert<object[]>(json));


        public static ITrakHoundObjectHashEntity FromArray(object[] entity)
        {
            if (entity != null && entity.Length >= 4)
            {
                return new TrakHoundObjectHashEntity
                {
                    ObjectUuid = entity[0]?.ToString(),
                    Key = entity[1]?.ToString(),
                    Value = entity[2]?.ToString(),
                    SourceUuid = entity[3]?.ToString(),
                    Created = entity[4].ToLong(),
                };
            }

            return new TrakHoundObjectHashEntity { };
        }

        public static IEnumerable<ITrakHoundObjectHashEntity> FromArray(IEnumerable<object[]> entities)
        {
            if (!entities.IsNullOrEmpty())
            {
                var x = new List<ITrakHoundObjectHashEntity>();
                foreach (var entity in entities)
                {
                    var y = FromArray(entity);
                    if (y.IsValid) x.Add(y);
                }

                return x;
            }

            return Enumerable.Empty<ITrakHoundObjectHashEntity>();
        }

        public static object[] ToArray(ITrakHoundObjectHashEntity entity) => new object[] {
            entity.ObjectUuid,
            entity.Key,
            entity.Value,
            entity.SourceUuid,
            entity.Created
        };

        public object[] ToArray() => new object[] {
            ObjectUuid,
            Key,
            Value,
            SourceUuid,
            Created
        };

        public void SetSource(string sourceUuid, bool overwrite = false)
        {
            if (string.IsNullOrEmpty(SourceUuid) || overwrite) SourceUuid = sourceUuid;
        }
    }
}
