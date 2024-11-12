// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Configurations;
using TrakHound.Packages;

namespace TrakHound.Blazor
{
    public class InstanceConfigurationGroup
    {
        private readonly List<TrakHoundPackagesFileItem> _packages = new List<TrakHoundPackagesFileItem>();


        public string Id { get; set; }

        public ITrakHoundConfigurationProfile ConfigurationProfile { get; set; }

        public IEnumerable<TrakHoundPackagesFileItem> Packages => _packages;


        public event EventHandler<TrakHoundPackagesFileItem> PackageAdded;


        public void AddPackage(TrakHoundPackagesFileItem packageItem)
        {
            if (packageItem != null)
            {
                _packages.RemoveAll(o => o.Id == packageItem.Id && o.Version == packageItem.Version);
                _packages.Add(packageItem);

                if (PackageAdded != null) PackageAdded.Invoke(this, packageItem);
            }
        }

        public void AddPackage(TrakHoundPackage package)
        {
            if (package != null)
            {
                var packageItem = new TrakHoundPackagesFileItem(package.Category, package.Id, package.Version);

                _packages.RemoveAll(o => o.Id == package.Id && o.Version == package.Version);
                _packages.Add(packageItem);

                if (PackageAdded != null) PackageAdded.Invoke(this, packageItem);
            }
        }

        public void RemovePackage(string packageId, string packageVersion)
        {
            if (!string.IsNullOrEmpty(packageId) && !string.IsNullOrEmpty(packageVersion))
            {
                _packages.RemoveAll(o => o.Id == packageId && o.Version == packageVersion);
            }
        }
    }
}
