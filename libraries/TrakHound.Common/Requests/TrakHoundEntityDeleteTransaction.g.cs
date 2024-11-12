// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TrakHound.Requests
{
    public class TrakHoundEntityDeleteTransaction
    {
        private static readonly TrakHoundUuid.UuidComparer _uuidComparer = new TrakHoundUuid.UuidComparer();
        private readonly Dictionary<byte[], TrakHoundObjectDeleteOperation> _object = new Dictionary<byte[], TrakHoundObjectDeleteOperation>(_uuidComparer);

        private readonly Dictionary<byte[], TrakHoundDefinitionDeleteOperation> _definition = new Dictionary<byte[], TrakHoundDefinitionDeleteOperation>(_uuidComparer);

        private readonly Dictionary<byte[], TrakHoundSourceDeleteOperation> _source = new Dictionary<byte[], TrakHoundSourceDeleteOperation>(_uuidComparer);

        private readonly Dictionary<byte[], TrakHoundAssignmentDeleteOperation> _assignment = new Dictionary<byte[], TrakHoundAssignmentDeleteOperation>(_uuidComparer);

        private readonly Dictionary<byte[], TrakHoundBlobDeleteOperation> _blob = new Dictionary<byte[], TrakHoundBlobDeleteOperation>(_uuidComparer);

        private readonly Dictionary<byte[], TrakHoundBooleanDeleteOperation> _boolean = new Dictionary<byte[], TrakHoundBooleanDeleteOperation>(_uuidComparer);

        private readonly Dictionary<byte[], TrakHoundDurationDeleteOperation> _duration = new Dictionary<byte[], TrakHoundDurationDeleteOperation>(_uuidComparer);

        private readonly Dictionary<byte[], TrakHoundEventDeleteOperation> _event = new Dictionary<byte[], TrakHoundEventDeleteOperation>(_uuidComparer);

        private readonly Dictionary<byte[], TrakHoundGroupDeleteOperation> _group = new Dictionary<byte[], TrakHoundGroupDeleteOperation>(_uuidComparer);

        private readonly Dictionary<byte[], TrakHoundHashDeleteOperation> _hash = new Dictionary<byte[], TrakHoundHashDeleteOperation>(_uuidComparer);

        private readonly Dictionary<byte[], TrakHoundLogDeleteOperation> _log = new Dictionary<byte[], TrakHoundLogDeleteOperation>(_uuidComparer);

        private readonly Dictionary<byte[], TrakHoundNumberDeleteOperation> _number = new Dictionary<byte[], TrakHoundNumberDeleteOperation>(_uuidComparer);

        private readonly Dictionary<byte[], TrakHoundObservationDeleteOperation> _observation = new Dictionary<byte[], TrakHoundObservationDeleteOperation>(_uuidComparer);

        private readonly Dictionary<byte[], TrakHoundQueueDeleteOperation> _queue = new Dictionary<byte[], TrakHoundQueueDeleteOperation>(_uuidComparer);

        private readonly Dictionary<byte[], TrakHoundReferenceDeleteOperation> _reference = new Dictionary<byte[], TrakHoundReferenceDeleteOperation>(_uuidComparer);

        private readonly Dictionary<byte[], TrakHoundSetDeleteOperation> _set = new Dictionary<byte[], TrakHoundSetDeleteOperation>(_uuidComparer);

        private readonly Dictionary<byte[], TrakHoundStateDeleteOperation> _state = new Dictionary<byte[], TrakHoundStateDeleteOperation>(_uuidComparer);

        private readonly Dictionary<byte[], TrakHoundStatisticDeleteOperation> _statistic = new Dictionary<byte[], TrakHoundStatisticDeleteOperation>(_uuidComparer);

        private readonly Dictionary<byte[], TrakHoundStringDeleteOperation> _string = new Dictionary<byte[], TrakHoundStringDeleteOperation>(_uuidComparer);

        private readonly Dictionary<byte[], TrakHoundTimeRangeDeleteOperation> _timerange = new Dictionary<byte[], TrakHoundTimeRangeDeleteOperation>(_uuidComparer);

        private readonly Dictionary<byte[], TrakHoundTimestampDeleteOperation> _timestamp = new Dictionary<byte[], TrakHoundTimestampDeleteOperation>(_uuidComparer);

        private readonly Dictionary<byte[], TrakHoundVocabularyDeleteOperation> _vocabulary = new Dictionary<byte[], TrakHoundVocabularyDeleteOperation>(_uuidComparer);

        private readonly Dictionary<byte[], TrakHoundVocabularySetDeleteOperation> _vocabularyset = new Dictionary<byte[], TrakHoundVocabularySetDeleteOperation>(_uuidComparer);



        [JsonPropertyName("object")]
        public IEnumerable<TrakHoundObjectDeleteOperation> Object
        {
            get => _object.Values;
            set
            {
                if (!value.IsNullOrEmpty())
                {
                    foreach (var x in value)
                    {
                        if (x != null && x.EntryId != null)
                        {
                            _object.Remove(x.EntryId);
                            _object.Add(x.EntryId, x);
                        }
                    }
                }
            }
        }

        [JsonPropertyName("definition")]
        public IEnumerable<TrakHoundDefinitionDeleteOperation> Definition
        {
            get => _definition.Values;
            set
            {
                if (!value.IsNullOrEmpty())
                {
                    foreach (var x in value)
                    {
                        if (x != null && x.EntryId != null)
                        {
                            _definition.Remove(x.EntryId);
                            _definition.Add(x.EntryId, x);
                        }
                    }
                }
            }
        }

        [JsonPropertyName("source")]
        public IEnumerable<TrakHoundSourceDeleteOperation> Source
        {
            get => _source.Values;
            set
            {
                if (!value.IsNullOrEmpty())
                {
                    foreach (var x in value)
                    {
                        if (x != null && x.EntryId != null)
                        {
                            _source.Remove(x.EntryId);
                            _source.Add(x.EntryId, x);
                        }
                    }
                }
            }
        }

        [JsonPropertyName("assignment")]
        public IEnumerable<TrakHoundAssignmentDeleteOperation> Assignment
        {
            get => _assignment.Values;
            set
            {
                if (!value.IsNullOrEmpty())
                {
                    foreach (var x in value)
                    {
                        if (x != null && x.EntryId != null)
                        {
                            _assignment.Remove(x.EntryId);
                            _assignment.Add(x.EntryId, x);
                        }
                    }
                }
            }
        }

        [JsonPropertyName("blob")]
        public IEnumerable<TrakHoundBlobDeleteOperation> Blob
        {
            get => _blob.Values;
            set
            {
                if (!value.IsNullOrEmpty())
                {
                    foreach (var x in value)
                    {
                        if (x != null && x.EntryId != null)
                        {
                            _blob.Remove(x.EntryId);
                            _blob.Add(x.EntryId, x);
                        }
                    }
                }
            }
        }

        [JsonPropertyName("boolean")]
        public IEnumerable<TrakHoundBooleanDeleteOperation> Boolean
        {
            get => _boolean.Values;
            set
            {
                if (!value.IsNullOrEmpty())
                {
                    foreach (var x in value)
                    {
                        if (x != null && x.EntryId != null)
                        {
                            _boolean.Remove(x.EntryId);
                            _boolean.Add(x.EntryId, x);
                        }
                    }
                }
            }
        }

        [JsonPropertyName("duration")]
        public IEnumerable<TrakHoundDurationDeleteOperation> Duration
        {
            get => _duration.Values;
            set
            {
                if (!value.IsNullOrEmpty())
                {
                    foreach (var x in value)
                    {
                        if (x != null && x.EntryId != null)
                        {
                            _duration.Remove(x.EntryId);
                            _duration.Add(x.EntryId, x);
                        }
                    }
                }
            }
        }

        [JsonPropertyName("event")]
        public IEnumerable<TrakHoundEventDeleteOperation> Event
        {
            get => _event.Values;
            set
            {
                if (!value.IsNullOrEmpty())
                {
                    foreach (var x in value)
                    {
                        if (x != null && x.EntryId != null)
                        {
                            _event.Remove(x.EntryId);
                            _event.Add(x.EntryId, x);
                        }
                    }
                }
            }
        }

        [JsonPropertyName("group")]
        public IEnumerable<TrakHoundGroupDeleteOperation> Group
        {
            get => _group.Values;
            set
            {
                if (!value.IsNullOrEmpty())
                {
                    foreach (var x in value)
                    {
                        if (x != null && x.EntryId != null)
                        {
                            _group.Remove(x.EntryId);
                            _group.Add(x.EntryId, x);
                        }
                    }
                }
            }
        }

        [JsonPropertyName("hash")]
        public IEnumerable<TrakHoundHashDeleteOperation> Hash
        {
            get => _hash.Values;
            set
            {
                if (!value.IsNullOrEmpty())
                {
                    foreach (var x in value)
                    {
                        if (x != null && x.EntryId != null)
                        {
                            _hash.Remove(x.EntryId);
                            _hash.Add(x.EntryId, x);
                        }
                    }
                }
            }
        }

        [JsonPropertyName("log")]
        public IEnumerable<TrakHoundLogDeleteOperation> Log
        {
            get => _log.Values;
            set
            {
                if (!value.IsNullOrEmpty())
                {
                    foreach (var x in value)
                    {
                        if (x != null && x.EntryId != null)
                        {
                            _log.Remove(x.EntryId);
                            _log.Add(x.EntryId, x);
                        }
                    }
                }
            }
        }

        [JsonPropertyName("number")]
        public IEnumerable<TrakHoundNumberDeleteOperation> Number
        {
            get => _number.Values;
            set
            {
                if (!value.IsNullOrEmpty())
                {
                    foreach (var x in value)
                    {
                        if (x != null && x.EntryId != null)
                        {
                            _number.Remove(x.EntryId);
                            _number.Add(x.EntryId, x);
                        }
                    }
                }
            }
        }

        [JsonPropertyName("observation")]
        public IEnumerable<TrakHoundObservationDeleteOperation> Observation
        {
            get => _observation.Values;
            set
            {
                if (!value.IsNullOrEmpty())
                {
                    foreach (var x in value)
                    {
                        if (x != null && x.EntryId != null)
                        {
                            _observation.Remove(x.EntryId);
                            _observation.Add(x.EntryId, x);
                        }
                    }
                }
            }
        }

        [JsonPropertyName("queue")]
        public IEnumerable<TrakHoundQueueDeleteOperation> Queue
        {
            get => _queue.Values;
            set
            {
                if (!value.IsNullOrEmpty())
                {
                    foreach (var x in value)
                    {
                        if (x != null && x.EntryId != null)
                        {
                            _queue.Remove(x.EntryId);
                            _queue.Add(x.EntryId, x);
                        }
                    }
                }
            }
        }

        [JsonPropertyName("reference")]
        public IEnumerable<TrakHoundReferenceDeleteOperation> Reference
        {
            get => _reference.Values;
            set
            {
                if (!value.IsNullOrEmpty())
                {
                    foreach (var x in value)
                    {
                        if (x != null && x.EntryId != null)
                        {
                            _reference.Remove(x.EntryId);
                            _reference.Add(x.EntryId, x);
                        }
                    }
                }
            }
        }

        [JsonPropertyName("set")]
        public IEnumerable<TrakHoundSetDeleteOperation> Set
        {
            get => _set.Values;
            set
            {
                if (!value.IsNullOrEmpty())
                {
                    foreach (var x in value)
                    {
                        if (x != null && x.EntryId != null)
                        {
                            _set.Remove(x.EntryId);
                            _set.Add(x.EntryId, x);
                        }
                    }
                }
            }
        }

        [JsonPropertyName("state")]
        public IEnumerable<TrakHoundStateDeleteOperation> State
        {
            get => _state.Values;
            set
            {
                if (!value.IsNullOrEmpty())
                {
                    foreach (var x in value)
                    {
                        if (x != null && x.EntryId != null)
                        {
                            _state.Remove(x.EntryId);
                            _state.Add(x.EntryId, x);
                        }
                    }
                }
            }
        }

        [JsonPropertyName("statistic")]
        public IEnumerable<TrakHoundStatisticDeleteOperation> Statistic
        {
            get => _statistic.Values;
            set
            {
                if (!value.IsNullOrEmpty())
                {
                    foreach (var x in value)
                    {
                        if (x != null && x.EntryId != null)
                        {
                            _statistic.Remove(x.EntryId);
                            _statistic.Add(x.EntryId, x);
                        }
                    }
                }
            }
        }

        [JsonPropertyName("string")]
        public IEnumerable<TrakHoundStringDeleteOperation> String
        {
            get => _string.Values;
            set
            {
                if (!value.IsNullOrEmpty())
                {
                    foreach (var x in value)
                    {
                        if (x != null && x.EntryId != null)
                        {
                            _string.Remove(x.EntryId);
                            _string.Add(x.EntryId, x);
                        }
                    }
                }
            }
        }

        [JsonPropertyName("timerange")]
        public IEnumerable<TrakHoundTimeRangeDeleteOperation> TimeRange
        {
            get => _timerange.Values;
            set
            {
                if (!value.IsNullOrEmpty())
                {
                    foreach (var x in value)
                    {
                        if (x != null && x.EntryId != null)
                        {
                            _timerange.Remove(x.EntryId);
                            _timerange.Add(x.EntryId, x);
                        }
                    }
                }
            }
        }

        [JsonPropertyName("timestamp")]
        public IEnumerable<TrakHoundTimestampDeleteOperation> Timestamp
        {
            get => _timestamp.Values;
            set
            {
                if (!value.IsNullOrEmpty())
                {
                    foreach (var x in value)
                    {
                        if (x != null && x.EntryId != null)
                        {
                            _timestamp.Remove(x.EntryId);
                            _timestamp.Add(x.EntryId, x);
                        }
                    }
                }
            }
        }

        [JsonPropertyName("vocabulary")]
        public IEnumerable<TrakHoundVocabularyDeleteOperation> Vocabulary
        {
            get => _vocabulary.Values;
            set
            {
                if (!value.IsNullOrEmpty())
                {
                    foreach (var x in value)
                    {
                        if (x != null && x.EntryId != null)
                        {
                            _vocabulary.Remove(x.EntryId);
                            _vocabulary.Add(x.EntryId, x);
                        }
                    }
                }
            }
        }

        [JsonPropertyName("vocabularyset")]
        public IEnumerable<TrakHoundVocabularySetDeleteOperation> VocabularySet
        {
            get => _vocabularyset.Values;
            set
            {
                if (!value.IsNullOrEmpty())
                {
                    foreach (var x in value)
                    {
                        if (x != null && x.EntryId != null)
                        {
                            _vocabularyset.Remove(x.EntryId);
                            _vocabularyset.Add(x.EntryId, x);
                        }
                    }
                }
            }
        }



        public IEnumerable<ITrakHoundEntityDeleteOperation> GetAllOperations()
        {
            var operations = new List<ITrakHoundEntityDeleteOperation>();
            if (!Object.IsNullOrEmpty()) foreach (var x in Object) operations.Add(x);

            if (!Definition.IsNullOrEmpty()) foreach (var x in Definition) operations.Add(x);

            if (!Source.IsNullOrEmpty()) foreach (var x in Source) operations.Add(x);

            if (!Assignment.IsNullOrEmpty()) foreach (var x in Assignment) operations.Add(x);

            if (!Blob.IsNullOrEmpty()) foreach (var x in Blob) operations.Add(x);

            if (!Boolean.IsNullOrEmpty()) foreach (var x in Boolean) operations.Add(x);

            if (!Duration.IsNullOrEmpty()) foreach (var x in Duration) operations.Add(x);

            if (!Event.IsNullOrEmpty()) foreach (var x in Event) operations.Add(x);

            if (!Group.IsNullOrEmpty()) foreach (var x in Group) operations.Add(x);

            if (!Hash.IsNullOrEmpty()) foreach (var x in Hash) operations.Add(x);

            if (!Log.IsNullOrEmpty()) foreach (var x in Log) operations.Add(x);

            if (!Number.IsNullOrEmpty()) foreach (var x in Number) operations.Add(x);

            if (!Observation.IsNullOrEmpty()) foreach (var x in Observation) operations.Add(x);

            if (!Queue.IsNullOrEmpty()) foreach (var x in Queue) operations.Add(x);

            if (!Reference.IsNullOrEmpty()) foreach (var x in Reference) operations.Add(x);

            if (!Set.IsNullOrEmpty()) foreach (var x in Set) operations.Add(x);

            if (!State.IsNullOrEmpty()) foreach (var x in State) operations.Add(x);

            if (!Statistic.IsNullOrEmpty()) foreach (var x in Statistic) operations.Add(x);

            if (!String.IsNullOrEmpty()) foreach (var x in String) operations.Add(x);

            if (!TimeRange.IsNullOrEmpty()) foreach (var x in TimeRange) operations.Add(x);

            if (!Timestamp.IsNullOrEmpty()) foreach (var x in Timestamp) operations.Add(x);

            if (!Vocabulary.IsNullOrEmpty()) foreach (var x in Vocabulary) operations.Add(x);

            if (!VocabularySet.IsNullOrEmpty()) foreach (var x in VocabularySet) operations.Add(x);

            return operations;
        }


        public void Add(TrakHoundObjectDeleteOperation operation)
        {
            if (operation != null)
            {
                _object.Remove(operation.EntryId);
                _object.Add(operation.EntryId, operation);
            }
        }

        public void Add(TrakHoundDefinitionDeleteOperation operation)
        {
            if (operation != null)
            {
                _definition.Remove(operation.EntryId);
                _definition.Add(operation.EntryId, operation);
            }
        }

        public void Add(TrakHoundSourceDeleteOperation operation)
        {
            if (operation != null)
            {
                _source.Remove(operation.EntryId);
                _source.Add(operation.EntryId, operation);
            }
        }

        public void Add(TrakHoundAssignmentDeleteOperation operation)
        {
            if (operation != null)
            {
                _assignment.Remove(operation.EntryId);
                _assignment.Add(operation.EntryId, operation);
            }
        }

        public void Add(TrakHoundBlobDeleteOperation operation)
        {
            if (operation != null)
            {
                _blob.Remove(operation.EntryId);
                _blob.Add(operation.EntryId, operation);
            }
        }

        public void Add(TrakHoundBooleanDeleteOperation operation)
        {
            if (operation != null)
            {
                _boolean.Remove(operation.EntryId);
                _boolean.Add(operation.EntryId, operation);
            }
        }

        public void Add(TrakHoundDurationDeleteOperation operation)
        {
            if (operation != null)
            {
                _duration.Remove(operation.EntryId);
                _duration.Add(operation.EntryId, operation);
            }
        }

        public void Add(TrakHoundEventDeleteOperation operation)
        {
            if (operation != null)
            {
                _event.Remove(operation.EntryId);
                _event.Add(operation.EntryId, operation);
            }
        }

        public void Add(TrakHoundGroupDeleteOperation operation)
        {
            if (operation != null)
            {
                _group.Remove(operation.EntryId);
                _group.Add(operation.EntryId, operation);
            }
        }

        public void Add(TrakHoundHashDeleteOperation operation)
        {
            if (operation != null)
            {
                _hash.Remove(operation.EntryId);
                _hash.Add(operation.EntryId, operation);
            }
        }

        public void Add(TrakHoundLogDeleteOperation operation)
        {
            if (operation != null)
            {
                _log.Remove(operation.EntryId);
                _log.Add(operation.EntryId, operation);
            }
        }

        public void Add(TrakHoundNumberDeleteOperation operation)
        {
            if (operation != null)
            {
                _number.Remove(operation.EntryId);
                _number.Add(operation.EntryId, operation);
            }
        }

        public void Add(TrakHoundObservationDeleteOperation operation)
        {
            if (operation != null)
            {
                _observation.Remove(operation.EntryId);
                _observation.Add(operation.EntryId, operation);
            }
        }

        public void Add(TrakHoundQueueDeleteOperation operation)
        {
            if (operation != null)
            {
                _queue.Remove(operation.EntryId);
                _queue.Add(operation.EntryId, operation);
            }
        }

        public void Add(TrakHoundReferenceDeleteOperation operation)
        {
            if (operation != null)
            {
                _reference.Remove(operation.EntryId);
                _reference.Add(operation.EntryId, operation);
            }
        }

        public void Add(TrakHoundSetDeleteOperation operation)
        {
            if (operation != null)
            {
                _set.Remove(operation.EntryId);
                _set.Add(operation.EntryId, operation);
            }
        }

        public void Add(TrakHoundStateDeleteOperation operation)
        {
            if (operation != null)
            {
                _state.Remove(operation.EntryId);
                _state.Add(operation.EntryId, operation);
            }
        }

        public void Add(TrakHoundStatisticDeleteOperation operation)
        {
            if (operation != null)
            {
                _statistic.Remove(operation.EntryId);
                _statistic.Add(operation.EntryId, operation);
            }
        }

        public void Add(TrakHoundStringDeleteOperation operation)
        {
            if (operation != null)
            {
                _string.Remove(operation.EntryId);
                _string.Add(operation.EntryId, operation);
            }
        }

        public void Add(TrakHoundTimeRangeDeleteOperation operation)
        {
            if (operation != null)
            {
                _timerange.Remove(operation.EntryId);
                _timerange.Add(operation.EntryId, operation);
            }
        }

        public void Add(TrakHoundTimestampDeleteOperation operation)
        {
            if (operation != null)
            {
                _timestamp.Remove(operation.EntryId);
                _timestamp.Add(operation.EntryId, operation);
            }
        }

        public void Add(TrakHoundVocabularyDeleteOperation operation)
        {
            if (operation != null)
            {
                _vocabulary.Remove(operation.EntryId);
                _vocabulary.Add(operation.EntryId, operation);
            }
        }

        public void Add(TrakHoundVocabularySetDeleteOperation operation)
        {
            if (operation != null)
            {
                _vocabularyset.Remove(operation.EntryId);
                _vocabularyset.Add(operation.EntryId, operation);
            }
        }



        public void Add(TrakHoundEntityDeleteTransaction transaction)
        {
            if (transaction != null)
            {
                if (!transaction._object.IsNullOrEmpty()) 
                {
                    foreach (var entry in transaction._object)
                    {
                        Add(entry.Value);
                    }
                }
                if (!transaction._definition.IsNullOrEmpty()) 
                {
                    foreach (var entry in transaction._definition)
                    {
                        Add(entry.Value);
                    }
                }
                if (!transaction._source.IsNullOrEmpty()) 
                {
                    foreach (var entry in transaction._source)
                    {
                        Add(entry.Value);
                    }
                }
                if (!transaction._assignment.IsNullOrEmpty()) 
                {
                    foreach (var entry in transaction._assignment)
                    {
                        Add(entry.Value);
                    }
                }
                if (!transaction._blob.IsNullOrEmpty()) 
                {
                    foreach (var entry in transaction._blob)
                    {
                        Add(entry.Value);
                    }
                }
                if (!transaction._boolean.IsNullOrEmpty()) 
                {
                    foreach (var entry in transaction._boolean)
                    {
                        Add(entry.Value);
                    }
                }
                if (!transaction._duration.IsNullOrEmpty()) 
                {
                    foreach (var entry in transaction._duration)
                    {
                        Add(entry.Value);
                    }
                }
                if (!transaction._event.IsNullOrEmpty()) 
                {
                    foreach (var entry in transaction._event)
                    {
                        Add(entry.Value);
                    }
                }
                if (!transaction._group.IsNullOrEmpty()) 
                {
                    foreach (var entry in transaction._group)
                    {
                        Add(entry.Value);
                    }
                }
                if (!transaction._hash.IsNullOrEmpty()) 
                {
                    foreach (var entry in transaction._hash)
                    {
                        Add(entry.Value);
                    }
                }
                if (!transaction._log.IsNullOrEmpty()) 
                {
                    foreach (var entry in transaction._log)
                    {
                        Add(entry.Value);
                    }
                }
                if (!transaction._number.IsNullOrEmpty()) 
                {
                    foreach (var entry in transaction._number)
                    {
                        Add(entry.Value);
                    }
                }
                if (!transaction._observation.IsNullOrEmpty()) 
                {
                    foreach (var entry in transaction._observation)
                    {
                        Add(entry.Value);
                    }
                }
                if (!transaction._queue.IsNullOrEmpty()) 
                {
                    foreach (var entry in transaction._queue)
                    {
                        Add(entry.Value);
                    }
                }
                if (!transaction._reference.IsNullOrEmpty()) 
                {
                    foreach (var entry in transaction._reference)
                    {
                        Add(entry.Value);
                    }
                }
                if (!transaction._set.IsNullOrEmpty()) 
                {
                    foreach (var entry in transaction._set)
                    {
                        Add(entry.Value);
                    }
                }
                if (!transaction._state.IsNullOrEmpty()) 
                {
                    foreach (var entry in transaction._state)
                    {
                        Add(entry.Value);
                    }
                }
                if (!transaction._statistic.IsNullOrEmpty()) 
                {
                    foreach (var entry in transaction._statistic)
                    {
                        Add(entry.Value);
                    }
                }
                if (!transaction._string.IsNullOrEmpty()) 
                {
                    foreach (var entry in transaction._string)
                    {
                        Add(entry.Value);
                    }
                }
                if (!transaction._timerange.IsNullOrEmpty()) 
                {
                    foreach (var entry in transaction._timerange)
                    {
                        Add(entry.Value);
                    }
                }
                if (!transaction._timestamp.IsNullOrEmpty()) 
                {
                    foreach (var entry in transaction._timestamp)
                    {
                        Add(entry.Value);
                    }
                }
                if (!transaction._vocabulary.IsNullOrEmpty()) 
                {
                    foreach (var entry in transaction._vocabulary)
                    {
                        Add(entry.Value);
                    }
                }
                if (!transaction._vocabularyset.IsNullOrEmpty()) 
                {
                    foreach (var entry in transaction._vocabularyset)
                    {
                        Add(entry.Value);
                    }
                }
            }
        }
    }
}
