// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using TrakHound.Management;

namespace TrakHound.Packages
{
    public class TrakHoundPackageManager : ITrakHoundPackageManager, IDisposable
    {
        private const int _fileWatcherBufferSize = 8192 * 100; // 8192 (8 kb) is default
        private const int _notifyInterval = 2000;
        private const int _updateInterval = 10000;

        private readonly string _path;
        private readonly TrakHoundManagementClient _client;
        private readonly List<TrakHoundPackagesFileItem> _packageFileItems = new List<TrakHoundPackagesFileItem>();
        private readonly Dictionary<string, TrakHoundPackageUpdateConfiguration> _updateConfigurations = new Dictionary<string, TrakHoundPackageUpdateConfiguration>(); // Sender => UpdateConfiguration
        private readonly Dictionary<string, TrakHoundPackage> _installedPackages = new Dictionary<string, TrakHoundPackage>(); // PackageUuid => Package
        private readonly ListDictionary<string, string> _installedPackageUuidsByCategory = new ListDictionary<string, string>(); // Category => PackageUuid
        private readonly ListDictionary<string, string> _installedPackageIdsByCategory = new ListDictionary<string, string>(); // Category => PackageId
        private readonly ListDictionary<string, string> _installedPackageUuidsById = new ListDictionary<string, string>(); // PackageId => PackageUuid
        private readonly ListDictionary<string, string> _installedPackageVersionsById = new ListDictionary<string, string>(); // PackageId => PackageVersion
        private readonly Dictionary<string, string> _latestPackages = new Dictionary<string, string>(); // PackageId => PackageUuid
        private readonly Dictionary<string, TrakHoundPackage> _addedPackages = new Dictionary<string, TrakHoundPackage>(); // PackageUuid => Package
        private readonly object _lock = new object();
        private Timer _notifyTimer;
        private Timer _updateTimer;
        private FileSystemWatcher _fileSystemWatcher;


        public string Path => _path;

        public IEnumerable<TrakHoundPackage> Packages => _installedPackages.Values;

        public IEnumerable<string> PackageIds => _installedPackages.Values.Select(o => o.Id).Distinct();


        public event EventHandler<TrakHoundPackage> PackageAdded;

        public event EventHandler<TrakHoundPackageChangedArgs> PackageRemoved;


        public TrakHoundPackageManager()
        {
            Init();
        }

        public TrakHoundPackageManager(string path)
        {
            _path = path;

            Init();
        }

        public TrakHoundPackageManager(string organization, string packageServer)
        {
            _client = new TrakHoundManagementClient(packageServer);

            Init();
        }

        public TrakHoundPackageManager(string organization, string packageServer, string path)
        {
            _client = new TrakHoundManagementClient(packageServer);
            _path = path;

            Init();
        }

        public TrakHoundPackageManager(string organization, TrakHoundManagementClient managementClient)
        {
            _client = managementClient;

            Init();
        }

        public TrakHoundPackageManager(string organization, TrakHoundManagementClient managementClient, string path)
        {
            _client = managementClient;
            _path = path;

            Init();
        }

        public void Init()
        {
            _notifyTimer = new Timer();
            _notifyTimer.Interval = _notifyInterval;
            _notifyTimer.Elapsed += NotifyTimerElapsed;
            _notifyTimer.Enabled = true;

            if (_client != null)
            {
                _updateTimer = new Timer();
                _updateTimer.Interval = _updateInterval;
                _updateTimer.Elapsed += UpdateTimerElapsed;
                _updateTimer.Enabled = true;
            }

            var dir = TrakHoundPackage.GetDirectory(_path);
            try
            {
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            }
            catch { }

            _fileSystemWatcher = new FileSystemWatcher(dir, TrakHoundPackage.InformationFilename);
            _fileSystemWatcher.IncludeSubdirectories = true;
            _fileSystemWatcher.InternalBufferSize = _fileWatcherBufferSize;
            _fileSystemWatcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.CreationTime | NotifyFilters.Attributes;
            _fileSystemWatcher.Created += FileSystemWatcherCreated;
            _fileSystemWatcher.Changed += FileSystemWatcherCreated;
            _fileSystemWatcher.Renamed += FileSystemWatcherDeleted;
            _fileSystemWatcher.Deleted += FileSystemWatcherDeleted;
            _fileSystemWatcher.Error += FileSystemWatcherError;
            _fileSystemWatcher.EnableRaisingEvents = true;
        }

