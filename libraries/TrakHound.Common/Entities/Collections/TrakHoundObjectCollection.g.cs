// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;

namespace TrakHound.Entities.Collections
{
    public partial class TrakHoundObjectCollection
    {
        private readonly TrakHoundEntityCollection _entityCollection;


        public bool IsEmpty 
        {
            get {
                var empty = true;
                if (empty) empty = _objects.IsNullOrEmpty();
                if (empty) empty = _metadata.IsNullOrEmpty();
                if (empty) empty = _assignments.IsNullOrEmpty();
                if (empty) empty = _blobs.IsNullOrEmpty();
                if (empty) empty = _booleans.IsNullOrEmpty();
                if (empty) empty = _durations.IsNullOrEmpty();
                if (empty) empty = _events.IsNullOrEmpty();
                if (empty) empty = _groups.IsNullOrEmpty();
                if (empty) empty = _hashes.IsNullOrEmpty();
                if (empty) empty = _logs.IsNullOrEmpty();
                if (empty) empty = _messages.IsNullOrEmpty();
                if (empty) empty = _messageQueues.IsNullOrEmpty();
                if (empty) empty = _numbers.IsNullOrEmpty();
                if (empty) empty = _observations.IsNullOrEmpty();
                if (empty) empty = _queues.IsNullOrEmpty();
                if (empty) empty = _references.IsNullOrEmpty();
                if (empty) empty = _sets.IsNullOrEmpty();
                if (empty) empty = _states.IsNullOrEmpty();
                if (empty) empty = _statistics.IsNullOrEmpty();
                if (empty) empty = _strings.IsNullOrEmpty();
                if (empty) empty = _timeRanges.IsNullOrEmpty();
                if (empty) empty = _timestamps.IsNullOrEmpty();
                if (empty) empty = _vocabularies.IsNullOrEmpty();
                if (empty) empty = _vocabularySets.IsNullOrEmpty();
                return empty;
            }
        }


        public TrakHoundObjectCollection(TrakHoundEntityCollection entityCollection)
        {
            _entityCollection = entityCollection;
        }


