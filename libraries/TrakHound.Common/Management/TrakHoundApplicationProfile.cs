// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TrakHound.Management
{
    public class TrakHoundApplicationProfile
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        private readonly Dictionary<string, object> _metadata = new Dictionary<string, object>();
        [JsonPropertyName("metadata")]
        public Dictionary<string, object> Metadata
        {
            get => _metadata;
            set
            {
                if (!value.IsNullOrEmpty())
                {
                    foreach (var entry in value)
                    {
                        _metadata.Remove(entry.Key);
                        _metadata.Add(entry.Key, entry.Value);
                    }
                }
            }
        }
    }
}
