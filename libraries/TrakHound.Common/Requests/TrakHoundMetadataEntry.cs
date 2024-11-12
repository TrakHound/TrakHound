// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text.Json.Serialization;
using TrakHound.Entities;

namespace TrakHound.Requests
{
    public class TrakHoundMetadataEntry : TrakHoundObjectsEntryBase
    {
        private byte[] _entryId;
        [JsonIgnore]
        public override byte[] EntryId
        {
            get
            {
                if (_entryId == null) _entryId = TrakHoundUuid.Create(EntityUuid, TrakHoundObjectsEntityClassName.Metadata, Name);
                return _entryId;
            }
        }


        [JsonIgnore]
        public override string AssemblyId
        {
            get => EntityUuid;
            set => EntityUuid = value;
        }

        [JsonPropertyName("entityUuid")]
        public string EntityUuid { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("definitionId")]
        public string DefinitionId { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }

        [JsonPropertyName("valueDefinitionId")]
        public string ValueDefinitionId { get; set; }


        public TrakHoundMetadataEntry() 
        {
            Category = TrakHoundEntityCategoryId.Objects;
            Class = TrakHoundObjectsEntityClassId.Metadata;
        }

        public TrakHoundMetadataEntry(string entityUuid, string name, string value)
        {
            Category = TrakHoundEntityCategoryId.Objects;
            Class = TrakHoundObjectsEntityClassId.Metadata;

            EntityUuid = entityUuid;
            Name = name;
            Value = value;
        }
    }
}
