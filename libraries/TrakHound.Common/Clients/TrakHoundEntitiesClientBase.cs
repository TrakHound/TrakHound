// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrakHound.Entities;
using TrakHound.Entities.Collections;
using TrakHound.Requests;
using TrakHound.Serialization;

namespace TrakHound.Clients
{
    public abstract partial class TrakHoundEntitiesClientBase
    {
        public const string DefaultApiRoute = "Entities";
        private const int _defaultSkip = 0;
        private const int _defaultTake = 1000;
        private const SortOrder _defaultSortOrder = SortOrder.Ascending;

        private readonly Dictionary<string, ITrakHoundClientMiddleware> _middleware = new Dictionary<string, ITrakHoundClientMiddleware>();
        private readonly object _lock = new object();

        protected ITrakHoundApiClient ApiClient;
        protected ITrakHoundSystemEntitiesClient EntitiesClient;


        public string ApiRoute { get; set; } = DefaultApiRoute;

        public IEnumerable<ITrakHoundClientMiddleware> Middleware => _middleware.Values;


        public async Task<IEnumerable<TReturn>> Query<TReturn>(string query)
        {
            if (!string.IsNullOrEmpty(query))
            {
                var response = await EntitiesClient.Query(query);
                if (!response.Results.IsNullOrEmpty())
                {
                    var collection = response.GetEntityCollection();
                    if (collection != null)
                    {
                        if (!collection.Objects.Objects.IsNullOrEmpty())
                        {
                            var targetObjectUuids = collection.Objects.Objects.Select(o => o.Uuid);
                            var targetObjects = await EntitiesClient.Objects.ReadByUuid(targetObjectUuids);

                            var contentObjectUuids = await GetContentObjectUuids<TReturn>(targetObjects?.Select(o => o.Path));
                            var contentObjects = await EntitiesClient.Objects.Query(contentObjectUuids);

                            var contentCollection = await GetObjectContent(EntitiesClient, targetObjects, contentObjects);
                            return TrakHoundSerializer.Deserialize<TReturn>(contentCollection, targetObjectUuids);
                        }
                    }
                }
            }

            return default;
        }

        public async Task<IEnumerable<TrakHoundObject>> QueryObjects(string query, int skip = _defaultSkip, int take = _defaultTake)
        {
            if (!string.IsNullOrEmpty(query))
            {
                var response = await EntitiesClient.Query(query);
                if (!response.Results.IsNullOrEmpty())
                {
                    var collection = response.GetEntityCollection();
                    if (collection != null)
                    {
                        var paths = collection.Objects.Objects.Select(o => $"uuid:{o.Uuid}");
                        return await GetObjects(paths, skip, take);
                    }
                }
            }

            return null;
        }

        public async Task<TReturn> GetSingle<TReturn>(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                var targetObjects = await EntitiesClient.Objects.Query(path);
                if (!targetObjects.IsNullOrEmpty())
                {
                    var targetObjectUuids = targetObjects.Select(o => o.Uuid);

                    var contentObjectUuids = await GetContentObjectUuids<TReturn>(targetObjects?.Select(o => o.GetAbsolutePath()));
                    var contentObjects = await EntitiesClient.Objects.Query(contentObjectUuids);

                    var contentCollection = new TrakHoundEntityCollection();
                    contentCollection.Add(await GetObjectContent(EntitiesClient, targetObjects, contentObjects));

                    // Root Objects
                    var rootObjectUuids = new List<string>();
                    if (!targetObjects.IsNullOrEmpty()) rootObjectUuids.AddRange(targetObjectUuids);
                    if (!contentObjects.IsNullOrEmpty()) rootObjectUuids.AddRange(contentObjects.Select(o => o.Uuid));
                    contentCollection.Add(await EntitiesClient.Objects.QueryRootByChildUuid(rootObjectUuids), false);

                    // Metadata
                    var metadataUuids = new List<string>();
                    if (!targetObjects.IsNullOrEmpty()) metadataUuids.AddRange(targetObjects.Select(o => o.Uuid));
                    if (!contentObjects.IsNullOrEmpty()) metadataUuids.AddRange(contentObjects.Select(o => o.Uuid));
                    contentCollection.Add(await EntitiesClient.Objects.Metadata.QueryByEntityUuid(metadataUuids.Distinct()), false);

                    // Definitions
                    var definitionUuids = new List<string>();
                    if (!targetObjects.IsNullOrEmpty()) definitionUuids.AddRange(targetObjects.Select(o => o.DefinitionUuid));
                    if (!contentObjects.IsNullOrEmpty()) definitionUuids.AddRange(contentObjects.Select(o => o.DefinitionUuid));
                    contentCollection.Add(await EntitiesClient.Definitions.ReadByUuid(definitionUuids.Distinct()), false);

                    var results = TrakHoundSerializer.Deserialize<TReturn>(contentCollection, targetObjectUuids);
                    if (!results.IsNullOrEmpty()) return results.FirstOrDefault();
                }
            }

