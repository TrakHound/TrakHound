// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Routing.Routers
{
    public class ObjectsRouter
    {
        private readonly TrakHoundObjectRouter _objects;
        private readonly TrakHoundObjectMetadataRouter _metadata;
        private readonly TrakHoundObjectAssignmentRouter _assignments;
        private readonly TrakHoundObjectBlobRouter _blobs;
        private readonly TrakHoundObjectBooleanRouter _booleans;
        private readonly TrakHoundObjectDurationRouter _durations;
        private readonly TrakHoundObjectEventRouter _events;
        private readonly TrakHoundObjectGroupRouter _groups;
        private readonly TrakHoundObjectHashRouter _hashes;
        private readonly TrakHoundObjectLogRouter _logs;
        private readonly TrakHoundObjectMessageRouter _messages;
        private readonly TrakHoundObjectMessageQueueRouter _messageQueues;
        private readonly TrakHoundObjectNumberRouter _numbers;
        private readonly TrakHoundObjectObservationRouter _observations;
        private readonly TrakHoundObjectQueueRouter _queues;
        private readonly TrakHoundObjectReferenceRouter _references;
        private readonly TrakHoundObjectSetRouter _sets;
        private readonly TrakHoundObjectStateRouter _states;
        private readonly TrakHoundObjectStatisticRouter _statistics;
        private readonly TrakHoundObjectStringRouter _strings;
        private readonly TrakHoundObjectTimeRangeRouter _timeRanges;
        private readonly TrakHoundObjectTimestampRouter _timestamps;
        private readonly TrakHoundObjectVocabularyRouter _vocabularies;
        private readonly TrakHoundObjectVocabularySetRouter _vocabularySets;



        public TrakHoundObjectRouter Objects => _objects;
        public TrakHoundObjectMetadataRouter Metadata => _metadata;
        public TrakHoundObjectAssignmentRouter Assignments => _assignments;
        public TrakHoundObjectBlobRouter Blobs => _blobs;
        public TrakHoundObjectBooleanRouter Booleans => _booleans;
        public TrakHoundObjectDurationRouter Durations => _durations;
        public TrakHoundObjectEventRouter Events => _events;
        public TrakHoundObjectGroupRouter Groups => _groups;
        public TrakHoundObjectHashRouter Hashes => _hashes;
        public TrakHoundObjectLogRouter Logs => _logs;
        public TrakHoundObjectMessageRouter Messages => _messages;
        public TrakHoundObjectMessageQueueRouter MessageQueues => _messageQueues;
        public TrakHoundObjectNumberRouter Numbers => _numbers;
        public TrakHoundObjectObservationRouter Observations => _observations;
        public TrakHoundObjectQueueRouter Queues => _queues;
        public TrakHoundObjectReferenceRouter References => _references;
        public TrakHoundObjectSetRouter Sets => _sets;
        public TrakHoundObjectStateRouter States => _states;
        public TrakHoundObjectStatisticRouter Statistics => _statistics;
        public TrakHoundObjectStringRouter Strings => _strings;
        public TrakHoundObjectTimeRangeRouter TimeRanges => _timeRanges;
        public TrakHoundObjectTimestampRouter Timestamps => _timestamps;
        public TrakHoundObjectVocabularyRouter Vocabularies => _vocabularies;
        public TrakHoundObjectVocabularySetRouter VocabularySets => _vocabularySets;


        public ObjectsRouter(TrakHoundRouter router)
        {
            _objects = new TrakHoundObjectRouter(router);
            _metadata = new TrakHoundObjectMetadataRouter(router);
            _assignments = new TrakHoundObjectAssignmentRouter(router);
            _blobs = new TrakHoundObjectBlobRouter(router);
            _booleans = new TrakHoundObjectBooleanRouter(router);
            _durations = new TrakHoundObjectDurationRouter(router);
            _events = new TrakHoundObjectEventRouter(router);
            _groups = new TrakHoundObjectGroupRouter(router);
            _hashes = new TrakHoundObjectHashRouter(router);
            _logs = new TrakHoundObjectLogRouter(router);
            _messages = new TrakHoundObjectMessageRouter(router);
            _messageQueues = new TrakHoundObjectMessageQueueRouter(router);
            _numbers = new TrakHoundObjectNumberRouter(router);
            _observations = new TrakHoundObjectObservationRouter(router);
            _queues = new TrakHoundObjectQueueRouter(router);
            _references = new TrakHoundObjectReferenceRouter(router);
            _sets = new TrakHoundObjectSetRouter(router);
            _states = new TrakHoundObjectStateRouter(router);
            _statistics = new TrakHoundObjectStatisticRouter(router);
            _strings = new TrakHoundObjectStringRouter(router);
            _timeRanges = new TrakHoundObjectTimeRangeRouter(router);
            _timestamps = new TrakHoundObjectTimestampRouter(router);
            _vocabularies = new TrakHoundObjectVocabularyRouter(router);
            _vocabularySets = new TrakHoundObjectVocabularySetRouter(router);
        }
    }
}
