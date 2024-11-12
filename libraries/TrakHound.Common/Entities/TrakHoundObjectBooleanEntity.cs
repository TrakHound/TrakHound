// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;

namespace TrakHound.Entities
{
    /// <summary>
    /// Boolean Entities are used to store content for an Object in the form of a boolean (true or false)
    /// </summary>
    public struct TrakHoundObjectBooleanEntity : ITrakHoundObjectBooleanEntity
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
        /// The UUID of the Object that this Boolean Entity is associated with
        /// </summary>
        public string ObjectUuid { get; set; }

        /// <summary>
        /// The value associated with this Boolean Entity
        /// </summary>
        public bool Value { get; set; }

        /// <summary>
        /// The UUID of the Source of the Boolean Entity
        /// </summary>
        public string SourceUuid { get; set; }

        /// <summary>
        /// The timestamp that this Boolean Entity was created
        /// </summary>
        public long Created { get; set; }


        public byte Category => TrakHoundEntityCategoryId.Objects;

        public byte Class => TrakHoundObjectsEntityClassId.Boolean;


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


        public TrakHoundObjectBooleanEntity()
        {
            ObjectUuid = null;
            Value = false;
            SourceUuid = null;
            Created = UnixDateTime.Now;
        }

        public TrakHoundObjectBooleanEntity(string objectUuid, bool value, string transactionUuid = null, long created = 0)
        {
            ObjectUuid = objectUuid;
            Value = value;
            SourceUuid = transactionUuid;
            Created = created > 0 ? created : UnixDateTime.Now;
        }

        public TrakHoundObjectBooleanEntity(ITrakHoundObjectBooleanEntity entity)
        {
            ObjectUuid = null;
            Value = false;
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
            return $"{objectUuid}:boolean".ToSHA256Hash();
        }

        public static string GenerateUuid(ITrakHoundObjectBooleanEntity entity)
        {
            if (entity != null)
            {
                return GenerateUuid(entity.ObjectUuid);
            }

            return null;
        }

        public static byte[] GenerateHash(ITrakHoundObjectBooleanEntity entity)
        {
            if (entity != null)
            {
                return $"{entity.ObjectUuid}:{entity.Value}:{entity.SourceUuid}:{entity.Created}".ToSHA256HashBytes();
            }

            return null;
        }


        public override string ToString() => ToJson();

        public string ToJson() => ToArray().ToJson();

        public static ITrakHoundObjectBooleanEntity FromJson(string json) => FromArray(Json.Convert<object[]>(json));

        public static object[] ToArray(ITrakHoundObjectBooleanEntity obj) => new object[] {
            obj.ObjectUuid,
            obj.Value,
            obj.SourceUuid,
            obj.Created
        };

        public object[] ToArray() => ToArray(this);


        public static ITrakHoundObjectBooleanEntity FromArray(object[] obj)
        {
            if (obj != null && obj.Length >= 4)
            {
                return new TrakHoundObjectBooleanEntity
                {
                    ObjectUuid = obj[0]?.ToString(),
                    Value = obj[1].ToBoolean(),
                    SourceUuid = obj[2]?.ToString(),
                    Created = obj[3].ToLong()
                };
            }

            return new TrakHoundObjectBooleanEntity { };
        }

        public static IEnumerable<ITrakHoundObjectBooleanEntity> FromArray(IEnumerable<object[]> objs)
        {
            if (!objs.IsNullOrEmpty())
            {
                var x = new List<ITrakHoundObjectBooleanEntity>();
                foreach (var obj in objs)
                {
                    var y = FromArray(obj);
                    if (y.IsValid) x.Add(y);
                }

                return x;
            }

            return Enumerable.Empty<ITrakHoundObjectBooleanEntity>();
        }

        public void SetSource(string sourceUuid, bool overwrite = false)
        {
            if (string.IsNullOrEmpty(SourceUuid) || overwrite) SourceUuid = sourceUuid;
        }
    }
}
