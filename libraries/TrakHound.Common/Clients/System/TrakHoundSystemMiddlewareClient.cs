// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;

namespace TrakHound.Clients
{
    internal class TrakHoundSystemMiddlewareClient : ITrakHoundSystemClient
    {
        private readonly ITrakHoundSystemClient _client;
        private readonly ITrakHoundSystemApiClient _api;
        private readonly ITrakHoundSystemBlobsClient _blobs;
        private readonly ITrakHoundSystemCommandsClient _commands;
        private readonly ITrakHoundSystemDriversClient _drivers;
        private readonly ITrakHoundSystemEntitiesClient _entities;
        private readonly ITrakHoundSystemFunctionsClient _functions;
        private readonly ITrakHoundSystemInstancesClient _instances;
        private readonly ITrakHoundSystemMessagesClient _messages;
        private readonly ITrakHoundSystemMessageQueuesClient _messageQueues;
        private readonly ITrakHoundSystemRoutersClient _routers;
        private readonly ITrakHoundSystemServicesClient _services;


        public TrakHoundConnectionStatus ConnectionStatus => _client.ConnectionStatus;


        public event EventHandler<TrakHoundConnectionStatus> ConnectionStatusUpdated
        {
            add { _client.ConnectionStatusUpdated += value; }
            remove { _client.ConnectionStatusUpdated -= value; }
        }


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


        public TrakHoundSystemMiddlewareClient(ITrakHoundSystemClient client)
        {
            _client = client;

            _api = client.Api;
            _blobs = client.Blobs;
            _commands = client.Commands;
            _drivers = client.Drivers;
            _entities = new TrakHoundSystemEntitiesMiddlewareClient(client.Entities);
            _functions = client.Functions;
            _instances = client.Instances;
            _messages = client.Messages;
            _messageQueues = client.MessageQueues;
            _routers = client.Routers;
            _services = client.Services;
        }
    }
}
