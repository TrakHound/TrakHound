// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Api;
using TrakHound.Apps;
using TrakHound.Blazor.ExplorerInternal.Components;
using TrakHound.Clients;
using TrakHound.Functions;
using TrakHound.Instances;
using TrakHound.Routing;
using TrakHound.Services;

namespace TrakHound.Blazor.ExplorerInternal.Services
{
    public class ExplorerService : ITrakHoundScopedAppInjectionService
    {
        private readonly Dictionary<string, ITrakHoundClientProvider> _trakhoundClientProdivers = new Dictionary<string, ITrakHoundClientProvider>();
        private readonly Dictionary<string, IEnumerable<ITrakHoundRouterInformation>> _routers = new Dictionary<string, IEnumerable<ITrakHoundRouterInformation>>();
        private readonly Dictionary<string, IEnumerable<TrakHoundApiInformation>> _apiRoutes = new Dictionary<string, IEnumerable<TrakHoundApiInformation>>();
        private readonly Dictionary<string, IEnumerable<TrakHoundApiInformation>> _apiPackages = new Dictionary<string, IEnumerable<TrakHoundApiInformation>>();
        private readonly Dictionary<string, IEnumerable<TrakHoundAppInformation>> _apps = new Dictionary<string, IEnumerable<TrakHoundAppInformation>>();
        private readonly Dictionary<string, IEnumerable<TrakHoundFunctionInformation>> _functions = new Dictionary<string, IEnumerable<TrakHoundFunctionInformation>>();
        private readonly Dictionary<string, IEnumerable<TrakHoundServiceInformation>> _services = new Dictionary<string, IEnumerable<TrakHoundServiceInformation>>();
        private readonly Dictionary<string, string> _serverUrls = new Dictionary<string, string>();
        private readonly object _lock = new object();

        private IEnumerable<TrakHoundInstanceInformation> _instances;
        private bool _isLoaded;


        public IEnumerable<TrakHoundInstanceInformation> Instances => _instances;

        private IEnumerable<BreadcrumbItem> _breadcrumbItems;
        public IEnumerable<BreadcrumbItem> BreadcrumbItems
        {
            get => _breadcrumbItems;
            set
            {
                _breadcrumbItems = value;
                if (PageUpdated != null) PageUpdated.Invoke(this, new EventArgs());
            }
        }

        public string SelectedInstanceId { get; set; }

        public string SelectedRouterId { get; set; }

        public EventHandler PageUpdated { get; set; }

        public EventHandler ConfigurationUpdated { get; set; }

        public EventHandler Loaded { get; set; }

        public bool IsLoaded => _isLoaded;


        public async Task LoadInstance(IInstanceManager instanceManager, string instanceId, bool forceReload = false)
        {
            if (instanceManager != null && !string.IsNullOrEmpty(instanceId))
            {
                if (!_isLoaded || forceReload)
                {
                    _isLoaded = true;

                    var instance = instanceManager.GetInstance(instanceId);
                    if (instance != null)
                    {
                        var client = instanceManager.GetClient(instanceId);
                        if (client != null)
                        {
                            var instances = instanceManager.GetInstances();
                            var routers = await client.System.Routers.GetRouters();
                            var apiRouteInformations = await client.System.Api.GetRouteInformation();
                            var appInformations = await client.Apps.GetInformation();
                            var functions = await client.System.Functions.GetInformation();
                            var services = await client.System.Services.GetInformation();

                            lock (_lock)
                            {
                                _instances = instances;

                                _routers.Remove(instance.Id);
                                _routers.Add(instance.Id, routers);

                                _apps.Remove(instance.Id);
                                _apps.Add(instance.Id, appInformations);

                                _apiRoutes.Remove(instance.Id);
                                _apiRoutes.Add(instance.Id, apiRouteInformations);

                                _functions.Remove(instance.Id);
                                _functions.Add(instance.Id, functions);

                                _services.Remove(instance.Id);
                                _services.Add(instance.Id, services);
                            }

                            if (ConfigurationUpdated != null) ConfigurationUpdated.Invoke(this, new EventArgs());
                        }
                    }
                }
            }
        }


        public IEnumerable<ITrakHoundRouterInformation> GetRouters(string instanceId)
        {
            if (!string.IsNullOrEmpty(instanceId))
            {
                if (_routers.ContainsKey(instanceId))
                {
                    return _routers[instanceId];
                }
            }

            return null;
        }

        public ITrakHoundRouterInformation GetRouter(string instanceId, string routerId)
        {
            if (!string.IsNullOrEmpty(instanceId))
            {
                if (_routers.ContainsKey(instanceId))
                {
                    var routers = _routers[instanceId];
                    return routers?.FirstOrDefault(o => o.Id == routerId);
                }
            }

            return null;
        }

        public IEnumerable<TrakHoundApiInformation> GetApiRoutes(string instanceId)
        {
            if (!string.IsNullOrEmpty(instanceId))
            {
                if (_apiRoutes.ContainsKey(instanceId))
                {
                    return _apiRoutes[instanceId];
                }
            }

            return null;
        }

        public IEnumerable<TrakHoundApiInformation> GetApiPackages(string instanceId)
        {
            if (!string.IsNullOrEmpty(instanceId))
            {
                if (_apiPackages.ContainsKey(instanceId))
                {
                    return _apiPackages[instanceId];
                }
            }

            return null;
        }

        public IEnumerable<TrakHoundAppInformation> GetApps(string instanceId)
        {
            if (!string.IsNullOrEmpty(instanceId))
            {
                if (_apps.ContainsKey(instanceId))
                {
                    return _apps[instanceId];
                }
            }

            return null;
        }

        public IEnumerable<TrakHoundFunctionInformation> GetFunctions(string instanceId)
        {
            if (!string.IsNullOrEmpty(instanceId))
            {
                if (_functions.ContainsKey(instanceId))
                {
                    return _functions[instanceId];
                }
            }

            return null;
        }

        public IEnumerable<TrakHoundServiceInformation> GetServices(string instanceId)
        {
            if (!string.IsNullOrEmpty(instanceId))
            {
                if (_services.ContainsKey(instanceId))
                {
                    return _services[instanceId];
                }
            }

            return null;
        }

        public TrakHoundInstanceInformation GetServer(string instanceId)
        {
            if (!string.IsNullOrEmpty(instanceId) && !_instances.IsNullOrEmpty())
            {
                return _instances.FirstOrDefault(o => o.Id == instanceId);
            }

            return null;
        }

        public string GetServerUrl(string instanceId)
        {
            if (!string.IsNullOrEmpty(instanceId))
            {
                if (_serverUrls.ContainsKey(instanceId))
                {
                    return _serverUrls[instanceId];
                }
            }

            return null;
        }


        public ITrakHoundClient GetClient(string instanceId, string routerId)
        {
            if (!string.IsNullOrEmpty(instanceId))
            {
                if (_trakhoundClientProdivers.ContainsKey(instanceId))
                {
                    var clientProvider = _trakhoundClientProdivers[instanceId];
                    if (clientProvider != null)
                    {
                        var client = clientProvider.GetClient();
                        client.RouterId = routerId;

                        return client;
                    }
                }
            }

            return null;
        }

        public ITrakHoundClientProvider GetClientProvider(string instanceId)
        {
            if (!string.IsNullOrEmpty(instanceId))
            {
                if (_trakhoundClientProdivers.ContainsKey(instanceId))
                {
                    return _trakhoundClientProdivers[instanceId];
                }
            }

            return null;
        }
    }
}
