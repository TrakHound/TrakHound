// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;

namespace TrakHound.Drivers.Entities
{
    public interface IObjectGroupDeleteDriver : ITrakHoundDriver
    {
        Task<TrakHoundResponse<bool>> DeleteByGroupUuid(IEnumerable<string> groupUuids);

        Task<TrakHoundResponse<bool>> DeleteByMemberUuid(IEnumerable<string> memberUuids);
    }
}
