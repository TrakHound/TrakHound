// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Text.Json.Serialization;
using TrakHound.Entities;

namespace TrakHound.Requests
{
    public class TrakHoundObservationEntry : TrakHoundObjectsEntryBase
    {
        private byte[] _entryId;
        [JsonIgnore]
        public override byte[] EntryId
        {
            get
            {
                if (_entryId == null) _entryId = TrakHoundUuid.Create(ObjectPath, TrakHoundObjectsEntityClassName.Observation, Timestamp?.ToUnixTime());
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

        [JsonPropertyName("dataType")]
        public TrakHoundObservationDataType DataType { get; set; }

        [JsonPropertyName("batchId")]
        public ulong? BatchId { get; set; }

        [JsonPropertyName("sequence")]
        public ulong? Sequence { get; set; }

        [JsonPropertyName("timestamp")]
        public DateTime? Timestamp { get; set; }


        public TrakHoundObservationEntry() 
        {
            Category = TrakHoundEntityCategoryId.Objects;
            Class = TrakHoundObjectsEntityClassId.Observation;
        }

        public TrakHoundObservationEntry(string objectPath, string value, TrakHoundObservationDataType dataType = TrakHoundObservationDataType.String)
        {
            Category = TrakHoundEntityCategoryId.Objects;
            Class = TrakHoundObjectsEntityClassId.Observation;

            ObjectPath = objectPath;
            Value = value;
            DataType = dataType;
        }

        public TrakHoundObservationEntry(string objectPath, string value, DateTime timestamp, TrakHoundObservationDataType dataType = TrakHoundObservationDataType.String)
        {
            Category = TrakHoundEntityCategoryId.Objects;
            Class = TrakHoundObjectsEntityClassId.Observation;

            ObjectPath = objectPath;
            Value = value;
            DataType = dataType;
            Timestamp = timestamp;
        }

        public TrakHoundObservationEntry(string objectPath, string value, long timestamp, TrakHoundObservationDataType dataType = TrakHoundObservationDataType.String)
        {
            Category = TrakHoundEntityCategoryId.Objects;
            Class = TrakHoundObjectsEntityClassId.Observation;

            ObjectPath = objectPath;
            Value = value;
            DataType = dataType;
            Timestamp = timestamp.ToDateTime();
        }
    }
}
