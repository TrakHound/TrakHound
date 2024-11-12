// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Text.Json.Serialization;
using TrakHound.Entities;

namespace TrakHound.Requests
{
    public class TrakHoundEventEntry : TrakHoundObjectsEntryBase
    {
        private byte[] _entryId;
        [JsonIgnore]
        public override byte[] EntryId
        {
            get
            {
                if (_entryId == null) _entryId = TrakHoundUuid.Create(ObjectPath, TrakHoundObjectsEntityClassName.Event, Timestamp?.ToUnixTime());
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

        [JsonPropertyName("targetPath")]
        public string TargetPath { get; set; }

        [JsonPropertyName("timestamp")]
		public DateTime? Timestamp { get; set; }


		public TrakHoundEventEntry() 
        {
            Category = TrakHoundEntityCategoryId.Objects;
            Class = TrakHoundObjectsEntityClassId.Event;
        }

		public TrakHoundEventEntry(string objectPath, string targetPath)
		{
            Category = TrakHoundEntityCategoryId.Objects;
            Class = TrakHoundObjectsEntityClassId.Event;

            ObjectPath = objectPath;
            TargetPath = targetPath;
		}

		public TrakHoundEventEntry(string objectPath, string targetPath, DateTime timestamp)
        {
            Category = TrakHoundEntityCategoryId.Objects;
            Class = TrakHoundObjectsEntityClassId.Event;

            ObjectPath = objectPath;
            TargetPath = targetPath;
            Timestamp = timestamp;
        }

		public TrakHoundEventEntry(string objectPath, string targetPath, long timestamp)
		{
            Category = TrakHoundEntityCategoryId.Objects;
            Class = TrakHoundObjectsEntityClassId.Event;

            ObjectPath = objectPath;
            TargetPath = targetPath;
			Timestamp = timestamp.ToDateTime();
		}
	}
}
