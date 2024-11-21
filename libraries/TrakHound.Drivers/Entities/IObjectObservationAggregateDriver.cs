// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;

namespace TrakHound.Drivers.Entities
{
    /// <summary>
    /// Driver used to Aggregate TrakHound Observation Entities.
    /// </summary>
    public interface IObjectObservationAggregateDriver : ITrakHoundDriver
    {
        Task<TrakHoundResponse<TrakHoundAggregate>> Aggregate(IEnumerable<TrakHoundRangeQuery> queries, TrakHoundAggregateType aggregateType);

        Task<TrakHoundResponse<TrakHoundAggregateWindow>> AggregateWindow(IEnumerable<TrakHoundRangeQuery> queries, TrakHoundAggregateType aggregateType, long window);
    }
}
