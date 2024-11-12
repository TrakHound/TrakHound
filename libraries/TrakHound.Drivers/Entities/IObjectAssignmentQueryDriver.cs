// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Entities;
using TrakHound.Entities;

namespace TrakHound.Drivers.Entities
{
    /// <summary>
    /// Api Driver used to Query TrakHound Assignment Entities.
    /// </summary>
    public interface IObjectAssignmentQueryDriver : ITrakHoundDriver
    {
        Task<TrakHoundResponse<ITrakHoundObjectAssignmentEntity>> QueryByAssignee(IEnumerable<TrakHoundRangeQuery> queries, long skip = 0, long take = 100, SortOrder order = SortOrder.Ascending);

        Task<TrakHoundResponse<ITrakHoundObjectAssignmentEntity>> QueryByMember(IEnumerable<TrakHoundRangeQuery> queries, long skip = 0, long take = 100, SortOrder sortOrder = SortOrder.Ascending);

        Task<TrakHoundResponse<TrakHoundCount>> CountByAssignee(IEnumerable<TrakHoundRangeQuery> queries);

        Task<TrakHoundResponse<TrakHoundCount>> CountByMember(IEnumerable<TrakHoundRangeQuery> queries);
    }
}
