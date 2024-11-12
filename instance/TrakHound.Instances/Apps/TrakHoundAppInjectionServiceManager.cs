// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using TrakHound.Clients;
using TrakHound.Configurations;
using TrakHound.Modules;
using TrakHound.Packages;
using TrakHound.Volumes;

namespace TrakHound.Apps
{
    public class TrakHoundAppInjectionServiceManager : ITrakHoundAppInjectionServiceManager, IDisposable
    {
        private static readonly string _packageCategory = "app";

        private readonly ITrakHoundConfigurationProfile _configurationProfile;
        private readonly ITrakHoundPackageManager _packageManager;
        private readonly ITrakHoundModuleManager _moduleManager;
        private readonly ITrakHoundClientProvider _clientProvider;
        private readonly ITrakHoundVolumeProvider _volumeProvider;
        private readonly Dictionary<string, TrakHoundAppInjectionServiceConfiguration> _serviceConfigurations = new Dictionary<string, TrakHoundAppInjectionServiceConfiguration>(); // TypeId => InjectionServiceInfo
        private readonly Dictionary<string, object> _services = new Dictionary<string, object>(); // ServiceId => Service (object)
        private readonly ListDictionary<string, string> _scopeServices = new ListDictionary<string, string>(); // ScopeId => ServiceId
        private readonly DelayEvent _delayLoadEvent = new DelayEvent(2000);
        private readonly object _lock = new object();

        private Dictionary<string, string> _installedConfigurations = new Dictionary<string, string>(); // Configuration.Id => Configuration.Hash
        private Dictionary<string, string> _installedPackages = new Dictionary<string, string>(); // Configuration.Id => Package.Uuid


        public IServiceProvider ServiceProvider { get; set; }


        public TrakHoundAppInjectionServiceManager(
            ITrakHoundConfigurationProfile configurationProfile,
            ITrakHoundPackageManager packageManager,
            ITrakHoundModuleProvider moduleProvider,
            TrakHoundModuleContext context,
            ITrakHoundClientProvider clientProvider,
            ITrakHoundVolumeProvider volumeProvider
            )
        {
            _delayLoadEvent.Elapsed += DelayElapsed;

            _configurationProfile = configurationProfile;
            _configurationProfile.ConfigurationAdded += (o, args) => _delayLoadEvent.Set();
            _configurationProfile.ConfigurationRemoved += (o, args) => _delayLoadEvent.Set();

            _packageManager = packageManager;
            _packageManager.PackageAdded += PackageAdded;
            _packageManager.PackageRemoved += PackageRemoved;

            _moduleManager = moduleProvider.Get<ITrakHoundAppInjectionService>(_packageCategory, context, false);

            _clientProvider = clientProvider;
            _volumeProvider = volumeProvider;

            Load();
        }

        public void Dispose()
        {
            _delayLoadEvent.Elapsed -= DelayElapsed;
            _delayLoadEvent.Dispose();

            _serviceConfigurations.Clear();
            _services.Clear();
            _scopeServices.Clear();

            _installedConfigurations.Clear();
            _installedPackages.Clear();
        }

        public void DisposeScope(string scopeId)
        {
            if (!string.IsNullOrEmpty(scopeId))
            {
                IEnumerable<string> serviceKeys;
                lock (_lock) serviceKeys = _scopeServices.Get(scopeId);

                if (!serviceKeys.IsNullOrEmpty())
                {
                    foreach (var serviceKey in serviceKeys)
                    {
                        object service;
                        lock (_lock) service = _services.GetValueOrDefault(serviceKey);

                        if (service != null)
                        {
                            // Don't Dispose Singleton
                            if (!typeof(ITrakHoundSingletonAppInjectionService).IsAssignableFrom(service.GetType()))
                            {
                                DisposeService(serviceKey);
                            }
                        }
                    }
                }
            }
        }

