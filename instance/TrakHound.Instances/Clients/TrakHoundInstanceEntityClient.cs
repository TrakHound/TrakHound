// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrakHound.Entities;

namespace TrakHound.Clients
{
    public abstract class TrakHoundInstanceEntityClient<TEntity> where TEntity : ITrakHoundEntity
    {
        protected readonly TrakHoundInstanceClient _baseClient;


        protected TrakHoundInstanceClient BaseClient => _baseClient;


        public TrakHoundInstanceEntityClient(TrakHoundInstanceClient baseClient)
        {
            _baseClient = baseClient;
        }


        #region "Read"

        public async Task<TEntity> ReadByUuid(string uuid, string routerId = null)
        {
            if (!string.IsNullOrEmpty(uuid))
            {
                var router = _baseClient.GetRouter(routerId);
                if (router != null)
                {
                    var uuids = new string[] { uuid };
                    var response = await router.Entities.GetEntityRouter<TEntity>().Read(uuids);
                    if (response.IsSuccess)
                    {
                        return response.Content.FirstOrDefault();
                    }
                }
            }

            return default;
        }

        public async Task<IEnumerable<TEntity>> ReadByUuid(IEnumerable<string> uuids, string routerId = null)
        {
            if (!uuids.IsNullOrEmpty())
            {
                var router = _baseClient.GetRouter(routerId);
                if (router != null)
                {
                    return (await router.Entities.GetEntityRouter<TEntity>().Read(uuids)).Content.ToDistinct();
                }
            }

            return Enumerable.Empty<TEntity>();
        }

        #endregion

        #region "Subscribe"

        public async Task<ITrakHoundConsumer<IEnumerable<TEntity>>> Subscribe(int interval = 0, int take = 1000, string consumerId = null, string routerId = null)
        {
            var router = _baseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router.Entities.GetEntityRouter<TEntity>().Subscribe();
                if (response.IsSuccess)
                {
                    return new TrakHoundConsumer<IEnumerable<TEntity>>(response.Content);
                }
            }

            return null;
        }

        #endregion

        #region "Publish"

        protected virtual void OnPublishBefore(TEntity entity) { }

        protected virtual void OnPublishBefore(IEnumerable<TEntity> entities) { }

        protected virtual void OnPublishAfter(TEntity entity) { }

        protected virtual void OnPublishAfter(IEnumerable<TEntity> entities) { }


