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
    internal partial class TrakHoundHttpSystemObjectMetadataClient : TrakHoundHttpEntityClientBase<ITrakHoundObjectMetadataEntity>, ITrakHoundSystemObjectMetadataClient
    {




        public TrakHoundHttpSystemObjectMetadataClient(TrakHoundHttpClient baseClient, TrakHoundHttpSystemEntitiesClient entitiesClient) : base (baseClient, entitiesClient) 
        {
        }


        public async Task<IEnumerable<ITrakHoundObjectMetadataEntity>> QueryByEntityUuid(
            string entityUuid,
            string routerId = null)
        {
            var route = "";
            route = Url.Combine(route, "entity/{entityUuid}");
            route = Url.AddRouteParameter(route, "entity/{entityUuid}", "entityUuid", entityUuid);
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectMetadataEntity>());
            url = Url.Combine(url, route);

            var result = await RestRequest.Get<object[][]>(url);
            return ProcessResponse(result);
        }

        public async Task<IEnumerable<ITrakHoundObjectMetadataEntity>> QueryByEntityUuid(
            IEnumerable<string> entityUuids,
            string routerId = null)
        {
            var route = "";
            route = Url.Combine(route, "entity");
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectMetadataEntity>());
            url = Url.Combine(url, route);

            var result = await RestRequest.Post<object[][]>(url, entityUuids);

            return ProcessResponse(result);
        }

        public async Task<IEnumerable<ITrakHoundObjectMetadataEntity>> QueryByEntityUuid(
            string entityUuid,
            string name,
            TrakHound.Entities.TrakHoundMetadataQueryType queryType,
            string query,
            string routerId = null)
        {
            var route = "";
            route = Url.Combine(route, "entity/name/{entityUuid}");
            route = Url.AddRouteParameter(route, "entity/name/{entityUuid}", "entityUuid", entityUuid);
            route = Url.AddQueryParameter(route, "name", (string)name);
            route = Url.AddQueryParameter(route, "queryType", (TrakHound.Entities.TrakHoundMetadataQueryType)queryType);
            route = Url.AddQueryParameter(route, "query", (string)query);
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectMetadataEntity>());
            url = Url.Combine(url, route);

            var result = await RestRequest.Get<object[][]>(url);
            return ProcessResponse(result);
        }

        public async Task<IEnumerable<ITrakHoundObjectMetadataEntity>> QueryByEntityUuid(
            IEnumerable<string> entityUuids,
            string name,
            TrakHound.Entities.TrakHoundMetadataQueryType queryType,
            string query,
            string routerId = null)
        {
            var route = "";
            route = Url.Combine(route, "entity/name");
            route = Url.AddQueryParameter(route, "name", (string)name);
            route = Url.AddQueryParameter(route, "queryType", (TrakHound.Entities.TrakHoundMetadataQueryType)queryType);
            route = Url.AddQueryParameter(route, "query", (string)query);
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectMetadataEntity>());
            url = Url.Combine(url, route);

            var result = await RestRequest.Post<object[][]>(url, entityUuids);

            return ProcessResponse(result);
        }

        public async Task<IEnumerable<ITrakHoundObjectMetadataEntity>> QueryByName(
            string name,
            TrakHound.Entities.TrakHoundMetadataQueryType queryType,
            string query,
            string routerId = null)
        {
            var route = "";
            route = Url.Combine(route, "name");
            route = Url.AddQueryParameter(route, "name", (string)name);
            route = Url.AddQueryParameter(route, "queryType", (TrakHound.Entities.TrakHoundMetadataQueryType)queryType);
            route = Url.AddQueryParameter(route, "query", (string)query);
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectMetadataEntity>());
            url = Url.Combine(url, route);

            var result = await RestRequest.Get<object[][]>(url);
            return ProcessResponse(result);
        }

        public async Task<bool> DeleteByObjectUuid(
            string objectUuid,
            string routerId = null)
        {
            var route = "";
            route = Url.Combine(route, "object/{objectUuid}");
            route = Url.AddRouteParameter(route, "object/{objectUuid}", "objectUuid", objectUuid);
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectMetadataEntity>());
            url = Url.Combine(url, route);
            return await RestRequest.Delete(url);
        }

        public async Task<bool> DeleteByObjectUuid(
            IEnumerable<string> objectUuids,
            string routerId = null)
        {
            var route = "";
            route = Url.Combine(route, "delete/object");
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectMetadataEntity>());
            url = Url.Combine(url, route);
            return (await RestRequest.PostResponse(url, objectUuids)).StatusCode == 200;
        }

    }
}
