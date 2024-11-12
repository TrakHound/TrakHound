// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;

namespace TrakHound.Entities
{
    /// <summary>
    /// Set Entities are used to store content for an Object in the form of a list of strings
    /// </summary>
    public struct TrakHoundObjectSetEntity : ITrakHoundObjectSetEntity
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

        /// <summary>
        /// The UUID of the Object that this Entity is the content for
        /// </summary>
        public string ObjectUuid { get; set; }

        /// <summary>
        /// The value in the list
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// The UUID of the Source Entity that created this Entity
        /// </summary>
        public string SourceUuid { get; set; }

        /// <summary>
        /// The timestamp in UNIX Nanoseconds of when the Entity was created
        /// </summary>
        public long Created { get; set; }


        public byte Category => TrakHoundEntityCategoryId.Objects;

        public byte Class => TrakHoundObjectsEntityClassId.Set;


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
                return !string.IsNullOrEmpty(Uuid) &&
                    !string.IsNullOrEmpty(ObjectUuid) &&
                    !string.IsNullOrEmpty(Value) &&
                    !string.IsNullOrEmpty(SourceUuid) &&
                    Created > 0;
            }
        }


        public TrakHoundObjectSetEntity()
        {
            ObjectUuid = null;
            Value = null;
            SourceUuid = null;
            Created = UnixDateTime.Now;
        }

        public TrakHoundObjectSetEntity(string objectUuid, string value, string sourceUuid = null, long created = 0)
        {
            ObjectUuid = objectUuid;
            Value = value;
            SourceUuid = sourceUuid;
            Created = created > 0 ? created : UnixDateTime.Now;
        }

        public TrakHoundObjectSetEntity(ITrakHoundObjectSetEntity entity)
        {
            ObjectUuid = null;
            Value = null;
            SourceUuid = null;
            Created = UnixDateTime.Now;

            if (entity != null)
            {
                ObjectUuid = entity.ObjectUuid;
                Value = entity.Value;
                SourceUuid = entity.SourceUuid;
                Created = entity.Created > 0 ? entity.Created : UnixDateTime.Now;
            }
        }

        public static string GenerateUuid(string objectUuid, string value)
        {
            return $"{objectUuid}:{value}".ToSHA256Hash();
        }

        public static string GenerateUuid(ITrakHoundObjectSetEntity entity)
        {
            if (entity != null)
            {
                return GenerateUuid(entity.ObjectUuid, entity.Value);
            }

            return null;
        }

        public static byte[] GenerateHash(ITrakHoundObjectSetEntity entity)
        {
            if (entity != null)
            {
                return $"{entity.ObjectUuid}:{entity.Value}:{entity.SourceUuid}:{entity.Created}".ToSHA256HashBytes();
            }

            return null;
        }


        public override string ToString() => ToJson();

        public string ToJson() => ToArray().ToJson();

        public static ITrakHoundObjectSetEntity FromJson(string json) => FromArray(Json.Convert<object[]>(json));

        public static object[] ToArray(ITrakHoundObjectSetEntity obj) => new object[] {
            obj.ObjectUuid,
            obj.Value,
            obj.SourceUuid,
            obj.Created
        };

        public object[] ToArray() => ToArray(this);


        public static ITrakHoundObjectSetEntity FromArray(object[] obj)
        {
            if (obj != null && obj.Length >= 4)
            {
                return new TrakHoundObjectSetEntity
                {
                    ObjectUuid = obj[0]?.ToString(),
                    Value = obj[1]?.ToString(),
                    SourceUuid = obj[2]?.ToString(),
                    Created = obj[3].ToLong()
                };
            }

            return new TrakHoundObjectSetEntity { };
        }

        public static IEnumerable<ITrakHoundObjectSetEntity> FromArray(IEnumerable<object[]> objs)
        {
            if (!objs.IsNullOrEmpty())
            {
                var x = new List<ITrakHoundObjectSetEntity>();
                foreach (var obj in objs)
                {
                    var y = FromArray(obj);
                    if (y.IsValid) x.Add(y);
                }

                return x;
            }

            return Enumerable.Empty<ITrakHoundObjectSetEntity>();
        }

        public void SetSource(string sourceUuid, bool overwrite = false)
        {
            if (string.IsNullOrEmpty(SourceUuid) || overwrite) SourceUuid = sourceUuid;
        }
    }
}