        public void Add(ITrakHoundEntity entity)
        {
            if (entity != null)
            {
                var type = entity.GetType();

                if (typeof(ITrakHoundObjectEntity).IsAssignableFrom(type))
                    AddObject((ITrakHoundObjectEntity)entity);

                else if (typeof(ITrakHoundObjectMetadataEntity).IsAssignableFrom(type))
                    AddMetadata((ITrakHoundObjectMetadataEntity)entity);

                else if (typeof(ITrakHoundObjectAssignmentEntity).IsAssignableFrom(type))
                    AddAssignment((ITrakHoundObjectAssignmentEntity)entity);

                else if (typeof(ITrakHoundObjectBlobEntity).IsAssignableFrom(type))
                    AddBlob((ITrakHoundObjectBlobEntity)entity);

                else if (typeof(ITrakHoundObjectBooleanEntity).IsAssignableFrom(type))
                    AddBoolean((ITrakHoundObjectBooleanEntity)entity);

                else if (typeof(ITrakHoundObjectDurationEntity).IsAssignableFrom(type))
                    AddDuration((ITrakHoundObjectDurationEntity)entity);

                else if (typeof(ITrakHoundObjectEventEntity).IsAssignableFrom(type))
                    AddEvent((ITrakHoundObjectEventEntity)entity);

                else if (typeof(ITrakHoundObjectGroupEntity).IsAssignableFrom(type))
                    AddGroup((ITrakHoundObjectGroupEntity)entity);

                else if (typeof(ITrakHoundObjectHashEntity).IsAssignableFrom(type))
                    AddHash((ITrakHoundObjectHashEntity)entity);

                else if (typeof(ITrakHoundObjectLogEntity).IsAssignableFrom(type))
                    AddLog((ITrakHoundObjectLogEntity)entity);

                else if (typeof(ITrakHoundObjectMessageEntity).IsAssignableFrom(type))
                    AddMessage((ITrakHoundObjectMessageEntity)entity);

                else if (typeof(ITrakHoundObjectMessageQueueEntity).IsAssignableFrom(type))
                    AddMessageQueue((ITrakHoundObjectMessageQueueEntity)entity);

                else if (typeof(ITrakHoundObjectNumberEntity).IsAssignableFrom(type))
                    AddNumber((ITrakHoundObjectNumberEntity)entity);

                else if (typeof(ITrakHoundObjectObservationEntity).IsAssignableFrom(type))
                    AddObservation((ITrakHoundObjectObservationEntity)entity);

                else if (typeof(ITrakHoundObjectQueueEntity).IsAssignableFrom(type))
                    AddQueue((ITrakHoundObjectQueueEntity)entity);

                else if (typeof(ITrakHoundObjectReferenceEntity).IsAssignableFrom(type))
                    AddReference((ITrakHoundObjectReferenceEntity)entity);

                else if (typeof(ITrakHoundObjectSetEntity).IsAssignableFrom(type))
                    AddSet((ITrakHoundObjectSetEntity)entity);

                else if (typeof(ITrakHoundObjectStateEntity).IsAssignableFrom(type))
                    AddState((ITrakHoundObjectStateEntity)entity);

                else if (typeof(ITrakHoundObjectStatisticEntity).IsAssignableFrom(type))
                    AddStatistic((ITrakHoundObjectStatisticEntity)entity);

                else if (typeof(ITrakHoundObjectStringEntity).IsAssignableFrom(type))
                    AddString((ITrakHoundObjectStringEntity)entity);

                else if (typeof(ITrakHoundObjectTimeRangeEntity).IsAssignableFrom(type))
                    AddTimeRange((ITrakHoundObjectTimeRangeEntity)entity);

                else if (typeof(ITrakHoundObjectTimestampEntity).IsAssignableFrom(type))
                    AddTimestamp((ITrakHoundObjectTimestampEntity)entity);

                else if (typeof(ITrakHoundObjectVocabularyEntity).IsAssignableFrom(type))
                    AddVocabulary((ITrakHoundObjectVocabularyEntity)entity);

                else if (typeof(ITrakHoundObjectVocabularySetEntity).IsAssignableFrom(type))
                    AddVocabularySet((ITrakHoundObjectVocabularySetEntity)entity);

            }
        }

        public IEnumerable<ITrakHoundEntity> GetEntities()
        {   
            var entities = new List<ITrakHoundEntity>();
            if (_objects.Count > 0) entities.AddRange(_objects.Values);
            if (_metadata.Count > 0) entities.AddRange(_metadata.Values);
            if (_assignments.Count > 0) entities.AddRange(_assignments.Values);
            if (_blobs.Count > 0) entities.AddRange(_blobs.Values);
            if (_booleans.Count > 0) entities.AddRange(_booleans.Values);
            if (_durations.Count > 0) entities.AddRange(_durations.Values);
            if (_events.Count > 0) entities.AddRange(_events.Values);
            if (_groups.Count > 0) entities.AddRange(_groups.Values);
            if (_hashes.Count > 0) entities.AddRange(_hashes.Values);
            if (_logs.Count > 0) entities.AddRange(_logs.Values);
            if (_messages.Count > 0) entities.AddRange(_messages.Values);
            if (_messageQueues.Count > 0) entities.AddRange(_messageQueues.Values);
            if (_numbers.Count > 0) entities.AddRange(_numbers.Values);
            if (_observations.Count > 0) entities.AddRange(_observations.Values);
            if (_queues.Count > 0) entities.AddRange(_queues.Values);
            if (_references.Count > 0) entities.AddRange(_references.Values);
            if (_sets.Count > 0) entities.AddRange(_sets.Values);
            if (_states.Count > 0) entities.AddRange(_states.Values);
            if (_statistics.Count > 0) entities.AddRange(_statistics.Values);
            if (_strings.Count > 0) entities.AddRange(_strings.Values);
            if (_timeRanges.Count > 0) entities.AddRange(_timeRanges.Values);
            if (_timestamps.Count > 0) entities.AddRange(_timestamps.Values);
            if (_vocabularies.Count > 0) entities.AddRange(_vocabularies.Values);
            if (_vocabularySets.Count > 0) entities.AddRange(_vocabularySets.Values);
            return entities;
        }

