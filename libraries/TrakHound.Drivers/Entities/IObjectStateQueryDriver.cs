// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Entities;

namespace TrakHound.Drivers.Entities
{
    /// <summary>
    /// Entity Driver used to Query TrakHound Object State Entities.
    /// </summary>
    public interface IObjectStateQueryDriver : ITrakHoundDriver
    {
        /// <summary>
        /// Query State Entities with the specified Object UUID's
        /// </summary>
        Task<TrakHoundResponse<ITrakHoundObjectStateEntity>> Query(IEnumerable<TrakHoundRangeQuery> queries, long skip = 0, long take = 0, SortOrder order = SortOrder.Ascending);

        /// <summary>
        /// Query State Entities with the specified Object UUID's
        /// </summary>
        Task<TrakHoundResponse<ITrakHoundObjectStateEntity>> Query(IEnumerable<TrakHoundRangeQuery> queries, string conditionQuery, long skip = 0, long take = 0, SortOrder order = SortOrder.Ascending);

        /// <summary>
        /// Query Last State Entities with the specified Queries
        /// </summary>
        Task<TrakHoundResponse<ITrakHoundObjectStateEntity>> Last(IEnumerable<TrakHoundTimeQuery> queries);

        /// <summary>
        /// Get the total count of State Entities with the specified Object UUID's
        /// </summary>
        Task<TrakHoundResponse<TrakHoundCount>> Count(IEnumerable<TrakHoundRangeQuery> queries);
    }
}
