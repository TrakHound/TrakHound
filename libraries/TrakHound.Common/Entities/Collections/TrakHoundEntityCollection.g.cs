// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;

namespace TrakHound.Entities.Collections
{
    public partial class TrakHoundEntityCollection
    {
        public void AddTarget(ITrakHoundEntity targetEntity)
        {
            if (targetEntity != null)
            {
                string targetUuid = null;

                switch (targetEntity.Category)
                {
                    case TrakHoundEntityCategoryId.Objects:

                        switch (targetEntity.Class)
                        {

                            case TrakHoundObjectsEntityClassId.Object:
                                targetUuid = ((ITrakHoundObjectEntity)targetEntity).Uuid;
                                break;
                            case TrakHoundObjectsEntityClassId.Metadata:
                                targetUuid = ((ITrakHoundObjectMetadataEntity)targetEntity).EntityUuid;
                                break;
                            case TrakHoundObjectsEntityClassId.Assignment:
                                targetUuid = ((ITrakHoundObjectAssignmentEntity)targetEntity).AssigneeUuid;
                                break;
                            case TrakHoundObjectsEntityClassId.Blob:
                                targetUuid = ((ITrakHoundObjectBlobEntity)targetEntity).ObjectUuid;
                                break;
                            case TrakHoundObjectsEntityClassId.Boolean:
                                targetUuid = ((ITrakHoundObjectBooleanEntity)targetEntity).ObjectUuid;
                                break;
                            case TrakHoundObjectsEntityClassId.Duration:
                                targetUuid = ((ITrakHoundObjectDurationEntity)targetEntity).ObjectUuid;
                                break;
                            case TrakHoundObjectsEntityClassId.Event:
                                targetUuid = ((ITrakHoundObjectEventEntity)targetEntity).ObjectUuid;
                                break;
                            case TrakHoundObjectsEntityClassId.Group:
                                targetUuid = ((ITrakHoundObjectGroupEntity)targetEntity).GroupUuid;
                                break;
                            case TrakHoundObjectsEntityClassId.Hash:
                                targetUuid = ((ITrakHoundObjectHashEntity)targetEntity).ObjectUuid;
                                break;
                            case TrakHoundObjectsEntityClassId.Log:
                                targetUuid = ((ITrakHoundObjectLogEntity)targetEntity).ObjectUuid;
                                break;
                            case TrakHoundObjectsEntityClassId.Message:
                                targetUuid = ((ITrakHoundObjectMessageEntity)targetEntity).ObjectUuid;
                                break;
                            case TrakHoundObjectsEntityClassId.MessageQueue:
                                targetUuid = ((ITrakHoundObjectMessageQueueEntity)targetEntity).ObjectUuid;
                                break;
                            case TrakHoundObjectsEntityClassId.Number:
                                targetUuid = ((ITrakHoundObjectNumberEntity)targetEntity).ObjectUuid;
                                break;
                            case TrakHoundObjectsEntityClassId.Observation:
                                targetUuid = ((ITrakHoundObjectObservationEntity)targetEntity).ObjectUuid;
                                break;
                            case TrakHoundObjectsEntityClassId.Queue:
                                targetUuid = ((ITrakHoundObjectQueueEntity)targetEntity).QueueUuid;
                                break;
                            case TrakHoundObjectsEntityClassId.Reference:
                                targetUuid = ((ITrakHoundObjectReferenceEntity)targetEntity).ObjectUuid;
                                break;
                            case TrakHoundObjectsEntityClassId.Set:
                                targetUuid = ((ITrakHoundObjectSetEntity)targetEntity).ObjectUuid;
                                break;
                            case TrakHoundObjectsEntityClassId.State:
                                targetUuid = ((ITrakHoundObjectStateEntity)targetEntity).ObjectUuid;
                                break;
                            case TrakHoundObjectsEntityClassId.Statistic:
                                targetUuid = ((ITrakHoundObjectStatisticEntity)targetEntity).ObjectUuid;
                                break;
                            case TrakHoundObjectsEntityClassId.String:
                                targetUuid = ((ITrakHoundObjectStringEntity)targetEntity).ObjectUuid;
                                break;
                            case TrakHoundObjectsEntityClassId.TimeRange:
                                targetUuid = ((ITrakHoundObjectTimeRangeEntity)targetEntity).ObjectUuid;
                                break;
                            case TrakHoundObjectsEntityClassId.Timestamp:
                                targetUuid = ((ITrakHoundObjectTimestampEntity)targetEntity).ObjectUuid;
                                break;
                            case TrakHoundObjectsEntityClassId.Vocabulary:
                                targetUuid = ((ITrakHoundObjectVocabularyEntity)targetEntity).ObjectUuid;
                                break;
                            case TrakHoundObjectsEntityClassId.VocabularySet:
                                targetUuid = ((ITrakHoundObjectVocabularySetEntity)targetEntity).ObjectUuid;
                                break;
                        }

                        break;

                    case TrakHoundEntityCategoryId.Definitions:

                        switch (targetEntity.Class)
                        {

                        }

                        break;

                    case TrakHoundEntityCategoryId.Sources:

                        switch (targetEntity.Class)
                        {

                            case TrakHoundObjectsEntityClassId.Object:
                                targetUuid = ((ITrakHoundObjectEntity)targetEntity).Uuid;
                                break;
                            case TrakHoundObjectsEntityClassId.Metadata:
                                targetUuid = ((ITrakHoundObjectMetadataEntity)targetEntity).EntityUuid;
                                break;
                            case TrakHoundObjectsEntityClassId.Assignment:
                                targetUuid = ((ITrakHoundObjectAssignmentEntity)targetEntity).AssigneeUuid;
                                break;
                            case TrakHoundObjectsEntityClassId.Blob:
                                targetUuid = ((ITrakHoundObjectBlobEntity)targetEntity).ObjectUuid;
                                break;
                            case TrakHoundObjectsEntityClassId.Boolean:
                                targetUuid = ((ITrakHoundObjectBooleanEntity)targetEntity).ObjectUuid;
                                break;
                            case TrakHoundObjectsEntityClassId.Duration:
                                targetUuid = ((ITrakHoundObjectDurationEntity)targetEntity).ObjectUuid;
                                break;
                            case TrakHoundObjectsEntityClassId.Event:
                                targetUuid = ((ITrakHoundObjectEventEntity)targetEntity).ObjectUuid;
                                break;
                            case TrakHoundObjectsEntityClassId.Group:
                                targetUuid = ((ITrakHoundObjectGroupEntity)targetEntity).GroupUuid;
                                break;
                            case TrakHoundObjectsEntityClassId.Hash:
                                targetUuid = ((ITrakHoundObjectHashEntity)targetEntity).ObjectUuid;
                                break;
                            case TrakHoundObjectsEntityClassId.Log:
                                targetUuid = ((ITrakHoundObjectLogEntity)targetEntity).ObjectUuid;
                                break;
                            case TrakHoundObjectsEntityClassId.Message:
                                targetUuid = ((ITrakHoundObjectMessageEntity)targetEntity).ObjectUuid;
                                break;
                            case TrakHoundObjectsEntityClassId.MessageQueue:
                                targetUuid = ((ITrakHoundObjectMessageQueueEntity)targetEntity).ObjectUuid;
                                break;
                            case TrakHoundObjectsEntityClassId.Number:
                                targetUuid = ((ITrakHoundObjectNumberEntity)targetEntity).ObjectUuid;
                                break;
                            case TrakHoundObjectsEntityClassId.Observation:
                                targetUuid = ((ITrakHoundObjectObservationEntity)targetEntity).ObjectUuid;
                                break;
                            case TrakHoundObjectsEntityClassId.Queue:
                                targetUuid = ((ITrakHoundObjectQueueEntity)targetEntity).QueueUuid;
                                break;
                            case TrakHoundObjectsEntityClassId.Reference:
                                targetUuid = ((ITrakHoundObjectReferenceEntity)targetEntity).ObjectUuid;
                                break;
                            case TrakHoundObjectsEntityClassId.Set:
                                targetUuid = ((ITrakHoundObjectSetEntity)targetEntity).ObjectUuid;
                                break;
                            case TrakHoundObjectsEntityClassId.State:
                                targetUuid = ((ITrakHoundObjectStateEntity)targetEntity).ObjectUuid;
                                break;
                            case TrakHoundObjectsEntityClassId.Statistic:
                                targetUuid = ((ITrakHoundObjectStatisticEntity)targetEntity).ObjectUuid;
                                break;
                            case TrakHoundObjectsEntityClassId.String:
                                targetUuid = ((ITrakHoundObjectStringEntity)targetEntity).ObjectUuid;
                                break;
                            case TrakHoundObjectsEntityClassId.TimeRange:
                                targetUuid = ((ITrakHoundObjectTimeRangeEntity)targetEntity).ObjectUuid;
                                break;
                            case TrakHoundObjectsEntityClassId.Timestamp:
                                targetUuid = ((ITrakHoundObjectTimestampEntity)targetEntity).ObjectUuid;
                                break;
                            case TrakHoundObjectsEntityClassId.Vocabulary:
                                targetUuid = ((ITrakHoundObjectVocabularyEntity)targetEntity).ObjectUuid;
                                break;
                            case TrakHoundObjectsEntityClassId.VocabularySet:
                                targetUuid = ((ITrakHoundObjectVocabularySetEntity)targetEntity).ObjectUuid;
                                break;
                        }

                        break;
                }


                if (targetUuid != null && !_targetUuids.Contains(targetUuid))
                {
                    _targetUuids.Add(targetUuid);
                }
            }
        }
    }
}
