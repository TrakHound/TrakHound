// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrakHound.Entities;
using TrakHound.Entities.Collections;
using TrakHound.Entities.QueryEngines;

namespace TrakHound.Clients
{
    public abstract partial class TrakHoundSystemEntitiesClientBase : ITrakHoundSystemEntitiesClient
    {
        private readonly Dictionary<string, ITrakHoundEntitiesClientMiddleware> _middleware = new Dictionary<string, ITrakHoundEntitiesClientMiddleware>();
        private readonly object _lock = new object();


        protected ITrakHoundSystemSourceClient _sources;
        protected ITrakHoundSystemDefinitionClient _definitions;
        protected ITrakHoundSystemObjectClient _objects;


        public IEnumerable<ITrakHoundEntitiesClientMiddleware> Middleware => _middleware.Values;

        public ITrakHoundSystemObjectClient Objects => _objects;

        public ITrakHoundSystemDefinitionClient Definitions => _definitions;

        public ITrakHoundSystemSourceClient Sources => _sources;


        public virtual async Task<TrakHoundQueryResponse> Query(string query, string routerId = null) => new TrakHoundQueryResponse();

        public virtual async Task<ITrakHoundConsumer<TrakHoundEntityCollection>> Subscribe(int interval = 100, int count = 1000, string routerId = null) => null;
        
        public virtual async Task<ITrakHoundConsumer<TrakHoundQueryResponse>> Subscribe(string query, int interval = 100, int count = 1000, string routerId = null) => null;

        public virtual async Task<ITrakHoundConsumer<TrakHoundEntityCollection>> Subscribe(IEnumerable<TrakHoundEntitySubscriptionRequest> requests, int interval = 100, int count = 1000, string routerId = null) => null;

        public virtual async Task<ITrakHoundConsumer<IEnumerable<TrakHoundEntityNotification>>> Notify(string query, TrakHoundEntityNotificationType notificationType = TrakHoundEntityNotificationType.All, string routerId = null) => null;


        public async Task<ITrakHoundConsumer<IEnumerable<ITrakHoundEntity>>> CreateSubscriptions(IEnumerable<TrakHoundEntitySubscriptionRequest> requests, string consumerId = null, string routerId = null)
        {
            if (!requests.IsNullOrEmpty())
            {
                var contentConsumers = new List<ITrakHoundConsumer<IEnumerable<ITrakHoundEntity>>>();
                var assignmentExpressions = new List<string>();
                var eventExpressions = new List<string>();
                var messageExpressions = new List<string>();
                var observationExpressions = new List<string>();
                var stateExpressions = new List<string>();
                var stringExpressions = new List<string>();

                foreach (var request in requests)
                {
                    var entityCategory = request.EntityCategory.ConvertEnum<TrakHoundEntityCategory>();
                    switch (entityCategory)
                    {
                        case TrakHoundEntityCategory.Objects:

                            var entityClass = request.EntityClass.ConvertEnum<TrakHoundObjectsEntityClass>();
                            switch (entityClass)
                            {
                                case TrakHoundObjectsEntityClass.Assignment: assignmentExpressions.Add(request.Expression); break;
                                case TrakHoundObjectsEntityClass.Event: eventExpressions.Add(request.Expression); break;
                                case TrakHoundObjectsEntityClass.Message: messageExpressions.Add(request.Expression); break;
                                case TrakHoundObjectsEntityClass.Observation: observationExpressions.Add(request.Expression); break;
                                case TrakHoundObjectsEntityClass.State: stateExpressions.Add(request.Expression); break;
                                case TrakHoundObjectsEntityClass.String: stringExpressions.Add(request.Expression); break;
                            }
                            break;
                    }
                }

                if (!assignmentExpressions.IsNullOrEmpty()) CreateEntityConsumer(contentConsumers, await Objects.Assignment.SubscribeByAssignee(assignmentExpressions, consumerId: consumerId));
                if (!eventExpressions.IsNullOrEmpty()) CreateEntityConsumer(contentConsumers, await Objects.Event.SubscribeByObject(eventExpressions, consumerId: consumerId));
                if (!messageExpressions.IsNullOrEmpty()) CreateEntityConsumer(contentConsumers, await Objects.Message.SubscribeByObject(messageExpressions, consumerId: consumerId));
                if (!observationExpressions.IsNullOrEmpty()) CreateEntityConsumer(contentConsumers, await Objects.Observation.SubscribeByObject(observationExpressions, consumerId: consumerId));
                if (!stateExpressions.IsNullOrEmpty()) CreateEntityConsumer(contentConsumers, await Objects.State.SubscribeByObject(stateExpressions, consumerId: consumerId));
                if (!stringExpressions.IsNullOrEmpty()) CreateEntityConsumer(contentConsumers, await Objects.String.SubscribeByObject(stringExpressions, consumerId: consumerId));
                return new TrakHoundConsumer<IEnumerable<ITrakHoundEntity>>(contentConsumers);
            }

            return null;
        }

