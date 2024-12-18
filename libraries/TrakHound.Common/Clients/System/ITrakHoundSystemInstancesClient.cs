// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Buffers;
using TrakHound.Instances;
using TrakHound.Logging;

namespace TrakHound.Clients
{
    public interface ITrakHoundSystemInstancesClient
    {
        Task<TrakHoundInstanceInformation> GetHostInformation();


        Task Start(string instanceId);

        Task Stop(string instanceId);


        Task<ITrakHoundConsumer<TrakHoundInstanceInformation>> Subscribe();

        Task<ITrakHoundConsumer<TrakHoundInstanceInformation>> Subscribe(string instanceId);


        Task<ITrakHoundConsumer<IEnumerable<TrakHoundLogItem>>> SubscribeToLog(string instanceId, TrakHoundLogLevel minimumLevel = TrakHoundLogLevel.Trace);

        Task<ITrakHoundConsumer<IEnumerable<TrakHoundLogItem>>> SubscribeToLog(string instanceId, string logName, TrakHoundLogLevel minimumLevel = TrakHoundLogLevel.Trace);


        Task<IEnumerable<string>> QueryLogNames(string instanceId);


        Task<IEnumerable<TrakHoundLogItem>> QueryLogs(string instanceId, DateTime from, DateTime to, int skip = 0, int take = 1000, SortOrder sortOrder = SortOrder.Ascending);

        Task<IEnumerable<TrakHoundLogItem>> QueryLogsByMinimumLevel(string instanceId, TrakHoundLogLevel minimumLevel, DateTime from, DateTime to, int skip = 0, int take = 1000, SortOrder sortOrder = SortOrder.Ascending);

        Task<IEnumerable<TrakHoundLogItem>> QueryLogsByLevel(string instanceId, TrakHoundLogLevel level, DateTime from, DateTime to, int skip = 0, int take = 1000, SortOrder sortOrder = SortOrder.Ascending);


        Task<IEnumerable<TrakHoundLogItem>> QueryLogs(string instanceId, string logName, DateTime from, DateTime to, int skip = 0, int take = 1000, SortOrder sortOrder = SortOrder.Ascending);

        Task<IEnumerable<TrakHoundLogItem>> QueryLogsByMinimumLevel(string instanceId, string logName, TrakHoundLogLevel minimumLevel, DateTime from, DateTime to, int skip = 0, int take = 1000, SortOrder sortOrder = SortOrder.Ascending);

        Task<IEnumerable<TrakHoundLogItem>> QueryLogsByLevel(string instanceId, string logName, TrakHoundLogLevel level, DateTime from, DateTime to, int skip = 0, int take = 1000, SortOrder sortOrder = SortOrder.Ascending);



        Task<IEnumerable<TrakHoundBufferMetrics>> GetBufferMetrics();

        Task<TrakHoundBufferMetrics> GetBufferMetrics(string bufferId);
    }
}
