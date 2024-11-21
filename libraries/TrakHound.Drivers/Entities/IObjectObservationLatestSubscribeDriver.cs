// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Entities;

namespace TrakHound.Drivers.Entities
{
    /// <summary>
    /// Driver used to Subscribe to TrakHound Object Observation Entities
    /// </summary>
    public interface IObjectObservationLatestSubscribeDriver : ITrakHoundDriver
    {
        /// <summary>
        /// Subsribe to the Consumer Driver that will consume all of the latest TrakHound Observation Entities
        /// </summary>
        Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectObservationEntity>>>> SubscribeLatest();

        /// <summary>
        /// Subsribe to the Consumer Driver that will consume the latest TrakHound Observation Entities by Object UUID's
        /// </summary>
        Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectObservationEntity>>>> SubscribeLatest(IEnumerable<string> objectUuids);
    }
}
