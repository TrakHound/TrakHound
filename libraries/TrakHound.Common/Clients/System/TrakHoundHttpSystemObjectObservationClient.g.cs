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
    internal partial class TrakHoundHttpSystemObjectObservationClient : TrakHoundHttpEntityClientBase<ITrakHoundObjectObservationEntity>, ITrakHoundSystemObjectObservationClient
    {




        public TrakHoundHttpSystemObjectObservationClient(TrakHoundHttpClient baseClient, TrakHoundHttpSystemEntitiesClient entitiesClient) : base (baseClient, entitiesClient) 
        {
        }


        public async Task<IEnumerable<ITrakHoundObjectObservationEntity>> LatestByObject(
            string path,
            string routerId = null)
        {
            var route = "";
            route = Url.Combine(route, "latest");
            route = Url.AddQueryParameter(route, "path", (string)path);
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectObservationEntity>());
            url = Url.Combine(url, route);

            var result = await RestRequest.Get<object[][]>(url);
            return ProcessResponse(result);
        }

        public async Task<IEnumerable<ITrakHoundObjectObservationEntity>> LatestByObject(
            IEnumerable<string> paths,
            string routerId = null)
        {
            var route = "";
            route = Url.Combine(route, "latest");
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectObservationEntity>());
            url = Url.Combine(url, route);

            var result = await RestRequest.Post<object[][]>(url, paths);

            return ProcessResponse(result);
        }

        public async Task<IEnumerable<ITrakHoundObjectObservationEntity>> LatestByObjectUuid(
            string objectUuid,
            string routerId = null)
        {
            var route = "";
            route = Url.Combine(route, "latest/object/{objectUuid}");
            route = Url.AddRouteParameter(route, "latest/object/{objectUuid}", "objectUuid", objectUuid);
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectObservationEntity>());
            url = Url.Combine(url, route);

            var result = await RestRequest.Get<object[][]>(url);
            return ProcessResponse(result);
        }

        public async Task<IEnumerable<ITrakHoundObjectObservationEntity>> LatestByObjectUuid(
            IEnumerable<string> objectUuids,
            string routerId = null)
        {
            var route = "";
            route = Url.Combine(route, "latest/object");
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectObservationEntity>());
            url = Url.Combine(url, route);

            var result = await RestRequest.Post<object[][]>(url, objectUuids);

            return ProcessResponse(result);
        }

        public async Task<IEnumerable<ITrakHoundObjectObservationEntity>> QueryByObject(
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

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectObservationEntity>());
            url = Url.Combine(url, route);

            var result = await RestRequest.Get<object[][]>(url);
            return ProcessResponse(result);
        }

        public async Task<IEnumerable<ITrakHoundObjectObservationEntity>> QueryByObject(
            IEnumerable<string> paths,
            long start,
            long stop,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null)
        {
            var route = "";
            route = Url.Combine(route, "object/path");
            route = Url.AddQueryParameter(route, "start", (long)start);
            route = Url.AddQueryParameter(route, "stop", (long)stop);
            route = Url.AddQueryParameter(route, "skip", (long)skip);
            route = Url.AddQueryParameter(route, "take", (long)take);
            route = Url.AddQueryParameter(route, "sortOrder", (int)sortOrder);
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectObservationEntity>());
            url = Url.Combine(url, route);

            var result = await RestRequest.Post<object[][]>(url, paths);

            return ProcessResponse(result);
        }

        public async Task<IEnumerable<ITrakHoundObjectObservationEntity>> QueryByObjectUuid(
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

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectObservationEntity>());
            url = Url.Combine(url, route);

            var result = await RestRequest.Get<object[][]>(url);
            return ProcessResponse(result);
        }

        public async Task<IEnumerable<ITrakHoundObjectObservationEntity>> QueryByObjectUuid(
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

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectObservationEntity>());
            url = Url.Combine(url, route);

            var result = await RestRequest.Post<object[][]>(url, objectUuids);

            return ProcessResponse(result);
        }

        public async Task<IEnumerable<ITrakHoundObjectObservationEntity>> LastByObject(
            string path,
            long timestamp,
            string routerId = null)
        {
            var route = "";
            route = Url.Combine(route, "last");
            route = Url.AddQueryParameter(route, "path", (string)path);
            route = Url.AddQueryParameter(route, "timestamp", (long)timestamp);
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectObservationEntity>());
            url = Url.Combine(url, route);

            var result = await RestRequest.Get<object[][]>(url);
            return ProcessResponse(result);
        }

        public async Task<IEnumerable<ITrakHoundObjectObservationEntity>> LastByObject(
            IEnumerable<string> paths,
            long timestamp,
            string routerId = null)
        {
            var route = "";
            route = Url.Combine(route, "last");
            route = Url.AddQueryParameter(route, "timestamp", (long)timestamp);
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectObservationEntity>());
            url = Url.Combine(url, route);

            var result = await RestRequest.Post<object[][]>(url, paths);

            return ProcessResponse(result);
        }

        public async Task<IEnumerable<ITrakHoundObjectObservationEntity>> LastByObjectUuid(
            string objectUuid,
            long timestamp,
            string routerId = null)
        {
            var route = "";
            route = Url.Combine(route, "last/object/{objectUuid}");
            route = Url.AddRouteParameter(route, "last/object/{objectUuid}", "objectUuid", objectUuid);
            route = Url.AddQueryParameter(route, "timestamp", (long)timestamp);
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectObservationEntity>());
            url = Url.Combine(url, route);

            var result = await RestRequest.Get<object[][]>(url);
            return ProcessResponse(result);
        }

        public async Task<IEnumerable<ITrakHoundObjectObservationEntity>> LastByObjectUuid(
            IEnumerable<string> objectUuids,
            long timestamp,
            string routerId = null)
        {
            var route = "";
            route = Url.Combine(route, "last/object");
            route = Url.AddQueryParameter(route, "timestamp", (long)timestamp);
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectObservationEntity>());
            url = Url.Combine(url, route);

            var result = await RestRequest.Post<object[][]>(url, objectUuids);

            return ProcessResponse(result);
        }

        public async Task<IEnumerable<TrakHoundAggregate>> AggregateByObject(
            string path,
            TrakHound.TrakHoundAggregateType aggregateType,
            long start,
            long stop,
            string routerId = null)
        {
            var route = "";
            route = Url.Combine(route, "aggregate");
            route = Url.AddQueryParameter(route, "path", (string)path);
            route = Url.AddQueryParameter(route, "aggregateType", (TrakHound.TrakHoundAggregateType)aggregateType);
            route = Url.AddQueryParameter(route, "start", (long)start);
            route = Url.AddQueryParameter(route, "stop", (long)stop);
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectObservationEntity>());
            url = Url.Combine(url, route);

            var result = await RestRequest.Get<IEnumerable<TrakHoundHttpAggregateResponse>>(url);
            return ProcessResponse(result);
        }

        public async Task<IEnumerable<TrakHoundAggregate>> AggregateByObject(
            IEnumerable<string> paths,
            TrakHound.TrakHoundAggregateType aggregateType,
            long start,
            long stop,
            string routerId = null)
        {
            var route = "";
            route = Url.Combine(route, "aggregate");
            route = Url.AddQueryParameter(route, "aggregateType", (TrakHound.TrakHoundAggregateType)aggregateType);
            route = Url.AddQueryParameter(route, "start", (long)start);
            route = Url.AddQueryParameter(route, "stop", (long)stop);
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectObservationEntity>());
            url = Url.Combine(url, route);

            var result = await RestRequest.Post<IEnumerable<TrakHoundHttpAggregateResponse>>(url, paths);

            return ProcessResponse(result);
        }

        public async Task<IEnumerable<TrakHoundAggregate>> AggregateByObjectUuid(
            string objectUuid,
            TrakHound.TrakHoundAggregateType aggregateType,
            long start,
            long stop,
            string routerId = null)
        {
            var route = "";
            route = Url.Combine(route, "aggregate/object/{objectUuid}");
            route = Url.AddRouteParameter(route, "aggregate/object/{objectUuid}", "objectUuid", objectUuid);
            route = Url.AddQueryParameter(route, "aggregateType", (TrakHound.TrakHoundAggregateType)aggregateType);
            route = Url.AddQueryParameter(route, "start", (long)start);
            route = Url.AddQueryParameter(route, "stop", (long)stop);
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectObservationEntity>());
            url = Url.Combine(url, route);

            var result = await RestRequest.Get<IEnumerable<TrakHoundHttpAggregateResponse>>(url);
            return ProcessResponse(result);
        }

        public async Task<IEnumerable<TrakHoundAggregate>> AggregateByObjectUuid(
            IEnumerable<string> objectUuids,
            TrakHound.TrakHoundAggregateType aggregateType,
            long start,
            long stop,
            string routerId = null)
        {
            var route = "";
            route = Url.Combine(route, "aggregate/object");
            route = Url.AddQueryParameter(route, "aggregateType", (TrakHound.TrakHoundAggregateType)aggregateType);
            route = Url.AddQueryParameter(route, "start", (long)start);
            route = Url.AddQueryParameter(route, "stop", (long)stop);
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectObservationEntity>());
            url = Url.Combine(url, route);

            var result = await RestRequest.Post<IEnumerable<TrakHoundHttpAggregateResponse>>(url, objectUuids);

            return ProcessResponse(result);
        }

        public async Task<IEnumerable<TrakHoundAggregateWindow>> AggregateWindowByObject(
            string path,
            TrakHound.TrakHoundAggregateType aggregateType,
            long window,
            long start,
            long stop,
            string routerId = null)
        {
            var route = "";
            route = Url.Combine(route, "aggregate/window");
            route = Url.AddQueryParameter(route, "path", (string)path);
            route = Url.AddQueryParameter(route, "aggregateType", (TrakHound.TrakHoundAggregateType)aggregateType);
            route = Url.AddQueryParameter(route, "window", (long)window);
            route = Url.AddQueryParameter(route, "start", (long)start);
            route = Url.AddQueryParameter(route, "stop", (long)stop);
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectObservationEntity>());
            url = Url.Combine(url, route);

            var result = await RestRequest.Get<IEnumerable<TrakHoundHttpAggregateWindowResponse>>(url);
            return ProcessResponse(result);
        }

        public async Task<IEnumerable<TrakHoundAggregateWindow>> AggregateWindowByObject(
            IEnumerable<string> paths,
            TrakHound.TrakHoundAggregateType aggregateType,
            long window,
            long start,
            long stop,
            string routerId = null)
        {
            var route = "";
            route = Url.Combine(route, "aggregate/window");
            route = Url.AddQueryParameter(route, "aggregateType", (TrakHound.TrakHoundAggregateType)aggregateType);
            route = Url.AddQueryParameter(route, "window", (long)window);
            route = Url.AddQueryParameter(route, "start", (long)start);
            route = Url.AddQueryParameter(route, "stop", (long)stop);
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectObservationEntity>());
            url = Url.Combine(url, route);

            var result = await RestRequest.Post<IEnumerable<TrakHoundHttpAggregateWindowResponse>>(url, paths);

            return ProcessResponse(result);
        }

        public async Task<IEnumerable<TrakHoundAggregateWindow>> AggregateWindowByObjectUuid(
            string objectUuid,
            TrakHound.TrakHoundAggregateType aggregateType,
            long window,
            long start,
            long stop,
            string routerId = null)
        {
            var route = "";
            route = Url.Combine(route, "aggregate/window/object/{objectUuid}");
            route = Url.AddRouteParameter(route, "aggregate/window/object/{objectUuid}", "objectUuid", objectUuid);
            route = Url.AddQueryParameter(route, "aggregateType", (TrakHound.TrakHoundAggregateType)aggregateType);
            route = Url.AddQueryParameter(route, "window", (long)window);
            route = Url.AddQueryParameter(route, "start", (long)start);
            route = Url.AddQueryParameter(route, "stop", (long)stop);
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectObservationEntity>());
            url = Url.Combine(url, route);

            var result = await RestRequest.Get<IEnumerable<TrakHoundHttpAggregateWindowResponse>>(url);
            return ProcessResponse(result);
        }

        public async Task<IEnumerable<TrakHoundAggregateWindow>> AggregateWindowByObjectUuid(
            IEnumerable<string> objectUuids,
            TrakHound.TrakHoundAggregateType aggregateType,
            long window,
            long start,
            long stop,
            string routerId = null)
        {
            var route = "";
            route = Url.Combine(route, "aggregate/window/object");
            route = Url.AddQueryParameter(route, "aggregateType", (TrakHound.TrakHoundAggregateType)aggregateType);
            route = Url.AddQueryParameter(route, "window", (long)window);
            route = Url.AddQueryParameter(route, "start", (long)start);
            route = Url.AddQueryParameter(route, "stop", (long)stop);
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectObservationEntity>());
            url = Url.Combine(url, route);

            var result = await RestRequest.Post<IEnumerable<TrakHoundHttpAggregateWindowResponse>>(url, objectUuids);

            return ProcessResponse(result);
        }

        public async Task<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectObservationEntity>>> SubscribeByObject(
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

            var url = TrakHoundHttp.GetEntityPath<ITrakHoundObjectObservationEntity>();
            url = Url.Combine(url, route);


            var consumer = new TrakHoundEntityListClientConsumer<ITrakHoundObjectObservationEntity>(BaseClient.ClientConfiguration, url, paths);
            consumer.Subscribe();
            return consumer;
        }

        public async Task<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectObservationEntity>>> SubscribeByObjectUuid(
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

            var url = TrakHoundHttp.GetEntityPath<ITrakHoundObjectObservationEntity>();
            url = Url.Combine(url, route);


            var consumer = new TrakHoundEntityListClientConsumer<ITrakHoundObjectObservationEntity>(BaseClient.ClientConfiguration, url, objectUuids);
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

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectObservationEntity>());
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

            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectObservationEntity>());
            url = Url.Combine(url, route);
            return (await RestRequest.PostResponse(url, objectUuids)).StatusCode == 200;
        }

    }
}
