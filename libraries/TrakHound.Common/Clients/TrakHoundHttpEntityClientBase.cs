// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using TrakHound.Entities;
using TrakHound.Http;

namespace TrakHound.Clients
{
    internal abstract class TrakHoundHttpEntityClientBase<TEntity> : ITrakHoundEntityClient<TEntity> where TEntity : ITrakHoundEntity
    {
        public const long DefaultSkip = 0;
        public const long DefaultTake = 1000;
        public const SortOrder DefaultSortOrder = SortOrder.Ascending;


        private readonly TrakHoundHttpClient _baseClient;
        private readonly TrakHoundHttpSystemEntitiesClient _entitiesClient;


        public TrakHoundHttpClient BaseClient => _baseClient;

        public TrakHoundHttpSystemEntitiesClient EntitiesClient => _entitiesClient;

        public string BaseUrl => _baseClient.ClientConfiguration.GetHttpBaseUrl();

        public string RouterId => _baseClient.RouterId;


        public TrakHoundHttpEntityClientBase(TrakHoundHttpClient baseClient, TrakHoundHttpSystemEntitiesClient entitiesClient)
        {
            _baseClient = baseClient;
            _entitiesClient = entitiesClient;
        }


        protected TEntity ProcessResponse(object[] array)
        {
            return TrakHoundEntity.FromArray<TEntity>(array);
        }

        protected IEnumerable<TEntity> ProcessResponse(object[][] arrays)
        {
            if (!arrays.IsNullOrEmpty())
            {
                var entities = new List<TEntity>();
                foreach (var array in arrays)
                {
                    var entity = TrakHoundEntity.FromArray<TEntity>(array);
                    if (entity.IsValid) entities.Add(entity);
                }
                return entities;
            }

            return null;
        }

        protected TrakHoundCount ProcessResponse(TrakHoundHttpCountResponse response)
        {
            return response.ToCount();
        }

        protected TrakHoundAggregate ProcessResponse(TrakHoundHttpAggregateResponse response)
        {
            return response.ToAggregate();
        }

        protected TrakHoundAggregateWindow ProcessResponse(TrakHoundHttpAggregateWindowResponse response)
        {
            return response.ToAggregateWindow();
        }

        protected IEnumerable<TrakHoundCount> ProcessResponse(IEnumerable<TrakHoundHttpCountResponse> responses)
        {
            if (!responses.IsNullOrEmpty())
            {
                var results = new List<TrakHoundCount>();
                foreach (var response in responses)
                {
                    results.Add(response.ToCount());
                }
                return results;
            }

            return null;
        }

        protected IEnumerable<TrakHoundAggregate> ProcessResponse(IEnumerable<TrakHoundHttpAggregateResponse> responses)
        {
            if (!responses.IsNullOrEmpty())
            {
                var results = new List<TrakHoundAggregate>();
                foreach (var response in responses)
                {
                    results.Add(response.ToAggregate());
                }
                return results;
            }

            return null;
        }

        protected IEnumerable<TrakHoundAggregateWindow> ProcessResponse(IEnumerable<TrakHoundHttpAggregateWindowResponse> responses)
        {
            if (!responses.IsNullOrEmpty())
            {
                var results = new List<TrakHoundAggregateWindow>();
                foreach (var response in responses)
                {
                    results.Add(response.ToAggregateWindow());
                }
                return results;
            }

            return null;
        }

        protected TrakHoundTimeRangeSpan ProcessResponse(TrakHoundHttpTimeRangeSpanResponse response)
        {
            return response.ToTimeRangeSpan();
        }

        protected IEnumerable<TrakHoundTimeRangeSpan> ProcessResponse(IEnumerable<TrakHoundHttpTimeRangeSpanResponse> responses)
        {
            if (!responses.IsNullOrEmpty())
            {
                var results = new List<TrakHoundTimeRangeSpan>();
                foreach (var response in responses)
                {
                    results.Add(response.ToTimeRangeSpan());
                }
                return results;
            }

            return null;
        }


        #region "Read"

        public async Task<TEntity> ReadByUuid(string uuid, string routerId = null)
        {
            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<TEntity>());
            url = Url.Combine(url, HttpUtility.UrlEncode(uuid));
            url = Url.AddQueryParameter(url, "routerId", BaseClient.GetRouterId(routerId));

            var array = await RestRequest.Get<object[]>(url);
            return ProcessResponse(array);
        }

        public async Task<IEnumerable<TEntity>> ReadByUuid(IEnumerable<string> uuids, string routerId = null)
        {
            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<TEntity>());
            url = Url.AddQueryParameter(url, "routerId", BaseClient.GetRouterId(routerId));

