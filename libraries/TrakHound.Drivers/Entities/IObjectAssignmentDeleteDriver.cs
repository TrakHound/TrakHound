// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;

namespace TrakHound.Drivers.Entities
{
    public interface IObjectAssignmentDeleteDriver : ITrakHoundDriver
    {
        Task<TrakHoundResponse<bool>> DeleteByAssigneeUuid(IEnumerable<string> assigneeUuids);

        Task<TrakHoundResponse<bool>> DeleteByMemberUuid(IEnumerable<string> memberUuids);
    }
}
