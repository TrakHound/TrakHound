// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrakHound.Clients;
using TrakHound.Entities.Collections;

namespace TrakHound.Entities.Filters
{
    public partial class TrakHoundEntityPatternFilter
    {
        private string GetContentObjectUuid(ITrakHoundEntity entity)
        {
            if (entity != null)
            {
                switch (entity.Category)
                {
                    case TrakHoundEntityCategoryId.Objects:

                        switch (entity.Class)
                        {
                            case TrakHoundObjectsEntityClassId.Object: return ((ITrakHoundObjectEntity)entity).Uuid;
                            case TrakHoundObjectsEntityClassId.Metadata: return ((ITrakHoundObjectMetadataEntity)entity).EntityUuid;
                            case TrakHoundObjectsEntityClassId.Assignment: return ((ITrakHoundObjectAssignmentEntity)entity).AssigneeUuid;
                            case TrakHoundObjectsEntityClassId.Blob: return ((ITrakHoundObjectBlobEntity)entity).ObjectUuid;
                            case TrakHoundObjectsEntityClassId.Boolean: return ((ITrakHoundObjectBooleanEntity)entity).ObjectUuid;
                            case TrakHoundObjectsEntityClassId.Duration: return ((ITrakHoundObjectDurationEntity)entity).ObjectUuid;
                            case TrakHoundObjectsEntityClassId.Event: return ((ITrakHoundObjectEventEntity)entity).ObjectUuid;
                            case TrakHoundObjectsEntityClassId.Group: return ((ITrakHoundObjectGroupEntity)entity).GroupUuid;
                            case TrakHoundObjectsEntityClassId.Hash: return ((ITrakHoundObjectHashEntity)entity).ObjectUuid;
                            case TrakHoundObjectsEntityClassId.Log: return ((ITrakHoundObjectLogEntity)entity).ObjectUuid;
                            case TrakHoundObjectsEntityClassId.Message: return ((ITrakHoundObjectMessageEntity)entity).ObjectUuid;
                            case TrakHoundObjectsEntityClassId.MessageQueue: return ((ITrakHoundObjectMessageQueueEntity)entity).ObjectUuid;
                            case TrakHoundObjectsEntityClassId.Number: return ((ITrakHoundObjectNumberEntity)entity).ObjectUuid;
                            case TrakHoundObjectsEntityClassId.Observation: return ((ITrakHoundObjectObservationEntity)entity).ObjectUuid;
                            case TrakHoundObjectsEntityClassId.Queue: return ((ITrakHoundObjectQueueEntity)entity).QueueUuid;
                            case TrakHoundObjectsEntityClassId.Reference: return ((ITrakHoundObjectReferenceEntity)entity).ObjectUuid;
                            case TrakHoundObjectsEntityClassId.Set: return ((ITrakHoundObjectSetEntity)entity).ObjectUuid;
                            case TrakHoundObjectsEntityClassId.State: return ((ITrakHoundObjectStateEntity)entity).ObjectUuid;
                            case TrakHoundObjectsEntityClassId.Statistic: return ((ITrakHoundObjectStatisticEntity)entity).ObjectUuid;
                            case TrakHoundObjectsEntityClassId.String: return ((ITrakHoundObjectStringEntity)entity).ObjectUuid;
                            case TrakHoundObjectsEntityClassId.TimeRange: return ((ITrakHoundObjectTimeRangeEntity)entity).ObjectUuid;
                            case TrakHoundObjectsEntityClassId.Timestamp: return ((ITrakHoundObjectTimestampEntity)entity).ObjectUuid;
                            case TrakHoundObjectsEntityClassId.Vocabulary: return ((ITrakHoundObjectVocabularyEntity)entity).ObjectUuid;
                            case TrakHoundObjectsEntityClassId.VocabularySet: return ((ITrakHoundObjectVocabularySetEntity)entity).ObjectUuid;
                        }
                        break;
                }
            }

            return null;
        }
    }
}
