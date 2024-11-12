// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrakHound.Entities;
using TrakHound.Entities.Collections;

namespace TrakHound.Clients.Collections
{
    internal partial class TrakHoundCollectionObjectClient : TrakHoundCollectionEntityClient<ITrakHoundObjectEntity>, ITrakHoundSystemObjectClient
    {
        private readonly TrakHoundEntityCollection _collection;

        private readonly TrakHoundCollectionObjectMetadataClient _metadata;
        private readonly TrakHoundCollectionObjectAssignmentClient _assignments;
        private readonly TrakHoundCollectionObjectBlobClient _blobs;
        private readonly TrakHoundCollectionObjectBooleanClient _booleans;
        private readonly TrakHoundCollectionObjectDurationClient _durations;
        private readonly TrakHoundCollectionObjectEventClient _events;
        private readonly TrakHoundCollectionObjectGroupClient _groups;
        private readonly TrakHoundCollectionObjectHashClient _hashes;
        private readonly TrakHoundCollectionObjectLogClient _logs;
        private readonly TrakHoundCollectionObjectMessageClient _messages;
        private readonly TrakHoundCollectionObjectMessageQueueClient _messageQueues;
        private readonly TrakHoundCollectionObjectNumberClient _numbers;
        private readonly TrakHoundCollectionObjectObservationClient _observations;
        private readonly TrakHoundCollectionObjectQueueClient _queues;
        private readonly TrakHoundCollectionObjectReferenceClient _references;
        private readonly TrakHoundCollectionObjectSetClient _sets;
        private readonly TrakHoundCollectionObjectStateClient _states;
        private readonly TrakHoundCollectionObjectStatisticClient _statistics;
        private readonly TrakHoundCollectionObjectStringClient _strings;
        private readonly TrakHoundCollectionObjectTimeRangeClient _timeRanges;
        private readonly TrakHoundCollectionObjectTimestampClient _timestamps;
        private readonly TrakHoundCollectionObjectVocabularyClient _vocabularies;
        private readonly TrakHoundCollectionObjectVocabularySetClient _vocabularySets;



        public ITrakHoundSystemObjectMetadataClient Metadata => _metadata;
        public ITrakHoundSystemObjectAssignmentClient Assignment => _assignments;
        public ITrakHoundSystemObjectBlobClient Blob => _blobs;
        public ITrakHoundSystemObjectBooleanClient Boolean => _booleans;
        public ITrakHoundSystemObjectDurationClient Duration => _durations;
        public ITrakHoundSystemObjectEventClient Event => _events;
        public ITrakHoundSystemObjectGroupClient Group => _groups;
        public ITrakHoundSystemObjectHashClient Hash => _hashes;
        public ITrakHoundSystemObjectLogClient Log => _logs;
        public ITrakHoundSystemObjectMessageClient Message => _messages;
        public ITrakHoundSystemObjectMessageQueueClient MessageQueue => _messageQueues;
        public ITrakHoundSystemObjectNumberClient Number => _numbers;
        public ITrakHoundSystemObjectObservationClient Observation => _observations;
        public ITrakHoundSystemObjectQueueClient Queue => _queues;
        public ITrakHoundSystemObjectReferenceClient Reference => _references;
        public ITrakHoundSystemObjectSetClient Set => _sets;
        public ITrakHoundSystemObjectStateClient State => _states;
        public ITrakHoundSystemObjectStatisticClient Statistic => _statistics;
        public ITrakHoundSystemObjectStringClient String => _strings;
        public ITrakHoundSystemObjectTimeRangeClient TimeRange => _timeRanges;
        public ITrakHoundSystemObjectTimestampClient Timestamp => _timestamps;
        public ITrakHoundSystemObjectVocabularyClient Vocabulary => _vocabularies;
        public ITrakHoundSystemObjectVocabularySetClient VocabularySet => _vocabularySets;


        public TrakHoundCollectionObjectClient(TrakHoundEntityCollection collection) : base(collection) 
        { 
            _collection = collection;


            _metadata = new TrakHoundCollectionObjectMetadataClient(collection);
            _assignments = new TrakHoundCollectionObjectAssignmentClient(collection);
            _blobs = new TrakHoundCollectionObjectBlobClient(collection);
            _booleans = new TrakHoundCollectionObjectBooleanClient(collection);
            _durations = new TrakHoundCollectionObjectDurationClient(collection);
            _events = new TrakHoundCollectionObjectEventClient(collection);
            _groups = new TrakHoundCollectionObjectGroupClient(collection);
            _hashes = new TrakHoundCollectionObjectHashClient(collection);
            _logs = new TrakHoundCollectionObjectLogClient(collection);
            _messages = new TrakHoundCollectionObjectMessageClient(collection);
            _messageQueues = new TrakHoundCollectionObjectMessageQueueClient(collection);
            _numbers = new TrakHoundCollectionObjectNumberClient(collection);
            _observations = new TrakHoundCollectionObjectObservationClient(collection);
            _queues = new TrakHoundCollectionObjectQueueClient(collection);
            _references = new TrakHoundCollectionObjectReferenceClient(collection);
            _sets = new TrakHoundCollectionObjectSetClient(collection);
            _states = new TrakHoundCollectionObjectStateClient(collection);
            _statistics = new TrakHoundCollectionObjectStatisticClient(collection);
            _strings = new TrakHoundCollectionObjectStringClient(collection);
            _timeRanges = new TrakHoundCollectionObjectTimeRangeClient(collection);
            _timestamps = new TrakHoundCollectionObjectTimestampClient(collection);
            _vocabularies = new TrakHoundCollectionObjectVocabularyClient(collection);
            _vocabularySets = new TrakHoundCollectionObjectVocabularySetClient(collection);
        }

}
}
