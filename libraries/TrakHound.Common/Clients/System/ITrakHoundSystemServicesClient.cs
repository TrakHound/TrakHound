// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
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

        Task<ITrakHoundConsumer<IEnumerable<TrakHoundLogItem>>> SubscribeToLog(string serviceId, string logName, TrakHoundLogLevel minimumLevel = TrakHoundLogLevel.Trace);


        Task<IEnumerable<string>> QueryLogNames(string serviceId);


        Task<IEnumerable<TrakHoundLogItem>> QueryLogs(string serviceId, DateTime from, DateTime to, int skip = 0, int take = 1000, SortOrder sortOrder = SortOrder.Ascending);

        Task<IEnumerable<TrakHoundLogItem>> QueryLogsByMinimumLevel(string serviceId, TrakHoundLogLevel minimumLevel, DateTime from, DateTime to, int skip = 0, int take = 1000, SortOrder sortOrder = SortOrder.Ascending);

        Task<IEnumerable<TrakHoundLogItem>> QueryLogsByLevel(string serviceId, TrakHoundLogLevel level, DateTime from, DateTime to, int skip = 0, int take = 1000, SortOrder sortOrder = SortOrder.Ascending);


        Task<IEnumerable<TrakHoundLogItem>> QueryLogs(string serviceId, string logName, DateTime from, DateTime to, int skip = 0, int take = 1000, SortOrder sortOrder = SortOrder.Ascending);

        Task<IEnumerable<TrakHoundLogItem>> QueryLogsByMinimumLevel(string serviceId, string logName, TrakHoundLogLevel minimumLevel, DateTime from, DateTime to, int skip = 0, int take = 1000, SortOrder sortOrder = SortOrder.Ascending);

        Task<IEnumerable<TrakHoundLogItem>> QueryLogsByLevel(string serviceId, string logName, TrakHoundLogLevel level, DateTime from, DateTime to, int skip = 0, int take = 1000, SortOrder sortOrder = SortOrder.Ascending);
    }
}
