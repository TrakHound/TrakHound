// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrakHound.Entities;
using TrakHound.Entities.Collections;
using TrakHound.Entities.QueryEngines;
using TrakHound.Http;

namespace TrakHound.Clients
{
    internal class TrakHoundHttpSystemEntitiesClient : TrakHoundSystemEntitiesClientBase
    {
        private readonly TrakHoundHttpClient _baseClient;
        private readonly string _baseUrl;
        private readonly string _routerId;


        public TrakHoundHttpSystemEntitiesClient(TrakHoundHttpClient baseClient)
        {
            _baseClient = baseClient;

            _objects = new TrakHoundHttpSystemObjectClient(baseClient, this);
            _definitions = new TrakHoundHttpSystemDefinitionClient(baseClient, this);
            _sources = new TrakHoundHttpSystemSourceClient(baseClient, this);
        }


        public override async Task<TrakHoundQueryResponse> Query(string query, string routerId = null)
        {
            var url = Url.Combine(_baseClient.HttpBaseUrl, HttpConstants.EntitiesPrefix);
            url = Url.AddQueryParameter(url, "routerId", _baseClient.GetRouterId(routerId));

            var response = await RestRequest.Post<TrakHoundQueryJsonResponse>(url, query);
            return response != null ? response.ToResponse() : new TrakHoundQueryResponse();
        }

        public override async Task<ITrakHoundConsumer<TrakHoundEntityCollection>> Subscribe(int interval = 100, int count = 1000, string routerId = null)
        {
            var url = HttpConstants.EntitiesPrefix;
            url = Url.Combine(url, "subscribe");
            //url = Url.AddQueryParameter(url, "interval", interval);
            //url = Url.AddQueryParameter(url, "count", count);
            //if (!string.IsNullOrEmpty(consumerId)) url = Url.AddQueryParameter(url, "consumerId", consumerId);
            url = Url.AddQueryParameter(url, "routerId", _baseClient.GetRouterId(routerId));

            System.Console.WriteLine($"TrakHoundSystemEntitiesClient.Subscribe() = {url}");

            var consumer = new TrakHoundEntityCollectionClientConsumer(_baseClient.ClientConfiguration, url);
            consumer.Subscribe();
            return consumer;
        }

        public override async Task<ITrakHoundConsumer<TrakHoundQueryResponse>> Subscribe(string query, int interval = 100, int count = 1000, string routerId = null)
        {
            var url = HttpConstants.EntitiesPrefix;
            url = Url.Combine(url, "subscribe/query");
            url = Url.AddQueryParameter(url, "q", query);
            //url = Url.AddQueryParameter(url, "interval", interval);
            //url = Url.AddQueryParameter(url, "count", count);
            //if (!string.IsNullOrEmpty(consumerId)) url = Url.AddQueryParameter(url, "consumerId", consumerId);
            url = Url.AddQueryParameter(url, "routerId", _baseClient.GetRouterId(routerId));

            System.Console.WriteLine($"TrakHoundSystemEntitiesClient.Subscribe() = {url}");

            var consumer = new TrakHoundEntityQueryClientConsumer(_baseClient.ClientConfiguration, url);
            //var consumer = new TrakHoundEntityQueryClientConsumer(_baseClient.ClientConfiguration, url, query);
            consumer.Subscribe();
            return consumer;
        }

        public override async Task<ITrakHoundConsumer<TrakHoundEntityCollection>> Subscribe(IEnumerable<TrakHoundEntitySubscriptionRequest> requests, int interval = 100, int count = 1000, string routerId = null)
        {
            var url = HttpConstants.EntitiesPrefix;
            url = Url.Combine(url, "subscribe/query");
            url = Url.Combine(url, "requests");
            url = Url.AddQueryParameter(url, "interval", interval);
            url = Url.AddQueryParameter(url, "count", count);
            //if (!string.IsNullOrEmpty(consumerId)) url = Url.AddQueryParameter(url, "consumerId", consumerId);
            url = Url.AddQueryParameter(url, "routerId", _baseClient.GetRouterId(routerId));

            var httpRequests = TrakHoundHttpEntitySubscriptionRequest.Create(requests);
            var consumer = new TrakHoundEntityCollectionClientConsumer(_baseClient.ClientConfiguration, url, httpRequests);
            consumer.Subscribe();
            return consumer;
        }

        public override async Task<bool> Publish(ITrakHoundEntity entity, TrakHoundOperationMode mode = TrakHoundOperationMode.Async, string routerId = null)
        {
            if (entity != null)
            {
                var entities = new List<ITrakHoundEntity> { entity };
                return await Publish(entities, mode, routerId);
            }

            return false;
        }

        public override async Task<bool> Publish(IEnumerable<ITrakHoundEntity> entities, TrakHoundOperationMode mode = TrakHoundOperationMode.Async, string routerId = null)
        {
            if (!entities.IsNullOrEmpty())
            {
                var publishEntities = new List<ITrakHoundEntity>();
                foreach (var entity in entities)
                {
                    var publishEntity = Process(entity, mode);
                    if (publishEntity != null) publishEntities.Add(publishEntity);
                }

                //var publishEntities = Process(entities, mode);
                if (!publishEntities.IsNullOrEmpty())
                {
                    await OnBeforePublish(publishEntities, mode);

                    var url = Url.Combine(_baseClient.HttpBaseUrl, HttpConstants.EntitiesPrefix);
                    url = Url.Combine(url, "publish");
                    url = Url.AddQueryParameter(url, "routerId", _baseClient.GetRouterId(routerId));
                    if (mode == TrakHoundOperationMode.Sync) url = Url.AddQueryParameter(url, "async", false);

                    var sendEntities = new List<ITrakHoundEntity>();
                    var skip = 0;
                    var take = 1000;
                    var n = publishEntities.Count();

                    while (skip < n)
                    {
                        var collection = new TrakHoundEntityCollection();
                        collection.Add(publishEntities.Skip(skip).Take(take));

                        var httpCollection = new TrakHoundJsonEntityCollection(collection);

                        await RestRequest.Post(url, httpCollection);

                        skip += take;
                    }

                    await OnAfterPublish(publishEntities, mode);

                    return true;
                }
            }

            return false;
        }
    }
}
