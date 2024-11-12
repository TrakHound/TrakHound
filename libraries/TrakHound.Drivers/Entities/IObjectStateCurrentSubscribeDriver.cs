// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Entities;

namespace TrakHound.Drivers.Entities
{
    /// <summary>
    /// Entity Driver used to Subscribe to TrakHound Object State Entities
    /// </summary>
    public interface IObjectStateCurrentSubscribeDriver : ITrakHoundDriver
    {
        /// <summary>
        /// Subsribe to the Consumer Driver that will consume all of the current TrakHound State Entities
        /// </summary>
        Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectStateEntity>>>> SubscribeCurrent();

        /// <summary>
        /// Subsribe to the Consumer Driver that will consume the current TrakHound State Entities by Object UUID's
        /// </summary>
        Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectStateEntity>>>> SubscribeCurrent(IEnumerable<string> objectUuids);
    }
}
