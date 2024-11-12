// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Text.Json.Serialization;

namespace TrakHound.Requests
{
    public class TrakHoundMetadata
    {
        [JsonPropertyName("uuid")]
        public string Uuid { get; set; }

        [JsonPropertyName("entityUuid")]
        public string EntityUuid { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("definitionUuid")]
        public string DefinitionUuid { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }

        [JsonPropertyName("valueDefinitionUuid")]
        public string ValueDefinitionUuid { get; set; }

        [JsonPropertyName("valueType")]
        public string ValueType { get; set; }

        [JsonPropertyName("valueDescription")]
        public string ValueDescription { get; set; }

        [JsonPropertyName("sourceUuid")]
        public string SourceUuid { get; set; }

        [JsonPropertyName("created")]
        public DateTime Created { get; set; }
    }
}
