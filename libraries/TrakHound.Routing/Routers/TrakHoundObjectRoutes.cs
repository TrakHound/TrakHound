// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using TrakHound.Drivers.Entities;
using TrakHound.Entities;

namespace TrakHound.Routing.Routers
{
    public static class TrakHoundObjectRoutes
    {
        // Assignments
        public const string AssignmentsRead = "Objects.Assignments.Read.Absolute";
        public const string AssignmentsCurrent = "Objects.Assignments.Read.Current";
        public const string AssignmentsQuery = "Objects.Assignments.Read.Query";
        public const string AssignmentsSubscribe = "Objects.Assignments.Read.Subscribe";
        public const string AssignmentsPublish = "Objects.Assignments.Write.Publish";
        public const string AssignmentsDelete = "Objects.Assignments.Write.Delete";
        public const string AssignmentsEmpty = "Objects.Assignments.Write.Empty";
        public const string AssignmentsExpire = "Objects.Assignments.Write.Expire";

        // Blobs
        public const string BlobsRead = "Objects.Blobs.Read.Absolute";
        public const string BlobsQuery = "Objects.Blobs.Read.Query";
        public const string BlobsSubscribe = "Objects.Blobs.Read.Subscribe";
        public const string BlobsPublish = "Objects.Blobs.Write.Publish";
        public const string BlobsDelete = "Objects.Blobs.Write.Delete";
        public const string BlobsEmpty = "Objects.Blobs.Write.Empty";
        public const string BlobsExpire = "Objects.Blobs.Write.Expire";

        // Booleans
        public const string BooleansRead = "Objects.Booleans.Read.Absolute";
        public const string BooleansQuery = "Objects.Booleans.Read.Query";
        public const string BooleansSubscribe = "Objects.Booleans.Read.Subscribe";
        public const string BooleansPublish = "Objects.Booleans.Write.Publish";
        public const string BooleansDelete = "Objects.Booleans.Write.Delete";
        public const string BooleansEmpty = "Objects.Booleans.Write.Empty";
        public const string BooleansExpire = "Objects.Booleans.Write.Expire";

        // Durations
        public const string DurationsRead = "Objects.Durations.Read.Absolute";
        public const string DurationsQuery = "Objects.Durations.Read.Query";
        public const string DurationsSubscribe = "Objects.Durations.Read.Subscribe";
        public const string DurationsPublish = "Objects.Durations.Write.Publish";
        public const string DurationsDelete = "Objects.Durations.Write.Delete";
        public const string DurationsEmpty = "Objects.Durations.Write.Empty";
        public const string DurationsExpire = "Objects.Durations.Write.Expire";

        // Events
        public const string EventsRead = "Objects.Events.Read.Absolute";
        public const string EventsLatest = "Objects.Events.Read.Latest";
        public const string EventsQuery = "Objects.Events.Read.Query";
        public const string EventsSubscribe = "Objects.Events.Read.Subscribe";
        public const string EventsPublish = "Objects.Events.Write.Publish";
        public const string EventsDelete = "Objects.Events.Write.Delete";
        public const string EventsEmpty = "Objects.Events.Write.Empty";
        public const string EventsExpire = "Objects.Events.Write.Expire";

        // Groups
        public const string GroupsRead = "Objects.Groups.Read.Absolute";
        public const string GroupsQuery = "Objects.Groups.Read.Query";
        public const string GroupsSubscribe = "Objects.Groups.Read.Subscribe";
        public const string GroupsPublish = "Objects.Groups.Write.Publish";
        public const string GroupsDelete = "Objects.Groups.Write.Delete";
        public const string GroupsEmpty = "Objects.Groups.Write.Empty";
        public const string GroupsExpire = "Objects.Groups.Write.Expire";

        // Hashes
        public const string HashesRead = "Objects.Hashes.Read.Absolute";
        public const string HashesQuery = "Objects.Hashes.Read.Query";
        public const string HashesSubscribe = "Objects.Hashes.Read.Subscribe";
        public const string HashesPublish = "Objects.Hashes.Write.Publish";
        public const string HashesDelete = "Objects.Hashes.Write.Delete";
        public const string HashesEmpty = "Objects.Hashes.Write.Empty";
        public const string HashesExpire = "Objects.Hashes.Write.Expire";