        public void Dispose()
        {
            if (_notifyTimer != null) _notifyTimer.Dispose();
            if (_updateTimer != null) _updateTimer.Dispose();
            if (_fileSystemWatcher != null) _fileSystemWatcher.Dispose();
        }


        public void Load()
        {
            _notifyTimer.Enabled = false;
            if (_updateTimer != null) _updateTimer.Enabled = false;
            _fileSystemWatcher.EnableRaisingEvents = false;

            _packageFileItems.Clear();
            _installedPackages.Clear();

            // Read "trakhound-packages.json"
            var packagesFile = TrakHoundPackagesFile.Read(_path);
            if (packagesFile != null)
            {
                if (!packagesFile.Packages.IsNullOrEmpty())
                {
                    _packageFileItems.AddRange(packagesFile.Packages);
                }
            }

            // Read Installed Packages
            var installedPackages = TrakHoundPackage.GetInstalled(_path);
            if (!installedPackages.IsNullOrEmpty())
            {
                foreach (var installedPackage in installedPackages)
                {
                    _installedPackages.Remove(installedPackage.Uuid);
                    _installedPackages.Add(installedPackage.Uuid, installedPackage);

                    _installedPackageUuidsByCategory.Add(installedPackage.Category, installedPackage.Uuid);
                    _installedPackageIdsByCategory.Add(installedPackage.Category, installedPackage.Id);
                    _installedPackageUuidsById.Add(installedPackage.Id, installedPackage.Uuid);
                    _installedPackageVersionsById.Add(installedPackage.Id, installedPackage.Version);

                    // Update Latest Package
                    var latestPackageUuid = _latestPackages.GetValueOrDefault(installedPackage.Id);
                    if (latestPackageUuid != null)
                    {
                        var existingPackage = _installedPackages.GetValueOrDefault(latestPackageUuid);
                        if (existingPackage == null || installedPackage.Version.ToVersion() > existingPackage.Version.ToVersion())
                        {
                            _latestPackages.Remove(installedPackage.Id);
                            _latestPackages.Add(installedPackage.Id, installedPackage.Uuid);
                        }
                    }
                    else
                    {
                        _latestPackages.Add(installedPackage.Id, installedPackage.Uuid);
                    }

                    if (PackageAdded != null) PackageAdded.Invoke(this, installedPackage);
                }
            }

            _notifyTimer.Enabled = true;
            if (_updateTimer != null) _updateTimer.Enabled = true;
            _fileSystemWatcher.EnableRaisingEvents = true;
        }


        #region "File Watcher"

        private void FileSystemWatcherCreated(object sender, FileSystemEventArgs e)
        {
            var package = TrakHoundPackage.ReadInformationFromFile(e.FullPath);
            if (package != null && package.Uuid != null)
            {
                lock (_lock)
                {
                    if (!_addedPackages.ContainsKey(package.Uuid))
                    {
                        _addedPackages.Add(package.Uuid, package);
                    }
                }
            }
        }

        private void FileSystemWatcherDeleted(object sender, FileSystemEventArgs e)
        {
            var versionPath = System.IO.Path.GetDirectoryName(e.FullPath);
            var version = System.IO.Path.GetFileName(versionPath);

            var packagePath = System.IO.Path.GetDirectoryName(versionPath);
            var packageId = System.IO.Path.GetFileName(packagePath);

            var categoryPath = System.IO.Path.GetDirectoryName(packagePath);
            var category = System.IO.Path.GetFileName(categoryPath);

            if (PackageRemoved != null) PackageRemoved.Invoke(this, new TrakHoundPackageChangedArgs(category, packageId, version));
        }

        private void NotifyTimerElapsed(object sender, ElapsedEventArgs e)
        {
            var addedPackages = new List<TrakHoundPackage>();

            lock (_lock)
            {
                foreach (var package in _addedPackages)
                {
                    addedPackages.Add(package.Value);
                }

                _addedPackages.Clear();
            }

            foreach (var package in addedPackages)
            {
                _installedPackages.Remove(package.Uuid);
                _installedPackages.Add(package.Uuid, package);

                if (PackageAdded != null) PackageAdded.Invoke(this, package);
            }
        }

