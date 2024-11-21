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
    public interface IObjectObservationSubscribeDriver : IEntitySubscribeDriver<ITrakHoundObjectObservationEntity>
    {
        /// <summary>
        /// Subsribe to the Consumer Driver that will consume TrakHound Observation Entities by Object UUID's
        /// </summary>
        Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectObservationEntity>>>> Subscribe(IEnumerable<string> objectUuids);
    }
}
