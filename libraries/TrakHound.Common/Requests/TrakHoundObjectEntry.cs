// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Text.Json.Serialization;
using TrakHound.Entities;

namespace TrakHound.Requests
{
    public class TrakHoundObjectEntry : TrakHoundEntityEntryBase
    {
        private Dictionary<string, string> _metadata;


        private byte[] _entryId;
        [JsonIgnore]
        public override byte[] EntryId
        {
            get
            {
                if (_entryId == null) _entryId = TrakHoundUuid.Create(Path, TrakHoundObjectsEntityClassName.Object);
                return _entryId;
            }
        }


        [JsonIgnore]
        public override string AssemblyId
        {
            get => Path;
            set => Path = value;
        }

        [JsonPropertyName("path")]
        public string Path { get; set; }

        [JsonPropertyName("namespace")]
        public string Namespace { get; set; }

        [JsonPropertyName("contentType")]
        public string ContentType { get; set; }

        [JsonPropertyName("definitionId")]
        public string DefinitionId { get; set; }

        [JsonPropertyName("priority")]
        public byte Priority { get; set; }

        [JsonPropertyName("metadata")]
        public Dictionary<string, string> Metadata
        {
            get
            {
                if (_metadata == null) _metadata = new Dictionary<string, string>();
                return _metadata;
            }
            set
            {
                _metadata = value;
            }
        }

        [JsonPropertyName("indexes")]
        public IEnumerable<TrakHoundIndexEntry> Indexes { get; set; }


        public TrakHoundObjectEntry()
        {
            Category = TrakHoundEntityCategoryId.Objects;
            Class = TrakHoundObjectsEntityClassId.Object;
            Priority = TrakHoundObjectEntity.DefaultPriority;
        }

        public TrakHoundObjectEntry(
            string path,
            TrakHoundObjectContentType contentType = TrakHoundObjectContentType.Directory,
            string definitionId = null,
            string ns = null,
            byte priority = TrakHoundObjectEntity.DefaultPriority,
            IEnumerable<TrakHoundIndexEntry> indexes = null
            )
        {
            Category = TrakHoundEntityCategoryId.Objects;
            Class = TrakHoundObjectsEntityClassId.Object;
            Path = path;
            Namespace = ns;
            ContentType = contentType.ToString();
            DefinitionId = definitionId;
            Priority = priority;
            Indexes = indexes;
        }
    }
}
