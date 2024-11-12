// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text.Json.Serialization;

namespace TrakHound.Packages
{
    public class TrakHoundPackagePublishDestination
    {
        [JsonPropertyName("managementServer")]
        public string ManagementServer { get; set; }

        [JsonPropertyName("organization")]
        public string Organization { get; set; }

        [JsonPropertyName("apiKey")]
        public string ApiKey { get; set; }
    }
}