        public async Task<ITrakHoundConsumer<IEnumerable<ITrakHoundEntity>>> CreateSubscriptions(IEnumerable<string> expressions, string consumerId = null, string routerId = null)
        {
            if (!expressions.IsNullOrEmpty())
            {
                var contentConsumers = new List<ITrakHoundConsumer<IEnumerable<ITrakHoundEntity>>>();
                CreateEntityConsumer(contentConsumers, await Objects.Assignment.SubscribeByAssignee(expressions, consumerId: consumerId));
                CreateEntityConsumer(contentConsumers, await Objects.Boolean.SubscribeByObject(expressions, consumerId: consumerId));
                CreateEntityConsumer(contentConsumers, await Objects.Duration.SubscribeByObject(expressions, consumerId: consumerId));
                CreateEntityConsumer(contentConsumers, await Objects.Event.SubscribeByObject(expressions, consumerId: consumerId));
                //CreateEntityConsumer(contentConsumers, await Objects.Feeds.SubscribeByObject(GetContentExpression(expressions, TrakHoundObjectContentType.Feed)));
                //CreateEntityConsumer(contentConsumers, await Objects.Group.SubscribeByGroup(GetContentExpression(expressions, TrakHoundObjectContentType.Group)));
                CreateEntityConsumer(contentConsumers, await Objects.Message.SubscribeByObject(expressions, consumerId: consumerId));
                CreateEntityConsumer(contentConsumers, await Objects.Number.SubscribeByObject(expressions, consumerId: consumerId));
                CreateEntityConsumer(contentConsumers, await Objects.Observation.SubscribeByObject(expressions, consumerId: consumerId));
                CreateEntityConsumer(contentConsumers, await Objects.Queue.SubscribeByQueue(expressions));
                //CreateEntityConsumer(contentConsumers, await Objects.References.SubscribeByObject(GetContentExpression(expressions, TrakHoundObjectContentType.Reference)));
                CreateEntityConsumer(contentConsumers, await Objects.Statistic.SubscribeByObject(expressions, consumerId: consumerId));
                CreateEntityConsumer(contentConsumers, await Objects.State.SubscribeByObject(expressions, consumerId: consumerId));
                CreateEntityConsumer(contentConsumers, await Objects.String.SubscribeByObject(expressions, consumerId: consumerId));
                CreateEntityConsumer(contentConsumers, await Objects.Timestamp.SubscribeByObject(expressions, consumerId: consumerId));
                CreateEntityConsumer(contentConsumers, await Objects.Vocabulary.SubscribeByObject(expressions, consumerId: consumerId));
                return new TrakHoundConsumer<IEnumerable<ITrakHoundEntity>>(contentConsumers);
            }

            return null;
        }

        public async Task<ITrakHoundConsumer<IEnumerable<ITrakHoundEntity>>> SubscribeToContent(string expression, string consumerId = null, string routerId = null)
        {
            if (!string.IsNullOrEmpty(expression))
            {
                return await SubscribeToContent(new string[] { expression }, consumerId, routerId);
            }

            return null;
        }

        public async Task<ITrakHoundConsumer<IEnumerable<ITrakHoundEntity>>> SubscribeToContent(IEnumerable<string> expressions, string consumerId = null, string routerId = null)
        {
            if (!expressions.IsNullOrEmpty())
            {
                var contentConsumers = new List<ITrakHoundConsumer<IEnumerable<ITrakHoundEntity>>>();
                CreateEntityConsumer(contentConsumers, await Objects.Assignment.SubscribeByAssignee(GetContentExpression(expressions, TrakHoundObjectContentType.Assignment), consumerId: consumerId));
                CreateEntityConsumer(contentConsumers, await Objects.Boolean.SubscribeByObject(GetContentExpression(expressions, TrakHoundObjectContentType.Boolean), consumerId: consumerId));
                CreateEntityConsumer(contentConsumers, await Objects.Duration.SubscribeByObject(GetContentExpression(expressions, TrakHoundObjectContentType.Duration), consumerId: consumerId));
                CreateEntityConsumer(contentConsumers, await Objects.Event.SubscribeByObject(GetContentExpression(expressions, TrakHoundObjectContentType.Event), consumerId: consumerId));
                //CreateEntityConsumer(contentConsumers, await Objects.Feeds.SubscribeByObject(GetContentExpression(expressions, TrakHoundObjectContentType.Feed)));
                //CreateEntityConsumer(contentConsumers, await Objects.Group.SubscribeByGroup(GetContentExpression(expressions, TrakHoundObjectContentType.Group)));
                CreateEntityConsumer(contentConsumers, await Objects.Message.SubscribeByObject(GetContentExpression(expressions, TrakHoundObjectContentType.Message), consumerId: consumerId));
                CreateEntityConsumer(contentConsumers, await Objects.Number.SubscribeByObject(GetContentExpression(expressions, TrakHoundObjectContentType.Number), consumerId: consumerId));
                CreateEntityConsumer(contentConsumers, await Objects.Observation.SubscribeByObject(GetContentExpression(expressions, TrakHoundObjectContentType.Observation), consumerId: consumerId));
                CreateEntityConsumer(contentConsumers, await Objects.Queue.SubscribeByQueue(GetContentExpression(expressions, TrakHoundObjectContentType.Queue)));
                //CreateEntityConsumer(contentConsumers, await Objects.References.SubscribeByObject(GetContentExpression(expressions, TrakHoundObjectContentType.Reference)));
                CreateEntityConsumer(contentConsumers, await Objects.Statistic.SubscribeByObject(GetContentExpression(expressions, TrakHoundObjectContentType.Statistic), consumerId: consumerId));
                CreateEntityConsumer(contentConsumers, await Objects.State.SubscribeByObject(GetContentExpression(expressions, TrakHoundObjectContentType.State), consumerId: consumerId));
                CreateEntityConsumer(contentConsumers, await Objects.String.SubscribeByObject(GetContentExpression(expressions, TrakHoundObjectContentType.String), consumerId: consumerId));
                CreateEntityConsumer(contentConsumers, await Objects.Timestamp.SubscribeByObject(GetContentExpression(expressions, TrakHoundObjectContentType.Timestamp), consumerId: consumerId));
                CreateEntityConsumer(contentConsumers, await Objects.Vocabulary.SubscribeByObject(GetContentExpression(expressions, TrakHoundObjectContentType.Vocabulary), consumerId: consumerId));
                return new TrakHoundConsumer<IEnumerable<ITrakHoundEntity>>(contentConsumers);
            }

            return null;
        }

