// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TrakHound.Api
{
    public class TrakHoundApiEndpointInformation
    {
        [JsonIgnore]
        public TrakHoundApiInformation Api { get; set; }

        [JsonIgnore]
        public TrakHoundApiControllerInformation Controller { get; set; }


        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("group")]
        public string Group { get; set; }

        [JsonPropertyName("route")]
        public string Route { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("parameters")]
        public IEnumerable<TrakHoundApiParameterInformation> Parameters { get; set; }

        [JsonPropertyName("responses")]
        public IEnumerable<TrakHoundApiResponseInformation> Responses { get; set; }

        [JsonPropertyName("permissions")]
        public IEnumerable<string> Permissions { get; set; }
    }
}
