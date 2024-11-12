// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Entities;
using TrakHound.Entities.Collections;
using TrakHound.Entities.QueryEngines;

namespace TrakHound.Clients
{
    public interface ITrakHoundSystemEntitiesClient
    {
        IEnumerable<ITrakHoundEntitiesClientMiddleware> Middleware { get; }


        ITrakHoundSystemSourceClient Sources { get; }

        ITrakHoundSystemDefinitionClient Definitions { get; }

        ITrakHoundSystemObjectClient Objects { get; }



        Task<TrakHoundQueryResponse> Query(string query, string routerId = null);

        Task<ITrakHoundConsumer<TrakHoundEntityCollection>> Subscribe(int interval = 100, int count = 1000, string routerId = null);

        Task<ITrakHoundConsumer<TrakHoundQueryResponse>> Subscribe(string query, int interval = 100, int count = 1000, string routerId = null);

        Task<ITrakHoundConsumer<TrakHoundEntityCollection>> Subscribe(IEnumerable<TrakHoundEntitySubscriptionRequest> requests, int interval = 100, int count = 1000, string routerId = null);


        Task<ITrakHoundConsumer<IEnumerable<ITrakHoundEntity>>> CreateSubscriptions(IEnumerable<string> expressions, string consumerId = null, string routerId = null);

        Task<ITrakHoundConsumer<IEnumerable<ITrakHoundEntity>>> CreateSubscriptions(IEnumerable<TrakHoundEntitySubscriptionRequest> requests, string consumerId = null, string routerId = null);

        Task<ITrakHoundConsumer<IEnumerable<ITrakHoundEntity>>> SubscribeToContent(string expression, string consumerId = null, string routerId = null);

        Task<ITrakHoundConsumer<IEnumerable<ITrakHoundEntity>>> SubscribeToContent(IEnumerable<string> expressions, string consumerId = null, string routerId = null);


        Task<bool> Publish(ITrakHoundEntity entity, TrakHoundOperationMode mode = TrakHoundOperationMode.Async, string routerId = null);

        Task<bool> Publish(IEnumerable<ITrakHoundEntity> entities, TrakHoundOperationMode mode = TrakHoundOperationMode.Async, string routerId = null);


        ITrakHoundEntityClient<TEntity> GetEntityClient<TEntity>() where TEntity : ITrakHoundEntity;


        void AddMiddleware(ITrakHoundEntitiesClientMiddleware middleware);
    }
}
