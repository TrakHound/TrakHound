// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TrakHound.Requests
{
    public partial class TrakHoundEntityPublishTransaction
    {
        private static readonly TrakHoundUuid.UuidComparer _uuidComparer = new TrakHoundUuid.UuidComparer();
        private readonly Dictionary<byte[], TrakHoundObjectEntry> _object = new Dictionary<byte[], TrakHoundObjectEntry>(_uuidComparer);
        private readonly Dictionary<byte[], TrakHoundDefinitionEntry> _definition = new Dictionary<byte[], TrakHoundDefinitionEntry>(_uuidComparer);
        private readonly Dictionary<byte[], TrakHoundSourceEntry> _source = new Dictionary<byte[], TrakHoundSourceEntry>(_uuidComparer);
        private readonly Dictionary<byte[], TrakHoundAssignmentEntry> _assignment = new Dictionary<byte[], TrakHoundAssignmentEntry>(_uuidComparer);
        private readonly Dictionary<byte[], TrakHoundBlobEntry> _blob = new Dictionary<byte[], TrakHoundBlobEntry>(_uuidComparer);
        private readonly Dictionary<byte[], TrakHoundBooleanEntry> _boolean = new Dictionary<byte[], TrakHoundBooleanEntry>(_uuidComparer);
        private readonly Dictionary<byte[], TrakHoundDurationEntry> _duration = new Dictionary<byte[], TrakHoundDurationEntry>(_uuidComparer);
        private readonly Dictionary<byte[], TrakHoundEventEntry> _event = new Dictionary<byte[], TrakHoundEventEntry>(_uuidComparer);
        private readonly Dictionary<byte[], TrakHoundGroupEntry> _group = new Dictionary<byte[], TrakHoundGroupEntry>(_uuidComparer);
        private readonly Dictionary<byte[], TrakHoundHashEntry> _hash = new Dictionary<byte[], TrakHoundHashEntry>(_uuidComparer);
        private readonly Dictionary<byte[], TrakHoundLogEntry> _log = new Dictionary<byte[], TrakHoundLogEntry>(_uuidComparer);
        private readonly Dictionary<byte[], TrakHoundMessageMappingEntry> _messagemapping = new Dictionary<byte[], TrakHoundMessageMappingEntry>(_uuidComparer);
        private readonly Dictionary<byte[], TrakHoundMessageQueueMappingEntry> _messagequeuemapping = new Dictionary<byte[], TrakHoundMessageQueueMappingEntry>(_uuidComparer);
        private readonly Dictionary<byte[], TrakHoundNumberEntry> _number = new Dictionary<byte[], TrakHoundNumberEntry>(_uuidComparer);
        private readonly Dictionary<byte[], TrakHoundObservationEntry> _observation = new Dictionary<byte[], TrakHoundObservationEntry>(_uuidComparer);
        private readonly Dictionary<byte[], TrakHoundQueueEntry> _queue = new Dictionary<byte[], TrakHoundQueueEntry>(_uuidComparer);
        private readonly Dictionary<byte[], TrakHoundReferenceEntry> _reference = new Dictionary<byte[], TrakHoundReferenceEntry>(_uuidComparer);
        private readonly Dictionary<byte[], TrakHoundSetEntry> _set = new Dictionary<byte[], TrakHoundSetEntry>(_uuidComparer);
        private readonly Dictionary<byte[], TrakHoundStateEntry> _state = new Dictionary<byte[], TrakHoundStateEntry>(_uuidComparer);
        private readonly Dictionary<byte[], TrakHoundStatisticEntry> _statistic = new Dictionary<byte[], TrakHoundStatisticEntry>(_uuidComparer);
        private readonly Dictionary<byte[], TrakHoundStringEntry> _string = new Dictionary<byte[], TrakHoundStringEntry>(_uuidComparer);
        private readonly Dictionary<byte[], TrakHoundTimeRangeEntry> _timerange = new Dictionary<byte[], TrakHoundTimeRangeEntry>(_uuidComparer);
        private readonly Dictionary<byte[], TrakHoundTimestampEntry> _timestamp = new Dictionary<byte[], TrakHoundTimestampEntry>(_uuidComparer);
        private readonly Dictionary<byte[], TrakHoundVocabularyEntry> _vocabulary = new Dictionary<byte[], TrakHoundVocabularyEntry>(_uuidComparer);
        private readonly Dictionary<byte[], TrakHoundVocabularySetEntry> _vocabularyset = new Dictionary<byte[], TrakHoundVocabularySetEntry>(_uuidComparer);


        [JsonPropertyName("definition")]
        public IEnumerable<TrakHoundDefinitionEntry> Definition
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
        public IEnumerable<TrakHoundSourceEntry> Source
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
        public IEnumerable<TrakHoundAssignmentEntry> Assignment
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
        public IEnumerable<TrakHoundBlobEntry> Blob
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
        public IEnumerable<TrakHoundBooleanEntry> Boolean
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
        public IEnumerable<TrakHoundDurationEntry> Duration
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
        public IEnumerable<TrakHoundEventEntry> Event
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
        public IEnumerable<TrakHoundGroupEntry> Group
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
        public IEnumerable<TrakHoundHashEntry> Hash
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
        public IEnumerable<TrakHoundLogEntry> Log
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

        [JsonPropertyName("messagemapping")]
        public IEnumerable<TrakHoundMessageMappingEntry> MessageMapping
        {
            get => _messagemapping.Values;
            set
            {
                if (!value.IsNullOrEmpty())
                {
                    foreach (var x in value)
                    {
                        if (x != null && x.EntryId != null)
                        {
                            _messagemapping.Remove(x.EntryId);
                            _messagemapping.Add(x.EntryId, x);
                        }
                    }
                }
            }
        }

        [JsonPropertyName("messagequeuemapping")]
        public IEnumerable<TrakHoundMessageQueueMappingEntry> MessageQueueMapping
        {
            get => _messagequeuemapping.Values;
            set
            {
                if (!value.IsNullOrEmpty())
                {
                    foreach (var x in value)
                    {
                        if (x != null && x.EntryId != null)
                        {
                            _messagequeuemapping.Remove(x.EntryId);
                            _messagequeuemapping.Add(x.EntryId, x);
                        }
                    }
                }
            }
        }

        [JsonPropertyName("number")]
        public IEnumerable<TrakHoundNumberEntry> Number
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
        public IEnumerable<TrakHoundObservationEntry> Observation
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
        public IEnumerable<TrakHoundQueueEntry> Queue
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
        public IEnumerable<TrakHoundReferenceEntry> Reference
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
        public IEnumerable<TrakHoundSetEntry> Set
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
        public IEnumerable<TrakHoundStateEntry> State
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
        public IEnumerable<TrakHoundStatisticEntry> Statistic
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
        public IEnumerable<TrakHoundStringEntry> String
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
        public IEnumerable<TrakHoundTimeRangeEntry> TimeRange
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
        public IEnumerable<TrakHoundTimestampEntry> Timestamp
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
        public IEnumerable<TrakHoundVocabularyEntry> Vocabulary
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
        public IEnumerable<TrakHoundVocabularySetEntry> VocabularySet
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



        public IEnumerable<ITrakHoundEntityEntryOperation> GetAllOperations()
        {
            var operations = new List<ITrakHoundEntityEntryOperation>();
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
            if (!MessageMapping.IsNullOrEmpty()) foreach (var x in MessageMapping) operations.Add(x);
            if (!MessageQueueMapping.IsNullOrEmpty()) foreach (var x in MessageQueueMapping) operations.Add(x);
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


        public void Add(TrakHoundObjectEntry entry)
        {
            if (entry != null)
            {
                _object.Remove(entry.EntryId);
                _object.Add(entry.EntryId, entry);
            }
        }

        public void Add(TrakHoundDefinitionEntry entry)
        {
            if (entry != null)
            {
                _definition.Remove(entry.EntryId);
                _definition.Add(entry.EntryId, entry);
            }
        }

        public void Add(TrakHoundSourceEntry entry)
        {
            if (entry != null)
            {
                _source.Remove(entry.EntryId);
                _source.Add(entry.EntryId, entry);
            }
        }

        public void Add(TrakHoundAssignmentEntry entry)
        {
            if (entry != null)
            {
                _assignment.Remove(entry.EntryId);
                _assignment.Add(entry.EntryId, entry);
            }
        }

        public void Add(TrakHoundBlobEntry entry)
        {
            if (entry != null)
            {
                _blob.Remove(entry.EntryId);
                _blob.Add(entry.EntryId, entry);
            }
        }

        public void Add(TrakHoundBooleanEntry entry)
        {
            if (entry != null)
            {
                _boolean.Remove(entry.EntryId);
                _boolean.Add(entry.EntryId, entry);
            }
        }

        public void Add(TrakHoundDurationEntry entry)
        {
            if (entry != null)
            {
                _duration.Remove(entry.EntryId);
                _duration.Add(entry.EntryId, entry);
            }
        }

        public void Add(TrakHoundEventEntry entry)
        {
            if (entry != null)
            {
                _event.Remove(entry.EntryId);
                _event.Add(entry.EntryId, entry);
            }
        }

        public void Add(TrakHoundGroupEntry entry)
        {
            if (entry != null)
            {
                _group.Remove(entry.EntryId);
                _group.Add(entry.EntryId, entry);
            }
        }

        public void Add(TrakHoundHashEntry entry)
        {
            if (entry != null)
            {
                _hash.Remove(entry.EntryId);
                _hash.Add(entry.EntryId, entry);
            }
        }

        public void Add(TrakHoundLogEntry entry)
        {
            if (entry != null)
            {
                _log.Remove(entry.EntryId);
                _log.Add(entry.EntryId, entry);
            }
        }

        public void Add(TrakHoundMessageMappingEntry entry)
        {
            if (entry != null)
            {
                _messagemapping.Remove(entry.EntryId);
                _messagemapping.Add(entry.EntryId, entry);
            }
        }

        public void Add(TrakHoundMessageQueueMappingEntry entry)
        {
            if (entry != null)
            {
                _messagequeuemapping.Remove(entry.EntryId);
                _messagequeuemapping.Add(entry.EntryId, entry);
            }
        }

        public void Add(TrakHoundNumberEntry entry)
        {
            if (entry != null)
            {
                _number.Remove(entry.EntryId);
                _number.Add(entry.EntryId, entry);
            }
        }

        public void Add(TrakHoundObservationEntry entry)
        {
            if (entry != null)
            {
                _observation.Remove(entry.EntryId);
                _observation.Add(entry.EntryId, entry);
            }
        }

        public void Add(TrakHoundQueueEntry entry)
        {
            if (entry != null)
            {
                _queue.Remove(entry.EntryId);
                _queue.Add(entry.EntryId, entry);
            }
        }

        public void Add(TrakHoundReferenceEntry entry)
        {
            if (entry != null)
            {
                _reference.Remove(entry.EntryId);
                _reference.Add(entry.EntryId, entry);
            }
        }

        public void Add(TrakHoundSetEntry entry)
        {
            if (entry != null)
            {
                _set.Remove(entry.EntryId);
                _set.Add(entry.EntryId, entry);
            }
        }

        public void Add(TrakHoundStateEntry entry)
        {
            if (entry != null)
            {
                _state.Remove(entry.EntryId);
                _state.Add(entry.EntryId, entry);
            }
        }

        public void Add(TrakHoundStatisticEntry entry)
        {
            if (entry != null)
            {
                _statistic.Remove(entry.EntryId);
                _statistic.Add(entry.EntryId, entry);
            }
        }

        public void Add(TrakHoundStringEntry entry)
        {
            if (entry != null)
            {
                _string.Remove(entry.EntryId);
                _string.Add(entry.EntryId, entry);
            }
        }

        public void Add(TrakHoundTimeRangeEntry entry)
        {
            if (entry != null)
            {
                _timerange.Remove(entry.EntryId);
                _timerange.Add(entry.EntryId, entry);
            }
        }

        public void Add(TrakHoundTimestampEntry entry)
        {
            if (entry != null)
            {
                _timestamp.Remove(entry.EntryId);
                _timestamp.Add(entry.EntryId, entry);
            }
        }

        public void Add(TrakHoundVocabularyEntry entry)
        {
            if (entry != null)
            {
                _vocabulary.Remove(entry.EntryId);
                _vocabulary.Add(entry.EntryId, entry);
            }
        }

        public void Add(TrakHoundVocabularySetEntry entry)
        {
            if (entry != null)
            {
                _vocabularyset.Remove(entry.EntryId);
                _vocabularyset.Add(entry.EntryId, entry);
            }
        }


        public void Add(ITrakHoundEntityEntryOperation operation)
        {
            if (operation != null)
            {
                var type = operation.GetType();

                if (type == typeof(TrakHoundObjectEntry)) Add((TrakHoundObjectEntry)operation);

                if (type == typeof(TrakHoundDefinitionEntry)) Add((TrakHoundDefinitionEntry)operation);

                if (type == typeof(TrakHoundSourceEntry)) Add((TrakHoundSourceEntry)operation);

                if (type == typeof(TrakHoundAssignmentEntry)) Add((TrakHoundAssignmentEntry)operation);

                if (type == typeof(TrakHoundBlobEntry)) Add((TrakHoundBlobEntry)operation);

                if (type == typeof(TrakHoundBooleanEntry)) Add((TrakHoundBooleanEntry)operation);

                if (type == typeof(TrakHoundDurationEntry)) Add((TrakHoundDurationEntry)operation);

                if (type == typeof(TrakHoundEventEntry)) Add((TrakHoundEventEntry)operation);

                if (type == typeof(TrakHoundGroupEntry)) Add((TrakHoundGroupEntry)operation);

                if (type == typeof(TrakHoundHashEntry)) Add((TrakHoundHashEntry)operation);

                if (type == typeof(TrakHoundLogEntry)) Add((TrakHoundLogEntry)operation);

                if (type == typeof(TrakHoundMessageMappingEntry)) Add((TrakHoundMessageMappingEntry)operation);

                if (type == typeof(TrakHoundMessageQueueMappingEntry)) Add((TrakHoundMessageQueueMappingEntry)operation);

                if (type == typeof(TrakHoundNumberEntry)) Add((TrakHoundNumberEntry)operation);

                if (type == typeof(TrakHoundObservationEntry)) Add((TrakHoundObservationEntry)operation);

                if (type == typeof(TrakHoundQueueEntry)) Add((TrakHoundQueueEntry)operation);

                if (type == typeof(TrakHoundReferenceEntry)) Add((TrakHoundReferenceEntry)operation);

                if (type == typeof(TrakHoundSetEntry)) Add((TrakHoundSetEntry)operation);

                if (type == typeof(TrakHoundStateEntry)) Add((TrakHoundStateEntry)operation);

                if (type == typeof(TrakHoundStatisticEntry)) Add((TrakHoundStatisticEntry)operation);

                if (type == typeof(TrakHoundStringEntry)) Add((TrakHoundStringEntry)operation);

                if (type == typeof(TrakHoundTimeRangeEntry)) Add((TrakHoundTimeRangeEntry)operation);

                if (type == typeof(TrakHoundTimestampEntry)) Add((TrakHoundTimestampEntry)operation);

                if (type == typeof(TrakHoundVocabularyEntry)) Add((TrakHoundVocabularyEntry)operation);

                if (type == typeof(TrakHoundVocabularySetEntry)) Add((TrakHoundVocabularySetEntry)operation);

            }
        }

        public void Add(TrakHoundEntityPublishTransaction transaction)
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
                if (!transaction._messagemapping.IsNullOrEmpty()) 
                {
                    foreach (var entry in transaction._messagemapping)
                    {
                        Add(entry.Value);
                    }
                }
                if (!transaction._messagequeuemapping.IsNullOrEmpty()) 
                {
                    foreach (var entry in transaction._messagequeuemapping)
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