        // Logs
        public const string LogsRead = "Objects.Logs.Read.Absolute";
        public const string LogsQuery = "Objects.Logs.Read.Query";
        public const string LogsSubscribe = "Objects.Logs.Read.Subscribe";
        public const string LogsPublish = "Objects.Logs.Write.Publish";
        public const string LogsDelete = "Objects.Logs.Write.Delete";
        public const string LogsEmpty = "Objects.Logs.Write.Empty";
        public const string LogsExpire = "Objects.Logs.Write.Expire";
        public const string LogsExpireUpdate = "Objects.Logs.Write.Expire.Update";
        public const string LogsExpireAccess = "Objects.Logs.Write.Expire.Access";

        // Messages
        public const string MessagesRead = "Objects.Messages.Read.Absolute";
        public const string MessagesQuery = "Objects.Messages.Read.Query";
        public const string MessagesSubscribe = "Objects.Messages.Read.Subscribe";
        public const string MessagesPublish = "Objects.Messages.Write.Publish";
        public const string MessagesDelete = "Objects.Messages.Write.Delete";
        public const string MessagesEmpty = "Objects.Messages.Write.Empty";
        public const string MessagesExpire = "Objects.Messages.Write.Expire";

        // MessageQueues
        public const string MessageQueuesRead = "Objects.MessageQueues.Read.Absolute";
        public const string MessageQueuesQuery = "Objects.MessageQueues.Read.Query";
        public const string MessageQueuesSubscribe = "Objects.MessageQueues.Read.Subscribe";
        public const string MessageQueuesPublish = "Objects.MessageQueues.Write.Publish";
        public const string MessageQueuesDelete = "Objects.MessageQueues.Write.Delete";
        public const string MessageQueuesEmpty = "Objects.MessageQueues.Write.Empty";
        public const string MessageQueuesExpire = "Objects.MessageQueues.Write.Expire";

        // Metadata
        public const string MetadataRead = "Objects.Metadata.Read.Absolute";
        public const string MetadataQuery = "Objects.Metadata.Read.Query";
        public const string MetadataSubscribe = "Objects.Metadata.Read.Subscribe";
        public const string MetadataPublish = "Objects.Metadata.Write.Publish";
        public const string MetadataDelete = "Objects.Metadata.Write.Delete";
        public const string MetadataEmpty = "Objects.Metadata.Write.Empty";
        public const string MetadataExpire = "Objects.Metadata.Write.Expire";

        // Numbers
        public const string NumbersRead = "Objects.Numbers.Read.Absolute";
        public const string NumbersQuery = "Objects.Numbers.Read.Query";
        public const string NumbersSubscribe = "Objects.Numbers.Read.Subscribe";
        public const string NumbersPublish = "Objects.Numbers.Write.Publish";
        public const string NumbersDelete = "Objects.Numbers.Write.Delete";
        public const string NumbersEmpty = "Objects.Numbers.Write.Empty";
        public const string NumbersExpire = "Objects.Numbers.Write.Expire";

        // Objects
        public const string ObjectsRead = "Objects.Read.Absolute";
        public const string ObjectsQuery = "Objects.Read.Query";
        public const string ObjectsSubscribe = "Objects.Read.Subscribe";
        public const string ObjectsPublish = "Objects.Write.Publish";
        public const string ObjectsStore = "Objects.Write.Store";
        public const string ObjectsDelete = "Objects.Write.Delete";
        public const string ObjectsExpire = "Objects.Write.Expire";
        public const string ObjectsExpireUpdate = "Objects.Write.Expire.Update";
        public const string ObjectsExpireAccess = "Objects.Write.Expire.Access";
        public const string ObjectsIndexQuery = "Objects.Read.Index.Query";
        public const string ObjectsIndexUpdate = "Objects.Write.Index.Update";

        // Observations
        public const string ObservationsRead = "Objects.Observations.Read.Absolute";
        public const string ObservationsLatest = "Objects.Observations.Read.Latest";
        public const string ObservationsQuery = "Objects.Observations.Read.Query";
        public const string ObservationsSubscribe = "Objects.Observations.Read.Subscribe";
        public const string ObservationsSubscribeLatest = "Objects.Observations.Read.Subscribe.Latest";
        public const string ObservationsPublish = "Objects.Observations.Write.Publish";
        public const string ObservationsDelete = "Objects.Observations.Write.Delete";
        public const string ObservationsEmpty = "Objects.Observations.Write.Empty";
        public const string ObservationsExpire = "Objects.Observations.Write.Expire";
        public const string ObservationsExpireUpdate = "Objects.Observations.Write.Expire.Update";
        public const string ObservationsExpireAccess = "Objects.Observations.Write.Expire.Access";

