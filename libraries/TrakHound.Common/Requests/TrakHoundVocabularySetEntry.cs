// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Text.Json.Serialization;
using TrakHound.Entities;
using YamlDotNet.Core.Tokens;

namespace TrakHound.Requests
{
    public class TrakHoundVocabularySetEntry : TrakHoundObjectsEntryBase
    {
        private byte[] _entryId;
        [JsonIgnore]
        public override byte[] EntryId
        {
            get
            {
                if (_entryId == null)
                {
                    if (!DefinitionIds.IsNullOrEmpty())
                    {
                        var values = string.Join(':', DefinitionIds);
                        _entryId = TrakHoundUuid.Create(ObjectPath, TrakHoundObjectsEntityClassName.VocabularySet, values);
                    }
                    else
                    {
                        _entryId = TrakHoundUuid.Create(ObjectPath, TrakHoundObjectsEntityClassName.VocabularySet);
                    }
                }

                return _entryId;
            }
        }


        [JsonIgnore]
        public override string AssemblyId
        {
            get => ObjectPath;
            set => ObjectPath = value;
        }

        [JsonIgnore]
        public override string AssemblyDefinitionId
        {
            get => ObjectDefinitionId;
            set => ObjectDefinitionId = value;
        }

        [JsonPropertyName("objectPath")]
        public string ObjectPath { get; set; }

        [JsonPropertyName("objectDefinitionId")]
        public string ObjectDefinitionId { get; set; }

        [JsonPropertyName("definitionIds")]
        public IEnumerable<string> DefinitionIds { get; set; }


        public TrakHoundVocabularySetEntry() 
        {
            Category = TrakHoundEntityCategoryId.Objects;
            Class = TrakHoundObjectsEntityClassId.VocabularySet;
        }

        public TrakHoundVocabularySetEntry(string objectPath, string definitionId)
        {
            Category = TrakHoundEntityCategoryId.Objects;
            Class = TrakHoundObjectsEntityClassId.VocabularySet;

            ObjectPath = objectPath;

            if (definitionId != null)
            {
                DefinitionIds = new string[] { definitionId };
            }
        }

        public TrakHoundVocabularySetEntry(string objectPath, IEnumerable<string> definitionIds)
        {
            Category = TrakHoundEntityCategoryId.Objects;
            Class = TrakHoundObjectsEntityClassId.VocabularySet;

            ObjectPath = objectPath;
            DefinitionIds = definitionIds;
        }
    }
}