            var arrays = await RestRequest.Post<object[][]>(url, uuids);
            return ProcessResponse(arrays);
        }

        #endregion

        #region "Scan"

        public async Task<IEnumerable<TEntity>> Scan(long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending)
        {
            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<TEntity>());
            url = Url.Combine(url, "scan");
            url = Url.AddQueryParameter(url, "skip", skip);
            url = Url.AddQueryParameter(url, "take", take);
            url = Url.AddQueryParameter(url, "order", (int)sortOrder);
            if (!string.IsNullOrEmpty(RouterId)) url = Url.AddQueryParameter(url, "routerId", RouterId);

            var arrays = await RestRequest.Get<object[][]>(url);
            return ProcessResponse(arrays);
        }

        #endregion

        #region "Subscribe"

        public async Task<ITrakHoundConsumer<IEnumerable<TEntity>>> Subscribe(int interval = 0, int take = 1000, string consumerId = null, string routerId = null)
        {
            var url = TrakHoundHttp.GetEntityPath<TEntity>();
            url = Url.Combine(url, "subscribe");
            if (interval > 0) url = Url.AddQueryParameter(url, "interval", interval);
            url = Url.AddQueryParameter(url, "take", take);
            if (!string.IsNullOrEmpty(consumerId)) url = Url.AddQueryParameter(url, "consumerId", consumerId);
            url = Url.AddQueryParameter(url, "routerId", BaseClient.GetRouterId(routerId));

            var consumer = new TrakHoundEntityListClientConsumer<TEntity>(_baseClient.ClientConfiguration, url);
            consumer.Subscribe();
            return consumer;
        }

        #endregion

        #region "Publish"

        protected virtual void OnPublishBefore(TEntity entity) { }

        protected virtual void OnPublishBefore(IEnumerable<TEntity> entities) { }

        protected virtual void OnPublishAfter(TEntity entity) { }

        protected virtual void OnPublishAfter(IEnumerable<TEntity> entities) { }


        public virtual async Task<bool> Publish(TEntity entity, TrakHoundOperationMode mode = TrakHoundOperationMode.Async, string routerId = null)
        {
            if (entity != null)
            {
                var publishEntity = (TEntity)_entitiesClient.Process(entity, mode);
                if (publishEntity != null)
                {
                    OnPublishBefore(publishEntity);
                    await _entitiesClient.OnBeforePublish(new ITrakHoundEntity[] { publishEntity }, mode);

                    var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<TEntity>());
                    url = Url.Combine(url, "publish");
                    url = Url.AddQueryParameter(url, "routerId", BaseClient.GetRouterId(routerId));
                    if (mode == TrakHoundOperationMode.Sync) url = Url.AddQueryParameter(url, "async", false);

                    var arrays = new List<object[]>();
                    arrays.Add(TrakHoundEntity.ToArray(publishEntity));

                    await RestRequest.Post(url, arrays);

                    OnPublishAfter(publishEntity);
                    await _entitiesClient.OnAfterPublish(new ITrakHoundEntity[] { publishEntity }, mode);

                    return true;
                }
            }

            return false;
        }

        public virtual async Task<bool> Publish(IEnumerable<TEntity> entities, TrakHoundOperationMode mode = TrakHoundOperationMode.Async, string routerId = null)
        {
            if (!entities.IsNullOrEmpty())
            {
                var publishEntities = new List<TEntity>();
                foreach (var entity in entities)
                {
                    var publishEntity = (TEntity)_entitiesClient.Process(entity, mode);
                    if (publishEntity != null) publishEntities.Add(publishEntity);
                }

                //var publishEntities = _entitiesClient.Process(entities, mode);
                if (!publishEntities.IsNullOrEmpty())
                {
                    var sendEntities = new List<ITrakHoundEntity>();
                    foreach (var entity in publishEntities) sendEntities.Add(entity);

                    OnPublishBefore(publishEntities);
                    await _entitiesClient.OnBeforePublish(sendEntities, mode);

                    var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<TEntity>());
                    url = Url.Combine(url, "publish");
                    url = Url.AddQueryParameter(url, "routerId", BaseClient.GetRouterId(routerId));
                    if (mode == TrakHoundOperationMode.Sync) url = Url.AddQueryParameter(url, "async", false);

                    var arrays = new List<object[]>();
                    foreach (var entity in sendEntities)
                    {
                        arrays.Add(TrakHoundEntity.ToArray(entity));
                    }

                    await RestRequest.Post(url, arrays);

                    OnPublishAfter(publishEntities);
                    await _entitiesClient.OnAfterPublish(sendEntities, mode);

                    return true;
                }
            }

            return false;
        }

        #endregion

        #region "Delete"

        public async Task<bool> Delete(string uuid, TrakHoundOperationMode mode = TrakHoundOperationMode.Async, string routerId = null)
        {
            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<TEntity>());
            url = Url.Combine(url, uuid);
            url = Url.AddQueryParameter(url, "routerId", BaseClient.GetRouterId(routerId));
            if (mode == TrakHoundOperationMode.Sync) url = Url.AddQueryParameter(url, "async", false);

            return await RestRequest.Delete(url);
        }


        public async Task<bool> Delete(IEnumerable<string> uuids, TrakHoundOperationMode mode = TrakHoundOperationMode.Async, string routerId = null)
        {
            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<TEntity>());
            url = Url.Combine(url, "delete");
            url = Url.AddQueryParameter(url, "routerId", BaseClient.GetRouterId(routerId));
            if (mode == TrakHoundOperationMode.Sync) url = Url.AddQueryParameter(url, "async", false);

            var response = await RestRequest.PostResponse(url, uuids);
            return response.StatusCode == 200 || response.StatusCode == 202;
        }

        #endregion

        #region "Expire"

        public async Task<EntityDeleteResult> Expire(EntityDeleteRequest request, string routerId = null)
        {
            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<TEntity>());
            url = Url.Combine(url, "expire");
            url = Url.AddQueryParameter(url, "routerId", BaseClient.GetRouterId(routerId));

            var results = await RestRequest.Post<IEnumerable<TrakHoundHttpDeleteResult>>(url, TrakHoundHttpDeleteRequest.Create(request));
            if (!results.IsNullOrEmpty())
            {
                return results.ToResults().FirstOrDefault();
            }

            return default;
        }

        public async Task<IEnumerable<EntityDeleteResult>> Expire(IEnumerable<EntityDeleteRequest> requests, string routerId = null)
        {
            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<TEntity>());
            url = Url.Combine(url, "expire");
            url = Url.AddQueryParameter(url, "routerId", BaseClient.GetRouterId(routerId));

            var results = await RestRequest.Post<IEnumerable<TrakHoundHttpDeleteResult>>(url, TrakHoundHttpDeleteRequest.Create(requests));
            if (!results.IsNullOrEmpty())
            {
                return results.ToResults();
            }

            return null;
        }


        public async Task<EntityDeleteResult> ExpireByAccess(EntityDeleteRequest request, string routerId = null)
        {
            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<TEntity>());
            url = Url.Combine(url, "expire");
            url = Url.Combine(url, "access");
            url = Url.AddQueryParameter(url, "routerId", BaseClient.GetRouterId(routerId));

            var results = await RestRequest.Post<IEnumerable<TrakHoundHttpDeleteResult>>(url, TrakHoundHttpDeleteRequest.Create(request));
            if (!results.IsNullOrEmpty())
            {
                return results.ToResults().FirstOrDefault();
            }

            return default;
        }

        public async Task<IEnumerable<EntityDeleteResult>> ExpireByAccess(IEnumerable<EntityDeleteRequest> requests, string routerId = null)
        {
            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<TEntity>());
            url = Url.Combine(url, "expire");
            url = Url.Combine(url, "access");
            url = Url.AddQueryParameter(url, "routerId", BaseClient.GetRouterId(routerId));

            var results = await RestRequest.Post<IEnumerable<TrakHoundHttpDeleteResult>>(url, TrakHoundHttpDeleteRequest.Create(requests));
            if (!results.IsNullOrEmpty())
            {
                return results.ToResults();
            }

            return null;
        }


        public async Task<EntityDeleteResult> ExpireByUpdate(EntityDeleteRequest request, string routerId = null)
        {
            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<TEntity>());
            url = Url.Combine(url, "expire");
            url = Url.Combine(url, "update");
            url = Url.AddQueryParameter(url, "routerId", BaseClient.GetRouterId(routerId));

            var results = await RestRequest.Post<IEnumerable<TrakHoundHttpDeleteResult>>(url, TrakHoundHttpDeleteRequest.Create(request));
            if (!results.IsNullOrEmpty())
            {
                return results.ToResults().FirstOrDefault();
            }

            return default;
        }

        public async Task<IEnumerable<EntityDeleteResult>> ExpireByUpdate(IEnumerable<EntityDeleteRequest> requests, string routerId = null)
        {
            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<TEntity>());
            url = Url.Combine(url, "expire");
            url = Url.Combine(url, "update");
            url = Url.AddQueryParameter(url, "routerId", BaseClient.GetRouterId(routerId));

            var results = await RestRequest.Post<IEnumerable<TrakHoundHttpDeleteResult>>(url, TrakHoundHttpDeleteRequest.Create(requests));
            if (!results.IsNullOrEmpty())
            {
                return results.ToResults();
            }

            return null;
        }

        #endregion

    }
}
