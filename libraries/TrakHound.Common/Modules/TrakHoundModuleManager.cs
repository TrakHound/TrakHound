// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TrakHound.Packages;

namespace TrakHound.Modules
{
    public class TrakHoundModuleManager<TModule> : ITrakHoundModuleManager
    {
        public const string _directory = "_modules";
        private const string _defaultFileExtension = ".dll";

        private static Func<string, string> _defaultFilterFunction = (packageId) =>
        {
            return packageId + _defaultFileExtension;
        };

        private readonly TrakHoundModuleContext _context;
        private readonly bool _loadByDependencies;
        private readonly IEnumerable<string> _packageCategories;
        private readonly Func<string, string> _filterFunction;
        private readonly TrakHoundPackageManager _packageManager;
        private readonly Dictionary<string, TrakHoundModule> _modules = new Dictionary<string, TrakHoundModule>(); // ModuleKey => Module
        private readonly Dictionary<string, TrakHoundModule> _latestModules = new Dictionary<string, TrakHoundModule>(); // PackageId => Module
        private readonly ListDictionary<string, TrakHoundModule> _packageModules = new ListDictionary<string, TrakHoundModule>(); // PackageId => Module
        private readonly object _lock = new object();


        public IEnumerable<string> PackageCategories => _packageCategories;

        public Func<string, string> FilterFunction => _filterFunction;

        public IEnumerable<ITrakHoundModule> Modules => _modules.Values;

        public IEnumerable<Type> ModuleTypes => _modules.Values.SelectMany(o => o.ModuleTypes).ToList();

        public EventHandler<ITrakHoundModule> ModuleAdded { get; set; }

        public EventHandler<ITrakHoundModule> ModuleRemoved { get; set; }

        public EventHandler<Exception> ModuleLoadError { get; set; }


        public TrakHoundModuleManager(
            TrakHoundPackageManager packageManager,
            string packageCategory,
            Func<string, string> filterFunction = null, 
            TrakHoundModuleContext context = null,
            bool loadByDependencies = true
            )
        {
            _packageManager = packageManager;
            _packageManager.PackageAdded += PackageAdded;
            _packageManager.PackageRemoved += PackageRemoved;

            if (!string.IsNullOrEmpty(packageCategory)) _packageCategories = new string[] { packageCategory };
            _filterFunction = filterFunction != null ? filterFunction : _defaultFilterFunction;
            _context = context != null ? context : new TrakHoundModuleContext();
            _loadByDependencies = loadByDependencies;
        }

        public TrakHoundModuleManager(
            TrakHoundPackageManager packageManager,
            IEnumerable<string> packageCategories,
            Func<string, string> filterFunction = null,
            TrakHoundModuleContext context = null,
            bool loadByDependencies = true
            )
        {
            _packageManager = packageManager;
            _packageManager.PackageAdded += PackageAdded;
            _packageManager.PackageRemoved += PackageRemoved;

            _packageCategories = packageCategories;
            _filterFunction = filterFunction != null ? filterFunction : _defaultFilterFunction;
            _context = context != null ? context : new TrakHoundModuleContext();
            _loadByDependencies = loadByDependencies;
        }


        public ITrakHoundModule Get(string packageId, string packageVersion)
        {
            if (!string.IsNullOrEmpty(packageId) && !string.IsNullOrEmpty(packageVersion))
            {
                ITrakHoundModule module = null;

                if (packageVersion == "*")
                {
                    module = _latestModules.GetValueOrDefault(packageId);
                }
                else if (packageVersion.Contains('*'))
                {
                    var packageModules = _packageModules.Get(packageId);
                    if (!packageModules.IsNullOrEmpty())
                    {
                        var matched = packageModules.Where(o => StringFunctions.MatchVersion(o.PackageVersion, packageVersion));
                        if (!matched.IsNullOrEmpty())
                        {
                            var latestVersion = matched.Select(o => o.PackageVersion.ToVersion()).OrderByDescending(o => o).FirstOrDefault();
                            if (latestVersion != null)
                            {
                                module = matched.Where(o => o.PackageVersion == latestVersion.ToString())?.FirstOrDefault();
                            }
                        }
                    }
                }
                else
                {
                    var moduleKey = GetModuleKey(packageId, packageVersion);

                    module = _modules.GetValueOrDefault(moduleKey);
                }

                if (module == null)
                {
                    TrakHoundPackage package = null;

                    if (packageVersion == "*")
                    {
                        package = _packageManager.GetLatest(packageId);
                    }
                    else if (packageVersion.Contains('*'))
                    {
                        var versions = _packageManager.GetVersions(packageId);
                        if (!versions.IsNullOrEmpty())
                        {
                            var versionMatches = versions.Where(o => StringFunctions.MatchVersion(o, packageVersion));
                            if (!versionMatches.IsNullOrEmpty())
                            {
                                var latestVersion = versionMatches.Select(o => o.ToVersion()).OrderByDescending(o => o).FirstOrDefault();

                                package = _packageManager.Get(packageId, latestVersion.ToString());
                            }
                        }
                    }
                    else
                    {
                        package = _packageManager.Get(packageId, packageVersion);
                    }

                    if (package != null)
                    {
                        module = LoadModule(package);
                    }
                }

                return module;
            }

            return null;
        }

