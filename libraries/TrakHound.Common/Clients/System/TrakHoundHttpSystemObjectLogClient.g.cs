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
    internal partial class TrakHoundHttpSystemObjectLogClient : TrakHoundHttpEntityClientBase<ITrakHoundObjectLogEntity>, ITrakHoundSystemObjectLogClient
    {




        public TrakHoundHttpSystemObjectLogClient(TrakHoundHttpClient baseClient, TrakHoundHttpSystemEntitiesClient entitiesClient) : base (baseClient, entitiesClient) 
        {
        }


        public async Task<IEnumerable<ITrakHoundObjectLogEntity>> QueryByObject(
            string path,
            TrakHound.TrakHoundLogLevel minLevel,
            long start,
            long stop,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null)
        {
            var route = "";
            
            route = Url.AddQueryParameter(route, "path", (string)path);
            route = Url.AddQueryParameter(route, "minLevel", (TrakHound.TrakHoundLogLevel)minLevel);
            route = Url.AddQueryParameter(route, "start", (long)start);
            route = Url.AddQueryParameter(route, "stop", (long)stop);
            route = Url.AddQueryParameter(route, "skip", (long)skip);
            route = Url.AddQueryParameter(route, "take", (long)take);
            route = Url.AddQueryParameter(route, "sortOrder", (int)sortOrder);
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectLogEntity>());
            url = Url.Combine(url, route);

            var result = await RestRequest.Get<object[][]>(url);
            return ProcessResponse(result);
        }

        public async Task<IEnumerable<ITrakHoundObjectLogEntity>> QueryByObject(
            IEnumerable<string> paths,
            TrakHound.TrakHoundLogLevel minLevel,
            long start,
            long stop,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null)
        {
            var route = "";
            route = Url.Combine(route, "object/path");
            route = Url.AddQueryParameter(route, "minLevel", (TrakHound.TrakHoundLogLevel)minLevel);
            route = Url.AddQueryParameter(route, "start", (long)start);
            route = Url.AddQueryParameter(route, "stop", (long)stop);
            route = Url.AddQueryParameter(route, "skip", (long)skip);
            route = Url.AddQueryParameter(route, "take", (long)take);
            route = Url.AddQueryParameter(route, "sortOrder", (int)sortOrder);
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectLogEntity>());
            url = Url.Combine(url, route);

            var result = await RestRequest.Post<object[][]>(url, paths);

            return ProcessResponse(result);
        }

        public async Task<IEnumerable<ITrakHoundObjectLogEntity>> QueryByObjectUuid(
            string objectUuid,
            TrakHound.TrakHoundLogLevel minLevel,
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
            route = Url.AddQueryParameter(route, "minLevel", (TrakHound.TrakHoundLogLevel)minLevel);
            route = Url.AddQueryParameter(route, "start", (long)start);
            route = Url.AddQueryParameter(route, "stop", (long)stop);
            route = Url.AddQueryParameter(route, "skip", (long)skip);
            route = Url.AddQueryParameter(route, "take", (long)take);
            route = Url.AddQueryParameter(route, "sortOrder", (int)sortOrder);
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectLogEntity>());
            url = Url.Combine(url, route);

            var result = await RestRequest.Get<object[][]>(url);
            return ProcessResponse(result);
        }

        public async Task<IEnumerable<ITrakHoundObjectLogEntity>> QueryByObjectUuid(
            IEnumerable<string> objectUuids,
            TrakHound.TrakHoundLogLevel minLevel,
            long start,
            long stop,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null)
        {
            var route = "";
            route = Url.Combine(route, "object");
            route = Url.AddQueryParameter(route, "minLevel", (TrakHound.TrakHoundLogLevel)minLevel);
            route = Url.AddQueryParameter(route, "start", (long)start);
            route = Url.AddQueryParameter(route, "stop", (long)stop);
            route = Url.AddQueryParameter(route, "skip", (long)skip);
            route = Url.AddQueryParameter(route, "take", (long)take);
            route = Url.AddQueryParameter(route, "sortOrder", (int)sortOrder);
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectLogEntity>());
            url = Url.Combine(url, route);

            var result = await RestRequest.Post<object[][]>(url, objectUuids);

            return ProcessResponse(result);
        }

        public async Task<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectLogEntity>>> SubscribeByObject(
            IEnumerable<string> paths,
            TrakHound.TrakHoundLogLevel minLevel,
            int interval = 0,
            int take = 1000,
            string consumerId = null,
            string routerId = null)
        {
            var route = "";
            route = Url.Combine(route, "object/path/subscribe");
            route = Url.AddQueryParameter(route, "minLevel", (TrakHound.TrakHoundLogLevel)minLevel);
            route = Url.AddQueryParameter(route, "interval", (int)interval);
            route = Url.AddQueryParameter(route, "take", (int)take);
            route = Url.AddQueryParameter(route, "consumerId", (string)consumerId);
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));
            route = Url.AddQueryParameter(route, "requestBody", "true");

            var url = TrakHoundHttp.GetEntityPath<ITrakHoundObjectLogEntity>();
            url = Url.Combine(url, route);


            var consumer = new TrakHoundEntityListClientConsumer<ITrakHoundObjectLogEntity>(BaseClient.ClientConfiguration, url, paths);
            consumer.Subscribe();
            return consumer;
        }

        public async Task<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectLogEntity>>> SubscribeByObjectUuid(
            IEnumerable<string> objectUuids,
            TrakHound.TrakHoundLogLevel minLevel,
            int interval = 0,
            int take = 1000,
            string consumerId = null,
            string routerId = null)
        {
            var route = "";
            route = Url.Combine(route, "object/subscribe");
            route = Url.AddQueryParameter(route, "minLevel", (TrakHound.TrakHoundLogLevel)minLevel);
            route = Url.AddQueryParameter(route, "interval", (int)interval);
            route = Url.AddQueryParameter(route, "take", (int)take);
            route = Url.AddQueryParameter(route, "consumerId", (string)consumerId);
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));
            route = Url.AddQueryParameter(route, "requestBody", "true");

            var url = TrakHoundHttp.GetEntityPath<ITrakHoundObjectLogEntity>();
            url = Url.Combine(url, route);


            var consumer = new TrakHoundEntityListClientConsumer<ITrakHoundObjectLogEntity>(BaseClient.ClientConfiguration, url, objectUuids);
            consumer.Subscribe();
            return consumer;
        }

        public async Task<bool> DeleteByObjectUuid(
            string objectUuid,
            string routerId = null)
        {
            var route = "";
            route = Url.Combine(route, "object/{objectUuid}");
            route = Url.AddRouteParameter(route, "object/{objectUuid}", "objectUuid", objectUuid);
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectLogEntity>());
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

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectLogEntity>());
            url = Url.Combine(url, route);
            return (await RestRequest.PostResponse(url, objectUuids)).StatusCode == 200;
        }

    }
}
