// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text.Json.Serialization;

namespace TrakHound.Deployments
{
    public class TrakHoundDeploymentConfigurationManifestItem
    {
        [JsonPropertyName("category")]
        public string Category { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("lastUpdated")]
        public long LastUpdated { get; set; }
    }
}
