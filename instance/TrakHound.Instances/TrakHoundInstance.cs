// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading.Tasks;
using TrakHound.Api;
using TrakHound.Apps;
using TrakHound.Buffers;
using TrakHound.Clients;
using TrakHound.Configurations;
using TrakHound.Drivers;
using TrakHound.Functions;
using TrakHound.Licenses;
using TrakHound.Logging;
using TrakHound.Management;
using TrakHound.Modules;
using TrakHound.Packages;
using TrakHound.Routing;
using TrakHound.Security;
using TrakHound.Services;
using TrakHound.Volumes;

namespace TrakHound.Instances
{
    public sealed class TrakHoundInstance : ITrakHoundInstance
    {
        private readonly ITrakHoundLogger _logger = new TrakHoundLogger<TrakHoundInstance>();
        private readonly string _configurationProfileId = TrakHoundConfigurationProfile.Default;

        private TrakHoundManagementClient _managementClient;
        private bool _managementClientConnected;
        private DelayEvent _logDelayEvent;
        private DateTime _lastLogUpdate;

        private readonly string _version;
        private readonly DateTime _createdTime = DateTime.UtcNow;
        private DateTime _lastUpdated = DateTime.UtcNow;
        private DateTime? _startTime;
        private DateTime? _stopTime;
        private TrakHoundInstanceStatus _status;
        private bool _started;

        private TrakHoundInstanceConfiguration _configuration;
        private TrakHoundInstanceInformation _managementInstance;
        private TrakHoundInstanceInformation _instanceInformation;
        private TrakHoundConfigurationProfile _configurationProfile;
        private TrakHoundPackageManager _packageManager;
        private TrakHoundModuleProvider _moduleProvider;
        private TrakHoundSecurityManager _securityManager;
        private TrakHoundDriverProvider _driverProvider;
        private TrakHoundBufferProvider _bufferProvider;
        private TrakHoundRouterProvider _routerProvider;
        private TrakHoundApiProvider _apiProvider;
        private TrakHoundFunctionManager _functionManager;
        private TrakHoundServiceManager _serviceManager;
        private TrakHoundVolumeProvider _volumeProvider;
        private ITrakHoundClientProvider _clientProvider;


        public string Id => _configuration?.InstanceId;

        public string Version => _version;

        public TrakHoundInstanceType Type => !string.IsNullOrEmpty(_configuration.ManagementServer) ? TrakHoundInstanceType.Managed : TrakHoundInstanceType.Independent;

        public TrakHoundInstanceStatus Status => _status;

        public DateTime CreatedTime => _createdTime;

        public DateTime LastUpdated => _lastUpdated;

        public DateTime? StartTime => _startTime;

        public DateTime? StopTime => _stopTime;


        public TrakHoundInstanceConfiguration Configuration => _configuration;
        public ITrakHoundConfigurationProfile ConfigurationProfile => _configurationProfile;
        public TrakHoundInstanceInformation Information => _instanceInformation;
        public TrakHoundPackageManager PackageManager => _packageManager;
        public ITrakHoundModuleProvider ModuleProvider => _moduleProvider;
        public ITrakHoundSecurityManager SecurityManager => _securityManager;
        public ITrakHoundDriverProvider DriverProvider => _driverProvider;
        public ITrakHoundBufferProvider BufferProvider => _bufferProvider;
        public TrakHoundRouterProvider RouterProvider => _routerProvider;
        public ITrakHoundAppProvider AppProvider { get; set; }
        public ITrakHoundApiProvider ApiProvider => _apiProvider;
        public TrakHoundFunctionManager FunctionManager => _functionManager;
        public TrakHoundServiceManager ServiceManager => _serviceManager;
        public ITrakHoundVolumeProvider VolumeProvider => _volumeProvider;
        public ITrakHoundClientProvider ClientProvider => _clientProvider;

        public ITrakHoundLogger Logger => _logger;


        public event EventHandler Starting;

        public event EventHandler Started;

        public event EventHandler Stopping;

