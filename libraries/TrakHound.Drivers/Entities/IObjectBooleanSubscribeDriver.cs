// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Entities;

namespace TrakHound.Drivers.Entities
{
    /// <summary>
    /// Entity Driver used to Subscribe to TrakHound Object Boolean Entities
    /// </summary>
    public interface IObjectBooleanSubscribeDriver : IEntitySubscribeDriver<ITrakHoundObjectBooleanEntity>
    {
        /// <summary>
        /// Subsribe to the Consumer Driver that will consume TrakHound Boolean Entities by Object UUID's
        /// </summary>
        Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectBooleanEntity>>>> Subscribe(IEnumerable<string> objectUuids);
    }
}
