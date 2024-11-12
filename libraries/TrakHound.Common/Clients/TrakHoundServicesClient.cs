// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Requests;
using TrakHound.Services;

namespace TrakHound.Clients
{
    public class TrakHoundServicesClient : ITrakHoundServicesClient
    {
        private const string _apiRoute = "services";

        private readonly ITrakHoundClient _client;


        public TrakHoundServicesClient(ITrakHoundClient client)
        {
            _client = client;
        }


        public async Task<IEnumerable<TrakHoundServiceInformation>> List(long skip = 0, long take = 100, SortOrder sortOrder = SortOrder.Ascending)
        {
            var route = Url.Combine(_apiRoute, "list");
            return await _client.Api.QueryJson<IEnumerable<TrakHoundServiceInformation>>(route);
        }

        public async Task<IEnumerable<TrakHoundServiceInformation>> ListByPackageId(string packageId, long skip = 0, long take = 100, SortOrder sortOrder = SortOrder.Ascending)
        {
            var route = Url.Combine(_apiRoute, "list", "package", packageId);
            return await _client.Api.QueryJson<IEnumerable<TrakHoundServiceInformation>>(route);
        }

        public async Task<IEnumerable<TrakHoundServiceInformation>> ListByPackageId(IEnumerable<string> packageIds, long skip = 0, long take = 100, SortOrder sortOrder = SortOrder.Ascending)
        {
            var route = Url.Combine(_apiRoute, "list", "packages");
            return await _client.Api.QueryJson<IEnumerable<TrakHoundServiceInformation>>(route, packageIds);
        }


        public async Task<TrakHoundServiceInformation> GetByServiceId(string serviceId)
        {
            var route = Url.Combine(_apiRoute, serviceId);
            return await _client.Api.QueryJson<TrakHoundServiceInformation>(TrakHoundPath.Combine(_apiRoute, serviceId));
        }

        public async Task<IEnumerable<TrakHoundServiceInformation>> GetByServiceId(IEnumerable<string> serviceIds)
        {
            return await _client.Api.QueryJson<IEnumerable<TrakHoundServiceInformation>>(_apiRoute, serviceIds);
        }

        public async Task<ITrakHoundConsumer<IEnumerable<TrakHoundServiceInformation>>> Subscribe()
        {
            return await _client.Api.SubscribeJson<IEnumerable<TrakHoundServiceInformation>>(_apiRoute);
        }

        public async Task<ITrakHoundConsumer<TrakHoundServiceInformation>> SubscribeByServiceId(string serviceId)
        {
            var route = Url.Combine(_apiRoute, serviceId);
            return await _client.Api.SubscribeJson<TrakHoundServiceInformation>(route);
        }


        public async Task<IEnumerable<TrakHoundServiceLogInformation>> GetLogs(string serviceId)
        {
            var route = TrakHoundPath.Combine(_apiRoute, serviceId, "logs");
            return await _client.Api.QueryJson<IEnumerable<TrakHoundServiceLogInformation>>(route);
        }

        public async Task<ITrakHoundConsumer<IEnumerable<TrakHoundLog>>> SubscribeToLog(string logId, TrakHoundLogLevel minimumLevel = TrakHoundLogLevel.Information)
        {
            var path = $"uuid:{logId}";
            return await _client.Entities.SubscribeLogs(path, minimumLevel);
        }


        public async Task<TrakHoundServiceStatus> StartService(string serviceId)
        {
            var route = TrakHoundPath.Combine(_apiRoute, serviceId, "start");

            var response = await _client.Api.Publish(route);

            var result = new TrakHoundServiceStatus();
            result.ServiceId = serviceId;
            result.Status = response.Success ? TrakHoundServiceStatusType.Started : TrakHoundServiceStatusType.Error;
            result.Message = !response.Success ? response.GetContentUtf8String() : null;
            return result;
        }

        public async Task<TrakHoundServiceStatus> StopService(string serviceId)
        {
            var route = TrakHoundPath.Combine(_apiRoute, serviceId, "stop");

            var response = await _client.Api.Publish(route);

            var result = new TrakHoundServiceStatus();
            result.ServiceId = serviceId;
            result.Status = response.Success ? TrakHoundServiceStatusType.Stopped : TrakHoundServiceStatusType.Error;
            result.Message = !response.Success ? response.GetContentUtf8String() : null;
            return result;
        }
    }
}
