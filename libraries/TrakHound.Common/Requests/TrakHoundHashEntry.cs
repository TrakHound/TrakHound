// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Text.Json.Serialization;
using TrakHound.Entities;
using YamlDotNet.Core.Tokens;

namespace TrakHound.Requests
{
    public class TrakHoundHashEntry : TrakHoundObjectsEntryBase
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
                        var values = string.Join(':', Values.Keys);
                        _entryId = TrakHoundUuid.Create(ObjectPath, TrakHoundObjectsEntityClassName.Hash, values);
                    }
                    else
                    {
                        _entryId = TrakHoundUuid.Create(ObjectPath, TrakHoundObjectsEntityClassName.Hash);
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
        public Dictionary<string, string> Values { get; set; }


        public TrakHoundHashEntry() 
        {
            Category = TrakHoundEntityCategoryId.Objects;
            Class = TrakHoundObjectsEntityClassId.Hash;
        }

        public TrakHoundHashEntry(string objectPath, string key, object value)
        {
            Category = TrakHoundEntityCategoryId.Objects;
            Class = TrakHoundObjectsEntityClassId.Hash;

            ObjectPath = objectPath;

            if (!string.IsNullOrEmpty(key) && value != null)
            {
                Values = new Dictionary<string, string>();
                Values[key] = value.ToString();
            }
        }

        public TrakHoundHashEntry(string objectPath, IDictionary<string, string> values)
        {
            Category = TrakHoundEntityCategoryId.Objects;
            Class = TrakHoundObjectsEntityClassId.Hash;

            ObjectPath = objectPath;
            
            if (!values.IsNullOrEmpty())
            {
                var v = new Dictionary<string, string>();
                foreach (var value in values) v[value.Key] = value.Value;
                Values = v;
            }
        }

        public TrakHoundHashEntry(string objectPath, Dictionary<string, string> values)
        {
            Category = TrakHoundEntityCategoryId.Objects;
            Class = TrakHoundObjectsEntityClassId.Hash;

            ObjectPath = objectPath;
            Values = values;
        }
    }
}
