// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Drivers.Entities;
using TrakHound.Entities;
using TrakHound.Memory;

namespace TrakHound.Drivers.Memory
{
    public class ObjectVocabularyDriver : MemoryEntityDriver<ITrakHoundObjectVocabularyEntity>, IObjectVocabularySubscribeDriver, IObjectVocabularyQueryDriver
    {
        private readonly Dictionary<string, ITrakHoundObjectVocabularyEntity> _vocabularies = new Dictionary<string, ITrakHoundObjectVocabularyEntity>();
        private readonly HashSet<string> _empty = new HashSet<string>();


        public ObjectVocabularyDriver(ITrakHoundDriverConfiguration configuration) : base(configuration) { }


        protected override bool PublishCompare(ITrakHoundObjectVocabularyEntity newEntity, ITrakHoundObjectVocabularyEntity existingEntity)
        {
            return existingEntity != null ? newEntity.Created > existingEntity.Created : true;
        }


        #region "Subscribe"

        public async Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectVocabularyEntity>>>> Subscribe(IEnumerable<string> objectIds)
        {
            var consumer = new MemoryConsumer<IEnumerable<ITrakHoundObjectVocabularyEntity>>(objectIds, ProcessMessage);
            _publishConsumers.Add(consumer.Id, consumer);

            var results = new List<TrakHoundResult<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectVocabularyEntity>>>>();
            results.Add(new TrakHoundResult<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectVocabularyEntity>>>(Id, "All", TrakHoundResultType.Ok, consumer));

            return new TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectVocabularyEntity>>>(results);
        }

        private static IEnumerable<ITrakHoundObjectVocabularyEntity> ProcessMessage(IEnumerable<string> keys, IEnumerable<ITrakHoundObjectVocabularyEntity> entities)
        {
            var resultEntities = new List<ITrakHoundObjectVocabularyEntity>();
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


        public async Task<TrakHoundResponse<ITrakHoundObjectVocabularyEntity>> Query(IEnumerable<string> objectUuids)
        {
            var stpw = System.Diagnostics.Stopwatch.StartNew();
            var results = new List<TrakHoundResult<ITrakHoundObjectVocabularyEntity>>();

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

                                results.Add(new TrakHoundResult<ITrakHoundObjectVocabularyEntity>(Id, objectUuid, TrakHoundResultType.Empty));
                            }
                            else if (_vocabularies.TryGetValue(objectUuid, out var obj))
                            {
                                if (obj != null)
                                {
                                    // Update Last Accessed
                                    _accessed.Remove(objectUuid);
                                    _accessed.Add(objectUuid, UnixDateTime.Now);

                                    results.Add(new TrakHoundResult<ITrakHoundObjectVocabularyEntity>(Id, objectUuid, TrakHoundResultType.Ok, obj));
                                }
                            }
                            else
                            {
                                results.Add(new TrakHoundResult<ITrakHoundObjectVocabularyEntity>(Id, objectUuid, TrakHoundResultType.NotFound));
                            }
                        }
                    }
                    else
                    {
                        results.Add(new TrakHoundResult<ITrakHoundObjectVocabularyEntity>(Id, null, TrakHoundResultType.BadRequest));
                    }
                }
            }
            else
            {
                results.Add(new TrakHoundResult<ITrakHoundObjectVocabularyEntity>(Id, null, TrakHoundResultType.BadRequest));
            }

            stpw.Stop();
            return new TrakHoundResponse<ITrakHoundObjectVocabularyEntity>(results, stpw.ElapsedTicks);
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


        protected override TrakHoundPublishResult<ITrakHoundObjectVocabularyEntity> OnPublish(ITrakHoundObjectVocabularyEntity entity)
        {
            if (entity != null && !string.IsNullOrEmpty(entity.ObjectUuid))
            {
                lock (_lock)
                {
                    _empty.Remove(entity.ObjectUuid);

                    _vocabularies.Remove(entity.ObjectUuid);
                    _vocabularies.Add(entity.ObjectUuid, entity);

                    _updated.Remove(entity.ObjectUuid);
                    _updated.Add(entity.ObjectUuid, UnixDateTime.Now);
                }

                return base.OnPublish(entity);
            }

            return new TrakHoundPublishResult<ITrakHoundObjectVocabularyEntity>();
        }
    }
}
