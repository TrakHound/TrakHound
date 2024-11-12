// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Entities;

namespace TrakHound.Clients
{
    public partial interface ITrakHoundSystemObjectClient : ITrakHoundEntityClient<ITrakHoundObjectEntity>
    {
        ITrakHoundSystemObjectMetadataClient Metadata { get; }
        ITrakHoundSystemObjectAssignmentClient Assignment { get; }
        ITrakHoundSystemObjectBlobClient Blob { get; }
        ITrakHoundSystemObjectBooleanClient Boolean { get; }
        ITrakHoundSystemObjectDurationClient Duration { get; }
        ITrakHoundSystemObjectEventClient Event { get; }
        ITrakHoundSystemObjectGroupClient Group { get; }
        ITrakHoundSystemObjectHashClient Hash { get; }
        ITrakHoundSystemObjectLogClient Log { get; }
        ITrakHoundSystemObjectMessageClient Message { get; }
        ITrakHoundSystemObjectMessageQueueClient MessageQueue { get; }
        ITrakHoundSystemObjectNumberClient Number { get; }
        ITrakHoundSystemObjectObservationClient Observation { get; }
        ITrakHoundSystemObjectQueueClient Queue { get; }
        ITrakHoundSystemObjectReferenceClient Reference { get; }
        ITrakHoundSystemObjectSetClient Set { get; }
        ITrakHoundSystemObjectStateClient State { get; }
        ITrakHoundSystemObjectStatisticClient Statistic { get; }
        ITrakHoundSystemObjectStringClient String { get; }
        ITrakHoundSystemObjectTimeRangeClient TimeRange { get; }
        ITrakHoundSystemObjectTimestampClient Timestamp { get; }
        ITrakHoundSystemObjectVocabularyClient Vocabulary { get; }
        ITrakHoundSystemObjectVocabularySetClient VocabularySet { get; }
    }
}