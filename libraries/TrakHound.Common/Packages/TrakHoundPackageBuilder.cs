// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;

namespace TrakHound.Packages
{
    public class TrakHoundPackageBuilder
    {
        public string Id { get; set; }

        public string Version { get; set; }

        public string Category { get; set; }

        public string Location { get; set; }

        public DateTime BuildDate { get; set; }


        private readonly Dictionary<string, object> _metadata = new Dictionary<string, object>();
        public Dictionary<string, object> Metadata
        {
            get => _metadata;
            set
            {
                if (!value.IsNullOrEmpty())
                {
                    foreach (var entry in value)
                    {
                        _metadata.Remove(entry.Key);
                        _metadata.Add(entry.Key, entry.Value);
                    }
                }
            }
        }


        public IEnumerable<ITrakHoundPackageDependency> Dependencies { get; set; }


        public TrakHoundPackageBuilder() { }

        public TrakHoundPackageBuilder(TrakHoundPackage package)
        {
            if (package != null)
            {
                Id = package.Id;
                Category = package.Category;
                Version = package.Version;
                BuildDate = package.BuildDate;
                Metadata = package.Metadata;
                Dependencies = package.Dependencies;
            }
        }


        public TrakHoundPackage Build()
        {
            return new TrakHoundPackage(this);
        }
    }
}