        // Queues
        public const string QueuesRead = "Objects.Queues.Read.Absolute";
        public const string QueuesQuery = "Objects.Queues.Read.Query";
        public const string QueuesSubscribe = "Objects.Queues.Read.Subscribe";
        public const string QueuesPull = "Objects.Queues.Write.Pull";
        public const string QueuesPublish = "Objects.Queues.Write.Publish";
        public const string QueuesDelete = "Objects.Queues.Write.Delete";
        public const string QueuesEmpty = "Objects.Queues.Write.Empty";
        public const string QueuesExpire = "Objects.Queues.Write.Expire";

        // References
        public const string ReferencesRead = "Objects.References.Read.Absolute";
        public const string ReferencesQuery = "Objects.References.Read.Query";
        public const string ReferencesSubscribe = "Objects.References.Read.Subscribe";
        public const string ReferencesPublish = "Objects.References.Write.Publish";
        public const string ReferencesDelete = "Objects.References.Write.Delete";
        public const string ReferencesEmpty = "Objects.References.Write.Empty";
        public const string ReferencesExpire = "Objects.References.Write.Expire";

        // Sets
        public const string SetsRead = "Objects.Sets.Read.Absolute";
        public const string SetsQuery = "Objects.Sets.Read.Query";
        public const string SetsSubscribe = "Objects.Sets.Read.Subscribe";
        public const string SetsPublish = "Objects.Sets.Write.Publish";
        public const string SetsDelete = "Objects.Sets.Write.Delete";
        public const string SetsEmpty = "Objects.Sets.Write.Empty";
        public const string SetsExpire = "Objects.Sets.Write.Expire";

        // States
        public const string StatesRead = "Objects.States.Read.Absolute";
        public const string StatesLatest = "Objects.States.Read.Latest";
        public const string StatesQuery = "Objects.States.Read.Query";
        public const string StatesSubscribe = "Objects.States.Read.Subscribe";
        public const string StatesSubscribeCurrent = "Objects.States.Read.Subscribe.Current";
        public const string StatesPublish = "Objects.States.Write.Publish";
        public const string StatesDelete = "Objects.States.Write.Delete";
        public const string StatesEmpty = "Objects.States.Write.Empty";
        public const string StatesExpire = "Objects.States.Write.Expire";

        // Statistics
        public const string StatisticsRead = "Objects.Statistics.Read.Absolute";
        public const string StatisticsLatest = "Objects.Statistics.Read.Latest";
        public const string StatisticsQuery = "Objects.Statistics.Read.Query";
        public const string StatisticsSubscribe = "Objects.Statistics.Read.Subscribe";
        public const string StatisticsSubscribeLatest = "Objects.Statistics.Read.Subscribe.Latest";
        public const string StatisticsPublish = "Objects.Statistics.Write.Publish";
        public const string StatisticsDelete = "Objects.Statistics.Write.Delete";
        public const string StatisticsEmpty = "Objects.Statistics.Write.Empty";
        public const string StatisticsExpire = "Objects.Statistics.Write.Expire";

        // Strings
        public const string StringsRead = "Objects.Strings.Read.Absolute";
        public const string StringsQuery = "Objects.Strings.Read.Query";
        public const string StringsSubscribe = "Objects.Strings.Read.Subscribe";
        public const string StringsPublish = "Objects.Strings.Write.Publish";
        public const string StringsDelete = "Objects.Strings.Write.Delete";
        public const string StringsEmpty = "Objects.Strings.Write.Empty";
        public const string StringsExpire = "Objects.Strings.Write.Expire";

        // Timestamps
        public const string TimestampsRead = "Objects.Timestamps.Read.Absolute";
        public const string TimestampsQuery = "Objects.Timestamps.Read.Query";
        public const string TimestampsSubscribe = "Objects.Timestamps.Read.Subscribe";
        public const string TimestampsPublish = "Objects.Timestamps.Write.Publish";
        public const string TimestampsDelete = "Objects.Timestamps.Write.Delete";
        public const string TimestampsEmpty = "Objects.Timestamps.Write.Empty";
        public const string TimestampsExpire = "Objects.Timestamps.Write.Expire";

        // TimeRanges
        public const string TimeRangesRead = "Objects.TimeRanges.Read.Absolute";
        public const string TimeRangesQuery = "Objects.TimeRanges.Read.Query";
        public const string TimeRangesSubscribe = "Objects.TimeRanges.Read.Subscribe";
        public const string TimeRangesPublish = "Objects.TimeRanges.Write.Publish";
        public const string TimeRangesDelete = "Objects.TimeRanges.Write.Delete";
        public const string TimeRangesEmpty = "Objects.TimeRanges.Write.Empty";
        public const string TimeRangesExpire = "Objects.TimeRanges.Write.Expire";