        private static IEnumerable<string> GetContentExpression(IEnumerable<string> expressions, TrakHoundObjectContentType contentType)
        {
            if (!expressions.IsNullOrEmpty())
            {
                var results = new List<string>();

                foreach (var expression in expressions)
                {
                    results.Add(TrakHoundPath.Combine(expression, $"content-type:{contentType}"));
                }

                return results;
            }

            return null;
        }


        private void CreateEntityConsumer<TEntityInput>(List<ITrakHoundConsumer<IEnumerable<ITrakHoundEntity>>> consumers, ITrakHoundConsumer<IEnumerable<TEntityInput>> inputConsumer)
            where TEntityInput : ITrakHoundEntity
        {
            if (inputConsumer != null)
            {
                var outputConsumer = new TrakHoundConsumer<IEnumerable<TEntityInput>, IEnumerable<ITrakHoundEntity>>(inputConsumer);
                outputConsumer.OnReceived = (entityInput) =>
                {
                    var results = new List<ITrakHoundEntity>();
                    foreach (var entity in entityInput) results.Add(entity);
                    return results;
                };

                consumers.Add(outputConsumer);
            }
        }


        public virtual async Task<bool> Publish(ITrakHoundEntity entity, TrakHoundOperationMode mode = TrakHoundOperationMode.Async, string routerId = null)
        {
            if (entity != null)
            {
                var entities = new List<ITrakHoundEntity> { entity };
                return await Publish(entities, mode, routerId);
            }

            return false;
        }

        public virtual async Task<bool> Publish(IEnumerable<ITrakHoundEntity> entities, TrakHoundOperationMode mode = TrakHoundOperationMode.Async, string routerId = null)
        {
            if (!entities.IsNullOrEmpty())
            {
                await OnBeforePublish(entities, mode);

                var success = true;

                var categories = entities.Select(o => o.Category).Distinct();
                foreach (var category in categories)
                {
                    switch (category)
                    {
                        case TrakHoundEntityCategoryId.Sources:
                            if (!(await PublishSourceEntities(entities.Where(o => o.Category == TrakHoundEntityCategoryId.Sources), mode, routerId))) success = false;
                            break;

                        case TrakHoundEntityCategoryId.Definitions:
                            if (!(await PublishDefinitionEntities(entities.Where(o => o.Category == TrakHoundEntityCategoryId.Definitions), mode, routerId))) success = false;
                            break;

                        case TrakHoundEntityCategoryId.Objects:
                            if (!(await PublishObjectEntities(entities.Where(o => o.Category == TrakHoundEntityCategoryId.Objects), mode, routerId))) success = false;
                            break;
                    }
                }

                await OnAfterPublish(entities, mode);

                return success;
            }

            return false;
        }

        //public virtual async Task<bool> Publish(IEnumerable<ITrakHoundEntity> entities, TrakHoundOperationMode mode = TrakHoundOperationMode.Async, string routerId = null)
        //{
        //    if (!entities.IsNullOrEmpty())
        //    {
        //        var publishEntities = new List<ITrakHoundEntity>();
        //        foreach (var entity in entities)
        //        {
        //            var publishEntity = Process(entity, mode);
        //            if (publishEntity != null) publishEntities.Add(publishEntity);
        //        }


        //        //var publishEntities = Process(entities, mode);
        //        if (publishEntities != null)
        //        {
        //            await OnBeforePublish(publishEntities, mode);

        //            var success = true;


        //            //// Is this needed?? Just use publishEntities??
        //            //var collection = new TrakHoundEntityCollection();
        //            //collection.Add(publishEntities);

        //            //var x = collection.GetEntities();

        //            var categories = publishEntities.Select(o => o.Category).Distinct();
        //            foreach (var category in categories)
        //            {
        //                switch (category)
        //                {
        //                    case TrakHoundEntityCategoryName.Sources:
        //                        if (!(await PublishSourceEntities(publishEntities.Where(o => o.Category == TrakHoundEntityCategoryName.Sources), mode, routerId))) success = false;
        //                        break;

        //                    case TrakHoundEntityCategoryName.Definitions:
        //                        if (!(await PublishDefinitionEntities(publishEntities.Where(o => o.Category == TrakHoundEntityCategoryName.Definitions), mode, routerId))) success = false;
        //                        break;

        //                    case TrakHoundEntityCategoryName.Objects:
        //                        if (!(await PublishObjectEntities(publishEntities.Where(o => o.Category == TrakHoundEntityCategoryName.Objects), mode, routerId))) success = false;
        //                        break;
        //                }
        //            }

        //            await OnAfterPublish(publishEntities, mode);

        //            return success;
        //        }
        //    }

        //    return false;
        //}

        //public virtual async Task<bool> Publish(IEnumerable<ITrakHoundEntity> entities, TrakHoundOperationMode mode = TrakHoundOperationMode.Async, string routerId = null)
        //{
        //    if (!entities.IsNullOrEmpty())
        //    {
        //        var publishEntities = Process(entities, mode);
        //        if (publishEntities != null)
        //        {
        //            await OnBeforePublish(publishEntities, mode);

