// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Drivers.Entities;
using TrakHound.Entities;
using TrakHound.Memory;

namespace TrakHound.Drivers.Memory
{
    public class ObjectObservationDriver : MemoryEntityDriver<ITrakHoundObjectObservationEntity>,
        IObjectObservationLatestDriver,
        IObjectObservationSubscribeDriver
    {
        private readonly Dictionary<string, ITrakHoundObjectObservationEntity> _latest = new Dictionary<string, ITrakHoundObjectObservationEntity>();
        private readonly HashSet<string> _empty = new HashSet<string>();


        public ObjectObservationDriver(ITrakHoundDriverConfiguration configuration) : base(configuration) { }


        protected override bool PublishCompare(ITrakHoundObjectObservationEntity newEntity, ITrakHoundObjectObservationEntity existingEntity)
        {
            return existingEntity != null ? newEntity.Timestamp > existingEntity.Timestamp : true;
        }

        protected override TrakHoundPublishResult<ITrakHoundObjectObservationEntity> OnPublish(ITrakHoundObjectObservationEntity entity)
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

            return new TrakHoundPublishResult<ITrakHoundObjectObservationEntity>();
        }


        public async Task<TrakHoundResponse<ITrakHoundObjectObservationEntity>> Latest(IEnumerable<string> objectIds)
        {
            var stpw = System.Diagnostics.Stopwatch.StartNew();
            var results = new List<TrakHoundResult<ITrakHoundObjectObservationEntity>>();

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

                                results.Add(new TrakHoundResult<ITrakHoundObjectObservationEntity>(Id, objectId, TrakHoundResultType.Empty));
                            }
                            else if (_latest.TryGetValue(objectId, out var observation))
                            {
                                if (observation != null)
                                {
                                    // Update Last Accessed
                                    _accessed.Remove(objectId);
                                    _accessed.Add(objectId, UnixDateTime.Now);

                                    results.Add(new TrakHoundResult<ITrakHoundObjectObservationEntity>(Id, objectId, TrakHoundResultType.Ok, observation));
                                }
                            }
                            else
                            {
                                results.Add(new TrakHoundResult<ITrakHoundObjectObservationEntity>(Id, objectId, TrakHoundResultType.NotFound));
                            }
                        }
                    }
                    else
                    {
                        results.Add(new TrakHoundResult<ITrakHoundObjectObservationEntity>(Id, null, TrakHoundResultType.BadRequest));
                    }
                }
            }
            else
            {
                results.Add(new TrakHoundResult<ITrakHoundObjectObservationEntity>(Id, null, TrakHoundResultType.BadRequest));
            }

            stpw.Stop();
            return new TrakHoundResponse<ITrakHoundObjectObservationEntity>(results, stpw.ElapsedTicks);
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


        #region "Subscribe"

        public async Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectObservationEntity>>>> Subscribe(IEnumerable<string> objectIds)
        {
            var consumer = new MemoryConsumer<IEnumerable<ITrakHoundObjectObservationEntity>>(objectIds, ProcessMessage);
            _publishConsumers.Add(consumer.Id, consumer);

            var results = new List<TrakHoundResult<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectObservationEntity>>>>();
            results.Add(new TrakHoundResult<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectObservationEntity>>>(Id, "All", TrakHoundResultType.Ok, consumer));

            return new TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectObservationEntity>>>(results);
        }

        private static IEnumerable<ITrakHoundObjectObservationEntity> ProcessMessage(IEnumerable<string> keys, IEnumerable<ITrakHoundObjectObservationEntity> entities)
        {
            var resultEntities = new List<ITrakHoundObjectObservationEntity>();
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