        public event EventHandler Stopped;

        public event TrakHoundInstanceStatusHandler StatusUpdated;

        public event TrakHoundInstanceLogHandler LogUpdated;


        public TrakHoundInstance() 
        { 
            _version = GetInstanceVersion();

            TrakHoundLogProvider.Get().LogEntryReceived += LogReceived;
        }

        public TrakHoundInstance(string configurationProfileId)
        {
            _version = GetInstanceVersion();
            _configurationProfileId = configurationProfileId;

            TrakHoundLogProvider.Get().LogEntryReceived += LogReceived;
        }

        public TrakHoundInstance(TrakHoundInstanceConfiguration configuration)
        {
            _version = GetInstanceVersion();
            _configuration = configuration;

            TrakHoundLogProvider.Get().LogEntryReceived += LogReceived;
        }

        public TrakHoundInstance(TrakHoundInstanceConfiguration configuration, string configurationProfileId)
        {
            _version = GetInstanceVersion();
            _configuration = configuration;
            _configurationProfileId = configurationProfileId;

            TrakHoundLogProvider.Get().LogEntryReceived += LogReceived;
        }


        private string GetInstanceVersion()
        {
            //return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            return FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion;
        }


        public async Task Start()
        {
            if (!_started)
            {
                _started = true;

                var hostParameters = new Dictionary<string, string>();

                if (_configuration == null) _configuration = TrakHoundInstanceConfiguration.Read();
                if (_configuration == null)
                {
                    _configuration = new TrakHoundInstanceConfiguration();
                    _configuration.Save();
                }

                _instanceInformation = new TrakHoundInstanceInformation();
                _instanceInformation.Id = _configuration.InstanceId;
                _instanceInformation.Version = _version;
                _instanceInformation.Name = _configuration.Name;
                _instanceInformation.Description = _configuration.Description;
                _instanceInformation.Environment.Sender = Environment.MachineName;
                _instanceInformation.Environment.User = Environment.UserName;
                _instanceInformation.Environment.OperatingSystem.Type = Environment.OSVersion.Platform.ToString();
                _instanceInformation.Environment.OperatingSystem.Name = Environment.OSVersion.VersionString;
                _instanceInformation.Environment.OperatingSystem.Version = Environment.OSVersion.Version.ToString();

                var httpInterface = new TrakHoundInstanceInterfaceInformation();
                httpInterface.Id = "http-01";
                httpInterface.Type = "HTTP";
                httpInterface.Address = !string.IsNullOrEmpty(_configuration.HttpAddress) ? _configuration.HttpAddress : GetAddress();
                httpInterface.Port = _configuration.HttpPort;

                hostParameters["Protocol"] = "HTTP";
                hostParameters["Address"] = httpInterface.Address;
                hostParameters["Port"] = _configuration.HttpPort.ToString();

                var interfaces = new List<TrakHoundInstanceInterfaceInformation>();
                interfaces.Add(httpInterface);
                _instanceInformation.Interfaces = interfaces;


                _lastUpdated = DateTime.UtcNow;
                _status = TrakHoundInstanceStatus.Starting;
                _instanceInformation.Status = _status;
                _instanceInformation.LastUpdated = _lastUpdated;
                if (StatusUpdated != null) StatusUpdated.Invoke(Id, _status);
                if (Starting != null) Starting.Invoke(this, EventArgs.Empty);


                // Initialize License Manager(s)
                TrakHoundLicenseManagers.ManagerAdded += (s, publisherId) => _logger.LogInformation($"License Manager Loaded : Publisher ID = {publisherId}");
                TrakHoundLicenseManagers.Initialize();

                // Initialize Package Manager
                _packageManager = new TrakHoundPackageManager(_managementInstance?.Organization, _managementClient);

                // Load Configuration
                _logger.LogInformation($"Loading Configuration Profile ({_configurationProfileId})...");
                _configurationProfile = new TrakHoundConfigurationProfile(_configurationProfileId, _packageManager);
                _configurationProfile.ConfigurationAdded += (s, args) => { _logger.LogDebug($"Configuration Loaded : {args.Category} : {args.Id}"); };
                _configurationProfile.Load<TrakHoundFunctionConfiguration>(TrakHoundFunctionConfiguration.ConfigurationCategory);
                _configurationProfile.Load<TrakHoundServiceConfiguration>(TrakHoundServiceConfiguration.ConfigurationCategory);
                _configurationProfile.Load<TrakHoundRouterConfiguration>(TrakHoundRouterConfiguration.ConfigurationCategory);
                _configurationProfile.Load<TrakHoundApiConfiguration>(TrakHoundApiConfiguration.ConfigurationCategory);
                _configurationProfile.Load<TrakHoundAppConfiguration>(TrakHoundAppConfiguration.ConfigurationCategory);
                _configurationProfile.Load<TrakHoundDriverConfiguration>(TrakHoundDriverConfiguration.ConfigurationCategory);
                _configurationProfile.Load<TrakHoundIdentityProviderConfiguration>(TrakHoundIdentityProviderConfiguration.ConfigurationCategory);

                _packageManager.PackageAdded += (s, args) => { _logger.LogDebug($"Package Loaded : {args.Id} v{args.Version}"); };
                _logger.LogInformation("Loading Packages...");
                _packageManager.Load();

                // Initialize Module Provider
                _moduleProvider = new TrakHoundModuleProvider(_packageManager);

                // Initialize Volume Provider
                var volumeProvider = new TrakHoundVolumeProvider();
                _volumeProvider = volumeProvider;

                // Security Manager
                _securityManager = new TrakHoundSecurityManager(_configurationProfile, _moduleProvider, volumeProvider, _packageManager);
                _securityManager.IdentityProviderAdded += (s, args) => { Console.WriteLine($"Identity Provider Loaded : {args.Id}"); };
                _logger.LogInformation("Loading Identity Providers...");
                _securityManager.Load();

                // Initialize Driver Provider
                _driverProvider = new TrakHoundDriverProvider(_configurationProfile, _moduleProvider, volumeProvider, _packageManager);
                _driverProvider.DriverAdded += (s, args) => { _logger.LogInformation($"Entity Driver Loaded : {args.Id}"); };
                _driverProvider.DriverLoadError += (s, ex) => { _logger.LogError($"Entity Driver Load ERROR : {ex.Message}"); };
                _logger.LogInformation("Loading Drivers...");
                _driverProvider.Load();

                // Initialize Buffer Provider
                _bufferProvider = new TrakHoundBufferProvider();

                // Initialize Router Provider
                _routerProvider = new TrakHoundRouterProvider(_configurationProfile, _driverProvider, _bufferProvider);
                _routerProvider.RouterAdded += (s, args) => { _logger.LogInformation($"Router Loaded : {args.Configuration?.Id} => {args.Configuration?.Name}"); };
                _logger.LogInformation("Loading Routers...");


                // Initialize Client Provider
                var clientProvider = new TrakHoundInstanceClientProvider(this, _driverProvider, _routerProvider);
                clientProvider.AppProvider = AppProvider;
                _clientProvider = clientProvider;
                _routerProvider.ClientProvider = _clientProvider;
                _routerProvider.Load();


                // Initialize Api Provider
                _apiProvider = new TrakHoundApiProvider(this, _configurationProfile, _moduleProvider, clientProvider, volumeProvider, _packageManager);
                _apiProvider.ApiAdded += (s, args) => { _logger.LogInformation($"Api Loaded : {args.PackageId}: {args.PackageVersion} : {args.Id} => {args.Route}"); };
                _apiProvider.ApiRemoved += (s, args) => { _logger.LogInformation($"Api Removed : {args}"); };
                clientProvider.ApiProvider = _apiProvider;

                // Initialize Function Manager
                _functionManager = new TrakHoundFunctionManager(this, _configurationProfile, _moduleProvider, clientProvider, volumeProvider, _packageManager);
                _functionManager.EngineAdded += (s, args) => { _logger.LogInformation($"Function Engine Loaded : {args.PackageId}: {args.PackageVersion} : {args?.EngineId}"); };
                _functionManager.EngineRemoved += (s, args) => { _logger.LogInformation($"Function Engine Removed : {args}"); };
                clientProvider.FunctionManager = _functionManager;

                // Initialize Service Manager
                _serviceManager = new TrakHoundServiceManager(this, _configurationProfile, _moduleProvider, clientProvider, volumeProvider, _packageManager);
                _serviceManager.EngineAdded += (s, args) => { _logger.LogInformation($"Service Engine Loaded : {args.PackageId}: {args.PackageVersion} : {args?.EngineId}"); };
                _serviceManager.EngineRemoved += (s, args) => { _logger.LogInformation($"Service Engine Removed : {args}"); };
                clientProvider.ServiceManager = _serviceManager;

                if (!_routerProvider.Routers.IsNullOrEmpty())
                {
                    foreach (var router in _routerProvider.Routers)
                    {
                        var client = clientProvider.GetClient();
                        client.RouterId = router.Id;
                        router.Client = client;
                    }
                }

                _logger.LogInformation("Loading Apis...");
                _apiProvider.Load();

                _logger.LogInformation("Loading Functions...");
                _functionManager.Load();

                _logger.LogInformation("Loading Services...");
                await _serviceManager.Load();
                _serviceManager.StartAll();


                _lastUpdated = DateTime.UtcNow;
                _startTime = _lastUpdated;
                _status = TrakHoundInstanceStatus.Started;
                _instanceInformation.Status = _status;
                _instanceInformation.LastUpdated = _lastUpdated;

                if (StatusUpdated != null) StatusUpdated.Invoke(Id, _status);
                if (Started != null) Started.Invoke(this, EventArgs.Empty);
            }
        }

