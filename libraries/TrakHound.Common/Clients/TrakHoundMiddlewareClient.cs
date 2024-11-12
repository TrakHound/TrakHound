// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Clients
{
    internal class TrakHoundMiddlewareClient : TrakHoundClientBase, ITrakHoundClient
    {
        private readonly ITrakHoundApiClient _api;
        private readonly ITrakHoundAppsClient _apps;
        private readonly ITrakHoundEntitiesClient _entities;
        private readonly ITrakHoundFunctionsClient _functions;
        private readonly ITrakHoundServicesClient _services;
        private readonly ITrakHoundSystemClient _system;


        public string RouterId { get; set; }

        public ITrakHoundApiClient Api => _api;

        public ITrakHoundAppsClient Apps => _apps;

        public ITrakHoundEntitiesClient Entities => _entities;

        public ITrakHoundFunctionsClient Functions => _functions;

        public ITrakHoundServicesClient Services => _services;

        public ITrakHoundSystemClient System => _system;


        public TrakHoundMiddlewareClient(ITrakHoundClient client)
        {
            Client = this;

            RouterId = client.RouterId;

            _system = new TrakHoundSystemMiddlewareClient(client.System);

            _api = client.Api;
            _apps = client.Apps;
            _entities = new TrakHoundEntitiesMiddlewareClient(this);
            _functions = client.Functions;
            _services = client.Services;
        }
    }
}
