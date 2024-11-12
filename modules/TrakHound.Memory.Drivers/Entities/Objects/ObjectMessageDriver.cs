// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Drivers.Entities;
using TrakHound.Entities;
using TrakHound.Memory;

namespace TrakHound.Drivers.Memory
{
    public class ObjectMessageDriver : 
        MemoryEntityDriver<ITrakHoundObjectMessageEntity>, 
        IObjectMessageSubscribeDriver,
        IObjectMessageQueryDriver,
        IEntityEmptyDriver<ITrakHoundObjectMessageEntity>
    {
        private readonly Dictionary<string, ITrakHoundObjectMessageEntity> _messages = new Dictionary<string, ITrakHoundObjectMessageEntity>();
        private readonly HashSet<string> _empty = new HashSet<string>();


        public ObjectMessageDriver(ITrakHoundDriverConfiguration configuration) : base(configuration) { }


        protected override bool PublishCompare(ITrakHoundObjectMessageEntity newEntity, ITrakHoundObjectMessageEntity existingEntity)
        {
            return existingEntity != null ? newEntity.Created > existingEntity.Created : true;
        }


        #region "Subscribe"

        public async Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectMessageEntity>>>> Subscribe(IEnumerable<string> objectIds)
        {
            var consumer = new MemoryConsumer<IEnumerable<ITrakHoundObjectMessageEntity>>(objectIds, ProcessMessage);
			_publishConsumers.Add(consumer.Id, consumer);

            var results = new List<TrakHoundResult<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectMessageEntity>>>>();
            results.Add(new TrakHoundResult<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectMessageEntity>>>(Id, "All", TrakHoundResultType.Ok, consumer));

            return new TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectMessageEntity>>>(results);
        }

        private static IEnumerable<ITrakHoundObjectMessageEntity> ProcessMessage(IEnumerable<string> keys, IEnumerable<ITrakHoundObjectMessageEntity> entities)
        {
            var resultEntities = new List<ITrakHoundObjectMessageEntity>();
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


        public async Task<TrakHoundResponse<ITrakHoundObjectMessageEntity>> Query(IEnumerable<string> objectUuids)
        {
            var stpw = System.Diagnostics.Stopwatch.StartNew();
            var results = new List<TrakHoundResult<ITrakHoundObjectMessageEntity>>();

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

                                results.Add(new TrakHoundResult<ITrakHoundObjectMessageEntity>(Id, objectUuid, TrakHoundResultType.Empty));
                            }
                            else if (_messages.TryGetValue(objectUuid, out var obj))
                            {
                                if (obj != null)
                                {
                                    // Update Last Accessed
                                    _accessed.Remove(objectUuid);
                                    _accessed.Add(objectUuid, UnixDateTime.Now);

                                    results.Add(new TrakHoundResult<ITrakHoundObjectMessageEntity>(Id, objectUuid, TrakHoundResultType.Ok, obj));
                                }
                            }
                            else
                            {
                                results.Add(new TrakHoundResult<ITrakHoundObjectMessageEntity>(Id, objectUuid, TrakHoundResultType.NotFound));
                            }
                        }
                    }
                    else
                    {
                        results.Add(new TrakHoundResult<ITrakHoundObjectMessageEntity>(Id, null, TrakHoundResultType.BadRequest));
                    }
                }
            }
            else
            {
                results.Add(new TrakHoundResult<ITrakHoundObjectMessageEntity>(Id, null, TrakHoundResultType.BadRequest));
            }

            stpw.Stop();
            return new TrakHoundResponse<ITrakHoundObjectMessageEntity>(results, stpw.ElapsedTicks);
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


        protected override TrakHoundPublishResult<ITrakHoundObjectMessageEntity> OnPublish(ITrakHoundObjectMessageEntity entity)
        {
            if (entity != null && !string.IsNullOrEmpty(entity.ObjectUuid))
            {
                lock (_lock)
                {
                    _empty.Remove(entity.ObjectUuid);

                    _messages.Remove(entity.ObjectUuid);
                    _messages.Add(entity.ObjectUuid, entity);

                    _updated.Remove(entity.ObjectUuid);
                    _updated.Add(entity.ObjectUuid, UnixDateTime.Now);
                }

                return base.OnPublish(entity);
            }

            return new TrakHoundPublishResult<ITrakHoundObjectMessageEntity>();
        }
    }
}
