// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Entities;
using TrakHound.Http;

namespace TrakHound.Clients
{
    internal partial class TrakHoundHttpSystemObjectLogClient
    {
        public async Task<IEnumerable<ITrakHoundObjectLogEntity>> QueryByObjectUuid(
            IEnumerable<string> objectUuids,
            TrakHoundLogLevel minLevel,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null)
        {
            var route = "object";

            route = Url.AddQueryParameter(route, "minLevel", (TrakHound.TrakHoundLogLevel)minLevel);
            route = Url.AddQueryParameter(route, "skip", (long)skip);
            route = Url.AddQueryParameter(route, "take", (long)take);
            route = Url.AddQueryParameter(route, "sortOrder", (int)sortOrder);
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectLogEntity>());
            url = Url.Combine(url, route);

            var result = await RestRequest.Post<object[][]>(url, objectUuids);
            return ProcessResponse(result);
        }

        public async Task<IEnumerable<ITrakHoundObjectLogEntity>> QueryByRange(
            IEnumerable<TrakHoundRangeQuery> queries,
            TrakHoundLogLevel minLevel,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null)
        {
            var route = "range";

            route = Url.AddQueryParameter(route, "minLevel", (TrakHound.TrakHoundLogLevel)minLevel);
            route = Url.AddQueryParameter(route, "skip", (long)skip);
            route = Url.AddQueryParameter(route, "take", (long)take);
            route = Url.AddQueryParameter(route, "sortOrder", (int)sortOrder);
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectLogEntity>());
            url = Url.Combine(url, route);

            var httpQueries = TrakHoundHttpRangeQueryRequest.Create(queries);

            var result = await RestRequest.Post<object[][]>(url, httpQueries);
            return ProcessResponse(result);
        }
    }
}
