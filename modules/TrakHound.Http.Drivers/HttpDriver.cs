// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Clients;
using TrakHound.Drivers;

namespace TrakHound.Http
{
    public abstract class HttpDriver : TrakHoundDriver
    {
        private readonly TrakHoundHttpClient _client;
        private readonly string _instanceId;


        public override bool IsAvailable => true;

        public string InstanceId => _instanceId;

        protected TrakHoundHttpClient Client => _client;


        public HttpDriver() 
        {
            _instanceId = Guid.NewGuid().ToString();
        }

        public HttpDriver(ITrakHoundDriverConfiguration configuration) : base(configuration)
        {
            _instanceId = Guid.NewGuid().ToString();

            var clientConfiguration = new TrakHoundHttpClientConfiguration();
            clientConfiguration.Hostname = configuration.GetParameter("hostname");
            clientConfiguration.Port = configuration.GetParameter<int>("port");
            clientConfiguration.Path = configuration.GetParameter("path");
            clientConfiguration.UseSSL = configuration.GetParameter<bool>("useSsl");

            _client = new TrakHoundHttpClient(clientConfiguration);
        }
    }
}
