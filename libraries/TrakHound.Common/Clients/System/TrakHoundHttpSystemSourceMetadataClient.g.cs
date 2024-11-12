// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrakHound.Configurations;
using TrakHound.Entities;
using TrakHound.Http;

namespace TrakHound.Clients
{
    internal partial class TrakHoundHttpSystemSourceMetadataClient : TrakHoundHttpEntityClientBase<ITrakHoundSourceMetadataEntity>, ITrakHoundSystemSourceMetadataClient
    {




        public TrakHoundHttpSystemSourceMetadataClient(TrakHoundHttpClient baseClient, TrakHoundHttpSystemEntitiesClient entitiesClient) : base (baseClient, entitiesClient) 
        {
        }


        public async Task<IEnumerable<ITrakHoundSourceMetadataEntity>> QueryBySourceUuid(
            string sourceUuid,
            string routerId = null)
        {
            var route = "";
            route = Url.Combine(route, "source/{sourceUuid}");
            route = Url.AddRouteParameter(route, "source/{sourceUuid}", "sourceUuid", sourceUuid);
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundSourceMetadataEntity>());
            url = Url.Combine(url, route);

            var result = await RestRequest.Get<object[][]>(url);
            return ProcessResponse(result);
        }

        public async Task<IEnumerable<ITrakHoundSourceMetadataEntity>> QueryBySourceUuid(
            IEnumerable<string> sourceUuids,
            string routerId = null)
        {
            var route = "";
            route = Url.Combine(route, "source");
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundSourceMetadataEntity>());
            url = Url.Combine(url, route);

            var result = await RestRequest.Post<object[][]>(url, sourceUuids);

            return ProcessResponse(result);
        }

    }
}
