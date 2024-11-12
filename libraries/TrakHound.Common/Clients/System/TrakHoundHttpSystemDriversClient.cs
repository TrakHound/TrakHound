// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Drivers;
using TrakHound.Http;

namespace TrakHound.Clients
{
    internal class TrakHoundHttpSystemDriversClient : ITrakHoundSystemDriversClient
    {
        private readonly TrakHoundHttpClient _baseClient;


        public TrakHoundHttpSystemDriversClient(TrakHoundHttpClient baseClient)
        {
            _baseClient = baseClient;
        }


        public async Task<IEnumerable<TrakHoundDriverInformation>> GetDrivers()
        {
            var url = Url.Combine(_baseClient.HttpBaseUrl, HttpConstants.DriversPrefix);

            return await RestRequest.Get<IEnumerable<TrakHoundDriverInformation>>(url);
        }

        public async Task<TrakHoundDriverInformation> GetDriver(string configurationId)
        {
            var url = Url.Combine(_baseClient.HttpBaseUrl, HttpConstants.DriversPrefix);
            url = Url.Combine(url, configurationId);

            return await RestRequest.Get<TrakHoundDriverInformation>(url);
        }
    }
}
