// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Entities;
using TrakHound.Http;

namespace TrakHound.Clients
{
    internal partial class TrakHoundHttpSystemObjectAssignmentClient
    {
        public async Task<IEnumerable<ITrakHoundObjectAssignmentEntity>> QueryByAssigneeRange(
            IEnumerable<TrakHoundRangeQuery> queries,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null)
        {
            var route = "assignee/range";

            route = Url.AddQueryParameter(route, "skip", (long)skip);
            route = Url.AddQueryParameter(route, "take", (long)take);
            route = Url.AddQueryParameter(route, "sortOrder", (int)sortOrder);
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectAssignmentEntity>());
            url = Url.Combine(url, route);

            var httpQueries = TrakHoundHttpRangeQueryRequest.Create(queries);

            var result = await RestRequest.Post<object[][]>(url, httpQueries);
            return ProcessResponse(result);
        }

        public async Task<IEnumerable<ITrakHoundObjectAssignmentEntity>> QueryByMemberRange(
            IEnumerable<TrakHoundRangeQuery> queries,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null)
        {
            var route = "member/range";

            route = Url.AddQueryParameter(route, "skip", (long)skip);
            route = Url.AddQueryParameter(route, "take", (long)take);
            route = Url.AddQueryParameter(route, "sortOrder", (int)sortOrder);
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectAssignmentEntity>());
            url = Url.Combine(url, route);

            var httpQueries = TrakHoundHttpRangeQueryRequest.Create(queries);

            var result = await RestRequest.Post<object[][]>(url, httpQueries);
            return ProcessResponse(result);
        }
    }
}
