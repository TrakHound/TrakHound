// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Entities.Filters
{
    public class TrakHoundEntityOnChangeFilter : TrakHoundOnChangeFilter
    {
        public bool Filter(ITrakHoundEntity entity)
        {
            if (entity != null)
            {
                switch (entity.Category)
                {
                    case TrakHoundEntityCategoryId.Sources:

                        switch (entity.Class)
                        {
                            case TrakHoundSourcesEntityClassId.Source: return Filter((ITrakHoundSourceEntity)entity);
                            case TrakHoundSourcesEntityClassId.Metadata: return Filter((ITrakHoundSourceMetadataEntity)entity);
                        }
                        break;

                    case TrakHoundEntityCategoryId.Definitions:

                        switch (entity.Class)
                        {
                            case TrakHoundDefinitionsEntityClassId.Definition: return Filter((ITrakHoundDefinitionEntity)entity);
                            case TrakHoundDefinitionsEntityClassId.Metadata: return Filter((ITrakHoundDefinitionMetadataEntity)entity);
                            case TrakHoundDefinitionsEntityClassId.Description: return Filter((ITrakHoundDefinitionDescriptionEntity)entity);
                            case TrakHoundDefinitionsEntityClassId.Wiki: return Filter((ITrakHoundDefinitionWikiEntity)entity);
                        }

                        break;

                    case TrakHoundEntityCategoryId.Objects:

                        switch (entity.Class)
                        {
                            case TrakHoundObjectsEntityClassId.Assignment: return Filter((ITrakHoundObjectAssignmentEntity)entity);
                            case TrakHoundObjectsEntityClassId.Blob: return true;
                            case TrakHoundObjectsEntityClassId.Boolean: return Filter((ITrakHoundObjectBooleanEntity)entity);
                            case TrakHoundObjectsEntityClassId.Duration: return Filter((ITrakHoundObjectDurationEntity)entity);
                            case TrakHoundObjectsEntityClassId.Event: return Filter((ITrakHoundObjectEventEntity)entity);
                            case TrakHoundObjectsEntityClassId.Group: return Filter((ITrakHoundObjectGroupEntity)entity);
                            case TrakHoundObjectsEntityClassId.Hash: return Filter((ITrakHoundObjectHashEntity)entity);
                            case TrakHoundObjectsEntityClassId.Log: return true;
                            case TrakHoundObjectsEntityClassId.Message: return true;
                            case TrakHoundObjectsEntityClassId.Metadata: return Filter((ITrakHoundObjectMetadataEntity)entity);
                            case TrakHoundObjectsEntityClassId.Number: return Filter((ITrakHoundObjectNumberEntity)entity);
                            case TrakHoundObjectsEntityClassId.Object: return Filter((ITrakHoundObjectEntity)entity);
                            case TrakHoundObjectsEntityClassId.Observation: return Filter((ITrakHoundObjectObservationEntity)entity);                        
                            case TrakHoundObjectsEntityClassId.Queue: return Filter((ITrakHoundObjectQueueEntity)entity);                        
                            case TrakHoundObjectsEntityClassId.Reference: return Filter((ITrakHoundObjectReferenceEntity)entity);                        
                            case TrakHoundObjectsEntityClassId.Set: return Filter((ITrakHoundObjectSetEntity)entity);                        
                            case TrakHoundObjectsEntityClassId.MessageQueue: return true;                        
                            case TrakHoundObjectsEntityClassId.State: return Filter((ITrakHoundObjectStateEntity)entity);                        
                            case TrakHoundObjectsEntityClassId.Statistic: return Filter((ITrakHoundObjectStatisticEntity)entity);                        
                            case TrakHoundObjectsEntityClassId.String: return Filter((ITrakHoundObjectStringEntity)entity);                        
                            case TrakHoundObjectsEntityClassId.TimeRange: return Filter((ITrakHoundObjectTimeRangeEntity)entity);                        
                            case TrakHoundObjectsEntityClassId.Timestamp: return Filter((ITrakHoundObjectTimestampEntity)entity);                        
                            case TrakHoundObjectsEntityClassId.Vocabulary: return Filter((ITrakHoundObjectVocabularyEntity)entity);                        
                            case TrakHoundObjectsEntityClassId.VocabularySet: return Filter((ITrakHoundObjectVocabularySetEntity)entity);                       
                        }

                        break;
                }
            }

            return false;
        }


        public bool Filter(ITrakHoundDefinitionEntity entity)
        {
            if (entity != null)
            {
                return Filter(CreateKey("Definition", entity.Uuid), CreateKey(entity.Id, entity.ParentUuid));
            }

            return false;
        }

        public bool Filter(ITrakHoundDefinitionMetadataEntity entity)
        {
            if (entity != null)
            {
                return Filter(CreateKey("Definition.Metadata", entity.Uuid, entity.Name), CreateKey(entity.Value));
            }

            return false;
        }

        public bool Filter(ITrakHoundDefinitionDescriptionEntity entity)
        {
            if (entity != null)
            {
                return Filter(CreateKey("Definition.Description", entity.DefinitionUuid, entity.LanguageCode), CreateKey(entity.Text));
            }

            return false;
        }

        public bool Filter(ITrakHoundDefinitionWikiEntity entity)
        {
            if (entity != null)
            {
                return Filter(CreateKey("Definition.Wiki", entity.DefinitionUuid, entity.Section), CreateKey(entity.Text));
            }

            return false;
        }



        public bool Filter(ITrakHoundObjectEntity entity)
        {
            if (entity != null)
            {
                return Filter(CreateKey("Object", entity.Uuid), CreateKey(entity.Path, entity.ContentType, entity.DefinitionUuid, entity.ParentUuid));
            }

            return false;
        }

        public bool Filter(ITrakHoundObjectAssignmentEntity entity)
        {
            if (entity != null)
            {
                return Filter(CreateKey("Object.Assignment.Assignee", entity.AssigneeUuid), CreateKey(entity.MemberUuid, entity.AddTimestamp.ToString(), entity.RemoveTimestamp.ToString()));
            }

            return false;
        }

        public bool Filter(ITrakHoundObjectBooleanEntity entity)
        {
            if (entity != null)
            {
                return Filter(CreateKey("Object.Boolean", entity.ObjectUuid), entity.Value.ToString());
            }

            return false;
        }

        public bool Filter(ITrakHoundObjectDurationEntity entity)
        {
            if (entity != null)
            {
                return Filter(CreateKey("Object.Duration", entity.ObjectUuid), entity.Value.ToString());
            }

            return false;
        }

        public bool Filter(ITrakHoundObjectEventEntity entity)
        {
            if (entity != null)
            {
                return Filter(CreateKey("Object.Event", entity.ObjectUuid), entity.TargetUuid);
            }

            return false;
        }

        public bool Filter(ITrakHoundObjectGroupEntity entity)
        {
            if (entity != null)
            {
                return Filter(CreateKey("Object.Group", entity.GroupUuid), entity.MemberUuid);
            }

            return false;
        }

        public bool Filter(ITrakHoundObjectHashEntity entity)
        {
            if (entity != null)
            {
                return Filter(CreateKey("Object.Hash", entity.ObjectUuid, entity.Key), entity.Value);
            }

            return false;
        }

        public bool Filter(ITrakHoundObjectMetadataEntity entity)
        {
            if (entity != null)
            {
                return Filter(CreateKey("Object.Metadata", entity.Uuid, entity.Name), CreateKey(entity.DefinitionUuid, entity.Value, entity.ValueDefinitionUuid));
            }

            return false;
        }

        public bool Filter(ITrakHoundObjectNumberEntity entity)
        {
            if (entity != null)
            {
                return Filter(CreateKey("Object.Number", entity.ObjectUuid), entity.Value);
            }

            return false;
        }

        public bool Filter(ITrakHoundObjectObservationEntity entity)
        {
            if (entity != null)
            {
                return Filter(CreateKey("Object.Observation", entity.ObjectUuid), entity.Value);
            }

            return false;
        }

        public bool Filter(ITrakHoundObjectQueueEntity entity)
        {
            if (entity != null)
            {
                return Filter(CreateKey("Object.Queue", entity.QueueUuid), CreateKey(entity.Index.ToString(), entity.MemberUuid));
            }

            return false;
        }

        public bool Filter(ITrakHoundObjectReferenceEntity entity)
        {
            if (entity != null)
            {
                return Filter(CreateKey("Object.Reference", entity.ObjectUuid), entity.TargetUuid);
            }

            return false;
        }

        public bool Filter(ITrakHoundObjectSetEntity entity)
        {
            if (entity != null)
            {
                return Filter(CreateKey("Object.Set", entity.ObjectUuid), entity.Value);
            }

            return false;
        }

        public bool Filter(ITrakHoundObjectStateEntity entity)
        {
            if (entity != null)
            {
                return Filter(CreateKey("Object.State", entity.ObjectUuid), entity.DefinitionUuid);
            }

            return false;
        }

        public bool Filter(ITrakHoundObjectStatisticEntity entity)
        {
            if (entity != null)
            {
                return Filter(CreateKey("Object.Statistic", entity.ObjectUuid, entity.TimeRangeStart.ToString(), entity.TimeRangeEnd.ToString()), entity.Value);
            }

            return false;
        }

        public bool Filter(ITrakHoundObjectStringEntity entity)
        {
            if (entity != null)
            {
                return Filter(CreateKey("Object.String", entity.ObjectUuid), entity.Value);
            }

            return false;
        }

        public bool Filter(ITrakHoundObjectTimeRangeEntity entity)
        {
            if (entity != null)
            {
                return Filter(CreateKey("Object.TimeRange", entity.ObjectUuid), CreateKey(entity.Start.ToString(), entity.End.ToString()));
            }

            return false;
        }

        public bool Filter(ITrakHoundObjectTimestampEntity entity)
        {
            if (entity != null)
            {
                return Filter(CreateKey("Object.Timestamp", entity.ObjectUuid), entity.Value.ToString());
            }

            return false;
        }

        public bool Filter(ITrakHoundObjectVocabularyEntity entity)
        {
            if (entity != null)
            {
                return Filter(CreateKey("Object.Vocabulary", entity.ObjectUuid), entity.DefinitionUuid);
            }

            return false;
        }

        public bool Filter(ITrakHoundObjectVocabularySetEntity entity)
        {
            if (entity != null)
            {
                return Filter(CreateKey("Object.VocabularySet", entity.ObjectUuid), entity.DefinitionUuid);
            }

            return false;
        }


        public bool Filter(ITrakHoundSourceEntity entity)
        {
            if (entity != null)
            {
                return Filter(CreateKey("Source", entity.Uuid), entity.Uuid);
            }

            return false;
        }

        public bool Filter(ITrakHoundSourceMetadataEntity entity)
        {
            if (entity != null)
            {
                return Filter(CreateKey("Source.Metadata", entity.Uuid, entity.Name), CreateKey(entity.Value));
            }

            return false;
        }
    }
}
