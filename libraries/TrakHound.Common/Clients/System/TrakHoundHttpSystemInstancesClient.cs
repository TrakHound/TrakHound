// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
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

        public async Task<ITrakHoundConsumer<IEnumerable<TrakHoundLogItem>>> SubscribeToLog(string instanceId, string logName, TrakHoundLogLevel minimumLevel = TrakHoundLogLevel.Information)
        {
            return null; // Not Implemented
        }


        public async Task<IEnumerable<string>> QueryLogNames(string instanceId)
        {
            return null;
        }

        public async Task<IEnumerable<TrakHoundLogItem>> QueryLogs(string instanceId, DateTime from, DateTime to, int skip = 0, int take = 1000, SortOrder sortOrder = SortOrder.Ascending)
        {
            return null;
        }

        public async Task<IEnumerable<TrakHoundLogItem>> QueryLogsByMinimumLevel(string instanceId, TrakHoundLogLevel minimumLevel, DateTime from, DateTime to, int skip = 0, int take = 1000, SortOrder sortOrder = SortOrder.Ascending)
        {
            return null;
        }

        public async Task<IEnumerable<TrakHoundLogItem>> QueryLogsByLevel(string instanceId, TrakHoundLogLevel level, DateTime from, DateTime to, int skip = 0, int take = 1000, SortOrder sortOrder = SortOrder.Ascending)
        {
            return null;
        }

        public async Task<IEnumerable<TrakHoundLogItem>> QueryLogs(string instanceId, string logName, DateTime from, DateTime to, int skip = 0, int take = 1000, SortOrder sortOrder = SortOrder.Ascending)
        {
            return null;
        }

        public async Task<IEnumerable<TrakHoundLogItem>> QueryLogsByMinimumLevel(string instanceId, string logName, TrakHoundLogLevel minimumLevel, DateTime from, DateTime to, int skip = 0, int take = 1000, SortOrder sortOrder = SortOrder.Ascending)
        {
            return null;
        }

        public async Task<IEnumerable<TrakHoundLogItem>> QueryLogsByLevel(string instanceId, string logName, TrakHoundLogLevel level, DateTime from, DateTime to, int skip = 0, int take = 1000, SortOrder sortOrder = SortOrder.Ascending)
        {
            return null;
        }


        public Task<IEnumerable<TrakHoundBufferMetrics>> GetBufferMetrics() => null; // Not Implemented

        public Task<TrakHoundBufferMetrics> GetBufferMetrics(string bufferId) => null; // Not Implemented
    }
}