        private IEnumerable<TEntity> GetEntities<TEntity>(IEnumerable<ITrakHoundEntity> entities) where TEntity : ITrakHoundEntity
        {
            if (!entities.IsNullOrEmpty())
            {
                var x = new List<TEntity>();
                foreach (var entity in entities) x.Add((TEntity)entity);
                return x;
            }

            return null;
        }
        
        public IEnumerable<TEntity> GetEntities<TEntity>() where TEntity : ITrakHoundEntity
        {

            if (typeof(ITrakHoundObjectEntity).IsAssignableFrom(typeof(TEntity)))
                return GetEntities<TEntity>(_objects.Values);

            else if (typeof(ITrakHoundObjectMetadataEntity).IsAssignableFrom(typeof(TEntity)))
                return GetEntities<TEntity>(_metadata.Values);

            else if (typeof(ITrakHoundObjectAssignmentEntity).IsAssignableFrom(typeof(TEntity)))
                return GetEntities<TEntity>(_assignments.Values);

            else if (typeof(ITrakHoundObjectBlobEntity).IsAssignableFrom(typeof(TEntity)))
                return GetEntities<TEntity>(_blobs.Values);

            else if (typeof(ITrakHoundObjectBooleanEntity).IsAssignableFrom(typeof(TEntity)))
                return GetEntities<TEntity>(_booleans.Values);

            else if (typeof(ITrakHoundObjectDurationEntity).IsAssignableFrom(typeof(TEntity)))
                return GetEntities<TEntity>(_durations.Values);

            else if (typeof(ITrakHoundObjectEventEntity).IsAssignableFrom(typeof(TEntity)))
                return GetEntities<TEntity>(_events.Values);

            else if (typeof(ITrakHoundObjectGroupEntity).IsAssignableFrom(typeof(TEntity)))
                return GetEntities<TEntity>(_groups.Values);

            else if (typeof(ITrakHoundObjectHashEntity).IsAssignableFrom(typeof(TEntity)))
                return GetEntities<TEntity>(_hashes.Values);

            else if (typeof(ITrakHoundObjectLogEntity).IsAssignableFrom(typeof(TEntity)))
                return GetEntities<TEntity>(_logs.Values);

            else if (typeof(ITrakHoundObjectMessageEntity).IsAssignableFrom(typeof(TEntity)))
                return GetEntities<TEntity>(_messages.Values);

            else if (typeof(ITrakHoundObjectMessageQueueEntity).IsAssignableFrom(typeof(TEntity)))
                return GetEntities<TEntity>(_messageQueues.Values);

            else if (typeof(ITrakHoundObjectNumberEntity).IsAssignableFrom(typeof(TEntity)))
                return GetEntities<TEntity>(_numbers.Values);

            else if (typeof(ITrakHoundObjectObservationEntity).IsAssignableFrom(typeof(TEntity)))
                return GetEntities<TEntity>(_observations.Values);

            else if (typeof(ITrakHoundObjectQueueEntity).IsAssignableFrom(typeof(TEntity)))
                return GetEntities<TEntity>(_queues.Values);

            else if (typeof(ITrakHoundObjectReferenceEntity).IsAssignableFrom(typeof(TEntity)))
                return GetEntities<TEntity>(_references.Values);

            else if (typeof(ITrakHoundObjectSetEntity).IsAssignableFrom(typeof(TEntity)))
                return GetEntities<TEntity>(_sets.Values);

            else if (typeof(ITrakHoundObjectStateEntity).IsAssignableFrom(typeof(TEntity)))
                return GetEntities<TEntity>(_states.Values);

            else if (typeof(ITrakHoundObjectStatisticEntity).IsAssignableFrom(typeof(TEntity)))
                return GetEntities<TEntity>(_statistics.Values);

            else if (typeof(ITrakHoundObjectStringEntity).IsAssignableFrom(typeof(TEntity)))
                return GetEntities<TEntity>(_strings.Values);

            else if (typeof(ITrakHoundObjectTimeRangeEntity).IsAssignableFrom(typeof(TEntity)))
                return GetEntities<TEntity>(_timeRanges.Values);

            else if (typeof(ITrakHoundObjectTimestampEntity).IsAssignableFrom(typeof(TEntity)))
                return GetEntities<TEntity>(_timestamps.Values);

            else if (typeof(ITrakHoundObjectVocabularyEntity).IsAssignableFrom(typeof(TEntity)))
                return GetEntities<TEntity>(_vocabularies.Values);

            else if (typeof(ITrakHoundObjectVocabularySetEntity).IsAssignableFrom(typeof(TEntity)))
                return GetEntities<TEntity>(_vocabularySets.Values);

            return default;
        }