        public async Task Stop()
        {
            if (_started)
            {
                _started = false;

                _lastUpdated = DateTime.UtcNow;
                _status = TrakHoundInstanceStatus.Stopping;
                _instanceInformation.Status = _status;
                _instanceInformation.LastUpdated = _lastUpdated;
                if (StatusUpdated != null) StatusUpdated.Invoke(Id, _status);
                if (Stopping != null) Stopping.Invoke(this, EventArgs.Empty);

                if (_configurationProfile != null) _configurationProfile.Dispose();
                if (_packageManager != null) _packageManager.Dispose();
                if (_driverProvider != null) _driverProvider.Dispose();
                if (_routerProvider != null) _routerProvider.Dispose();
                if (_bufferProvider != null) _bufferProvider.Dispose();
                if (_apiProvider != null) _apiProvider.Dispose();
                if (_functionManager != null) _functionManager.Dispose();
                if (_serviceManager != null) await _serviceManager.DisposeAsync();

                _lastUpdated = DateTime.UtcNow;
                _stopTime = _lastUpdated;
                _status = TrakHoundInstanceStatus.Stopped;
                _instanceInformation.Status = _status;
                _instanceInformation.LastUpdated = _lastUpdated;

                if (StatusUpdated != null) StatusUpdated.Invoke(Id, _status);
                if (Stopped != null) Stopped.Invoke(this, EventArgs.Empty);
            }
        }


