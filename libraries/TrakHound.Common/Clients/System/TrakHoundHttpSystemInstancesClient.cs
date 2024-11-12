// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Buffers;
using TrakHound.Http;
using TrakHound.Instances;
using TrakHound.Logging;

namespace TrakHound.Clients
{
    internal class TrakHoundHttpSystemInstancesClient : ITrakHoundSystemInstancesClient
    {
        private readonly TrakHoundHttpClient _baseClient;


        public TrakHoundHttpSystemInstancesClient(TrakHoundHttpClient baseClient)
        {
            _baseClient = baseClient;
        }


        public async Task<TrakHoundInstanceInformation> GetHostInformation()
        {
            var url = Url.Combine(_baseClient.HttpBaseUrl, HttpConstants.InstancesPrefix);
            url = Url.Combine(url, "information", "host");

            return await RestRequest.Get<TrakHoundInstanceInformation>(url);
        }


        public Task Start(string instanceId) => Task.CompletedTask; // Not Implemented

        public Task Stop(string instanceId) => Task.CompletedTask; // Not Implemented


        public async Task<ITrakHoundConsumer<TrakHoundInstanceInformation>> Subscribe()
        {
            return null; // Not Implemented
        }

        public async Task<ITrakHoundConsumer<TrakHoundInstanceInformation>> Subscribe(string instanceId)
        {
            return null; // Not Implemented
        }

        public async Task<ITrakHoundConsumer<IEnumerable<TrakHoundLogItem>>> SubscribeToLog(string instanceId, TrakHoundLogLevel minimumLevel = TrakHoundLogLevel.Information)
        {
            return null; // Not Implemented
        }


        public Task<IEnumerable<TrakHoundBufferMetrics>> GetBufferMetrics() => null; // Not Implemented

        public Task<TrakHoundBufferMetrics> GetBufferMetrics(string bufferId) => null; // Not Implemented
    }
}
