// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrakHound.Entities;
using TrakHound.Entities.Collections;
using TrakHound.Entities.QueryEngines;

namespace TrakHound.Clients
{
    public partial class TrakHoundInstanceSystemEntitiesClient : TrakHoundSystemEntitiesClientBase
    {
        private readonly TrakHoundInstanceClient _baseClient;
        private TrakHoundQueryEngine _queryEngine;


        public TrakHoundInstanceSystemEntitiesClient(TrakHoundInstanceClient baseClient)
        {
            _baseClient = baseClient;

            _objects = new TrakHoundInstanceSystemObjectClient(baseClient);
            _definitions = new TrakHoundInstanceSystemDefinitionClient(baseClient);
            _sources = new TrakHoundInstanceSystemSourceClient(baseClient);
        }


        public override async Task<TrakHoundQueryResponse> Query(string query, string routerId = null)
        {
            if (_queryEngine == null) _queryEngine = new TrakHoundQueryEngine(_baseClient.System.Entities);

            var results = await _queryEngine.ExecuteQuery(query);
            return new TrakHoundQueryResponse(results, !results.IsNullOrEmpty());
        }

        public override async Task<ITrakHoundConsumer<TrakHoundEntityCollection>> Subscribe(int interval = 100, int count = 1000, string routerId = null)
        {
            var entityConsumers = new List<ITrakHoundConsumer<IEnumerable<ITrakHoundEntity>>>();
            entityConsumers.Add(await GetConsumer<ITrakHoundObjectEntity>(routerId: routerId));
            entityConsumers.Add(await GetConsumer<ITrakHoundObjectAssignmentEntity>(routerId: routerId));
            entityConsumers.Add(await GetConsumer<ITrakHoundObjectBlobEntity>(routerId: routerId));
            entityConsumers.Add(await GetConsumer<ITrakHoundObjectBooleanEntity>(routerId: routerId));
            entityConsumers.Add(await GetConsumer<ITrakHoundObjectDurationEntity>(routerId: routerId));
            entityConsumers.Add(await GetConsumer<ITrakHoundObjectEventEntity>(routerId: routerId));
            //entityConsumers.Add(await GetConsumer<ITrakHoundObjectFeedEntity>(routerId: routerId));
            entityConsumers.Add(await GetConsumer<ITrakHoundObjectGroupEntity>(routerId: routerId));
            entityConsumers.Add(await GetConsumer<ITrakHoundObjectHashEntity>(routerId: routerId));
            entityConsumers.Add(await GetConsumer<ITrakHoundObjectLogEntity>(routerId: routerId));
            entityConsumers.Add(await GetConsumer<ITrakHoundObjectMetadataEntity>(routerId: routerId));
            entityConsumers.Add(await GetConsumer<ITrakHoundObjectNumberEntity>(routerId: routerId));
            entityConsumers.Add(await GetConsumer<ITrakHoundObjectObservationEntity>(routerId: routerId));
            entityConsumers.Add(await GetConsumer<ITrakHoundObjectQueueEntity>(routerId: routerId));
            entityConsumers.Add(await GetConsumer<ITrakHoundObjectReferenceEntity>(routerId: routerId));
            entityConsumers.Add(await GetConsumer<ITrakHoundObjectSetEntity>(routerId: routerId));
            entityConsumers.Add(await GetConsumer<ITrakHoundObjectStateEntity>(routerId: routerId));
            entityConsumers.Add(await GetConsumer<ITrakHoundObjectStatisticEntity>(routerId: routerId));
            entityConsumers.Add(await GetConsumer<ITrakHoundObjectStringEntity>(routerId: routerId));
            entityConsumers.Add(await GetConsumer<ITrakHoundObjectTimeRangeEntity>(routerId: routerId));
            entityConsumers.Add(await GetConsumer<ITrakHoundObjectTimestampEntity>(routerId: routerId));
            entityConsumers.Add(await GetConsumer<ITrakHoundObjectVocabularyEntity>(routerId: routerId));
            entityConsumers.Add(await GetConsumer<ITrakHoundObjectVocabularySetEntity>(routerId: routerId));

            var queueConsumer = new TrakHoundQueueConsumer<ITrakHoundEntity, TrakHoundEntityCollection>(entityConsumers, interval, count);
            queueConsumer.OnReceived = (o) =>
            {
                var collection = new TrakHoundEntityCollection();
                collection.Add(o, false);
                return collection;
            };

            return queueConsumer;
        }

