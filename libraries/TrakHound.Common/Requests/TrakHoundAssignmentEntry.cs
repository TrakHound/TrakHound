// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Text.Json.Serialization;
using TrakHound.Entities;

namespace TrakHound.Requests
{
    public class TrakHoundAssignmentEntry : TrakHoundObjectsEntryBase
    {
        private byte[] _entryId;
        [JsonIgnore]
        public override byte[] EntryId
        {
            get
            {
                if (_entryId == null) _entryId = TrakHoundUuid.Create(AssigneePath, TrakHoundObjectsEntityClassName.Assignment, AddTimestamp.ToUnixTime());
                return _entryId;
            }
        }


        [JsonIgnore]
        public override string AssemblyId
        {
            get => AssigneePath;
            set => AssigneePath = value;
        }

        [JsonIgnore]
        public override string AssemblyDefinitionId
        {
            get => AssigneeDefinitionId;
            set => AssigneeDefinitionId = value;
        }

        [JsonPropertyName("assigneePath")]
        public string AssigneePath { get; set; }

        [JsonPropertyName("assigneeDefinitionId")]
        public string AssigneeDefinitionId { get; set; }

        [JsonPropertyName("memberPath")]
        public string MemberPath { get; set; }

        [JsonPropertyName("addTimestamp")]
        public DateTime AddTimestamp { get; set; }

        [JsonPropertyName("removeTimestamp")]
        public DateTime? RemoveTimestamp { get; set; }


        public TrakHoundAssignmentEntry() 
        {
            Category = TrakHoundEntityCategoryId.Objects;
            Class = TrakHoundObjectsEntityClassId.Assignment;
            AddTimestamp = DateTime.Now;
        }

        public TrakHoundAssignmentEntry(string assigneePath, string memberPath)
        {
            Category = TrakHoundEntityCategoryId.Objects;
            Class = TrakHoundObjectsEntityClassId.Assignment;

            AssigneePath = assigneePath;
            MemberPath = memberPath;
            AddTimestamp = DateTime.Now;
        }

        public TrakHoundAssignmentEntry(string assigneePath, string memberPath, DateTime addTimestamp, DateTime? removeTimestamp = null)
        {
            Category = TrakHoundEntityCategoryId.Objects;
            Class = TrakHoundObjectsEntityClassId.Assignment;

            AssigneePath = assigneePath;
            MemberPath = memberPath;
            AddTimestamp = addTimestamp;
            RemoveTimestamp = removeTimestamp;
        }

        public TrakHoundAssignmentEntry(string assigneePath, string memberPath, long addTimestamp, long? removeTimestamp = null)
        {
            Category = TrakHoundEntityCategoryId.Objects;
            Class = TrakHoundObjectsEntityClassId.Assignment;

            AssigneePath = assigneePath;
            MemberPath = memberPath;
            AddTimestamp = addTimestamp.ToDateTime();
            RemoveTimestamp = removeTimestamp?.ToDateTime();
        }
    }
}
