// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Entities;

namespace TrakHound.Drivers.Entities
{
    /// <summary>
    /// Entity Driver specifically for Setting TrakHound Object Assignment Entities to Empty.
    /// </summary>
    public interface IObjectAssignmentEmptyDriver : IEntityEmptyDriver
    {
        Task<TrakHoundResponse<bool>> EmptyAssignee(IEnumerable<EntityEmptyRequest> requests);


        Task<TrakHoundResponse<bool>> EmptyMember(IEnumerable<EntityEmptyRequest> requests);
    }
}
