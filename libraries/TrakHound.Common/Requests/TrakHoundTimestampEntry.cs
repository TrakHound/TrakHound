// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Text.Json.Serialization;
using TrakHound.Entities;

namespace TrakHound.Requests
{
    public class TrakHoundTimestampEntry : TrakHoundObjectsEntryBase
    {
        private byte[] _entryId;
        [JsonIgnore]
        public override byte[] EntryId
        {
            get
            {
                if (_entryId == null) _entryId = TrakHoundUuid.Create(ObjectPath, TrakHoundObjectsEntityClassName.Timestamp);
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


        public TrakHoundTimestampEntry() 
        {
            Category = TrakHoundEntityCategoryId.Objects;
            Class = TrakHoundObjectsEntityClassId.Timestamp;
        }

        public TrakHoundTimestampEntry(string objectPath, string value)
        {
            Category = TrakHoundEntityCategoryId.Objects;
            Class = TrakHoundObjectsEntityClassId.Timestamp;

            ObjectPath = objectPath;
            Value = value;
        }

        public TrakHoundTimestampEntry(string objectPath, long value)
        {
            Category = TrakHoundEntityCategoryId.Objects;
            Class = TrakHoundObjectsEntityClassId.Timestamp;

            ObjectPath = objectPath;
            Value = value.ToString();
        }

        public TrakHoundTimestampEntry(string objectPath, DateTime value)
        {
            Category = TrakHoundEntityCategoryId.Objects;
            Class = TrakHoundObjectsEntityClassId.Timestamp;

            ObjectPath = objectPath;
            Value = value.ToUnixTime().ToString();
        }
    }
}