            return default;
        }

        public async Task<IEnumerable<TReturn>> Get<TReturn>(string path, int skip = _defaultSkip, int take = _defaultTake)
        {
            if (!string.IsNullOrEmpty(path))
            {
                var targetObjects = await EntitiesClient.Objects.Query(path, skip, take);
                if (!targetObjects.IsNullOrEmpty())
                {
                    var targetObjectUuids = targetObjects.Select(o => o.Uuid);

                    var contentObjectUuids = await GetContentObjectUuids<TReturn>(targetObjects?.Select(o => o.GetAbsolutePath()));
                    var contentObjects = await EntitiesClient.Objects.Query(contentObjectUuids);

                    var contentCollection = new TrakHoundEntityCollection();
                    contentCollection.Add(await GetObjectContent(EntitiesClient, targetObjects, contentObjects));

                    // Root Objects
                    var rootObjectUuids = new List<string>();
                    if (!targetObjects.IsNullOrEmpty()) rootObjectUuids.AddRange(targetObjectUuids);
                    if (!contentObjects.IsNullOrEmpty()) rootObjectUuids.AddRange(contentObjects.Select(o => o.Uuid));
                    contentCollection.Add(await EntitiesClient.Objects.QueryRootByChildUuid(rootObjectUuids), false);

                    // Metadata
                    var metadataUuids = new List<string>();
                    if (!targetObjects.IsNullOrEmpty()) metadataUuids.AddRange(targetObjects.Select(o => o.Uuid));
                    if (!contentObjects.IsNullOrEmpty()) metadataUuids.AddRange(contentObjects.Select(o => o.Uuid));
                    contentCollection.Add(await EntitiesClient.Objects.Metadata.QueryByEntityUuid(metadataUuids.Distinct()), false);

                    // Definitions
                    var definitionUuids = new List<string>();
                    if (!targetObjects.IsNullOrEmpty()) definitionUuids.AddRange(targetObjects.Select(o => o.DefinitionUuid));
                    if (!contentObjects.IsNullOrEmpty()) definitionUuids.AddRange(contentObjects.Select(o => o.DefinitionUuid));
                    contentCollection.Add(await EntitiesClient.Definitions.ReadByUuid(definitionUuids.Distinct()), false);

                    return TrakHoundSerializer.Deserialize<TReturn>(contentCollection, targetObjectUuids);
                }
            }

            return default;
        }

        public async Task<IEnumerable<TReturn>> Get<TReturn>(IEnumerable<string> paths, int skip = _defaultSkip, int take = _defaultTake)
        {
            if (!paths.IsNullOrEmpty())
            {
                var targetObjects = await EntitiesClient.Objects.Query(paths, skip, take);
                if (!targetObjects.IsNullOrEmpty())
                {
                    var targetObjectUuids = targetObjects.Select(o => o.Uuid);

                    var contentObjectUuids = await GetContentObjectUuids<TReturn>(targetObjects?.Select(o => o.GetAbsolutePath()));
                    var contentObjects = await EntitiesClient.Objects.Query(contentObjectUuids);

                    var contentCollection = new TrakHoundEntityCollection();
                    contentCollection.Add(await GetObjectContent(EntitiesClient, targetObjects, contentObjects));

                    // Root Objects
                    var rootObjectUuids = new List<string>();
                    if (!targetObjects.IsNullOrEmpty()) rootObjectUuids.AddRange(targetObjectUuids);
                    if (!contentObjects.IsNullOrEmpty()) rootObjectUuids.AddRange(contentObjects.Select(o => o.Uuid));
                    contentCollection.Add(await EntitiesClient.Objects.QueryRootByChildUuid(rootObjectUuids), false);

                    // Metadata
                    var metadataUuids = new List<string>();
                    if (!targetObjects.IsNullOrEmpty()) metadataUuids.AddRange(targetObjects.Select(o => o.Uuid));
                    if (!contentObjects.IsNullOrEmpty()) metadataUuids.AddRange(contentObjects.Select(o => o.Uuid));
                    contentCollection.Add(await EntitiesClient.Objects.Metadata.QueryByEntityUuid(metadataUuids.Distinct()), false);

                    // Definitions
                    var definitionUuids = new List<string>();
                    if (!targetObjects.IsNullOrEmpty()) definitionUuids.AddRange(targetObjects.Select(o => o.DefinitionUuid));
                    if (!contentObjects.IsNullOrEmpty()) definitionUuids.AddRange(contentObjects.Select(o => o.DefinitionUuid));
                    contentCollection.Add(await EntitiesClient.Definitions.ReadByUuid(definitionUuids.Distinct()), false);

                    return TrakHoundSerializer.Deserialize<TReturn>(contentCollection, targetObjectUuids);
                }
            }

            return default;
        }

