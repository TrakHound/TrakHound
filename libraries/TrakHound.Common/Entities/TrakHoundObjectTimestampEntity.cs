// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;

namespace TrakHound.Entities
{
    /// <summary>
    /// Timestamp Entities are used to store content for an Object in the form of a timestamp
    /// </summary>
    public struct TrakHoundObjectTimestampEntity : ITrakHoundObjectTimestampEntity
    {
        private string _uuid = null;
        /// <summary>
        /// The Unique Identifier that identifies this Entity
        /// </summary>
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
        /// The UNIX Nanoseconds that represent the timestamp
        /// </summary>
        public long Value { get; set; }

        /// <summary>
        /// The UUID of the Source Entity that created this Entity
        /// </summary>
        public string SourceUuid { get; set; }

        /// <summary>
        /// The timestamp in UNIX Nanoseconds of when the Entity was created
        /// </summary>
        public long Created { get; set; }


        public byte Category => TrakHoundEntityCategoryId.Objects;

        public byte Class => TrakHoundObjectsEntityClassId.Timestamp;


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
                    !string.IsNullOrEmpty(SourceUuid) &&
                    Created > 0;
            }
        }


        public TrakHoundObjectTimestampEntity()
        {
            ObjectUuid = null;
            Value = 0;
            SourceUuid = null;
            Created = UnixDateTime.Now;
        }

        public TrakHoundObjectTimestampEntity(string objectUuid, DateTime value, string sourceUuid = null, long created = 0)
        {
            ObjectUuid = objectUuid;
            Value = value.ToUnixTime();
            SourceUuid = sourceUuid;
            Created = created > 0 ? created : UnixDateTime.Now;
        }

        public TrakHoundObjectTimestampEntity(string objectUuid, long value, string sourceUuid = null, long created = 0)
        {
            ObjectUuid = objectUuid;
            Value = value;
            SourceUuid = sourceUuid;
            Created = created > 0 ? created : UnixDateTime.Now;
        }

        public TrakHoundObjectTimestampEntity(string objectUuid, string value, string sourceUuid = null, long created = 0)
        {
            ObjectUuid = objectUuid;
            Value = value.ToDateTime().ToUnixTime();
            SourceUuid = sourceUuid;
            Created = created > 0 ? created : UnixDateTime.Now;
        }

        public TrakHoundObjectTimestampEntity(ITrakHoundObjectTimestampEntity entity)
        {
            ObjectUuid = null;
            Value = 0;
            SourceUuid = null;
            Created = UnixDateTime.Now;

            if (entity != null)
            {
                _uuid = entity.Uuid;
                ObjectUuid = entity.ObjectUuid;
                Value = entity.Value;
                SourceUuid = entity.SourceUuid;
                Created = entity.Created > 0 ? entity.Created : UnixDateTime.Now;
            }
        }

        public static string GenerateUuid(string objectUuid)
        {
            return $"{objectUuid}:timestamp".ToSHA256Hash();
        }

        public static string GenerateUuid(ITrakHoundObjectTimestampEntity entity)
        {
            if (entity != null)
            {
                return GenerateUuid(entity.ObjectUuid);
            }

            return null;
        }

        public static byte[] GenerateHash(ITrakHoundObjectTimestampEntity entity)
        {
            if (entity != null)
            {
                return $"{entity.ObjectUuid}:{entity.Value}:{entity.SourceUuid}:{entity.Created}".ToSHA256HashBytes();
            }

            return null;
        }


        public override string ToString() => ToJson();

        public string ToJson() => ToArray().ToJson();

        public static ITrakHoundObjectTimestampEntity FromJson(string json) => FromArray(Json.Convert<object[]>(json));

        public static object[] ToArray(ITrakHoundObjectTimestampEntity obj) => new object[] {
            obj.ObjectUuid,
            obj.Value,
            obj.SourceUuid,
            obj.Created
        };

        public object[] ToArray() => ToArray(this);


        public static ITrakHoundObjectTimestampEntity FromArray(object[] obj)
        {
            if (obj != null && obj.Length >= 4)
            {
                return new TrakHoundObjectTimestampEntity
                {
                    ObjectUuid = obj[0]?.ToString(),
                    Value = obj[1].ToLong(),
                    SourceUuid = obj[2]?.ToString(),
                    Created = obj[3].ToLong()
                };
            }

            return new TrakHoundObjectTimestampEntity { };
        }

        public static IEnumerable<ITrakHoundObjectTimestampEntity> FromArray(IEnumerable<object[]> objs)
        {
            if (!objs.IsNullOrEmpty())
            {
                var x = new List<ITrakHoundObjectTimestampEntity>();
                foreach (var obj in objs)
                {
                    var y = FromArray(obj);
                    if (y.IsValid) x.Add(y);
                }

                return x;
            }

            return Enumerable.Empty<ITrakHoundObjectTimestampEntity>();
        }

        public void SetSource(string sourceUuid, bool overwrite = false)
        {
            if (string.IsNullOrEmpty(SourceUuid) || overwrite) SourceUuid = sourceUuid;
        }
    }
}
