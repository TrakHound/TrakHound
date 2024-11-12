// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Entities;

namespace TrakHound.Drivers.Entities
{
    /// <summary>
    /// Entity Driver used to read TrakHound Object Group Entities
    /// </summary>
    public interface IObjectGroupQueryDriver : ITrakHoundDriver
    {
        /// <summary>
        /// Read Group Entities with the specified Group UUID's
        /// </summary>
        Task<TrakHoundResponse<ITrakHoundObjectGroupEntity>> QueryByGroup(IEnumerable<string> groupUuids);

        /// <summary>
        /// Read Group Entities with the specified Member UUID's
        /// </summary>
        Task<TrakHoundResponse<ITrakHoundObjectGroupEntity>> QueryByMember(IEnumerable<string> memberUuids);
    }
}
