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
    internal partial class TrakHoundHttpSystemObjectStatisticClient : TrakHoundHttpEntityClientBase<ITrakHoundObjectStatisticEntity>, ITrakHoundSystemObjectStatisticClient
    {




        public TrakHoundHttpSystemObjectStatisticClient(TrakHoundHttpClient baseClient, TrakHoundHttpSystemEntitiesClient entitiesClient) : base (baseClient, entitiesClient) 
        {
        }


        public async Task<IEnumerable<ITrakHoundObjectStatisticEntity>> QueryByObject(
            string path,
            long start,
            long stop,
            long span,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null)
        {
            var route = "";
            
            route = Url.AddQueryParameter(route, "path", (string)path);
            route = Url.AddQueryParameter(route, "start", (long)start);
            route = Url.AddQueryParameter(route, "stop", (long)stop);
            route = Url.AddQueryParameter(route, "span", (long)span);
            route = Url.AddQueryParameter(route, "skip", (long)skip);
            route = Url.AddQueryParameter(route, "take", (long)take);
            route = Url.AddQueryParameter(route, "sortOrder", (int)sortOrder);
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectStatisticEntity>());
            url = Url.Combine(url, route);

            var result = await RestRequest.Get<object[][]>(url);
            return ProcessResponse(result);
        }

        public async Task<IEnumerable<ITrakHoundObjectStatisticEntity>> QueryByObject(
            IEnumerable<string> paths,
            long start,
            long stop,
            long span,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null)
        {
            var route = "";
            route = Url.Combine(route, "object/path");
            route = Url.AddQueryParameter(route, "start", (long)start);
            route = Url.AddQueryParameter(route, "stop", (long)stop);
            route = Url.AddQueryParameter(route, "span", (long)span);
            route = Url.AddQueryParameter(route, "skip", (long)skip);
            route = Url.AddQueryParameter(route, "take", (long)take);
            route = Url.AddQueryParameter(route, "sortOrder", (int)sortOrder);
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectStatisticEntity>());
            url = Url.Combine(url, route);

            var result = await RestRequest.Post<object[][]>(url, paths);

            return ProcessResponse(result);
        }

        public async Task<IEnumerable<ITrakHoundObjectStatisticEntity>> QueryByObjectUuid(
            string objectUuid,
            long start,
            long stop,
            long span,
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
            route = Url.AddQueryParameter(route, "span", (long)span);
            route = Url.AddQueryParameter(route, "skip", (long)skip);
            route = Url.AddQueryParameter(route, "take", (long)take);
            route = Url.AddQueryParameter(route, "sortOrder", (int)sortOrder);
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectStatisticEntity>());
            url = Url.Combine(url, route);

            var result = await RestRequest.Get<object[][]>(url);
            return ProcessResponse(result);
        }

        public async Task<IEnumerable<ITrakHoundObjectStatisticEntity>> QueryByObjectUuid(
            IEnumerable<string> objectUuids,
            long start,
            long stop,
            long span,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null)
        {
            var route = "";
            route = Url.Combine(route, "object");
            route = Url.AddQueryParameter(route, "start", (long)start);
            route = Url.AddQueryParameter(route, "stop", (long)stop);
            route = Url.AddQueryParameter(route, "span", (long)span);
            route = Url.AddQueryParameter(route, "skip", (long)skip);
            route = Url.AddQueryParameter(route, "take", (long)take);
            route = Url.AddQueryParameter(route, "sortOrder", (int)sortOrder);
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectStatisticEntity>());
            url = Url.Combine(url, route);

            var result = await RestRequest.Post<object[][]>(url, objectUuids);

            return ProcessResponse(result);
        }

        public async Task<IEnumerable<TrakHoundTimeRangeSpan>> SpansByObject(
            string path,
            long start,
            long stop,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null)
        {
            var route = "";
            route = Url.Combine(route, "spans");
            route = Url.AddQueryParameter(route, "path", (string)path);
            route = Url.AddQueryParameter(route, "start", (long)start);
            route = Url.AddQueryParameter(route, "stop", (long)stop);
            route = Url.AddQueryParameter(route, "skip", (long)skip);
            route = Url.AddQueryParameter(route, "take", (long)take);
            route = Url.AddQueryParameter(route, "sortOrder", (int)sortOrder);
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectStatisticEntity>());
            url = Url.Combine(url, route);

            var result = await RestRequest.Get<IEnumerable<TrakHoundHttpTimeRangeSpanResponse>>(url);
            return ProcessResponse(result);
        }

        public async Task<IEnumerable<TrakHoundTimeRangeSpan>> SpansByObject(
            IEnumerable<string> paths,
            long start,
            long stop,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null)
        {
            var route = "";
            route = Url.Combine(route, "spans/object/path");
            route = Url.AddQueryParameter(route, "start", (long)start);
            route = Url.AddQueryParameter(route, "stop", (long)stop);
            route = Url.AddQueryParameter(route, "skip", (long)skip);
            route = Url.AddQueryParameter(route, "take", (long)take);
            route = Url.AddQueryParameter(route, "sortOrder", (int)sortOrder);
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectStatisticEntity>());
            url = Url.Combine(url, route);

            var result = await RestRequest.Post<IEnumerable<TrakHoundHttpTimeRangeSpanResponse>>(url, paths);

            return ProcessResponse(result);
        }

        public async Task<IEnumerable<TrakHoundTimeRangeSpan>> SpansByObjectUuid(
            string objectUuid,
            long start,
            long stop,
            string routerId = null)
        {
            var route = "";
            route = Url.Combine(route, "object/{objectUuid}/spans");
            route = Url.AddRouteParameter(route, "object/{objectUuid}/spans", "objectUuid", objectUuid);
            route = Url.AddQueryParameter(route, "start", (long)start);
            route = Url.AddQueryParameter(route, "stop", (long)stop);
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectStatisticEntity>());
            url = Url.Combine(url, route);

            var result = await RestRequest.Get<IEnumerable<TrakHoundHttpTimeRangeSpanResponse>>(url);
            return ProcessResponse(result);
        }

        public async Task<IEnumerable<TrakHoundTimeRangeSpan>> SpansByObjectUuid(
            IEnumerable<string> objectUuids,
            long start,
            long stop,
            string routerId = null)
        {
            var route = "";
            route = Url.Combine(route, "object/spans");
            route = Url.AddQueryParameter(route, "start", (long)start);
            route = Url.AddQueryParameter(route, "stop", (long)stop);
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectStatisticEntity>());
            url = Url.Combine(url, route);

            var result = await RestRequest.Post<IEnumerable<TrakHoundHttpTimeRangeSpanResponse>>(url, objectUuids);

            return ProcessResponse(result);
        }

        public async Task<IEnumerable<TrakHoundCount>> CountByObject(
            string path,
            long start,
            long stop,
            long span,
            string routerId = null)
        {
            var route = "";
            route = Url.Combine(route, "count");
            route = Url.AddQueryParameter(route, "path", (string)path);
            route = Url.AddQueryParameter(route, "start", (long)start);
            route = Url.AddQueryParameter(route, "stop", (long)stop);
            route = Url.AddQueryParameter(route, "span", (long)span);
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectStatisticEntity>());
            url = Url.Combine(url, route);

            var result = await RestRequest.Get<IEnumerable<TrakHoundHttpCountResponse>>(url);
            return ProcessResponse(result);
        }

        public async Task<IEnumerable<TrakHoundCount>> CountByObject(
            IEnumerable<string> paths,
            long start,
            long stop,
            long span,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null)
        {
            var route = "";
            route = Url.Combine(route, "count/object/path");
            route = Url.AddQueryParameter(route, "start", (long)start);
            route = Url.AddQueryParameter(route, "stop", (long)stop);
            route = Url.AddQueryParameter(route, "span", (long)span);
            route = Url.AddQueryParameter(route, "skip", (long)skip);
            route = Url.AddQueryParameter(route, "take", (long)take);
            route = Url.AddQueryParameter(route, "sortOrder", (int)sortOrder);
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectStatisticEntity>());
            url = Url.Combine(url, route);

            var result = await RestRequest.Post<IEnumerable<TrakHoundHttpCountResponse>>(url, paths);

            return ProcessResponse(result);
        }

        public async Task<TrakHoundCount> CountByObjectUuid(
            string objectUuid,
            long start,
            long stop,
            long span,
            string routerId = null)
        {
            var route = "";
            route = Url.Combine(route, "object/{objectUuid}/count");
            route = Url.AddRouteParameter(route, "object/{objectUuid}/count", "objectUuid", objectUuid);
            route = Url.AddQueryParameter(route, "start", (long)start);
            route = Url.AddQueryParameter(route, "stop", (long)stop);
            route = Url.AddQueryParameter(route, "span", (long)span);
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectStatisticEntity>());
            url = Url.Combine(url, route);

            var result = await RestRequest.Get<TrakHoundHttpCountResponse>(url);
            return ProcessResponse(result);
        }

        public async Task<IEnumerable<TrakHoundCount>> CountByObjectUuid(
            IEnumerable<string> objectUuids,
            long start,
            long stop,
            long span,
            string routerId = null)
        {
            var route = "";
            route = Url.Combine(route, "object/count");
            route = Url.AddQueryParameter(route, "start", (long)start);
            route = Url.AddQueryParameter(route, "stop", (long)stop);
            route = Url.AddQueryParameter(route, "span", (long)span);
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectStatisticEntity>());
            url = Url.Combine(url, route);

            var result = await RestRequest.Post<IEnumerable<TrakHoundHttpCountResponse>>(url, objectUuids);

            return ProcessResponse(result);
        }

        public async Task<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectStatisticEntity>>> SubscribeByObject(
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

            var url = TrakHoundHttp.GetEntityPath<ITrakHoundObjectStatisticEntity>();
            url = Url.Combine(url, route);


            var consumer = new TrakHoundEntityListClientConsumer<ITrakHoundObjectStatisticEntity>(BaseClient.ClientConfiguration, url, paths);
            consumer.Subscribe();
            return consumer;
        }

        public async Task<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectStatisticEntity>>> SubscribeByObjectUuid(
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

            var url = TrakHoundHttp.GetEntityPath<ITrakHoundObjectStatisticEntity>();
            url = Url.Combine(url, route);


            var consumer = new TrakHoundEntityListClientConsumer<ITrakHoundObjectStatisticEntity>(BaseClient.ClientConfiguration, url, objectUuids);
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

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectStatisticEntity>());
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

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectStatisticEntity>());
            url = Url.Combine(url, route);
            return (await RestRequest.PostResponse(url, objectUuids)).StatusCode == 200;
        }

    }
}
