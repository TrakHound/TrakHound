// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using TrakHound.Api;
using TrakHound.Apps;
using TrakHound.Drivers;
using TrakHound.Functions;
using TrakHound.Instances;
using TrakHound.Routing;
using TrakHound.Services;

namespace TrakHound.Clients
{
    internal class TrakHoundInstanceSystemClient : ITrakHoundSystemClient
    {
        private readonly TrakHoundInstanceClient _baseClient;
        private readonly TrakHoundInstanceSystemApiClient _api;
        private readonly TrakHoundInstanceSystemBlobsClient _blobs;
        private readonly TrakHoundInstanceSystemCommandsClient _commands;
        private readonly TrakHoundInstanceSystemDriversClient _drivers;
        private readonly TrakHoundInstanceSystemEntitiesClient _entities;
        private readonly TrakHoundInstanceSystemFunctionsClient _functions;
        private readonly TrakHoundInstanceSystemInstancesClient _instances;
        private readonly TrakHoundInstanceSystemMessagesClient _messages;
        private readonly TrakHoundInstanceSystemMessageQueuesClient _messageQueues;
        private readonly TrakHoundInstanceSystemRoutersClient _routers;
        private readonly TrakHoundInstanceSystemServicesClient _services;


        public TrakHoundConnectionStatus ConnectionStatus
        {
            get
            {
                var status = new TrakHoundConnectionStatus();
                status.IsConnected = true;

                var timestamp = _baseClient.Instance.StartTime;
                status.Timestamp = timestamp != null ? timestamp.Value.ToUnixTime() : 0;

                return status;
            }
        }

        public event EventHandler<TrakHoundConnectionStatus> ConnectionStatusUpdated;


        public ITrakHoundSystemApiClient Api => _api;

        public ITrakHoundSystemBlobsClient Blobs => _blobs;

        public ITrakHoundSystemCommandsClient Commands => _commands;

        public ITrakHoundSystemDriversClient Drivers => _drivers;

        public ITrakHoundSystemEntitiesClient Entities => _entities;

        public ITrakHoundSystemFunctionsClient Functions => _functions;

        public ITrakHoundSystemInstancesClient Instances => _instances;

        public ITrakHoundSystemMessagesClient Messages => _messages;

        public ITrakHoundSystemMessageQueuesClient MessageQueues => _messageQueues;

        public ITrakHoundSystemRoutersClient Routers => _routers;

		public ITrakHoundSystemServicesClient Services => _services;


        public TrakHoundInstanceSystemClient(
            TrakHoundInstanceClient baseClient,
            ITrakHoundInstance instance,
            ITrakHoundApiProvider apiProvider,
            ITrakHoundAppProvider appProvider,
            TrakHoundDriverProvider driverProvider,
            TrakHoundRouterProvider routerProvider,
            TrakHoundFunctionManager functionManager,
            TrakHoundServiceManager serviceManager
            )
        {
            _baseClient = baseClient;
            _api = new TrakHoundInstanceSystemApiClient(apiProvider);
            _blobs = new TrakHoundInstanceSystemBlobsClient(baseClient);
            _commands = new TrakHoundInstanceSystemCommandsClient(baseClient);
            _drivers = new TrakHoundInstanceSystemDriversClient(driverProvider);
            _entities = new TrakHoundInstanceSystemEntitiesClient(baseClient);
            _functions = new TrakHoundInstanceSystemFunctionsClient(functionManager);
            _instances = new TrakHoundInstanceSystemInstancesClient(instance, instance.LogProvider);
            _messages = new TrakHoundInstanceSystemMessagesClient(baseClient);
            _messageQueues = new TrakHoundInstanceSystemMessageQueuesClient(baseClient);
            _routers = new TrakHoundInstanceSystemRoutersClient(routerProvider);
            _services = new TrakHoundInstanceSystemServicesClient(serviceManager, instance.LogProvider);
        }
    }
}
