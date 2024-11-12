// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text.Json.Serialization;

namespace TrakHound.Packages
{
    public class TrakHoundPackageUpdateConfiguration
    {
        [JsonPropertyName("category")]
        public string Category { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("version")]
        public string Version { get; set; }


        public TrakHoundPackageUpdateConfiguration() { }

        public TrakHoundPackageUpdateConfiguration(string category, string id, string version = null)
        {
            Category = category;
            Id = id;
            Version = version;
        }
    }
}