        private void FileSystemWatcherError(object sender, ErrorEventArgs e)
        {
            Console.WriteLine("ERROR :: " + e.GetException().Message);
        }

        #endregion

        #region "Updates"

        public void AddUpdateConfiguration(string sender, string packageId, string packageVersion)
        {
            if (!string.IsNullOrEmpty(packageId) && !string.IsNullOrEmpty(packageVersion))
            {
                var configuration = new TrakHoundPackageUpdateConfiguration();
                configuration.Id = packageId;
                configuration.Version = packageVersion;

                lock (_lock)
                {
                    _updateConfigurations.Remove(sender);
                    _updateConfigurations.Add(sender, configuration);
                }
            }
        }

        public void RemoveUpdateConfiguration(string sender)
        {
            if (!string.IsNullOrEmpty(sender))
            {
                lock (_lock)
                {
                    _updateConfigurations.Remove(sender);
                }
            }
        }


        private async void UpdateTimerElapsed(object sender, ElapsedEventArgs e)
        {
            IEnumerable<TrakHoundPackageUpdateConfiguration> updateConfigurations;
            lock (_lock) updateConfigurations = _updateConfigurations.Values.ToList();

            if (!updateConfigurations.IsNullOrEmpty())
            {
                var queries = new List<TrakHoundPackageQuery>();
                foreach (var updateConfiguration in updateConfigurations)
                {
                    if (updateConfiguration.Id != null)
                    {
                        var latestPackageUuid = _latestPackages.GetValueOrDefault(updateConfiguration.Id);
                        if (latestPackageUuid != null)
                        {
                            var installedPackage = _installedPackages.GetValueOrDefault(latestPackageUuid);
                            if (installedPackage != null)
                            {
                                if (!queries.Any(o => o.Id == updateConfiguration.Id))
                                {
                                    queries.Add(new TrakHoundPackageQuery(updateConfiguration.Id, installedPackage.Version, updateConfiguration.Version));
                                }
                            }
                        }
                    }
                }

                if (!queries.IsNullOrEmpty())
                {
                    var updatedPackages = await _client.Packages.QueryUpdates(queries);
                    if (!updatedPackages.IsNullOrEmpty())
                    {
                        queries.Clear();
                        foreach (var updatedPackage in updatedPackages)
                        {
                            queries.Add(new TrakHoundPackageQuery(updatedPackage.Id, updatedPackage.Version));
                        }

                        var packageArchiveBytes = await _client.Packages.DownloadArchive(queries);
                        await InstallArchive(packageArchiveBytes);

                        //foreach (var package in updatedPackages)
                        //{
                        //    var packageBytes = await _client.Packages.Download(_organization, package.Id, package.Version);
                        //    await Install(packageBytes);
                        //}
                    }
                }
            }
        }

        //private async void UpdateTimerElapsed(object sender, ElapsedEventArgs e)
        //{
        //    IEnumerable<TrakHoundPackageUpdateConfiguration> updateConfigurations;
        //    IEnumerable<TrakHoundPackage> installedPackages;
        //    lock (_lock)
        //    {
        //        updateConfigurations = _updateConfigurations.Values.ToList();
        //        installedPackages = _installedPackages.ToList();
        //    }

        //    if (!updateConfigurations.IsNullOrEmpty() && !installedPackages.IsNullOrEmpty())
        //    {
        //        var queries = new List<TrakHoundPackageQuery>();
        //        foreach (var updateConfiguration in updateConfigurations)
        //        {
        //            var installedPackage = installedPackages.Where(o => o.Id == updateConfiguration.Id)?.OrderByDescending(o => o.Version.ToVersion()).FirstOrDefault();
        //            if (installedPackage != null)
        //            {
        //                if (!queries.Any(o => o.Id == updateConfiguration.Id))
        //                {
        //                    queries.Add(new TrakHoundPackageQuery(updateConfiguration.Id, installedPackage.Version, updateConfiguration.Version));
        //                }
        //            }
        //        }

