// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrakHound.Entities;

namespace TrakHound.Clients
{
    public abstract partial class TrakHoundSystemEntitiesClientBase : ITrakHoundSystemEntitiesClient
    {

        private async Task<bool> PublishSourceEntities(IEnumerable<ITrakHoundEntity> entities, TrakHoundOperationMode mode = TrakHoundOperationMode.Async, string routerId = null)
        {
            if (!entities.IsNullOrEmpty())
            {
                var success = true;

                var entityDictionary = new TrakHoundEntityClassDictionary();
                entityDictionary.Add(entities);

                foreach (var entityClass in entityDictionary.SourceClassIds)
                {
                    switch (entityClass)
                    {

                    case TrakHoundSourcesEntityClassId.Source:

                        if (!(await Sources.Publish(entityDictionary.Get(TrakHoundEntityCategoryId.Sources, TrakHoundSourcesEntityClassId.Source)?.Select(o => (ITrakHoundSourceEntity)o), mode, routerId))) success = false;
                        break;

                    case TrakHoundSourcesEntityClassId.Metadata:

                        if (!(await Sources.Metadata.Publish(entityDictionary.Get(TrakHoundEntityCategoryId.Sources, TrakHoundSourcesEntityClassId.Metadata)?.Select(o => (ITrakHoundSourceMetadataEntity)o), mode, routerId))) success = false;
                        break;

                    }
                }

                return success;
            }

            return false;
        }

        private async Task<bool> PublishDefinitionEntities(IEnumerable<ITrakHoundEntity> entities, TrakHoundOperationMode mode = TrakHoundOperationMode.Async, string routerId = null)
        {
            if (!entities.IsNullOrEmpty())
            {
                var success = true;

                var entityDictionary = new TrakHoundEntityClassDictionary();
                entityDictionary.Add(entities);

                foreach (var entityClass in entityDictionary.DefinitionClassIds)
                {
                    switch (entityClass)
                    {

                        case TrakHoundDefinitionsEntityClassId.Definition:

                            if (!(await Definitions.Publish(entityDictionary.Get(TrakHoundEntityCategoryId.Definitions, TrakHoundDefinitionsEntityClassId.Definition)?.Select(o => (ITrakHoundDefinitionEntity)o), mode, routerId))) success = false;
                            break;

                        case TrakHoundDefinitionsEntityClassId.Metadata:

                            if (!(await Definitions.Metadata.Publish(entityDictionary.Get(TrakHoundEntityCategoryId.Definitions, TrakHoundDefinitionsEntityClassId.Metadata)?.Select(o => (ITrakHoundDefinitionMetadataEntity)o), mode, routerId))) success = false;
                            break;

                        case TrakHoundDefinitionsEntityClassId.Description:

                            if (!(await Definitions.Description.Publish(entityDictionary.Get(TrakHoundEntityCategoryId.Definitions, TrakHoundDefinitionsEntityClassId.Description)?.Select(o => (ITrakHoundDefinitionDescriptionEntity)o), mode, routerId))) success = false;
                            break;

                        case TrakHoundDefinitionsEntityClassId.Wiki:

                            if (!(await Definitions.Wiki.Publish(entityDictionary.Get(TrakHoundEntityCategoryId.Definitions, TrakHoundDefinitionsEntityClassId.Wiki)?.Select(o => (ITrakHoundDefinitionWikiEntity)o), mode, routerId))) success = false;
                            break;

                    }
                }

                return success;
            }

            return false;
        }

