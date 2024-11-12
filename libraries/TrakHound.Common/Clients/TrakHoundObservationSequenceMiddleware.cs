// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrakHound.Entities;

namespace TrakHound.Clients
{
    public class TrakHoundObservationSequenceMiddleware : ITrakHoundClientMiddleware, ITrakHoundEntitiesClientMiddleware
    {
        private const string _defaultId = "observation-sequence";

        private readonly string _id;
        private readonly PersistentDictionary<string, ulong> _sequences;
        private readonly object _lock = new object();


        public string Id => _id;

        public ITrakHoundClient Client { get; set; }


        public TrakHoundObservationSequenceMiddleware()
        {
            _id = _defaultId;

            _sequences = new PersistentDictionary<string, ulong>(_defaultId);
            _sequences.Recover();
        }


        public ITrakHoundEntity Process(ITrakHoundEntity entity, TrakHoundOperationMode operationMode)
        {
            if (entity != null && typeof(ITrakHoundObjectObservationEntity).IsAssignableFrom(entity.GetType()))
            {
                var observationEntity = (TrakHoundObjectObservationEntity)entity;
                var key = $"{observationEntity.ObjectUuid}:{observationEntity.BatchId}";

                lock (_lock)
                {
                    var sequence = _sequences.Get(key);
                    sequence++;
                    observationEntity.Sequence = sequence;

                    _sequences.Remove(key);
                    _sequences.Add(key, sequence);
                }

                return observationEntity;
            }

            return entity;
        }

        public IEnumerable<ITrakHoundEntity> Process(IEnumerable<ITrakHoundEntity> entities, TrakHoundOperationMode operationMode)
        {
            if (!entities.IsNullOrEmpty())
            {
                var sendEntities = new List<ITrakHoundEntity>();
                
                var nonMatchedEntities = entities.Where(o => o != null && !typeof(ITrakHoundObjectObservationEntity).IsAssignableFrom(o.GetType()));
                if (!nonMatchedEntities.IsNullOrEmpty()) sendEntities.AddRange(nonMatchedEntities);

                var matchedEntities = entities.Where(o => o != null && typeof(ITrakHoundObjectObservationEntity).IsAssignableFrom(o.GetType()));
                if (!matchedEntities.IsNullOrEmpty())
                {
                    var observationEntities = new List<TrakHoundObjectObservationEntity>();
                    foreach (var entity in matchedEntities)
                    {
                        observationEntities.Add((TrakHoundObjectObservationEntity)entity);
                    }

                    lock (_lock)
                    {
                        foreach (var entity in observationEntities.OrderBy(o => o.Timestamp))
                        {
                            var observationEntity = (TrakHoundObjectObservationEntity)entity;

                            var sequence = _sequences.Get(observationEntity.ObjectUuid);
                            sequence++;
                            observationEntity.Sequence = sequence;

                            _sequences.Remove(observationEntity.ObjectUuid);
                            _sequences.Add(observationEntity.ObjectUuid, sequence);

                            sendEntities.Add(observationEntity);
                        }
                    }
                }

                return sendEntities;
            }

            return entities;
        }


        public Task OnBeforePublish(IEnumerable<ITrakHoundEntity> entities, TrakHoundOperationMode operationMode) => Task.CompletedTask;

        public Task OnAfterPublish(IEnumerable<ITrakHoundEntity> entities, TrakHoundOperationMode operationMode) => Task.CompletedTask;


        public Task OnBeforePublish() => Task.CompletedTask;

        public Task OnAfterPublish() => Task.CompletedTask;
    }
}
