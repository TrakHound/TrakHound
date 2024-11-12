// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Drivers.Entities;
using TrakHound.Entities;
using TrakHound.Memory;

namespace TrakHound.Drivers.Memory
{
    public class ObjectDurationDriver : 
        MemoryEntityDriver<ITrakHoundObjectDurationEntity>,
        IObjectDurationSubscribeDriver,
        IObjectDurationQueryDriver,
        IEntityEmptyDriver<ITrakHoundObjectDurationEntity>
    {
        private readonly Dictionary<string, ITrakHoundObjectDurationEntity> _durations = new Dictionary<string, ITrakHoundObjectDurationEntity>();
        private readonly HashSet<string> _empty = new HashSet<string>();


        public ObjectDurationDriver(ITrakHoundDriverConfiguration configuration) : base(configuration) { }


        protected override bool PublishCompare(ITrakHoundObjectDurationEntity newEntity, ITrakHoundObjectDurationEntity existingEntity)
        {
            return existingEntity != null ? newEntity.Created > existingEntity.Created : true;
        }


        #region "Subscribe"

        public async Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectDurationEntity>>>> Subscribe(IEnumerable<string> objectIds)
        {
            var consumer = new MemoryConsumer<IEnumerable<ITrakHoundObjectDurationEntity>>(objectIds, ProcessMessage);
            _publishConsumers.Add(consumer.Id, consumer);

            var results = new List<TrakHoundResult<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectDurationEntity>>>>();
            results.Add(new TrakHoundResult<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectDurationEntity>>>(Id, "All", TrakHoundResultType.Ok, consumer));

            return new TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectDurationEntity>>>(results);
        }

        private static IEnumerable<ITrakHoundObjectDurationEntity> ProcessMessage(IEnumerable<string> keys, IEnumerable<ITrakHoundObjectDurationEntity> entities)
        {
            var resultEntities = new List<ITrakHoundObjectDurationEntity>();
            foreach (var entity in entities)
            {
                if (keys.Contains(entity.ObjectUuid))
                {
                    resultEntities.Add(entity);
                }
            }
            return resultEntities;
        }

        #endregion


        public async Task<TrakHoundResponse<ITrakHoundObjectDurationEntity>> Query(IEnumerable<string> objectUuids)
        {
            var stpw = System.Diagnostics.Stopwatch.StartNew();
            var results = new List<TrakHoundResult<ITrakHoundObjectDurationEntity>>();

            if (!objectUuids.IsNullOrEmpty())
            {
                foreach (var objectUuid in objectUuids)
                {
                    if (!string.IsNullOrEmpty(objectUuid))
                    {
                        lock (_lock)
                        {
                            var empty = _empty.Contains(objectUuid);
                            if (empty)
                            {
                                // Update Last Accessed
                                _accessed.Remove(objectUuid);
                                _accessed.Add(objectUuid, UnixDateTime.Now);

                                results.Add(new TrakHoundResult<ITrakHoundObjectDurationEntity>(Id, objectUuid, TrakHoundResultType.Empty));
                            }
                            else if (_durations.TryGetValue(objectUuid, out var obj))
                            {
                                if (obj != null)
                                {
                                    // Update Last Accessed
                                    _accessed.Remove(objectUuid);
                                    _accessed.Add(objectUuid, UnixDateTime.Now);

                                    results.Add(new TrakHoundResult<ITrakHoundObjectDurationEntity>(Id, objectUuid, TrakHoundResultType.Ok, obj));
                                }
                            }
                            else
                            {
                                results.Add(new TrakHoundResult<ITrakHoundObjectDurationEntity>(Id, objectUuid, TrakHoundResultType.NotFound));
                            }
                        }
                    }
                    else
                    {
                        results.Add(new TrakHoundResult<ITrakHoundObjectDurationEntity>(Id, null, TrakHoundResultType.BadRequest));
                    }
                }
            }
            else
            {
                results.Add(new TrakHoundResult<ITrakHoundObjectDurationEntity>(Id, null, TrakHoundResultType.BadRequest));
            }

            stpw.Stop();
            return new TrakHoundResponse<ITrakHoundObjectDurationEntity>(results, stpw.ElapsedTicks);
        }

        public async Task<TrakHoundResponse<ITrakHoundObjectDurationEntity>> Query(IEnumerable<string> objectUuids, long min, long max)
        {
            var stpw = System.Diagnostics.Stopwatch.StartNew();
            var results = new List<TrakHoundResult<ITrakHoundObjectDurationEntity>>();

            if (!objectUuids.IsNullOrEmpty())
            {
                foreach (var objectUuid in objectUuids)
                {
                    if (!string.IsNullOrEmpty(objectUuid))
                    {
                        lock (_lock)
                        {
                            var empty = _empty.Contains(objectUuid);
                            if (empty)
                            {
                                // Update Last Accessed
                                _accessed.Remove(objectUuid);
                                _accessed.Add(objectUuid, UnixDateTime.Now);

                                results.Add(new TrakHoundResult<ITrakHoundObjectDurationEntity>(Id, objectUuid, TrakHoundResultType.Empty));
                            }
                            else if (_durations.TryGetValue(objectUuid, out var obj))
                            {
                                if (obj != null && obj.Value.ToDouble() >= min && obj.Value.ToDouble() <= max)
                                {
                                    // Update Last Accessed
                                    _accessed.Remove(objectUuid);
                                    _accessed.Add(objectUuid, UnixDateTime.Now);

                                    results.Add(new TrakHoundResult<ITrakHoundObjectDurationEntity>(Id, objectUuid, TrakHoundResultType.Ok, obj));
                                }
                                else
                                {
                                    results.Add(new TrakHoundResult<ITrakHoundObjectDurationEntity>(Id, objectUuid, TrakHoundResultType.NotFound));
                                }
                            }
                            else
                            {
                                results.Add(new TrakHoundResult<ITrakHoundObjectDurationEntity>(Id, objectUuid, TrakHoundResultType.NotFound));
                            }
                        }
                    }
                    else
                    {
                        results.Add(new TrakHoundResult<ITrakHoundObjectDurationEntity>(Id, null, TrakHoundResultType.BadRequest));
                    }
                }
            }
            else
            {
                results.Add(new TrakHoundResult<ITrakHoundObjectDurationEntity>(Id, null, TrakHoundResultType.BadRequest));
            }

            stpw.Stop();
            return new TrakHoundResponse<ITrakHoundObjectDurationEntity>(results, stpw.ElapsedTicks);
        }


        public async Task<TrakHoundResponse<bool>> Empty(IEnumerable<EntityEmptyRequest> requests)
        {
            if (!requests.IsNullOrEmpty())
            {
                var stpw = System.Diagnostics.Stopwatch.StartNew();
                var results = new List<TrakHoundResult<bool>>();

                foreach (var request in requests)
                {
                    if (!string.IsNullOrEmpty(request.EntityUuid))
                    {
                        lock (_lock)
                        {
                            if (!_empty.Contains(request.EntityUuid))
                            {
                                _empty.Add(request.EntityUuid);

                                _updated.Remove(request.EntityUuid);
                                _updated.Add(request.EntityUuid, UnixDateTime.Now);
                            }
                        }

                        results.Add(new TrakHoundResult<bool>(Id, request.EntityUuid, TrakHoundResultType.Ok, true));
                    }
                }

                stpw.Stop();
                return new TrakHoundResponse<bool>(results, stpw.ElapsedTicks);
            }

            return TrakHoundResponse<bool>.InternalError(Id, null);
        }


        protected override TrakHoundPublishResult<ITrakHoundObjectDurationEntity> OnPublish(ITrakHoundObjectDurationEntity entity)
        {
            if (entity != null && !string.IsNullOrEmpty(entity.ObjectUuid))
            {
                lock (_lock)
                {
                    _empty.Remove(entity.ObjectUuid);

                    _durations.Remove(entity.ObjectUuid);
                    _durations.Add(entity.ObjectUuid, entity);

                    _updated.Remove(entity.ObjectUuid);
                    _updated.Add(entity.ObjectUuid, UnixDateTime.Now);
                }

                return base.OnPublish(entity);
            }

            return new TrakHoundPublishResult<ITrakHoundObjectDurationEntity>();
        }
    }
}
