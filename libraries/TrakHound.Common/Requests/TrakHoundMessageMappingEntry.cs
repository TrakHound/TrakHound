// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text.Json.Serialization;
using TrakHound.Entities;

namespace TrakHound.Requests
{
    public class TrakHoundMessageMappingEntry : TrakHoundObjectsEntryBase
    {
        private byte[] _entryId;
        [JsonIgnore]
        public override byte[] EntryId
        {
            get
            {
                if (_entryId == null) _entryId = TrakHoundUuid.Create(ObjectPath, TrakHoundObjectsEntityClassName.Message);
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

        [JsonPropertyName("brokerId")]
        public string BrokerId { get; set; }

        [JsonPropertyName("topic")]
        public string Topic { get; set; }

        [JsonPropertyName("contentType")]
        public string ContentType { get; set; }

        [JsonPropertyName("retain")]
        public bool Retain { get; set; }

        [JsonPropertyName("qos")]
        public int QoS { get; set; }


        public TrakHoundMessageMappingEntry() 
        {
            Category = TrakHoundEntityCategoryId.Objects;
            Class = TrakHoundObjectsEntityClassId.Message;
        }

        public TrakHoundMessageMappingEntry(string objectPath, string brokerId, string topic, string contentType = null, bool retain = false, int qos = 0)
        {
            Category = TrakHoundEntityCategoryId.Objects;
            Class = TrakHoundObjectsEntityClassId.Message;

            ObjectPath = objectPath;
            BrokerId = brokerId;
            Topic = topic;
            ContentType = contentType;
            Retain = retain;
            QoS = qos;
        }
    }
}