        private async Task<bool> PublishObjectEntities(IEnumerable<ITrakHoundEntity> entities, TrakHoundOperationMode mode = TrakHoundOperationMode.Async, string routerId = null)
        {
            if (!entities.IsNullOrEmpty())
            {
                var success = true;

                var entityDictionary = new TrakHoundEntityClassDictionary();
                entityDictionary.Add(entities);

                foreach (var entityClass in entityDictionary.ObjectClassIds)
                {
                    switch (entityClass)
                    {

                        case TrakHoundObjectsEntityClassId.Object:

                            if (!(await Objects.Publish(entityDictionary.Get(TrakHoundEntityCategoryId.Objects, TrakHoundObjectsEntityClassId.Object)?.Select(o => (ITrakHoundObjectEntity)o), mode, routerId))) success = false;
                            break;

                        case TrakHoundObjectsEntityClassId.Metadata:

                            if (!(await Objects.Metadata.Publish(entityDictionary.Get(TrakHoundEntityCategoryId.Objects, TrakHoundObjectsEntityClassId.Metadata)?.Select(o => (ITrakHoundObjectMetadataEntity)o), mode, routerId))) success = false;
                            break;

                        case TrakHoundObjectsEntityClassId.Assignment:

                            if (!(await Objects.Assignment.Publish(entityDictionary.Get(TrakHoundEntityCategoryId.Objects, TrakHoundObjectsEntityClassId.Assignment)?.Select(o => (ITrakHoundObjectAssignmentEntity)o), mode, routerId))) success = false;
                            break;

                        case TrakHoundObjectsEntityClassId.Blob:

                            if (!(await Objects.Blob.Publish(entityDictionary.Get(TrakHoundEntityCategoryId.Objects, TrakHoundObjectsEntityClassId.Blob)?.Select(o => (ITrakHoundObjectBlobEntity)o), mode, routerId))) success = false;
                            break;

                        case TrakHoundObjectsEntityClassId.Boolean:

                            if (!(await Objects.Boolean.Publish(entityDictionary.Get(TrakHoundEntityCategoryId.Objects, TrakHoundObjectsEntityClassId.Boolean)?.Select(o => (ITrakHoundObjectBooleanEntity)o), mode, routerId))) success = false;
                            break;

                        case TrakHoundObjectsEntityClassId.Duration:

                            if (!(await Objects.Duration.Publish(entityDictionary.Get(TrakHoundEntityCategoryId.Objects, TrakHoundObjectsEntityClassId.Duration)?.Select(o => (ITrakHoundObjectDurationEntity)o), mode, routerId))) success = false;
                            break;

                        case TrakHoundObjectsEntityClassId.Event:

                            if (!(await Objects.Event.Publish(entityDictionary.Get(TrakHoundEntityCategoryId.Objects, TrakHoundObjectsEntityClassId.Event)?.Select(o => (ITrakHoundObjectEventEntity)o), mode, routerId))) success = false;
                            break;

                        case TrakHoundObjectsEntityClassId.Group:

                            if (!(await Objects.Group.Publish(entityDictionary.Get(TrakHoundEntityCategoryId.Objects, TrakHoundObjectsEntityClassId.Group)?.Select(o => (ITrakHoundObjectGroupEntity)o), mode, routerId))) success = false;
                            break;

                        case TrakHoundObjectsEntityClassId.Hash:

                            if (!(await Objects.Hash.Publish(entityDictionary.Get(TrakHoundEntityCategoryId.Objects, TrakHoundObjectsEntityClassId.Hash)?.Select(o => (ITrakHoundObjectHashEntity)o), mode, routerId))) success = false;
                            break;

                        case TrakHoundObjectsEntityClassId.Log:

                            if (!(await Objects.Log.Publish(entityDictionary.Get(TrakHoundEntityCategoryId.Objects, TrakHoundObjectsEntityClassId.Log)?.Select(o => (ITrakHoundObjectLogEntity)o), mode, routerId))) success = false;
                            break;

                        case TrakHoundObjectsEntityClassId.Message:

                            if (!(await Objects.Message.Publish(entityDictionary.Get(TrakHoundEntityCategoryId.Objects, TrakHoundObjectsEntityClassId.Message)?.Select(o => (ITrakHoundObjectMessageEntity)o), mode, routerId))) success = false;
                            break;

                        case TrakHoundObjectsEntityClassId.MessageQueue:

                            if (!(await Objects.MessageQueue.Publish(entityDictionary.Get(TrakHoundEntityCategoryId.Objects, TrakHoundObjectsEntityClassId.MessageQueue)?.Select(o => (ITrakHoundObjectMessageQueueEntity)o), mode, routerId))) success = false;
                            break;

                        case TrakHoundObjectsEntityClassId.Number:

                            if (!(await Objects.Number.Publish(entityDictionary.Get(TrakHoundEntityCategoryId.Objects, TrakHoundObjectsEntityClassId.Number)?.Select(o => (ITrakHoundObjectNumberEntity)o), mode, routerId))) success = false;
                            break;

                        case TrakHoundObjectsEntityClassId.Observation:

                            if (!(await Objects.Observation.Publish(entityDictionary.Get(TrakHoundEntityCategoryId.Objects, TrakHoundObjectsEntityClassId.Observation)?.Select(o => (ITrakHoundObjectObservationEntity)o), mode, routerId))) success = false;
                            break;

                        case TrakHoundObjectsEntityClassId.Queue:

                            if (!(await Objects.Queue.Publish(entityDictionary.Get(TrakHoundEntityCategoryId.Objects, TrakHoundObjectsEntityClassId.Queue)?.Select(o => (ITrakHoundObjectQueueEntity)o), mode, routerId))) success = false;
                            break;

                        case TrakHoundObjectsEntityClassId.Reference:

                            if (!(await Objects.Reference.Publish(entityDictionary.Get(TrakHoundEntityCategoryId.Objects, TrakHoundObjectsEntityClassId.Reference)?.Select(o => (ITrakHoundObjectReferenceEntity)o), mode, routerId))) success = false;
                            break;

                        case TrakHoundObjectsEntityClassId.Set:

                            if (!(await Objects.Set.Publish(entityDictionary.Get(TrakHoundEntityCategoryId.Objects, TrakHoundObjectsEntityClassId.Set)?.Select(o => (ITrakHoundObjectSetEntity)o), mode, routerId))) success = false;
                            break;

                        case TrakHoundObjectsEntityClassId.State:

                            if (!(await Objects.State.Publish(entityDictionary.Get(TrakHoundEntityCategoryId.Objects, TrakHoundObjectsEntityClassId.State)?.Select(o => (ITrakHoundObjectStateEntity)o), mode, routerId))) success = false;
                            break;

                        case TrakHoundObjectsEntityClassId.Statistic:

                            if (!(await Objects.Statistic.Publish(entityDictionary.Get(TrakHoundEntityCategoryId.Objects, TrakHoundObjectsEntityClassId.Statistic)?.Select(o => (ITrakHoundObjectStatisticEntity)o), mode, routerId))) success = false;
                            break;

                        case TrakHoundObjectsEntityClassId.String:

                            if (!(await Objects.String.Publish(entityDictionary.Get(TrakHoundEntityCategoryId.Objects, TrakHoundObjectsEntityClassId.String)?.Select(o => (ITrakHoundObjectStringEntity)o), mode, routerId))) success = false;
                            break;

                        case TrakHoundObjectsEntityClassId.TimeRange:

                            if (!(await Objects.TimeRange.Publish(entityDictionary.Get(TrakHoundEntityCategoryId.Objects, TrakHoundObjectsEntityClassId.TimeRange)?.Select(o => (ITrakHoundObjectTimeRangeEntity)o), mode, routerId))) success = false;
                            break;

                        case TrakHoundObjectsEntityClassId.Timestamp:

                            if (!(await Objects.Timestamp.Publish(entityDictionary.Get(TrakHoundEntityCategoryId.Objects, TrakHoundObjectsEntityClassId.Timestamp)?.Select(o => (ITrakHoundObjectTimestampEntity)o), mode, routerId))) success = false;
                            break;

                        case TrakHoundObjectsEntityClassId.Vocabulary:

                            if (!(await Objects.Vocabulary.Publish(entityDictionary.Get(TrakHoundEntityCategoryId.Objects, TrakHoundObjectsEntityClassId.Vocabulary)?.Select(o => (ITrakHoundObjectVocabularyEntity)o), mode, routerId))) success = false;
                            break;

                        case TrakHoundObjectsEntityClassId.VocabularySet:

                            if (!(await Objects.VocabularySet.Publish(entityDictionary.Get(TrakHoundEntityCategoryId.Objects, TrakHoundObjectsEntityClassId.VocabularySet)?.Select(o => (ITrakHoundObjectVocabularySetEntity)o), mode, routerId))) success = false;
                            break;

                    }
                }

                return success;
            }

            return false;
        }



