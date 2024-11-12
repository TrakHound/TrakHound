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
    internal partial class TrakHoundHttpSystemDefinitionMetadataClient : TrakHoundHttpEntityClientBase<ITrakHoundDefinitionMetadataEntity>, ITrakHoundSystemDefinitionMetadataClient
    {




        public TrakHoundHttpSystemDefinitionMetadataClient(TrakHoundHttpClient baseClient, TrakHoundHttpSystemEntitiesClient entitiesClient) : base (baseClient, entitiesClient) 
        {
        }


        public async Task<IEnumerable<ITrakHoundDefinitionMetadataEntity>> QueryByDefinitionUuid(
            string definitionUuid,
            string routerId = null)
        {
            var route = "";
            route = Url.Combine(route, "definition/{definitionUuid}");
            route = Url.AddRouteParameter(route, "definition/{definitionUuid}", "definitionUuid", definitionUuid);
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundDefinitionMetadataEntity>());
            url = Url.Combine(url, route);

            var result = await RestRequest.Get<object[][]>(url);
            return ProcessResponse(result);
        }

        public async Task<IEnumerable<ITrakHoundDefinitionMetadataEntity>> QueryByDefinitionUuid(
            IEnumerable<string> definitionUuids,
            string routerId = null)
        {
            var route = "";
            route = Url.Combine(route, "definition");
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundDefinitionMetadataEntity>());
            url = Url.Combine(url, route);

            var result = await RestRequest.Post<object[][]>(url, definitionUuids);

            return ProcessResponse(result);
        }

    }
}
