// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Entities;

namespace TrakHound.Clients.Collections
{
    internal partial class TrakHoundCollectionObjectAssignmentClient
    {
        public async Task<IEnumerable<ITrakHoundObjectAssignmentEntity>> QueryByAssigneeRange(
            IEnumerable<TrakHoundRangeQuery> queries,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null)
        {
            return default;
        }

        public async Task<IEnumerable<ITrakHoundObjectAssignmentEntity>> QueryByMemberRange(
            IEnumerable<TrakHoundRangeQuery> queries,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null)
        {
            return default;
        }
    }
}
