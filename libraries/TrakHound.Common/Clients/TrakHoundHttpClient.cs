// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Configurations;

namespace TrakHound.Clients
{
    public class TrakHoundHttpClient : TrakHoundClientBase, ITrakHoundClient
    {
        private readonly string _httpBaseUrl;
        private readonly string _webSocketsBaseUrl;
        private readonly string _routerId;
        private readonly TrakHoundHttpClientConfiguration _clientConfiguration;
        private readonly TrakHoundHttpApiClient _api;
        private readonly TrakHoundHttpAppsClient _apps;
        private readonly TrakHoundEntitiesHttpClient _entities;
        private readonly TrakHoundFunctionsClient _functions;
        private readonly TrakHoundServicesClient _services;
        private readonly TrakHoundHttpSystemClient _system;


        public TrakHoundHttpClientConfiguration ClientConfiguration => _clientConfiguration;

        public string HttpBaseUrl => _httpBaseUrl;

        public string WebSocketsBaseUrl => _webSocketsBaseUrl;

        public string RouterId { get; set; }

        public ITrakHoundApiClient Api => _api;

        public ITrakHoundAppsClient Apps => _apps;

        public ITrakHoundEntitiesClient Entities => _entities;

        public ITrakHoundFunctionsClient Functions => _functions;

        public ITrakHoundServicesClient Services => _services;

        public ITrakHoundSystemClient System => _system;


        public TrakHoundHttpClient(
            string hostname, 
            int port = TrakHoundHttpClientConfiguration.DefaultPort,
            string path = TrakHoundHttpClientConfiguration.DefaultPath,
            bool useSSL = TrakHoundHttpClientConfiguration.DefaultUseSSL,
            string routerId = null
            )
        {
            Client = this;

            var clientConfiguration = new TrakHoundHttpClientConfiguration(hostname, port, path, useSSL);
            _clientConfiguration = clientConfiguration;

            _httpBaseUrl = clientConfiguration.GetHttpBaseUrl();
            _webSocketsBaseUrl = clientConfiguration.GetWebSocketBaseUrl();
            _routerId = routerId;

            _system = new TrakHoundHttpSystemClient(this);

            _api = new TrakHoundHttpApiClient(this);
            _apps = new TrakHoundHttpAppsClient(this);
            _entities = new TrakHoundEntitiesHttpClient(this);
            _functions = new TrakHoundFunctionsClient(this);
            _services = new TrakHoundServicesClient(this);
        }

        public TrakHoundHttpClient(TrakHoundHttpClientConfiguration clientConfiguration, string routerId = null)
        {
            Client = this;

            // Set Client Configuration
            _clientConfiguration = clientConfiguration;
            if (clientConfiguration != null)
            {
                _httpBaseUrl = clientConfiguration.GetHttpBaseUrl();
                _webSocketsBaseUrl = clientConfiguration.GetWebSocketBaseUrl();

                _routerId = routerId;
            }

            _system = new TrakHoundHttpSystemClient(this);

            _api = new TrakHoundHttpApiClient(this);
            _apps = new TrakHoundHttpAppsClient(this);
            _entities = new TrakHoundEntitiesHttpClient(this);
            _functions = new TrakHoundFunctionsClient(this);
            _services = new TrakHoundServicesClient(this);
        }


        internal string GetRouterId(string routerId)
        {
            string x = null;
            if (!string.IsNullOrEmpty(routerId)) x = routerId;
            else x = RouterId;
            return x != "default" ? x : null;
        }
    }
}