        public ITrakHoundEntityClient<TEntity> GetSourcesEntityClient<TEntity>() where TEntity : ITrakHoundEntity
        {

            if (typeof(ITrakHoundSourceEntity).IsAssignableFrom(typeof(TEntity))) return (ITrakHoundEntityClient<TEntity>)Sources;

            else if (typeof(ITrakHoundSourceMetadataEntity).IsAssignableFrom(typeof(TEntity))) return (ITrakHoundEntityClient<TEntity>)Sources.Metadata;

            return null;
        }

        public ITrakHoundEntityClient<TEntity> GetDefinitionsEntityClient<TEntity>() where TEntity : ITrakHoundEntity
        {

            if (typeof(ITrakHoundDefinitionEntity).IsAssignableFrom(typeof(TEntity))) return (ITrakHoundEntityClient<TEntity>)Definitions;

            else if (typeof(ITrakHoundDefinitionMetadataEntity).IsAssignableFrom(typeof(TEntity))) return (ITrakHoundEntityClient<TEntity>)Definitions.Metadata;

            else if (typeof(ITrakHoundDefinitionDescriptionEntity).IsAssignableFrom(typeof(TEntity))) return (ITrakHoundEntityClient<TEntity>)Definitions.Description;

            else if (typeof(ITrakHoundDefinitionWikiEntity).IsAssignableFrom(typeof(TEntity))) return (ITrakHoundEntityClient<TEntity>)Definitions.Wiki;

            return null;
        }