        private void DisposeService(string serviceKey)
        {
            if (!string.IsNullOrEmpty(serviceKey))
            {
                try
                {
                    object service;
                    lock (_lock) service = _services.GetValueOrDefault(serviceKey);

                    if (service != null)
                    {
                        // Dispose of the service
                        if (typeof(IDisposable).IsAssignableFrom(service.GetType()))
                        {
                            ((IDisposable)service).Dispose();
                        }

                        // Remove from services list
                        lock (_lock) _services.Remove(serviceKey);
                    }
                }
                catch { }
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


        public object GetService(string scopeId, Type serviceType)
        {
            if (!string.IsNullOrEmpty(scopeId) && serviceType != null)
            {
                var serviceKey = CreateServiceKey(scopeId, serviceType);

                object service;
                lock (_lock) service = _services.GetValueOrDefault(serviceKey);

                if (service == null)
                {
                    var typeKey = CreateTypeKey(serviceType);

                    TrakHoundAppInjectionServiceConfiguration serviceConfiguration;
                    lock (_lock) serviceConfiguration = _serviceConfigurations.GetValueOrDefault(typeKey);

                    if (serviceConfiguration != null)
                    {
                        service = CreateService(scopeId, serviceConfiguration);

                        if (service != null)
                        {
                            RegisterService(scopeId, service);
                        }
                    }
                }

                return service;
            }

            return null;
        }

        public object GetService<TServiceType>(string scopeId, Type serviceType) where TServiceType : ITrakHoundAppInjectionService
        {
            if (!string.IsNullOrEmpty(scopeId) && serviceType != null)
            {
                var serviceKey = CreateServiceKey(scopeId, serviceType);

                object service;
                lock (_lock) service = _services.GetValueOrDefault(serviceKey);

                if (service == null)
                {
                    var typeKey = CreateTypeKey(serviceType);

                    TrakHoundAppInjectionServiceConfiguration serviceConfiguration;
                    lock (_lock) serviceConfiguration = _serviceConfigurations.GetValueOrDefault(typeKey);

                    if (serviceConfiguration != null)
                    {
                        service = CreateService(scopeId, serviceConfiguration);

                        if (service != null)
                        {
                            RegisterService<TServiceType>(scopeId, service);
                        }
                    }
                }

                return service;
            }

            return null;
        }


        public IEnumerable<TrakHoundAppInjectionServiceConfiguration> GetServiceConfigurations<TServiceType>() where TServiceType : ITrakHoundAppInjectionService
        {
            var result = new List<TrakHoundAppInjectionServiceConfiguration>();

            IEnumerable<TrakHoundAppInjectionServiceConfiguration> allServices;
            lock (_lock) allServices = _serviceConfigurations.Values;

            if (!allServices.IsNullOrEmpty())
            {
                foreach (var service in allServices)
                {
                    if (typeof(TServiceType).IsAssignableFrom(service.ServiceType))
                    {
                        result.Add(service);
                    }
                }
            }

            return result;
        }

        public IEnumerable<TrakHoundAppInjectionServiceConfiguration> GetRequiredServiceConfigurations<TServiceType>() where TServiceType : ITrakHoundAppInjectionService
        {
            var result = new List<TrakHoundAppInjectionServiceConfiguration>();

            if (typeof(ITrakHoundTransientAppInjectionService).IsAssignableFrom(typeof(TServiceType)))
            {
                result.AddRange(GetServiceConfigurations<ITrakHoundTransientAppInjectionService>());
                result.AddRange(GetServiceConfigurations<ITrakHoundScopedAppInjectionService>());
                result.AddRange(GetServiceConfigurations<ITrakHoundSingletonAppInjectionService>());
            }
            else if (typeof(ITrakHoundScopedAppInjectionService).IsAssignableFrom(typeof(TServiceType)))
            {
                result.AddRange(GetServiceConfigurations<ITrakHoundScopedAppInjectionService>());
                result.AddRange(GetServiceConfigurations<ITrakHoundSingletonAppInjectionService>());
            }
            else if (typeof(ITrakHoundSingletonAppInjectionService).IsAssignableFrom(typeof(TServiceType)))
            {
                result.AddRange(GetServiceConfigurations<ITrakHoundSingletonAppInjectionService>());
            }

            return result;
        }


        public void RegisterService(string scopeId, object service)
        {
            if (!string.IsNullOrEmpty(scopeId) && service != null)
            {
                var serviceKey = CreateServiceKey(scopeId, service.GetType());

                lock (_lock)
                {
                    if (!_services.ContainsKey(serviceKey)) _services.Add(serviceKey, service);
                }

                // Add to list of scope services
                _scopeServices.Add(scopeId, serviceKey);
            }
        }

        public void RegisterService<TServiceType>(string scopeId, object service) where TServiceType : ITrakHoundAppInjectionService
        {
            if (!string.IsNullOrEmpty(scopeId) && service != null)
            {
                var serviceKey = CreateServiceKey(scopeId, service.GetType());

                lock (_lock)
                {
                    if (!_services.ContainsKey(serviceKey)) _services.Add(serviceKey, service);
                }

                // Add to list of scope services
                _scopeServices.Add(scopeId, serviceKey);
            }
        }


        private object CreateService(string scopeId, TrakHoundAppInjectionServiceConfiguration serviceConfiguration)
        {
            if (!string.IsNullOrEmpty(scopeId) && serviceConfiguration != null)
            {
                try
                {
                    var serviceProvider = new TrakHoundServiceProvider(scopeId, this);

                    // Add App Configuration
                    serviceProvider.AddService<ITrakHoundAppConfiguration>(serviceConfiguration.Configuration);

                    // Add client to allow injection of ITrakHoundClient for the specified RouterId set in the AppConfiguration
                    serviceProvider.AddService<ITrakHoundClient>(serviceConfiguration.Client);

                    // Add client to allow injection of ITrakHoundVolume
                    serviceProvider.AddService<ITrakHoundVolume>(serviceConfiguration.Volume);

                    var service = ActivatorUtilities.CreateInstance(serviceProvider, serviceConfiguration.ServiceType);
                    serviceProvider.AddService(service);
                    return service;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return null;
        }


        private void DelayElapsed(object sender, EventArgs e)
        {
            Load();
        }

        private void Load()
        {
            var foundConfigurationIds = new List<string>();

            var appConfigurations = _configurationProfile.Get<TrakHoundAppConfiguration>(TrakHoundAppConfiguration.ConfigurationCategory);

            lock (_lock)
            {
                //_serviceConfigurations.Clear();

                if (!appConfigurations.IsNullOrEmpty())
                {
                    foreach (var appConfiguration in appConfigurations.ToList())
                    {
                        if (!string.IsNullOrEmpty(appConfiguration.Route))
                        {
                            var module = _moduleManager.Get(appConfiguration.PackageId, appConfiguration.PackageVersion);
                            if (module != null)
                            {
                                var packageUuid = TrakHoundPackage.GenerateUuid(module.PackageId, module.PackageVersion);

                                // Get the Installed Hash (to check if the configuration has changed)
                                var installedConfigurationHash = _installedConfigurations.GetValueOrDefault(appConfiguration.Id);
                                var installedPackageUuid = _installedPackages.GetValueOrDefault(appConfiguration.Id);

                                if (appConfiguration.Hash != installedConfigurationHash || packageUuid != installedPackageUuid)
                                {
                                    _installedConfigurations.Remove(appConfiguration.Id);
                                    _installedPackages.Remove(appConfiguration.Id);

                                    LoadModule(appConfiguration, module);

                                    // Add to installed List
                                    _installedConfigurations.Add(appConfiguration.Id, appConfiguration.Hash);
                                    _installedPackages.Add(appConfiguration.Id, packageUuid);

                                    foundConfigurationIds.Add(appConfiguration.Id);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void LoadModule(ITrakHoundAppConfiguration configuration, ITrakHoundModule module)
        {
            if (configuration != null && module != null && !module.ModuleTypes.IsNullOrEmpty())
            {
                foreach (var moduleType in module.ModuleTypes)
                {
                    // Get Client
                    var client = _clientProvider.GetClient();
                    client.RouterId = configuration.RouterId;

                    // Get Volume
                    var volumeId = configuration.VolumeId ?? configuration.Id;
                    var volume = _volumeProvider.GetVolume(volumeId);

                    var serviceInfo = new TrakHoundAppInjectionServiceConfiguration(configuration, client, volume, moduleType);

                    var typeKey = CreateTypeKey(moduleType);

                    _serviceConfigurations.Remove(typeKey);
                    _serviceConfigurations.Add(typeKey, serviceInfo);
                }
            }
        }


        private static string CreateServiceKey(string scopeId, Type serviceType)
        {
            if (!string.IsNullOrEmpty(scopeId) && serviceType != null)
            {
                return $"{scopeId}:{CreateTypeKey(serviceType)}".ToMD5Hash();
            }

            return null;
        }

        private static string CreateTypeKey(Type serviceType)
        {
            if (serviceType != null && serviceType.Assembly != null)
            {
                return $"{serviceType.Assembly.Location}:{serviceType.FullName}".ToMD5Hash();
            }

            return null;
        }
    }
}
