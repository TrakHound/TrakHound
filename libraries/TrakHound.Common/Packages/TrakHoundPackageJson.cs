// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TrakHound.Packages
{
    public class TrakHoundPackageJson
    {
        [JsonPropertyName("uuid")]
        public string Uuid { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("version")]
        public string Version { get; set; }

        [JsonPropertyName("category")]
        public string Category { get; set; }

        [JsonPropertyName("buildDate")]
        public DateTime BuildDate { get; set; }

        [JsonPropertyName("hash")]
        public string Hash { get; set; }

        [JsonPropertyName("metadata")]
        public Dictionary<string, object> Metadata { get; set; }

        [JsonPropertyName("dependencies")]
        public IEnumerable<TrakHoundPackageDependency> Dependencies { get; set; }


        public TrakHoundPackageJson() { }

        public TrakHoundPackageJson(TrakHoundPackage package)
        {
            if (package != null)
            {
                Uuid = package.Uuid;
                Id = package.Id;
                Version = package.Version;
                Category = package.Category;
                BuildDate = package.BuildDate;
                Hash = package.Hash;
                Metadata = package.Metadata;

                if (!package.Dependencies.IsNullOrEmpty())
                {
                    var dependencies = new List<TrakHoundPackageDependency>();
                    foreach (var dependency in package.Dependencies)
                    {
                        dependencies.Add((TrakHoundPackageDependency)dependency);
                    }

                    Dependencies = dependencies;             
                }
            }
        }


        public TrakHoundPackage ToPackage()
        {
            var builder = new TrakHoundPackageBuilder();
            builder.Id = Id;
            builder.Version = Version;
            builder.Category = Category;
            builder.BuildDate = BuildDate;
            builder.Metadata = Metadata;

            if (!Dependencies.IsNullOrEmpty())
            {
                var dependencies = new List<ITrakHoundPackageDependency>();

                foreach (var dependency in Dependencies)
                {
                    dependencies.Add(dependency);
                }

                builder.Dependencies = dependencies;
            }

            return builder.Build();
        }
    }
}
