// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Apps;
using TrakHound.Http;
using TrakHound.Logging;

namespace TrakHound.Clients
{
    internal class TrakHoundHttpAppsClient : ITrakHoundAppsClient
    {
        private readonly TrakHoundHttpClient _baseClient;


        public string BaseUrl => _baseClient != null ? _baseClient.HttpBaseUrl : null;

        public string RouterId => _baseClient != null ? _baseClient.RouterId : null;


        public TrakHoundHttpAppsClient(TrakHoundHttpClient baseClient)
        {
            _baseClient = baseClient;
        }


        public async Task<IEnumerable<TrakHoundAppInformation>> GetInformation()
        {
            var url = Url.Combine(BaseUrl, "_apps/information");
            return await RestRequest.Get<IEnumerable<TrakHoundAppInformation>>(url);
        }

        public async Task<TrakHoundAppInformation> GetInformation(string appId)
        {
            var url = Url.Combine(BaseUrl, "_apps/information");
            url = Url.Combine(url, appId);

            return await RestRequest.Get<TrakHoundAppInformation>(url);
        }


        public async Task<ITrakHoundConsumer<IEnumerable<TrakHoundLogItem>>> SubscribeToLog(string appId, TrakHoundLogLevel minimumLevel = TrakHoundLogLevel.Information)
        {
            return null;
        }
    }
}
