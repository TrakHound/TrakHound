// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TrakHound.Packages;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace TrakHound.Configurations
{
    public class TrakHoundConfigurationProfile : ITrakHoundConfigurationProfile
    {
        public const string Default = "default";
        private const string _packageCategory = "configuration";

        private readonly string _id;
        private readonly string _path;
        private readonly Dictionary<string, ITrakHoundConfiguration> _configurations = new Dictionary<string, ITrakHoundConfiguration>();
        private readonly Dictionary<string, IDisposable> _watchers = new Dictionary<string, IDisposable>();
        private readonly DelayEvent _watcherDelay = new DelayEvent(100);
        private readonly Dictionary<string, ITrakHoundConfiguration> _watcherDelayConfigurations = new Dictionary<string, ITrakHoundConfiguration>();
        private readonly TrakHoundPackageManager _packageManager;
        private readonly object _lock = new object();


        [YamlIgnore]
        public IEnumerable<ITrakHoundConfiguration> Configurations
        {
            get
            {
                lock (_lock) return _configurations.Values;
            }
        }

        [YamlIgnore]
        public string Id => _id;

        [YamlIgnore]
        public string Path => _path;

        public string Name { get; set; }

        public string Version { get; set; }

        public bool Enabled { get; set; }


        public event EventHandler<ITrakHoundConfiguration> ConfigurationAdded;

        public event EventHandler<ITrakHoundConfiguration> ConfigurationRemoved;



        public TrakHoundConfigurationProfile(string id, TrakHoundPackageManager packageManager = null, string path = null)
        {
            _id = id;
            _path = !string.IsNullOrEmpty(path) ? path : GetDirectory(id);

            _packageManager = packageManager;
            _packageManager.PackageAdded += PackageAdded;

            _watcherDelay.Elapsed += WatcherDelayElapsed;
        }

        public void Dispose()
        {
            if (!_watchers.IsNullOrEmpty())
            {
                foreach (var watcher in _watchers)
                {
                    if (watcher.Value != null) watcher.Value.Dispose();
                }

                _watchers.Clear();
            }
        }


        private void PackageAdded(object sender, TrakHoundPackage package)
        {
            if (package != null && package.Category == _packageCategory)
            {
                try
                {
                    var packageDist = System.IO.Path.Combine(package.Location, TrakHoundPackage.DistributableDirectory);
                    var configurationCategoryDirectories = Directory.GetDirectories(packageDist);
                    if (!configurationCategoryDirectories.IsNullOrEmpty())
                    {
                        foreach (var configurationCategoryDirectory in configurationCategoryDirectories)
                        {
                            var configurationFiles = Directory.GetFiles(configurationCategoryDirectory);
                            if (!configurationFiles.IsNullOrEmpty())
                            {
                                var configurationCategory = System.IO.Path.GetFileName(configurationCategoryDirectory);

                                foreach (var configurationFile in configurationFiles)
                                {
                                    Add(configurationCategory, configurationFile);
                                }
                            }
                        }
                    }
                }
                catch { }
            }
        }


        public IEnumerable<TConfiguration> Get<TConfiguration>(string category) where TConfiguration : ITrakHoundConfiguration
        {
            if (!string.IsNullOrEmpty(category))
            {
                lock (_lock)
                {
                    var configurations = _configurations.Values.Where(o => o.Category == category);
                    if (!configurations.IsNullOrEmpty())
                    {
                        var typeConfigurations = new List<TConfiguration>();

                        foreach (var configuration in configurations)
                        {
                            try
                            {
                                var typeConfiguration = (TConfiguration)configuration;
                                typeConfigurations.Add(typeConfiguration);
                            }
                            catch { }
                        }

                        return typeConfigurations.ToList();
                    }
                }
            }

            return default;
        }

        public TConfiguration Get<TConfiguration>(string category, string configurationId) where TConfiguration : ITrakHoundConfiguration
        {
            if (!string.IsNullOrEmpty(category) && !string.IsNullOrEmpty(configurationId))
            {
                lock (_lock)
                {
                    var configuration = _configurations.Values.FirstOrDefault(o => o.Category == category && o.Id == configurationId);
                    if (configuration != null)
                    {
                        try
                        {
                            return (TConfiguration)configuration;
                        }
                        catch { }
                    }
                }
            }

            return default;
        }

        public object Get(string category, string configurationId)
        {
            if (!string.IsNullOrEmpty(category) && !string.IsNullOrEmpty(configurationId))
            {
                lock (_lock)
                {
                    var configuration = _configurations.Values.FirstOrDefault(o => o.Category == category && o.Id == configurationId);
                    if (configuration != null)
                    {
                        return configuration;
                    }
                }
            }

            return null;
        }


        public void Add(ITrakHoundConfiguration configuration, bool save = false)
        {
            if (configuration != null && !string.IsNullOrEmpty(configuration.Id))
            {
                var add = false;

                ITrakHoundConfiguration existingConfiguration;
                lock (_lock)
                {
                    existingConfiguration = _configurations.GetValueOrDefault(configuration.Id);
                    if (existingConfiguration == null || existingConfiguration.Hash != configuration.Hash)
                    {
                        _configurations.Remove(configuration.Id);
                        _configurations.Add(configuration.Id, configuration);
                        add = true;
                    }
                }

                if (add)
                {
                    if (save) Save(configuration.Category, configuration.Id);

                    if (ConfigurationAdded != null) ConfigurationAdded(this, configuration);
                }
            }
        }

        public void Add(string category, string configurationPath)
        {
            if (!string.IsNullOrEmpty(category) && !string.IsNullOrEmpty(configurationPath))
            {
                try
                {
                    var categoryDirectory = System.IO.Path.Combine(_path, category);
                    if (!Directory.Exists(categoryDirectory)) Directory.CreateDirectory(categoryDirectory);

                    var configurationId = System.IO.Path.GetFileName(configurationPath);
                    var savePath = System.IO.Path.Combine(categoryDirectory, configurationId);

                    if (!File.Exists(savePath))
                    {
                        File.Copy(configurationPath, savePath);
                    }
                }
                catch { }
            }
        }

        public void Remove(string category, string configurationId)
        {
            if (!string.IsNullOrEmpty(category) && !string.IsNullOrEmpty(configurationId))
            {
                IEnumerable<ITrakHoundConfiguration> configurations;
                lock (_lock) configurations = _configurations.Values.Where(o => o.Category == category && o.Id == configurationId).ToList();
                if (!configurations.IsNullOrEmpty())
                {
                    foreach (var configuration in configurations)
                    {
                        lock (_lock) _configurations.Remove(configuration.Id);

                        Delete(configuration.Category, configuration.Id);

                        if (ConfigurationRemoved != null) ConfigurationRemoved(this, configuration);
                    }
                }
            }
        }

        public void Clear(string category)
        {
            if (!string.IsNullOrEmpty(category))
            {
                IEnumerable<ITrakHoundConfiguration> configurations;
                lock (_lock) configurations = _configurations.Values.Where(o => o.Category == category).ToList();
                if (!configurations.IsNullOrEmpty())
                {
                    foreach (var configuration in configurations)
                    {
                        lock (_lock) _configurations.Remove(configuration.Id);

                        Delete(configuration.Category, configuration.Id);

                        if (ConfigurationRemoved != null) ConfigurationRemoved(this, configuration);
                    }
                }
            }
        }


        public IEnumerable<TConfiguration> Read<TConfiguration>(string category) where TConfiguration : ITrakHoundConfiguration
        {
            var configurations = new List<TConfiguration>();

            if (!string.IsNullOrEmpty(category))
            {
                var categoryDirectory = System.IO.Path.Combine(_path, category);
                if (Directory.Exists(categoryDirectory))
                {
                    var configurationPaths = Directory.GetFiles(categoryDirectory);
                    if (!configurationPaths.IsNullOrEmpty())
                    {
                        foreach (var configurationPath in configurationPaths)
                        {
                            var configuration = ReadFile<TConfiguration>(configurationPath);
                            if (configuration != null)
                            {
                                configurations.Add(configuration);
                            }
                        }
                    }
                }
            }

            return configurations;
        }

        public TConfiguration Read<TConfiguration>(string category, string configurationId) where TConfiguration : ITrakHoundConfiguration
        {
            return default;
        }

        public void Save(string category, string configurationId)
        {
            var categoryDirectory = System.IO.Path.Combine(_path, category);

            try
            {
                if (!Directory.Exists(categoryDirectory)) Directory.CreateDirectory(categoryDirectory);

                var configurationPath = System.IO.Path.Combine(categoryDirectory, configurationId);

                var configuration = Get(category, configurationId);
                if (configuration != null)
                {
                    SaveFile(configuration, configurationPath);
                }
            }
            catch { }
        }

        public void Delete(string category, string configurationId)
        {
            var categoryDirectory = System.IO.Path.Combine(_path, category);
            var configurationPath = System.IO.Path.Combine(categoryDirectory, configurationId);

            DeleteFile(configurationPath);
        }


        private TConfiguration ReadFile<TConfiguration>(string configurationPath) where TConfiguration : ITrakHoundConfiguration
        {
            if (!string.IsNullOrEmpty(configurationPath))
            {
                if (File.Exists(configurationPath))
                {
                    return ReadYaml<TConfiguration>(configurationPath);
                }
            }

            return default;
        }

        private void SaveFile(object configuration, string configurationPath)
        {
            if (!string.IsNullOrEmpty(configurationPath))
            {
                SaveYaml(configuration, configurationPath);
            }
        }

        private void DeleteFile(string configurationPath)
        {
            if (!string.IsNullOrEmpty(configurationPath))
            {
                DeleteYaml(configurationPath);
            }
        }


        private static TConfiguration ReadYaml<TConfiguration>(string configurationPath) where TConfiguration : ITrakHoundConfiguration
        {
            if (!string.IsNullOrEmpty(configurationPath))
            {
                try
                {
                    var text = File.ReadAllText(configurationPath);
                    if (!string.IsNullOrEmpty(text))
                    {
                        var deserializer = new DeserializerBuilder()
                            .WithNamingConvention(CamelCaseNamingConvention.Instance)
                            .IgnoreUnmatchedProperties()
                            .Build();

                        var configuration = deserializer.Deserialize<TConfiguration>(text);
                        configuration.Path = configurationPath;
                        return configuration;
                    }
                }
                catch { }
            }

            return default;
        }

        private static void SaveYaml(object configuration, string configurationPath)
        {
            if (configuration != null && !string.IsNullOrEmpty(configurationPath))
            {
                var path = configurationPath + ".yaml";

                try
                {
                    var serializer = new SerializerBuilder()
                        .WithNamingConvention(CamelCaseNamingConvention.Instance)
                        .Build();
                    var yaml = serializer.Serialize(configuration);
                    File.WriteAllText(path, yaml);
                }
                catch { }
            }
        }

        private static void DeleteYaml(string configurationPath)
        {
            if (!string.IsNullOrEmpty(configurationPath))
            {
                var path = configurationPath + ".yaml";

                try
                {
                    File.Delete(path);
                }
                catch { }
            }
        }


        public void Load<TConfiguration>(string category) where TConfiguration : ITrakHoundConfiguration
        {
            if (!string.IsNullOrEmpty(category))
            {
                var configurations = Read<TConfiguration>(category);
                if (!configurations.IsNullOrEmpty())
                {
                    foreach (var configuration in configurations)
                    {
                        if (!string.IsNullOrEmpty(configuration.Id))
                        {
                            lock (_lock)
                            {
                                _configurations.Remove(configuration.Id);
                                _configurations.Add(configuration.Id, configuration);
                            }

                            if (ConfigurationAdded != null) ConfigurationAdded(this, configuration);
                        }
                    }
                }


                var categoryDirectory = System.IO.Path.Combine(_path, category);
                var watcher = new ConfigurationFileWatcher<TConfiguration>(category, categoryDirectory, ReadFile<TConfiguration>);
                watcher.ConfigurationUpdated += ConfigurationFileUpdated;
                watcher.ConfigurationRemoved += ConfigurationFileRemoved;

                lock (_lock)
                {
                    _watchers.Remove(categoryDirectory);
                    _watchers.Add(categoryDirectory, watcher);
                }
            }
        }

        private void ConfigurationFileUpdated<TConfiguration>(object sender, TConfiguration configuration) where TConfiguration : ITrakHoundConfiguration
        {
            if (configuration != null && !string.IsNullOrEmpty(configuration.Id))
            {
                lock (_lock)
                {
                    _watcherDelayConfigurations.Remove(configuration.Id);
                    _watcherDelayConfigurations.Add(configuration.Id, configuration);
                }

                _watcherDelay.Set();
            }
        }

        private void ConfigurationFileRemoved(object sender, ConfigurationFileRemovedArgs args)
        {
            Remove(args.Category, args.Id);
        }

        public void Load<TConfiguration>(IEnumerable<string> categories) where TConfiguration : ITrakHoundConfiguration
        {
            if (!categories.IsNullOrEmpty())
            {
                foreach (var category in categories)
                {
                    Load<TConfiguration>(category);
                }
            }
        }

        private void WatcherDelayElapsed(object sender, EventArgs e)
        {
            var configurations = new List<ITrakHoundConfiguration>();
            lock (_lock)
            {
                foreach (var configuration in _watcherDelayConfigurations)
                {
                    configurations.Add(configuration.Value);
                }

                _watcherDelayConfigurations.Clear();
            }

            foreach (var configuration in configurations)
            {
                Add(configuration);
            }
        }


        public static string GetDirectory(string profileId)
        {
            if (!string.IsNullOrEmpty(profileId))
            {
                return System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, TrakHoundConfigurations.Directory, profileId);
            }

            return null;
        }
    }
}
