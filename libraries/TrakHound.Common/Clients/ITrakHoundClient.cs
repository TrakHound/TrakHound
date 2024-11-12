// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace TrakHound.Clients
{
    public partial interface ITrakHoundClient
    {
        string RouterId { get; set; }

        IEnumerable<ITrakHoundClientMiddleware> Middleware { get; }


        ITrakHoundApiClient Api { get; }

        ITrakHoundAppsClient Apps { get; }

        ITrakHoundEntitiesClient Entities { get; }

        ITrakHoundFunctionsClient Functions { get; }

        ITrakHoundServicesClient Services { get; }

        ITrakHoundSystemClient System { get; }


        void AddMiddleware(ITrakHoundClientMiddleware middleware);
    }
}
