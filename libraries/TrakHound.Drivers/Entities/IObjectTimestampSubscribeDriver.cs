// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Entities;

namespace TrakHound.Drivers.Entities
{
    /// <summary>
    /// Entity Driver used to Subscribe to TrakHound Object Timestamp Entities
    /// </summary>
    public interface IObjectTimestampSubscribeDriver : IEntitySubscribeDriver<ITrakHoundObjectTimestampEntity>
    {
        /// <summary>
        /// Subsribe to the Consumer Driver that will consume TrakHound Timestamp Entities by Object UUID's
        /// </summary>
        Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectTimestampEntity>>>> Subscribe(IEnumerable<string> objectUuids);
    }
}
