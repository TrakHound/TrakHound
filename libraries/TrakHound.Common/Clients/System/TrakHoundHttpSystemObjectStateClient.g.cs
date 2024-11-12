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
    internal partial class TrakHoundHttpSystemObjectStateClient : TrakHoundHttpEntityClientBase<ITrakHoundObjectStateEntity>, ITrakHoundSystemObjectStateClient
    {




        public TrakHoundHttpSystemObjectStateClient(TrakHoundHttpClient baseClient, TrakHoundHttpSystemEntitiesClient entitiesClient) : base (baseClient, entitiesClient) 
        {
        }


        public async Task<IEnumerable<ITrakHoundObjectStateEntity>> LatestByObject(
            string path,
            string routerId = null)
        {
            var route = "";
            route = Url.Combine(route, "latest");
            route = Url.AddQueryParameter(route, "path", (string)path);
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectStateEntity>());
            url = Url.Combine(url, route);

            var result = await RestRequest.Get<object[][]>(url);
            return ProcessResponse(result);
        }

        public async Task<IEnumerable<ITrakHoundObjectStateEntity>> LatestByObject(
            IEnumerable<string> paths,
            string routerId = null)
        {
            var route = "";
            route = Url.Combine(route, "latest");
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectStateEntity>());
            url = Url.Combine(url, route);

            var result = await RestRequest.Post<object[][]>(url, paths);

            return ProcessResponse(result);
        }

        public async Task<IEnumerable<ITrakHoundObjectStateEntity>> LatestByObjectUuid(
            string objectUuid,
            string routerId = null)
        {
            var route = "";
            route = Url.Combine(route, "latest/object/{objectUuid}");
            route = Url.AddRouteParameter(route, "latest/object/{objectUuid}", "objectUuid", objectUuid);
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectStateEntity>());
            url = Url.Combine(url, route);

            var result = await RestRequest.Get<object[][]>(url);
            return ProcessResponse(result);
        }

        public async Task<IEnumerable<ITrakHoundObjectStateEntity>> LatestByObjectUuid(
            IEnumerable<string> objectUuids,
            string routerId = null)
        {
            var route = "";
            route = Url.Combine(route, "latest/object");
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectStateEntity>());
            url = Url.Combine(url, route);

            var result = await RestRequest.Post<object[][]>(url, objectUuids);

            return ProcessResponse(result);
        }

        public async Task<IEnumerable<ITrakHoundObjectStateEntity>> QueryByObject(
            string path,
            long start,
            long stop,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null)
        {
            var route = "";
            
            route = Url.AddQueryParameter(route, "path", (string)path);
            route = Url.AddQueryParameter(route, "start", (long)start);
            route = Url.AddQueryParameter(route, "stop", (long)stop);
            route = Url.AddQueryParameter(route, "skip", (long)skip);
            route = Url.AddQueryParameter(route, "take", (long)take);
            route = Url.AddQueryParameter(route, "sortOrder", (int)sortOrder);
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectStateEntity>());
            url = Url.Combine(url, route);

            var result = await RestRequest.Get<object[][]>(url);
            return ProcessResponse(result);
        }

        public async Task<IEnumerable<ITrakHoundObjectStateEntity>> QueryByObject(
            IEnumerable<string> paths,
            long start,
            long stop,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null)
        {
            var route = "";
            
            route = Url.AddQueryParameter(route, "start", (long)start);
            route = Url.AddQueryParameter(route, "stop", (long)stop);
            route = Url.AddQueryParameter(route, "skip", (long)skip);
            route = Url.AddQueryParameter(route, "take", (long)take);
            route = Url.AddQueryParameter(route, "sortOrder", (int)sortOrder);
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectStateEntity>());
            url = Url.Combine(url, route);

            var result = await RestRequest.Post<object[][]>(url, paths);

            return ProcessResponse(result);
        }

        public async Task<IEnumerable<ITrakHoundObjectStateEntity>> QueryByObjectUuid(
            string objectUuid,
            long start,
            long stop,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null)
        {
            var route = "";
            route = Url.Combine(route, "object/{objectUuid}");
            route = Url.AddRouteParameter(route, "object/{objectUuid}", "objectUuid", objectUuid);
            route = Url.AddQueryParameter(route, "start", (long)start);
            route = Url.AddQueryParameter(route, "stop", (long)stop);
            route = Url.AddQueryParameter(route, "skip", (long)skip);
            route = Url.AddQueryParameter(route, "take", (long)take);
            route = Url.AddQueryParameter(route, "sortOrder", (int)sortOrder);
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectStateEntity>());
            url = Url.Combine(url, route);

            var result = await RestRequest.Get<object[][]>(url);
            return ProcessResponse(result);
        }

        public async Task<IEnumerable<ITrakHoundObjectStateEntity>> QueryByObjectUuid(
            IEnumerable<string> objectUuids,
            long start,
            long stop,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null)
        {
            var route = "";
            route = Url.Combine(route, "object");
            route = Url.AddQueryParameter(route, "start", (long)start);
            route = Url.AddQueryParameter(route, "stop", (long)stop);
            route = Url.AddQueryParameter(route, "skip", (long)skip);
            route = Url.AddQueryParameter(route, "take", (long)take);
            route = Url.AddQueryParameter(route, "sortOrder", (int)sortOrder);
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectStateEntity>());
            url = Url.Combine(url, route);

            var result = await RestRequest.Post<object[][]>(url, objectUuids);

            return ProcessResponse(result);
        }

        public async Task<IEnumerable<ITrakHoundObjectStateEntity>> LastByObject(
            string path,
            long timestamp,
            string routerId = null)
        {
            var route = "";
            route = Url.Combine(route, "last");
            route = Url.AddQueryParameter(route, "path", (string)path);
            route = Url.AddQueryParameter(route, "timestamp", (long)timestamp);
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectStateEntity>());
            url = Url.Combine(url, route);

            var result = await RestRequest.Get<object[][]>(url);
            return ProcessResponse(result);
        }

        public async Task<IEnumerable<ITrakHoundObjectStateEntity>> LastByObject(
            IEnumerable<string> paths,
            long timestamp,
            string routerId = null)
        {
            var route = "";
            route = Url.Combine(route, "last");
            route = Url.AddQueryParameter(route, "timestamp", (long)timestamp);
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectStateEntity>());
            url = Url.Combine(url, route);

            var result = await RestRequest.Post<object[][]>(url, paths);

            return ProcessResponse(result);
        }

        public async Task<IEnumerable<ITrakHoundObjectStateEntity>> LastByObjectUuid(
            string objectUuid,
            long timestamp,
            string routerId = null)
        {
            var route = "";
            route = Url.Combine(route, "last/object/{objectUuid}");
            route = Url.AddRouteParameter(route, "last/object/{objectUuid}", "objectUuid", objectUuid);
            route = Url.AddQueryParameter(route, "timestamp", (long)timestamp);
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectStateEntity>());
            url = Url.Combine(url, route);

            var result = await RestRequest.Get<object[][]>(url);
            return ProcessResponse(result);
        }

        public async Task<IEnumerable<ITrakHoundObjectStateEntity>> LastByObjectUuid(
            IEnumerable<string> objectUuids,
            long timestamp,
            string routerId = null)
        {
            var route = "";
            route = Url.Combine(route, "last/object");
            route = Url.AddQueryParameter(route, "timestamp", (long)timestamp);
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectStateEntity>());
            url = Url.Combine(url, route);

            var result = await RestRequest.Post<object[][]>(url, objectUuids);

            return ProcessResponse(result);
        }

        public async Task<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectStateEntity>>> SubscribeByObject(
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

            var url = TrakHoundHttp.GetEntityPath<ITrakHoundObjectStateEntity>();
            url = Url.Combine(url, route);


            var consumer = new TrakHoundEntityListClientConsumer<ITrakHoundObjectStateEntity>(BaseClient.ClientConfiguration, url, paths);
            consumer.Subscribe();
            return consumer;
        }

        public async Task<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectStateEntity>>> SubscribeByObjectUuid(
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

            var url = TrakHoundHttp.GetEntityPath<ITrakHoundObjectStateEntity>();
            url = Url.Combine(url, route);


            var consumer = new TrakHoundEntityListClientConsumer<ITrakHoundObjectStateEntity>(BaseClient.ClientConfiguration, url, objectUuids);
            consumer.Subscribe();
            return consumer;
        }

    }
}
