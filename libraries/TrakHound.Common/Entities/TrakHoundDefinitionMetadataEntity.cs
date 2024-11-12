// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;

namespace TrakHound.Entities
{
    public struct TrakHoundDefinitionMetadataEntity : ITrakHoundDefinitionMetadataEntity
    {
        private string _uuid = null;
        public string Uuid
        {
            get
            {
                if (_uuid == null) _uuid = GenerateUuid(DefinitionUuid, Name);
                return _uuid;
            }
        }

        public string DefinitionUuid { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }

        public string SourceUuid { get; set; }

        public long Created { get; set; }


        public byte Category => TrakHoundEntityCategoryId.Definitions;

        public byte Class => TrakHoundDefinitionsEntityClassId.Metadata;

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
                return !string.IsNullOrEmpty(DefinitionUuid) &&
                       !string.IsNullOrEmpty(Name) &&
                       !string.IsNullOrEmpty(Value) &&
                       Created > 0;
            }
        }


        public TrakHoundDefinitionMetadataEntity()
        {
            DefinitionUuid = null;
            Name = null;
            Value = null;
            SourceUuid = null;
            Created = UnixDateTime.Now;
        }

        public TrakHoundDefinitionMetadataEntity(ITrakHoundDefinitionMetadataEntity entity)
        {
            DefinitionUuid = null;
            Name = null;
            Value = null;
            SourceUuid = null;
            Created = UnixDateTime.Now;

            if (entity != null)
            {
                DefinitionUuid = entity.DefinitionUuid;
                Name = entity.Name;
                Value = entity.Value;
                SourceUuid = entity.SourceUuid;
                Created = entity.Created;
            }
        }

        public TrakHoundDefinitionMetadataEntity(string definitionUuid, string name, object value, string sourceUuid = null, long created = 0)
        {
            DefinitionUuid = definitionUuid;
            Name = name;
            Value = value?.ToString();
            SourceUuid = sourceUuid;
            Created = created > 0 ? created : UnixDateTime.Now;
        }


        public static string GenerateUuid(ITrakHoundDefinitionMetadataEntity entity)
        {
            if (entity != null)
            {
                return GenerateUuid(entity.DefinitionUuid, entity.Name);
            }

            return null;
        }

        public static string GenerateUuid(string definitionUuid, string name)
        {
            if (!string.IsNullOrEmpty(definitionUuid) && !string.IsNullOrEmpty(name))
            {
                return $"{definitionUuid}:{name}".ToSHA256Hash();
            }

            return null;
        }

        public static byte[] GenerateHash(ITrakHoundDefinitionMetadataEntity entity)
        {
            if (entity != null)
            {
                return $"{entity.DefinitionUuid}:{entity.Name}:{entity.Value}:{entity.SourceUuid}:{entity.Created}".ToSHA256HashBytes();
            }

            return null;
        }


        public override string ToString() => ToJson();

        public string ToJson() => ToArray().ToJson();

        public static ITrakHoundDefinitionMetadataEntity FromJson(string json) => FromArray(Json.Convert<object[]>(json));


        public static object[] ToArray(ITrakHoundDefinitionMetadataEntity entity) => new object[] {
            entity.DefinitionUuid,
            entity.Name,
            entity.Value,
            entity.SourceUuid,
            entity.Created
        };

        public object[] ToArray() => ToArray(this);


        public static ITrakHoundDefinitionMetadataEntity FromArray(object[] obj)
        {
            if (obj != null && obj.Length >= 5)
            {
                return new TrakHoundDefinitionMetadataEntity
                {
                    DefinitionUuid = obj[0]?.ToString(),
                    Name = obj[1]?.ToString(),
                    Value = obj[2]?.ToString(),
                    SourceUuid = obj[3]?.ToString(),
                    Created = obj[4].ToLong()
                };
            }

            return new TrakHoundDefinitionMetadataEntity { };
        }


        public static IEnumerable<ITrakHoundDefinitionMetadataEntity> FromArray(IEnumerable<object[]> objs)
        {
            if (!objs.IsNullOrEmpty())
            {
                var x = new List<ITrakHoundDefinitionMetadataEntity>();
                foreach (var obj in objs)
                {
                    var y = FromArray(obj);
                    if (y.IsValid) x.Add(y);
                }

                return x;
            }

            return Enumerable.Empty<ITrakHoundDefinitionMetadataEntity>();
        }

        public void SetSource(string sourceUuid, bool overwrite = false)
        {
            if (string.IsNullOrEmpty(SourceUuid) || overwrite) SourceUuid = sourceUuid;
        }
    }
}
