// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Entities;

namespace TrakHound.Drivers.Entities
{
    /// <summary>
    /// Entity Driver used to Subscribe to TrakHound Object Group Entities
    /// </summary>
    public interface IObjectGroupSubscribeDriver : IEntitySubscribeDriver<ITrakHoundObjectGroupEntity>
    {
        /// <summary>
        /// Subsribe to the Consumer Driver that will consume TrakHound Grouping Entities by Object UUID's
        /// </summary>
        Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectGroupEntity>>>> SubscribeByGroup(IEnumerable<string> groupUuids);

        /// <summary>
        /// Subsribe to the Consumer Driver that will consume TrakHound Grouping Entities by Object UUID's
        /// </summary>
        Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectGroupEntity>>>> SubscribeByMember(IEnumerable<string> memberUuids);
    }
}