        public async Task<TReturn> GetByObjectUuid<TReturn>(string objectUuid)
        {
            //if (!string.IsNullOrEmpty(objectUuid))
            //{
            //    var obj = await EntitiesClient.Objects.ReadModelByUuid(objectUuid);
            //    if (obj != null)
            //    {
            //        var objs = new ITrakHoundObjectEntityModel[] { obj };
            //        var contentCollection = await ProcessGetContent(EntitiesClient, objs);
            //        var results = TrakHoundSerializer.Deserialize<TReturn>(contentCollection);
            //        if (!results.IsNullOrEmpty()) return results.FirstOrDefault();
            //    }
            //}

            return default;
        }

        public async Task<IEnumerable<TReturn>> GetByObjectUuid<TReturn>(IEnumerable<string> objectUuids)
        {
            //if (!objectUuids.IsNullOrEmpty())
            //{
            //    var objs = await EntitiesClient.Objects.ReadModelsByUuid(objectUuids);
            //    var contentCollection = await ProcessGetContent(EntitiesClient, objs);
            //}

            return default;
        }


        public async Task<ITrakHoundConsumer<IEnumerable<TReturn>>> Subscribe<TReturn>(string expression, int interval = 1000, int count = 1000)
        {
            if (!string.IsNullOrEmpty(expression))
            {
                var outputModels = new Dictionary<string, object>(); // ObjectUuid => TReturn

                var targetObjectModels = await EntitiesClient.Objects.Query(expression);
                var targetObjectUuids = targetObjectModels?.Select(o => o.Uuid);

                await InitializeSubscribe<TReturn>(targetObjectModels, outputModels);

                var subscriptionRequests = TrakHoundSerializer.CreateSubscriptionRequests<TReturn>(expression);

                var consumer = await EntitiesClient.Subscribe(subscriptionRequests, interval, count);
                if (consumer != null)
                {
                    var serializerConsumer = new TrakHoundConsumer<TrakHoundEntityCollection, IEnumerable<TReturn>>(consumer);
                    serializerConsumer.InitialValue = GetTargetModels<TReturn>(targetObjectUuids, outputModels);
                    serializerConsumer.OnReceived = (collection) =>
                    {
                        var results = TrakHoundSerializer.DeserializeObjectsToResult<TReturn>(collection, targetObjectUuids, outputModels);
                        if (!results.IsNullOrEmpty())
                        {
                            foreach (var result in results)
                            {
                                outputModels.Remove(result.ObjectUuid);
                                outputModels.Add(result.ObjectUuid, result.Model);
                            }

                            return GetTargetModels<TReturn>(targetObjectUuids, outputModels);
                        }

                        return null;
                    };

                    return serializerConsumer;
                }
            }

            return default;
        }

        private static IEnumerable<TOutput> GetTargetModels<TOutput>(IEnumerable<string> targetUuids, Dictionary<string, object> outputModels)
        {
            if (!targetUuids.IsNullOrEmpty() && !outputModels.IsNullOrEmpty())
            {
                var returnModels = new List<TOutput>();
                foreach (var outputModel in outputModels)
                {
                    if (targetUuids.Contains(outputModel.Key) && outputModel.Value.GetType() == typeof(TOutput))
                    {
                        returnModels.Add((TOutput)outputModel.Value);
                    }
                }
                return returnModels;
            }

            return null;
        }

        private async Task InitializeSubscribe<TReturn>(IEnumerable<ITrakHoundObjectEntity> targetModels, Dictionary<string, object> outputModels)
        {
            if (!targetModels.IsNullOrEmpty())
            {
                //var contentCollection = await ProcessGetContent(EntitiesClient, targetModels);

                //var results = TrakHoundSerializer.DeserializeObjectsToResult<TReturn>(contentCollection, outputModels, targetModels.Select(o => o.Uuid));
                //if (!results.IsNullOrEmpty())
                //{
                //    foreach (var result in results)
                //    {
                //        outputModels.Remove(result.ObjectUuid);
                //        outputModels.Add(result.ObjectUuid, result.Model);
                //    }
                //}
            }
        }

