// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Entities;

namespace TrakHound.Drivers.Entities
{
    /// <summary>
    /// Driver used to Query TrakHound Observation Entities.
    /// </summary>
    public interface IObjectObservationQueryDriver : ITrakHoundDriver
    {
        /// <summary>
        /// Query Observation Entities with the specified Queries
        /// </summary>
        Task<TrakHoundResponse<ITrakHoundObjectObservationEntity>> Query(IEnumerable<TrakHoundRangeQuery> queries, long skip = 0, long take = 0, SortOrder order = SortOrder.Ascending);

        /// <summary>
        /// Query Observation Entities with the specified Queries and Conditions
        /// </summary>
        Task<TrakHoundResponse<ITrakHoundObjectObservationEntity>> Query(IEnumerable<TrakHoundRangeQuery> queries, IEnumerable<TrakHoundConditionGroupQuery> conditionGroups, long skip = 0, long take = 0, SortOrder order = SortOrder.Ascending);


        /// <summary>
        /// Query Last Observation Entities with the specified Queries
        /// </summary>
        Task<TrakHoundResponse<ITrakHoundObjectObservationEntity>> Last(IEnumerable<TrakHoundTimeQuery> queries);

        /// <summary>
        /// Query Last Observation Entities with the specified Queries and Conditions
        /// </summary>
        Task<TrakHoundResponse<ITrakHoundObjectObservationEntity>> Last(IEnumerable<TrakHoundTimeQuery> queries, IEnumerable<TrakHoundConditionGroupQuery> conditionGroups);


        /// <summary>
        /// Get the total count of Observation Entities with the specified Queries
        /// </summary>
        Task<TrakHoundResponse<TrakHoundCount>> Count(IEnumerable<TrakHoundRangeQuery> queries);
    }
}
