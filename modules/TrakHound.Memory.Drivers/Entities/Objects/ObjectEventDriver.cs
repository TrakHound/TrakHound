// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Drivers.Entities;
using TrakHound.Entities;
using TrakHound.Memory;

namespace TrakHound.Drivers.Memory
{
    public class ObjectEventDriver : MemoryEntityDriver<ITrakHoundObjectEventEntity>,
        IObjectEventLatestDriver,
        IObjectEventSubscribeDriver,
        IObjectEventDeleteDriver,
        IEntityEmptyDriver<ITrakHoundObjectEventEntity>
    {
        private readonly Dictionary<string, ITrakHoundObjectEventEntity> _latest = new Dictionary<string, ITrakHoundObjectEventEntity>();
        private readonly HashSet<string> _empty = new HashSet<string>();


        public ObjectEventDriver(ITrakHoundDriverConfiguration configuration) : base(configuration) { }


        protected override bool PublishCompare(ITrakHoundObjectEventEntity newEntity, ITrakHoundObjectEventEntity existingEntity)
        {
            return existingEntity != null ? newEntity.Timestamp > existingEntity.Timestamp : true;
        }

        protected override TrakHoundPublishResult<ITrakHoundObjectEventEntity> OnPublish(ITrakHoundObjectEventEntity entity)
        {
            if (entity != null && !string.IsNullOrEmpty(entity.ObjectUuid))
            {
                lock (_lock)
                {
                    _empty.Remove(entity.ObjectUuid);

                    _latest.Remove(entity.ObjectUuid);
                    _latest.Add(entity.ObjectUuid, entity);
                }

                return base.OnPublish(entity);
            }

            return new TrakHoundPublishResult<ITrakHoundObjectEventEntity>();
        }


        public async Task<TrakHoundResponse<ITrakHoundObjectEventEntity>> Latest(IEnumerable<string> objectIds)
        {
            var stpw = System.Diagnostics.Stopwatch.StartNew();
            var results = new List<TrakHoundResult<ITrakHoundObjectEventEntity>>();

            if (!objectIds.IsNullOrEmpty())
            {
                foreach (var objectId in objectIds)
                {
                    if (!string.IsNullOrEmpty(objectId))
                    {
                        lock (_lock)
                        {
                            var empty = _empty.Contains(objectId);
                            if (empty)
                            {
                                // Update Last Accessed
                                _accessed.Remove(objectId);
                                _accessed.Add(objectId, UnixDateTime.Now);

                                results.Add(new TrakHoundResult<ITrakHoundObjectEventEntity>(Id, objectId, TrakHoundResultType.Empty));
                            }
                            else if (_latest.TryGetValue(objectId, out var observation))
                            {
                                if (observation != null)
                                {
                                    // Update Last Accessed
                                    _accessed.Remove(objectId);
                                    _accessed.Add(objectId, UnixDateTime.Now);

                                    results.Add(new TrakHoundResult<ITrakHoundObjectEventEntity>(Id, objectId, TrakHoundResultType.Ok, observation));
                                }
                            }
                            else
                            {
                                results.Add(new TrakHoundResult<ITrakHoundObjectEventEntity>(Id, objectId, TrakHoundResultType.NotFound));
                            }
                        }
                    }
                    else
                    {
                        results.Add(new TrakHoundResult<ITrakHoundObjectEventEntity>(Id, null, TrakHoundResultType.BadRequest));
                    }
                }
            }
            else
            {
                results.Add(new TrakHoundResult<ITrakHoundObjectEventEntity>(Id, null, TrakHoundResultType.BadRequest));
            }

            stpw.Stop();
            return new TrakHoundResponse<ITrakHoundObjectEventEntity>(results, stpw.ElapsedTicks);
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
                            long latestTimestamp = 0;

                            var existingEntity = _latest.GetValueOrDefault(request.EntityUuid);
                            if (existingEntity != null)
                            {
                                latestTimestamp = existingEntity.Timestamp;
                            }

                            if (request.Timestamp > latestTimestamp && !_empty.Contains(request.EntityUuid))
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

        public async Task<TrakHoundResponse<bool>> DeleteByObjectUuid(IEnumerable<string> objectUuids)
        {
            if (!objectUuids.IsNullOrEmpty())
            {
                var stpw = System.Diagnostics.Stopwatch.StartNew();
                var results = new List<TrakHoundResult<bool>>();

                foreach (var objectUuid in objectUuids)
                {
                    if (!string.IsNullOrEmpty(objectUuid))
                    {
                        lock (_lock)
                        {
                            var latest = _latest.GetValueOrDefault(objectUuid);
                            if (latest != null && latest.Uuid != null) _entities.Remove(latest.Uuid);

                            _latest.Remove(objectUuid);
                            _empty.Remove(objectUuid);
                            _updated.Remove(objectUuid);
                        }

                        results.Add(new TrakHoundResult<bool>(Id, objectUuid, TrakHoundResultType.Ok, true));
                    }
                }

                stpw.Stop();
                return new TrakHoundResponse<bool>(results, stpw.ElapsedTicks);
            }

            return TrakHoundResponse<bool>.InternalError(Id, null);
        }


        #region "Subscribe"

        public async Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectEventEntity>>>> Subscribe(IEnumerable<string> objectIds)
        {
            var consumer = new MemoryConsumer<IEnumerable<ITrakHoundObjectEventEntity>>(objectIds, ProcessMessage);
            _publishConsumers.Add(consumer.Id, consumer);

            var results = new List<TrakHoundResult<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectEventEntity>>>>();
            results.Add(new TrakHoundResult<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectEventEntity>>>(Id, "All", TrakHoundResultType.Ok, consumer));

            return new TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectEventEntity>>>(results);
        }

        private static IEnumerable<ITrakHoundObjectEventEntity> ProcessMessage(IEnumerable<string> keys, IEnumerable<ITrakHoundObjectEventEntity> entities)
        {
            var resultEntities = new List<ITrakHoundObjectEventEntity>();
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

    }
}
