// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using TrakHound.Configurations;
using TrakHound.Modules;
using TrakHound.Packages;
using TrakHound.Volumes;

namespace TrakHound.Drivers
{
    public class TrakHoundDriverProvider : ITrakHoundDriverProvider
    {
        private const string _packageCategory = "driver";

        private readonly Dictionary<string, TrakHoundDriverInformation> _driverInformations = new Dictionary<string, TrakHoundDriverInformation>();
        private readonly ListDictionary<string, ITrakHoundDriver> _drivers = new ListDictionary<string, ITrakHoundDriver>();
        private readonly ITrakHoundConfigurationProfile _configurationProfile;
        private readonly ITrakHoundModuleProvider _moduleSource;
        private readonly ITrakHoundModuleManager _moduleManager;
        private readonly ITrakHoundVolumeProvider _volumeProvider;
        private readonly TrakHoundPackageManager _packageManager;
        private readonly object _lock = new object();
        private readonly DelayEvent _delayLoadEvent = new DelayEvent(5000);

        private readonly Dictionary<string, string> _installedConfigurations = new Dictionary<string, string>(); // Configuration.Id => Configuration.Hash
        private readonly Dictionary<string, string> _installedModuleHashes = new Dictionary<string, string>(); // Configuration.Id => Module.Hash


        public IEnumerable<ITrakHoundDriver> Drivers => _drivers.Values;

        public EventHandler<ITrakHoundDriver> DriverAdded { get; set; }

        public EventHandler<string> DriverRemoved { get; set; }

        public EventHandler<Exception> DriverLoadError { get; set; }


        public TrakHoundDriverProvider(
            ITrakHoundConfigurationProfile configurationProfile,
            ITrakHoundModuleProvider moduleProvider,
            ITrakHoundVolumeProvider volumeProvider,
            TrakHoundPackageManager packageManager
            )
        {
            _configurationProfile = configurationProfile;
            _configurationProfile.ConfigurationAdded += ConfigurationUpdated;
            _configurationProfile.ConfigurationRemoved += ConfigurationUpdated;

            _volumeProvider = volumeProvider;

            _packageManager = packageManager;
            _packageManager.PackageAdded += PackageAdded;
            _packageManager.PackageRemoved += PackageRemoved;

            _moduleManager = moduleProvider.Get<ITrakHoundDriver>(_packageCategory);

            _delayLoadEvent.Elapsed += LoadDelayElapsed;
        }

        public void Dispose()
        {
            _delayLoadEvent.Elapsed -= LoadDelayElapsed;
            _delayLoadEvent.Dispose();

            lock (_lock)
            {
                // Dispose of all of the Drivers
                if (!_drivers.Values.IsNullOrEmpty())
                {
                    foreach (var service in _drivers.Values) service.Dispose();
                    _drivers.Clear();
                }

                _installedConfigurations.Clear();
                _installedModuleHashes.Clear();
            }
        }


        public IEnumerable<TrakHoundDriverInformation> GetInformation()
        {
            return _driverInformations.Values;
        }

        public TrakHoundDriverInformation GetInformation(string configurationId)
        {
            if (!string.IsNullOrEmpty(configurationId))
            {
                return _driverInformations.GetValueOrDefault(configurationId);
            }

            return null;
        }


        private void ConfigurationUpdated(object sender, ITrakHoundConfiguration configuration)
        {
            if (configuration != null && configuration.Category == TrakHoundDriverConfiguration.ConfigurationCategory)
            {
                _delayLoadEvent.Set();
            }
        }

        private void PackageAdded(object sender, TrakHoundPackage package)
        {
            if (package != null && package.Category == _packageCategory)
            {
                _delayLoadEvent.Set();
            }
        }

        private void PackageRemoved(object sender, TrakHoundPackageChangedArgs args)
        {
            if (args.PackageCategory == _packageCategory)
            {
                _delayLoadEvent.Set();
            }
        }