        //        if (!queries.IsNullOrEmpty())
        //        {
        //            var updatedPackages = await _client.Packages.QueryUpdates(queries);
        //            if (!updatedPackages.IsNullOrEmpty())
        //            {
        //                queries.Clear();
        //                foreach (var updatedPackage in updatedPackages)
        //                {
        //                    queries.Add(new TrakHoundPackageQuery(updatedPackage.Id, updatedPackage.Version));
        //                }

        //                var packageArchiveBytes = await _client.Packages.DownloadArchive(queries);
        //                await InstallArchive(packageArchiveBytes);

        //                //foreach (var package in updatedPackages)
        //                //{
        //                //    var packageBytes = await _client.Packages.Download(_organization, package.Id, package.Version);
        //                //    await Install(packageBytes);
        //                //}
        //            }
        //        }
        //    }
        //}

        #endregion


        public IEnumerable<string> GetIds(string packageCategory)
        {
            return _installedPackageIdsByCategory.Get(packageCategory);
        }

        public IEnumerable<TrakHoundPackage> GetByCategory(string packageCategory)
        {
            if (!string.IsNullOrEmpty(packageCategory))
            {
                var packageUuids = _installedPackageUuidsByCategory.Get(packageCategory);
                if (!packageUuids.IsNullOrEmpty())
                {
                    var packages = new List<TrakHoundPackage>();
                    foreach (var packageUuid in packageUuids)
                    {
                        var package = _installedPackages.GetValueOrDefault(packageUuid);
                        if (package != null)
                        {
                            packages.Add(package);
                        }
                    }
                    return packages;
                }
            }

            return null;
        }

        public IEnumerable<TrakHoundPackage> GetById(string packageId)
        {
            if (!string.IsNullOrEmpty(packageId))
            {
                var packageUuids = _installedPackageUuidsById.Get(packageId);
                if (!packageUuids.IsNullOrEmpty())
                {
                    var packages = new List<TrakHoundPackage>();
                    foreach (var packageUuid in packageUuids)
                    {
                        var package = _installedPackages.GetValueOrDefault(packageUuid);
                        if (package != null)
                        {
                            packages.Add(package);
                        }
                    }
                    return packages;
                }
            }

            return null;
        }

        public TrakHoundPackage Get(string packageId, string packageVersion = null)
        {
            if (!string.IsNullOrEmpty(packageVersion))
            {
                var packageUuid = TrakHoundPackage.GenerateUuid(packageId, packageVersion);
                if (packageUuid != null)
                {
                    return _installedPackages.GetValueOrDefault(packageUuid);
                }
            }
            else
            {
                return GetLatest(packageId);
            }

            return null;
        }

        public IEnumerable<TrakHoundPackage> GetLatestByCategory(string packageCategory)
        {
            var categoryPackageIds = _installedPackageIdsByCategory.Get(packageCategory);
            if (!categoryPackageIds.IsNullOrEmpty())
            {
                var latestPackages = new List<TrakHoundPackage>();

                foreach (var packageId in categoryPackageIds)
                {
                    var latestUuid = _latestPackages.GetValueOrDefault(packageId);
                    if (latestUuid != null)
                    {
                        var package = _installedPackages.GetValueOrDefault(latestUuid);
                        if (package != null)
                        {
                            latestPackages.Add(package);
                        }
                    }
                }

                return latestPackages;
            }

            return null;
        }

        public TrakHoundPackage GetLatest(string packageId)
        {
            if (!string.IsNullOrEmpty(packageId))
            {
                var latestUuid = _latestPackages.GetValueOrDefault(packageId);
                if (latestUuid != null)
                {
                    return _installedPackages.GetValueOrDefault(latestUuid);
                }
            }

            return null;
        }


        public IEnumerable<string> GetVersions(string packageId)
        {
            return _installedPackageVersionsById.Get(packageId);
        }

