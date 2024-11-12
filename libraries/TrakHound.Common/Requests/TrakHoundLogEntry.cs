// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Text.Json.Serialization;
using TrakHound.Entities;

namespace TrakHound.Requests
{
    public class TrakHoundLogEntry : TrakHoundObjectsEntryBase
    {
        private byte[] _entryId;
        [JsonIgnore]
        public override byte[] EntryId
        {
            get
            {
                if (_entryId == null) _entryId = TrakHoundUuid.Create(ObjectPath, TrakHoundObjectsEntityClassName.Log, LogLevel, Timestamp?.ToUnixTime());
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

        [JsonPropertyName("logLevel")]
        public TrakHoundLogLevel LogLevel { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("code")]
        public string Code { get; set; }

        [JsonPropertyName("timestamp")]
        public DateTime? Timestamp { get; set; }


        public TrakHoundLogEntry() 
        {
            Category = TrakHoundEntityCategoryId.Objects;
            Class = TrakHoundObjectsEntityClassId.Log;
            Timestamp = DateTime.Now;
        }

        public TrakHoundLogEntry(string objectPath, string message, TrakHoundLogLevel logLevel = TrakHoundLogLevel.Information)
        {
            Category = TrakHoundEntityCategoryId.Objects;
            Class = TrakHoundObjectsEntityClassId.Log;

            ObjectPath = objectPath;
            Message = message;
            LogLevel = logLevel;
        }

        public TrakHoundLogEntry(string objectPath, string message, DateTime timestamp, TrakHoundLogLevel logLevel = TrakHoundLogLevel.Information)
        {
            Category = TrakHoundEntityCategoryId.Objects;
            Class = TrakHoundObjectsEntityClassId.Log;

            ObjectPath = objectPath;
            Message = message;
            Timestamp = timestamp;
        }

        public TrakHoundLogEntry(string objectPath, string message, long timestamp, TrakHoundLogLevel logLevel = TrakHoundLogLevel.Information)
        {
            Category = TrakHoundEntityCategoryId.Objects;
            Class = TrakHoundObjectsEntityClassId.Log;

            ObjectPath = objectPath;
            Message = message;
            Timestamp = timestamp.ToDateTime();
        }
    }
}
