// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Routing;

namespace TrakHound.Clients
{
    public class TrakHoundInstanceSystemRoutersClient : ITrakHoundSystemRoutersClient
    {
        private readonly TrakHoundRouterProvider _routerProvider;


        public TrakHoundInstanceSystemRoutersClient(TrakHoundRouterProvider routerProvider)
        {
            _routerProvider = routerProvider;
        }


		public async Task<IEnumerable<ITrakHoundRouterInformation>> GetRouters()
        {
            var informations = _routerProvider.GetInformation();
            if (!informations.IsNullOrEmpty())
            {
                var results = new List<ITrakHoundRouterInformation>();
                foreach (var information in informations)
                {
                    results.Add(information);
                }
                return results;
            }

            return null;
        }

        public async Task<ITrakHoundRouterInformation> GetRouter(string routerId)
        {
            return _routerProvider.GetInformation(routerId);
        }
    }
}
