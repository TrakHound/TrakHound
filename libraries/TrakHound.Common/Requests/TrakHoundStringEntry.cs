// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Text.Json.Serialization;
using TrakHound.Entities;

namespace TrakHound.Requests
{
    public class TrakHoundStringEntry : TrakHoundObjectsEntryBase
    {
        private byte[] _entryId;
        [JsonIgnore]
        public override byte[] EntryId
        {
            get
            {
                if (_entryId == null) _entryId = TrakHoundUuid.Create(ObjectPath, TrakHoundObjectsEntityClassName.String);
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

        [JsonPropertyName("value")]
        public string Value { get; set; }


        public TrakHoundStringEntry() 
        {
            Category = TrakHoundEntityCategoryId.Objects;
            Class = TrakHoundObjectsEntityClassId.String;
        }

        public TrakHoundStringEntry(string objectPath, string value)
        {
            Category = TrakHoundEntityCategoryId.Objects;
            Class = TrakHoundObjectsEntityClassId.String;

            ObjectPath = objectPath;
            Value = value;
        }
    }
}
