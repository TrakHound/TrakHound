// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace TrakHound.Entities
{
    public partial struct TrakHoundQueryResult
    {
        public static IEnumerable<ITrakHoundEntity> GetEntities(TrakHoundQueryResult queryResult)
        {
            if (queryResult.Schema != null)
            {
                switch (queryResult.Schema)
                {
                    case "trakhound.entities.objects.target": return GetEntities<ITrakHoundObjectEntity>(queryResult);


                    case "trakhound.entities.objects": return GetEntities<ITrakHoundObjectEntity>(queryResult);
                    case "trakhound.entities.objects.metadata": return GetEntities<ITrakHoundObjectMetadataEntity>(queryResult);
                    case "trakhound.entities.objects.assignment": return GetEntities<ITrakHoundObjectAssignmentEntity>(queryResult);
                    case "trakhound.entities.objects.blob": return GetEntities<ITrakHoundObjectBlobEntity>(queryResult);
                    case "trakhound.entities.objects.boolean": return GetEntities<ITrakHoundObjectBooleanEntity>(queryResult);
                    case "trakhound.entities.objects.duration": return GetEntities<ITrakHoundObjectDurationEntity>(queryResult);
                    case "trakhound.entities.objects.event": return GetEntities<ITrakHoundObjectEventEntity>(queryResult);
                    case "trakhound.entities.objects.group": return GetEntities<ITrakHoundObjectGroupEntity>(queryResult);
                    case "trakhound.entities.objects.hash": return GetEntities<ITrakHoundObjectHashEntity>(queryResult);
                    case "trakhound.entities.objects.log": return GetEntities<ITrakHoundObjectLogEntity>(queryResult);
                    case "trakhound.entities.objects.message": return GetEntities<ITrakHoundObjectMessageEntity>(queryResult);
                    case "trakhound.entities.objects.messagequeue": return GetEntities<ITrakHoundObjectMessageQueueEntity>(queryResult);
                    case "trakhound.entities.objects.number": return GetEntities<ITrakHoundObjectNumberEntity>(queryResult);
                    case "trakhound.entities.objects.observation": return GetEntities<ITrakHoundObjectObservationEntity>(queryResult);
                    case "trakhound.entities.objects.queue": return GetEntities<ITrakHoundObjectQueueEntity>(queryResult);
                    case "trakhound.entities.objects.reference": return GetEntities<ITrakHoundObjectReferenceEntity>(queryResult);
                    case "trakhound.entities.objects.set": return GetEntities<ITrakHoundObjectSetEntity>(queryResult);
                    case "trakhound.entities.objects.state": return GetEntities<ITrakHoundObjectStateEntity>(queryResult);
                    case "trakhound.entities.objects.statistic": return GetEntities<ITrakHoundObjectStatisticEntity>(queryResult);
                    case "trakhound.entities.objects.string": return GetEntities<ITrakHoundObjectStringEntity>(queryResult);
                    case "trakhound.entities.objects.timerange": return GetEntities<ITrakHoundObjectTimeRangeEntity>(queryResult);
                    case "trakhound.entities.objects.timestamp": return GetEntities<ITrakHoundObjectTimestampEntity>(queryResult);
                    case "trakhound.entities.objects.vocabulary": return GetEntities<ITrakHoundObjectVocabularyEntity>(queryResult);
                    case "trakhound.entities.objects.vocabularyset": return GetEntities<ITrakHoundObjectVocabularySetEntity>(queryResult);
                }
            }

            return null;
        }
    }
}
