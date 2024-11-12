// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Management;

namespace TrakHound.Packages
{
    public interface ITrakHoundPackageManager
    {
        IEnumerable<TrakHoundPackage> Packages { get; }


        event EventHandler<TrakHoundPackage> PackageAdded;

        event EventHandler<TrakHoundPackageChangedArgs> PackageRemoved;


        IEnumerable<string> GetIds(string packageCategory);

        IEnumerable<TrakHoundPackage> GetByCategory(string packageCategory);

        IEnumerable<TrakHoundPackage> GetById(string packageId);

        TrakHoundPackage Get(string packageId, string packageVersion = null);

        IEnumerable<TrakHoundPackage> GetLatestByCategory(string packageCategory);

        TrakHoundPackage GetLatest(string packageId);

        IEnumerable<string> GetVersions(string packageId);

        string GetReadMe(string packageCategory, string packageId, string packageVersion = null);


        Task<IEnumerable<TrakHoundPackageInstallResult>> Install(string packageUuid, TrakHoundManagementClient managementClient = null);

        Task<IEnumerable<TrakHoundPackageInstallResult>> Install(string packageId, string packageVersion = null, TrakHoundManagementClient managementClient = null);

        Task<IEnumerable<TrakHoundPackageInstallResult>> Install(byte[] packageBytes, string version = null, TrakHoundManagementClient managementClient = null);

        bool Uninstall(string packageId, string packageVersion);


        void AddUpdateConfiguration(string sender, string packageId, string packageVersion);

        void RemoveUpdateConfiguration(string sender);
    }
}
