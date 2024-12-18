// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Http;
using TrakHound.Logging;
using TrakHound.Services;

namespace TrakHound.Clients
{
    internal class TrakHoundHttpSystemServicesClient : ITrakHoundSystemServicesClient
    {
        private readonly TrakHoundHttpClient _baseClient;


        public string BaseUrl => _baseClient != null ? _baseClient.HttpBaseUrl : null;

        public string RouterId => _baseClient != null ? _baseClient.RouterId : null;


        public TrakHoundHttpSystemServicesClient(TrakHoundHttpClient baseClient)
        {
            _baseClient = baseClient;
        }


        public async Task<IEnumerable<TrakHoundServiceInformation>> GetInformation()
        {
            var url = Url.Combine(BaseUrl, "_services/information");
            return await RestRequest.Get<IEnumerable<TrakHoundServiceInformation>>(url);
        }

        public async Task<TrakHoundServiceInformation> GetInformation(string apiId)
        {
            var url = Url.Combine(BaseUrl, "_services/information");
            url = Url.Combine(url, apiId);

            return await RestRequest.Get<TrakHoundServiceInformation>(url);
        }

        public async Task<IEnumerable<TrakHoundServiceInformation>> GetInformation(IEnumerable<string> functionIds)
        {
            var url = Url.Combine(BaseUrl, "_services/information");
            return await RestRequest.Post<IEnumerable<TrakHoundServiceInformation>>(url, functionIds);
        }


        public async Task<IEnumerable<string>> Retrieve(int skip = 0, int take = 100, SortOrder sortOrder = SortOrder.Ascending)
        {
            var url = Url.Combine(BaseUrl, HttpConstants.FunctionsPrefix);
            url = Url.AddQueryParameter(url, "skip", skip);
            url = Url.AddQueryParameter(url, "take", take);
            url = Url.AddQueryParameter(url, "order", (int)sortOrder);

            return await RestRequest.Get<IEnumerable<string>>(url);
        }

        public async Task<IEnumerable<TrakHoundServiceStatus>> GetStatus(IEnumerable<string> serviceIds)
        {
            var url = Url.Combine(BaseUrl, HttpConstants.FunctionsPrefix);
            url = Url.Combine(url, "status");

            var httpStatuses = await RestRequest.Post<IEnumerable<TrakHoundHttpServiceStatus>>(url, serviceIds);
            return TrakHoundHttpServiceStatus.ToStatuses(httpStatuses);
        }

        public async Task<TrakHoundServiceStatus> StartService(string serviceId)
        {
            var url = Url.Combine(BaseUrl, HttpConstants.FunctionsPrefix);
            url = Url.Combine(url, serviceId);
            url = Url.Combine(url, "start");

            var httpStatus = await RestRequest.Put<TrakHoundHttpServiceStatus>(url);
            return httpStatus.ToStatus();
        }

        public async Task<TrakHoundServiceStatus> StopService(string serviceId)
        {
            var url = Url.Combine(BaseUrl, HttpConstants.FunctionsPrefix);
            url = Url.Combine(url, serviceId);
            url = Url.Combine(url, "stop");

            var httpStatus = await RestRequest.Put<TrakHoundHttpServiceStatus>(url);
            return httpStatus.ToStatus();
        }

        public async Task<ITrakHoundConsumer<TrakHoundServiceInformation>> Subscribe()
        {
            return null;
        }

        public async Task<ITrakHoundConsumer<TrakHoundServiceInformation>> Subscribe(string serviceId)
        {
            return null;
        }

        public async Task<ITrakHoundConsumer<IEnumerable<TrakHoundLogItem>>> SubscribeToLog(string serviceId, TrakHoundLogLevel minimumLevel = TrakHoundLogLevel.Information)
        {
            return null;
        }

        public async Task<ITrakHoundConsumer<IEnumerable<TrakHoundLogItem>>> SubscribeToLog(string serviceId, string logName, TrakHoundLogLevel minimumLevel = TrakHoundLogLevel.Information)
        {
            return null;
        }


        public async Task<IEnumerable<string>> QueryLogNames(string serviceId)
        {
            return null;
        }

        public async Task<IEnumerable<TrakHoundLogItem>> QueryLogs(string serviceId, DateTime from, DateTime to, int skip = 0, int take = 1000, SortOrder sortOrder = SortOrder.Ascending)
        {
            return null;
        }

        public async Task<IEnumerable<TrakHoundLogItem>> QueryLogsByMinimumLevel(string serviceId, TrakHoundLogLevel minimumLevel, DateTime from, DateTime to, int skip = 0, int take = 1000, SortOrder sortOrder = SortOrder.Ascending)
        {
            return null;
        }

        public async Task<IEnumerable<TrakHoundLogItem>> QueryLogsByLevel(string serviceId, TrakHoundLogLevel level, DateTime from, DateTime to, int skip = 0, int take = 1000, SortOrder sortOrder = SortOrder.Ascending)
        {
            return null;
        }

        public async Task<IEnumerable<TrakHoundLogItem>> QueryLogs(string serviceId, string logName, DateTime from, DateTime to, int skip = 0, int take = 1000, SortOrder sortOrder = SortOrder.Ascending)
        {
            return null;
        }

        public async Task<IEnumerable<TrakHoundLogItem>> QueryLogsByMinimumLevel(string serviceId, string logName, TrakHoundLogLevel minimumLevel, DateTime from, DateTime to, int skip = 0, int take = 1000, SortOrder sortOrder = SortOrder.Ascending)
        {
            return null;
        }

        public async Task<IEnumerable<TrakHoundLogItem>> QueryLogsByLevel(string serviceId, string logName, TrakHoundLogLevel level, DateTime from, DateTime to, int skip = 0, int take = 1000, SortOrder sortOrder = SortOrder.Ascending)
        {
            return null;
        }
    }
}
