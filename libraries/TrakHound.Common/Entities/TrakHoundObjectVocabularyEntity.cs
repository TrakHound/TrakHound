// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;

namespace TrakHound.Entities
{
    /// <summary>
    /// Vocabulary Entities are used to store content for an Object in the form of a Definition UUID
    /// </summary>
    public struct TrakHoundObjectVocabularyEntity : ITrakHoundObjectVocabularyEntity
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
        /// The timestamp in UNIX Ticks (1 / 10,000) of when the Entity was created
        /// </summary>
        public long Created { get; set; }


        public byte Category => TrakHoundEntityCategoryId.Objects;

        public byte Class => TrakHoundObjectsEntityClassId.Vocabulary;

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
                return !string.IsNullOrEmpty(ObjectUuid) &&
                       !string.IsNullOrEmpty(DefinitionUuid);
            }
        }


        public TrakHoundObjectVocabularyEntity()
        {
            ObjectUuid = null;
            DefinitionUuid = null;
            SourceUuid = null;
            Created = UnixDateTime.Now;
        }

        public TrakHoundObjectVocabularyEntity(ITrakHoundObjectVocabularyEntity entity)
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
                Created = entity.Created;
            }
        }

        public TrakHoundObjectVocabularyEntity(string objectUuid, string definitionUuid, string sourceUuid = null, long created = 0)
        {
            ObjectUuid = objectUuid;
            DefinitionUuid = definitionUuid;
            SourceUuid = sourceUuid;
            Created = created > 0 ? created : UnixDateTime.Now;
        }


        public static string GenerateUuid(ITrakHoundObjectVocabularyEntity entity)
        {
            if (entity != null)
            {
                return GenerateUuid(entity.ObjectUuid);
            }

            return null;
        }

        public static string GenerateUuid(string objectUuid)
        {
            return $"{objectUuid}:vocabulary".ToSHA256Hash();
        }


        public static byte[] GenerateHash(ITrakHoundObjectVocabularyEntity entity)
        {
            if (entity != null)
            {
                return $"{entity.ObjectUuid}:{entity.DefinitionUuid}:{entity.SourceUuid}:{entity.Created}".ToSHA256HashBytes();
            }

            return null;
        }


        public override string ToString() => ToJson();

        public string ToJson() => ToArray().ToJson();

        public static ITrakHoundObjectVocabularyEntity FromJson(string json) => FromArray(Json.Convert<object[]>(json));


        public static ITrakHoundObjectVocabularyEntity FromArray(object[] entity)
        {
            if (entity != null && entity.Length >= 4)
            {
                return new TrakHoundObjectVocabularyEntity
                {
                    ObjectUuid = entity[0]?.ToString(),
                    DefinitionUuid = entity[1]?.ToString(),
                    SourceUuid = entity[2]?.ToString(),
                    Created = entity[3].ToLong(),
                };
            }

            return new TrakHoundObjectVocabularyEntity { };
        }

        public static IEnumerable<ITrakHoundObjectVocabularyEntity> FromArray(IEnumerable<object[]> entities)
        {
            if (!entities.IsNullOrEmpty())
            {
                var x = new List<ITrakHoundObjectVocabularyEntity>();
                foreach (var entity in entities)
                {
                    var y = FromArray(entity);
                    if (y.IsValid) x.Add(y);
                }

                return x;
            }

            return Enumerable.Empty<ITrakHoundObjectVocabularyEntity>();
        }

        public static object[] ToArray(ITrakHoundObjectVocabularyEntity entity) => new object[] {
            entity.ObjectUuid,
            entity.DefinitionUuid,
            entity.SourceUuid,
            entity.Created
        };

        public object[] ToArray() => new object[] {
            ObjectUuid,
            DefinitionUuid,
            SourceUuid,
            Created
        };

        public void SetSource(string sourceUuid, bool overwrite = false)
        {
            if (string.IsNullOrEmpty(SourceUuid) || overwrite) SourceUuid = sourceUuid;
        }
    }
}
