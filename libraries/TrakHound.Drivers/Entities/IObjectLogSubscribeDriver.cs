// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Entities;

namespace TrakHound.Drivers.Entities
{
    /// <summary>
    /// A TrakHound Driver used to create Consumers for TrakHound Logs
    /// </summary>
    public interface IObjectLogSubscribeDriver : ITrakHoundDriver
    {
        /// <summary>
        /// Subscribe to the Consumer Driver that will consume TrakHound ObjectLogs by LogLevel
        /// </summary>
        Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectLogEntity>>>> Subscribe(TrakHoundLogLevel minimumLevel);

        /// <summary>
        /// Subscribe to the Consumer Driver that will consume TrakHound ObjectLogs by Object UUID
        /// </summary>
        Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectLogEntity>>>> Subscribe(IEnumerable<string> objectUuids, TrakHoundLogLevel minimumLevel);
    }
}
