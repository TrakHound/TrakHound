// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;

namespace TrakHound.Entities
{
    /// <summary>
    /// Duration Entities are used to store content for an Object in the form of a time duration
    /// </summary>
    public struct TrakHoundObjectDurationEntity : ITrakHoundObjectDurationEntity
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
        /// The UUID of the Object that this Duration Entity is associated with
        /// </summary>
        public string ObjectUuid { get; set; }

        /// <summary>
        /// The UNIX Nanoseconds that represent the Duration
        /// </summary>
        public ulong Value { get; set; }

        /// <summary>
        /// The UUID of the Source of the Duration Entity
        /// </summary>
        public string SourceUuid { get; set; }

        /// <summary>
        /// The timestamp in UNIX Nanoseconds that this Duration Entity was created
        /// </summary>
        public long Created { get; set; }


        public byte Category => TrakHoundEntityCategoryId.Objects;

        public byte Class => TrakHoundObjectsEntityClassId.Duration;


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
                    Value >= 0 &&
                    !string.IsNullOrEmpty(SourceUuid) &&
                    Created > 0;
            }
        }


        public TrakHoundObjectDurationEntity()
        {
            ObjectUuid = null;
            Value = 0;
            SourceUuid = null;
            Created = UnixDateTime.Now;
        }

        public TrakHoundObjectDurationEntity(string objectUuid, TimeSpan value, string sourceUuid = null, long created = 0)
        {
            ObjectUuid = objectUuid;
            Value = value.TotalNanoseconds.ToULong();
            SourceUuid = sourceUuid;
            Created = created > 0 ? created : UnixDateTime.Now;
        }

        public TrakHoundObjectDurationEntity(string objectUuid, ulong value, string sourceUuid = null, long created = 0)
        {
            ObjectUuid = objectUuid;
            Value = value;
            SourceUuid = sourceUuid;
            Created = created > 0 ? created : UnixDateTime.Now;
        }

        public TrakHoundObjectDurationEntity(string objectUuid, string value, string sourceUuid = null, long created = 0)
        {
            ObjectUuid = objectUuid;
            Value = value.ToTimeSpan().TotalNanoseconds.ToULong();
            SourceUuid = sourceUuid;
            Created = created > 0 ? created : UnixDateTime.Now;
        }

        public TrakHoundObjectDurationEntity(ITrakHoundObjectDurationEntity entity)
        {
            ObjectUuid = null;
            Value = 0;
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

        public static string GenerateUuid(string objectUuid)
        {
            return $"{objectUuid}:duration".ToSHA256Hash();
        }

        public static string GenerateUuid(ITrakHoundObjectDurationEntity entity)
        {
            if (entity != null)
            {
                return GenerateUuid(entity.ObjectUuid);
            }

            return null;
        }

        public static byte[] GenerateHash(ITrakHoundObjectDurationEntity entity)
        {
            if (entity != null)
            {
                return $"{entity.ObjectUuid}:{entity.Value}:{entity.SourceUuid}:{entity.Created}".ToSHA256HashBytes();
            }

            return null;
        }


        public override string ToString() => ToJson();

        public string ToJson() => ToArray().ToJson();

        public static ITrakHoundObjectDurationEntity FromJson(string json) => FromArray(Json.Convert<object[]>(json));

        public static object[] ToArray(ITrakHoundObjectDurationEntity obj) => new object[] {
            obj.ObjectUuid,
            obj.Value,
            obj.SourceUuid,
            obj.Created
        };

        public object[] ToArray() => ToArray(this);


        public static ITrakHoundObjectDurationEntity FromArray(object[] obj)
        {
            if (obj != null && obj.Length >= 4)
            {
                return new TrakHoundObjectDurationEntity
                {
                    ObjectUuid = obj[0]?.ToString(),
                    Value = obj[1].ToULong(),
                    SourceUuid = obj[2]?.ToString(),
                    Created = obj[3].ToLong()
                };
            }

            return new TrakHoundObjectDurationEntity { };
        }

        public static IEnumerable<ITrakHoundObjectDurationEntity> FromArray(IEnumerable<object[]> objs)
        {
            if (!objs.IsNullOrEmpty())
            {
                var x = new List<ITrakHoundObjectDurationEntity>();
                foreach (var obj in objs)
                {
                    var y = FromArray(obj);
                    if (y.IsValid) x.Add(y);
                }

                return x;
            }

            return Enumerable.Empty<ITrakHoundObjectDurationEntity>();
        }

        public void SetSource(string sourceUuid, bool overwrite = false)
        {
            if (string.IsNullOrEmpty(SourceUuid) || overwrite) SourceUuid = sourceUuid;
        }
    }
}
