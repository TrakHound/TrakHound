// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Entities;

namespace TrakHound.Drivers.Entities
{
    /// <summary>
    /// Driver used to read TrakHound Timestamp Entities.
    /// </summary>
    public interface IObjectTimestampQueryDriver : ITrakHoundDriver
    {
        /// <summary>
        /// Query the Timestamp Entities with the specified Object UUID's
        /// </summary>
        Task<TrakHoundResponse<ITrakHoundObjectTimestampEntity>> Query(IEnumerable<string> objectUuids);

        Task<TrakHoundResponse<ITrakHoundObjectTimestampEntity>> Query(IEnumerable<string> objectUuids, long start, long end);
    }
}