        public string GetReadMe(string packageCategory, string packageId, string packageVersion = null)
        {
            string packageDirectory;
            if (!string.IsNullOrEmpty(packageVersion)) packageDirectory = TrakHoundPackage.GetPackageVersionDirectory(packageCategory, packageId, packageVersion);
            else packageDirectory = TrakHoundPackage.GetLatestPackageDirectory(packageCategory, packageId);

            if (!string.IsNullOrEmpty(packageDirectory))
            {
                var filePaths = new List<string>();
                foreach (var extension in TrakHoundPackage.ReadMeFileExtensions)
                {
                    filePaths.Add(System.IO.Path.Combine(packageDirectory, $"{TrakHoundPackage.ReadMeFilename}{extension}"));
                }

                foreach (var filePath in filePaths)
                {
                    try
                    {
                        if (File.Exists(filePath))
                        {
                            var file = File.ReadAllText(filePath);
                            if (file != null) return file;
                        }
                    }
                    catch { }
                }
            }

            return null;
        }


        public async Task<IEnumerable<TrakHoundPackageInstallResult>> Install(string packageUuid, TrakHoundManagementClient managementClient = null)
        {
            var results = new List<TrakHoundPackageInstallResult>();

            var client = managementClient != null ? managementClient : _client;
            if (client != null && !string.IsNullOrEmpty(packageUuid))
            {
                var packageBytes = await client.Packages.Download(packageUuid);
                return await Install(packageBytes, managementClient: managementClient);
            }

            return results;
        }

        public async Task<IEnumerable<TrakHoundPackageInstallResult>> Install(string packageId, string packageVersion = null, TrakHoundManagementClient managementClient = null)
        {
            var results = new List<TrakHoundPackageInstallResult>();

            var client = managementClient != null ? managementClient : _client;
            if (client != null && !string.IsNullOrEmpty(packageId))
            {
                var packageBytes = await client.Packages.Download(packageId, packageVersion);
                return await Install(packageBytes, packageVersion, managementClient);
            }

            return results;
        }

        public async Task<IEnumerable<TrakHoundPackageInstallResult>> Install(byte[] packageBytes, string version = null, TrakHoundManagementClient managementClient = null)
        {
            var results = new List<TrakHoundPackageInstallResult>();

            if (packageBytes != null && packageBytes.Length > 0)
            {
                var result = new TrakHoundPackageInstallResult();

                // Read Information from Package
                var information = TrakHoundPackage.ReadInformationFromPackage(packageBytes);
                if (information != null && !string.IsNullOrEmpty(information.Uuid) && !string.IsNullOrEmpty(information.Category) && !string.IsNullOrEmpty(information.Id))
                {
                    var packageDirectory = TrakHoundPackage.GetDirectory(_path);
                    var packagePath = System.IO.Path.Combine(packageDirectory, information.Category, information.Id, information.Version);

                    information.Location = packagePath;

                    if (!information.Dependencies.IsNullOrEmpty())
                    {
                        foreach (var dependency in information.Dependencies)
                        {
                            results.AddRange(await Install(dependency.Id, dependency.Version, managementClient));
                        }
                    }

                    if (TrakHoundPackage.Install(packageBytes))
                    {
                        _installedPackages.Remove(information.Uuid);
                        _installedPackages.Add(information.Uuid, information);

                        _installedPackageUuidsByCategory.Add(information.Category, information.Uuid);
                        _installedPackageIdsByCategory.Add(information.Category, information.Id);
                        _installedPackageUuidsById.Add(information.Id, information.Uuid);
                        _installedPackageVersionsById.Add(information.Id, information.Version);

                        // Update Latest Package
                        var latestPackageUuid = _latestPackages.GetValueOrDefault(information.Id);
                        if (latestPackageUuid != null)
                        {
                            var existingPackage = _installedPackages.GetValueOrDefault(latestPackageUuid);
                            if (existingPackage == null || information.Version.ToVersion() > existingPackage.Version.ToVersion())
                            {
                                _latestPackages.Remove(information.Id);
                                _latestPackages.Add(information.Id, information.Uuid);
                            }
                        }
                        else
                        {
                            _latestPackages.Add(information.Id, information.Uuid);
                        }


                        UpdatePackagesFile(version);

                        result.Success = true;
                        result.PackageId = information.Id;
                        result.PackageVersion = information.Version;
                    }
                }

                results.Add(result);
            }

            return results;
        }

