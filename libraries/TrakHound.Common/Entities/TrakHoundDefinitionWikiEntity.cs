// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;

namespace TrakHound.Entities
{
    /// <summary>
    /// Used to describe a Wiki entry for a TrakHound Definition
    /// </summary>
    public struct TrakHoundDefinitionWikiEntity : ITrakHoundDefinitionWikiEntity
    {
        public const string DefaultSection = "Main";


        private string _uuid = null;
        /// <summary>
        /// The ID of the Entity that the Wiki describes
        /// </summary>
        public string Uuid
        {
            get
            {
                if (_uuid == null) _uuid = GenerateUuid(DefinitionUuid, Section);
                return _uuid;
            }
        }

        /// <summary>
        /// The ID of the Definition that the Wiki describes
        /// </summary>
        public string DefinitionUuid { get; set; }

        /// <summary>
        /// The identifier for the Section (ex. Main, History, Setup, etc.)
        /// </summary>
        public string Section { get; set; }

        /// <summary>
        /// The text formatted in Markdown that describes the Wiki
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// The ID of the Source of the Wiki
        /// </summary>
        public string SourceUuid { get; set; }

        /// <summary>
        /// The timestamp that the Wiki was created
        /// </summary>
        public long Created { get; set; }


        public byte Category => TrakHoundEntityCategoryId.Definitions;

        public byte Class => TrakHoundDefinitionsEntityClassId.Wiki;

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
                       !string.IsNullOrEmpty(Section) &&
                       !string.IsNullOrEmpty(Text) &&
                       !string.IsNullOrEmpty(SourceUuid);
            }
        }


        public TrakHoundDefinitionWikiEntity()
        {
            DefinitionUuid = null;
            Section = DefaultSection;
            Text = null;
            SourceUuid = null;
            Created = 0;
        }

        public TrakHoundDefinitionWikiEntity(string definitionId, string text, string section = null, string sourceId = null, long created = 0)
        {
            DefinitionUuid = definitionId;
            Section = !string.IsNullOrEmpty(section) ? section : DefaultSection;
            Text = text;
            SourceUuid = sourceId;
            Created = created > 0 ? created : UnixDateTime.Now;
        }

        public TrakHoundDefinitionWikiEntity(ITrakHoundDefinitionWikiEntity definition)
        {
            DefinitionUuid = null;
            Section = DefaultSection;
            Text = null;
            SourceUuid = null;
            Created = 0;

            if (definition != null)
            {
                DefinitionUuid = definition.DefinitionUuid;
                Section = definition.Section;
                Text = definition.Text;
                SourceUuid = definition.SourceUuid;
                Created = definition.Created;
            }
        }

        public static string GenerateUuid(string definitionId, string section)
        {
            return $"{definitionId}:{section}".ToSHA256Hash();
        }

        public static string GenerateUuid(ITrakHoundDefinitionWikiEntity wiki)
        {
            return GenerateUuid(wiki.DefinitionUuid, wiki.Section);
        }

        public static byte[] GenerateHash(ITrakHoundDefinitionWikiEntity wiki)
        {
            if (wiki != null)
            {
                return $"{wiki.DefinitionUuid}:{wiki.Section}:{wiki.Text}:{wiki.SourceUuid}:{wiki.Created}".ToSHA256HashBytes();
            }

            return null;
        }


        public override string ToString() => ToJson();

        public string ToJson() => ToArray().ToJson();

        public static ITrakHoundDefinitionWikiEntity FromJson(string json) => FromArray(Json.Convert<object[]>(json));


        public static object[] ToArray(ITrakHoundDefinitionWikiEntity wiki) => new object[] {
            wiki.DefinitionUuid,
            wiki.Section,
            wiki.Text,
            wiki.SourceUuid,
            wiki.Created
        };

        public object[] ToArray() => ToArray(this);


        public static ITrakHoundDefinitionWikiEntity FromArray(object[] obj)
        {
            if (obj != null && obj.Length >= 5)
            {
                return new TrakHoundDefinitionWikiEntity
                {
                    DefinitionUuid = obj[0]?.ToString(),
                    Section = obj[1]?.ToString(),
                    Text = obj[2]?.ToString(),
                    SourceUuid = obj[3]?.ToString(),
                    Created = obj[4].ToLong()
                };
            }

            return new TrakHoundDefinitionWikiEntity { };
        }

        public static IEnumerable<ITrakHoundDefinitionWikiEntity> FromArray(IEnumerable<object[]> objs)
        {
            if (!objs.IsNullOrEmpty())
            {
                var x = new List<ITrakHoundDefinitionWikiEntity>();
                foreach (var obj in objs)
                {
                    var y = FromArray(obj);
                    if (y.IsValid) x.Add(y);
                }

                return x;
            }

            return Enumerable.Empty<ITrakHoundDefinitionWikiEntity>();
        }

        public void SetSource(string sourceUuid, bool overwrite = false)
        {
            if (string.IsNullOrEmpty(SourceUuid) || overwrite) SourceUuid = sourceUuid;
        }
    }
}
