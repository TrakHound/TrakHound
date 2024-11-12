// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Data;
using TrakHound.Drivers;
using TrakHound.Drivers.Entities;
using TrakHound.Entities;

namespace TrakHound.Memory
{
    public abstract class MemoryEntityDriver<TEntity> :
        MemoryDriver<TEntity>,
        IEntityReadDriver<TEntity>,
        IEntitySubscribeDriver<TEntity>,
        IEntityPublishDriver<TEntity>,
        IEntityDeleteDriver<TEntity>,
        IEntityExpirationUpdateDriver<TEntity>,
        IEntityExpirationAccessDriver<TEntity>
        where TEntity : ITrakHoundEntity
    {
        private const int _defaultTTL = 300; // 5 Minutes
        private const int _expirationInterval = 10000;


        private readonly int _ttl; // Seconds
        protected readonly long _initializedTimestamp;
        protected readonly Dictionary<string, TEntity> _entities = new Dictionary<string, TEntity>();
        protected readonly Dictionary<string, long> _updated = new Dictionary<string, long>();
        protected readonly Dictionary<string, long> _accessed = new Dictionary<string, long>();
        protected readonly Dictionary<string, MemoryConsumer<IEnumerable<TEntity>>> _publishConsumers = new Dictionary<string, MemoryConsumer<IEnumerable<TEntity>>>();
        protected readonly object _lock = new object();

        private CancellationTokenSource _stop;


        public int TTL => _ttl;


        public MemoryEntityDriver(ITrakHoundDriverConfiguration configuration) : base(configuration) 
        {
			_initializedTimestamp = UnixDateTime.Now;
            _ttl = configuration.ParameterExists("ttl") ? configuration.GetParameter<int>("ttl") : _defaultTTL;

            _stop = new CancellationTokenSource();
            _ = Task.Run(() => Worker(_stop.Token));
        }

		protected override void OnDisposed()
		{
            try
            {
                if (_stop != null) _stop.Cancel();

				lock (_lock)
				{
                    if (!_publishConsumers.IsNullOrEmpty())
                    {
                        foreach (var consumer in _publishConsumers) consumer.Value.Dispose();
                    }
				}
			}
            catch { }
		}


        #region "Read"

        public async Task<TrakHoundResponse<TEntity>> Read(long skip = 0, long take = 0, SortOrder order = SortOrder.Ascending)
        {
            var stpw = System.Diagnostics.Stopwatch.StartNew();
            var results = new List<TrakHoundResult<TEntity>>();

            List<TEntity> objs;
            lock (_lock) objs = _entities.Values.ToList();

            if (!objs.IsNullOrEmpty())
            {
                IEnumerable<TEntity> x;
                int lTake = (int)take;
                if (lTake < 1) lTake = 100;

                if (order == SortOrder.Ascending) x = objs.Skip(0).Take(lTake);
                else x = objs.OrderByDescending(o => o).Skip(0).Take(lTake);

                if (!x.IsNullOrEmpty())
                {
                    foreach (var y in x)
                    {
                        if (y.IsValid)
                        {
                            lock (_lock)
                            {
                                // Update Last Accessed
                                _accessed.Remove(y.Uuid);
                                _accessed.Add(y.Uuid, UnixDateTime.Now);
                            }

                            results.Add(new TrakHoundResult<TEntity>(Id, y.Uuid, TrakHoundResultType.Ok, y));
                        }
                    }
                }
            }

            stpw.Stop();
            return new TrakHoundResponse<TEntity>(results, stpw.ElapsedTicks);
        }

        public virtual async Task<TrakHoundResponse<TEntity>> Read(IEnumerable<string> uuids)
        {
            var stpw = System.Diagnostics.Stopwatch.StartNew();
            var results = new List<TrakHoundResult<TEntity>>();

            if (!uuids.IsNullOrEmpty())
            {
                lock (_lock)
                {
                    foreach (var uuid in uuids)
                    {
                        if (!string.IsNullOrEmpty(uuid))
                        {
                            if (_entities.TryGetValue(uuid, out var obj))
                            {
                                if (obj.IsValid)
                                {
                                    // Update Last Accessed
                                    _accessed.Remove(uuid);
                                    _accessed.Add(uuid, UnixDateTime.Now);

                                    results.Add(new TrakHoundResult<TEntity>(Id, uuid, TrakHoundResultType.Ok, obj));
                                }
                            }
                            else
                            {
                                results.Add(new TrakHoundResult<TEntity>(Id, uuid, TrakHoundResultType.NotFound));
                            }
                        }
                        else
                        {
                            results.Add(new TrakHoundResult<TEntity>(Id, null, TrakHoundResultType.BadRequest));
                        }
                    }
                }
            }
            else
            {
                results.Add(new TrakHoundResult<TEntity>(Id, null, TrakHoundResultType.BadRequest));
            }

            stpw.Stop();
            return new TrakHoundResponse<TEntity>(results, stpw.ElapsedTicks);
        }

        #endregion

        #region "Subscribe"

        public async Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<TEntity>>>> Subscribe()
        {
            var consumer = new MemoryConsumer<IEnumerable<TEntity>>(ProcessMessage);
            consumer.Disposed += ConsumerDisposed;
            lock (_lock) _publishConsumers.Add(consumer.Id, consumer);

            var results = new List<TrakHoundResult<ITrakHoundConsumer<IEnumerable<TEntity>>>>();
            results.Add(new TrakHoundResult<ITrakHoundConsumer<IEnumerable<TEntity>>>(Id, "All", TrakHoundResultType.Ok, consumer));

            return new TrakHoundResponse<ITrakHoundConsumer<IEnumerable<TEntity>>>(results);
        }

        private void ConsumerDisposed(object sender, string consumerId)
        {
            if (consumerId != null)
            {
                lock (_lock) _publishConsumers.Remove(consumerId);
            }
        }

        private static IEnumerable<TEntity> ProcessMessage(IEnumerable<string> keys, IEnumerable<TEntity> entities)
        {
            return entities;
        }

        #endregion

        #region "Publish"

        protected virtual TrakHoundPublishResult<TEntity> OnPublish(TEntity entity)
        {
            var result = new TrakHoundPublishResult<TEntity>();

            if (entity != null && !string.IsNullOrEmpty(entity.Uuid) && entity.IsValid)
            {
                var now = UnixDateTime.Now;

                lock (_lock)
                {
                    var existingEntity = _entities.GetValueOrDefault(entity.Uuid);

                    if (existingEntity != null) result.Type = TrakHoundPublishResultType.Changed;
                    else result.Type = TrakHoundPublishResultType.Created;

                    if (PublishCompare(entity, existingEntity))
                    {
                        _entities.Remove(entity.Uuid);
                        _entities.Add(entity.Uuid, entity);

                        result.Result = entity;
                    }

                    _updated.Remove(entity.Uuid);
                    _updated.Add(entity.Uuid, now);
                }
            }

            return result;
        }

        protected virtual void OnPublishBefore(IEnumerable<TEntity> entities) { }

        protected virtual void OnPublishAfter(IEnumerable<TEntity> entities) { }

        protected virtual bool PublishCompare(TEntity newEntity, TEntity existingEntity)
        {
            return existingEntity == null || (!ObjectExtensions.ByteArraysEqual(existingEntity.Hash, newEntity.Hash) && newEntity.Created >= existingEntity.Created);
        }

		public async Task<TrakHoundResponse<TrakHoundPublishResult<TEntity>>> Publish(IEnumerable<TEntity> entities)
        {
            if (!entities.IsNullOrEmpty())
            {
                var stpw = System.Diagnostics.Stopwatch.StartNew();
                var results = new List<TrakHoundResult<TrakHoundPublishResult<TEntity>>>();

                var consumers = !_publishConsumers.IsNullOrEmpty() ? _publishConsumers.Values : null;
                var consumerEntities = new List<TEntity>();

                OnPublishBefore(entities);

                foreach (var entity in entities)
                {
                    var publishResult = OnPublish(entity);
                    if (publishResult.Result != null)
                    {
                        // Update Publish Consumers
                        if (entity.Created > _initializedTimestamp && consumers != null)
                        {
                            consumerEntities.Add(entity);
                        }
                    }

                    results.Add(new TrakHoundResult<TrakHoundPublishResult<TEntity>>(Id, entity.Uuid, TrakHoundResultType.Ok, publishResult));
                }

                // Update Publish Consumers
                if (!consumerEntities.IsNullOrEmpty() && consumers != null)
                {
                    foreach (var consumer in consumers)
                    {
                        if (consumer != null) consumer.Push(consumerEntities);
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
                lock (_lock)
                {
                    _entities.Remove(request.Target);
                    _updated.Remove(request.Target);
                    _accessed.Remove(request.Target);
                }
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
                        results.Add(new TrakHoundResult<bool>(Id, request.Target, TrakHoundResultType.Ok, true));
                    }
                    else
                    {
                        results.Add(new TrakHoundResult<bool>(Id, request.Target, TrakHoundResultType.InternalError, false));
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

            if (!requests.IsNullOrEmpty())
            {
                foreach (var request in requests)
                {
                    lock (_lock)
                    {
                        if (_updated.TryGetValue(request.Target, out var accessed))
                        {
                            if (accessed <= request.Timestamp)
                            {
                                _entities.Remove(request.Target);
                                _updated.Remove(request.Target);
                                _accessed.Remove(request.Target);

                                var result = new EntityDeleteResult(request.Target, 1);
                                results.Add(new TrakHoundResult<EntityDeleteResult>(Id, request.Target, TrakHoundResultType.Ok, result));
                            }
                        }
                    }
                }
            }

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

                lock (_lock)
                {
                    foreach (var acceesed in _updated)
                    {
                        if (acceesed.Value <= timestamp) removeKeys.Add(acceesed.Key);
                    }
                }

                foreach (var removeKey in removeKeys)
                {
                    lock (_lock)
                    {
                        _entities.Remove(removeKey);
                        _updated.Remove(removeKey);
                        _accessed.Remove(removeKey);
                    }

                    var result = new EntityDeleteResult(removeKey, 1);
                    results.Add(new TrakHoundResult<EntityDeleteResult>(Id, removeKey, TrakHoundResultType.Ok, result));
                }

                stpw.Stop();
                return new TrakHoundResponse<EntityDeleteResult>(results, stpw.ElapsedTicks);
            }

            return TrakHoundResponse<EntityDeleteResult>.InternalError(Id, null);
        }


        //public async Task<TrakHoundResponse<EntityDeleteResult>> ExpireByUpdate(long lastUpdated)
        //{
        //    if (lastUpdated > 0)
        //    {
        //        var stpw = System.Diagnostics.Stopwatch.StartNew();
        //        var results = new List<TrakHoundResult<EntityDeleteResult>>();

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

        //                            results.Add(new TrakHoundResult<EntityDeleteResult>(Id, entityId, TrakHoundResultType.Ok, 1));
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

        //                results.Add(new TrakHoundResult<EntityDeleteResult>(Id, null, TrakHoundResultType.Ok, 1));
        //            }
        //        }

        //        stpw.Stop();
        //        return new TrakHoundResponse<EntityDeleteResult>(results, stpw.ElapsedTicks);
        //    }

        //    return TrakHoundResponse<EntityDeleteResult>.InternalError(Id, null);
        //}


        public async Task<TrakHoundResponse<EntityDeleteResult>> ExpireByAccess(IEnumerable<EntityDeleteRequest> requests)
        {
            var stpw = System.Diagnostics.Stopwatch.StartNew();
            var results = new List<TrakHoundResult<EntityDeleteResult>>();

            if (!requests.IsNullOrEmpty())
            {
                foreach (var request in requests)
                {
                    lock (_lock)
                    {
                        if (_accessed.TryGetValue(request.Target, out var accessed))
                        {
                            if (accessed <= request.Timestamp)
                            {
                                _entities.Remove(request.Target);
                                _updated.Remove(request.Target);
                                _accessed.Remove(request.Target);

                                var result = new EntityDeleteResult(request.Target, 1);
                                results.Add(new TrakHoundResult<EntityDeleteResult>(Id, request.Target, TrakHoundResultType.Ok, result));
                            }
                        }
                    }
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

                var removeKeys = new List<string>();

                lock (_lock)
                {
                    foreach (var acceesed in _accessed)
                    {
                        if (acceesed.Value <= timestamp) removeKeys.Add(acceesed.Key);
                    }
                }

                foreach (var removeKey in removeKeys)
                {
                    lock (_lock)
                    {
                        _entities.Remove(removeKey);
                        _updated.Remove(removeKey);
                        _accessed.Remove(removeKey);
                    }

                    var result = new EntityDeleteResult(removeKey, 1);
                    results.Add(new TrakHoundResult<EntityDeleteResult>(Id, removeKey, TrakHoundResultType.Ok, result));
                }

                stpw.Stop();
                return new TrakHoundResponse<EntityDeleteResult>(results, stpw.ElapsedTicks);
            }

            return TrakHoundResponse<EntityDeleteResult>.InternalError(Id, null);
        }

        #endregion


        #region "TTL"

        private async Task Worker(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var expiredKeys = new HashSet<string>();
                    var now = UnixDateTime.Now;

                    lock (_lock)
                    {
                        foreach (var pair in _accessed)
                        {
                            long diff = now - pair.Value;
                            long ttl = (long)_ttl * 1000000000;

                            if (diff > ttl) // Convert TTL (seconds) to Nanoseconds
                            {
                                expiredKeys.Add(pair.Key);
                            }
                        }

                        foreach (var key in expiredKeys)
                        {
                            _entities.Remove(key);
                            _accessed.Remove(key);
                            _updated.Remove(key);
                        }
                    }

                    await Task.Delay(_expirationInterval);
                }
            }
            catch { }
        }

        #endregion

    }
}
