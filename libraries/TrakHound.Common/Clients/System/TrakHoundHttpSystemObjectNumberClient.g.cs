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
    internal partial class TrakHoundHttpSystemObjectNumberClient : TrakHoundHttpEntityClientBase<ITrakHoundObjectNumberEntity>, ITrakHoundSystemObjectNumberClient
    {




        public TrakHoundHttpSystemObjectNumberClient(TrakHoundHttpClient baseClient, TrakHoundHttpSystemEntitiesClient entitiesClient) : base (baseClient, entitiesClient) 
        {
        }


        public async Task<IEnumerable<ITrakHoundObjectNumberEntity>> QueryByObject(
            string path,
            string routerId = null)
        {
            var route = "";
            
            route = Url.AddQueryParameter(route, "path", (string)path);
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectNumberEntity>());
            url = Url.Combine(url, route);

            var result = await RestRequest.Get<object[][]>(url);
            return ProcessResponse(result);
        }

        public async Task<IEnumerable<ITrakHoundObjectNumberEntity>> QueryByObject(
            IEnumerable<string> paths,
            string routerId = null)
        {
            var route = "";
            route = Url.Combine(route, "object/path");
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectNumberEntity>());
            url = Url.Combine(url, route);

            var result = await RestRequest.Post<object[][]>(url, paths);

            return ProcessResponse(result);
        }

        public async Task<ITrakHoundObjectNumberEntity> QueryByObjectUuid(
            string objectUuid,
            string routerId = null)
        {
            var route = "";
            route = Url.Combine(route, "object/{objectUuid}");
            route = Url.AddRouteParameter(route, "object/{objectUuid}", "objectUuid", objectUuid);
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectNumberEntity>());
            url = Url.Combine(url, route);

            var result = await RestRequest.Get<object[]>(url);
            return ProcessResponse(result);
        }

        public async Task<IEnumerable<ITrakHoundObjectNumberEntity>> QueryByObjectUuid(
            IEnumerable<string> objectUuids,
            string routerId = null)
        {
            var route = "";
            route = Url.Combine(route, "object");
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectNumberEntity>());
            url = Url.Combine(url, route);

            var result = await RestRequest.Post<object[][]>(url, objectUuids);

            return ProcessResponse(result);
        }

        public async Task<IEnumerable<ITrakHoundObjectNumberEntity>> QueryByObject(
            string path,
            double minimum,
            double maximum,
            long objectSkip,
            long objectTake,
            string routerId = null)
        {
            var route = "";
            route = Url.Combine(route, "object/filter");
            route = Url.AddQueryParameter(route, "path", (string)path);
            route = Url.AddQueryParameter(route, "minimum", (double)minimum);
            route = Url.AddQueryParameter(route, "maximum", (double)maximum);
            route = Url.AddQueryParameter(route, "objectSkip", (long)objectSkip);
            route = Url.AddQueryParameter(route, "objectTake", (long)objectTake);
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectNumberEntity>());
            url = Url.Combine(url, route);

            var result = await RestRequest.Get<object[][]>(url);
            return ProcessResponse(result);
        }

        public async Task<IEnumerable<ITrakHoundObjectNumberEntity>> QueryByObject(
            IEnumerable<string> paths,
            double minimum,
            double maximum,
            long objectSkip,
            long objectTake,
            string routerId = null)
        {
            var route = "";
            route = Url.Combine(route, "object/path/filter");
            route = Url.AddQueryParameter(route, "minimum", (double)minimum);
            route = Url.AddQueryParameter(route, "maximum", (double)maximum);
            route = Url.AddQueryParameter(route, "objectSkip", (long)objectSkip);
            route = Url.AddQueryParameter(route, "objectTake", (long)objectTake);
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectNumberEntity>());
            url = Url.Combine(url, route);

            var result = await RestRequest.Post<object[][]>(url, paths);

            return ProcessResponse(result);
        }

        public async Task<ITrakHoundObjectNumberEntity> QueryByObjectUuid(
            string objectUuid,
            double minimum,
            double maximum,
            string routerId = null)
        {
            var route = "";
            route = Url.Combine(route, "object/{objectUuid}/filter");
            route = Url.AddRouteParameter(route, "object/{objectUuid}/filter", "objectUuid", objectUuid);
            route = Url.AddQueryParameter(route, "minimum", (double)minimum);
            route = Url.AddQueryParameter(route, "maximum", (double)maximum);
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectNumberEntity>());
            url = Url.Combine(url, route);

            var result = await RestRequest.Get<object[]>(url);
            return ProcessResponse(result);
        }

        public async Task<IEnumerable<ITrakHoundObjectNumberEntity>> QueryByObjectUuid(
            IEnumerable<string> objectUuids,
            double minimum,
            double maximum,
            string routerId = null)
        {
            var route = "";
            route = Url.Combine(route, "object/filter");
            route = Url.AddQueryParameter(route, "minimum", (double)minimum);
            route = Url.AddQueryParameter(route, "maximum", (double)maximum);
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectNumberEntity>());
            url = Url.Combine(url, route);

            var result = await RestRequest.Post<object[][]>(url, objectUuids);

            return ProcessResponse(result);
        }

        public async Task<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectNumberEntity>>> SubscribeByObject(
            IEnumerable<string> paths,
            int interval = 0,
            int take = 1000,
            string consumerId = null,
            string routerId = null)
        {
            var route = "";
            route = Url.Combine(route, "object/path/subscribe");
            route = Url.AddQueryParameter(route, "interval", (int)interval);
            route = Url.AddQueryParameter(route, "take", (int)take);
            route = Url.AddQueryParameter(route, "consumerId", (string)consumerId);
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));
            route = Url.AddQueryParameter(route, "requestBody", "true");

            var url = TrakHoundHttp.GetEntityPath<ITrakHoundObjectNumberEntity>();
            url = Url.Combine(url, route);


            var consumer = new TrakHoundEntityListClientConsumer<ITrakHoundObjectNumberEntity>(BaseClient.ClientConfiguration, url, paths);
            consumer.Subscribe();
            return consumer;
        }

        public async Task<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectNumberEntity>>> SubscribeByObjectUuid(
            IEnumerable<string> objectUuids,
            int interval = 0,
            int take = 1000,
            string consumerId = null,
            string routerId = null)
        {
            var route = "";
            route = Url.Combine(route, "object/subscribe");
            route = Url.AddQueryParameter(route, "interval", (int)interval);
            route = Url.AddQueryParameter(route, "take", (int)take);
            route = Url.AddQueryParameter(route, "consumerId", (string)consumerId);
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));
            route = Url.AddQueryParameter(route, "requestBody", "true");

            var url = TrakHoundHttp.GetEntityPath<ITrakHoundObjectNumberEntity>();
            url = Url.Combine(url, route);


            var consumer = new TrakHoundEntityListClientConsumer<ITrakHoundObjectNumberEntity>(BaseClient.ClientConfiguration, url, objectUuids);
            consumer.Subscribe();
            return consumer;
        }

    }
}