        public void Load()
        {
            var foundConfigurationIds = new List<string>();

            var configurations = _configurationProfile.Get<TrakHoundDriverConfiguration>(TrakHoundDriverConfiguration.ConfigurationCategory);
            if (!configurations.IsNullOrEmpty())
            {
                foreach (var configuration in configurations.ToList())
                {
                    if (!string.IsNullOrEmpty(configuration.PackageId) && !string.IsNullOrEmpty(configuration.PackageVersion))
                    {
                        var module = _moduleManager.Get(configuration.PackageId, configuration.PackageVersion);
                        if (module != null && !module.ModuleTypes.IsNullOrEmpty())
                        {
                            // Get the Installed Hash (to check if the configuration has changed)
                            var installedHash = _installedConfigurations.GetValueOrDefault(configuration.Id);
                            var installedModuleHash = _installedModuleHashes.GetValueOrDefault(configuration.Id);

                            if (configuration.Hash != installedHash || module.Package.Hash != installedModuleHash)
                            {
                                _installedConfigurations.Remove(configuration.Id);
                                _installedModuleHashes.Remove(configuration.Id);
                                _drivers.Remove(configuration.Id);

                                // Add Drivers
                                foreach (var moduleType in module.ModuleTypes)
                                {
                                    AddDriver(configuration, moduleType);
                                }


                                var information = new TrakHoundDriverInformation();
                                information.ConfigurationId = configuration.Id;
                                information.Name = configuration.Name;
                                information.Description = configuration.Description;
                                information.PackageId = module.PackageId;
                                information.PackageVersion = module.PackageVersion;

                                _driverInformations.Remove(configuration.Id);
                                _driverInformations.Add(configuration.Id, information);


                                _installedConfigurations.Add(configuration.Id, configuration.Hash);
                                _installedModuleHashes.Add(configuration.Id, module.Package.Hash);
                            }

                            foundConfigurationIds.Add(configuration.Id);
                        }
                    }
                }
            }

            // Remove unused Drivers
            lock (_lock)
            {
                var configurationIds = _drivers.Keys.ToList();
                foreach (var configurationId in configurationIds)
                {
                    if (!foundConfigurationIds.Contains(configurationId))
                    {
                        RemoveDriver(configurationId);

                        _driverInformations.Remove(configurationId);
                        _installedConfigurations.Remove(configurationId);
                        _installedModuleHashes.Remove(configurationId);
                    }
                }
            }
        }

        public void AddDriver(ITrakHoundDriverConfiguration configuration, Type type)
        {
            if (configuration != null && !type.IsInterface && !type.IsAbstract)
            {
                try
                {
                    var constructor = type.GetConstructor(new Type[] { typeof(ITrakHoundDriverConfiguration) });
                    if (constructor != null)
                    {
                        var driver = (ITrakHoundDriver)constructor.Invoke(new object[] { configuration });
                        if (driver != null)
                        {
                            var volumeId = configuration.VolumeId ?? configuration.Id;
                            ((TrakHoundDriver)driver).Volume = _volumeProvider.GetVolume(volumeId);

                            _drivers.Add(configuration.Id, driver);

                            if (DriverAdded != null) DriverAdded.Invoke(this, driver);
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (DriverLoadError != null) DriverLoadError.Invoke(this, ex);
                }
            }
        }

        public void RemoveDriver(string configurationId)
        {
            if (!string.IsNullOrEmpty(configurationId))
            {
                _drivers.Remove(configurationId);

                if (DriverRemoved != null) DriverRemoved.Invoke(this, configurationId);
            }
        }


        private void LoadDelayElapsed(object sender, EventArgs args)
        {
            Load();
        }

        public TDriver GetDriver<TDriver>(string configurationId) where TDriver : ITrakHoundDriver
        {
            // Get the Driver that matches the specified ITrakHoundDriver (TDriver)
            if (!string.IsNullOrEmpty(configurationId))
            {
                var configurationDrivers = _drivers.Get(configurationId);
                if (!configurationDrivers.IsNullOrEmpty())
                {
                    return (TDriver)configurationDrivers.FirstOrDefault(x => typeof(TDriver).IsAssignableFrom(x.GetType()));
                }
            }

            return default;
        }
    }
}
