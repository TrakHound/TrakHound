// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrakHound.Configurations;
using TrakHound.Entities;
using TrakHound.Http;

namespace TrakHound.Clients
{
    internal partial class TrakHoundHttpSystemObjectClient : TrakHoundHttpEntityClientBase<ITrakHoundObjectEntity>, ITrakHoundSystemObjectClient
    {
        private readonly TrakHoundHttpSystemObjectMetadataClient _metadata;
        private readonly TrakHoundHttpSystemObjectAssignmentClient _assignments;
        private readonly TrakHoundHttpSystemObjectBlobClient _blobs;
        private readonly TrakHoundHttpSystemObjectBooleanClient _booleans;
        private readonly TrakHoundHttpSystemObjectDurationClient _durations;
        private readonly TrakHoundHttpSystemObjectEventClient _events;
        private readonly TrakHoundHttpSystemObjectGroupClient _groups;
        private readonly TrakHoundHttpSystemObjectHashClient _hashes;
        private readonly TrakHoundHttpSystemObjectLogClient _logs;
        private readonly TrakHoundHttpSystemObjectMessageClient _messages;
        private readonly TrakHoundHttpSystemObjectMessageQueueClient _messageQueues;
        private readonly TrakHoundHttpSystemObjectNumberClient _numbers;
        private readonly TrakHoundHttpSystemObjectObservationClient _observations;
        private readonly TrakHoundHttpSystemObjectQueueClient _queues;
        private readonly TrakHoundHttpSystemObjectReferenceClient _references;
        private readonly TrakHoundHttpSystemObjectSetClient _sets;
        private readonly TrakHoundHttpSystemObjectStateClient _states;
        private readonly TrakHoundHttpSystemObjectStatisticClient _statistics;
        private readonly TrakHoundHttpSystemObjectStringClient _strings;
        private readonly TrakHoundHttpSystemObjectTimeRangeClient _timeRanges;
        private readonly TrakHoundHttpSystemObjectTimestampClient _timestamps;
        private readonly TrakHoundHttpSystemObjectVocabularyClient _vocabularies;
        private readonly TrakHoundHttpSystemObjectVocabularySetClient _vocabularySets;


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


        public TrakHoundHttpSystemObjectClient(TrakHoundHttpClient baseClient, TrakHoundHttpSystemEntitiesClient entitiesClient) : base (baseClient, entitiesClient) 
        {
            _metadata = new TrakHoundHttpSystemObjectMetadataClient(baseClient, entitiesClient);
            _assignments = new TrakHoundHttpSystemObjectAssignmentClient(baseClient, entitiesClient);
            _blobs = new TrakHoundHttpSystemObjectBlobClient(baseClient, entitiesClient);
            _booleans = new TrakHoundHttpSystemObjectBooleanClient(baseClient, entitiesClient);
            _durations = new TrakHoundHttpSystemObjectDurationClient(baseClient, entitiesClient);
            _events = new TrakHoundHttpSystemObjectEventClient(baseClient, entitiesClient);
            _groups = new TrakHoundHttpSystemObjectGroupClient(baseClient, entitiesClient);
            _hashes = new TrakHoundHttpSystemObjectHashClient(baseClient, entitiesClient);
            _logs = new TrakHoundHttpSystemObjectLogClient(baseClient, entitiesClient);
            _messages = new TrakHoundHttpSystemObjectMessageClient(baseClient, entitiesClient);
            _messageQueues = new TrakHoundHttpSystemObjectMessageQueueClient(baseClient, entitiesClient);
            _numbers = new TrakHoundHttpSystemObjectNumberClient(baseClient, entitiesClient);
            _observations = new TrakHoundHttpSystemObjectObservationClient(baseClient, entitiesClient);
            _queues = new TrakHoundHttpSystemObjectQueueClient(baseClient, entitiesClient);
            _references = new TrakHoundHttpSystemObjectReferenceClient(baseClient, entitiesClient);
            _sets = new TrakHoundHttpSystemObjectSetClient(baseClient, entitiesClient);
            _states = new TrakHoundHttpSystemObjectStateClient(baseClient, entitiesClient);
            _statistics = new TrakHoundHttpSystemObjectStatisticClient(baseClient, entitiesClient);
            _strings = new TrakHoundHttpSystemObjectStringClient(baseClient, entitiesClient);
            _timeRanges = new TrakHoundHttpSystemObjectTimeRangeClient(baseClient, entitiesClient);
            _timestamps = new TrakHoundHttpSystemObjectTimestampClient(baseClient, entitiesClient);
            _vocabularies = new TrakHoundHttpSystemObjectVocabularyClient(baseClient, entitiesClient);
            _vocabularySets = new TrakHoundHttpSystemObjectVocabularySetClient(baseClient, entitiesClient);
        }


    }
}
