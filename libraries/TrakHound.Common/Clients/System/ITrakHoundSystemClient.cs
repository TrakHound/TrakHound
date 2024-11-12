// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;

namespace TrakHound.Clients
{
    public interface ITrakHoundSystemClient
    {
        TrakHoundConnectionStatus ConnectionStatus { get; }

        event EventHandler<TrakHoundConnectionStatus> ConnectionStatusUpdated;


        ITrakHoundSystemApiClient Api { get; }

        ITrakHoundSystemBlobsClient Blobs { get; }

        ITrakHoundSystemCommandsClient Commands { get; }

        ITrakHoundSystemDriversClient Drivers { get; }

        ITrakHoundSystemEntitiesClient Entities { get; }

        ITrakHoundSystemFunctionsClient Functions { get; }

        ITrakHoundSystemInstancesClient Instances { get; }

        ITrakHoundSystemMessagesClient Messages { get; }

        ITrakHoundSystemMessageQueuesClient MessageQueues { get; }

        ITrakHoundSystemRoutersClient Routers { get; }

        ITrakHoundSystemServicesClient Services { get; }
    }
}
