// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;

namespace TrakHound.Drivers.Entities
{
    /// <summary>
    /// Driver used to Delete TrakHound Objects
    /// </summary>
    public interface IObjectDeleteDriver : ITrakHoundDriver
    {
        Task<TrakHoundResponse<bool>> DeleteByRootUuid(IEnumerable<string> rootUuids);
    }
}
