// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text.Json.Serialization;

namespace TrakHound.Packages
{
    public class TrakHoundPackageQuery
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("installed")]
        public string Installed { get; set; }

        [JsonPropertyName("update")]
        public string Update { get; set; }


        public TrakHoundPackageQuery() { }

        public TrakHoundPackageQuery(string id, string installedVersion, string updateVersion = null)
        {
            Id = id;
            Installed = installedVersion;
            Update = updateVersion;
        }
    }
}