        private async Task<ITrakHoundConsumer<IEnumerable<ITrakHoundEntity>>> GetConsumer<TEntity>(string routerId) where TEntity : ITrakHoundEntity
        {
            var entityConsumer = await _baseClient.System.Entities.GetEntityClient<TEntity>()?.Subscribe(routerId: routerId);
            var genericConsumer = new TrakHoundConsumer<IEnumerable<TEntity>, IEnumerable<ITrakHoundEntity>>(entityConsumer);
            genericConsumer.OnReceived = (o) =>
            {
                var n = o.Count();
                var i = 0;

                var entities = new ITrakHoundEntity[n];
                foreach (var x in o)
                {
                    entities[i] = x;
                    i++;
                }
                return entities;
            };

            return genericConsumer;
        }


        public override async Task<ITrakHoundConsumer<TrakHoundQueryResponse>> Subscribe(string query, int interval = 100, int count = 1000, string routerId = null)
        {
            if (!string.IsNullOrEmpty(query))
            {
                if (_queryEngine == null) _queryEngine = new TrakHoundQueryEngine(_baseClient.System.Entities);

                var subscriptions = TrakHoundQueryEngine.GetSubscriptions(query);

                var consumerId = Guid.NewGuid().ToString();
                ITrakHoundConsumer<IEnumerable<ITrakHoundEntity>> entityContentConsumer = null;

                var objectConsumer = await _baseClient.System.Entities.Objects.Notify(TrakHoundExpression.Convert(subscriptions), TrakHoundEntityNotificationType.All, routerId);
                objectConsumer.Received += async (s, notification) =>
                {
                    Console.WriteLine("Subscribe() : OBJECTS CHANGED!!!!");

                    //entityContentConsumer = await _baseClient.Entities.SubscribeToContent(subscriptions, consumerId);
                };

                entityContentConsumer = await _baseClient.System.Entities.CreateSubscriptions(subscriptions, consumerId, routerId);

                var queueConsumer = new TrakHoundQueueConsumer<ITrakHoundEntity>(entityContentConsumer, consumerId, interval, count);

                var matchConsumer = new TrakHoundConsumer<IEnumerable<ITrakHoundEntity>, TrakHoundQueryResponse>(queueConsumer, consumerId);
                matchConsumer.OnReceivedAsync = async (items) =>
                {
                    var matchCollection = new TrakHoundEntityCollection();
                    matchCollection.Add(items);
                    matchCollection.Add(await _queryEngine.GetEntityModel(items));

                    return new TrakHoundQueryResponse(_queryEngine.ExecuteQuery(query, matchCollection), true);
                };

                return matchConsumer;
            }

            return null;
        }

        public override async Task<ITrakHoundConsumer<TrakHoundEntityCollection>> Subscribe(IEnumerable<TrakHoundEntitySubscriptionRequest> requests, int interval = 100, int count = 1000, string routerId = null)
        {
            if (!requests.IsNullOrEmpty())
            {
                if (_queryEngine == null) _queryEngine = new TrakHoundQueryEngine(_baseClient.System.Entities);

                var consumerId = Guid.NewGuid().ToString();

                var entityContentConsumer = await _baseClient.System.Entities.CreateSubscriptions(requests, consumerId);
                var queueConsumer = new TrakHoundQueueConsumer<ITrakHoundEntity>(entityContentConsumer, consumerId, interval, count);

                var matchConsumer = new TrakHoundConsumer<IEnumerable<ITrakHoundEntity>, TrakHoundEntityCollection>(queueConsumer, consumerId);
                matchConsumer.OnReceived = (entities) =>
                {
                    var collection = new TrakHoundEntityCollection();
                    collection.Add(entities);
                    return collection;
                };

                return matchConsumer;
            }

            return null;
        }
    }
}
