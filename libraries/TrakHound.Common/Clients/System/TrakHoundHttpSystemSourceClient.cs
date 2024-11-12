// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Entities;
using TrakHound.Http;

namespace TrakHound.Clients
{
    internal partial class TrakHoundHttpSystemSourceClient
    {
        public async Task<IEnumerable<ITrakHoundSourceEntity>> QueryChain(string uuid, string routerId = null)
        {
            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundSourceEntity>());
            url = Url.Combine(url, uuid);
            url = Url.Combine(url, "chain");
            url = Url.AddQueryParameter(url, "routerId", BaseClient.GetRouterId(routerId));

            var array = await RestRequest.Get<object[][]>(url);
            return ProcessResponse(array);
        }

        public async Task<IEnumerable<ITrakHoundSourceEntity>> QueryChain(IEnumerable<string> uuids, string routerId = null)
        {
            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundSourceEntity>());
            url = Url.Combine(url, "chain");
            url = Url.AddQueryParameter(url, "routerId", BaseClient.GetRouterId(routerId));

            var array = await RestRequest.Post<object[][]>(url, uuids);
            return ProcessResponse(array);
        }


        public async Task<IEnumerable<ITrakHoundSourceQueryResult>> QueryUuidChain(string uuid, string routerId = null)
        {
            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundSourceEntity>());
            url = Url.Combine(url, uuid);
            url = Url.Combine(url, "chain/uuid");
            url = Url.AddQueryParameter(url, "routerId", BaseClient.GetRouterId(routerId));

            var httpResults = await RestRequest.Get<IEnumerable<TrakHoundHttpSourceQueryResult>>(url);
            if (!httpResults.IsNullOrEmpty())
            {
                var results = new List<ITrakHoundSourceQueryResult>();

                foreach (var httpResult in httpResults)
                {
                    results.Add(httpResult.ToQueryResult());
                }

                return results;
            }

            return null;
        }

        public async Task<IEnumerable<ITrakHoundSourceQueryResult>> QueryUuidChain(IEnumerable<string> uuids, string routerId = null)
        {
            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundSourceEntity>());
            url = Url.Combine(url, "chain/uuid");
            url = Url.AddQueryParameter(url, "routerId", BaseClient.GetRouterId(routerId));

            var httpResults = await RestRequest.Post<IEnumerable<TrakHoundHttpSourceQueryResult>>(url, uuids);
            if (!httpResults.IsNullOrEmpty())
            {
                var results = new List<ITrakHoundSourceQueryResult>();

                foreach (var httpResult in httpResults)
                {
                    results.Add(httpResult.ToQueryResult());
                }

                return results;
            }

            return null;
        }
    }
}
