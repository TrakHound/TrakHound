// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Routing;

namespace TrakHound.Clients
{
    public interface ITrakHoundSystemRoutersClient
    {
        Task<IEnumerable<ITrakHoundRouterInformation>> GetRouters();

        Task<ITrakHoundRouterInformation> GetRouter(string routerId);
    }
}
