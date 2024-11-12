// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;
using TrakHound.Entities;

namespace TrakHound.Entities
{
    public struct TrakHoundObjectMetadataEntity : ITrakHoundObjectMetadataEntity
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

		public string EntityUuid { get; set; }

        public string Name { get; set; }

        public string DefinitionUuid { get; set; }

        public string Value { get; set; }

        public string ValueDefinitionUuid { get; set; }

        public string SourceUuid { get; set; }

        public long Created { get; set; }

        public string Type => TrakHoundType.GetType(DefinitionUuid);

        public string ValueType => TrakHoundType.GetType(ValueDefinitionUuid);


        public byte Category => TrakHoundEntityCategoryId.Objects;

        public byte Class => TrakHoundObjectsEntityClassId.Metadata;

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
                return !string.IsNullOrEmpty(EntityUuid) &&
                       !string.IsNullOrEmpty(Name) &&
                       !string.IsNullOrEmpty(Value) &&
                       !string.IsNullOrEmpty(SourceUuid) &&
                       Created > 0;
            }
        }


        public TrakHoundObjectMetadataEntity()
        {
            EntityUuid = null;
            Name = null;
            DefinitionUuid = null;
            Value = null;
            ValueDefinitionUuid = null;
            SourceUuid = null;
            Created = UnixDateTime.Now;
        }

        public TrakHoundObjectMetadataEntity(ITrakHoundObjectMetadataEntity entity)
        {
            EntityUuid = null;
            Name = null;
            DefinitionUuid = null;
            Value = null;
            ValueDefinitionUuid = null;
            SourceUuid = null;
            Created = UnixDateTime.Now;

            if (entity != null)
            {
                EntityUuid = entity.EntityUuid;
                Name = entity.Name;
                DefinitionUuid = entity.DefinitionUuid;
                Value = entity.Value;
                ValueDefinitionUuid = entity.ValueDefinitionUuid;
                SourceUuid = entity.SourceUuid;
                Created = entity.Created;
            }
        }

        public TrakHoundObjectMetadataEntity(
            string objectUuid,
            string name,
            object value,
            string definitionUuid = null,
            string valueDefinitionUuid = null,
            string sourceUuid = null,
            long created = 0
            )
        {
            EntityUuid = objectUuid;
            Name = name;
            DefinitionUuid = definitionUuid;
            Value = value?.ToString();
            ValueDefinitionUuid = valueDefinitionUuid;
            SourceUuid = sourceUuid;
            Created = created > 0 ? created : UnixDateTime.Now;
        }


        public static string GenerateUuid(string objectUuid, string name)
        {
            return $"{objectUuid}:{name}".ToSHA256Hash();
        }

        public static string GenerateUuid(ITrakHoundObjectMetadataEntity entity)
        {
            if (entity != null)
            {
                return GenerateUuid(entity.EntityUuid, entity.Name);
            }

            return null;
        }

        public static byte[] GenerateHash(ITrakHoundObjectMetadataEntity metadata)
        {
            if (metadata != null)
            {
                return $"{metadata.EntityUuid}:{metadata.Name}:{metadata.DefinitionUuid}:{metadata.Value}:{metadata.ValueDefinitionUuid}:{metadata.SourceUuid}:{metadata.Created}".ToSHA256HashBytes();
            }

            return null;
        }


        public override string ToString() => ToJson();

        public string ToJson() => ToArray().ToJson();

        public static ITrakHoundObjectMetadataEntity FromJson(string json) => FromArray(Json.Convert<object[]>(json));


        public static object[] ToArray(ITrakHoundObjectMetadataEntity obj) => new object[] {
            obj.EntityUuid,
            obj.Name,
            obj.DefinitionUuid,
            obj.Value,
            obj.ValueDefinitionUuid,
            obj.SourceUuid,
            obj.Created
        };

        public object[] ToArray() => ToArray(this);


        public static ITrakHoundObjectMetadataEntity FromArray(object[] obj)
        {
            if (obj != null && obj.Length >= 7)
            {
                return new TrakHoundObjectMetadataEntity
                {
                    EntityUuid = obj[0]?.ToString(),
                    Name = obj[1]?.ToString(),
                    DefinitionUuid = obj[2]?.ToString(),
                    Value = obj[3]?.ToString(),
                    ValueDefinitionUuid = obj[4]?.ToString(),
                    SourceUuid = obj[5]?.ToString(),
                    Created = obj[6].ToLong()
                };
            }

            return new TrakHoundObjectMetadataEntity { };
        }


        public static IEnumerable<ITrakHoundObjectMetadataEntity> FromArray(IEnumerable<object[]> objs)
        {
            if (!objs.IsNullOrEmpty())
            {
                var x = new List<ITrakHoundObjectMetadataEntity>();
                foreach (var obj in objs)
                {
                    var y = FromArray(obj);
                    if (y.IsValid) x.Add(y);
                }

                return x;
            }

            return Enumerable.Empty<ITrakHoundObjectMetadataEntity>();
        }


        public void SetSource(string sourceUuid, bool overwrite = false)
        {
            if (string.IsNullOrEmpty(SourceUuid) || overwrite) SourceUuid = sourceUuid;
        }
    }
}