        private void LogReceived(object sender, TrakHoundLogItem logItem)
        {
            if (LogUpdated != null) LogUpdated.Invoke(Id, logItem);
        }


        public async Task InstallPackage(TrakHoundPackage package, bool downloadPackage = true, TrakHoundManagementClient managementClient = null)
        {
            if (package != null)
            {
                // Install Package
                IEnumerable<TrakHoundPackageInstallResult> packageResults = null;
                if (downloadPackage) packageResults = await _packageManager.Install(package.Uuid, managementClient);
                if (!downloadPackage || !packageResults.IsNullOrEmpty())
                {
                    switch (package.Category)
                    {
                        case "app":
                            var appConfigurations = _configurationProfile.Get<TrakHoundAppConfiguration>(TrakHoundAppConfiguration.ConfigurationCategory);
                            if (appConfigurations.IsNullOrEmpty() || !appConfigurations.Any(o => o.PackageId == package.Id))
                            {
                                var appConfiguration = new TrakHoundAppConfiguration();
                                appConfiguration.Id = Guid.NewGuid().ToString();
                                appConfiguration.Name = package.Metadata?.GetValueOrDefault(".defaultName")?.ToString();
                                appConfiguration.PackageId = package.Id;
                                appConfiguration.PackageVersion = "*";
                                appConfiguration.Route = package.Metadata?.GetValueOrDefault(".defaultRoute")?.ToString();
                                appConfiguration.VolumeId = package.Metadata?.GetValueOrDefault(".defaultVolumeId")?.ToString();
                                appConfiguration.RouterId = TrakHoundRouter.Default;
                                _configurationProfile.Add(appConfiguration, true);
                            }
                            break;

                        case "api":
                            var apiConfigurations = _configurationProfile.Get<TrakHoundApiConfiguration>(TrakHoundApiConfiguration.ConfigurationCategory);
                            if (apiConfigurations.IsNullOrEmpty() || !apiConfigurations.Any(o => o.PackageId == package.Id))
                            {
                                var apiConfiguration = new TrakHoundApiConfiguration();
                                apiConfiguration.Id = Guid.NewGuid().ToString();
                                apiConfiguration.PackageId = package.Id;
                                apiConfiguration.PackageVersion = "*";
                                apiConfiguration.Route = package.Metadata?.GetValueOrDefault(".defaultRoute")?.ToString();
                                apiConfiguration.VolumeId = package.Metadata?.GetValueOrDefault(".defaultVolumeId")?.ToString();
                                apiConfiguration.RouterId = TrakHoundRouter.Default;
                                _configurationProfile.Add(apiConfiguration, true);
                            }
                            break;

                        case "function":
                            var functionConfigurations = _configurationProfile.Get<TrakHoundFunctionConfiguration>(TrakHoundFunctionConfiguration.ConfigurationCategory);
                            if (functionConfigurations.IsNullOrEmpty() || !functionConfigurations.Any(o => o.PackageId == package.Id))
                            {
                                var functionConfiguration = new TrakHoundFunctionConfiguration();
                                functionConfiguration.Id = Guid.NewGuid().ToString();
                                functionConfiguration.PackageId = package.Id;
                                functionConfiguration.PackageVersion = "*";
                                functionConfiguration.FunctionId = package.Id;
                                functionConfiguration.VolumeId = package.Metadata?.GetValueOrDefault(".defaultVolumeId")?.ToString();
                                functionConfiguration.RouterId = TrakHoundRouter.Default;
                                _configurationProfile.Add(functionConfiguration, true);
                            }
                            break;

                        case "service":
                            var serviceConfigurations = _configurationProfile.Get<TrakHoundServiceConfiguration>(TrakHoundServiceConfiguration.ConfigurationCategory);
                            if (serviceConfigurations.IsNullOrEmpty() || !serviceConfigurations.Any(o => o.PackageId == package.Id))
                            {
                                var serviceConfiguration = new TrakHoundServiceConfiguration();
                                serviceConfiguration.Id = Guid.NewGuid().ToString();
                                serviceConfiguration.PackageId = package.Id;
                                serviceConfiguration.PackageVersion = "*";
                                serviceConfiguration.VolumeId = package.Metadata?.GetValueOrDefault(".defaultVolumeId")?.ToString();
                                serviceConfiguration.RouterId = TrakHoundRouter.Default;
                                _configurationProfile.Add(serviceConfiguration, true);
                            }
                            break;
                    }
                }
            }
        }

