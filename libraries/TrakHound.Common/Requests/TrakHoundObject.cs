// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace TrakHound.Requests
{
    public class TrakHoundObject
    {
        [JsonPropertyName("path")]
        public string Path { get; set; }

        [JsonPropertyName("uuid")]
        public string Uuid { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("namespace")]
        public string Namespace { get; set; }

        [JsonPropertyName("contentType")]
        public string ContentType { get; set; }

        [JsonPropertyName("definitionUuid")]
        public string DefinitionUuid { get; set; }

        [JsonPropertyName("definitionId")]
        public string DefinitionId { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("parentUuid")]
        public string ParentUuid { get; set; }

        [JsonPropertyName("sourceUuid")]
        public string SourceUuid { get; set; }

        [JsonPropertyName("created")]
        public DateTime Created { get; set; }

        [JsonPropertyName("metadata")]
        public IEnumerable<TrakHoundMetadata> Metadata { get; set; }


        public bool MetadataExists(string name)
        {
            if (!string.IsNullOrEmpty(name) && !Metadata.IsNullOrEmpty())
            {
                return Metadata.Any(o => o.Name == name);
            }

            return false;
        }

        public string GetMetadata(string name)
        {
            if (!string.IsNullOrEmpty(name) && !Metadata.IsNullOrEmpty())
            {
                var value = Metadata.FirstOrDefault(o => o.Name == name)?.Value;
                if (value != null)
                {
                    return value.ToString();
                }
            }

            return null;
        }

        public T GetMetadata<T>(string name)
        {
            if (!string.IsNullOrEmpty(name) && !Metadata.IsNullOrEmpty())
            {
                var value = Metadata.FirstOrDefault(o => o.Name == name)?.Value;
                if (value != null)
                {
                    try
                    {
                        return (T)Convert.ChangeType(value, typeof(T));
                    }
                    catch { }
                }
            }

            return default;
        }
    }
}
