// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrakHound.Configurations;
using TrakHound.Http;
using TrakHound.Routing;

namespace TrakHound.Clients
{
    public class TrakHoundHttpClientProvider : ITrakHoundClientProvider
    {
        private readonly TrakHoundHttpClientConfiguration _clientConfiguration;


        public TrakHoundHttpClientConfiguration ClientConfiguration => _clientConfiguration;


        public TrakHoundHttpClientProvider(TrakHoundHttpClientConfiguration clientConfiguration)
        {
            _clientConfiguration = clientConfiguration;
        }

        public ITrakHoundClient GetClient()
        {
            return new TrakHoundHttpClient(_clientConfiguration);
        }

        public async Task<IEnumerable<ITrakHoundRouterInformation>> GetRouters()
        {
            var url = Url.Combine(_clientConfiguration.GetHttpBaseUrl(), HttpConstants.RoutersPrefix);

            var response = await RestRequest.Get<IEnumerable<TrakHoundHttpRouterInformation>>(url);
            if (!response.IsNullOrEmpty())
            {
                var routerInformations = new List<ITrakHoundRouterInformation>();
                foreach (var x in response) routerInformations.Add(x);
                return routerInformations;
            }

            return Enumerable.Empty<ITrakHoundRouterInformation>();
        }
    }
}
