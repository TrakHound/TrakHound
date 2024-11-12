// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Components;
using System.Reflection;
using TrakHound.Apps;
using TrakHound.Clients;
using TrakHound.Configurations;
using TrakHound.Logging;
using TrakHound.Modules;
using TrakHound.Volumes;

namespace TrakHound.Blazor.Routing
{
    public class TrakHoundPageRouteManager<T> : TrakHoundPageRouteManager
    {
        public TrakHoundPageRouteManager(
            string baseUrl,
            ITrakHoundConfigurationProfile configurationProfile,
            ITrakHoundModuleProvider moduleProvider, 
            string packageCategory,
            TrakHoundModuleContext context,
            ITrakHoundClientProvider clientProvider,
            ITrakHoundVolumeProvider volumeProvider
            )
            : base(baseUrl)
        {
            _moduleManager = moduleProvider.Get<T>(packageCategory, context);

            _configurationProfile = configurationProfile;
            _clientProvider = clientProvider;
            _volumeProvider = volumeProvider;
        }
    }


    public abstract class TrakHoundPageRouteManager : ITrakHoundPageRouteManager
    {
        private const string _packagesDirectory = "_packages";
        private static readonly RouteComparer _routeComparer = new RouteComparer();


        private readonly string _baseUrl;
        private readonly ListDictionary<string, Route> _routes = new ListDictionary<string, Route>();
        private readonly Dictionary<string, ITrakHoundAppConfiguration> _appConfigurations = new Dictionary<string, ITrakHoundAppConfiguration>();
        private readonly DelayEvent _updateDelay = new DelayEvent(2000);
        private readonly object _lock = new object();

        protected ITrakHoundConfigurationProfile _configurationProfile;
        protected ITrakHoundModuleManager _moduleManager;
        protected ITrakHoundClientProvider _clientProvider;
        protected ITrakHoundVolumeProvider _volumeProvider;


        public IEnumerable<Route> Routes => _routes.Values;


        public event EventHandler<string> AppUpdated; // AppId as argument


        public TrakHoundPageRouteManager(string baseUrl)
        {
            _baseUrl = baseUrl;
            _updateDelay.Elapsed += UpdateDelayElapsed;
        }


        private void UpdateDelayElapsed(object sender, EventArgs e)
        {
            Initialise();
        }

        protected void Update(object sender, ITrakHoundModule module)
        {
            _updateDelay.Set();
        }


        public void AddApp(ITrakHoundAppConfiguration configuration)
        {
            if (configuration != null && configuration.Id != null)
            {
                lock (_lock)
                {
                    _appConfigurations.Remove(configuration.Id);
                    _appConfigurations.Add(configuration.Id, configuration);
                }

                if (AppUpdated != null) AppUpdated.Invoke(this, configuration.Id);
            }
        }

        public void RemoveApp(string configurationId)
        {
            if (configurationId != null)
            {
                lock (_lock) _appConfigurations.Remove(configurationId);
            }
        }


        public void ClearPages()
        {
            lock (_lock) _appConfigurations.Clear();
        }


        public void Initialise()
        {
            lock (_lock) _routes.Clear();

            InitializeAssemblyRoutes();
            InitializePackageRoutes();
        }

        private void InitializeAssemblyRoutes()
        {
            var pageComponentTypes = Assembly.GetEntryAssembly().ExportedTypes.Where(t => t.IsSubclassOf(typeof(ComponentBase)));
            if (pageComponentTypes != null)
            {
                var routes = new List<Route>();

                foreach (var pageComponentType in pageComponentTypes)
                {
                    var routeAttributes = pageComponentType.GetCustomAttributes<RouteAttribute>();
                    if (!routeAttributes.IsNullOrEmpty())
                    {
                        foreach (var routeAttribute in routeAttributes)
                        {
                            if (!string.IsNullOrEmpty(routeAttribute.Template))
                            {
                                var newRoute = new Route
                                {
                                    Template = routeAttribute.Template,
                                    Handler = pageComponentType
                                };

                                if (!string.IsNullOrEmpty(newRoute.AppId))
                                {
                                    _routes.Add(newRoute.AppId, newRoute);
                                }
                                else
                                {
                                    var appId = Guid.NewGuid().ToString();
                                    _routes.Add(appId, newRoute);
                                }
                            }
                        }                            
                    }
                }
            }
        }

        private void InitializePackageRoutes()
        {
            if (!_appConfigurations.IsNullOrEmpty())
            {
                var appConfigurations = _appConfigurations.Values.ToList();
                foreach (var appConfiguration in appConfigurations)
                {
                    var baseRoute = appConfiguration.Route;
                    if (!baseRoute.StartsWith('/')) baseRoute = "/" + baseRoute;

                    var module = _moduleManager.Get(appConfiguration.PackageId, appConfiguration.PackageVersion);
                    if (module != null)
                    {
                        // Get Client
                        var client = _clientProvider.GetClient();
                        client.RouterId = appConfiguration.RouterId;

                        // Get Volume
                        var volumeId = appConfiguration.VolumeId ?? appConfiguration.Id;
                        var volume = _volumeProvider.GetVolume(volumeId);

                        // Create Logger
                        var logger = new TrakHoundLogger(appConfiguration.Id);

                        foreach (var moduleType in module.ModuleTypes)
                        {
                            var routeAttributes = moduleType.GetCustomAttributes<RouteAttribute>();
                            if (!routeAttributes.IsNullOrEmpty())
                            {
                                foreach (var routeAttribute in routeAttributes)
                                {
                                    if (!string.IsNullOrEmpty(routeAttribute.Template))
                                    {
                                        var newRoute = new Route
                                        {
                                            AppId = appConfiguration.Id,
                                            AppName = appConfiguration.Name,
                                            Package = module.Package,
                                            Client = client,
                                            Volume = volume,
                                            Logger = logger,
                                            BaseUrl = _baseUrl,
                                            BasePath = appConfiguration.Route,
                                            BaseLocation = $"{_packagesDirectory}/{module.PackageId}/{module.PackageVersion}",
                                            Template = $"/{Url.Combine(baseRoute, routeAttribute.Template).TrimEnd('/')}",
                                            Handler = moduleType
                                        };

                                        _routes.Add(newRoute.Id, newRoute);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public MatchResult Match(string url)
        {
            var routes = GetOrderedRoutes();
            if (!routes.IsNullOrEmpty())
            {
                foreach (var route in routes)
                {
                    var matchResult = route.Match(url);
                    if (matchResult.IsMatch)
                    {
                        return matchResult;
                    }
                }
            }

            return MatchResult.NoMatch();
        }


        private IEnumerable<Route> GetOrderedRoutes()
        {
            var routes = _routes.Values;
            if (!routes.IsNullOrEmpty())
            {
                return routes.OrderBy(o => o.Template, _routeComparer);
            }

            return null;
        }

        public class RouteComparer : IComparer<string>
        {
            public int Compare(string x, string y)
            {
                if (x == y) return 0;
                if (x.Contains('{') && !y.Contains('{')) return 1;
                if (!x.Contains('{') && y.Contains('{')) return -1;

                return x.CompareTo(y);
            }
        }
    }
}
