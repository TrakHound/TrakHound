// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TrakHound.Apps
{
    public class TrakHoundAppInformation
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("route")]
        public string Route { get; set; }

        [JsonPropertyName("routerId")]
        public string RouterId { get; set; }

        [JsonPropertyName("packageId")]
        public string PackageId { get; set; }

        [JsonPropertyName("packageVersion")]
        public string PackageVersion { get; set; }

        [JsonPropertyName("packageBuildDate")]
        public DateTime PackageBuildDate { get; set; }

        [JsonPropertyName("packageHash")]
        public string PackageHash { get; set; }

        [JsonPropertyName("packageIcon")]
        public string PackageIcon { get; set; }

        [JsonPropertyName("packageReadMe")]
        public string PackageReadMe { get; set; }

        [JsonPropertyName("repository")]
        public string Repository { get; set; }

        [JsonPropertyName("repositoryBranch")]
        public string RepositoryBranch { get; set; }

        [JsonPropertyName("repositoryDirectory")]
        public string RepositoryDirectory { get; set; }

        [JsonPropertyName("repositoryCommit")]
        public string RepositoryCommit { get; set; }

        [JsonPropertyName("trakhoundVersion")]
        public string TrakHoundVersion { get; set; }

        [JsonPropertyName("pages")]
        public IEnumerable<TrakHoundAppPageInformation> Pages { get; set; }
    }
}
