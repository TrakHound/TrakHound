// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Apps;
using TrakHound.Logging;

namespace TrakHound.Clients
{
    public interface ITrakHoundAppsClient
    {
        Task<IEnumerable<TrakHoundAppInformation>> GetInformation();

        Task<TrakHoundAppInformation> GetInformation(string appId);


        Task<ITrakHoundConsumer<IEnumerable<TrakHoundLogItem>>> SubscribeToLog(string appId, TrakHoundLogLevel minimumLevel = TrakHoundLogLevel.Trace);
    }
}
