// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrakHound.Clients;
using TrakHound.Configurations;
using TrakHound.Instances;
using TrakHound.Logging;
using TrakHound.Modules;
using TrakHound.Packages;
using TrakHound.Requests;
using TrakHound.Volumes;

namespace TrakHound.Services
{
    public class TrakHoundServiceManager : IAsyncDisposable
    {
        private const string _packageCategory = "service";
        private const string _instancesPath = "/.Instances";
        private const string _basePath = "Services";
        private const string _logsPath = "Logs";

        private readonly ITrakHoundInstance _instance;
        private readonly TrakHoundConfigurationProfile _configurationProfile;
        private readonly ITrakHoundModuleManager _moduleManager;
        private readonly ITrakHoundClientProvider _clientProvider;
        private readonly ITrakHoundVolumeProvider _volumeProvider;
        private readonly ITrakHoundLogProvider _logProvider;
        private readonly ITrakHoundClient _client;
        private readonly TrakHoundPackageManager _packageManager;
        private readonly Dictionary<string, ServiceItem> _services = new Dictionary<string, ServiceItem>();
        private readonly Dictionary<string, TrakHoundServiceEngine> _engines = new Dictionary<string, TrakHoundServiceEngine>(); // Service.Id => Engine
        private readonly Dictionary<string, string> _configurationEngines = new Dictionary<string, string>(); // Configuration.Id => Service.Id
        private readonly DelayEvent _delayLoadEvent = new DelayEvent(2000);
        private readonly object _lock = new object();

        private readonly Dictionary<string, string> _installedConfigurations = new Dictionary<string, string>(); // Configuration.Id => Configuration.Hash
        private readonly Dictionary<string, string> _installedModuleHashes = new Dictionary<string, string>(); // Configuration.Id => Module.Hash

        private bool _initialLoad = true;
        private bool _started;


        public IEnumerable<TrakHoundServiceEngine> Engines => _engines.Values;

        public event EventHandler<TrakHoundServiceEngine> EngineAdded;

        public event EventHandler<string> EngineRemoved;

        public event TrakHoundServiceStatusHandler ServiceStatusChanged;

        public event TrakHoundServiceLogHandler ServiceLogUpdated;


        class ServiceItem
        {
            public ITrakHoundModule Module { get; set; }
            public ITrakHoundServiceConfiguration Configuration { get; set; }
            public TrakHoundServiceStatusType Status { get; set; }
            public DateTime LastUpdated { get; set; }
        }


        public TrakHoundServiceManager(
            ITrakHoundInstance instance,
            TrakHoundConfigurationProfile configurationProfile,
            ITrakHoundModuleProvider moduleProvider,
            ITrakHoundClientProvider clientProvider,
            ITrakHoundVolumeProvider volumeProvider,
            ITrakHoundLogProvider logProvider,
            TrakHoundPackageManager packageManager
            )
        {
            _instance = instance;

            _configurationProfile = configurationProfile;
            _configurationProfile.ConfigurationAdded += ConfigurationAdded;
            _configurationProfile.ConfigurationRemoved += ConfigurationRemoved;

            _packageManager = packageManager;
            _packageManager.PackageAdded += PackageAdded;
            _packageManager.PackageRemoved += PackageRemoved;

            _moduleManager = moduleProvider.Get<ITrakHoundService>(_packageCategory);

            _clientProvider = clientProvider;
            _client = _clientProvider.GetClient();

            _volumeProvider = volumeProvider;

            _logProvider = logProvider;

            _delayLoadEvent.Elapsed += LoadDelayElapsed;
        }

