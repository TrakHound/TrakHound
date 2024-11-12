// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Api;
using TrakHound.Apps;
using TrakHound.Drivers;
using TrakHound.Functions;
using TrakHound.Instances;
using TrakHound.Routing;
using TrakHound.Services;

namespace TrakHound.Clients
{
    public partial class TrakHoundInstanceClient : TrakHoundClientBase, ITrakHoundClient
    {
        private readonly ITrakHoundInstance _instance;
        private readonly TrakHoundRouterProvider _routerProvider;
        private readonly TrakHoundInstanceApiClient _api;
        private readonly TrakHoundInstanceAppsClient _apps;
        private readonly TrakHoundInstanceEntitiesClient _entities;
        private readonly TrakHoundFunctionsClient _functions;
        private readonly TrakHoundServicesClient _services;
        private readonly TrakHoundInstanceSystemClient _system;


        public string RouterId { get; set; }

        public ITrakHoundInstance Instance => _instance;

        public ITrakHoundApiClient Api => _api;

        public ITrakHoundAppsClient Apps => _apps;

        public ITrakHoundEntitiesClient Entities => _entities;

        public ITrakHoundFunctionsClient Functions => _functions;

        public ITrakHoundServicesClient Services => _services;

        public ITrakHoundSystemClient System => _system;



        public TrakHoundInstanceClient(
            ITrakHoundInstance instance,
            TrakHoundDriverProvider driverProvider,
            TrakHoundRouterProvider routerProvider,
            ITrakHoundApiProvider apiProvider,
            ITrakHoundAppProvider appProvider,
            TrakHoundFunctionManager functionManager,
            TrakHoundServiceManager serviceManager
            )
        {
            Client = this;

            _instance = instance;
            _routerProvider = routerProvider;

            _system = new TrakHoundInstanceSystemClient(this, instance, apiProvider, appProvider, driverProvider, routerProvider, functionManager, serviceManager);

            _api = new TrakHoundInstanceApiClient(apiProvider);
            _apps = new TrakHoundInstanceAppsClient(appProvider);
            _entities = new TrakHoundInstanceEntitiesClient(this);
            _functions = new TrakHoundFunctionsClient(this);
            _services = new TrakHoundServicesClient(this);
        }


        internal TrakHoundRouter GetRouter(string routerId)
        {
            return _routerProvider.GetRouter(!string.IsNullOrEmpty(routerId) ? routerId : RouterId);
        }
    }
}
