// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;

namespace TrakHound.Clients
{
    internal class TrakHoundHttpSystemClient : ITrakHoundSystemClient
    {
        private readonly TrakHoundHttpSystemApiClient _api;
        private readonly TrakHoundHttpSystemBlobsClient _blobs;
        private readonly TrakHoundHttpSystemCommandsClient _commands;
        private readonly TrakHoundHttpSystemDriversClient _drivers;
        private readonly TrakHoundHttpSystemEntitiesClient _entities;
        private readonly TrakHoundHttpSystemFunctionsClient _functions;
        private readonly TrakHoundHttpSystemInstancesClient _instances;
        private readonly TrakHoundHttpSystemMessagesClient _messages;
        private readonly TrakHoundHttpSystemMessageQueuesClient _messageQueues;
        private readonly TrakHoundHttpSystemRoutersClient _routers;
        private readonly TrakHoundHttpSystemServicesClient _services;

        private TrakHoundConnectionStatus _connectionStatus;


        public TrakHoundConnectionStatus ConnectionStatus => _connectionStatus;

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


        public TrakHoundHttpSystemClient(TrakHoundHttpClient baseClient)
        {
            _api = new TrakHoundHttpSystemApiClient(baseClient);
            _blobs = new TrakHoundHttpSystemBlobsClient(baseClient);
            _commands = new TrakHoundHttpSystemCommandsClient(baseClient);
            _drivers = new TrakHoundHttpSystemDriversClient(baseClient);
            _entities = new TrakHoundHttpSystemEntitiesClient(baseClient);
            _functions = new TrakHoundHttpSystemFunctionsClient(baseClient);
            _instances = new TrakHoundHttpSystemInstancesClient(baseClient);
            _messages = new TrakHoundHttpSystemMessagesClient(baseClient);
            _messageQueues = new TrakHoundHttpSystemMessageQueuesClient(baseClient);
            _routers = new TrakHoundHttpSystemRoutersClient(baseClient);
            _services = new TrakHoundHttpSystemServicesClient(baseClient);
        }
    }
}