        public ITrakHoundEntityClient<TEntity> GetObjectsEntityClient<TEntity>() where TEntity : ITrakHoundEntity
        {

            if (typeof(ITrakHoundObjectEntity).IsAssignableFrom(typeof(TEntity))) return (ITrakHoundEntityClient<TEntity>)Objects;

            else if (typeof(ITrakHoundObjectMetadataEntity).IsAssignableFrom(typeof(TEntity))) return (ITrakHoundEntityClient<TEntity>)Objects.Metadata;

            else if (typeof(ITrakHoundObjectAssignmentEntity).IsAssignableFrom(typeof(TEntity))) return (ITrakHoundEntityClient<TEntity>)Objects.Assignment;

            else if (typeof(ITrakHoundObjectBlobEntity).IsAssignableFrom(typeof(TEntity))) return (ITrakHoundEntityClient<TEntity>)Objects.Blob;

            else if (typeof(ITrakHoundObjectBooleanEntity).IsAssignableFrom(typeof(TEntity))) return (ITrakHoundEntityClient<TEntity>)Objects.Boolean;

            else if (typeof(ITrakHoundObjectDurationEntity).IsAssignableFrom(typeof(TEntity))) return (ITrakHoundEntityClient<TEntity>)Objects.Duration;

            else if (typeof(ITrakHoundObjectEventEntity).IsAssignableFrom(typeof(TEntity))) return (ITrakHoundEntityClient<TEntity>)Objects.Event;

            else if (typeof(ITrakHoundObjectGroupEntity).IsAssignableFrom(typeof(TEntity))) return (ITrakHoundEntityClient<TEntity>)Objects.Group;

            else if (typeof(ITrakHoundObjectHashEntity).IsAssignableFrom(typeof(TEntity))) return (ITrakHoundEntityClient<TEntity>)Objects.Hash;

            else if (typeof(ITrakHoundObjectLogEntity).IsAssignableFrom(typeof(TEntity))) return (ITrakHoundEntityClient<TEntity>)Objects.Log;

            else if (typeof(ITrakHoundObjectMessageEntity).IsAssignableFrom(typeof(TEntity))) return (ITrakHoundEntityClient<TEntity>)Objects.Message;

            else if (typeof(ITrakHoundObjectMessageQueueEntity).IsAssignableFrom(typeof(TEntity))) return (ITrakHoundEntityClient<TEntity>)Objects.MessageQueue;

            else if (typeof(ITrakHoundObjectNumberEntity).IsAssignableFrom(typeof(TEntity))) return (ITrakHoundEntityClient<TEntity>)Objects.Number;

            else if (typeof(ITrakHoundObjectObservationEntity).IsAssignableFrom(typeof(TEntity))) return (ITrakHoundEntityClient<TEntity>)Objects.Observation;

            else if (typeof(ITrakHoundObjectQueueEntity).IsAssignableFrom(typeof(TEntity))) return (ITrakHoundEntityClient<TEntity>)Objects.Queue;

            else if (typeof(ITrakHoundObjectReferenceEntity).IsAssignableFrom(typeof(TEntity))) return (ITrakHoundEntityClient<TEntity>)Objects.Reference;

            else if (typeof(ITrakHoundObjectSetEntity).IsAssignableFrom(typeof(TEntity))) return (ITrakHoundEntityClient<TEntity>)Objects.Set;

            else if (typeof(ITrakHoundObjectStateEntity).IsAssignableFrom(typeof(TEntity))) return (ITrakHoundEntityClient<TEntity>)Objects.State;

            else if (typeof(ITrakHoundObjectStatisticEntity).IsAssignableFrom(typeof(TEntity))) return (ITrakHoundEntityClient<TEntity>)Objects.Statistic;

            else if (typeof(ITrakHoundObjectStringEntity).IsAssignableFrom(typeof(TEntity))) return (ITrakHoundEntityClient<TEntity>)Objects.String;

            else if (typeof(ITrakHoundObjectTimeRangeEntity).IsAssignableFrom(typeof(TEntity))) return (ITrakHoundEntityClient<TEntity>)Objects.TimeRange;

            else if (typeof(ITrakHoundObjectTimestampEntity).IsAssignableFrom(typeof(TEntity))) return (ITrakHoundEntityClient<TEntity>)Objects.Timestamp;

            else if (typeof(ITrakHoundObjectVocabularyEntity).IsAssignableFrom(typeof(TEntity))) return (ITrakHoundEntityClient<TEntity>)Objects.Vocabulary;

            else if (typeof(ITrakHoundObjectVocabularySetEntity).IsAssignableFrom(typeof(TEntity))) return (ITrakHoundEntityClient<TEntity>)Objects.VocabularySet;

            return null;
        }
    }
}