        //            var success = true;


        //            // Is this needed?? Just use publishEntities??
        //            var collection = new TrakHoundEntityCollection();
        //            collection.Add(publishEntities);

        //            var x = collection.GetEntities();

        //            var categories = x.Select(o => o.Category).Distinct();
        //            foreach (var category in categories)
        //            {
        //                switch (category)
        //                {
        //                    case TrakHoundEntityCategoryName.Sources:
        //                        if (!(await PublishSourceEntities(x.Where(o => o.Category == TrakHoundEntityCategoryName.Sources), mode, routerId))) success = false;
        //                        break;

        //                    case TrakHoundEntityCategoryName.Definitions:
        //                        if (!(await PublishDefinitionEntities(x.Where(o => o.Category == TrakHoundEntityCategoryName.Definitions), mode, routerId))) success = false;
        //                        break;

        //                    case TrakHoundEntityCategoryName.Objects:
        //                        if (!(await PublishObjectEntities(x.Where(o => o.Category == TrakHoundEntityCategoryName.Objects), mode, routerId))) success = false;
        //                        break;
        //                }
        //            }

        //            await OnAfterPublish(publishEntities, mode);

        //            return success;
        //        }
        //    }

        //    return false;
        //}


        //private async Task<bool> PublishSourceEntities(IEnumerable<ITrakHoundEntity> entities, TrakHoundOperationMode mode = TrakHoundOperationMode.Async, string routerId = null)
        //{
        //    if (!entities.IsNullOrEmpty())
        //    {
        //        var success = true;

        //        var entityClasses = entities.Select(o => o.Class).Distinct();
        //        foreach (var entityClass in entityClasses)
        //        {
        //            switch (entityClass)
        //            {
        //                case TrakHoundSourcesEntityClassName.Source:

        //                    if (!(await Sources.Publish(entities.Where(o => o.Class == TrakHoundSourcesEntityClassName.Source).Select(o => (ITrakHoundSourceEntity)o), mode, routerId))) success = false;
        //                    break;

        //                case TrakHoundSourcesEntityClassName.Metadata:

        //                    if (!(await Sources.Metadata.Publish(entities.Where(o => o.Class == TrakHoundSourcesEntityClassName.Metadata).Select(o => (ITrakHoundSourceMetadataEntity)o), mode, routerId))) success = false;
        //                    break;
        //            }
        //        }

        //        return success;
        //    }

        //    return false;
        //}

        //private async Task<bool> PublishDefinitionEntities(IEnumerable<ITrakHoundEntity> entities, TrakHoundOperationMode mode = TrakHoundOperationMode.Async, string routerId = null)
        //{
        //    if (!entities.IsNullOrEmpty())
        //    {
        //        var success = true;

        //        var entityClasses = entities.Select(o => o.Class).Distinct();
        //        foreach (var entityClass in entityClasses)
        //        {
        //            switch (entityClass)
        //            {
        //                case TrakHoundDefinitionsEntityClassName.Definition:

        //                    if (!(await Definitions.Publish(entities.Where(o => o.Class == TrakHoundDefinitionsEntityClassName.Definition).Select(o => (ITrakHoundDefinitionEntity)o), mode, routerId))) success = false;
        //                    break;

        //                case TrakHoundDefinitionsEntityClassName.Metadata:

        //                    if (!(await Definitions.Metadata.Publish(entities.Where(o => o.Class == TrakHoundDefinitionsEntityClassName.Metadata).Select(o => (ITrakHoundDefinitionMetadataEntity)o), mode, routerId))) success = false;
        //                    break;

        //                case TrakHoundDefinitionsEntityClassName.Description:

        //                    if (!(await Definitions.Description.Publish(entities.Where(o => o.Class == TrakHoundDefinitionsEntityClassName.Description).Select(o => (ITrakHoundDefinitionDescriptionEntity)o), mode, routerId))) success = false;
        //                    break;

        //                case TrakHoundDefinitionsEntityClassName.Wiki:

        //                    if (!(await Definitions.Wiki.Publish(entities.Where(o => o.Class == TrakHoundDefinitionsEntityClassName.Wiki).Select(o => (ITrakHoundDefinitionWikiEntity)o), mode, routerId))) success = false;
        //                    break;
        //            }
        //        }

        //        return success;
        //    }

        //    return false;
        //}

        //private async Task<bool> PublishObjectEntities(IEnumerable<ITrakHoundEntity> entities, TrakHoundOperationMode mode = TrakHoundOperationMode.Async, string routerId = null)
        //{
        //    if (!entities.IsNullOrEmpty())
        //    {
        //        var success = true;

        //        var entityClasses = entities.Select(o => o.Class).Distinct();
        //        foreach (var entityClass in entityClasses)
        //        {
        //            switch (entityClass)
        //            {
        //                case TrakHoundObjectsEntityClassName.Object:

        //                    if (!(await Objects.Publish(entities.Where(o => o.Class == TrakHoundObjectsEntityClassName.Object).Select(o => (ITrakHoundObjectEntity)o), mode, routerId))) success = false;
        //                    break;

        //                case TrakHoundObjectsEntityClassName.Metadata:

        //                    if (!(await Objects.Metadata.Publish(entities.Where(o => o.Class == TrakHoundObjectsEntityClassName.Metadata).Select(o => (ITrakHoundObjectMetadataEntity)o), mode, routerId))) success = false;
        //                    break;

