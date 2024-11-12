// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text.Json.Serialization;
using TrakHound.Entities;

namespace TrakHound.Requests
{
    public class TrakHoundBlobEntry : TrakHoundObjectsEntryBase
    {
        private byte[] _entryId;
        [JsonIgnore]
        public override byte[] EntryId
        {
            get
            {
                if (_entryId == null) _entryId = TrakHoundUuid.Create(ObjectPath, TrakHoundObjectsEntityClassName.Blob);
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

        [JsonPropertyName("blobId")]
        public string BlobId { get; set; }

        [JsonPropertyName("contentType")]
        public string ContentType { get; set; }

        [JsonPropertyName("size")]
        public long Size { get; set; }

        [JsonPropertyName("filename")]
        public string Filename { get; set; }


        public TrakHoundBlobEntry() 
        {
            Category = TrakHoundEntityCategoryId.Objects;
            Class = TrakHoundObjectsEntityClassId.Blob;
        }

        public TrakHoundBlobEntry(string objectPath, string blobId, string contentType, long size, string filename = null)
        {
            Category = TrakHoundEntityCategoryId.Objects;
            Class = TrakHoundObjectsEntityClassId.Blob;

            ObjectPath = objectPath;
            BlobId = blobId;
            ContentType = contentType;
            Size = size;
            Filename = filename;
        }
    }
}
