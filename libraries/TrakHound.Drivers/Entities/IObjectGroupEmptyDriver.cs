// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Entities;

namespace TrakHound.Drivers.Entities
{
    /// <summary>
    /// Entity Driver specifically for Setting TrakHound Object Group Entities to Empty.
    /// </summary>
    public interface IObjectGroupEmptyDriver : IEntityEmptyDriver
    {
        Task<TrakHoundResponse<bool>> EmptyByGroup(IEnumerable<EntityEmptyRequest> requests);

        Task<TrakHoundResponse<bool>> EmptyByMember(IEnumerable<EntityEmptyRequest> requests);
    }
}
