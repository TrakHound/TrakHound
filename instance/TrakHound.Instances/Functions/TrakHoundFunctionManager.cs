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

namespace TrakHound.Functions
{
    public class TrakHoundFunctionManager : IDisposable
    {
        private const string _packageCategory = "function";
        private const string _instancesBasePath = "TrakHound:/.Instances";
        private const string _functionsBasePath = "Functions";
        private const string _logsPath = "Logs";

        private readonly ITrakHoundInstance _instance;
        private readonly TrakHoundConfigurationProfile _configurationProfile;
        private readonly ITrakHoundModuleManager _moduleManager;
        private readonly ITrakHoundClientProvider _clientProvider;
        private readonly ITrakHoundClient _client;
        private readonly ITrakHoundVolumeProvider _volumeProvider;
        private readonly TrakHoundPackageManager _packageManager;
        private readonly Dictionary<string, FunctionItem> _functions = new Dictionary<string, FunctionItem>();
        private readonly List<TrakHoundFunctionEngine> _engines = new List<TrakHoundFunctionEngine>();
        private readonly Dictionary<string, string> _configurationFunctions = new Dictionary<string, string>(); // Configuration.Id => Function.Id
        private readonly DelayEvent _delayLoadEvent = new DelayEvent(2000);
        private readonly object _lock = new object();

        private bool _initialLoad = true;

        private readonly Dictionary<string, string> _installedConfigurations = new Dictionary<string, string>(); // Configuration.Id => Configuration.Hash
        private readonly Dictionary<string, string> _installedModuleHashes = new Dictionary<string, string>(); // Configuration.Id => Module.Hash


        public IEnumerable<TrakHoundFunctionEngine> Engines => _engines;


        public event EventHandler<TrakHoundFunctionEngine> EngineAdded;

        public event EventHandler<string> EngineRemoved;


        class FunctionItem
        {
            public ITrakHoundModule Module { get; set; }
            public ITrakHoundFunctionConfiguration Configuration { get; set; }
        }


        public TrakHoundFunctionManager(
            ITrakHoundInstance instance,
            TrakHoundConfigurationProfile configurationProfile,
            ITrakHoundModuleProvider moduleProvider,
            ITrakHoundClientProvider clientProvider,
            ITrakHoundVolumeProvider volumeProvider,
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

            _moduleManager = moduleProvider.Get<ITrakHoundFunction>(_packageCategory);

            _clientProvider = clientProvider;
            _client = _clientProvider.GetClient();

            _volumeProvider = volumeProvider;

            _delayLoadEvent.Elapsed += LoadDelayElapsed;
        }

