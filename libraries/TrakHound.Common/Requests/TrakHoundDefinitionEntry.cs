// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Text.Json.Serialization;
using TrakHound.Entities;

namespace TrakHound.Requests
{
    public class TrakHoundDefinitionEntry : TrakHoundEntityEntryBase
    {
        private Dictionary<string, string> _descriptions;
        private Dictionary<string, string> _metadata;


        private byte[] _entryId;
        [JsonIgnore]
        public override byte[] EntryId
        {
            get
            {
                if (_entryId == null) _entryId = TrakHoundUuid.Create(Id, TrakHoundDefinitionsEntityClassName.Definition);
                return _entryId;
            }
        }


        [JsonIgnore]
        public override string AssemblyId => Id;

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("parentId")]
        public string ParentId { get; set; }

        [JsonPropertyName("metadata")]
        public Dictionary<string, string> Metadata
        {
            get
            {
                if (_metadata == null) _metadata = new Dictionary<string, string>();
                return _metadata;
            }
            set
            {
                _metadata = value;
            }
        }

        [JsonPropertyName("descriptions")]
        public Dictionary<string, string> Descriptions
        {
            get
            {
                if (_descriptions == null) _descriptions = new Dictionary<string, string>();
                return _descriptions;
            }
            set
            {
                _descriptions = value;
            }
        }


        public TrakHoundDefinitionEntry()
        {
            Category = TrakHoundEntityCategoryId.Definitions;
            Class = TrakHoundDefinitionsEntityClassId.Definition;
        }

        public TrakHoundDefinitionEntry(string id, string description = null, string parentId = null)
        {
            Category = TrakHoundEntityCategoryId.Definitions;
            Class = TrakHoundDefinitionsEntityClassId.Definition;

            Id = id;
            Descriptions.Add(TrakHoundDefinitionDescriptionEntity.DefaultLanguageCode, description);
            ParentId = parentId;
        }
    }
}
