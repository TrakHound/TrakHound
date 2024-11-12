// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Clients
{
    internal class TrakHoundSystemEntitiesMiddlewareClient : TrakHoundSystemEntitiesClientBase, ITrakHoundSystemEntitiesClient
    {
        public TrakHoundSystemEntitiesMiddlewareClient(ITrakHoundSystemEntitiesClient client)
        {
            _definitions = client.Definitions;
            _objects = client.Objects;
            _sources = client.Sources;
        }
    }
}
