// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Entities;

namespace TrakHound.Drivers.Entities
{
    /// <summary>
    /// Entity Driver used to Subscribe to TrakHound Object Assignment Entities
    /// </summary>
    public interface IObjectAssignmentSubscribeDriver : IEntitySubscribeDriver<ITrakHoundObjectAssignmentEntity>
    {
        /// <summary>
        /// Subsribe to the Consumer Driver that will consume TrakHound Assignment Entities by Assignee ID's
        /// </summary>
        Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectAssignmentEntity>>>> SubscribeByAssignee(IEnumerable<string> assigneeUuids);


        /// <summary>
        /// Subsribe to the Consumer Driver that will consume TrakHound Assignment Entities by Member ID's
        /// </summary>
        Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectAssignmentEntity>>>> SubscribeByMember(IEnumerable<string> memberUuids);
    }
}
