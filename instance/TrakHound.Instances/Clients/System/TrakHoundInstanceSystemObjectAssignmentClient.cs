// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Entities;

namespace TrakHound.Clients
{
    internal partial class TrakHoundInstanceSystemObjectAssignmentClient
    {
        public async Task<IEnumerable<ITrakHoundObjectAssignmentEntity>> QueryByAssigneeRange(
           IEnumerable<TrakHoundRangeQuery> queries,
           long skip = 0,
           long take = 1000,
           SortOrder sortOrder = TrakHound.SortOrder.Ascending,
           string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router.Entities.Objects.Assignments.QueryByAssigneeRange(queries, skip, take, sortOrder);
                if (response.IsSuccess)
                {
                    return response.Content.ToDistinct();
                }
            }

            return default;
        }

        public async Task<IEnumerable<ITrakHoundObjectAssignmentEntity>> QueryByMemberRange(
           IEnumerable<TrakHoundRangeQuery> queries,
           long skip = 0,
           long take = 1000,
           SortOrder sortOrder = TrakHound.SortOrder.Ascending,
           string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router.Entities.Objects.Assignments.QueryByMemberRange(queries, skip, take, sortOrder);
                if (response.IsSuccess)
                {
                    return response.Content.ToDistinct();
                }
            }

            return default;
        }
    }
}
