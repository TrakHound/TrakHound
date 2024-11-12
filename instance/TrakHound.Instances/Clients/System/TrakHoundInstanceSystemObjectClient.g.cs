// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrakHound.Entities;
using TrakHound.Routing;

namespace TrakHound.Clients
{
    internal partial class TrakHoundInstanceSystemObjectClient : TrakHoundInstanceEntityClient<ITrakHoundObjectEntity>, ITrakHoundSystemObjectClient
    {
        private readonly TrakHoundInstanceSystemObjectMetadataClient _metadata;
        private readonly TrakHoundInstanceSystemObjectAssignmentClient _assignments;
        private readonly TrakHoundInstanceSystemObjectBlobClient _blobs;
        private readonly TrakHoundInstanceSystemObjectBooleanClient _booleans;
        private readonly TrakHoundInstanceSystemObjectDurationClient _durations;
        private readonly TrakHoundInstanceSystemObjectEventClient _events;
        private readonly TrakHoundInstanceSystemObjectGroupClient _groups;
        private readonly TrakHoundInstanceSystemObjectHashClient _hashes;
        private readonly TrakHoundInstanceSystemObjectLogClient _logs;
        private readonly TrakHoundInstanceSystemObjectMessageClient _messages;
        private readonly TrakHoundInstanceSystemObjectMessageQueueClient _messageQueues;
        private readonly TrakHoundInstanceSystemObjectNumberClient _numbers;
        private readonly TrakHoundInstanceSystemObjectObservationClient _observations;
        private readonly TrakHoundInstanceSystemObjectQueueClient _queues;
        private readonly TrakHoundInstanceSystemObjectReferenceClient _references;
        private readonly TrakHoundInstanceSystemObjectSetClient _sets;
        private readonly TrakHoundInstanceSystemObjectStateClient _states;
        private readonly TrakHoundInstanceSystemObjectStatisticClient _statistics;
        private readonly TrakHoundInstanceSystemObjectStringClient _strings;
        private readonly TrakHoundInstanceSystemObjectTimeRangeClient _timeRanges;
        private readonly TrakHoundInstanceSystemObjectTimestampClient _timestamps;
        private readonly TrakHoundInstanceSystemObjectVocabularyClient _vocabularies;
        private readonly TrakHoundInstanceSystemObjectVocabularySetClient _vocabularySets;
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


        public TrakHoundInstanceSystemObjectClient(TrakHoundInstanceClient baseClient) : base(baseClient) 
        {
            _metadata = new TrakHoundInstanceSystemObjectMetadataClient(baseClient);
            _assignments = new TrakHoundInstanceSystemObjectAssignmentClient(baseClient);
            _blobs = new TrakHoundInstanceSystemObjectBlobClient(baseClient);
            _booleans = new TrakHoundInstanceSystemObjectBooleanClient(baseClient);
            _durations = new TrakHoundInstanceSystemObjectDurationClient(baseClient);
            _events = new TrakHoundInstanceSystemObjectEventClient(baseClient);
            _groups = new TrakHoundInstanceSystemObjectGroupClient(baseClient);
            _hashes = new TrakHoundInstanceSystemObjectHashClient(baseClient);
            _logs = new TrakHoundInstanceSystemObjectLogClient(baseClient);
            _messages = new TrakHoundInstanceSystemObjectMessageClient(baseClient);
            _messageQueues = new TrakHoundInstanceSystemObjectMessageQueueClient(baseClient);
            _numbers = new TrakHoundInstanceSystemObjectNumberClient(baseClient);
            _observations = new TrakHoundInstanceSystemObjectObservationClient(baseClient);
            _queues = new TrakHoundInstanceSystemObjectQueueClient(baseClient);
            _references = new TrakHoundInstanceSystemObjectReferenceClient(baseClient);
            _sets = new TrakHoundInstanceSystemObjectSetClient(baseClient);
            _states = new TrakHoundInstanceSystemObjectStateClient(baseClient);
            _statistics = new TrakHoundInstanceSystemObjectStatisticClient(baseClient);
            _strings = new TrakHoundInstanceSystemObjectStringClient(baseClient);
            _timeRanges = new TrakHoundInstanceSystemObjectTimeRangeClient(baseClient);
            _timestamps = new TrakHoundInstanceSystemObjectTimestampClient(baseClient);
            _vocabularies = new TrakHoundInstanceSystemObjectVocabularyClient(baseClient);
            _vocabularySets = new TrakHoundInstanceSystemObjectVocabularySetClient(baseClient);
        }


    }
}