        public IEnumerable<Type> GetTypes(string packageId, string packageVersion)
        {
            if (!string.IsNullOrEmpty(packageId) && !string.IsNullOrEmpty(packageVersion))
            {
                var module = Get(packageId, packageVersion);
                if (module != null)
                {
                    return module.ModuleTypes;
                }
            }

            return Enumerable.Empty<Type>();
        }


        private void PackageAdded(object sender, TrakHoundPackage package)
        {
            if (package != null && !_packageCategories.IsNullOrEmpty() && _packageCategories.Contains(package.Category) && !string.IsNullOrEmpty(package.Id))
            {
                _latestModules.Remove(package.Id);
                _packageModules.Remove(package.Id);
            }
        }

        private void PackageRemoved(object sender, TrakHoundPackageChangedArgs args)
        {
            var moduleKey = GetModuleKey(args.PackageId, args.PackageVersion);

            var removeModule = _modules.GetValueOrDefault(moduleKey);
            if (removeModule != null)
            {
                UnloadModule(args.PackageCategory, args.PackageId, args.PackageVersion);
                _modules.Remove(moduleKey);

                if (ModuleRemoved != null) ModuleRemoved.Invoke(this, removeModule);
            }
        }


        private void ReadPackageModuleTypes()
        {
            if (!_packageCategories.IsNullOrEmpty())
            {
                foreach (var packageCategory in _packageCategories)
                {
                    var rootDirectory = TrakHoundPackage.GetCategoryDirectory(packageCategory);
                    if (Directory.Exists(rootDirectory))
                    {
                        var packageDirectories = Directory.GetDirectories(rootDirectory);
                        if (!packageDirectories.IsNullOrEmpty())
                        {
                            foreach (var packageDirectory in packageDirectories)
                            {
                                // Set Package ID
                                var packageId = Path.GetFileName(packageDirectory);

                                var packages = _packageManager.GetById(packageId);
                                if (!packages.IsNullOrEmpty())
                                {
                                    foreach (var package in packages)
                                    {
                                        LoadModules(package.Category, package.Id, package.Version);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }


        private TrakHoundModule LoadModule(TrakHoundPackage package)
        {
            if (package != null)
            {
                try
                {
                    var packageReadMe = TrakHoundPackage.GetReadMe(package.Category, package.Id, package.Version);

                    var packageDirectory = TrakHoundPackage.GetPackageVersionDirectory(package.Category, package.Id, package.Version);

                    var types = ReadPackageModuleTypes(package.Category, package.Id, package.Version);
                    if (!types.IsNullOrEmpty())
                    {
                        var module = new TrakHoundModule(package, packageDirectory, types, packageReadMe);

                        var moduleKey = GetModuleKey(package.Id, package.Version);

                        // Add to cache
                        _modules.Remove(moduleKey);
                        _modules.Add(moduleKey, module);

                        // Add to "By Package" cache
                        _packageModules.Add(module.PackageId, module);

                        // Update Latest Module
                        var latestModule = _latestModules.GetValueOrDefault(package.Id);
                        if (latestModule == null || module.PackageVersion.ToVersion() > latestModule.PackageVersion.ToVersion())
                        {
                            _latestModules.Remove(package.Id);
                            _latestModules.Add(package.Id, module);
                        }

                        if (ModuleAdded != null) ModuleAdded.Invoke(this, module);

                        return module;
                    }
                }
                catch (Exception ex)
                {
                    if (ModuleLoadError != null) ModuleLoadError.Invoke(this, ex);
                }
            }

            return null;
        }

        private IEnumerable<TrakHoundModule> LoadModules(string packageCategory, string packageId, string packageVersion)
        {
            if (!string.IsNullOrEmpty(packageCategory) && !string.IsNullOrEmpty(packageId) && !string.IsNullOrEmpty(packageVersion))
            {
                try
                {
                    var package = TrakHoundPackage.ReadInformation(packageCategory, packageId, packageVersion);

                    var packageReadMe = TrakHoundPackage.GetReadMe(packageCategory, packageId, packageVersion);

                    var packageDirectory = TrakHoundPackage.GetPackageVersionDirectory(packageCategory, packageId, packageVersion);

                    var types = ReadPackageModuleTypes(packageCategory, packageId, packageVersion);
                    if (!types.IsNullOrEmpty())
                    {
                        var modules = new List<TrakHoundModule>();

                        var module = new TrakHoundModule(package, packageDirectory, types, packageReadMe);
                        modules.Add(module);

                        var moduleKey = GetModuleKey(packageId, packageVersion);

                        // Add to cache
                        _modules.Remove(moduleKey);
                        _modules.Add(moduleKey, module);

                        // Add to "By Package" cache
                        _packageModules.Add(module.PackageId, module);

                        // Update Latest Module
                        var latestModule = _latestModules.GetValueOrDefault(packageId);
                        if (latestModule == null || module.PackageVersion.ToVersion() > latestModule.PackageVersion.ToVersion())
                        {
                            _latestModules.Remove(packageId);
                            _latestModules.Add(packageId, module);
                        }

                        if (ModuleAdded != null) ModuleAdded.Invoke(this, module);
                    }
                }
                catch (Exception ex) 
                {
                    if (ModuleLoadError != null) ModuleLoadError.Invoke(this, ex);
                }
            }

            return null;
        }

        private void UnloadModule(string packageCategory, string packageId, string packageVersion)
        {
            if (!string.IsNullOrEmpty(packageCategory) && !string.IsNullOrEmpty(packageId) && !string.IsNullOrEmpty(packageVersion))
            {
                try
                {
                    var loadContextId = CreateLoadContextId(packageCategory, packageId, packageVersion);

                    _context.RemoveLoadContext(loadContextId);

                    var moduleKey = GetModuleKey(packageId, packageVersion);
                    _modules.Remove(moduleKey);
                }
                catch (Exception ex)
                {
                    if (ModuleLoadError != null) ModuleLoadError.Invoke(this, ex);
                }
            }
        }


        private IEnumerable<Type> ReadPackageModuleTypes(string packageCategory, string packageId, string packageVersion)
        {
            if (!string.IsNullOrEmpty(packageId) && !string.IsNullOrEmpty(packageVersion))
            {
                var packageDirectory = TrakHoundPackage.GetPackageVersionDistributableDirectory(packageCategory, packageId, packageVersion);
                if (packageDirectory != null)
                {
                    var key = $"{packageId}-{packageVersion}";

                    var tempPath = CreateModuleDirectory(_context.Id, key, packageDirectory);
                    if (tempPath != null)
                    {
                        var loadContextId = CreateLoadContextId(packageCategory, packageId, packageVersion);

                        var assemblyFilename = _filterFunction(key);
                        var assemblyPath = Path.Combine(tempPath, assemblyFilename);

                        var types = new List<Type>();

                        if (_loadByDependencies)
                        {
                            var loadContext = _context.AddLoadContext(loadContextId, assemblyPath);

                            var assemblyTypes = GetTypesFromFile(loadContext, assemblyPath);
                            if (!assemblyTypes.IsNullOrEmpty())
                            {
                                types.AddRange(assemblyTypes);
                            }
                        }
                        else
                        {
                            var dllFiles = Directory.GetFiles(tempPath, "*.dll");
                            if (!dllFiles.IsNullOrEmpty())
                            {
                                foreach (var dllFile in dllFiles)
                                {
                                    var loadContext = _context.AddLoadContext(loadContextId, assemblyPath);

                                    var assemblyTypes = GetTypesFromFile(loadContext, dllFile);
                                    if (!assemblyTypes.IsNullOrEmpty())
                                    {
                                        types.AddRange(assemblyTypes);
                                    }
                                }
                            }
                        }

                        return types;
                    }
                }
            }

            return Enumerable.Empty<Type>();
        }


        private static string GetModuleKey(string packageId, string packageVersion)
        {
            return $"{packageId}:{packageVersion}";
        }


        private IEnumerable<Type> GetTypesFromFile(TrakHoundModuleLoadContext context, string path)
        {
            var types = new List<Type>();

            if (context != null && !string.IsNullOrEmpty(path))
            {
                if (File.Exists(path))
                {
                    var assembly = context.LoadFromAssemblyPath(path);
                    if (assembly != null)
                    {
                        var assemblyTypes = assembly.GetTypes();
                        foreach (var type in assemblyTypes)
                        {
                            if (typeof(TModule).IsAssignableFrom(type))
                            {     
                                types.Add(type);
                            }
                        }
                    }
                }
            }

            return types;
        }


        private static string CreateLoadContextId(string packageCategory, string packageId, string packageVersion)
        {
            return $"{packageCategory}:{packageId}:{packageVersion}".ToMD5Hash();
        }

        private string CreateModuleDirectory(string contextId, string packageHash, string path)
        {
            if (!string.IsNullOrEmpty(contextId) && !string.IsNullOrEmpty(path))
            {
                try
                {
                    var tempId = contextId;
                    var tempPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _directory, packageHash);
                    if (!Directory.Exists(tempPath))
                    {
                        Directory.CreateDirectory(tempPath);

                        // Copy Files and Directories to Temp directory
                        Files.Copy(path, tempPath);
                    }
                    else
                    {
                        //Console.WriteLine($"CreateModuleDirectory() : Directory Already Exists : Skipping Copy Files");
                    }

                    return tempPath;
                }
                catch (Exception ex)
                {
                    if (ModuleLoadError != null) ModuleLoadError.Invoke(this, ex);
                }
            }

            return null;
        }
    }
}