        private async Task InitializeSubscribe<TReturn>(string query, Dictionary<string, object> outputModels)
        {
            if (!string.IsNullOrEmpty(query))
            {
                //var response = await EntitiesClient.Query(query);
                //if (response != null && response.Entities != null)
                //{
                //    var collection = response.Entities;
                //    var objectUuids = new List<string>();

                //    if (!collection.TargetUuids.IsNullOrEmpty()) objectUuids.AddRange(collection.TargetUuids);
                //    if (!collection.ComponentTargets.IsNullOrEmpty()) objectUuids.AddRange(collection.ComponentTargets.Select(o => o.AssemblyUuid));

                //    //if (!objectUuids.IsNullOrEmpty())
                //    //{
                //    //    var objectModels = await EntitiesClient.Objects.ReadModelsByUuid(objectUuids);
                //    //    var contentCollection = await ProcessGetContent(EntitiesClient, objectModels);

                //    //    var results = TrakHoundSerializer.DeserializeToResult<TReturn>(collection, outputModels);
                //    //    if (!results.IsNullOrEmpty())
                //    //    {
                //    //        foreach (var result in results)
                //    //        {
                //    //            outputModels.Remove(result.ObjectUuid);
                //    //            outputModels.Add(result.ObjectUuid, result.Model);
                //    //        }
                //    //    }
                //    //}
                //}
            }
        }


        public async Task<IEnumerable<string>> GetContentObjectUuids<TReturn>(IEnumerable<string> targetPaths)
        {
            if (!targetPaths.IsNullOrEmpty())
            {
                var contentObjectUuids = new List<string>();
                foreach (var targetPath in targetPaths)
                {
                    var objectUuids = TrakHoundSerializer.CreateContentQueries(typeof(TReturn), targetPath);
                    if (!objectUuids.IsNullOrEmpty())
                    {
                        contentObjectUuids.AddRange(objectUuids);
                    }
                }
                return contentObjectUuids;
            }

            return null;
        }

        public async Task<TrakHoundEntityCollection> GetObjectContent(IEnumerable<ITrakHoundObjectEntity> targetObjs, IEnumerable<ITrakHoundObjectEntity> contentObjs)
        {
            return await GetObjectContent(EntitiesClient, targetObjs, contentObjs);
        }