        public ITrakHoundEntity GetEntity(string uuid)
        {
            if (!string.IsNullOrEmpty(uuid))
            {

                ITrakHoundEntity entity = GetObject(uuid);

                if (entity == null) entity = GetMetadata(uuid);

                if (entity == null) entity = GetAssignment(uuid);

                if (entity == null) entity = GetBlob(uuid);

                if (entity == null) entity = GetBoolean(uuid);

                if (entity == null) entity = GetDuration(uuid);

                if (entity == null) entity = GetEvent(uuid);

                if (entity == null) entity = GetGroup(uuid);

                if (entity == null) entity = GetHash(uuid);

                if (entity == null) entity = GetLog(uuid);

                if (entity == null) entity = GetMessage(uuid);

                if (entity == null) entity = GetMessageQueue(uuid);

                if (entity == null) entity = GetNumber(uuid);

                if (entity == null) entity = GetObservation(uuid);

                if (entity == null) entity = GetQueue(uuid);

                if (entity == null) entity = GetReference(uuid);

                if (entity == null) entity = GetSet(uuid);

                if (entity == null) entity = GetState(uuid);

                if (entity == null) entity = GetStatistic(uuid);

                if (entity == null) entity = GetString(uuid);

                if (entity == null) entity = GetTimeRange(uuid);

                if (entity == null) entity = GetTimestamp(uuid);

                if (entity == null) entity = GetVocabulary(uuid);

                if (entity == null) entity = GetVocabularySet(uuid);

                return entity;
            }

            return default;
        }

