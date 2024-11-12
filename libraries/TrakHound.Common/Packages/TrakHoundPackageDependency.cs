// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text.Json.Serialization;

namespace TrakHound.Packages
{
    public class TrakHoundPackageDependency : ITrakHoundPackageDependency
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("version")]
        public string Version { get; set; }


        public TrakHoundPackageDependency() { }

        public TrakHoundPackageDependency(string id, string version)
        {
            Id = id;
            Version = version;
        }
    }
}