        public async Task<TrakHoundEntityCollection> GetObjectContent(ITrakHoundSystemEntitiesClient entitiesClient, IEnumerable<ITrakHoundObjectEntity> targetObjs, IEnumerable<ITrakHoundObjectEntity> contentObjs)
        {
            if (entitiesClient != null && !targetObjs.IsNullOrEmpty())
            {
                var collection = new TrakHoundEntityCollection();
                collection.Add(targetObjs);

                if (!contentObjs.IsNullOrEmpty())
                {
                    collection.Add(contentObjs, false);

                    var contentTypeDictionary = new TrakHoundObjectContentTypeDictionary();
                    contentTypeDictionary.Add(contentObjs);

                    var tasks = new List<Task<IEnumerable<ITrakHoundEntity>>>();

                    // Get Metadata
                    tasks.Add(GetEntities(entitiesClient.Objects.Metadata.QueryByEntityUuid(targetObjs?.Select(o => o.Uuid))));

                    var assignmentObjs = contentTypeDictionary.Get(TrakHoundObjectContentType.Assignment);
                    if (!assignmentObjs.IsNullOrEmpty()) tasks.Add(GetEntities(entitiesClient.Objects.Assignment.CurrentByAssigneeUuid(assignmentObjs?.Select(o => o.Uuid))));

                    var booleanObjs = contentTypeDictionary.Get(TrakHoundObjectContentType.Boolean);
                    if (!booleanObjs.IsNullOrEmpty()) tasks.Add(GetEntities(entitiesClient.Objects.Boolean.QueryByObjectUuid(booleanObjs?.Select(o => o.Uuid))));

                    var durationObjs = contentTypeDictionary.Get(TrakHoundObjectContentType.Duration);
                    if (!durationObjs.IsNullOrEmpty()) tasks.Add(GetEntities(entitiesClient.Objects.Duration.QueryByObjectUuid(durationObjs?.Select(o => o.Uuid))));

                    var eventObjs = contentTypeDictionary.Get(TrakHoundObjectContentType.Event);
                    if (!eventObjs.IsNullOrEmpty()) tasks.Add(GetEntities(entitiesClient.Objects.Event.LatestByObjectUuid(eventObjs?.Select(o => o.Uuid))));

                    var groupObjs = contentTypeDictionary.Get(TrakHoundObjectContentType.Group);
                    if (!groupObjs.IsNullOrEmpty())
                    {
                        var groupEntities = await entitiesClient.Objects.Group.QueryByGroupUuid(groupObjs?.Select(o => o.Uuid));
                        if (!groupEntities.IsNullOrEmpty())
                        {
                            var groupMemberObjects = await entitiesClient.Objects.ReadByUuid(groupEntities.Select(o => o.MemberUuid).Distinct());
                            collection.Add(groupEntities, false);
                            collection.Add(groupMemberObjects, false);
                            collection.Add(await GetContent(groupMemberObjects), false);
                        }
                    }

                    var hashObjs = contentTypeDictionary.Get(TrakHoundObjectContentType.Hash);
                    if (!hashObjs.IsNullOrEmpty()) tasks.Add(GetEntities(entitiesClient.Objects.Hash.QueryByObjectUuid(hashObjs?.Select(o => o.Uuid))));

                    var numberObjs = contentTypeDictionary.Get(TrakHoundObjectContentType.Number);
                    if (!numberObjs.IsNullOrEmpty()) tasks.Add(GetEntities(entitiesClient.Objects.Number.QueryByObjectUuid(numberObjs?.Select(o => o.Uuid))));

                    var observationObjs = contentTypeDictionary.Get(TrakHoundObjectContentType.Observation);
                    if (!observationObjs.IsNullOrEmpty()) tasks.Add(GetEntities(entitiesClient.Objects.Observation.LatestByObjectUuid(observationObjs?.Select(o => o.Uuid))));

                    var queueObjs = contentTypeDictionary.Get(TrakHoundObjectContentType.Queue);
                    if (!queueObjs.IsNullOrEmpty())
                    {
                        var queueEntities = await entitiesClient.Objects.Queue.QueryByQueueUuid(queueObjs?.Select(o => o.Uuid));
                        if (!queueEntities.IsNullOrEmpty())
                        {
                            var queueMemberObjects = await entitiesClient.Objects.ReadByUuid(queueEntities.Select(o => o.MemberUuid).Distinct());
                            collection.Add(queueEntities, false);
                            collection.Add(queueMemberObjects, false);
                            collection.Add(await GetContent(queueMemberObjects), false);
                        }
                    }

                    var referenceObjs = contentTypeDictionary.Get(TrakHoundObjectContentType.Reference);
                    if (!referenceObjs.IsNullOrEmpty())
                    {
                        var referenceEntities = await entitiesClient.Objects.Reference.QueryByObjectUuid(referenceObjs?.Select(o => o.Uuid));
                        if (!referenceEntities.IsNullOrEmpty())
                        {
                            var referenceTargetObjects = await entitiesClient.Objects.ReadByUuid(referenceEntities.Select(o => o.TargetUuid).Distinct());
                            collection.Add(referenceEntities, false);
                            collection.Add(referenceTargetObjects, false);
                            collection.Add(await GetContent(referenceTargetObjects), false);
                        }
                    }

                    var setObjs = contentTypeDictionary.Get(TrakHoundObjectContentType.Set);
                    if (!setObjs.IsNullOrEmpty()) tasks.Add(GetEntities(entitiesClient.Objects.Set.QueryByObjectUuid(setObjs?.Select(o => o.Uuid))));

                    var stateObjs = contentTypeDictionary.Get(TrakHoundObjectContentType.State);
                    if (!stateObjs.IsNullOrEmpty())
                    {
                        var stateEntities = await entitiesClient.Objects.State.LatestByObjectUuid(stateObjs?.Select(o => o.Uuid));
                        if (!stateEntities.IsNullOrEmpty())
                        {
                            var stateDefinitionUuids = stateEntities.Select(o => o.DefinitionUuid).Distinct();
                            var stateDefinitions = await entitiesClient.Definitions.ReadByUuid(stateDefinitionUuids);
                            collection.Add(stateObjs, false);
                            collection.Add(stateEntities, false);
                            collection.Add(stateDefinitions, false);
                        }
                    }

                    //var statisticObjs = fObjs.Where(o => o.ContentType == TrakHoundObjectContentType.Statistic.ToString());
                    //if (!statisticObjs.IsNullOrEmpty()) collection.Add(await entitiesClient.Objects.Statistics.LatestByObjectUuid(statisticObjs?.Select(o => o.Uuid), 0, long.MaxValue), false);

                    var stringObjs = contentTypeDictionary.Get(TrakHoundObjectContentType.String);
                    if (!stringObjs.IsNullOrEmpty()) tasks.Add(GetEntities(entitiesClient.Objects.String.QueryByObjectUuid(stringObjs?.Select(o => o.Uuid))));

                    var timeRangeObjs = contentTypeDictionary.Get(TrakHoundObjectContentType.TimeRange);
                    if (!timeRangeObjs.IsNullOrEmpty()) tasks.Add(GetEntities(entitiesClient.Objects.TimeRange.QueryByObjectUuid(timeRangeObjs?.Select(o => o.Uuid))));

                    var timestampObjs = contentTypeDictionary.Get(TrakHoundObjectContentType.Timestamp);
                    if (!timestampObjs.IsNullOrEmpty()) tasks.Add(GetEntities(entitiesClient.Objects.Timestamp.QueryByObjectUuid(timestampObjs?.Select(o => o.Uuid))));

                    var vocabularyObjs = contentTypeDictionary.Get(TrakHoundObjectContentType.Vocabulary);
                    if (!vocabularyObjs.IsNullOrEmpty()) tasks.Add(GetEntities(entitiesClient.Objects.Vocabulary.QueryByObjectUuid(vocabularyObjs?.Select(o => o.Uuid))));

                    var vocabularySetObjs = contentTypeDictionary.Get(TrakHoundObjectContentType.VocabularySet);
                    if (!vocabularySetObjs.IsNullOrEmpty()) tasks.Add(GetEntities(entitiesClient.Objects.VocabularySet.QueryByObjectUuid(vocabularySetObjs?.Select(o => o.Uuid))));


                    var taskResults = await Task.WhenAll(tasks);
                    if (!taskResults.IsNullOrEmpty())
                    {
                        foreach (var taskResult in taskResults) collection.Add(taskResult, false);


                        var additionalObjectUuids = new HashSet<string>();
                        var additionalDefinitionUuids = new HashSet<string>();

                        if (!collection.Objects.Assignments.IsNullOrEmpty())
                        {
                            foreach (var entity in collection.Objects.Assignments)
                            {
                                additionalObjectUuids.Add(entity.MemberUuid);
                            }
                        }

                        if (!collection.Objects.Events.IsNullOrEmpty())
                        {
                            foreach (var entity in collection.Objects.Events)
                            {
                                additionalObjectUuids.Add(entity.TargetUuid);
                            }
                        }

                        if (!collection.Objects.Groups.IsNullOrEmpty())
                        {
                            foreach (var entity in collection.Objects.Groups)
                            {
                                additionalObjectUuids.Add(entity.MemberUuid);
                            }
                        }

                        if (!collection.Objects.References.IsNullOrEmpty())
                        {
                            foreach (var entity in collection.Objects.References)
                            {
                                additionalObjectUuids.Add(entity.TargetUuid);
                            }
                        }

                        if (!collection.Objects.States.IsNullOrEmpty())
                        {
                            foreach (var entity in collection.Objects.States)
                            {
                                additionalDefinitionUuids.Add(entity.DefinitionUuid);
                            }
                        }

                        if (!additionalObjectUuids.IsNullOrEmpty())
                        {
                            collection.Add(await EntitiesClient.Objects.ReadByUuid(additionalObjectUuids), false);
                        }

                        if (!additionalDefinitionUuids.IsNullOrEmpty())
                        {
                            collection.Add(await EntitiesClient.Definitions.ReadByUuid(additionalDefinitionUuids), false);
                        }
                    }
                }

                return collection;
            }

            return default;
        }