        public async ValueTask DisposeAsync()
        {
            if (!_engines.IsNullOrEmpty())
            {
                foreach (var engine in _engines.Values)
                {
                    await engine.Stop();
                }
            }

            _delayLoadEvent.Elapsed -= LoadDelayElapsed;
            _delayLoadEvent.Dispose();

            lock (_lock)
            {
                _services.Clear();
                _engines.Clear();
                _configurationEngines.Clear();
                _installedConfigurations.Clear();
                _installedModuleHashes.Clear();
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


        private async void ConfigurationAdded(object sender, ITrakHoundConfiguration configuration)
        {
            if (configuration.Category == TrakHoundServiceConfiguration.ConfigurationCategory)
            {
                await AddConfiguration((ITrakHoundServiceConfiguration)configuration);
            }
        }

        private void ConfigurationRemoved(object sender, ITrakHoundConfiguration configuration)
        {
            if (configuration.Category == TrakHoundServiceConfiguration.ConfigurationCategory)
            {
                RemoveConfiguration(configuration.Id);
            }
        }


        public void StartAll()
        {
            if (!_engines.IsNullOrEmpty())
            {
                foreach (var engine in _engines.Values)
                {
                    engine.Start();
                }
            }

            _started = true;
        }

        public void Start(string engineId)
        {
            if (!string.IsNullOrEmpty(engineId) && !_engines.IsNullOrEmpty())
            {
                var engine = _engines.GetValueOrDefault(engineId.ToLower());
                if (engine != null)
                {
                    engine.Start();
                }
            }
        }

        public async Task StopAll()
        {
            if (!_engines.IsNullOrEmpty())
            {
                foreach (var engine in _engines.Values)
                {
                    await engine.Stop();
                }
            }

            _started = false;
        }

        public async Task Stop(string engineId)
        {
            if (!string.IsNullOrEmpty(engineId) && !_engines.IsNullOrEmpty())
            {
                var engine = _engines.GetValueOrDefault(engineId.ToLower());
                if (engine != null)
                {
                    await engine.Stop();
                }
            }
        }


        #region "Information"

        public IEnumerable<TrakHoundServiceInformation> GetInformation()
        {
            var informations = new List<TrakHoundServiceInformation>();

            IEnumerable<ServiceItem> items;
            lock (_lock) items = _services.Values.ToList();
            if (!items.IsNullOrEmpty())
            {
                foreach (var item in items)
                {
                    var information = TrakHoundServiceInformation.Create(item.Configuration, item.Module.ModuleTypes?.FirstOrDefault(), item.Module.Package, item.Module.PackageReadMe);
                    if (information != null)
                    {
                        information.Id = GenerateServiceId(_instance.Id, item.Module.Package.Id, item.Configuration.Id);
                        information.Status = item.Status;
                        information.LastUpdated = item.LastUpdated;

                        informations.Add(information);
                    }
                }
            }

            return informations;
        }

        public TrakHoundServiceInformation GetInformation(string serviceId)
        {
            TrakHoundServiceInformation information = null;

            if (!string.IsNullOrEmpty(serviceId))
            {
                var functionKey = serviceId.ToLower();

                ServiceItem item;
                lock (_lock) item = _services.GetValueOrDefault(functionKey);
                if (item != null)
                {
                    information = TrakHoundServiceInformation.Create(item.Configuration, item.Module.ModuleTypes?.FirstOrDefault(), item.Module.Package, item.Module.PackageReadMe);
                    if (information != null)
                    {
                        information.Id = GenerateServiceId(_instance.Id, item.Module.Package.Id, item.Configuration.Id);
                        information.Status = item.Status;
                        information.LastUpdated = item.LastUpdated;
                    }
                }
            }

            return information;
        }

        public IEnumerable<TrakHoundServiceInformation> GetInformation(IEnumerable<string> serviceIds)
        {
            if (!serviceIds.IsNullOrEmpty())
            {
                var informations = new List<TrakHoundServiceInformation>();

                foreach (var serviceId in serviceIds)
                {
                    var information = GetInformation(serviceId);
                    if (information != null) informations.Add(information);
                }

                return informations;
            }

            return null;
        }

        #endregion


        public TrakHoundServiceEngine GetEngine(string serviceId)
        {
            if (!string.IsNullOrEmpty(serviceId))
            {
                TrakHoundServiceEngine engine;
                lock (_lock) engine = _engines.GetValueOrDefault(serviceId.ToLower());
                if (engine != null)
                {
                    return engine;
                }
            }

            return null;
        }


        public async Task AddConfiguration(ITrakHoundServiceConfiguration configuration)
        {
            if (configuration != null)
            {
                var module = _moduleManager.Get(configuration.PackageId, configuration.PackageVersion);
                if (module != null)
                {
                    await LoadConfiguration(module, configuration, true);
                }
            }
        }

        public async void RemoveConfiguration(string configurationId)
        {
            if (!string.IsNullOrEmpty(configurationId))
            {
                var serviceId = _configurationEngines.GetValueOrDefault(configurationId);
                if (serviceId != null)
                {
                    await Stop(serviceId);

                    var serviceKey = serviceId.ToLower();

                    lock (_lock)
                    {
                        _services.Remove(serviceKey);
                        _engines.Remove(serviceKey);
                        _configurationEngines.Remove(configurationId);
                    }

                    if (EngineRemoved != null) EngineRemoved.Invoke(this, serviceId);
                }
            }
        }


        private async void LoadDelayElapsed(object sender, EventArgs args)
        {
            await Load();
        }

        public async Task Load()
        {
            var configurations = _configurationProfile.Get<TrakHoundServiceConfiguration>(TrakHoundServiceConfiguration.ConfigurationCategory);
            if (!configurations.IsNullOrEmpty())
            {
                foreach (var configuration in configurations)
                {
                    if (!string.IsNullOrEmpty(configuration.Id))
                    {
                        var module = _moduleManager.Get(configuration.PackageId, configuration.PackageVersion);
                        if (module != null)
                        {
                            // Get the Installed Hash (to check if the configuration has changed)
                            var installedHash = _installedConfigurations.GetValueOrDefault(configuration.Id);
                            var installedModuleHash = _installedModuleHashes.GetValueOrDefault(configuration.Id);

                            if (configuration.Hash != installedHash || module.Package.Hash != installedModuleHash)
                            {
                                await LoadConfiguration(module, configuration, _started);
                            }
                        }
                    }
                }
            }
        }

        private async Task LoadConfiguration(ITrakHoundModule module, ITrakHoundServiceConfiguration configuration, bool startEngine = false)
        {
            var service = CreateInstance(module, configuration);
            if (service != null)
            {
                var now = DateTime.UtcNow;

                // Stop any existing Engines with the same Service ID
                await Stop(service.Id);

                var serviceItem = new ServiceItem();
                serviceItem.Module = module;
                serviceItem.Configuration = configuration;
                serviceItem.LastUpdated = now;

                var engine = new TrakHoundServiceEngine(service, module.PackageId, module.PackageVersion);
                engine.StatusChanged += ServiceStatusChangeHandler;
                engine.LogReceived += ServiceLogReceivedHandler;

                var serviceKey = service.Id.ToLower();

                lock (_lock)
                {
                    _services.Remove(serviceKey);
                    _services.Add(serviceKey, serviceItem);

                    _engines.Remove(serviceKey);
                    _engines.Add(serviceKey, engine);

                    _configurationEngines.Remove(configuration.Id);
                    _configurationEngines.Add(configuration.Id, service.Id);

                    _installedConfigurations.Remove(configuration.Id);
                    _installedModuleHashes.Remove(configuration.Id);

                    // Add to installed List
                    _installedConfigurations.Add(configuration.Id, configuration.Hash);
                    _installedModuleHashes.Add(configuration.Id, module.Package.Hash);
                }

                if (EngineAdded != null) EngineAdded.Invoke(this, engine);

                if (startEngine) engine.Start();
            }
        }


        private void ServiceLogReceivedHandler(object sender, TrakHoundLogItem item)
        {
            if (sender != null)
            {
                var service = sender as ITrakHoundService;
                if (service != null)
                {
                    var logger = _logProvider.GetLogger($"service/{service.Id}/{item.Sender}");
                    logger.Log(item);

                    if (ServiceLogUpdated != null) ServiceLogUpdated.Invoke(service.Id, item);
                }
            }
        }

        private void ServiceStatusChangeHandler(object sender, TrakHoundServiceStatusType status)
        {
            if (sender != null)
            {
                var service = sender as ITrakHoundService;
                if (service != null && service.Id != null)
                {
                    var timestamp = DateTime.UtcNow;

                    lock (_lock)
                    {
                        var serviceKey = service.Id.ToLower();
                        var item = _services.GetValueOrDefault(serviceKey);
                        if (item != null)
                        {
                            item.Status = status;
                            item.LastUpdated = timestamp;
                        }
                    }

                    if (ServiceStatusChanged != null) ServiceStatusChanged.Invoke(service.Id, status);
                }
            }
        }

        private ITrakHoundService CreateInstance(ITrakHoundModule module, ITrakHoundServiceConfiguration configuration)
        {
            if (module != null && module.ModuleTypes != null && configuration != null)
            {
                try
                {
                    var constructor = module.ModuleTypes?.FirstOrDefault().GetConstructor(new Type[] {
                         typeof(ITrakHoundServiceConfiguration),
                         typeof(ITrakHoundClient),
                         typeof(ITrakHoundVolume)
                         });
                    if (constructor != null)
                    {
                        var client = _clientProvider.GetClient();

                        var volumeId = configuration.VolumeId ?? configuration.Id;
                        var volume = _volumeProvider.GetVolume(volumeId);

                        if (client != null && volume != null)
                        {
                            var source = TrakHoundSourceEntry.CreateModuleSource(module);

                            var sourceChain = new TrakHoundSourceChain();
                            sourceChain.Add(source);
                            sourceChain.Add(TrakHoundSourceEntry.CreateApplicationSource());
                            sourceChain.Add(TrakHoundSourceEntry.CreateUserSource());
                            sourceChain.Add(TrakHoundSourceEntry.CreateDeviceSource());
                            sourceChain.Add(TrakHoundSourceEntry.CreateNetworkSource());
                            sourceChain.Add(TrakHoundSourceEntry.CreateInstanceSource(_instance.Id));

                            client.RouterId = configuration.RouterId;

                            var service = (ITrakHoundService)constructor.Invoke(new object[] { configuration, client, volume });
                            if (service != null)
                            {
                                ((TrakHoundService)service).InstanceId = _instance.Id;
                                ((TrakHoundService)service).Package = module.Package;
                                ((TrakHoundService)service).Id = GenerateServiceId(_instance.Id, configuration.PackageId, configuration.Id);
                                ((TrakHoundService)service).SourceChain = sourceChain;
                                ((TrakHoundService)service).Source = source;

                                return service;
                            }
                        }
                    }
                }
                catch { }
            }

            return null;
        }

        private static string GenerateServiceId(string instanceId, string packageId, string configurationId)
        {
            return $"{instanceId}:{packageId}:{configurationId}".ToMD5Hash();
        }
    }
}
