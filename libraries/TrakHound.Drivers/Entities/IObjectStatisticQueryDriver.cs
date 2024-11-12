// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Entities;

namespace TrakHound.Drivers.Entities
{
    public interface IObjectStatisticQueryDriver : ITrakHoundDriver
    {
        Task<TrakHoundResponse<ITrakHoundObjectStatisticEntity>> Query(IEnumerable<TrakHoundTimeRangeQuery> queries, long skip = 0, long take = 0, SortOrder sortOrder = SortOrder.Ascending);

        Task<TrakHoundResponse<TrakHoundTimeRangeSpan>> Spans(IEnumerable<TrakHoundRangeQuery> queries);

        Task<TrakHoundResponse<TrakHoundCount>> Count(IEnumerable<TrakHoundTimeRangeQuery> queries);
    }
}