        private async Task<IEnumerable<ITrakHoundEntity>> GetContent(IEnumerable<ITrakHoundObjectEntity> objs)
        {
            if (!objs.IsNullOrEmpty())
            {
                var collection = new TrakHoundEntityCollection();
                collection.Add((IEnumerable<ITrakHoundEntity>)objs);

                var contentTypeDictionary = new TrakHoundObjectContentTypeDictionary();
                contentTypeDictionary.Add(objs);

                var tasks = new List<Task<IEnumerable<ITrakHoundEntity>>>();

                // Get Metadata
                tasks.Add(GetEntities(EntitiesClient.Objects.Metadata.QueryByEntityUuid(objs?.Select(o => o.Uuid))));

                var assignmentObjs = contentTypeDictionary.Get(TrakHoundObjectContentType.Assignment);
                if (!assignmentObjs.IsNullOrEmpty()) tasks.Add(GetEntities(EntitiesClient.Objects.Assignment.CurrentByAssigneeUuid(assignmentObjs?.Select(o => o.Uuid))));

                var booleanObjs = contentTypeDictionary.Get(TrakHoundObjectContentType.Boolean);
                if (!booleanObjs.IsNullOrEmpty()) tasks.Add(GetEntities(EntitiesClient.Objects.Boolean.QueryByObjectUuid(booleanObjs?.Select(o => o.Uuid))));

                var durationObjs = contentTypeDictionary.Get(TrakHoundObjectContentType.Duration);
                if (!durationObjs.IsNullOrEmpty()) tasks.Add(GetEntities(EntitiesClient.Objects.Duration.QueryByObjectUuid(durationObjs?.Select(o => o.Uuid))));

                var eventObjs = contentTypeDictionary.Get(TrakHoundObjectContentType.Event);
                if (!eventObjs.IsNullOrEmpty()) tasks.Add(GetEntities(EntitiesClient.Objects.Event.LatestByObjectUuid(eventObjs?.Select(o => o.Uuid))));

                var groupObjs = contentTypeDictionary.Get(TrakHoundObjectContentType.Group);
                if (!groupObjs.IsNullOrEmpty()) tasks.Add(GetEntities(EntitiesClient.Objects.Group.QueryByGroupUuid(groupObjs?.Select(o => o.Uuid))));

                var numberObjs = contentTypeDictionary.Get(TrakHoundObjectContentType.Number);
                if (!numberObjs.IsNullOrEmpty()) tasks.Add(GetEntities(EntitiesClient.Objects.Number.QueryByObjectUuid(numberObjs?.Select(o => o.Uuid))));

                var observationObjs = contentTypeDictionary.Get(TrakHoundObjectContentType.Observation);
                if (!observationObjs.IsNullOrEmpty()) tasks.Add(GetEntities(EntitiesClient.Objects.Observation.LatestByObjectUuid(observationObjs?.Select(o => o.Uuid))));

                var queueObjs = contentTypeDictionary.Get(TrakHoundObjectContentType.Queue);
                if (!queueObjs.IsNullOrEmpty()) tasks.Add(GetEntities(EntitiesClient.Objects.Queue.QueryByQueueUuid(queueObjs?.Select(o => o.Uuid))));

                var referenceObjs = contentTypeDictionary.Get(TrakHoundObjectContentType.Reference);
                if (!referenceObjs.IsNullOrEmpty())
                {
                    var referenceEntities = await EntitiesClient.Objects.Reference.QueryByObjectUuid(referenceObjs?.Select(o => o.Uuid));
                    if (!referenceEntities.IsNullOrEmpty())
                    {
                        var referenceTargetObjects = await EntitiesClient.Objects.ReadByUuid(referenceEntities.Select(o => o.TargetUuid).Distinct());
                        collection.Add(referenceEntities, false);
                        collection.Add(referenceTargetObjects, false);
                        collection.Add(await GetContent(referenceTargetObjects), false);
                    }
                }

                var setObjs = contentTypeDictionary.Get(TrakHoundObjectContentType.Set);
                if (!setObjs.IsNullOrEmpty()) tasks.Add(GetEntities(EntitiesClient.Objects.Set.QueryByObjectUuid(setObjs?.Select(o => o.Uuid))));

                var stateObjs = contentTypeDictionary.Get(TrakHoundObjectContentType.State);
                if (!stateObjs.IsNullOrEmpty()) tasks.Add(GetEntities(EntitiesClient.Objects.State.LatestByObjectUuid(stateObjs?.Select(o => o.Uuid))));

                var stringObjs = contentTypeDictionary.Get(TrakHoundObjectContentType.String);
                if (!stringObjs.IsNullOrEmpty()) tasks.Add(GetEntities(EntitiesClient.Objects.String.QueryByObjectUuid(stringObjs?.Select(o => o.Uuid))));

                var timeRangeObjs = contentTypeDictionary.Get(TrakHoundObjectContentType.TimeRange);
                if (!timeRangeObjs.IsNullOrEmpty()) tasks.Add(GetEntities(EntitiesClient.Objects.TimeRange.QueryByObjectUuid(timeRangeObjs?.Select(o => o.Uuid))));

                var timestampObjs = contentTypeDictionary.Get(TrakHoundObjectContentType.Timestamp);
                if (!timestampObjs.IsNullOrEmpty()) tasks.Add(GetEntities(EntitiesClient.Objects.Timestamp.QueryByObjectUuid(timestampObjs?.Select(o => o.Uuid))));


                var taskResults = await Task.WhenAll(tasks);
                if (!taskResults.IsNullOrEmpty())
                {
                    foreach (var taskResult in taskResults) collection.Add(taskResult, false);


                    var additionalObjectUuids = new HashSet<string>();

                    if (!collection.Objects.Assignments.IsNullOrEmpty())
                    {
                        foreach (var entity in collection.Objects.Assignments)
                        {
                            additionalObjectUuids.Add(entity.MemberUuid);
                        }
                    }

                    if (!collection.Objects.Events.IsNullOrEmpty())
                    {
                        foreach (var entity in collection.Objects.Events)
                        {
                            additionalObjectUuids.Add(entity.TargetUuid);
                        }
                    }

                    if (!collection.Objects.Groups.IsNullOrEmpty())
                    {
                        foreach (var entity in collection.Objects.Groups)
                        {
                            additionalObjectUuids.Add(entity.MemberUuid);
                        }
                    }

                    if (!collection.Objects.References.IsNullOrEmpty())
                    {
                        foreach (var entity in collection.Objects.References)
                        {
                            additionalObjectUuids.Add(entity.TargetUuid);
                        }
                    }

                    if (!additionalObjectUuids.IsNullOrEmpty())
                    {
                        collection.Add(await EntitiesClient.Objects.ReadByUuid(additionalObjectUuids), false);
                    }
                }

                return collection.GetEntities();
            }

            return default;
        }

