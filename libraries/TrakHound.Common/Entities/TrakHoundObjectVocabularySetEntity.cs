// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;

namespace TrakHound.Entities
{
    /// <summary>
    /// UVocabularySet Entities are used to store content for an Object in the form of a list of Definition UUIDs
    /// </summary>
    public struct TrakHoundObjectVocabularySetEntity : ITrakHoundObjectVocabularySetEntity
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
        /// The UUID of the Definition that defines the entity
        /// </summary>
        public string DefinitionUuid { get; set; }

        /// <summary>
        /// The UUID of the Source Entity that created this Entity
        /// </summary>
        public string SourceUuid { get; set; }

        /// <summary>
        /// The timestamp in UNIX Nanoseconds of when the Entity was created
        /// </summary>
        public long Created { get; set; }


        public byte Category => TrakHoundEntityCategoryId.Objects;

        public byte Class => TrakHoundObjectsEntityClassId.VocabularySet;


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
                    !string.IsNullOrEmpty(DefinitionUuid) &&
                    !string.IsNullOrEmpty(SourceUuid) &&
                    Created > 0;
            }
        }


        public TrakHoundObjectVocabularySetEntity()
        {
            ObjectUuid = null;
            DefinitionUuid = null;
            SourceUuid = null;
            Created = UnixDateTime.Now;
        }

        public TrakHoundObjectVocabularySetEntity(string objectUuid, string definitionUuid, string sourceUuid = null, long created = 0)
        {
            ObjectUuid = objectUuid;
            DefinitionUuid = definitionUuid;
            SourceUuid = sourceUuid;
            Created = created > 0 ? created : UnixDateTime.Now;
        }

        public TrakHoundObjectVocabularySetEntity(ITrakHoundObjectVocabularySetEntity entity)
        {
            ObjectUuid = null;
            DefinitionUuid = null;
            SourceUuid = null;
            Created = UnixDateTime.Now;

            if (entity != null)
            {
                ObjectUuid = entity.ObjectUuid;
                DefinitionUuid = entity.DefinitionUuid;
                SourceUuid = entity.SourceUuid;
                Created = entity.Created > 0 ? entity.Created : UnixDateTime.Now;
            }
        }

        public static string GenerateUuid(string objectUuid, string definitionUuid)
        {
            return $"{objectUuid}:{definitionUuid}".ToSHA256Hash();
        }

        public static string GenerateUuid(ITrakHoundObjectVocabularySetEntity entity)
        {
            if (entity != null)
            {
                return GenerateUuid(entity.ObjectUuid, entity.DefinitionUuid);
            }

            return null;
        }

        public static byte[] GenerateHash(ITrakHoundObjectVocabularySetEntity entity)
        {
            if (entity != null)
            {
                return $"{entity.ObjectUuid}:{entity.DefinitionUuid}:{entity.SourceUuid}:{entity.Created}".ToSHA256HashBytes();
            }

            return null;
        }


        public override string ToString() => ToJson();

        public string ToJson() => ToArray().ToJson();

        public static ITrakHoundObjectVocabularySetEntity FromJson(string json) => FromArray(Json.Convert<object[]>(json));

        public static object[] ToArray(ITrakHoundObjectVocabularySetEntity entity) => new object[] {
            entity.ObjectUuid,
            entity.DefinitionUuid,
            entity.SourceUuid,
            entity.Created
        };

        public object[] ToArray() => ToArray(this);


        public static ITrakHoundObjectVocabularySetEntity FromArray(object[] entity)
        {
            if (entity != null && entity.Length >= 4)
            {
                return new TrakHoundObjectVocabularySetEntity
                {
                    ObjectUuid = entity[0]?.ToString(),
                    DefinitionUuid = entity[1]?.ToString(),
                    SourceUuid = entity[2]?.ToString(),
                    Created = entity[3].ToLong()
                };
            }

            return new TrakHoundObjectVocabularySetEntity { };
        }

        public static IEnumerable<ITrakHoundObjectVocabularySetEntity> FromArray(IEnumerable<object[]> objs)
        {
            if (!objs.IsNullOrEmpty())
            {
                var x = new List<ITrakHoundObjectVocabularySetEntity>();
                foreach (var obj in objs)
                {
                    var y = FromArray(obj);
                    if (y.IsValid) x.Add(y);
                }

                return x;
            }

            return Enumerable.Empty<ITrakHoundObjectVocabularySetEntity>();
        }

        public void SetSource(string sourceUuid, bool overwrite = false)
        {
            if (string.IsNullOrEmpty(SourceUuid) || overwrite) SourceUuid = sourceUuid;
        }
    }
}