        // Vocabularies
        public const string VocabulariesRead = "Objects.Vocabularies.Read.Absolute";
        public const string VocabulariesQuery = "Objects.Vocabularies.Read.Query";
        public const string VocabulariesSubscribe = "Objects.Vocabularies.Read.Subscribe";
        public const string VocabulariesPublish = "Objects.Vocabularies.Write.Publish";
        public const string VocabulariesDelete = "Objects.Vocabularies.Write.Delete";
        public const string VocabulariesEmpty = "Objects.Vocabularies.Write.Empty";
        public const string VocabulariesExpire = "Objects.Vocabularies.Write.Expire";

        // VocabularySets
        public const string VocabularySetsRead = "Objects.VocabularySets.Read.Absolute";
        public const string VocabularySetsQuery = "Objects.VocabularySets.Read.Query";
        public const string VocabularySetsSubscribe = "Objects.VocabularySets.Read.Subscribe";
        public const string VocabularySetsPublish = "Objects.VocabularySets.Write.Publish";
        public const string VocabularySetsDelete = "Objects.VocabularySets.Write.Delete";
        public const string VocabularySetsEmpty = "Objects.VocabularySets.Write.Empty";
        public const string VocabularySetsExpire = "Objects.VocabularySets.Write.Expire";



        public static readonly Dictionary<Type, string> _routes = new Dictionary<Type, string>
        {
            // Assignments
            { typeof(IEntityReadDriver<ITrakHoundObjectAssignmentEntity>), AssignmentsRead },
            { typeof(IObjectAssignmentCurrentDriver), AssignmentsCurrent },
            { typeof(IObjectAssignmentQueryDriver), AssignmentsQuery },
            { typeof(IEntitySubscribeDriver<ITrakHoundObjectAssignmentEntity>), AssignmentsSubscribe },
            { typeof(IEntityPublishDriver<ITrakHoundObjectAssignmentEntity>), AssignmentsPublish },
            { typeof(IEntityDeleteDriver<ITrakHoundObjectAssignmentEntity>), AssignmentsDelete },
            { typeof(IObjectAssignmentDeleteDriver), AssignmentsDelete },
            { typeof(IEntityEmptyDriver<ITrakHoundObjectAssignmentEntity>), AssignmentsEmpty },
            { typeof(IEntityExpirationDriver<ITrakHoundObjectAssignmentEntity>), AssignmentsExpire },

            // Blobs
            { typeof(IEntityReadDriver<ITrakHoundObjectBlobEntity>), BlobsRead },
            { typeof(IObjectBlobQueryDriver), BlobsQuery },
            { typeof(IEntitySubscribeDriver<ITrakHoundObjectBlobEntity>), BlobsSubscribe },
            { typeof(IEntityPublishDriver<ITrakHoundObjectBlobEntity>), BlobsPublish },
            { typeof(IEntityDeleteDriver<ITrakHoundObjectBlobEntity>), BlobsDelete },
            { typeof(IEntityEmptyDriver<ITrakHoundObjectBlobEntity>), BlobsEmpty },
            { typeof(IEntityExpirationDriver<ITrakHoundObjectBlobEntity>), BlobsExpire },

            // Booleans
            { typeof(IEntityReadDriver<ITrakHoundObjectBooleanEntity>), BooleansRead },
            { typeof(IObjectBooleanQueryDriver), BooleansQuery },
            { typeof(IEntitySubscribeDriver<ITrakHoundObjectBooleanEntity>), BooleansSubscribe },
            { typeof(IEntityPublishDriver<ITrakHoundObjectBooleanEntity>), BooleansPublish },
            { typeof(IEntityDeleteDriver<ITrakHoundObjectBooleanEntity>), BooleansDelete },
            { typeof(IEntityEmptyDriver<ITrakHoundObjectBooleanEntity>), BooleansEmpty },
            { typeof(IEntityExpirationDriver<ITrakHoundObjectBooleanEntity>), BooleansExpire },

            // Durations
            { typeof(IEntityReadDriver<ITrakHoundObjectDurationEntity>), DurationsRead },
            { typeof(IObjectDurationQueryDriver), DurationsQuery },
            { typeof(IEntitySubscribeDriver<ITrakHoundObjectDurationEntity>), DurationsSubscribe },
            { typeof(IEntityPublishDriver<ITrakHoundObjectDurationEntity>), DurationsPublish },
            { typeof(IEntityDeleteDriver<ITrakHoundObjectDurationEntity>), DurationsDelete },
            { typeof(IEntityEmptyDriver<ITrakHoundObjectDurationEntity>), DurationsEmpty },
            { typeof(IEntityExpirationDriver<ITrakHoundObjectDurationEntity>), DurationsExpire },

            // Events
            { typeof(IEntityReadDriver<ITrakHoundObjectEventEntity>), EventsRead },
            { typeof(IObjectEventLatestDriver), EventsLatest },
            { typeof(IObjectEventQueryDriver), EventsQuery },
            { typeof(IObjectEventSubscribeDriver), EventsSubscribe },
            { typeof(IEntitySubscribeDriver<ITrakHoundObjectEventEntity>), EventsSubscribe },
            { typeof(IEntityPublishDriver<ITrakHoundObjectEventEntity>), EventsPublish },
            { typeof(IEntityDeleteDriver<ITrakHoundObjectEventEntity>), EventsDelete },
            { typeof(IObjectEventDeleteDriver), EventsDelete },
            { typeof(IEntityEmptyDriver<ITrakHoundObjectEventEntity>), EventsEmpty },
            { typeof(IEntityExpirationDriver<ITrakHoundObjectEventEntity>), EventsExpire },

            // Groups
            { typeof(IEntityReadDriver<ITrakHoundObjectGroupEntity>), GroupsRead },
            { typeof(IEntitySubscribeDriver<ITrakHoundObjectGroupEntity>), GroupsSubscribe },
            { typeof(IEntityPublishDriver<ITrakHoundObjectGroupEntity>), GroupsPublish },
            { typeof(IEntityDeleteDriver<ITrakHoundObjectGroupEntity>), GroupsDelete },
            { typeof(IObjectGroupDeleteDriver), GroupsDelete },
            { typeof(IEntityEmptyDriver<ITrakHoundObjectGroupEntity>), GroupsEmpty },
            { typeof(IEntityExpirationDriver<ITrakHoundObjectGroupEntity>), GroupsExpire },

            // Hashes
            { typeof(IEntityReadDriver<ITrakHoundObjectHashEntity>), HashesRead },
            { typeof(IEntitySubscribeDriver<ITrakHoundObjectHashEntity>), HashesSubscribe },
            { typeof(IEntityPublishDriver<ITrakHoundObjectHashEntity>), HashesPublish },
            { typeof(IEntityDeleteDriver<ITrakHoundObjectHashEntity>), HashesDelete },
            { typeof(IObjectHashDeleteDriver), HashesDelete },
            { typeof(IEntityEmptyDriver<ITrakHoundObjectHashEntity>), HashesEmpty },
            { typeof(IEntityExpirationDriver<ITrakHoundObjectHashEntity>), HashesExpire },

            // Logs
            { typeof(IEntityReadDriver<ITrakHoundObjectLogEntity>), LogsRead },
            { typeof(IEntitySubscribeDriver<ITrakHoundObjectLogEntity>), LogsSubscribe },
            { typeof(IEntityPublishDriver<ITrakHoundObjectLogEntity>), LogsPublish },
            { typeof(IEntityDeleteDriver<ITrakHoundObjectLogEntity>), LogsDelete },
            { typeof(IObjectLogDeleteDriver), LogsDelete },
            { typeof(IEntityEmptyDriver<ITrakHoundObjectLogEntity>), LogsEmpty },
            { typeof(IEntityExpirationDriver<ITrakHoundObjectLogEntity>), LogsExpire },
            { typeof(IEntityExpirationUpdateDriver<ITrakHoundObjectLogEntity>), LogsExpireUpdate },
            { typeof(IEntityExpirationAccessDriver<ITrakHoundObjectLogEntity>), LogsExpireAccess },

            // Messages
            { typeof(IEntityReadDriver<ITrakHoundObjectMessageEntity>), MessagesRead },
            { typeof(IObjectMessageQueryDriver), MessagesQuery },
            { typeof(IEntitySubscribeDriver<ITrakHoundObjectMessageEntity>), MessagesSubscribe },
            { typeof(IEntityPublishDriver<ITrakHoundObjectMessageEntity>), MessagesPublish },
            { typeof(IEntityDeleteDriver<ITrakHoundObjectMessageEntity>), MessagesDelete },
            { typeof(IEntityEmptyDriver<ITrakHoundObjectMessageEntity>), MessagesEmpty },
            { typeof(IEntityExpirationDriver<ITrakHoundObjectMessageEntity>), MessagesExpire },

            // MessageQueues
            { typeof(IEntityReadDriver<ITrakHoundObjectMessageQueueEntity>), MessagesRead },
            { typeof(IObjectMessageQueueQueryDriver), MessagesQuery },
            { typeof(IEntitySubscribeDriver<ITrakHoundObjectMessageQueueEntity>), MessagesSubscribe },
            { typeof(IEntityPublishDriver<ITrakHoundObjectMessageQueueEntity>), MessagesPublish },
            { typeof(IEntityDeleteDriver<ITrakHoundObjectMessageQueueEntity>), MessagesDelete },
            { typeof(IEntityEmptyDriver<ITrakHoundObjectMessageQueueEntity>), MessagesEmpty },
            { typeof(IEntityExpirationDriver<ITrakHoundObjectMessageQueueEntity>), MessagesExpire },

            // Metadata
            { typeof(IEntityReadDriver<ITrakHoundObjectMetadataEntity>), MetadataRead },
            { typeof(IObjectMetadataQueryDriver), MetadataQuery },
            { typeof(IEntitySubscribeDriver<ITrakHoundObjectMetadataEntity>), MetadataSubscribe },
            { typeof(IEntityPublishDriver<ITrakHoundObjectMetadataEntity>), MetadataPublish },
            { typeof(IEntityDeleteDriver<ITrakHoundObjectMetadataEntity>), MetadataDelete },
            { typeof(IObjectMetadataDeleteDriver), MetadataDelete },
            { typeof(IEntityEmptyDriver<ITrakHoundObjectMetadataEntity>), MetadataEmpty },
            { typeof(IEntityExpirationDriver<ITrakHoundObjectMetadataEntity>), MetadataExpire },

            // Numbers
            { typeof(IEntityReadDriver<ITrakHoundObjectNumberEntity>), NumbersRead },
            { typeof(IEntitySubscribeDriver<ITrakHoundObjectNumberEntity>), NumbersSubscribe },
            { typeof(IEntityPublishDriver<ITrakHoundObjectNumberEntity>), NumbersPublish },
            { typeof(IEntityDeleteDriver<ITrakHoundObjectNumberEntity>), NumbersDelete },
            { typeof(IEntityEmptyDriver<ITrakHoundObjectNumberEntity>), NumbersEmpty },
            { typeof(IEntityExpirationDriver<ITrakHoundObjectNumberEntity>), NumbersExpire },

            // Objects
            { typeof(IEntityReadDriver<ITrakHoundObjectEntity>), ObjectsRead },
            { typeof(IObjectQueryDriver), ObjectsQuery },
            { typeof(IEntitySubscribeDriver<ITrakHoundObjectEntity>), ObjectsSubscribe },
            { typeof(IEntityPublishDriver<ITrakHoundObjectEntity>), ObjectsPublish },
            { typeof(IObjectQueryStoreDriver), ObjectsStore },
            { typeof(IEntityDeleteDriver<ITrakHoundObjectEntity>), ObjectsDelete },
            { typeof(IObjectDeleteDriver), ObjectsDelete },
            { typeof(IObjectExpirationDriver), ObjectsExpire },
            { typeof(IObjectExpirationUpdateDriver), ObjectsExpireUpdate },
            { typeof(IObjectExpirationAccessDriver), ObjectsExpireAccess },
            { typeof(IEntityExpirationDriver<ITrakHoundObjectEntity>), ObjectsExpire },
            { typeof(IEntityExpirationUpdateDriver<ITrakHoundObjectEntity>), ObjectsExpireUpdate },
            { typeof(IEntityExpirationAccessDriver<ITrakHoundObjectEntity>), ObjectsExpireAccess },
            { typeof(IEntityIndexQueryDriver<ITrakHoundObjectEntity>), ObjectsIndexQuery },
            { typeof(IEntityIndexUpdateDriver<ITrakHoundObjectEntity>), ObjectsIndexUpdate },

            // Observations
            { typeof(IEntityReadDriver<ITrakHoundObjectObservationEntity>), ObservationsRead },
            { typeof(IObjectObservationLatestDriver), ObservationsLatest },
            { typeof(IObjectObservationQueryDriver), ObservationsQuery },
            { typeof(IEntitySubscribeDriver<ITrakHoundObjectObservationEntity>), ObservationsSubscribe },
            { typeof(IObjectObservationLatestSubscribeDriver), ObservationsSubscribeLatest },
            { typeof(IEntityPublishDriver<ITrakHoundObjectObservationEntity>), ObservationsPublish },
            { typeof(IEntityDeleteDriver<ITrakHoundObjectObservationEntity>), ObservationsDelete },
            { typeof(IObjectObservationDeleteDriver), ObservationsDelete },
            { typeof(IEntityEmptyDriver<ITrakHoundObjectObservationEntity>), ObservationsEmpty },
            { typeof(IEntityExpirationDriver<ITrakHoundObjectObservationEntity>), ObservationsExpire },
            { typeof(IEntityExpirationUpdateDriver<ITrakHoundObjectObservationEntity>), ObservationsExpireUpdate },
            { typeof(IEntityExpirationAccessDriver<ITrakHoundObjectObservationEntity>), ObservationsExpireAccess },

            // Queues
            { typeof(IEntityReadDriver<ITrakHoundObjectQueueEntity>), QueuesRead },
            { typeof(IObjectQueueQueryDriver), QueuesQuery },
            { typeof(IEntitySubscribeDriver<ITrakHoundObjectQueueEntity>), QueuesSubscribe },
            { typeof(IObjectQueuePullDriver), QueuesPull },
            { typeof(IEntityPublishDriver<ITrakHoundObjectQueueEntity>), QueuesPublish },
            { typeof(IEntityDeleteDriver<ITrakHoundObjectQueueEntity>), QueuesDelete },
            { typeof(IEntityEmptyDriver<ITrakHoundObjectQueueEntity>), QueuesEmpty },
            { typeof(IEntityExpirationDriver<ITrakHoundObjectQueueEntity>), QueuesExpire },

            // References
            { typeof(IEntityReadDriver<ITrakHoundObjectReferenceEntity>), ReferencesRead },
            { typeof(IObjectReferenceQueryDriver), ReferencesQuery },
            { typeof(IEntitySubscribeDriver<ITrakHoundObjectReferenceEntity>), ReferencesSubscribe },
            { typeof(IEntityPublishDriver<ITrakHoundObjectReferenceEntity>), ReferencesPublish },
            { typeof(IEntityDeleteDriver<ITrakHoundObjectReferenceEntity>), ReferencesDelete },
            { typeof(IEntityEmptyDriver<ITrakHoundObjectReferenceEntity>), ReferencesEmpty },
            { typeof(IEntityExpirationDriver<ITrakHoundObjectReferenceEntity>), ReferencesExpire },

            // Sets
            { typeof(IEntityReadDriver<ITrakHoundObjectSetEntity>), SetsRead },
            { typeof(IObjectSetQueryDriver), SetsQuery },
            { typeof(IEntitySubscribeDriver<ITrakHoundObjectSetEntity>), SetsSubscribe },
            { typeof(IEntityPublishDriver<ITrakHoundObjectSetEntity>), SetsPublish },
            { typeof(IEntityDeleteDriver<ITrakHoundObjectSetEntity>), SetsDelete },
            { typeof(IObjectSetDeleteDriver), SetsDelete },
            { typeof(IEntityEmptyDriver<ITrakHoundObjectSetEntity>), SetsEmpty },
            { typeof(IEntityExpirationDriver<ITrakHoundObjectSetEntity>), SetsExpire },

            // States
            { typeof(IEntityReadDriver<ITrakHoundObjectStateEntity>), StatesRead },
            { typeof(IObjectStateLatestDriver), StatesLatest },
            { typeof(IObjectStateQueryDriver), StatesQuery },
            { typeof(IObjectStateSubscribeDriver), StatesSubscribe },
            { typeof(IEntitySubscribeDriver<ITrakHoundObjectStateEntity>), StatesSubscribe },
            { typeof(IEntityPublishDriver<ITrakHoundObjectStateEntity>), StatesPublish },
            { typeof(IEntityDeleteDriver<ITrakHoundObjectStateEntity>), StatesDelete },
            { typeof(IEntityEmptyDriver<ITrakHoundObjectStateEntity>), StatesEmpty },
            { typeof(IEntityExpirationDriver<ITrakHoundObjectStateEntity>), StatesExpire },

            // Statistics
            { typeof(IEntityReadDriver<ITrakHoundObjectStatisticEntity>), StatisticsRead },
            { typeof(IObjectStatisticQueryDriver), StatisticsQuery },
            { typeof(IEntitySubscribeDriver<ITrakHoundObjectStatisticEntity>), StatisticsSubscribe },
            { typeof(IEntityPublishDriver<ITrakHoundObjectStatisticEntity>), StatisticsPublish },
            { typeof(IEntityDeleteDriver<ITrakHoundObjectStatisticEntity>), StatisticsDelete },
            { typeof(IObjectStatisticDeleteDriver), StatisticsDelete },
            { typeof(IEntityEmptyDriver<ITrakHoundObjectStatisticEntity>), StatisticsEmpty },
            { typeof(IEntityExpirationDriver<ITrakHoundObjectStatisticEntity>), StatisticsExpire },

            // Strings
            { typeof(IEntityReadDriver<ITrakHoundObjectStringEntity>), StringsRead },
            { typeof(IEntitySubscribeDriver<ITrakHoundObjectStringEntity>), StringsSubscribe },
            { typeof(IEntityPublishDriver<ITrakHoundObjectStringEntity>), StringsPublish },
            { typeof(IEntityDeleteDriver<ITrakHoundObjectStringEntity>), StringsDelete },
            { typeof(IEntityEmptyDriver<ITrakHoundObjectStringEntity>), StringsEmpty },
            { typeof(IEntityExpirationDriver<ITrakHoundObjectStringEntity>), StringsExpire },

            // Timestamps
            { typeof(IEntityReadDriver<ITrakHoundObjectTimestampEntity>), TimestampsRead },
            { typeof(IEntitySubscribeDriver<ITrakHoundObjectTimestampEntity>), TimestampsSubscribe },
            { typeof(IEntityPublishDriver<ITrakHoundObjectTimestampEntity>), TimestampsPublish },
            { typeof(IEntityDeleteDriver<ITrakHoundObjectTimestampEntity>), TimestampsDelete },
            { typeof(IEntityEmptyDriver<ITrakHoundObjectTimestampEntity>), TimestampsEmpty },
            { typeof(IEntityExpirationDriver<ITrakHoundObjectTimestampEntity>), TimestampsExpire },

            // TimeRanges
            { typeof(IEntityReadDriver<ITrakHoundObjectTimeRangeEntity>), TimeRangesRead },
            { typeof(IObjectTimeRangeQueryDriver), TimeRangesQuery },
            { typeof(IEntitySubscribeDriver<ITrakHoundObjectTimeRangeEntity>), TimeRangesSubscribe },
            { typeof(IEntityPublishDriver<ITrakHoundObjectTimeRangeEntity>), TimeRangesPublish },
            { typeof(IEntityDeleteDriver<ITrakHoundObjectTimeRangeEntity>), TimeRangesDelete },
            { typeof(IEntityEmptyDriver<ITrakHoundObjectTimeRangeEntity>), TimeRangesEmpty },
            { typeof(IEntityExpirationDriver<ITrakHoundObjectTimeRangeEntity>), TimeRangesExpire },

            // Vocabularies
            { typeof(IEntityReadDriver<ITrakHoundObjectVocabularyEntity>), VocabulariesRead },
            { typeof(IEntitySubscribeDriver<ITrakHoundObjectVocabularyEntity>), VocabulariesSubscribe },
            { typeof(IEntityPublishDriver<ITrakHoundObjectVocabularyEntity>), VocabulariesPublish },
            { typeof(IEntityDeleteDriver<ITrakHoundObjectVocabularyEntity>), VocabulariesDelete },
            { typeof(IEntityEmptyDriver<ITrakHoundObjectVocabularyEntity>), VocabulariesEmpty },
            { typeof(IEntityExpirationDriver<ITrakHoundObjectVocabularyEntity>), VocabulariesExpire },

            // VocabularySets
            { typeof(IEntityReadDriver<ITrakHoundObjectVocabularySetEntity>), VocabularySetsRead },
            { typeof(IObjectVocabularySetQueryDriver), VocabularySetsQuery },
            { typeof(IEntitySubscribeDriver<ITrakHoundObjectVocabularySetEntity>), VocabularySetsSubscribe },
            { typeof(IEntityPublishDriver<ITrakHoundObjectVocabularySetEntity>), VocabularySetsPublish },
            { typeof(IEntityDeleteDriver<ITrakHoundObjectVocabularySetEntity>), VocabularySetsDelete },
            { typeof(IObjectVocabularySetDeleteDriver), VocabularySetsDelete },
            { typeof(IEntityEmptyDriver<ITrakHoundObjectVocabularySetEntity>), VocabularySetsEmpty },
            { typeof(IEntityExpirationDriver<ITrakHoundObjectVocabularySetEntity>), VocabularySetsExpire }
        };
    }
}
