// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Text.Json.Serialization;
using TrakHound.Entities;

namespace TrakHound.Requests
{
    public class TrakHoundQueueEntry : TrakHoundObjectsEntryBase
    {
        private byte[] _entryId;
        [JsonIgnore]
        public override byte[] EntryId
        {
            get
            {
                if (_entryId == null) _entryId = TrakHoundUuid.Create(QueuePath, TrakHoundObjectsEntityClassName.Queue, MemberPath);
                return _entryId;
            }
        }



        [JsonIgnore]
        public override string AssemblyId
        {
            get => QueuePath;
            set => QueuePath = value;
        }

        [JsonIgnore]
        public override string AssemblyDefinitionId
        {
            get => QueueDefinitionId;
            set => QueueDefinitionId = value;
        }

        [JsonPropertyName("queuePath")]
        public string QueuePath { get; set; }

        [JsonPropertyName("queueDefinitionId")]
        public string QueueDefinitionId { get; set; }

        [JsonPropertyName("memberPath")]
        public string MemberPath { get; set; }

        [JsonPropertyName("index")]
        public int Index { get; set; }

        [JsonPropertyName("timestamp")]
        public DateTime? Timestamp { get; set; }


        public TrakHoundQueueEntry() 
        {
            Category = TrakHoundEntityCategoryId.Objects;
            Class = TrakHoundObjectsEntityClassId.Queue;
        }

        public TrakHoundQueueEntry(string queuePath, string memberPath, int index = 0)
        {
            Category = TrakHoundEntityCategoryId.Objects;
            Class = TrakHoundObjectsEntityClassId.Queue;

            QueuePath = queuePath;
            MemberPath = memberPath;
            Index = index;
        }
    }
}