        public void Dispose()
        {
            if (!_engines.IsNullOrEmpty())
            {
                foreach (var engine in _engines)
                {
                    engine.Dispose();
                }
            }

            _delayLoadEvent.Elapsed -= LoadDelayElapsed;
            _delayLoadEvent.Dispose();

            lock (_lock)
            {
                _functions.Clear();
                _engines.Clear();
                _configurationFunctions.Clear();
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


        private void ConfigurationAdded(object sender, ITrakHoundConfiguration configuration)
        {
            if (configuration.Category == TrakHoundFunctionConfiguration.ConfigurationCategory)
            {
                AddConfiguration((ITrakHoundFunctionConfiguration)configuration);
            }
        }

        private void ConfigurationRemoved(object sender, ITrakHoundConfiguration configuration)
        {
            if (configuration.Category == TrakHoundFunctionConfiguration.ConfigurationCategory)
            {
                RemoveConfiguration(configuration.Id);
            }
        }


        #region "Information"

        public IEnumerable<TrakHoundFunctionInformation> GetInformation()
        {
            var informations = new List<TrakHoundFunctionInformation>();

            IEnumerable<FunctionItem> functionItems;
            lock (_lock) functionItems = _functions.Values.ToList();
            if (!functionItems.IsNullOrEmpty())
            {
                foreach (var functionItem in functionItems)
                {
                    var information = TrakHoundFunctionInformation.Create(functionItem.Configuration, functionItem.Module.ModuleTypes?.FirstOrDefault(), functionItem.Module.Package, functionItem.Module.PackageReadMe);
                    if (information != null)
                    {
                        informations.Add(information);
                    }
                }
            }

            return informations;
        }

        public TrakHoundFunctionInformation GetInformation(string functionId)
        {
            TrakHoundFunctionInformation information = null;

            if (!string.IsNullOrEmpty(functionId))
            {
                var functionKey = functionId.ToLower();

                FunctionItem functionItem;
                lock (_lock) functionItem = _functions.GetValueOrDefault(functionKey);
                if (functionItem != null)
                {
                    information = TrakHoundFunctionInformation.Create(functionItem.Configuration, functionItem.Module.ModuleTypes?.FirstOrDefault(), functionItem.Module.Package, functionItem.Module.PackageReadMe);
                }
            }     

            return information;
        }

        public IEnumerable<TrakHoundFunctionInformation> GetInformation(IEnumerable<string> functionIds)
        {
            if (!functionIds.IsNullOrEmpty())
            {
                var informations = new List<TrakHoundFunctionInformation>();

                foreach (var functionId in functionIds)
                {
                    var information = GetInformation(functionId);
                    if (information != null) informations.Add(information);
                }

                return informations;
            }

            return null;
        }

        #endregion


        public async Task<TrakHoundFunctionResponse> Run(string functionId, IReadOnlyDictionary<string, string> parameters = null, string runId = null, long timestamp = 0)
        {
            if (!string.IsNullOrEmpty(functionId))
            {
                var functionKey = functionId.ToLower();
                var functionItem = _functions.GetValueOrDefault(functionKey);
                if (functionItem != null)
                {
                    return await RunConfiguration(functionItem.Module, functionItem.Configuration, parameters, runId, timestamp);
                }
            }

            return new TrakHoundFunctionResponse();
        }


        public void AddConfiguration(ITrakHoundFunctionConfiguration configuration)
        {
            if (configuration != null && !string.IsNullOrEmpty(configuration.Id))
            {
                var module = _moduleManager.Get(configuration.PackageId, configuration.PackageVersion);
                if (module != null)
                {
                    // Get the Installed Hash (to check if the configuration has changed)
                    var installedHash = _installedConfigurations.GetValueOrDefault(configuration.Id);
                    var installedModuleHash = _installedModuleHashes.GetValueOrDefault(configuration.Id);

                    if (configuration.Hash != installedHash || module.Package.Hash != installedModuleHash)
                    {
                        LoadConfiguration(module, configuration);
                    }
                }
            }
        }

        public void RemoveConfiguration(string configurationId)
        {
            if (!string.IsNullOrEmpty(configurationId))
            {
                string functionId;
                lock (_lock)
                {
                    _engines.RemoveAll(o => o.Function.Configuration.Id == configurationId);
                    functionId = _configurationFunctions.GetValueOrDefault(configurationId);
                }

                _configurationProfile.Remove(TrakHoundFunctionConfiguration.ConfigurationCategory, configurationId);
            }
        }


        private void LoadDelayElapsed(object sender, EventArgs args)
        {
            Load();
        }

        public void Load()
        {
            var configurations = _configurationProfile.Get<TrakHoundFunctionConfiguration>(TrakHoundFunctionConfiguration.ConfigurationCategory);
            if (!configurations.IsNullOrEmpty())
            {
                foreach (var configuration in configurations.ToList())
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
                                LoadConfiguration(module, configuration);
                            }
                        }
                    }
                }
            }
        }

        private void LoadConfiguration(ITrakHoundModule module, ITrakHoundFunctionConfiguration configuration)
        {
            if (module != null && configuration != null && configuration.Id != null && !string.IsNullOrEmpty(configuration.FunctionId))
            {
                // Create a new instance of the Function Type
                var function = CreateInstance(module, configuration);
                if (function != null)
                {
                    var functionItem = new FunctionItem();
                    functionItem.Module = module;
                    functionItem.Configuration = configuration;

                    var functionKey = configuration.FunctionId.ToLower();

                    lock (_lock)
                    {
                        _functions.Remove(functionKey);
                        _functions.Add(functionKey, functionItem);

                        _configurationFunctions.Remove(configuration.Id);
                        _configurationFunctions.Add(configuration.Id, configuration.FunctionId);

                        _installedConfigurations.Remove(configuration.Id);
                        _installedModuleHashes.Remove(configuration.Id);

                        // Add to installed List
                        _installedConfigurations.Add(configuration.Id, configuration.Hash);
                        _installedModuleHashes.Add(configuration.Id, module.Package.Hash);
                    }
                }
            }
        }


        private async Task<TrakHoundFunctionResponse> RunConfiguration(ITrakHoundModule module, ITrakHoundFunctionConfiguration configuration, IReadOnlyDictionary<string, string> parameters, string runId, long timestamp)
        {
            var function = CreateInstance(module, configuration);
            if (function != null)
            {
                var engine = new TrakHoundFunctionEngine(function, parameters, module.PackageId, module.PackageVersion, runId);
                engine.StatusReceived += FunctionStatusReceivedHandler;
                engine.LogReceived += FunctionLogReceivedHandler;

                lock (_lock)
                {
                    _engines.RemoveAll(o => o.EngineId == function.Id);
                    _engines.Add(engine);
                }

                if (EngineAdded != null) EngineAdded.Invoke(this, engine);

                return await engine.Run(timestamp);
            }

            return new TrakHoundFunctionResponse();
        }


        private async void FunctionStatusReceivedHandler(ITrakHoundFunction function, string runId, TrakHoundStatusItem item)
        {
            if (function != null)
            {
                var path = TrakHoundPath.Combine(_instancesBasePath, _instance.Id, _functionsBasePath, function.Configuration.FunctionId, "Run", runId, "Status");
                await function.Client.Entities.PublishState(path, item.StatusType.ToString(), timestamp: item.Timestamp.ToDateTime(), async: true);
            }
        }

        private async void FunctionLogReceivedHandler(ITrakHoundFunction function, string runId, TrakHoundLogItem item)
        {
            if (function != null)
            {
                var path = TrakHoundPath.Combine(_instancesBasePath, _instance.Id, _functionsBasePath, function.Configuration.FunctionId, "Run", runId, "Log");
                await function.Client.Entities.PublishLog(path, item.LogLevel, item.Message, item.Code, item.Timestamp.ToDateTime(), true);
            }
        }

        private ITrakHoundFunction CreateInstance(ITrakHoundModule module, ITrakHoundFunctionConfiguration configuration)
        {
            if (module != null && module.ModuleTypes != null && configuration != null)
            {
                try
                {
                    var constructor = module.ModuleTypes?.FirstOrDefault().GetConstructor(new Type[] {
                         typeof(ITrakHoundFunctionConfiguration),
                         typeof(ITrakHoundClient),
                         typeof(ITrakHoundVolume)
                         });
                    if (constructor != null)
                    {
                        // Get Client
                        var client = _clientProvider.GetClient();

                        // Get Volume
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

                            var function = (ITrakHoundFunction)constructor.Invoke(new object[] { configuration, client, volume });
                            if (function != null)
                            {
                                ((TrakHoundFunction)function).InstanceId = _instance.Id;
                                ((TrakHoundFunction)function).Package = module.Package;
                                ((TrakHoundFunction)function).SourceChain = sourceChain;
                                ((TrakHoundFunction)function).Source = source;

                                return function;
                            }
                        }
                    }
                }
                catch { }
            }

            return null;
        }
    }
}