        public TEntity GetEntity<TEntity>(string uuid) where TEntity : ITrakHoundEntity
        {
            if (!string.IsNullOrEmpty(uuid))
            {

                if (typeof(ITrakHoundObjectEntity).IsAssignableFrom(typeof(TEntity)))
                    return (TEntity)GetObject(uuid);

                else if (typeof(ITrakHoundObjectMetadataEntity).IsAssignableFrom(typeof(TEntity)))
                    return (TEntity)GetMetadata(uuid);

                else if (typeof(ITrakHoundObjectAssignmentEntity).IsAssignableFrom(typeof(TEntity)))
                    return (TEntity)GetAssignment(uuid);

                else if (typeof(ITrakHoundObjectBlobEntity).IsAssignableFrom(typeof(TEntity)))
                    return (TEntity)GetBlob(uuid);

                else if (typeof(ITrakHoundObjectBooleanEntity).IsAssignableFrom(typeof(TEntity)))
                    return (TEntity)GetBoolean(uuid);

                else if (typeof(ITrakHoundObjectDurationEntity).IsAssignableFrom(typeof(TEntity)))
                    return (TEntity)GetDuration(uuid);

                else if (typeof(ITrakHoundObjectEventEntity).IsAssignableFrom(typeof(TEntity)))
                    return (TEntity)GetEvent(uuid);

                else if (typeof(ITrakHoundObjectGroupEntity).IsAssignableFrom(typeof(TEntity)))
                    return (TEntity)GetGroup(uuid);

                else if (typeof(ITrakHoundObjectHashEntity).IsAssignableFrom(typeof(TEntity)))
                    return (TEntity)GetHash(uuid);

                else if (typeof(ITrakHoundObjectLogEntity).IsAssignableFrom(typeof(TEntity)))
                    return (TEntity)GetLog(uuid);

                else if (typeof(ITrakHoundObjectMessageEntity).IsAssignableFrom(typeof(TEntity)))
                    return (TEntity)GetMessage(uuid);

                else if (typeof(ITrakHoundObjectMessageQueueEntity).IsAssignableFrom(typeof(TEntity)))
                    return (TEntity)GetMessageQueue(uuid);

                else if (typeof(ITrakHoundObjectNumberEntity).IsAssignableFrom(typeof(TEntity)))
                    return (TEntity)GetNumber(uuid);

                else if (typeof(ITrakHoundObjectObservationEntity).IsAssignableFrom(typeof(TEntity)))
                    return (TEntity)GetObservation(uuid);

                else if (typeof(ITrakHoundObjectQueueEntity).IsAssignableFrom(typeof(TEntity)))
                    return (TEntity)GetQueue(uuid);

                else if (typeof(ITrakHoundObjectReferenceEntity).IsAssignableFrom(typeof(TEntity)))
                    return (TEntity)GetReference(uuid);

                else if (typeof(ITrakHoundObjectSetEntity).IsAssignableFrom(typeof(TEntity)))
                    return (TEntity)GetSet(uuid);

                else if (typeof(ITrakHoundObjectStateEntity).IsAssignableFrom(typeof(TEntity)))
                    return (TEntity)GetState(uuid);

                else if (typeof(ITrakHoundObjectStatisticEntity).IsAssignableFrom(typeof(TEntity)))
                    return (TEntity)GetStatistic(uuid);

                else if (typeof(ITrakHoundObjectStringEntity).IsAssignableFrom(typeof(TEntity)))
                    return (TEntity)GetString(uuid);

                else if (typeof(ITrakHoundObjectTimeRangeEntity).IsAssignableFrom(typeof(TEntity)))
                    return (TEntity)GetTimeRange(uuid);

                else if (typeof(ITrakHoundObjectTimestampEntity).IsAssignableFrom(typeof(TEntity)))
                    return (TEntity)GetTimestamp(uuid);

                else if (typeof(ITrakHoundObjectVocabularyEntity).IsAssignableFrom(typeof(TEntity)))
                    return (TEntity)GetVocabulary(uuid);

                else if (typeof(ITrakHoundObjectVocabularySetEntity).IsAssignableFrom(typeof(TEntity)))
                    return (TEntity)GetVocabularySet(uuid);

            }

            return default;
        }

        public IEnumerable<ITrakHoundEntity> GetEntityArrays()
        {
            var entities = new List<ITrakHoundEntity>();
            entities.AddRange(_objects.Values);
            entities.AddRange(_metadata.Values);
            entities.AddRange(_assignments.Values);
            entities.AddRange(_blobs.Values);
            entities.AddRange(_booleans.Values);
            entities.AddRange(_durations.Values);
            entities.AddRange(_events.Values);
            entities.AddRange(_groups.Values);
            entities.AddRange(_hashes.Values);
            entities.AddRange(_logs.Values);
            entities.AddRange(_messages.Values);
            entities.AddRange(_messageQueues.Values);
            entities.AddRange(_numbers.Values);
            entities.AddRange(_observations.Values);
            entities.AddRange(_queues.Values);
            entities.AddRange(_references.Values);
            entities.AddRange(_sets.Values);
            entities.AddRange(_states.Values);
            entities.AddRange(_statistics.Values);
            entities.AddRange(_strings.Values);
            entities.AddRange(_timeRanges.Values);
            entities.AddRange(_timestamps.Values);
            entities.AddRange(_vocabularies.Values);
            entities.AddRange(_vocabularySets.Values);
            return entities;
        }

        public void Clear()
        {
            _objects.Clear();
            _metadata.Clear();
            _assignments.Clear();
            _blobs.Clear();
            _booleans.Clear();
            _durations.Clear();
            _events.Clear();
            _groups.Clear();
            _hashes.Clear();
            _logs.Clear();
            _messages.Clear();
            _messageQueues.Clear();
            _numbers.Clear();
            _observations.Clear();
            _queues.Clear();
            _references.Clear();
            _sets.Clear();
            _states.Clear();
            _statistics.Clear();
            _strings.Clear();
            _timeRanges.Clear();
            _timestamps.Clear();
            _vocabularies.Clear();
            _vocabularySets.Clear();
        }

        public partial void OnClear();
    }
}
