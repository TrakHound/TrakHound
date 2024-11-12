// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Entities;
using TrakHound.Entities.Collections;
using TrakHound.Entities.QueryEngines;

namespace TrakHound.Clients.Collections
{
    public class TrakHoundCollectionEntitiesClient : TrakHoundSystemEntitiesClientBase
    {
        private readonly TrakHoundEntityCollection _collection;
        private readonly TrakHoundQueryEngine _queryEngine;


        public TrakHoundCollectionEntitiesClient(TrakHoundEntityCollection collection)
        {
            _collection = collection;
            _objects = new TrakHoundCollectionObjectClient(collection);
            _definitions = new TrakHoundCollectionDefinitionClient(collection);
            _sources = new TrakHoundCollectionSourceClient(collection);
            _queryEngine = new TrakHoundQueryEngine(this);
        }


        public override async Task<TrakHoundQueryResponse> Query(string query, string routerId = null)
        {
            var results = await _queryEngine.ExecuteQuery(query);
            if (!results.IsNullOrEmpty())
            {
                return new TrakHoundQueryResponse(results, true);
            }

            return new TrakHoundQueryResponse();
        }

        public override async Task<ITrakHoundConsumer<IEnumerable<TrakHoundEntityNotification>>> Notify(string query, TrakHoundEntityNotificationType notificationType = TrakHoundEntityNotificationType.All, string routerId = null)
        {
            return null;
        }


        public override async Task<bool> Publish(ITrakHoundEntity entity, TrakHoundOperationMode mode = TrakHoundOperationMode.Async, string routerId = null)
        {
            if (entity != null)
            {
                var publishEntity = Process(entity, mode);
                if (publishEntity != null)
                {
                    await OnBeforePublish(new ITrakHoundEntity[] { entity }, mode);
                    _collection.Add(entity);
                    await OnAfterPublish(new ITrakHoundEntity[] { entity }, mode);
                }

                return true;
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

                if (!publishEntities.IsNullOrEmpty())
                {
                    await OnBeforePublish(publishEntities, mode);
                    _collection.Add(publishEntities);
                    await OnAfterPublish(publishEntities, mode);

                    return true;
                }



                //var publishEntities = Process(entities, mode);
                //if (!publishEntities.IsNullOrEmpty())
                //{
                //    await OnBeforePublish(publishEntities, mode);
                //    _collection.Add(publishEntities);
                //    await OnAfterPublish(publishEntities, mode);

                //    return true;
                //}
            }
            return false;
        }
    }
}