        //                case TrakHoundObjectsEntityClassName.Assignment:

        //                    if (!(await Objects.Assignment.Publish(entities.Where(o => o.Class == TrakHoundObjectsEntityClassName.Assignment).Select(o => (ITrakHoundObjectAssignmentEntity)o), mode, routerId))) success = false;
        //                    break;

        //                case TrakHoundObjectsEntityClassName.Blob:

        //                    if (!(await Objects.Blob.Publish(entities.Where(o => o.Class == TrakHoundObjectsEntityClassName.Blob).Select(o => (ITrakHoundObjectBlobEntity)o), mode, routerId))) success = false;
        //                    break;

        //                case TrakHoundObjectsEntityClassName.Boolean:

        //                    if (!(await Objects.Boolean.Publish(entities.Where(o => o.Class == TrakHoundObjectsEntityClassName.Boolean).Select(o => (ITrakHoundObjectBooleanEntity)o), mode, routerId))) success = false;
        //                    break;

        //                case TrakHoundObjectsEntityClassName.Duration:

        //                    if (!(await Objects.Duration.Publish(entities.Where(o => o.Class == TrakHoundObjectsEntityClassName.Duration).Select(o => (ITrakHoundObjectDurationEntity)o), mode, routerId))) success = false;
        //                    break;

        //                case TrakHoundObjectsEntityClassName.Event:

        //                    if (!(await Objects.Event.Publish(entities.Where(o => o.Class == TrakHoundObjectsEntityClassName.Event).Select(o => (ITrakHoundObjectEventEntity)o), mode, routerId))) success = false;
        //                    break;

        //                case TrakHoundObjectsEntityClassName.Feed:

        //                    if (!(await Objects.Feed.Publish(entities.Where(o => o.Class == TrakHoundObjectsEntityClassName.Feed).Select(o => (ITrakHoundObjectFeedEntity)o), mode, routerId))) success = false;
        //                    break;

        //                case TrakHoundObjectsEntityClassName.Group:

        //                    if (!(await Objects.Group.Publish(entities.Where(o => o.Class == TrakHoundObjectsEntityClassName.Group).Select(o => (ITrakHoundObjectGroupEntity)o), mode, routerId))) success = false;
        //                    break;

        //                case TrakHoundObjectsEntityClassName.Hash:

        //                    if (!(await Objects.Hash.Publish(entities.Where(o => o.Class == TrakHoundObjectsEntityClassName.Hash).Select(o => (ITrakHoundObjectHashEntity)o), mode, routerId))) success = false;
        //                    break;

        //                case TrakHoundObjectsEntityClassName.Ledger:

        //                    if (!(await Objects.Ledger.Publish(entities.Where(o => o.Class == TrakHoundObjectsEntityClassName.Ledger).Select(o => (ITrakHoundObjectLedgerEntity)o), mode, routerId))) success = false;
        //                    break;

        //                case TrakHoundObjectsEntityClassName.Log:

        //                    if (!(await Objects.Log.Publish(entities.Where(o => o.Class == TrakHoundObjectsEntityClassName.Log).Select(o => (ITrakHoundObjectLogEntity)o), mode, routerId))) success = false;
        //                    break;

        //                case TrakHoundObjectsEntityClassName.Message:

        //                    if (!(await Objects.Message.Publish(entities.Where(o => o.Class == TrakHoundObjectsEntityClassName.Message).Select(o => (ITrakHoundObjectMessageEntity)o), mode, routerId))) success = false;
        //                    break;

        //                case TrakHoundObjectsEntityClassName.MessageQueue:

        //                    if (!(await Objects.MessageQueue.Publish(entities.Where(o => o.Class == TrakHoundObjectsEntityClassName.MessageQueue).Select(o => (ITrakHoundObjectMessageQueueEntity)o), mode, routerId))) success = false;
        //                    break;

        //                case TrakHoundObjectsEntityClassName.Number:

        //                    if (!(await Objects.Number.Publish(entities.Where(o => o.Class == TrakHoundObjectsEntityClassName.Number).Select(o => (ITrakHoundObjectNumberEntity)o), mode, routerId))) success = false;
        //                    break;

        //                case TrakHoundObjectsEntityClassName.Observation:

        //                    if (!(await Objects.Observation.Publish(entities.Where(o => o.Class == TrakHoundObjectsEntityClassName.Observation).Select(o => (ITrakHoundObjectObservationEntity)o), mode, routerId))) success = false;
        //                    break;

        //                case TrakHoundObjectsEntityClassName.Queue:

        //                    if (!(await Objects.Queue.Publish(entities.Where(o => o.Class == TrakHoundObjectsEntityClassName.Queue).Select(o => (ITrakHoundObjectQueueEntity)o), mode, routerId))) success = false;
        //                    break;

        //                case TrakHoundObjectsEntityClassName.Reference:

        //                    if (!(await Objects.Reference.Publish(entities.Where(o => o.Class == TrakHoundObjectsEntityClassName.Reference).Select(o => (ITrakHoundObjectReferenceEntity)o), mode, routerId))) success = false;
        //                    break;

        //                case TrakHoundObjectsEntityClassName.Set:

        //                    if (!(await Objects.Set.Publish(entities.Where(o => o.Class == TrakHoundObjectsEntityClassName.Set).Select(o => (ITrakHoundObjectSetEntity)o), mode, routerId))) success = false;
        //                    break;

        //                case TrakHoundObjectsEntityClassName.State:

        //                    if (!(await Objects.State.Publish(entities.Where(o => o.Class == TrakHoundObjectsEntityClassName.State).Select(o => (ITrakHoundObjectStateEntity)o), mode, routerId))) success = false;
        //                    break;

        //                case TrakHoundObjectsEntityClassName.Statistic:

        //                    if (!(await Objects.Statistic.Publish(entities.Where(o => o.Class == TrakHoundObjectsEntityClassName.Statistic).Select(o => (ITrakHoundObjectStatisticEntity)o), mode, routerId))) success = false;
        //                    break;

        //                case TrakHoundObjectsEntityClassName.String:

        //                    if (!(await Objects.String.Publish(entities.Where(o => o.Class == TrakHoundObjectsEntityClassName.String).Select(o => (ITrakHoundObjectStringEntity)o), mode, routerId))) success = false;
        //                    break;

        //                case TrakHoundObjectsEntityClassName.TimeRange:

        //                    if (!(await Objects.TimeRange.Publish(entities.Where(o => o.Class == TrakHoundObjectsEntityClassName.TimeRange).Select(o => (ITrakHoundObjectTimeRangeEntity)o), mode, routerId))) success = false;
        //                    break;

        //                case TrakHoundObjectsEntityClassName.Timestamp:

        //                    if (!(await Objects.Timestamp.Publish(entities.Where(o => o.Class == TrakHoundObjectsEntityClassName.Timestamp).Select(o => (ITrakHoundObjectTimestampEntity)o), mode, routerId))) success = false;
        //                    break;

        //                case TrakHoundObjectsEntityClassName.Vocabulary:

        //                    if (!(await Objects.Vocabulary.Publish(entities.Where(o => o.Class == TrakHoundObjectsEntityClassName.Vocabulary).Select(o => (ITrakHoundObjectVocabularyEntity)o), mode, routerId))) success = false;
        //                    break;

        //                case TrakHoundObjectsEntityClassName.VocabularySet:

        //                    if (!(await Objects.VocabularySet.Publish(entities.Where(o => o.Class == TrakHoundObjectsEntityClassName.VocabularySet).Select(o => (ITrakHoundObjectVocabularySetEntity)o), mode, routerId))) success = false;
        //                    break;
        //            }
        //        }

        //        return success;
        //    }

        //    return false;
        //}



        public ITrakHoundEntityClient<TEntity> GetEntityClient<TEntity>() where TEntity : ITrakHoundEntity
        {
            if (TrakHoundSourcesEntity.IsSourcesEntity<TEntity>()) return GetSourcesEntityClient<TEntity>();
            else if (TrakHoundDefinitionsEntity.IsDefinitionsEntity<TEntity>()) return GetDefinitionsEntityClient<TEntity>();
            else if (TrakHoundObjectsEntity.IsObjectsEntity<TEntity>()) return GetObjectsEntityClient<TEntity>();

            return null;
        }

        //public ITrakHoundEntityClient<TEntity> GetSourcesEntityClient<TEntity>() where TEntity : ITrakHoundEntity
        //{
        //    if (typeof(ITrakHoundSourceEntity).IsAssignableFrom(typeof(TEntity))) return (ITrakHoundEntityClient<TEntity>)Sources;

        //    else if (typeof(ITrakHoundSourceMetadataEntity).IsAssignableFrom(typeof(TEntity))) return (ITrakHoundEntityClient<TEntity>)Sources.Metadata;

        //    return null;
        //}

        //public ITrakHoundEntityClient<TEntity> GetDefinitionsEntityClient<TEntity>() where TEntity : ITrakHoundEntity
        //{
        //    if (typeof(ITrakHoundDefinitionEntity).IsAssignableFrom(typeof(TEntity))) return (ITrakHoundEntityClient<TEntity>)Definitions;

        //    else if (typeof(ITrakHoundDefinitionMetadataEntity).IsAssignableFrom(typeof(TEntity))) return (ITrakHoundEntityClient<TEntity>)Definitions.Metadata;

        //    else if (typeof(ITrakHoundDefinitionDescriptionEntity).IsAssignableFrom(typeof(TEntity))) return (ITrakHoundEntityClient<TEntity>)Definitions.Description;

        //    else if (typeof(ITrakHoundDefinitionWikiEntity).IsAssignableFrom(typeof(TEntity))) return (ITrakHoundEntityClient<TEntity>)Definitions.Wiki;

        //    return null;
        //}

