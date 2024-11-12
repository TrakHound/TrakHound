// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using TrakHound.Packages;

namespace TrakHound.Functions
{
    public class TrakHoundFunctionInformation
    {
        [JsonPropertyName("functionId")]
        public string FunctionId { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("configurationId")]
        public string ConfigurationId { get; set; }

        [JsonPropertyName("packageId")]
        public string PackageId { get; set; }

        [JsonPropertyName("packageVersion")]
        public string PackageVersion { get; set; }

        [JsonPropertyName("packageBuildDate")]
        public DateTime? PackageBuildDate { get; set; }

        [JsonPropertyName("packageUuid")]
        public string PackageUuid { get; set; }

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

        [JsonPropertyName("lastUpdated")]
        public DateTime LastUpdated { get; set; }

        [JsonPropertyName("averageDuration")]
        public TimeSpan AverageDuration { get; set; }

        [JsonPropertyName("runCount")]
        public long RunCount { get; set; }


        public static TrakHoundFunctionInformation Create(ITrakHoundFunctionConfiguration configuration, Type moduleType, TrakHoundPackage package, string packageReadme = null)
        {
            if (configuration != null && moduleType != null && package != null)
            {
                var information = new TrakHoundFunctionInformation();
                information.FunctionId = configuration.FunctionId;
                information.Description = !string.IsNullOrEmpty(configuration.Description) ? configuration.Description : package?.Metadata?.GetValueOrDefault("description")?.ToString();
                information.ConfigurationId = configuration.Id;
                information.PackageId = configuration.PackageId;

                if (package != null)
                {
                    information.PackageVersion = package.Version;
                    information.PackageBuildDate = package.BuildDate;
                    information.PackageUuid = package.Uuid;
                    information.PackageHash = package.Hash;
                    information.PackageIcon = package.GetMetadata(TrakHoundPackage.ImageName);

                    information.TrakHoundVersion = package.GetMetadata(TrakHoundPackage.TrakHoundVersionName);

                    information.Repository = package.GetMetadata(TrakHoundPackage.RepositoryName);
                    information.RepositoryBranch = package.GetMetadata(TrakHoundPackage.RepositoryBranchName);
                    information.RepositoryDirectory = package.GetMetadata(TrakHoundPackage.RepositoryDirectoryName);
                    information.RepositoryCommit = package.GetMetadata(TrakHoundPackage.RepositoryCommitName);
                }

                information.PackageReadMe = packageReadme;

                return information;
            }

            return null;
        }
    }
}