        public void CleanPackages()
        {
            var installedPackages = TrakHoundPackage.GetInstalled();
            if (!installedPackages.IsNullOrEmpty())
            {
                var activePackageUuids = GetActivePackages();
                if (!activePackageUuids.IsNullOrEmpty())
                {
                    foreach (var installedPackage in installedPackages)
                    {
                        if (!activePackageUuids.Contains(installedPackage.Uuid))
                        {
                            _packageManager.Uninstall(installedPackage.Id, installedPackage.Version);
                        }
                    }
                }
                else
                {
                    foreach (var installedPackage in installedPackages)
                    {
                        _packageManager.Uninstall(installedPackage.Id, installedPackage.Version);
                    }
                }
            }
        }

        private IEnumerable<string> GetActivePackages()
        {
            var activePackageUuids = new HashSet<string>();

            // API
            var apiConfigurations = _configurationProfile.Get<TrakHoundApiConfiguration>(TrakHoundApiConfiguration.ConfigurationCategory);
            if (!apiConfigurations.IsNullOrEmpty())
            {
                foreach (var configuration in apiConfigurations)
                {
                    var packageUuid = GetPackageUuid(configuration.PackageId, configuration.PackageVersion);
                    if (packageUuid != null) activePackageUuids.Add(packageUuid);
                }
            }

            // App
            var appConfigurations = _configurationProfile.Get<TrakHoundAppConfiguration>(TrakHoundAppConfiguration.ConfigurationCategory);
            if (!appConfigurations.IsNullOrEmpty())
            {
                foreach (var configuration in appConfigurations)
                {
                    var packageUuid = GetPackageUuid(configuration.PackageId, configuration.PackageVersion);
                    if (packageUuid != null) activePackageUuids.Add(packageUuid);
                }
            }

            // Driver
            var driverConfigurations = _configurationProfile.Get<TrakHoundDriverConfiguration>(TrakHoundDriverConfiguration.ConfigurationCategory);
            if (!driverConfigurations.IsNullOrEmpty())
            {
                foreach (var configuration in driverConfigurations)
                {
                    var packageUuid = GetPackageUuid(configuration.PackageId, configuration.PackageVersion);
                    if (packageUuid != null) activePackageUuids.Add(packageUuid);
                }
            }

            // Function
            var functionConfigurations = _configurationProfile.Get<TrakHoundFunctionConfiguration>(TrakHoundFunctionConfiguration.ConfigurationCategory);
            if (!functionConfigurations.IsNullOrEmpty())
            {
                foreach (var configuration in functionConfigurations)
                {
                    var packageUuid = GetPackageUuid(configuration.PackageId, configuration.PackageVersion);
                    if (packageUuid != null) activePackageUuids.Add(packageUuid);
                }
            }

            // Identity Provider
            var identityConfigurations = _configurationProfile.Get<TrakHoundIdentityProviderConfiguration>(TrakHoundIdentityProviderConfiguration.ConfigurationCategory);
            if (!identityConfigurations.IsNullOrEmpty())
            {
                foreach (var configuration in identityConfigurations)
                {
                    var packageUuid = GetPackageUuid(configuration.PackageId, configuration.PackageVersion);
                    if (packageUuid != null) activePackageUuids.Add(packageUuid);
                }
            }

            // Service
            var serviceConfigurations = _configurationProfile.Get<TrakHoundServiceConfiguration>(TrakHoundServiceConfiguration.ConfigurationCategory);
            if (!serviceConfigurations.IsNullOrEmpty())
            {
                foreach (var configuration in serviceConfigurations)
                {
                    var packageUuid = GetPackageUuid(configuration.PackageId, configuration.PackageVersion);
                    if (packageUuid != null) activePackageUuids.Add(packageUuid);
                }
            }

            return activePackageUuids;
        }

        private string GetPackageUuid(string packageId, string packageVersion)
        {
            TrakHoundPackage package;

            if (packageVersion.Contains('*'))
            {
                package = _packageManager.GetLatest(packageId);               
            }
            else
            {
                package = _packageManager.Get(packageId, packageVersion);
            }

            if (package != null)
            {
                return package.Uuid;
            }

            return null;
        }

        private static string GetAddress()
        {
            try
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        return ip.ToString();
                    }
                }
            }
            catch { }

            return null;
        }
    }
}