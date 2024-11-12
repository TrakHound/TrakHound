// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Entities;

namespace TrakHound.Clients
{
    public partial interface ITrakHoundSystemSourceClient
    {
        Task<IEnumerable<ITrakHoundSourceEntity>> QueryChain(string uuid, string routerId = null);

        Task<IEnumerable<ITrakHoundSourceEntity>> QueryChain(IEnumerable<string> uuids, string routerId = null);


        Task<IEnumerable<ITrakHoundSourceQueryResult>> QueryUuidChain(string uuid, string routerId = null);

        Task<IEnumerable<ITrakHoundSourceQueryResult>> QueryUuidChain(IEnumerable<string> uuids, string routerId = null);
    }
}
