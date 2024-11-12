// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text.Json.Serialization;
using TrakHound.Entities;

namespace TrakHound.Requests
{
    public class TrakHoundDurationDeleteOperation : TrakHoundEntityRemoveBase
    {
        private byte[] _entryId;
        public override byte[] EntryId
        {
            get
            {
                if (_entryId == null) _entryId = TrakHoundUuid.Create(Path);
                return _entryId;
            }
        }


        [JsonPropertyName("path")]
        public string Path { get; set; }


        public TrakHoundDurationDeleteOperation()
        {
            Category = TrakHoundEntityCategoryId.Objects;
            Class = TrakHoundObjectsEntityClassId.Duration;
        }
    }
}
