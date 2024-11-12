// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Text.Json.Serialization;
using TrakHound.Entities;

namespace TrakHound.Requests
{
    public class TrakHoundStateEntry : TrakHoundObjectsEntryBase
    {
        private byte[] _entryId;
        [JsonIgnore]
        public override byte[] EntryId
        {
            get
            {
                if (_entryId == null) _entryId = TrakHoundUuid.Create(ObjectPath, TrakHoundObjectsEntityClassName.State, Timestamp?.ToUnixTime());
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

        [JsonPropertyName("definitionId")]
        public string DefinitionId { get; set; }

        [JsonPropertyName("ttl")]
        public int TTL { get; set; }

        [JsonPropertyName("timestamp")]
        public DateTime? Timestamp { get; set; }


        public TrakHoundStateEntry() 
        {
            Category = TrakHoundEntityCategoryId.Objects;
            Class = TrakHoundObjectsEntityClassId.State;
        }

        public TrakHoundStateEntry(string objectPath, string definitionId, int ttl = 0)
        {
            Category = TrakHoundEntityCategoryId.Objects;
            Class = TrakHoundObjectsEntityClassId.State;

            ObjectPath = objectPath;
            DefinitionId = definitionId;
            TTL = ttl;
        }

        public TrakHoundStateEntry(string objectPath, string definitionId, DateTime timestamp, int ttl = 0)
        {
            Category = TrakHoundEntityCategoryId.Objects;
            Class = TrakHoundObjectsEntityClassId.State;

            ObjectPath = objectPath;
            DefinitionId = definitionId;
            TTL = ttl;
            Timestamp = timestamp;
        }

        public TrakHoundStateEntry(string objectPath, string definitionId, long timestamp, int ttl = 0)
        {
            Category = TrakHoundEntityCategoryId.Objects;
            Class = TrakHoundObjectsEntityClassId.State;

            ObjectPath = objectPath;
            DefinitionId = definitionId;
            TTL = ttl;
            Timestamp = timestamp.ToDateTime();
        }
    }
}