        public async Task<bool> Publish(TEntity entity, TrakHoundOperationMode mode = TrakHoundOperationMode.Async, string routerId = null)
        {
            if (entity != null)
            {
                var router = _baseClient.GetRouter(routerId);
                if (router != null)
                {
                    var publishEntity = (TEntity)(((TrakHoundInstanceSystemEntitiesClient)_baseClient.System.Entities).Process(entity, mode));
                    if (publishEntity != null)
                    {
                        OnPublishBefore(publishEntity);
                        await ((TrakHoundInstanceSystemEntitiesClient)_baseClient.System.Entities).OnBeforePublish(new ITrakHoundEntity[] { entity }, mode);

                        var entities = new TEntity[] { publishEntity };
                        var response = await router.Entities.GetEntityRouter<TEntity>().Publish(entities, mode);

                        OnPublishAfter(publishEntity);
                        await ((TrakHoundInstanceSystemEntitiesClient)_baseClient.System.Entities).OnAfterPublish(new ITrakHoundEntity[] { entity }, mode);

                        return response.IsSuccess;
                    }
                    else
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public async Task<bool> Publish(IEnumerable<TEntity> entities, TrakHoundOperationMode mode = TrakHoundOperationMode.Async, string routerId = null)
        {
            if (!entities.IsNullOrEmpty())
            {
                var router = _baseClient.GetRouter(routerId);
                if (router != null)
                {
                    var publishEntities = new List<TEntity>();
                    foreach (var entity in entities)
                    {
                        var publishEntity = (TEntity)(((TrakHoundInstanceSystemEntitiesClient)_baseClient.System.Entities).Process(entity, mode));
                        if (publishEntity != null) publishEntities.Add(publishEntity);
                    }

                    if (!publishEntities.IsNullOrEmpty())
                    {
                        var sendEntities = new List<ITrakHoundEntity>();
                        foreach (var entity in publishEntities) sendEntities.Add(entity);

                        OnPublishBefore(publishEntities);
                        await ((TrakHoundInstanceSystemEntitiesClient)_baseClient.System.Entities).OnBeforePublish(sendEntities, mode);

                        var response = await router.Entities.GetEntityRouter<TEntity>().Publish(publishEntities, mode);

                        OnPublishAfter(publishEntities);
                        await ((TrakHoundInstanceSystemEntitiesClient)_baseClient.System.Entities).OnAfterPublish(sendEntities, mode);

                        return response.IsSuccess;
                    }
                    else
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        #endregion

        #region "Delete"

        public async Task<bool> Delete(string entityId, TrakHoundOperationMode mode = TrakHoundOperationMode.Async, string routerId = null)
        {
            if (!string.IsNullOrEmpty(entityId))
            {
                var requests = EntityDeleteRequest.Create(entityId);

                var router = _baseClient.GetRouter(routerId);
                if (router != null)
                {
                    var response = await router.Entities.GetEntityRouter<TEntity>().Delete(requests, mode);
                    if (response.IsSuccess)
                    {
                        return response.Content.FirstOrDefault();
                    }
                }   
            }

            return false;
        }

        public async Task<bool> Delete(IEnumerable<string> entityIds, TrakHoundOperationMode mode = TrakHoundOperationMode.Async, string routerId = null)
        {
            if (!entityIds.IsNullOrEmpty())
            {
                var requests = EntityDeleteRequest.Create(entityIds);

                var router = _baseClient.GetRouter(routerId);
                if (router != null)
                {
                    var response = await router.Entities.GetEntityRouter<TEntity>().Delete(requests, mode);
                    return response.IsSuccess;
                }
            }

            return false;
        }

        #endregion

        #region "Expire"

        public async Task<EntityDeleteResult> Expire(EntityDeleteRequest request, string routerId = null)
        {
            if (!string.IsNullOrEmpty(request.Target) && request.Timestamp > 0)
            {
                var router = _baseClient.GetRouter(routerId);
                if (router != null)
                {
                    var requests = new EntityDeleteRequest[] { request };
                    var response = await router.Entities.GetEntityRouter<TEntity>().Expire(requests);
                    if (response.IsSuccess)
                    {
                        return response.Content.FirstOrDefault();
                    }
                }
            }

            return default;
        }

        public async Task<IEnumerable<EntityDeleteResult>> Expire(IEnumerable<EntityDeleteRequest> requests, string routerId = null)
        {
            if (!requests.IsNullOrEmpty())
            {
                var router = _baseClient.GetRouter(routerId);
                if (router != null)
                {
                    var response = await router.Entities.GetEntityRouter<TEntity>().Expire(requests);
                    if (response.IsSuccess)
                    {
                        return response.Content;
                    }
                }
            }

            return null;
        }

        public async Task<EntityDeleteResult> ExpireByAccess(EntityDeleteRequest request, string routerId = null)
        {
            if (!string.IsNullOrEmpty(request.Target) && request.Timestamp > 0)
            {
                var router = _baseClient.GetRouter(routerId);
                if (router != null)
                {
                    var requests = new EntityDeleteRequest[] { request };
                    var response = await router.Entities.GetEntityRouter<TEntity>().ExpireByAccess(requests);
                    if (response.IsSuccess)
                    {
                        return response.Content.FirstOrDefault();
                    }
                }
            }

            return default;
        }

        public async Task<IEnumerable<EntityDeleteResult>> ExpireByAccess(IEnumerable<EntityDeleteRequest> requests, string routerId = null)
        {
            if (!requests.IsNullOrEmpty())
            {
                var router = _baseClient.GetRouter(routerId);
                if (router != null)
                {
                    var response = await router.Entities.GetEntityRouter<TEntity>().ExpireByAccess(requests);
                    if (response.IsSuccess)
                    {
                        return response.Content;
                    }
                }
            }

            return null;
        }

        public async Task<EntityDeleteResult> ExpireByUpdate(EntityDeleteRequest request, string routerId = null)
        {
            if (!string.IsNullOrEmpty(request.Target) && request.Timestamp > 0)
            {
                var router = _baseClient.GetRouter(routerId);
                if (router != null)
                {
                    var requests = new EntityDeleteRequest[] { request };
                    var response = await router.Entities.GetEntityRouter<TEntity>().ExpireByUpdate(requests);
                    if (response.IsSuccess)
                    {
                        return response.Content.FirstOrDefault();
                    }
                }
            }

            return default;
        }

        public async Task<IEnumerable<EntityDeleteResult>> ExpireByUpdate(IEnumerable<EntityDeleteRequest> requests, string routerId = null)
        {
            if (!requests.IsNullOrEmpty())
            {
                var router = _baseClient.GetRouter(routerId);
                if (router != null)
                {
                    var response = await router.Entities.GetEntityRouter<TEntity>().ExpireByUpdate(requests);
                    if (response.IsSuccess)
                    {
                        return response.Content;
                    }
                }
            }

            return null;
        }

        #endregion

    }
}