        private async Task<IEnumerable<ITrakHoundEntity>> GetEntities<TEntity>(Task<IEnumerable<TEntity>> task) where TEntity : ITrakHoundEntity
        {
            if (task != null)
            {
                var x = await task;
                if (!x.IsNullOrEmpty())
                {
                    var results = new List<ITrakHoundEntity>();
                    foreach (var y in x)
                    {
                        results.Add(y);
                    }
                    return results;
                }
            }

            return null;
        }


        public async Task<bool> Publish(object obj, bool async = false, string routerId = null)
        {
            if (obj != null)
            {
                var transaction = TrakHoundSerializer.Serialize(obj);
                if (transaction != null)
                {
                    return await Publish(transaction, async, routerId);
                }
            }

            return false;
        }

        public async Task<bool> Publish(TrakHoundEntityTransaction transaction, bool async = false, string routerId = null)
        {
            if (transaction != null)
            {
                var route = Url.Combine(ApiRoute, "entries");

                var parameters = new Dictionary<string, string>();
                parameters["async"] = async.ToString();
                parameters["routerId"] = routerId;

                var response = await ApiClient.Publish(route, transaction, queryParameters: parameters);
                await PublishAfter();
                return response.Success;
            }

            return false;
        }

        private async Task PublishAfter()
        {
            await OnAfterPublish();
        }


