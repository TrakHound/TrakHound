// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Entities;

namespace TrakHound.Drivers.Entities
{
    /// <summary>
    /// Entity Driver used to read the Current TrakHound Object Assignment Entities.
    /// </summary>
    public interface IObjectAssignmentCurrentDriver : ITrakHoundDriver
    {
        /// <summary>
        /// Read Current Entities with the specified Object ID's
        /// </summary>
        Task<TrakHoundResponse<ITrakHoundObjectAssignmentEntity>> CurrentByAssignee(IEnumerable<string> assigneeUuids);

        /// <summary>
        /// Read Current Entity Members of the specified Object ID's
        /// </summary>
        Task<TrakHoundResponse<ITrakHoundObjectAssignmentEntity>> CurrentByMember(IEnumerable<string> memberUuids);
    }
}
