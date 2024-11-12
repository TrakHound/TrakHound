// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Requests;
using TrakHound.Services;

namespace TrakHound.Clients
{
    public interface ITrakHoundServicesClient
    {
        Task<IEnumerable<TrakHoundServiceInformation>> List(long skip = 0, long take = 100, SortOrder sortOrder = SortOrder.Ascending);

        Task<IEnumerable<TrakHoundServiceInformation>> ListByPackageId(string packageId, long skip = 0, long take = 100, SortOrder sortOrder = SortOrder.Ascending);

        Task<IEnumerable<TrakHoundServiceInformation>> ListByPackageId(IEnumerable<string> packageIds, long skip = 0, long take = 100, SortOrder sortOrder = SortOrder.Ascending);
        
        Task<ITrakHoundConsumer<IEnumerable<TrakHoundServiceInformation>>> Subscribe();


        Task<TrakHoundServiceInformation> GetByServiceId(string serviceId);

        Task<IEnumerable<TrakHoundServiceInformation>> GetByServiceId(IEnumerable<string> serviceIds);

        Task<ITrakHoundConsumer<TrakHoundServiceInformation>> SubscribeByServiceId(string serviceId);


        Task<IEnumerable<TrakHoundServiceLogInformation>> GetLogs(string serviceId);

        Task<ITrakHoundConsumer<IEnumerable<TrakHoundLog>>> SubscribeToLog(string logId, TrakHoundLogLevel minimumLevel = TrakHoundLogLevel.Information);


        Task<TrakHoundServiceStatus> StartService(string serviceId);

        Task<TrakHoundServiceStatus> StopService(string serviceId);
    }
}
