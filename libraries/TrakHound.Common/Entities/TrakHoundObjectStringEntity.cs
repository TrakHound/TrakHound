// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;

namespace TrakHound.Entities
{
    /// <summary>
    /// String Entities are used to store content for an Object in the form of a string
    /// </summary>
    public struct TrakHoundObjectStringEntity : ITrakHoundObjectStringEntity
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
        /// The string content
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// The Uuid of the Transaction that created the Entity
        /// </summary>
        public string SourceUuid { get; set; }

        /// <summary>
        /// The timestamp in UNIX Nanoseconds of when the Entity was created
        /// </summary>
        public long Created { get; set; }


        public byte Category => TrakHoundEntityCategoryId.Objects;

        public byte Class => TrakHoundObjectsEntityClassId.String;


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


        public TrakHoundObjectStringEntity()
        {
            ObjectUuid = null;
            Value = null;
            SourceUuid = null;
            Created = UnixDateTime.Now;
        }

        public TrakHoundObjectStringEntity(string objectUuid, string value, string transactionUuid = null, long created = 0)
        {
            ObjectUuid = objectUuid;
            Value = value;
            SourceUuid = transactionUuid;
            Created = created > 0 ? created : UnixDateTime.Now;
        }

        public TrakHoundObjectStringEntity(ITrakHoundObjectStringEntity entity)
        {
            ObjectUuid = null;
            Value = null;
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
            return $"{objectUuid}:string".ToSHA256Hash();
        }

        public static string GenerateUuid(ITrakHoundObjectStringEntity entity)
        {
            if (entity != null)
            {
                return GenerateUuid(entity.ObjectUuid);
            }

            return null;
        }

        public static byte[] GenerateHash(ITrakHoundObjectStringEntity entity)
        {
            if (entity != null)
            {
                return $"{entity.ObjectUuid}:{entity.Value}:{entity.SourceUuid}:{entity.Created}".ToSHA256HashBytes();
            }

            return null;
        }


        public override string ToString() => ToJson();

        public string ToJson() => ToArray().ToJson();

        public static ITrakHoundObjectStringEntity FromJson(string json) => FromArray(Json.Convert<object[]>(json));

        public static object[] ToArray(ITrakHoundObjectStringEntity obj) => new object[] {
            obj.ObjectUuid,
            obj.Value,
            obj.SourceUuid,
            obj.Created
        };

        public object[] ToArray() => ToArray(this);


        public static ITrakHoundObjectStringEntity FromArray(object[] obj)
        {
            if (obj != null && obj.Length >= 4)
            {
                return new TrakHoundObjectStringEntity
                {
                    ObjectUuid = obj[0]?.ToString(),
                    Value = obj[1]?.ToString(),
                    SourceUuid = obj[2]?.ToString(),
                    Created = obj[3].ToLong()
                };
            }

            return new TrakHoundObjectStringEntity { };
        }

        public static IEnumerable<ITrakHoundObjectStringEntity> FromArray(IEnumerable<object[]> objs)
        {
            if (!objs.IsNullOrEmpty())
            {
                var x = new List<ITrakHoundObjectStringEntity>();
                foreach (var obj in objs)
                {
                    var y = FromArray(obj);
                    if (y.IsValid) x.Add(y);
                }

                return x;
            }

            return Enumerable.Empty<ITrakHoundObjectStringEntity>();
        }


        public void SetSource(string sourceUuid, bool overwrite = false)
        {
            if (string.IsNullOrEmpty(SourceUuid) || overwrite) SourceUuid = sourceUuid;
        }
    }
}
