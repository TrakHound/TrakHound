// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;

namespace TrakHound.Entities
{
    public struct TrakHoundDefinitionDescriptionEntity : ITrakHoundDefinitionDescriptionEntity
    {
        public const string DefaultLanguageCode = "en";


        private string _uuid = null;
        public string Uuid
        {
            get
            {
                if (_uuid == null) _uuid = GenerateUuid(DefinitionUuid, LanguageCode);
                return _uuid;
            }
        }

        public string DefinitionUuid { get; set; }

        public string LanguageCode { get; set; }

        public string Text { get; set; }

        public string SourceUuid { get; set; }

        public long Created { get; set; }


        public byte Category => TrakHoundEntityCategoryId.Definitions;

        public byte Class => TrakHoundDefinitionsEntityClassId.Description;

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
                       !string.IsNullOrEmpty(LanguageCode) &&
                       !string.IsNullOrEmpty(Text) &&
                       Created > 0;
            }
        }


        public TrakHoundDefinitionDescriptionEntity()
        {
            DefinitionUuid = null;
            LanguageCode = null;
            Text = null;
            SourceUuid = null;
            Created = UnixDateTime.Now;
        }

        public TrakHoundDefinitionDescriptionEntity(ITrakHoundDefinitionDescriptionEntity entity)
        {
            DefinitionUuid = null;
            LanguageCode = null;
            Text = null;
            SourceUuid = null;
            Created = UnixDateTime.Now;

            if (entity != null)
            {
                DefinitionUuid = entity.DefinitionUuid;
                LanguageCode = entity.LanguageCode;
                Text = entity.Text;
                SourceUuid = entity.SourceUuid;
                Created = entity.Created;
            }
        }

        public TrakHoundDefinitionDescriptionEntity(string definitionUuid, string description, string languageCode = DefaultLanguageCode, string sourceUuid = null, long created = 0)
        {
            DefinitionUuid = definitionUuid;
            LanguageCode = languageCode;
            Text = description;
            SourceUuid = sourceUuid;
            Created = created > 0 ? created : UnixDateTime.Now;
        }


        public static string GenerateUuid(ITrakHoundDefinitionDescriptionEntity entity)
        {
            if (entity != null)
            {
                return GenerateUuid(entity.DefinitionUuid, entity.LanguageCode);
            }

            return null;
        }

        public static string GenerateUuid(string definitionUuid, string languageCode)
        {
            if (!string.IsNullOrEmpty(definitionUuid) && !string.IsNullOrEmpty(languageCode))
            {
                return $"{definitionUuid}:{languageCode}".ToSHA256Hash();
            }

            return null;
        }

        public static byte[] GenerateHash(ITrakHoundDefinitionDescriptionEntity entity)
        {
            if (entity != null)
            {
                return $"{entity.DefinitionUuid}:{entity.LanguageCode}:{entity.Text}:{entity.SourceUuid}:{entity.Created}".ToSHA256HashBytes();
            }

            return null;
        }


        public override string ToString() => ToJson();

        public string ToJson() => ToArray().ToJson();

        public static ITrakHoundDefinitionDescriptionEntity FromJson(string json) => FromArray(Json.Convert<object[]>(json));


        public static object[] ToArray(ITrakHoundDefinitionDescriptionEntity entity) => new object[] {
            entity.DefinitionUuid,
            entity.LanguageCode,
            entity.Text,
            entity.SourceUuid,
            entity.Created
        };

        public object[] ToArray() => ToArray(this);


        public static ITrakHoundDefinitionDescriptionEntity FromArray(object[] obj)
        {
            if (obj != null && obj.Length >= 5)
            {
                return new TrakHoundDefinitionDescriptionEntity
                {
                    DefinitionUuid = obj[0]?.ToString(),
                    LanguageCode = obj[1]?.ToString(),
                    Text = obj[2]?.ToString(),
                    SourceUuid = obj[3]?.ToString(),
                    Created = obj[4].ToLong()
                };
            }

            return new TrakHoundDefinitionDescriptionEntity { };
        }


        public static IEnumerable<ITrakHoundDefinitionDescriptionEntity> FromArray(IEnumerable<object[]> objs)
        {
            if (!objs.IsNullOrEmpty())
            {
                var x = new List<ITrakHoundDefinitionDescriptionEntity>();
                foreach (var obj in objs)
                {
                    var y = FromArray(obj);
                    if (y.IsValid) x.Add(y);
                }

                return x;
            }

            return Enumerable.Empty<ITrakHoundDefinitionDescriptionEntity>();
        }

        public void SetSource(string sourceUuid, bool overwrite = false)
        {
            if (string.IsNullOrEmpty(SourceUuid) || overwrite) SourceUuid = sourceUuid;
        }
    }
}
