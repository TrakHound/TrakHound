// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;

namespace TrakHound.Entities
{
    /// <summary>
    /// Number Entities are used to store content for an Object in the form of a number
    /// </summary>
    public struct TrakHoundObjectNumberEntity : ITrakHoundObjectNumberEntity
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
        /// The UUID of the Object that this Number Entity is associated with
        /// </summary>
        public string ObjectUuid { get; set; }

        public int DataType { get; set; }

        /// <summary>
        /// The value associate with this Number Entity
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// The UUID of the Source of the Number Entity
        /// </summary>
        public string SourceUuid { get; set; }

        /// <summary>
        /// The timestamp that this Number Entity was created
        /// </summary>
        public long Created { get; set; }


        public byte Category => TrakHoundEntityCategoryId.Objects;

        public byte Class => TrakHoundObjectsEntityClassId.Number;


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
                    DataType >= 0 &&
                    !string.IsNullOrEmpty(Value) &&
                    !string.IsNullOrEmpty(SourceUuid) &&
                    Created > 0;
            }
        }


        public TrakHoundObjectNumberEntity()
        {
            ObjectUuid = null;
            DataType = (int)TrakHoundNumberDataType.Float;
            Value = null;
            SourceUuid = null;
            Created = UnixDateTime.Now;
        }

        public TrakHoundObjectNumberEntity(string objectUuid, string value, TrakHoundNumberDataType dataType = TrakHoundNumberDataType.Float, string sourceUuid = null, long created = 0)
        {
            ObjectUuid = objectUuid;
            DataType = (int)dataType;
            Value = value;
            SourceUuid = sourceUuid;
            Created = created > 0 ? created : UnixDateTime.Now;
        }

        public TrakHoundObjectNumberEntity(ITrakHoundObjectNumberEntity entity)
        {
            ObjectUuid = null;
            DataType = (int)TrakHoundNumberDataType.Float;
            Value = null;
            SourceUuid = null;
            Created = UnixDateTime.Now;

            if (entity != null)
            {
                _uuid = entity.Uuid;
                ObjectUuid = entity.ObjectUuid;
                DataType = entity.DataType;
                Value = entity.Value;
                SourceUuid = entity.SourceUuid;
                Created = entity.Created > 0 ? entity.Created : UnixDateTime.Now;
            }
        }

        public static string GenerateUuid(string objectUuid)
        {
            return $"{objectUuid}:number".ToSHA256Hash();
        }

        public static string GenerateUuid(ITrakHoundObjectNumberEntity entity)
        {
            if (entity != null)
            {
                return GenerateUuid(entity.ObjectUuid);
            }

            return null;
        }

        public static byte[] GenerateHash(ITrakHoundObjectNumberEntity entity)
        {
            if (entity != null)
            {
                return $"{entity.ObjectUuid}:{entity.DataType}:{entity.Value}:{entity.SourceUuid}:{entity.Created}".ToSHA256HashBytes();
            }

            return null;
        }


        public override string ToString() => ToJson();

        public string ToJson() => ToArray().ToJson();

        public static ITrakHoundObjectNumberEntity FromJson(string json) => FromArray(Json.Convert<object[]>(json));

        public static object[] ToArray(ITrakHoundObjectNumberEntity obj) => new object[] {
            obj.ObjectUuid,
            obj.DataType,
            obj.Value,
            obj.SourceUuid,
            obj.Created
        };

        public object[] ToArray() => ToArray(this);


        public static ITrakHoundObjectNumberEntity FromArray(object[] obj)
        {
            if (obj != null && obj.Length >= 5)
            {
                return new TrakHoundObjectNumberEntity
                {
                    ObjectUuid = obj[0]?.ToString(),
                    DataType = obj[1].ToInt(),
                    Value = obj[2]?.ToString(),
                    SourceUuid = obj[3]?.ToString(),
                    Created = obj[4].ToLong()
                };
            }

            return new TrakHoundObjectNumberEntity { };
        }

        public static IEnumerable<ITrakHoundObjectNumberEntity> FromArray(IEnumerable<object[]> objs)
        {
            if (!objs.IsNullOrEmpty())
            {
                var x = new List<ITrakHoundObjectNumberEntity>();
                foreach (var obj in objs)
                {
                    var y = FromArray(obj);
                    if (y.IsValid) x.Add(y);
                }

                return x;
            }

            return Enumerable.Empty<ITrakHoundObjectNumberEntity>();
        }

        public void SetSource(string sourceUuid, bool overwrite = false)
        {
            if (string.IsNullOrEmpty(SourceUuid) || overwrite) SourceUuid = sourceUuid;
        }
    }
}
