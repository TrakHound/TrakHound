// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Entities;
using TrakHound.Entities;

namespace TrakHound.Drivers.Entities
{
    /// <summary>
    /// Api Driver used to Query TrakHound Event Entities.
    /// </summary>
    public interface IObjectEventQueryDriver : ITrakHoundDriver
    {
        /// <summary>
        /// Query Event Entities with the specified Object ID's
        /// </summary>
        Task<TrakHoundResponse<ITrakHoundObjectEventEntity>> Query(IEnumerable<TrakHoundRangeQuery> queries, long skip = 0, long take = 0, SortOrder order = SortOrder.Ascending);

        /// <summary>
        /// Get the total count of Event Entities with the specified Object ID's
        /// </summary>
        Task<TrakHoundResponse<TrakHoundCount>> Count(IEnumerable<TrakHoundRangeQuery> queries);
    }
}
