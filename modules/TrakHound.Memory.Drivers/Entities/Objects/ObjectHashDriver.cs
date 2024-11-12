// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Drivers.Entities;
using TrakHound.Entities;
using TrakHound.Memory;

namespace TrakHound.Drivers.Memory
{
    public class ObjectHashDriver : MemoryEntityDriver<ITrakHoundObjectHashEntity>, IObjectHashSubscribeDriver
    {
        private readonly Dictionary<string, ITrakHoundObjectHashEntity> _hashes = new Dictionary<string, ITrakHoundObjectHashEntity>();
        private readonly HashSet<string> _empty = new HashSet<string>();


        public ObjectHashDriver(ITrakHoundDriverConfiguration configuration) : base(configuration) { }


        protected override bool PublishCompare(ITrakHoundObjectHashEntity newEntity, ITrakHoundObjectHashEntity existingEntity)
        {
            return existingEntity != null ? newEntity.Created > existingEntity.Created : true;
        }


        #region "Subscribe"

        public async Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectHashEntity>>>> Subscribe(IEnumerable<string> objectIds)
        {
            var consumer = new MemoryConsumer<IEnumerable<ITrakHoundObjectHashEntity>>(objectIds, ProcessMessage);
            _publishConsumers.Add(consumer.Id, consumer);

            var results = new List<TrakHoundResult<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectHashEntity>>>>();
            results.Add(new TrakHoundResult<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectHashEntity>>>(Id, "All", TrakHoundResultType.Ok, consumer));

            return new TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectHashEntity>>>(results);
        }

        private static IEnumerable<ITrakHoundObjectHashEntity> ProcessMessage(IEnumerable<string> keys, IEnumerable<ITrakHoundObjectHashEntity> entities)
        {
            var resultEntities = new List<ITrakHoundObjectHashEntity>();
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


        protected override TrakHoundPublishResult<ITrakHoundObjectHashEntity> OnPublish(ITrakHoundObjectHashEntity entity)
        {
            if (entity != null && !string.IsNullOrEmpty(entity.ObjectUuid))
            {
                lock (_lock)
                {
                    _empty.Remove(entity.ObjectUuid);

                    _hashes.Remove(entity.ObjectUuid);
                    _hashes.Add(entity.ObjectUuid, entity);

                    _updated.Remove(entity.ObjectUuid);
                    _updated.Add(entity.ObjectUuid, UnixDateTime.Now);
                }

                return base.OnPublish(entity);
            }

            return new TrakHoundPublishResult<ITrakHoundObjectHashEntity>();
        }
    }
}
