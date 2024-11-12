// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;

namespace TrakHound.Entities
{
    /// <summary>
    /// Metadata Entities are used to store additional information about a Source
    /// </summary>
    public struct TrakHoundSourceMetadataEntity : ITrakHoundSourceMetadataEntity
    {
        /// <summary>
        /// The Unique Identifier that identifies this Entity
        /// </summary>
        public string Uuid => GenerateUuid(SourceUuid, Name);

        /// <summary>
        /// The UUID of the Source that this Entity is the content for
        /// </summary>
        public string SourceUuid { get; set; }

        /// <summary>
        /// The name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The value
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// The timestamp in UNIX Nanoseconds of when the Entity was created
        /// </summary>
        public long Created { get; set; }


        public byte Category => TrakHoundEntityCategoryId.Sources;

        public byte Class => TrakHoundSourcesEntityClassId.Metadata;

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
                return !string.IsNullOrEmpty(SourceUuid) &&
                       !string.IsNullOrEmpty(Name) &&
                       !string.IsNullOrEmpty(Value);
            }
        }


        public TrakHoundSourceMetadataEntity()
        {
            SourceUuid = null;
            Name = null;
            Value = null;
            Created = UnixDateTime.Now;
        }

        public TrakHoundSourceMetadataEntity(ITrakHoundSourceMetadataEntity entity)
        {
            SourceUuid = null;
            Name = null;
            Value = null;
            Created = UnixDateTime.Now;

            if (entity != null)
            {
                SourceUuid = entity.SourceUuid;
                Name = entity.Name;
                Value = entity.Value;
                Created = entity.Created;
            }
        }

        public TrakHoundSourceMetadataEntity(string sourceUuid, string name, object value, long created = 0)
        {
            SourceUuid = sourceUuid;
            Name = name;
            Value = value?.ToString();
            Created = created > 0 ? created : UnixDateTime.Now;
        }


        public static string GenerateUuid(ITrakHoundSourceMetadataEntity entity)
        {
            if (entity != null)
            {
                return GenerateUuid(entity.SourceUuid, entity.Name);
            }

            return null;
        }

        public static string GenerateUuid(string sourceUuid, string name)
        {
            if (!string.IsNullOrEmpty(sourceUuid) && !string.IsNullOrEmpty(name))
            {
                return $"{sourceUuid}:{name}".ToSHA256Hash();
            }

            return null;
        }


        public static byte[] GenerateHash(ITrakHoundSourceMetadataEntity entity)
        {
            if (entity != null)
            {
                return $"{entity.SourceUuid}:{entity.Name}:{entity.Value}:{entity.Created}".ToSHA256HashBytes();
            }

            return null;
        }


        public override string ToString() => ToJson();

        public string ToJson() => ToArray().ToJson();

        public static ITrakHoundSourceMetadataEntity FromJson(string json) => FromArray(Json.Convert<object[]>(json));


        public static ITrakHoundSourceMetadataEntity FromArray(object[] obj)
        {
            if (obj != null && obj.Length >= 4)
            {
                return new TrakHoundSourceMetadataEntity
                {
                    SourceUuid = obj[0]?.ToString(),
                    Name = obj[1]?.ToString(),
                    Value = obj[2]?.ToString(),
                    Created = obj[3].ToLong(),
                };
            }

            return new TrakHoundSourceMetadataEntity { };
        }

        public static IEnumerable<ITrakHoundSourceMetadataEntity> FromArray(IEnumerable<object[]> objs)
        {
            if (!objs.IsNullOrEmpty())
            {
                var x = new List<ITrakHoundSourceMetadataEntity>();
                foreach (var obj in objs)
                {
                    var y = FromArray(obj);
                    if (y.IsValid) x.Add(y);
                }

                return x;
            }

            return Enumerable.Empty<ITrakHoundSourceMetadataEntity>();
        }

        public static object[] ToArray(ITrakHoundSourceMetadataEntity obj) => new object[] {
            obj.SourceUuid,
            obj.Name,
            obj.Value,
            obj.Created
        };

        public object[] ToArray() => new object[] {
            SourceUuid,
            Name,
            Value,
            Created
        };
    }
}