        //public ITrakHoundEntityClient<TEntity> GetObjectsEntityClient<TEntity>() where TEntity : ITrakHoundEntity
        //{
        //    if (typeof(ITrakHoundObjectEntity).IsAssignableFrom(typeof(TEntity))) return (ITrakHoundEntityClient<TEntity>)Objects;
        //    else if (typeof(ITrakHoundObjectMetadataEntity).IsAssignableFrom(typeof(TEntity))) return (ITrakHoundEntityClient<TEntity>)Objects.Metadata;
        //    else if (typeof(ITrakHoundObjectAssignmentEntity).IsAssignableFrom(typeof(TEntity))) return (ITrakHoundEntityClient<TEntity>)Objects.Assignment;
        //    else if (typeof(ITrakHoundObjectBlobEntity).IsAssignableFrom(typeof(TEntity))) return (ITrakHoundEntityClient<TEntity>)Objects.Blob;
        //    else if (typeof(ITrakHoundObjectBooleanEntity).IsAssignableFrom(typeof(TEntity))) return (ITrakHoundEntityClient<TEntity>)Objects.Boolean;
        //    else if (typeof(ITrakHoundObjectDurationEntity).IsAssignableFrom(typeof(TEntity))) return (ITrakHoundEntityClient<TEntity>)Objects.Duration;
        //    else if (typeof(ITrakHoundObjectEventEntity).IsAssignableFrom(typeof(TEntity))) return (ITrakHoundEntityClient<TEntity>)Objects.Event;
        //    else if (typeof(ITrakHoundObjectFeedEntity).IsAssignableFrom(typeof(TEntity))) return (ITrakHoundEntityClient<TEntity>)Objects.Feed;
        //    else if (typeof(ITrakHoundObjectGroupEntity).IsAssignableFrom(typeof(TEntity))) return (ITrakHoundEntityClient<TEntity>)Objects.Group;
        //    else if (typeof(ITrakHoundObjectHashEntity).IsAssignableFrom(typeof(TEntity))) return (ITrakHoundEntityClient<TEntity>)Objects.Hash;
        //    else if (typeof(ITrakHoundObjectLedgerEntity).IsAssignableFrom(typeof(TEntity))) return (ITrakHoundEntityClient<TEntity>)Objects.Ledger;
        //    //else if (typeof(ITrakHoundObjectLinkEntity).IsAssignableFrom(typeof(TEntity))) return (ITrakHoundEntityClient<TEntity>)Objects.Link;
        //    else if (typeof(ITrakHoundObjectLogEntity).IsAssignableFrom(typeof(TEntity))) return (ITrakHoundEntityClient<TEntity>)Objects.Log;
        //    else if (typeof(ITrakHoundObjectMessageEntity).IsAssignableFrom(typeof(TEntity))) return (ITrakHoundEntityClient<TEntity>)Objects.Message;
        //    else if (typeof(ITrakHoundObjectMessageQueueEntity).IsAssignableFrom(typeof(TEntity))) return (ITrakHoundEntityClient<TEntity>)Objects.MessageQueue;
        //    else if (typeof(ITrakHoundObjectNumberEntity).IsAssignableFrom(typeof(TEntity))) return (ITrakHoundEntityClient<TEntity>)Objects.Number;
        //    else if (typeof(ITrakHoundObjectObservationEntity).IsAssignableFrom(typeof(TEntity))) return (ITrakHoundEntityClient<TEntity>)Objects.Observation;
        //    else if (typeof(ITrakHoundObjectQueueEntity).IsAssignableFrom(typeof(TEntity))) return (ITrakHoundEntityClient<TEntity>)Objects.Queue;
        //    else if (typeof(ITrakHoundObjectReferenceEntity).IsAssignableFrom(typeof(TEntity))) return (ITrakHoundEntityClient<TEntity>)Objects.Reference;
        //    //else if (typeof(ITrakHoundObjectSequenceEntity).IsAssignableFrom(typeof(TEntity))) return (ITrakHoundEntityClient<TEntity>)Objects.Sequence;
        //    else if (typeof(ITrakHoundObjectSetEntity).IsAssignableFrom(typeof(TEntity))) return (ITrakHoundEntityClient<TEntity>)Objects.Set;
        //    //else if (typeof(ITrakHoundObjectSnapshotEntity).IsAssignableFrom(typeof(TEntity))) return (ITrakHoundEntityClient<TEntity>)Objects.Snapshot;
        //    else if (typeof(ITrakHoundObjectStateEntity).IsAssignableFrom(typeof(TEntity))) return (ITrakHoundEntityClient<TEntity>)Objects.State;
        //    else if (typeof(ITrakHoundObjectStatisticEntity).IsAssignableFrom(typeof(TEntity))) return (ITrakHoundEntityClient<TEntity>)Objects.Statistic;
        //    else if (typeof(ITrakHoundObjectStringEntity).IsAssignableFrom(typeof(TEntity))) return (ITrakHoundEntityClient<TEntity>)Objects.String;
        //    else if (typeof(ITrakHoundObjectTimestampEntity).IsAssignableFrom(typeof(TEntity))) return (ITrakHoundEntityClient<TEntity>)Objects.Timestamp;
        //    //else if (typeof(ITrakHoundObjectTimeGroupEntity).IsAssignableFrom(typeof(TEntity))) return (ITrakHoundEntityClient<TEntity>)Objects.TimeGroup;
        //    //else if (typeof(ITrakHoundObjectTimeHashEntity).IsAssignableFrom(typeof(TEntity))) return (ITrakHoundEntityClient<TEntity>)Objects.TimeHash;
        //    else if (typeof(ITrakHoundObjectTimeRangeEntity).IsAssignableFrom(typeof(TEntity))) return (ITrakHoundEntityClient<TEntity>)Objects.TimeRange;
        //    //else if (typeof(ITrakHoundObjectTimeSequenceEntity).IsAssignableFrom(typeof(TEntity))) return (ITrakHoundEntityClient<TEntity>)Objects.TimeSequence;
        //    //else if (typeof(ITrakHoundObjectTimeSetEntity).IsAssignableFrom(typeof(TEntity))) return (ITrakHoundEntityClient<TEntity>)Objects.TimeSet;
        //    //else if (typeof(ITrakHoundObjectTimeWikiEntity).IsAssignableFrom(typeof(TEntity))) return (ITrakHoundEntityClient<TEntity>)Objects.TimeWiki;
        //    else if (typeof(ITrakHoundObjectVocabularyEntity).IsAssignableFrom(typeof(TEntity))) return (ITrakHoundEntityClient<TEntity>)Objects.Vocabulary;
        //    //else if (typeof(ITrakHoundObjectWikiEntity).IsAssignableFrom(typeof(TEntity))) return (ITrakHoundEntityClient<TEntity>)Objects.Wiki;

