// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text.Json.Serialization;
using TrakHound.Entities;

namespace TrakHound.Requests
{
    public class TrakHoundGroupEntry : TrakHoundObjectsEntryBase
    {
        private byte[] _entryId;
        [JsonIgnore]
        public override byte[] EntryId
        {
            get
            {
                if (_entryId == null) _entryId = TrakHoundUuid.Create(GroupPath, TrakHoundObjectsEntityClassName.Group, MemberPath);
                return _entryId;
            }
        }


        [JsonIgnore]
        public override string AssemblyId
        {
            get => GroupPath;
            set => GroupPath = value;
        }

        [JsonIgnore]
        public override string AssemblyDefinitionId
        {
            get => GroupDefinitionId;
            set => GroupDefinitionId = value;
        }

        [JsonPropertyName("groupPath")]
        public string GroupPath { get; set; }

        [JsonPropertyName("groupDefinitionId")]
        public string GroupDefinitionId { get; set; }

        [JsonPropertyName("memberPath")]
        public string MemberPath { get; set; }


        public TrakHoundGroupEntry() 
        {
            Category = TrakHoundEntityCategoryId.Objects;
            Class = TrakHoundObjectsEntityClassId.Group;
        }

        public TrakHoundGroupEntry(string groupPath, string memberPath)
        {
            Category = TrakHoundEntityCategoryId.Objects;
            Class = TrakHoundObjectsEntityClassId.Group;

            GroupPath = groupPath;
            MemberPath = memberPath;
        }
    }
}
