// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace TrakHound.Requests
{
    public class TrakHoundDefinition
    {
        [JsonPropertyName("uuid")]
        public string Uuid { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("parentUuid")]
        public string ParentUuid { get; set; }

        [JsonPropertyName("sourceUuid")]
        public string SourceUuid { get; set; }

        [JsonPropertyName("created")]
        public DateTime Created { get; set; }

        [JsonPropertyName("metadata")]
        public IEnumerable<TrakHoundDefinitionMetadata> Metadata { get; set; }

        [JsonPropertyName("descriptions")]
        public IEnumerable<TrakHoundDefinitionDescription> Descriptions { get; set; }


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
            return Metadata?.FirstOrDefault(o => o.Name == name)?.Value;
        }

        public T GetMetadata<T>(string name)
        {
            var value = GetMetadata(name);
            if (value != null)
            {
                try
                {
                    return (T)Convert.ChangeType(value.ToString(), typeof(T));
                }
                catch { }
            }

            return default;
        }


        public bool DescriptionExists(string languageCode)
        {
            if (!string.IsNullOrEmpty(languageCode) && !Descriptions.IsNullOrEmpty())
            {
                return Descriptions.Any(o => o.LanguageCode == languageCode);
            }

            return false;
        }

        public string GetDescription(string languageCode)
        {
            return Descriptions?.FirstOrDefault(o => o.LanguageCode == languageCode)?.Text;
        }
    }
}