        public async Task<IEnumerable<TrakHoundPackageInstallResult>> InstallArchive(byte[] packageArchiveBytes, TrakHoundManagementClient managementClient = null)
        {
            var results = new List<TrakHoundPackageInstallResult>();

            if (!packageArchiveBytes.IsNullOrEmpty())
            {
                try
                {
                    var tempDir = TrakHoundTemp.CreateDirectory();
                    Files.Unzip(packageArchiveBytes, tempDir);

                    foreach (var file in Directory.GetFiles(tempDir))
                    {
                        var packageBytes = File.ReadAllBytes(file);
                        var packageResults = await Install(packageBytes);
                        results.AddRange(packageResults);
                    }
                }
                catch { }
            }

            return results;
        }

        public async Task<IEnumerable<TrakHoundPackageInstallResult>> InstallArchive(string packageArchivePath, TrakHoundManagementClient managementClient = null)
        {
            var results = new List<TrakHoundPackageInstallResult>();

            if (!string.IsNullOrEmpty(packageArchivePath))
            {
                try
                {
                    var packageArchiveBytes = File.ReadAllBytes(packageArchivePath);
                    if (packageArchiveBytes != null)
                    {
                        var tempDir = TrakHoundTemp.CreateDirectory();
                        Files.Unzip(packageArchiveBytes, tempDir);

                        foreach (var file in Directory.GetFiles(tempDir))
                        {
                            var packageBytes = File.ReadAllBytes(file);
                            var packageResults = await Install(packageBytes, managementClient: managementClient);
                            results.AddRange(packageResults);
                        }
                    }
                }
                catch { }
            }

            return results;
        }

        public bool Uninstall(string packageId, string packageVersion)
        {
            var packageUuid = TrakHoundPackage.GenerateUuid(packageId, packageVersion);
            if (packageUuid != null)
            {
                var package = _installedPackages.GetValueOrDefault(packageUuid);
                if (package != null)
                {
                    TrakHoundPackage.Uninstall(packageId, packageVersion, _path);

                    _installedPackageUuidsByCategory.Remove(package.Category, packageUuid);
                    _installedPackageIdsByCategory.Remove(package.Category, package.Id);
                    _installedPackageUuidsById.Remove(package.Id, packageUuid);
                    _installedPackageVersionsById.Remove(package.Id, packageVersion);

                    // Reset Latest Package
                    var latestUuid = _latestPackages.GetValueOrDefault(package.Id);
                    if (latestUuid != null && latestUuid == package.Uuid)
                    {
                        _latestPackages.Remove(package.Id);

                        var packageVersions = _installedPackageVersionsById.Get(package.Id);
                        if (!packageVersions.IsNullOrEmpty())
                        {
                            var latestVersion = packageVersions.OrderByDescending(o => o.ToVersion()).FirstOrDefault();
                            latestUuid = TrakHoundPackage.GenerateUuid(packageId, latestVersion);
                            if (latestUuid != null)
                            {
                                _latestPackages.Add(packageId, latestVersion);
                            }
                        }
                    }

                    _installedPackages.Remove(packageUuid);

                    UpdatePackagesFile();

                    return true;
                }
            }

            return false;
        }

        //public bool Uninstall(string packageId, string packageVersion)
        //{
        //    if (_installedPackages.Exists(o => o.Id == packageId && o.Version == packageVersion))
        //    {
        //        if (TrakHoundPackage.Uninstall(packageId, packageVersion))
        //        {
        //            _installedPackages.RemoveAll(o => o.Id == packageId && o.Version == packageVersion);

        //            UpdatePackagesFile();

        //            return true;
        //        }
        //    }

        //    return false;
        //}


        private bool UpdatePackagesFile(string version = null)
        {
            if (!_installedPackages.IsNullOrEmpty())
            {
                var items = new List<TrakHoundPackagesFileItem>();
                foreach (var package in _installedPackages.Values)
                {
                    var item = new TrakHoundPackagesFileItem();
                    item.Id = package.Id;
                    item.Version = !string.IsNullOrEmpty(version) ? version : package.Version;
                    item.Category = package.Category;
                    item.Installed = package.Version;
                    items.Add(item);
                }

                return TrakHoundPackagesFile.Write(items);
            }
            else
            {
                return true;
            }
        }
    }
}
