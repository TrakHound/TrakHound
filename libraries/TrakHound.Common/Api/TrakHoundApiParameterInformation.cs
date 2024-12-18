// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text.Json.Serialization;

namespace TrakHound.Api
{
    public class TrakHoundApiParameterInformation
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("dataType")]
        public string DataType { get; set; }

        [JsonPropertyName("required")]
        public bool Required { get; set; }

        [JsonPropertyName("defaultValue")]
        public string DefaultValue { get; set; }
    }
}