        public async Task<string> GetJson(string path, string routerId = null)
        {
            if (!string.IsNullOrEmpty(path))
            {
                var route = "json";

                var parameters = new Dictionary<string, string>();
                parameters["path"] = path;
                parameters["routerId"] = routerId;

                return await ApiClient.QueryString(route, parameters);
            }

            return default;
        }

        public async Task<TResult> GetJson<TResult>(string path, string routerId = null)
        {
            if (!string.IsNullOrEmpty(path))
            {
                var route = "json";

                var parameters = new Dictionary<string, string>();
                parameters["path"] = path;
                parameters["routerId"] = routerId;

                return await ApiClient.QueryJson<TResult>(route, parameters);
            }

            return default;
        }

        public async Task<bool> PublishJson(string basePath, string json, bool async = false, string routerId = null)
        {
            if (!string.IsNullOrEmpty(json))
            {
                var route = "json";

                var parameters = new Dictionary<string, string>();
                parameters["basePath"] = basePath;
                parameters["async"] = async.ToString();
                parameters["routerId"] = routerId;

                var requestBody = System.Text.Encoding.UTF8.GetBytes(json);

                var response = await ApiClient.Publish(route, requestBody, "text/plain", parameters);
                await PublishAfter();
                return response.Success;
            }

            return false;
        }



        public async Task<bool> CopyToClipboard(string clipboardId, string path)
        {
            if (!string.IsNullOrEmpty(clipboardId) && !string.IsNullOrEmpty(path))
            {
                var route = $"{DefaultApiRoute}/clipboard/copy";

                var parameters = new Dictionary<string, string>();
                parameters["clipboardId"] = clipboardId;
                parameters["path"] = path;

                var response = await ApiClient.Query(route, parameters);
                return response.Success;
            }

            return false;
        }

        public async Task<bool> PasteClipboard(string clipboardId, string destinationBasePath)
        {
            if (!string.IsNullOrEmpty(clipboardId) && !string.IsNullOrEmpty(destinationBasePath))
            {
                var route = $"{DefaultApiRoute}/clipboard/paste";

                var parameters = new Dictionary<string, string>();
                parameters["clipboardId"] = clipboardId;
                parameters["destinationBasePath"] = destinationBasePath;

                var response = await ApiClient.Publish(route, parameters);
                await PublishAfter();
                return response.Success;
            }

            return false;
        }

        public async Task<bool> DeleteClipboard(string clipboardId)
        {
            if (!string.IsNullOrEmpty(clipboardId))
            {
                var route = $"{DefaultApiRoute}/clipboard";

                var parameters = new Dictionary<string, string>();
                parameters["clipboardId"] = clipboardId;

                var response = await ApiClient.Delete(route, parameters);
                return response.Success;
            }

            return false;
        }



        public void AddMiddleware(ITrakHoundClientMiddleware middleware)
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


        public async Task OnBeforePublish()
        {
            IEnumerable<ITrakHoundClientMiddleware> middlewares;
            lock (_lock) middlewares = _middleware.Values;
            if (!middlewares.IsNullOrEmpty())
            {
                foreach (var middleware in middlewares.Reverse())
                {
                    await middleware.OnBeforePublish();
                }
            }
        }

        public async Task OnAfterPublish()
        {
            IEnumerable<ITrakHoundClientMiddleware> middlewares;
            lock (_lock) middlewares = _middleware.Values;
            if (!middlewares.IsNullOrEmpty())
            {
                foreach (var middleware in middlewares.Reverse())
                {
                    await middleware.OnAfterPublish();
                }
            }
        }
    }
}
