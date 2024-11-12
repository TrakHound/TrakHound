// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Drivers;
using TrakHound.Drivers.Entities;
using TrakHound.Entities;

namespace TrakHound.Http
{
    public abstract class HttpEntityDriver<TEntity> :
        HttpDriver,
        IEntityReadDriver<TEntity>,
        IEntitySubscribeDriver<TEntity>,
        IEntityPublishDriver<TEntity>,
        IEntityDeleteDriver<TEntity>,
        IEntityExpirationUpdateDriver<TEntity>,
        IEntityExpirationAccessDriver<TEntity>
        where TEntity : ITrakHoundEntity
    {
        protected readonly Dictionary<string, ITrakHoundConsumer<IEnumerable<TEntity>>> _consumers = new Dictionary<string, ITrakHoundConsumer<IEnumerable<TEntity>>>();
        protected readonly object _lock = new object();


        public HttpEntityDriver() : base() { }

        public HttpEntityDriver(ITrakHoundDriverConfiguration configuration) : base(configuration) { }


		protected override void OnDisposed()
		{
            try
            {
				lock (_lock)
				{
                    if (!_consumers.IsNullOrEmpty())
                    {
                        foreach (var consumer in _consumers) consumer.Value.Dispose();
                    }
				}
			}
            catch { }
		}


        #region "Read"

        public virtual async Task<TrakHoundResponse<TEntity>> Read(IEnumerable<string> uuids)
        {
            var stpw = System.Diagnostics.Stopwatch.StartNew();
            var results = new List<TrakHoundResult<TEntity>>();

            if (!uuids.IsNullOrEmpty())
            {
                var entities = await Client.System.Entities.GetEntityClient<TEntity>().ReadByUuid(uuids);
                if (!entities.IsNullOrEmpty())
                {
                    var dEntities = entities.ToDictionary(o => o.Uuid);

                    foreach (var uuid in uuids)
                    {
                        var entity = dEntities.GetValueOrDefault(uuid);
                        if (entity != null)
                        {
                            results.Add(new TrakHoundResult<TEntity>(Constants.TypeId, uuid, TrakHoundResultType.Ok, entity));
                        }
                        else
                        {
                            results.Add(new TrakHoundResult<TEntity>(Constants.TypeId, uuid, TrakHoundResultType.NotFound));
                        }
                    }
                }
                else
                {
                    foreach (var uuid in uuids)
                    {
                        results.Add(new TrakHoundResult<TEntity>(Constants.TypeId, uuid, TrakHoundResultType.NotFound));
                    }
                }
            }
            else
            {
                results.Add(new TrakHoundResult<TEntity>(Constants.TypeId, null, TrakHoundResultType.BadRequest));
            }

            stpw.Stop();
            return new TrakHoundResponse<TEntity>(results, stpw.ElapsedTicks);
        }

        #endregion

        #region "Subscribe"

        public async Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<TEntity>>>> Subscribe()
        {
            var consumer = await Client.System.Entities.GetEntityClient<TEntity>().Subscribe();
            consumer.Disposed += ConsumerDisposed;
            lock (_lock) _consumers.Add(consumer.Id, consumer);

            var results = new List<TrakHoundResult<ITrakHoundConsumer<IEnumerable<TEntity>>>>();
            results.Add(new TrakHoundResult<ITrakHoundConsumer<IEnumerable<TEntity>>>(Id, "All", TrakHoundResultType.Ok, consumer));

            return new TrakHoundResponse<ITrakHoundConsumer<IEnumerable<TEntity>>>(results);
        }

        protected void ConsumerDisposed(object sender, string consumerId)
        {
            if (consumerId != null)
            {
                lock (_lock) _consumers.Remove(consumerId);
            }
        }

        private static IEnumerable<TEntity> ProcessMessage(IEnumerable<string> keys, IEnumerable<TEntity> entities)
        {
            return entities;
        }

        #endregion

        #region "Publish"

        protected virtual void OnPublishBefore(IEnumerable<TEntity> entities) { }

        protected virtual void OnPublishAfter(IEnumerable<TEntity> entities) { }

		public virtual async Task<TrakHoundResponse<TrakHoundPublishResult<TEntity>>> Publish(IEnumerable<TEntity> entities)
        {
            if (!entities.IsNullOrEmpty())
            {
                var stpw = System.Diagnostics.Stopwatch.StartNew();
                var results = new List<TrakHoundResult<TrakHoundPublishResult<TEntity>>>();

                OnPublishBefore(entities);

                if (await Client.System.Entities.GetEntityClient<TEntity>().Publish(entities))
                {
                    foreach (var entity in entities)
                    {
                        var publishResult = new TrakHoundPublishResult<TEntity>(TrakHoundPublishResultType.Created, entity);
                        results.Add(new TrakHoundResult<TrakHoundPublishResult<TEntity>>(Id, entity.Uuid, TrakHoundResultType.Ok, publishResult));
                    }
                }
                else
                {
                    foreach (var entity in entities)
                    {
                        results.Add(new TrakHoundResult<TrakHoundPublishResult<TEntity>>(Id, entity.Uuid, TrakHoundResultType.InternalError));
                    }
                }

                OnPublishAfter(entities);

                stpw.Stop();
                return new TrakHoundResponse<TrakHoundPublishResult<TEntity>>(results, stpw.ElapsedTicks);
            }

            return TrakHoundResponse<TrakHoundPublishResult<TEntity>>.InternalError(Id, null);
        }

        #endregion

        #region "Delete"

        protected virtual bool OnDelete(EntityDeleteRequest request)
        {
            if (!string.IsNullOrEmpty(request.Target))
            {
                //lock (_lock)
                //{
                //    _entities.Remove(request.Target);
                //    _updated.Remove(request.Target);
                //    _accessed.Remove(request.Target);
                //}
            }

            return true;
        }

        protected virtual void OnDeleteBefore(IEnumerable<EntityDeleteRequest> requests) { }

        protected virtual void OnDeleteAfter(IEnumerable<EntityDeleteRequest> requests) { }

        public async Task<TrakHoundResponse<bool>> Delete(IEnumerable<EntityDeleteRequest> requests)
        {
            var stpw = System.Diagnostics.Stopwatch.StartNew();
            var results = new List<TrakHoundResult<bool>>();

            if (!requests.IsNullOrEmpty())
            {
                OnDeleteBefore(requests);

                foreach (var request in requests)
                {
                    if (OnDelete(request))
                    {
                        results.Add(new TrakHoundResult<bool>(Constants.TypeId, request.Target, TrakHoundResultType.Ok, true));
                    }
                    else
                    {
                        results.Add(new TrakHoundResult<bool>(Constants.TypeId, request.Target, TrakHoundResultType.InternalError, false));
                    }
                }

                OnDeleteAfter(requests);
            }

            stpw.Stop();
            return new TrakHoundResponse<bool>(results, stpw.ElapsedTicks);
        }

        #endregion

        #region "Expire"

        public async Task<TrakHoundResponse<EntityDeleteResult>> ExpireByUpdate(IEnumerable<EntityDeleteRequest> requests)
        {
            var stpw = System.Diagnostics.Stopwatch.StartNew();
            var results = new List<TrakHoundResult<EntityDeleteResult>>();

            //if (!requests.IsNullOrEmpty())
            //{
            //    foreach (var request in requests)
            //    {
            //        lock (_lock)
            //        {
            //            if (_updated.TryGetValue(request.Target, out var accessed))
            //            {
            //                if (accessed <= request.Timestamp)
            //                {
            //                    _entities.Remove(request.Target);
            //                    _updated.Remove(request.Target);
            //                    _accessed.Remove(request.Target);

            //                    var result = new EntityDeleteResult(request.Target, 1);
            //                    results.Add(new TrakHoundResult<EntityDeleteResult>(Constants.TypeId, request.Target, TrakHoundResultType.Ok, result));
            //                }
            //            }
            //        }
            //    }
            //}

            stpw.Stop();
            return new TrakHoundResponse<EntityDeleteResult>(results, stpw.ElapsedTicks);
        }

        public async Task<TrakHoundResponse<EntityDeleteResult>> ExpireByUpdate(long timestamp)
        {
            if (timestamp > 0)
            {
                var stpw = System.Diagnostics.Stopwatch.StartNew();
                var results = new List<TrakHoundResult<EntityDeleteResult>>();

                var removeKeys = new List<string>();

                //lock (_lock)
                //{
                //    foreach (var acceesed in _updated)
                //    {
                //        if (acceesed.Value <= timestamp) removeKeys.Add(acceesed.Key);
                //    }
                //}

                //foreach (var removeKey in removeKeys)
                //{
                //    lock (_lock)
                //    {
                //        _entities.Remove(removeKey);
                //        _updated.Remove(removeKey);
                //        _accessed.Remove(removeKey);
                //    }

                //    var result = new EntityDeleteResult(removeKey, 1);
                //    results.Add(new TrakHoundResult<EntityDeleteResult>(Constants.TypeId, removeKey, TrakHoundResultType.Ok, result));
                //}

                stpw.Stop();
                return new TrakHoundResponse<EntityDeleteResult>(results, stpw.ElapsedTicks);
            }

            return TrakHoundResponse<EntityDeleteResult>.InternalError(Constants.TypeId, null);
        }


        //public async Task<TrakHoundResponse<EntityDeleteResult>> ExpireByUpdate(long lastUpdated)
        //{
        //    if (lastUpdated > 0)
        //    {
        //        var stpw = System.Diagnostics.Stopwatch.StartNew();
        //        var results = new List<ITrakHoundResult<EntityDeleteResult>>();

        //        if (!entityIds.IsNullOrEmpty())
        //        {
        //            foreach (var entityId in entityIds)
        //            {
        //                lock (_lock)
        //                {
        //                    if (_updated.TryGetValue(entityId, out var updated))
        //                    {
        //                        if (updated <= lastUpdated)
        //                        {
        //                            _entities.Remove(entityId);
        //                            _updated.Remove(entityId);
        //                            _accessed.Remove(entityId);

        //                            results.Add(new TrakHoundResult<EntityDeleteResult>(Constants.TypeId, entityId, TrakHoundResultType.Ok, 1));
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        else
        //        {
        //            var removeKeys = new List<string>();

        //            lock (_lock)
        //            {
        //                foreach (var updated in _updated)
        //                {
        //                    if (updated.Value <= lastUpdated) removeKeys.Add(updated.Key);
        //                }
        //            }

        //            foreach (var removeKey in removeKeys)
        //            {
        //                lock (_lock)
        //                {
        //                    _entities.Remove(removeKey);
        //                    _updated.Remove(removeKey);
        //                    _accessed.Remove(removeKey);
        //                }

        //                results.Add(new TrakHoundResult<EntityDeleteResult>(Constants.TypeId, null, TrakHoundResultType.Ok, 1));
        //            }
        //        }

        //        stpw.Stop();
        //        return new TrakHoundResponse<EntityDeleteResult>(results, stpw.ElapsedTicks);
        //    }

        //    return TrakHoundResponse<EntityDeleteResult>.InternalError(Constants.TypeId, null);
        //}


        public async Task<TrakHoundResponse<EntityDeleteResult>> ExpireByAccess(IEnumerable<EntityDeleteRequest> requests)
        {
            var stpw = System.Diagnostics.Stopwatch.StartNew();
            var results = new List<TrakHoundResult<EntityDeleteResult>>();

            if (!requests.IsNullOrEmpty())
            {
                foreach (var request in requests)
                {
                    //lock (_lock)
                    //{
                    //    if (_accessed.TryGetValue(request.Target, out var accessed))
                    //    {
                    //        if (accessed <= request.Timestamp)
                    //        {
                    //            _entities.Remove(request.Target);
                    //            _updated.Remove(request.Target);
                    //            _accessed.Remove(request.Target);

                    //            var result = new EntityDeleteResult(request.Target, 1);
                    //            results.Add(new TrakHoundResult<EntityDeleteResult>(Constants.TypeId, request.Target, TrakHoundResultType.Ok, result));
                    //        }
                    //    }
                    //}
                }
            }

            stpw.Stop();
            return new TrakHoundResponse<EntityDeleteResult>(results, stpw.ElapsedTicks);
        }

        public async Task<TrakHoundResponse<EntityDeleteResult>> ExpireByAccess(long timestamp)
        {
            if (timestamp > 0)
            {
                var stpw = System.Diagnostics.Stopwatch.StartNew();
                var results = new List<TrakHoundResult<EntityDeleteResult>>();

                //var removeKeys = new List<string>();

                //lock (_lock)
                //{
                //    foreach (var acceesed in _accessed)
                //    {
                //        if (acceesed.Value <= timestamp) removeKeys.Add(acceesed.Key);
                //    }
                //}

                //foreach (var removeKey in removeKeys)
                //{
                //    lock (_lock)
                //    {
                //        _entities.Remove(removeKey);
                //        _updated.Remove(removeKey);
                //        _accessed.Remove(removeKey);
                //    }

                //    var result = new EntityDeleteResult(removeKey, 1);
                //    results.Add(new TrakHoundResult<EntityDeleteResult>(Constants.TypeId, removeKey, TrakHoundResultType.Ok, result));
                //}

                stpw.Stop();
                return new TrakHoundResponse<EntityDeleteResult>(results, stpw.ElapsedTicks);
            }

            return TrakHoundResponse<EntityDeleteResult>.InternalError(Constants.TypeId, null);
        }

        #endregion

    }
}
