// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Entities;

namespace TrakHound.Drivers.Entities
{
    /// <summary>
    /// Driver used to Query TrakHound Sources
    /// </summary>
    public interface ISourceQueryDriver : ITrakHoundDriver
    {
        Task<TrakHoundResponse<ITrakHoundSourceQueryResult>> QueryUuidChain(IEnumerable<string> uuids);
    }
}
