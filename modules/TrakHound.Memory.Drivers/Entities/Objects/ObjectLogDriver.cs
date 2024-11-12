// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Drivers.Entities;
using TrakHound.Entities;
using TrakHound.Memory;

namespace TrakHound.Drivers.Memory
{
    public class ObjectLogDriver : MemoryEntityDriver<ITrakHoundObjectLogEntity>, IObjectLogSubscribeDriver
    {
        public ObjectLogDriver(ITrakHoundDriverConfiguration configuration) : base(configuration) { }


        protected override bool PublishCompare(ITrakHoundObjectLogEntity newEntity, ITrakHoundObjectLogEntity existingEntity)
        {
            return existingEntity != null ? newEntity.Timestamp > existingEntity.Timestamp : true;
        }


        #region "Subscribe"

        public async Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectLogEntity>>>> Subscribe(IEnumerable<string> objectUuids, TrakHoundLogLevel level)
        {
            var matchUuids = new HashSet<string>();
            if (!objectUuids.IsNullOrEmpty())
            {
                foreach (var objectUuid in objectUuids) matchUuids.Add(objectUuid);
            }

            var processFunction = (IEnumerable<string> keys, IEnumerable<ITrakHoundObjectLogEntity> entities) =>
            {
                var resultEntities = new List<ITrakHoundObjectLogEntity>();
                foreach (var entity in entities)
                {
                    if (matchUuids.Contains(entity.ObjectUuid) && entity.LogLevel <= (int)level)
                    {
                        resultEntities.Add(entity);
                    }
                }
                return resultEntities;
            };

            var consumer = new MemoryConsumer<IEnumerable<ITrakHoundObjectLogEntity>>(objectUuids, processFunction);
            _publishConsumers.Add(consumer.Id, consumer);

            var results = new List<TrakHoundResult<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectLogEntity>>>>();
            foreach (var objectUuid in objectUuids)
            {
                results.Add(new TrakHoundResult<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectLogEntity>>>(Id, objectUuid, TrakHoundResultType.Ok, consumer));
            }

            return new TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectLogEntity>>>(results);
        }

        public async Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectLogEntity>>>> Subscribe(TrakHoundLogLevel level)
        {
            var processFunction = (IEnumerable<string> keys, IEnumerable<ITrakHoundObjectLogEntity> entities) =>
            {
                var resultEntities = new List<ITrakHoundObjectLogEntity>();
                foreach (var entity in entities)
                {
                    if (entity.LogLevel <= (int)level)
                    {
                        resultEntities.Add(entity);                    
                    }
                }
                return resultEntities;
            };

            var consumer = new MemoryConsumer<IEnumerable<ITrakHoundObjectLogEntity>>(null, processFunction);
            _publishConsumers.Add(consumer.Id, consumer);

            var results = new List<TrakHoundResult<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectLogEntity>>>>();
            results.Add(new TrakHoundResult<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectLogEntity>>>(Id, "All", TrakHoundResultType.Ok, consumer));

            return new TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectLogEntity>>>(results);
        }

        #endregion

    }
}
