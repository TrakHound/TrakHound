// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrakHound.Http;
using TrakHound.Routing;

namespace TrakHound.Clients
{
    internal class TrakHoundHttpSystemRoutersClient : ITrakHoundSystemRoutersClient
    {
        private readonly TrakHoundHttpClient _baseClient;


        public TrakHoundHttpSystemRoutersClient(TrakHoundHttpClient baseClient)
        {
            _baseClient = baseClient;
        }


        public async Task<IEnumerable<ITrakHoundRouterInformation>> GetRouters()
        {
            var url = Url.Combine(_baseClient.HttpBaseUrl, HttpConstants.RoutersPrefix);

            var response = await RestRequest.Get<IEnumerable<TrakHoundHttpRouterInformation>>(url);
            if (!response.IsNullOrEmpty())
            {
                var routerInformations = new List<ITrakHoundRouterInformation>();
                foreach (var x in response) routerInformations.Add(x);
                return routerInformations;
            }

            return Enumerable.Empty<ITrakHoundRouterInformation>();
        }

        public async Task<ITrakHoundRouterInformation> GetRouter(string routerId)
        {
            var url = Url.Combine(_baseClient.HttpBaseUrl, HttpConstants.RoutersPrefix);
            url = Url.Combine(url, routerId);

            return await RestRequest.Get<TrakHoundHttpRouterInformation>(url);
        }
    }
}