        //    return null;
        //}


        public void AddMiddleware(ITrakHoundEntitiesClientMiddleware middleware)
        {
            if (middleware != null && !string.IsNullOrEmpty(middleware.Id))
            {
                lock (_lock)
                {
                    _middleware.Remove(middleware.Id);
                    _middleware.Add(middleware.Id, middleware);
                }
            }
        }

        public ITrakHoundEntity Process(ITrakHoundEntity entity, TrakHoundOperationMode operationMode)
        {
            if (entity != null)
            {
                IEnumerable<ITrakHoundEntitiesClientMiddleware> middlewares;
                lock (_lock) middlewares = _middleware.Values;
                if (!middlewares.IsNullOrEmpty())
                {
                    var outputEntity = entity;
                    foreach (var middleware in middlewares.Reverse())
                    {
                        outputEntity = middleware.Process(outputEntity, operationMode);
                        if (outputEntity == null) break;
                    }
                    return outputEntity;
                }
            }

            return entity;
        }

        //public TEntity Process<TEntity>(TEntity entity, TrakHoundOperationMode operationMode) where TEntity : ITrakHoundEntity
        //{
        //    if (entity != null)
        //    {
        //        IEnumerable<ITrakHoundEntitiesClientMiddleware> middlewares;
        //        lock (_lock) middlewares = _middleware.Values;
        //        if (!middlewares.IsNullOrEmpty())
        //        {
        //            var outputEntity = entity;
        //            foreach (var middleware in middlewares.Reverse())
        //            {
        //                outputEntity = (TEntity)middleware.Process(outputEntity, operationMode);
        //                if (outputEntity == null) break;
        //            }
        //            return outputEntity;
        //        }
        //    }

        //    return entity;
        //}

        //public IEnumerable<ITrakHoundEntity> Process(IEnumerable<ITrakHoundEntity> entities, TrakHoundOperationMode operationMode)
        //{
        //    if (!entities.IsNullOrEmpty())
        //    {
        //        //IEnumerable<ITrakHoundEntitiesClientMiddleware> middlewares;
        //        //lock (_lock) middlewares = _middleware.Values;
        //        //if (!middlewares.IsNullOrEmpty())
        //        //{
        //        //    IEnumerable<ITrakHoundEntity> outputEntities = entities;
        //        //    foreach (var middleware in middlewares.Reverse())
        //        //    {
        //        //        outputEntities = middleware.Process(outputEntities, operationMode);
        //        //    }
        //        //    return outputEntities;
        //        //}
        //    }

        //    return entities;
        //}

        //public IEnumerable<TEntity> Process<TEntity>(IEnumerable<TEntity> entities, TrakHoundOperationMode operationMode) where TEntity : ITrakHoundEntity
        //{
        //    if (!entities.IsNullOrEmpty())
        //    {
        //        IEnumerable<ITrakHoundEntitiesClientMiddleware> middlewares;
        //        lock (_lock) middlewares = _middleware.Values;
        //        if (!middlewares.IsNullOrEmpty())
        //        {
        //            IEnumerable<TEntity> outputEntities = entities;
        //            foreach (var middleware in middlewares)
        //            {
        //                var x = new List<TEntity>();
        //                foreach (var entity in outputEntities)
        //                {
        //                    var processedEntity = middleware.Process((TEntity)entity, operationMode);
        //                    if (processedEntity != null) x.Add((TEntity)processedEntity);
        //                }
        //                outputEntities = x;

        //                //var processedEntities = middleware.Process((TEntity)outputEntities, operationMode);
        //                //if (!processedEntities.IsNullOrEmpty())
        //                //{
        //                //    outputEntities = processedEntities;
        //                //};
        //            }
        //            return outputEntities;
        //        }

        //        //var outputEntities = new List<TEntity>();
        //        //foreach (var entity in entities)
        //        //{
        //        //    var outputEntity = Process(entity, operationMode);
        //        //    if (outputEntity != null) outputEntities.Add(outputEntity);
        //        //}
        //        //return outputEntities;
        //    }

        //    return entities;
        //}


        public async Task OnBeforePublish(IEnumerable<ITrakHoundEntity> entities, TrakHoundOperationMode operationMode) 
        {
            if (!entities.IsNullOrEmpty())
            {
                IEnumerable<ITrakHoundEntitiesClientMiddleware> middlewares;
                lock (_lock) middlewares = _middleware.Values;
                if (!middlewares.IsNullOrEmpty())
                {
                    foreach (var middleware in middlewares.Reverse())
                    {
                        await middleware.OnBeforePublish();
                        await middleware.OnBeforePublish(entities, operationMode);
                    }
                }
            }
        }

        public async Task OnAfterPublish(IEnumerable<ITrakHoundEntity> entities, TrakHoundOperationMode operationMode) 
        {
            if (!entities.IsNullOrEmpty())
            {
                IEnumerable<ITrakHoundEntitiesClientMiddleware> middlewares;
                lock (_lock) middlewares = _middleware.Values;
                if (!middlewares.IsNullOrEmpty())
                {
                    foreach (var middleware in middlewares.Reverse())
                    {
                        await middleware.OnAfterPublish();
                        await middleware.OnAfterPublish(entities, operationMode);
                    }
                }
            }
        }
    }
}
