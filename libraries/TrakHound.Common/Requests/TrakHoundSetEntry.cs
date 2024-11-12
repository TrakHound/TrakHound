// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Text.Json.Serialization;
using TrakHound.Entities;
using TrakHound.Serialization;
using YamlDotNet.Core.Tokens;

namespace TrakHound.Requests
{
    public class TrakHoundSetEntry : TrakHoundObjectsEntryBase
    {
        private byte[] _entryId;
        [JsonIgnore]
        public override byte[] EntryId
        {
            get
            {
                if (_entryId == null)
                {
                    if (!Values.IsNullOrEmpty())
                    {
                        var values = string.Join(':', Values);
                        _entryId = TrakHoundUuid.Create(ObjectPath, TrakHoundObjectsEntityClassName.Set, values);
                    }
                    else
                    {
                        _entryId = TrakHoundUuid.Create(ObjectPath, TrakHoundObjectsEntityClassName.Set);
                    }
                }

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

        [JsonPropertyName("values")]
        public IEnumerable<string> Values { get; set; }

        [JsonPropertyName("entryType")]
        public TrakHoundEntryType EntryType { get; set; }


        public TrakHoundSetEntry() 
        {
            Category = TrakHoundEntityCategoryId.Objects;
            Class = TrakHoundObjectsEntityClassId.Set;
        }

        public TrakHoundSetEntry(string objectPath, string value)
        {
            Category = TrakHoundEntityCategoryId.Objects;
            Class = TrakHoundObjectsEntityClassId.Set;

            ObjectPath = objectPath;

            if (value != null)
            {
                Values = new string[] { value };
            }
        }

        public TrakHoundSetEntry(string objectPath, IEnumerable<string> values)
        {
            Category = TrakHoundEntityCategoryId.Objects;
            Class = TrakHoundObjectsEntityClassId.Set;

            ObjectPath = objectPath;
            Values = values;
        }
    }
}
