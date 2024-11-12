// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Logging;
using TrakHound.Services;

namespace TrakHound.Clients
{
    public interface ITrakHoundSystemServicesClient
    {
        Task<IEnumerable<TrakHoundServiceInformation>> GetInformation();

        Task<TrakHoundServiceInformation> GetInformation(string serviceId);

        Task<IEnumerable<TrakHoundServiceInformation>> GetInformation(IEnumerable<string> serviceIds);


        Task<IEnumerable<string>> Retrieve(int skip = 0, int take = 1000, SortOrder sortOrder = SortOrder.Ascending);

        Task<IEnumerable<TrakHoundServiceStatus>> GetStatus(IEnumerable<string> serviceIds);

        Task<TrakHoundServiceStatus> StartService(string serviceId);

        Task<TrakHoundServiceStatus> StopService(string serviceId);

        Task<ITrakHoundConsumer<TrakHoundServiceInformation>> Subscribe();

        Task<ITrakHoundConsumer<TrakHoundServiceInformation>> Subscribe(string serviceId);

        Task<ITrakHoundConsumer<IEnumerable<TrakHoundLogItem>>> SubscribeToLog(string serviceId, TrakHoundLogLevel minimumLevel = TrakHoundLogLevel.Trace);
    }
}
