// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Drivers.Entities;
using TrakHound.Entities;
using TrakHound.Memory;

namespace TrakHound.Drivers.Memory
{
    public class ObjectStateDriver : MemoryEntityDriver<ITrakHoundObjectStateEntity>,
        IObjectStateLatestDriver,
        IObjectStateSubscribeDriver,
        IEntityEmptyDriver<ITrakHoundObjectStateEntity>
    {
        private readonly Dictionary<string, ITrakHoundObjectStateEntity> _latest = new Dictionary<string, ITrakHoundObjectStateEntity>();
        private readonly HashSet<string> _empty = new HashSet<string>();


        public ObjectStateDriver(ITrakHoundDriverConfiguration configuration) : base(configuration) { }


        protected override bool PublishCompare(ITrakHoundObjectStateEntity newEntity, ITrakHoundObjectStateEntity existingEntity)
        {
            return existingEntity != null ? newEntity.Timestamp >= existingEntity.Timestamp : true;
        }

        protected override TrakHoundPublishResult<ITrakHoundObjectStateEntity> OnPublish(ITrakHoundObjectStateEntity entity)
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

            return new TrakHoundPublishResult<ITrakHoundObjectStateEntity>();
        }


        public async Task<TrakHoundResponse<ITrakHoundObjectStateEntity>> Latest(IEnumerable<string> objectIds)
        {
            var stpw = System.Diagnostics.Stopwatch.StartNew();
            var results = new List<TrakHoundResult<ITrakHoundObjectStateEntity>>();

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

                                results.Add(new TrakHoundResult<ITrakHoundObjectStateEntity>(Id, objectId, TrakHoundResultType.Empty));
                            }
                            else if (_latest.TryGetValue(objectId, out var observation))
                            {
                                if (observation != null)
                                {
                                    // Update Last Accessed
                                    _accessed.Remove(objectId);
                                    _accessed.Add(objectId, UnixDateTime.Now);

                                    results.Add(new TrakHoundResult<ITrakHoundObjectStateEntity>(Id, objectId, TrakHoundResultType.Ok, observation));
                                }
                            }
                            else
                            {
                                results.Add(new TrakHoundResult<ITrakHoundObjectStateEntity>(Id, objectId, TrakHoundResultType.NotFound));
                            }
                        }
                    }
                    else
                    {
                        results.Add(new TrakHoundResult<ITrakHoundObjectStateEntity>(Id, null, TrakHoundResultType.BadRequest));
                    }
                }
            }
            else
            {
                results.Add(new TrakHoundResult<ITrakHoundObjectStateEntity>(Id, null, TrakHoundResultType.BadRequest));
            }

            stpw.Stop();
            return new TrakHoundResponse<ITrakHoundObjectStateEntity>(results, stpw.ElapsedTicks);
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

        public async Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectStateEntity>>>> Subscribe(IEnumerable<string> objectIds)
        {
            var consumer = new MemoryConsumer<IEnumerable<ITrakHoundObjectStateEntity>>(objectIds, ProcessMessage);
            _publishConsumers.Add(consumer.Id, consumer);

            var results = new List<TrakHoundResult<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectStateEntity>>>>();
            results.Add(new TrakHoundResult<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectStateEntity>>>(Id, "All", TrakHoundResultType.Ok, consumer));

            return new TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectStateEntity>>>(results);
        }

        private static IEnumerable<ITrakHoundObjectStateEntity> ProcessMessage(IEnumerable<string> keys, IEnumerable<ITrakHoundObjectStateEntity> entities)
        {
            var resultEntities = new List<ITrakHoundObjectStateEntity>();
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
