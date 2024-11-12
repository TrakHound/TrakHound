// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Entities;

namespace TrakHound.Drivers.Entities
{
    /// <summary>
    /// Driver used to read TrakHound Duration Entities.
    /// </summary>
    public interface IObjectDurationQueryDriver : ITrakHoundDriver
    {
        /// <summary>
        /// Query the Duration Entities with the specified Object UUID's
        /// </summary>
        Task<TrakHoundResponse<ITrakHoundObjectDurationEntity>> Query(IEnumerable<string> objectUuids);

        Task<TrakHoundResponse<ITrakHoundObjectDurationEntity>> Query(IEnumerable<string> objectUuids, long min, long max);
    }
}
