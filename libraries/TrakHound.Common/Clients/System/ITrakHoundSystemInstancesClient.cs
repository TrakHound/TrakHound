// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

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


        Task<IEnumerable<TrakHoundBufferMetrics>> GetBufferMetrics();

        Task<TrakHoundBufferMetrics> GetBufferMetrics(string bufferId);
    }
}
