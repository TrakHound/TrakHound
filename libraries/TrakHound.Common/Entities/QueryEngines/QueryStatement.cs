// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TrakHound.Clients;
using TrakHound.Entities.Collections;
using TrakHound.Entities.QueryEngines.Evaluators;
using TrakHound.Entities.QueryEngines.Evaluators.Entities;

namespace TrakHound.Entities.QueryEngines
{
    public class QueryStatement
    {
        internal const int DefaultTake = 1000;


        public string Id { get; set; }

        public QueryScope Scope { get; set; }

        public string Command { get; set; }

        public string Target { get; set; }

        public List<QuerySelect> Select { get; set; }

        public List<QueryConditionGroup> ConditionGroups { get; set; }

        public long? Start { get; set; }

        public long? Stop { get; set; }

        public long? Span { get; set; }

        public string Range { get; set; }

        public int? Skip { get; set; }

        public int? Take { get; set; }

        public SortOrder? Order { get; set; }

        public IEnumerable<string> GroupBy { get; set; }


        public QueryStatement(QueryScope scope, string command) 
        {
            Id = Guid.NewGuid().ToString();
            Scope = scope;
            Command = command;
            Target = null;
            Select = new List<QuerySelect>();
            ConditionGroups = new List<QueryConditionGroup>();
            Start = null;
            Stop = null;
            Skip = null;
            Take = DefaultTake;
            Order = SortOrder.Ascending;
        }


        #region "Execute"

        public async Task<TrakHoundQueryResult[]> Execute(ITrakHoundSystemEntitiesClient client)
        {
            if (client != null)
            {
                if (!ConditionGroups.IsNullOrEmpty())
                {
                    //var queries = new List<QueryCondition>();
                    var queries = new List<ConditionObjectQuery>();
                    var indexQueries = new List<ConditionIndexQuery>();

                    var target = Scope.GetVariable(Target)?.Value;
                    //var query = "";
                    //var queryOperator = TrakHoundConditionOperator.EQUALS;

                    IEnumerable<string> targetObjectUuids = null;

                    foreach (var conditionGroup in ConditionGroups)
                    {
                        if (!conditionGroup.Conditions.IsNullOrEmpty())
                        {
                            foreach (var condition in conditionGroup.Conditions)
                            {
                                var conditionVariable = Scope.GetVariable(condition.Variable);
                                if (conditionVariable != null)
                                {
                                    var conditionTarget = GetFunctionTarget(conditionVariable.Value);
                                    if (conditionTarget != null)
                                    {
                                        var conditionValue = Scope.GetVariable(condition.Value);
                                        if (conditionValue != null)
                                        {
                                            IEnumerable<string> subTargetObjectUuids = null;

                                            conditionTarget = conditionTarget.TrimStart('[').TrimEnd(']');
                                            var indexTarget = TrakHoundPath.Combine(target, conditionTarget)?.ToLower().ToSHA256Hash();

                                            if (await IndexExists(client, indexTarget))
                                            {
                                                var indexRequest = new EntityIndexRequest();
                                                indexRequest.Target = indexTarget;
                                                indexRequest.Value = FormatValue(GetFunctionTarget(conditionValue.Value));

                                                var queryType = TrakHoundIndexQueryType.Equal;
                                                switch (condition.Operator)
                                                {
                                                    case TrakHoundConditionOperator.EQUALS: queryType = TrakHoundIndexQueryType.Equal; break;
                                                    case TrakHoundConditionOperator.NOT_EQUALS: queryType = TrakHoundIndexQueryType.NotEqual; break;
                                                    case TrakHoundConditionOperator.LIKE: queryType = TrakHoundIndexQueryType.Like; break;
                                                    case TrakHoundConditionOperator.GREATER_THAN: queryType = TrakHoundIndexQueryType.GreaterThan; break;
                                                    case TrakHoundConditionOperator.GREATER_THAN_OR_EQUAL: queryType = TrakHoundIndexQueryType.GreaterThanOrEqual; break;
                                                    case TrakHoundConditionOperator.LESS_THAN: queryType = TrakHoundIndexQueryType.LessThan; break;
                                                    case TrakHoundConditionOperator.LESS_THAN_OR_EQUAL: queryType = TrakHoundIndexQueryType.LessThanOrEqual; break;
                                                }
                                                indexRequest.QueryType = queryType;
                                                var indexRequests = new EntityIndexRequest[] { indexRequest };

                                                long skip = Skip != null ? Skip.Value : 0;
                                                long take = Take != null ? Take.Value : 0;
                                                SortOrder sortOrder = Order != null ? Order.Value : SortOrder.Ascending;

                                                subTargetObjectUuids = await client.Objects.QueryIndex(indexRequests, skip, take, sortOrder);
                                            }
                                            else
                                            {
                                                var targetQuery = TrakHoundPath.Combine(target, conditionTarget);
                                                var conditionTargetObjects = await client.Objects.Query(targetQuery);
                                                if (!conditionTargetObjects.IsNullOrEmpty())
                                                {
                                                    var evaluators = new List<IEvaluator>();
                                                    foreach (var conditionTargetObject in conditionTargetObjects)
                                                    {
                                                        var query = new ConditionObjectQuery();
                                                        query.TargetObjectUuid = conditionTargetObject.ParentUuid;
                                                        query.ConditionObject = conditionTargetObject;
                                                        query.ConditionGroup = conditionGroup;
                                                        query.Condition = condition;

                                                        var evaluator = EntityEvaluator.Create(query, conditionTargetObject);
                                                        if (evaluator != null) evaluators.Add(evaluator);
                                                    }

                                                    var evaluatorResults = new List<IEvaluatorResult>();

                                                    var results = await EntityEvaluator.Evaluate(this, client, evaluators);
                                                    if (!results.IsNullOrEmpty())
                                                    {
                                                        subTargetObjectUuids = results.Select(o => o.Query.TargetObjectUuid).Distinct();
                                                    }
                                                }
                                            }

                                            if (!subTargetObjectUuids.IsNullOrEmpty())
                                            {
                                                if (!targetObjectUuids.IsNullOrEmpty())
                                                {
                                                    switch (conditionGroup.GroupOperator)
                                                    {
                                                        case TrakHoundConditionGroupOperator.AND:
                                                            targetObjectUuids = targetObjectUuids.Intersect(subTargetObjectUuids);
                                                            break;

                                                        case TrakHoundConditionGroupOperator.OR:
                                                            var x = new List<string>();
                                                            x.AddRange(targetObjectUuids);
                                                            x.AddRange(subTargetObjectUuids);
                                                            targetObjectUuids = x;
                                                            break;
                                                    }
                                                }
                                                else
                                                {
                                                    targetObjectUuids = subTargetObjectUuids;
                                                }
                                            }
                                            else
                                            {
                                                if (conditionGroup.GroupOperator == TrakHoundConditionGroupOperator.AND)
                                                {
                                                    targetObjectUuids = null;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }




                                //break; // DEBUG




                                //var valueVariable = Scope.GetVariable(condition.Variable);
                                //if (valueVariable != null)
                                //{
                                //    var valueTarget = GetFunctionTarget(valueVariable.Value);
                                //    if (valueTarget != null) valueTarget = valueTarget.TrimStart('[').TrimEnd(']');

                                //    query = valueVariable.Value;
                                //    break;

                                //    //foreach (var targetObject in targetObjects)
                                //    //{
                                //    //    var query = new ConditionObjectQuery();
                                //    //    query.TargetObject = targetObject;
                                //    //    query.ConditionGroup = conditionGroup;
                                //    //    query.Condition = condition;
                                //    //    query.Query = $"uuid:{targetObject.Uuid}/{valueTarget}";
                                //    //    queries.Add(query);
                                //    //}
                                //}
                            }
                        }
                    }

                    //var stpw = Stopwatch.StartNew();
                    //var fstpw = Stopwatch.StartNew();

                    //IEnumerable<string> targetObjectUuids = null;

                    //var conditionGroupIds = indexQueries.Select(o => o.ConditionGroup.GroupId).Distinct();
                    //foreach (var conditionGroupId in conditionGroupIds)
                    //{
                    //    var conditionGroupQueries = indexQueries.Where(o => o.ConditionGroup.GroupId == conditionGroupId);

                    //    var indexRequests = new List<EntityIndexRequest>();
                    //    foreach (var indexQuery in conditionGroupQueries)
                    //    {
                    //        var indexRequest = new EntityIndexRequest();
                    //        indexRequest.Target = indexQuery.Target;
                    //        indexRequest.Value = FormatValue(indexQuery.Query);

                    //        var queryType = TrakHoundIndexQueryType.Equal;
                    //        switch (indexQuery.Operator)
                    //        {
                    //            case TrakHoundConditionOperator.EQUALS: queryType = TrakHoundIndexQueryType.Equal; break;
                    //            case TrakHoundConditionOperator.NOT_EQUALS: queryType = TrakHoundIndexQueryType.NotEqual; break;
                    //            case TrakHoundConditionOperator.LIKE: queryType = TrakHoundIndexQueryType.Like; break;
                    //            case TrakHoundConditionOperator.GREATER_THAN: queryType = TrakHoundIndexQueryType.GreaterThan; break;
                    //            case TrakHoundConditionOperator.GREATER_THAN_OR_EQUAL: queryType = TrakHoundIndexQueryType.GreaterThanOrEqual; break;
                    //            case TrakHoundConditionOperator.LESS_THAN: queryType = TrakHoundIndexQueryType.LessThan; break;
                    //            case TrakHoundConditionOperator.LESS_THAN_OR_EQUAL: queryType = TrakHoundIndexQueryType.LessThanOrEqual; break;
                    //        }
                    //        indexRequest.QueryType = queryType;

                    //        indexRequests.Add(indexRequest);
                    //    }


                    //    long skip = Skip != null ? Skip.Value : 0;
                    //    long take = Take != null ? Take.Value : 0;
                    //    SortOrder sortOrder = Order != null ? Order.Value : SortOrder.Ascending;

                    //    targetObjectUuids = await client.Objects.QueryIndex(indexRequests, skip, take, sortOrder);
                    //}

                    //foreach (var indexQuery in indexQueries)
                    //{
                    //    if (indexQuery.Target != null)
                    //    {
                    //        var queryType = TrakHoundIndexQueryType.Equal;
                    //        switch (indexQuery.Operator)
                    //        {
                    //            case TrakHoundConditionOperator.EQUALS: queryType = TrakHoundIndexQueryType.Equal; break;
                    //            case TrakHoundConditionOperator.NOT_EQUALS: queryType = TrakHoundIndexQueryType.NotEqual; break;
                    //            case TrakHoundConditionOperator.LIKE: queryType = TrakHoundIndexQueryType.Like; break;
                    //            case TrakHoundConditionOperator.GREATER_THAN: queryType = TrakHoundIndexQueryType.GreaterThan; break;
                    //            case TrakHoundConditionOperator.GREATER_THAN_OR_EQUAL: queryType = TrakHoundIndexQueryType.GreaterThanOrEqual; break;
                    //            case TrakHoundConditionOperator.LESS_THAN: queryType = TrakHoundIndexQueryType.LessThan; break;
                    //            case TrakHoundConditionOperator.LESS_THAN_OR_EQUAL: queryType = TrakHoundIndexQueryType.LessThanOrEqual; break;
                    //        }


                    //        long skip = Skip != null ? Skip.Value : 0;
                    //        long take = Take != null ? Take.Value : 0;
                    //        var sortOrder = SortOrder.Ascending; // Needs to be read from query !!

                    //        var subTargetObjectUuids = await client.Objects.QueryIndex(indexQuery.Target, queryType, indexQuery.Query, skip, take, sortOrder);
                    //        fstpw.Stop();
                    //        Console.WriteLine($"QueryStatement.Execute().QueryIndex() : {fstpw.ElapsedMilliseconds}ms");
                    //        fstpw.Restart();

                    //        if (!subTargetObjectUuids.IsNullOrEmpty())
                    //        {
                    //            if (!targetObjectUuids.IsNullOrEmpty())
                    //            {
                    //                switch (indexQuery.ConditionGroup.GroupOperator)
                    //                {
                    //                    case TrakHoundConditionGroupOperator.AND:
                    //                        targetObjectUuids = targetObjectUuids.Intersect(subTargetObjectUuids);
                    //                        break;

                    //                    case TrakHoundConditionGroupOperator.OR:
                    //                        var x = new List<string>();
                    //                        x.AddRange(targetObjectUuids);
                    //                        x.AddRange(subTargetObjectUuids);
                    //                        targetObjectUuids = x;
                    //                        break;
                    //                }
                    //            }
                    //            else
                    //            {
                    //                targetObjectUuids = subTargetObjectUuids;
                    //            }
                    //        }
                    //        else
                    //        {
                    //            break;
                    //        }

                    //        fstpw.Stop();
                    //        Console.WriteLine($"QueryStatement.Execute().QueryIndexJoin() : {fstpw.ElapsedMilliseconds}ms");
                    //        fstpw.Restart();
                    //    }
                    //}


                    var targetObjects = (await client.Objects.ReadByUuid(targetObjectUuids))?.ToArray();
                    if (!targetObjects.IsNullOrEmpty())
                    {
                        var collection = new TrakHoundEntityCollection();

                        var selectObjects = await GetSelectObjects(client, collection, targetObjects);
                        if (!selectObjects.IsNullOrEmpty())
                        {
                            collection.AddTargetIds(selectObjects.Select(o => o.Uuid));
                            collection.Objects.AddObjects(targetObjects);
                            collection.Objects.AddObjects(selectObjects);

                            await GetDefaultContentResults(client, collection, selectObjects);
                        }
                        else
                        {
                            await GetDefaultContentResults(client, collection, targetObjects);
                        }

                        return EvaluateSelect(targetObjects, collection);
                    }




                    //var targetObjects = await GetTarget(client, 0, 10000); // Probably shouldn't have a 'take' here?
                    //if (!targetObjects.IsNullOrEmpty())
                    //{
                    //    if (!ConditionGroups.IsNullOrEmpty())
                    //    {
                    //        var queries = new List<ConditionObjectQuery>();

                    //        foreach (var conditionGroup in ConditionGroups)
                    //        {
                    //            if (!conditionGroup.Conditions.IsNullOrEmpty())
                    //            {
                    //                foreach (var condition in conditionGroup.Conditions)
                    //                {
                    //                    var valueVariable = Scope.GetVariable(condition.Variable);
                    //                    if (valueVariable != null)
                    //                    {
                    //                        var valueTarget = GetFunctionTarget(valueVariable.Value);
                    //                        if (valueTarget != null)
                    //                            if (valueTarget != null) valueTarget = valueTarget.TrimStart('[').TrimEnd(']');

                    //                        foreach (var targetObject in targetObjects)
                    //                        {
                    //                            var query = new ConditionObjectQuery();
                    //                            query.TargetObject = targetObject;
                    //                            query.ConditionGroup = conditionGroup;
                    //                            query.Condition = condition;
                    //                            query.Query = $"uuid:{targetObject.Uuid}/{valueTarget}";
                    //                            queries.Add(query);
                    //                        }
                    //                    }
                    //                }
                    //            }
                    //        }

                    //        var conditionQueryResults = await client.Objects.QueryUuids(TrakHoundObjectExpression.Convert(queries.Select(o => o.Query).Distinct()), 0, 10000);
                    //        if (!conditionQueryResults.IsNullOrEmpty())
                    //        {
                    //            var conditionObjects = await client.Objects.ReadPartialModels(conditionQueryResults.Select(o => o.Uuid).Distinct());
                    //            if (!conditionObjects.IsNullOrEmpty())
                    //            {
                    //                var evaluators = new List<IEvaluator>();

                    //                foreach (var query in queries)
                    //                {
                    //                    var conditionQueryMatchResults = conditionQueryResults.Where(o => o.Query == query.Query);
                    //                    if (!conditionQueryMatchResults.IsNullOrEmpty())
                    //                    {
                    //                        foreach (var conditionQueryMatchResult in conditionQueryMatchResults)
                    //                        {
                    //                            var conditionObject = conditionObjects.FirstOrDefault(o => o.Uuid == conditionQueryMatchResult.Uuid);
                    //                            if (conditionObject != null)
                    //                            {
                    //                                var evaluator = EntityEvaluator.Create(query, conditionObject);
                    //                                if (evaluator != null) evaluators.Add(evaluator);
                    //                            }
                    //                        }
                    //                    }
                    //                }

                    //                if (!evaluators.IsNullOrEmpty())
                    //                {
                    //                    var evaluatorResults = new List<IEvaluatorResult>();

                    //                    var entityEvaluators = await EntityEvaluator.Evaluate(this, client, evaluators.Where(o => typeof(EntityEvaluator).IsAssignableFrom(o.GetType())));

                    //                    foreach (var conditionGroup in ConditionGroups)
                    //                    {
                    //                        var conditionGroupEvaluator = new ConditionGroupEvaluator(conditionGroup);
                    //                        evaluatorResults.AddRange(await conditionGroupEvaluator.Evaluate(this, client, entityEvaluators));
                    //                    }

                    //                    results.AddRange(await GetResults(client, targetObjects, evaluatorResults));
                    //                }
                    //            }
                    //        }
                    //    }
                    //    else
                    //    {
                    //        var collection = new TrakHoundEntityCollection();

                    //        var selectObjects = await GetSelectObjects(client, collection, targetObjects);
                    //        if (!selectObjects.IsNullOrEmpty())
                    //        {
                    //            collection.Add(selectObjects, false);
                    //            await GetDefaultContentResults(client, collection, selectObjects);
                    //        }
                    //        else
                    //        {
                    //            await GetDefaultContentResults(client, collection, targetObjects);
                    //        }

                    //        results.AddRange(await EvaluateSelect(collection));
                    //    }
                    //}
                }
                else
                {
                    var targetObjects = (await GetTarget(client, Skip ?? 0, Take ?? DefaultTake))?.ToArray();
                    if (!targetObjects.IsNullOrEmpty())
                    {
                        var collection = new TrakHoundEntityCollection();

                        var selectObjects = await GetSelectObjects(client, collection, targetObjects);
                        if (!selectObjects.IsNullOrEmpty())
                        {
                            collection.AddTargetIds(selectObjects.Select(o => o.Uuid));
                            collection.Objects.AddObjects(targetObjects);
                            collection.Objects.AddObjects(selectObjects);

                            if (GetSelectType() != QuerySelectType.TargetObject) await GetDefaultContentResults(client, collection, selectObjects);
                        }
                        else
                        {
                            if (GetSelectType() != QuerySelectType.TargetObject) await GetDefaultContentResults(client, collection, targetObjects);
                        }

                        return EvaluateSelect(targetObjects, collection);
                    }
                }
            }

            return null;
        }

        public TrakHoundQueryResult[] Execute(TrakHoundEntityCollection collection)
        {
            if (collection != null)
            {
                if (!ConditionGroups.IsNullOrEmpty())
                {
                    // Needs to be implemented
                }
                else
                {
                    var targetObjects = (GetTarget(collection, Skip ?? 0, Take ?? DefaultTake))?.ToArray();
                    if (!targetObjects.IsNullOrEmpty())
                    {
                        var selectObjects = GetSelectObjects(collection, targetObjects);
                        if (!selectObjects.IsNullOrEmpty())
                        {
                            collection.AddTargetIds(selectObjects.Select(o => o.Uuid));
                            collection.Objects.AddObjects(targetObjects);
                            collection.Objects.AddObjects(selectObjects);

                            //if (GetSelectType() != QuerySelectType.TargetObject) GetDefaultContentResults(collection, selectObjects);
                        }
                        else
                        {
                            //if (GetSelectType() != QuerySelectType.TargetObject) GetDefaultContentResults(collection, targetObjects);
                        }

                        return EvaluateSelect(targetObjects, collection);
                    }
                }
            }

            return null;
        }


        private IEnumerable<ITrakHoundObjectEntity> GetSelectObjects(TrakHoundEntityCollection collection, IEnumerable<ITrakHoundObjectEntity> targetObjects)
        {
            if (collection != null && !targetObjects.IsNullOrEmpty())
            {
                var selectObjects = new List<ITrakHoundObjectEntity>();

                var selectType = GetSelectType();
                switch (selectType)
                {
                    case QuerySelectType.TargetObject:
                        collection.AddTargetIds(targetObjects.Select(o => o.Uuid));
                        selectObjects.AddRange(targetObjects);
                        break;

                    case QuerySelectType.Target:
                        collection.AddTargetIds(targetObjects.Select(o => o.Uuid));
                        selectObjects.AddRange(targetObjects);
                        break;

                    default:

                        if (!Select.IsNullOrEmpty())
                        {
                            var selectExpressions = new List<string>();
                            foreach (var select in Select)
                            {
                                var selectVariable = Scope.GetVariable(select.Expression);
                                if (selectVariable != null)
                                {
                                    if (!string.IsNullOrEmpty(selectVariable.Value))
                                    {
                                        //foreach (var targetObject in targetObjects)
                                        //{
                                        //    selectExpressions.Add(TrakHoundPath.Combine(targetObject.NamePath, selectVariable.Value));
                                        //}

                                        selectExpressions.Add(selectVariable.Value);
                                    }
                                    else
                                    {
                                        // If empty then add as Target
                                        selectObjects.AddRange(targetObjects);
                                    }
                                }
                            }

                            var foundObjects = TrakHoundExpression.Match(TrakHoundExpression.Convert(selectExpressions), collection);
                            if (!foundObjects.IsNullOrEmpty())
                            {
                                selectObjects.AddRange(foundObjects);
                            }
                        }

                        break;
                }

                return selectObjects.Distinct();
            }

            return null;
        }

        private async Task<IEnumerable<ITrakHoundObjectEntity>> GetSelectObjects(ITrakHoundSystemEntitiesClient client, TrakHoundEntityCollection collection, IEnumerable<ITrakHoundObjectEntity> targetObjects)
        {
            if (client != null && !targetObjects.IsNullOrEmpty())
            {
                var selectObjects = new List<ITrakHoundObjectEntity>();

                var selectType = GetSelectType();
                switch (selectType)
                {
                    case QuerySelectType.TargetObject:
                        collection.AddTargetIds(targetObjects.Select(o => o.Uuid));
                        selectObjects.AddRange(targetObjects);
                        break;

                    case QuerySelectType.Target:
                        collection.AddTargetIds(targetObjects.Select(o => o.Uuid));
                        selectObjects.AddRange(targetObjects);
                        break;

                    default:

                        if (!Select.IsNullOrEmpty())
                        {
                            var selectExpressions = new List<string>();
                            foreach (var select in Select)
                            {
                                var selectVariable = Scope.GetVariable(select.Expression);
                                if (selectVariable != null)
                                {
                                    if (!string.IsNullOrEmpty(selectVariable.Value))
                                    {
                                        //foreach (var targetObject in targetObjects)
                                        //{
                                        //    selectExpressions.Add(TrakHoundPath.Combine(targetObject.NamePath, selectVariable.Value));
                                        //}

                                        selectExpressions.Add(selectVariable.Value);
                                    }
                                    else
                                    {
                                        // If empty then add as Target
                                        selectObjects.AddRange(targetObjects);
                                    }
                                }
                            }

                            var expressionResults = await TrakHoundExpression.Evaluate(client, TrakHoundExpression.Convert(selectExpressions), targetObjects.Select(o => o.Uuid), 0, int.MaxValue);
                            if (!expressionResults.IsNullOrEmpty())
                            {
                                var objectUuids = expressionResults.Select(o => o.Uuid);
                                var foundObjects = await client.Objects.ReadByUuid(objectUuids);
                                if (!foundObjects.IsNullOrEmpty()) selectObjects.AddRange(foundObjects);
                            }
                        }

                        break;
                }

                return selectObjects.Distinct();
            }

            return null;
        }


        private TrakHoundQueryResult[] EvaluateSelect(ITrakHoundObjectEntity[] targetObjects, TrakHoundEntityCollection collection)
        {
            var results = new List<TrakHoundQueryResult>();

            if (!Select.IsNullOrEmpty() && collection != null)
            {
                var selectType = GetSelectType();
                switch (selectType)
                {
                    case QuerySelectType.Target:

                        var contentTypesDictionary = new TrakHoundObjectContentTypeDictionary();
                        contentTypesDictionary.Add(targetObjects);

                        foreach (var contentType in contentTypesDictionary.ContentTypes)
                        {
                            var contentTargetObjects = contentTypesDictionary.Get(contentType);
                            if (!contentTargetObjects.IsNullOrEmpty())
                            {
                                results.Add(CreateTargetResult(contentType.ConvertEnum<TrakHoundObjectContentType>(), contentTargetObjects.ToArray(), collection));
                            }
                        }
                        break;

                    case QuerySelectType.TargetObject:

                        results.Add(TrakHoundQueryResults.CreateObjectsResult(targetObjects, true));
                        break;

                    case QuerySelectType.Content:

                        results.Add(CreateContentResult(targetObjects, collection));
                        break;

                    default:
                        //var expressions = GetSelectExpressions();
                        //objectEntities = collection.Objects.QueryObjects(TrakHoundObjectExpression.Convert(expressions));
                        //return await ReadContent(objectEntities.ToArray(), collection);
                        break;
                }
            }

            return results.ToArray();
        }

        public static TrakHoundQueryResult CreateTargetResult(TrakHoundObjectContentType contentType, ITrakHoundObjectEntity[] targetObjects, TrakHoundEntityCollection collection)
        {
            switch (contentType)
            {
                case TrakHoundObjectContentType.Assignment: return TrakHoundQueryResults.CreateObjectsAssignmentResult(targetObjects, collection);
                case TrakHoundObjectContentType.Boolean: return TrakHoundQueryResults.CreateObjectsBooleanResult(targetObjects, collection);
                case TrakHoundObjectContentType.Directory: return TrakHoundQueryResults.CreateObjectsResult(targetObjects);
                case TrakHoundObjectContentType.Duration: return TrakHoundQueryResults.CreateObjectsDurationResult(targetObjects, collection);
                case TrakHoundObjectContentType.Event: return TrakHoundQueryResults.CreateObjectsEventResult(targetObjects, collection);
                case TrakHoundObjectContentType.Group: return TrakHoundQueryResults.CreateObjectsGroupResult(targetObjects, collection);
                case TrakHoundObjectContentType.Hash: return TrakHoundQueryResults.CreateObjectsHashResult(targetObjects, collection);
                case TrakHoundObjectContentType.Log: return TrakHoundQueryResults.CreateObjectsLogResult(targetObjects, collection);
                case TrakHoundObjectContentType.Number: return TrakHoundQueryResults.CreateObjectsNumberResult(targetObjects, collection);
                case TrakHoundObjectContentType.Message: return TrakHoundQueryResults.CreateObjectsMessageResult(targetObjects, collection);
                case TrakHoundObjectContentType.Observation: return TrakHoundQueryResults.CreateObjectsObservationResult(targetObjects, collection);
                case TrakHoundObjectContentType.Set: return TrakHoundQueryResults.CreateObjectsSetResult(targetObjects, collection);
                case TrakHoundObjectContentType.State: return TrakHoundQueryResults.CreateObjectsStateResult(targetObjects, collection);
                case TrakHoundObjectContentType.Statistic: return TrakHoundQueryResults.CreateObjectsStatisticResult(targetObjects, collection);
                case TrakHoundObjectContentType.String: return TrakHoundQueryResults.CreateObjectsStringResult(targetObjects, collection);
                case TrakHoundObjectContentType.TimeRange: return TrakHoundQueryResults.CreateObjectsTimeRangeResult(targetObjects, collection);
                case TrakHoundObjectContentType.Timestamp: return TrakHoundQueryResults.CreateObjectsTimestampResult(targetObjects, collection);
                case TrakHoundObjectContentType.Vocabulary: return TrakHoundQueryResults.CreateObjectsVocabularyResult(targetObjects, collection);
            }

            return new TrakHoundQueryResult();
        }

        private TrakHoundQueryResult CreateContentResult(ITrakHoundObjectEntity[] targetObjects, TrakHoundEntityCollection collection)
        {
            var result = new TrakHoundQueryResult();

            if (!targetObjects.IsNullOrEmpty() && collection != null)
            {
                var columnNames = new HashSet<string>();

                var columns = new List<TrakHoundQueryColumnDefinition>();
                foreach (var targetObject in targetObjects)
                {
                    var contentObjects = collection.Objects.QueryObjectsByParentUuid(targetObject.Uuid);
                    if (!contentObjects.IsNullOrEmpty())
                    {
                        foreach (var contentObject in contentObjects)
                        {
                            if (contentObject.ContentType != TrakHoundObjectContentTypes.Directory)
                            {
                                if (!columnNames.Contains(contentObject.Name))
                                {
                                    columns.Add(new TrakHoundQueryColumnDefinition
                                    {
                                        Name = contentObject.Name,
                                        DataType = TrakHoundQueryResult.GetValueDataType(contentObject.ContentType)
                                    });

                                    columnNames.Add(contentObject.Name);
                                }
                            }
                        }
                    }
                }
                result.Columns = columns.ToArray();


                var rows = new object[targetObjects.Length, result.Columns.Length];

                for (var i = 0; i < targetObjects.Length; i++)
                {
                    for (var j = 0; j < result.ColumnCount; j++)
                    {
                        object value = null;
                        var column = result.Columns[j];

                        var contentObjectPath = TrakHoundPath.Combine(targetObjects[i].Path, column.Name);
                        var contentObjectUuid = TrakHoundPath.GetUuid(targetObjects[i].Namespace, contentObjectPath);

                        var contentObject = collection.Objects.GetObject(contentObjectUuid);
                        if (contentObject != null)
                        {
                            switch (contentObject.ContentType)
                            {
                                //case TrakHoundObjectContentTypes.Directory:
                                //    value = contentObject.Uuid;
                                //    break;

                                case TrakHoundObjectContentTypes.Assignment:
                                    value = collection.Objects.QueryAssignmentsByAssigneeUuid(contentObject.Uuid)?.FirstOrDefault()?.MemberUuid;
                                    break;

                                case TrakHoundObjectContentTypes.Boolean:
                                    value = collection.Objects.QueryBooleanByObjectUuid(contentObject.Uuid)?.Value;
                                    break;

                                case TrakHoundObjectContentTypes.Duration:
                                    value = collection.Objects.QueryDurationByObjectUuid(contentObject.Uuid)?.Value;
                                    break;

                                case TrakHoundObjectContentTypes.Number:
                                    value = collection.Objects.QueryNumberByObjectUuid(contentObject.Uuid)?.Value;
                                    break;

                                case TrakHoundObjectContentTypes.Observation:
                                    value = collection.Objects.QueryObservationsByObjectUuid(contentObject.Uuid)?.FirstOrDefault()?.Value;
                                    break;

                                case TrakHoundObjectContentTypes.Reference:
                                    value = collection.Objects.QueryReferenceByObjectUuid(contentObject.Uuid)?.TargetUuid;
                                    break;

                                case TrakHoundObjectContentTypes.State:
                                    value = collection.Objects.QueryStatesByObjectUuid(contentObject.Uuid)?.FirstOrDefault()?.DefinitionUuid;
                                    break;

                                case TrakHoundObjectContentTypes.Statistic:
                                    value = collection.Objects.QueryStatisticsByObjectUuid(contentObject.Uuid)?.FirstOrDefault()?.Value;
                                    break;

                                case TrakHoundObjectContentTypes.String:
                                    value = collection.Objects.QueryStringByObjectUuid(contentObject.Uuid)?.Value;
                                    break;

                                case TrakHoundObjectContentTypes.Timestamp:
                                    value = collection.Objects.QueryTimestampByObjectUuid(contentObject.Uuid)?.Value;
                                    break;
                            }
                        }

                        rows[i, j] = value;
                    }
                }

                result.Rows = rows;
            }

            return result;
        }

        private IEnumerable<ITrakHoundObjectEntity> GetTarget(TrakHoundEntityCollection collection, int skip, int take)
        {
            if (collection != null && Target != null)
            {
                IEnumerable<ITrakHoundObjectEntity> targetObjects = null;

                var target = Scope.GetVariable(Target);
                if (target != null)
                {
                    switch (target.Type)
                    {
                        case QueryVariableType.Expression:
                            var targetExpression = ReplaceVariables(target.Value);
                            targetObjects = collection.Objects.QueryObjects(targetExpression);
                            break;

                        case QueryVariableType.Query:
                            //var statement = Create(Scope, target.Value);
                            //var results = await statement.Execute(client);
                            //if (!results.IsNullOrEmpty())
                            //{
                            //    var entities = results.OfType<TrakHoundEntityQueryResult>();
                            //    targetObjects = entities?.Where(o => o.Entity != null)?.Select(o => o.Entity).OfType<ITrakHoundObjectEntity>();
                            //}
                            break;
                    }
                }

                return targetObjects;
            }

            return null;
        }

        private async Task<IEnumerable<ITrakHoundObjectEntity>> GetTarget(ITrakHoundSystemEntitiesClient client, int skip, int take)
        {
            if (client != null && Target != null)
            {
                IEnumerable<ITrakHoundObjectEntity> targetObjects = null;

                var target = Scope.GetVariable(Target);
                if (target != null)
                {
                    switch (target.Type)
                    {
                        case QueryVariableType.Expression:
                            var targetExpression = ReplaceVariables(target.Value);
                            targetObjects = await client.Objects.Query(targetExpression, skip, take);
                            break;

                        case QueryVariableType.Query:
                            //var statement = Create(Scope, target.Value);
                            //var results = await statement.Execute(client);
                            //if (!results.IsNullOrEmpty())
                            //{
                            //    var entities = results.OfType<TrakHoundEntityQueryResult>();
                            //    targetObjects = entities?.Where(o => o.Entity != null)?.Select(o => o.Entity).OfType<ITrakHoundObjectEntity>();
                            //}
                            break;
                    }
                }

                return targetObjects;
            }

            return null;
        }

        private async Task GetDefaultContentResults(ITrakHoundSystemEntitiesClient client, TrakHoundEntityCollection collection, IEnumerable<ITrakHoundObjectEntity> objects)
        {
            if (client != null && collection != null && !objects.IsNullOrEmpty())
            {
                var start = Start ?? 0;
                var stop = Stop ?? long.MaxValue;
                var span = Span ?? 36000000000; // Default to Hour?
                var skip = Skip ?? 0;
                var take = Take ?? DefaultTake;
                var sortOrder = Order != null ? Order.Value : SortOrder.Ascending;
                var now = UnixDateTime.Now;

                // Get TimeRange
                IEnumerable<string> timeRangeUuids = null;
                if (Range != null)
                {
                    var relativeTimeRange = TimeRange.GetRelative(Range, now);
                    if (relativeTimeRange != null)
                    {
                        if (Start == null && Stop == null)
                        {
                            start = relativeTimeRange.Value.From.ToUnixTime();
                            stop = relativeTimeRange.Value.To.ToUnixTime();
                        }
                    }
                    else
                    {
                        //if (Start != null || Stop != null)
                        //{
                        //    timeRangeUuids = (await client.Objects.TimeRange.QueryByObject(Range, start, stop, span))?.Select(o => o.Uuid);
                        //}
                        //else
                        //{
                        //    timeRangeUuids = (await client.Objects.TimeRange.QueryByObject(Range, now, now, span))?.Select(o => o.Uuid);
                        //}
                    }
                }

                var contentTypeDictionary = new TrakHoundObjectContentTypeDictionary();
                contentTypeDictionary.Add(objects);

                var tasks = new List<Task<IEnumerable<ITrakHoundEntity>>>();

                var assignmentObjects = contentTypeDictionary.Get(TrakHoundObjectContentType.Assignment);
                if (!assignmentObjects.IsNullOrEmpty())
                {
                    if (Start != null || Stop != null || Range != null) tasks.Add(GetEntities(client.Objects.Assignment.QueryByAssigneeUuid(assignmentObjects.Select(o => o.Uuid), start, stop, skip, take, sortOrder)));
                    else tasks.Add(GetEntities(client.Objects.Assignment.CurrentByAssigneeUuid(assignmentObjects.Select(o => o.Uuid))));
                }

                var booleanObjects = contentTypeDictionary.Get(TrakHoundObjectContentType.Boolean);
                if (!booleanObjects.IsNullOrEmpty()) tasks.Add(GetEntities(client.Objects.Boolean.QueryByObjectUuid(booleanObjects.Select(o => o.Uuid))));

                var durationObjects = contentTypeDictionary.Get(TrakHoundObjectContentType.Duration);
                if (!durationObjects.IsNullOrEmpty()) tasks.Add(GetEntities(client.Objects.Duration.QueryByObjectUuid(durationObjects.Select(o => o.Uuid))));

                var eventObjects = contentTypeDictionary.Get(TrakHoundObjectContentType.Event);
                if (!eventObjects.IsNullOrEmpty())
                {
                    if (Start != null || Stop != null || Range != null) tasks.Add(GetEntities(client.Objects.Event.QueryByObjectUuid(eventObjects.Select(o => o.Uuid), start, stop, skip, take, sortOrder)));
                    else tasks.Add(GetEntities(client.Objects.Event.LatestByObjectUuid(eventObjects.Select(o => o.Uuid))));
                }

                var groupObjects = contentTypeDictionary.Get(TrakHoundObjectContentType.Group);
                if (!groupObjects.IsNullOrEmpty()) tasks.Add(GetEntities(client.Objects.Group.QueryByGroupUuid(groupObjects.Select(o => o.Uuid))));

                var hashObjects = contentTypeDictionary.Get(TrakHoundObjectContentType.Hash);
                if (!hashObjects.IsNullOrEmpty()) tasks.Add(GetEntities(client.Objects.Hash.QueryByObjectUuid(hashObjects.Select(o => o.Uuid))));

                var logObjects = objects.Where(o => o.ContentType.ConvertEnum<TrakHoundObjectContentType>() == TrakHoundObjectContentType.Log);
                if (!logObjects.IsNullOrEmpty())
                {
                    if (Start != null || Stop != null || Range != null) collection.Add(await client.Objects.Log.QueryByObjectUuid(logObjects.Select(o => o.Uuid), TrakHoundLogLevel.Trace, start, stop, skip, take));
                    else collection.Add(await client.Objects.Log.QueryByObjectUuid(logObjects.Select(o => o.Uuid), TrakHoundLogLevel.Trace, 0, long.MaxValue, 0, 1, SortOrder.Descending));
                }

                var numberObjects = contentTypeDictionary.Get(TrakHoundObjectContentType.Number);
                if (!numberObjects.IsNullOrEmpty()) tasks.Add(GetEntities(client.Objects.Number.QueryByObjectUuid(numberObjects.Select(o => o.Uuid))));

                var observationObjects = contentTypeDictionary.Get(TrakHoundObjectContentType.Observation);
                if (!observationObjects.IsNullOrEmpty())
                {
                    if (Start != null || Stop != null || Range != null) tasks.Add(GetEntities(client.Objects.Observation.QueryByObjectUuid(observationObjects.Select(o => o.Uuid), start, stop, skip, take, sortOrder)));
                    else tasks.Add(GetEntities(client.Objects.Observation.LatestByObjectUuid(observationObjects.Select(o => o.Uuid))));
                }

                var queueObjects = contentTypeDictionary.Get(TrakHoundObjectContentType.Queue);
                if (!queueObjects.IsNullOrEmpty()) tasks.Add(GetEntities(client.Objects.Queue.QueryByQueueUuid(queueObjects.Select(o => o.Uuid))));

                var referenceObjects = contentTypeDictionary.Get(TrakHoundObjectContentType.Reference);
                if (!referenceObjects.IsNullOrEmpty())
                {
                    var references = await client.Objects.Reference.QueryByObjectUuid(referenceObjects.Select(o => o.Uuid));
                    collection.Add(references, false);
                    //collection.Add(references?.Select(o => o.Target), false);
                }

                var setObjects = contentTypeDictionary.Get(TrakHoundObjectContentType.Set);
                if (!setObjects.IsNullOrEmpty()) tasks.Add(GetEntities(client.Objects.Set.QueryByObjectUuid(setObjects.Select(o => o.Uuid))));

                var stateObjects = contentTypeDictionary.Get(TrakHoundObjectContentType.State);
                if (!stateObjects.IsNullOrEmpty())
                {
                    if (Start != null || Stop != null || Range != null) tasks.Add(GetEntities(client.Objects.State.QueryByObjectUuid(stateObjects.Select(o => o.Uuid), start, stop, skip, take, sortOrder)));
                    else tasks.Add(GetEntities(client.Objects.State.LatestByObjectUuid(stateObjects.Select(o => o.Uuid))));
                }

                var stringObjects = contentTypeDictionary.Get(TrakHoundObjectContentType.String);
                if (!stringObjects.IsNullOrEmpty()) tasks.Add(GetEntities(client.Objects.String.QueryByObjectUuid(stringObjects.Select(o => o.Uuid))));

                var statisticObjects = contentTypeDictionary.Get(TrakHoundObjectContentType.Statistic);
                if (!statisticObjects.IsNullOrEmpty())
                {
                    if (Start != null || Stop != null || Range != null) tasks.Add(GetEntities(client.Objects.Statistic.QueryByObjectUuid(statisticObjects.Select(o => o.Uuid), start, stop, span, skip, take, sortOrder)));
                    //else tasks.Add(GetEntities(client.Objects.Statistic.LatestByObjectUuid(statisticObjects.Select(o => o.Uuid))));
                }

                //var statisticObjects = objects.Where(o => o.ContentType.ConvertEnum<TrakHoundObjectContentType>() == TrakHoundObjectContentType.Statistic);
                //if (!statisticObjects.IsNullOrEmpty())
                //{
                //    if (Start != null || Stop != null || Range != null) collection.Add(await client.Objects.Statistic.QueryModelsByObjectUuid(statisticObjects.Select(o => o.Uuid), start, stop, skip, take));
                //    else collection.Add(await client.Objects.Statistic.QueryModelsByObjectUuid(statisticObjects.Select(o => o.Uuid), timeRangeUuids));
                //}

                //var timeGroupObjects = objects.Where(o => o.ContentType.ConvertEnum<TrakHoundObjectContentType>() == TrakHoundObjectContentType.TimeGroup);
                //if (!timeGroupObjects.IsNullOrEmpty()) collection.Add(await client.Objects.TimeGroup.QueryModelsByGroupUuid(timeGroupObjects.Select(o => o.Uuid), timeRangeUuids));

                //var timeHashObjects = objects.Where(o => o.ContentType.ConvertEnum<TrakHoundObjectContentType>() == TrakHoundObjectContentType.TimeHash);
                //if (!timeHashObjects.IsNullOrEmpty()) collection.Add(await client.Objects.TimeHash.QueryModelsByObjectUuid(timeHashObjects.Select(o => o.Uuid), timeRangeUuids));

                //var timeSequenceObjects = objects.Where(o => o.ContentType.ConvertEnum<TrakHoundObjectContentType>() == TrakHoundObjectContentType.TimeSequence);
                //if (!timeSequenceObjects.IsNullOrEmpty())
                //{
                //    if (Start != null || Stop != null || Range != null) collection.Add(await client.Objects.TimeSequence.QueryModelsByObjectUuid(timeSequenceObjects.Select(o => o.Uuid), start, stop, skip, take));
                //    else collection.Add(await client.Objects.TimeSequence.QueryModelsByObjectUuid(timeSequenceObjects.Select(o => o.Uuid), timeRangeUuids));
                //}

                //var timeSetObjects = objects.Where(o => o.ContentType.ConvertEnum<TrakHoundObjectContentType>() == TrakHoundObjectContentType.TimeSet);
                //if (!timeSetObjects.IsNullOrEmpty()) collection.Add(await client.Objects.TimeSet.QueryModelsByObjectUuid(timeSetObjects.Select(o => o.Uuid), timeRangeUuids));

                var timestampObjects = contentTypeDictionary.Get(TrakHoundObjectContentType.Timestamp);
                if (!timestampObjects.IsNullOrEmpty()) tasks.Add(GetEntities(client.Objects.Timestamp.QueryByObjectUuid(timestampObjects.Select(o => o.Uuid))));

                //var timeWikiObjects = objects.Where(o => o.ContentType.ConvertEnum<TrakHoundObjectContentType>() == TrakHoundObjectContentType.TimeWiki);
                //if (!timeWikiObjects.IsNullOrEmpty()) collection.Add(await client.Objects.TimeWiki.QueryModelsByObjectUuid(timeWikiObjects.Select(o => o.Uuid), timeRangeUuids));

                //var wikiObjects = contentTypeDictionary.Get(TrakHoundObjectContentType.Wiki);
                //if (!wikiObjects.IsNullOrEmpty()) tasks.Add(GetEntities(client.Objects.Wiki.QueryByObjectUuid(wikiObjects.Select(o => o.Uuid))));


                var taskResults = await Task.WhenAll(tasks);
                if (!taskResults.IsNullOrEmpty())
                {
                    foreach (var taskResult in taskResults)
                    {
                        collection.Add(taskResult);
                    }


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
                        collection.Add(await client.Objects.ReadByUuid(additionalObjectUuids));
                    }
                }
            }
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

        #endregion

        #region "Create"

        public static QueryStatement Create(QueryScope scope, string query)
        {
            var statement = new QueryStatement(scope, query);

            if (!string.IsNullOrEmpty(query))
            {
                var regexPattern = "select\\s+\\(([^)]*)\\)+";
                var matches = Regex.Matches(query, regexPattern);
                if (!matches.IsNullOrEmpty())
                {
                    foreach (Match match in matches)
                    {
                        var variableName = CreateVariableName();
                        var variableValue = match.Groups[1].Value;
                        var variableIndex = match.Groups[1].Index;
                        statement.Command = ReplaceCommandVariable(statement.Command, variableIndex, variableName, variableValue);
                        scope.Variables.Add(new QueryVariable(variableName, variableValue, QueryVariableType.Query));
                    }
                }
                else
                {
                    statement.Command = query;
                }

                if (query.StartsWith("declare"))
                {
                    ParseDeclare(statement);
                }
                else
                {
                    ParseQuery(statement);
                }
            }

            return statement;
        }


        private static void ParseDeclare(QueryStatement statement)
        {
            var regex = @"declare\s([.\s\S]+)\s\=\s\'([.\s\S]+)\'";
            //var regex = @"declare\s([.\s\S]+)\s\=\s([.\s\S]+)";
            var declareMatches = Regex.Matches(statement.Command, regex);
            if (declareMatches != null)
            {
                foreach (Match declareMatch in declareMatches)
                {
                    var variableName = declareMatch.Groups[1].Value;
                    var variableValue = declareMatch.Groups[2].Value;

                    statement.Scope.Variables.Add(new QueryVariable(variableName, variableValue, QueryVariableType.Scalar));
                }
            }
        }


        private static void ParseQuery(QueryStatement statement)
        {
            // Process Select
            ParseSelect(statement);

            // Process Target
            ParseTarget(statement);

            // Process Conditions
            var where = ParseWhere(statement.Command);
            ParseConditionGroups(statement, where);

            statement.Start = ParseStart(statement.Command);
            statement.Stop = ParseStop(statement.Command);
            statement.Range = ParseRange(statement.Command);
            statement.Span = ParseSpan(statement.Command);

            statement.Skip = ParseSkip(statement.Command);
            statement.Take = ParseTake(statement.Command);
            statement.Order = ParseOrder(statement.Command);

            // Process GroupBy
            ParseGroupBy(statement);
        }

        private static void ParseSelect(QueryStatement statement)
        {
            var regex = @"select\s+([.\s\S]+)\s+from";
            var selectStatementMatch = Regex.Match(statement.Command, regex);
            if (selectStatementMatch.Success)
            {
                var originalSelectStatement = selectStatementMatch.Groups[1].Value;
                var selectStatement = selectStatementMatch.Groups[1].Value;
                var selectStatementIndex = selectStatementMatch.Groups[1].Index;

                var selectMatches = Regex.Matches(selectStatement, @"\[?(?:((?:(?:\.{2}\/?)+)?[^\[\]\,\.\s]*))\]?(?:\.{1}([^\[\]\,\.\s]+))?(?:\sas\s([^\[\]\,\.\s]+))?|(\*{1})");
                //var selectMatches = Regex.Matches(selectStatement, @"\[?(?:((?:\.{2})?[^\[\]\,\.\s]+))\]?(?:\.{1}([^\[\]\,\.\s]+))?(?:\sas\s([^\[\]\,\.\s]+))?|(\*{1})");
                //var selectMatches = Regex.Matches(selectStatement, @"\[?(?:([^\[\]\,\.\s]*))\]?(?:\.{1}([^\[\]\,\s]+))?(?:\sas\s([^\[\]\,\s]+))?|(\*{1})");
                if (selectMatches != null)
                {
                    var index = 0;

                    foreach (Match selectMatch in selectMatches)
                    {
                        if (selectMatch.Success)
                        {
                            var success = !string.IsNullOrEmpty(selectMatch.Groups[1].Value) || !string.IsNullOrEmpty(selectMatch.Groups[2].Value);
                            if (success)
                            {
                                // Get Expression
                                string expression;
                                if (selectMatch.Groups[1].Success) expression = selectMatch.Groups[1].Value;
                                else expression = selectMatch.Groups[3].Value;
                                expression = expression?.Trim();

                                // Create Expression Variable
                                var expressionVariableName = CreateVariableName();
                                var expressionVariableValue = expression;
                                statement.Scope.Variables.Add(new QueryVariable(expressionVariableName, expressionVariableValue, QueryVariableType.Expression));


                                // Set Select Type
                                QuerySelectType queryType = QuerySelectType.Content;
                                switch (expression)
                                {
                                    case ">>": queryType = QuerySelectType.TargetObject; break;
                                    case ">": queryType = QuerySelectType.Target; break;
                                    case "*": queryType = QuerySelectType.Content; break;
                                }


                                // Get Property (if specified)
                                string property = null;
                                if (selectMatch.Groups[2].Success) property = selectMatch.Groups[2].Value;

                                // Create Property Variable
                                string propertyVariableName = null;
                                if (property != null)
                                {
                                    queryType = QuerySelectType.Property;

                                    propertyVariableName = CreateVariableName();
                                    var propertyVariableValue = property;
                                    statement.Scope.Variables.Add(new QueryVariable(propertyVariableName, propertyVariableValue, QueryVariableType.Property));
                                }


                                // Get Alias (if specified)
                                string alias = null;
                                if (selectMatch.Groups[3].Success) alias = selectMatch.Groups[3].Value;

                                // Create Property Variable
                                string aliasVariableName = null;
                                if (alias != null)
                                {
                                    aliasVariableName = CreateVariableName();
                                    var aliasVariableValue = alias;
                                    statement.Scope.Variables.Add(new QueryVariable(aliasVariableName, aliasVariableValue, QueryVariableType.Property));
                                }


                                // Add to Select
                                statement.Select.Add(new QuerySelect(index++, queryType, expressionVariableName, propertyVariableName, aliasVariableName));
                            }
                        }
                    }
                }
            }
        }

        private static void ParseTarget(QueryStatement statement)
        {
            var regex = @"from\s+\[?([^\]\s]+)\]?";
            var match = Regex.Match(statement.Command, regex);
            if (match.Success)
            {
                var variableName = CreateVariableName();
                var variableValue = match.Groups[1].Value?.Trim();
                var variableIndex = match.Groups[1].Index;

                statement.Target = variableName;
                statement.Command = ReplaceCommandVariable(statement.Command, variableIndex, variableName, variableValue);
                statement.Scope.Variables.Add(new QueryVariable(variableName, variableValue, QueryVariableType.Expression));
            }
        }


        private static string ParseWhere(string query)
        {
            var regex = @"(?:where)\s+(.+)\s+(?:start)|(?:where)\s+(.+)\s+(?:stop)|(?:where)\s+(.*)";
            var match = Regex.Match(query, regex);
            return GetFirstGroupMatch(match)?.Value;
        }

        private static void ParseConditionGroups(QueryStatement statement, string where, int order = 0)
        {
            if (!string.IsNullOrEmpty(where))
            {
                var groupRegex = @"\B(?![^\(\)]+)(\(([^()]+)\))";
                var groupMatches = Regex.Matches(where, groupRegex);
                if (!groupMatches.IsNullOrEmpty())
                {
                    var originalConditionStatement = where;
                    var indexDiff = 0;
                    var conditionStatement = where;

                    foreach (Match groupMatch in groupMatches)
                    {
                        var groupFullStatement = groupMatch.Groups[1].Value;
                        var groupFullStatementIndex = groupMatch.Groups[1].Index + indexDiff;

                        var groupStatement = groupMatch.Groups[2].Value;
                        var groupStatementIndex = groupMatch.Groups[2].Index;

                        var groupId = CreateGroupId();

                        var groupVariableName = CreateGroupVariableName(groupId);
                        var groupVariableValue = groupId;

                        var newConditionStatement = ReplaceCommandVariable(conditionStatement, groupFullStatementIndex, groupVariableName, groupFullStatement);
                        indexDiff = newConditionStatement.Length - originalConditionStatement.Length;
                        conditionStatement = newConditionStatement;

                        ParseConditionGroup(statement, groupStatement, groupId, order);
                    }

                    ParseConditionGroups(statement, conditionStatement, order + 1);
                }
                else
                {
                    ParseConditionGroup(statement, where, order: order);
                }
            }
        }

        private static QueryConditionGroup ParseConditionGroup(QueryStatement statement, string groupStatement, string groupId = null, int order = 0)
        {
            QueryConditionGroup group = null;

            if (!string.IsNullOrEmpty(groupStatement))
            {
                var groupBy = new List<string>();

                var groupByMatch = Regex.Match(groupStatement, @"group\sby\s+(.*)\s?$|group\sby\s+(.*)\s+start|group\sby\s+(.*)\s+stop");
                if (groupByMatch.Success)
                {
                    var groupByGroup = GetFirstGroupMatch(groupByMatch);
                    if (groupByGroup.Success)
                    {
                        var groupByGroupsMatches = Regex.Matches(groupByGroup.Value, @"\[(?:([^\]]*))\]");
                        foreach (Match groupByGroupsMatch in groupByGroupsMatches)
                        {
                           var groupByGroupsMatchValue = GetFirstGroupMatch(groupByGroupsMatch);
                           if (groupByGroupsMatchValue.Success) groupBy.Add(groupByGroupsMatchValue.Value);
                        }
                    }
                }

                var orStatements = groupStatement.Split(" or ", StringSplitOptions.RemoveEmptyEntries);
                if (orStatements.Length > 1)
                {
                    group = new QueryConditionGroup(groupId ?? CreateGroupId(), TrakHoundConditionGroupOperator.OR, order);
                    group.GroupBy = groupBy;

                    foreach (var conditionStatement in orStatements)
                    {
                        var subGroup = ParseConditionGroup(statement, conditionStatement, order: order);
                        if (subGroup != null)
                        {
                            var subGroupVariableName = CreateGroupVariableName(subGroup.GroupId);
                            var subGroupVariableValue = subGroup.GroupId;

                            group.Conditions.Add(new QueryCondition(subGroupVariableName, subGroupVariableValue));
                        }
                        else
                        {
                            ProcessCondition(statement, group, conditionStatement);
                        }
                    }

                    statement.ConditionGroups.Add(group);
                    statement.Scope.Variables.Add(new QueryVariable(CreateGroupVariableName(group.GroupId), group.GroupId, QueryVariableType.ConditionGroup));
                }
                else
                {
                    var andStatements = groupStatement.Split(" and ", StringSplitOptions.RemoveEmptyEntries);
                    if (andStatements.Length > 1)
                    {
                        group = new QueryConditionGroup(groupId ?? CreateGroupId(), TrakHoundConditionGroupOperator.AND, order);
                        group.GroupBy = groupBy;

                        foreach (var conditionStatement in andStatements)
                        {
                            ProcessCondition(statement, group, conditionStatement);
                        }

                        statement.ConditionGroups.Add(group);
                        statement.Scope.Variables.Add(new QueryVariable(CreateGroupVariableName(group.GroupId), group.GroupId, QueryVariableType.ConditionGroup));
                    }
                    else
                    {
                        group = new QueryConditionGroup(groupId ?? CreateGroupId(), TrakHoundConditionGroupOperator.OR, order);
                        group.GroupBy = groupBy;
                        ProcessCondition(statement, group, groupStatement);

                        statement.ConditionGroups.Add(group);
                        statement.Scope.Variables.Add(new QueryVariable(CreateGroupVariableName(group.GroupId), group.GroupId, QueryVariableType.ConditionGroup));
                        group.Conditions.Add(new QueryCondition(CreateGroupVariableName(group.GroupId), group.GroupId));
                    }
                }
            }

            return group;
        }

        private static void ProcessCondition(QueryStatement statement, QueryConditionGroup conditionGroup, string input)
        {
            var inputStatement = input.Trim();

            var conditionRegex = @"((?:[^()]+)\(\[?([^\]\.]+)\]?(?:\.\[?([^\]]*)\]?)?\))\s(.+)\s(.*)";
            var conditionMatch = Regex.Match(inputStatement, conditionRegex);
            if (conditionMatch.Success)
            {
                var originalConditionStatement = conditionMatch.Groups[0].Value;
                var conditionStatement = conditionMatch.Groups[0].Value;
                var conditionStatementIndex = conditionMatch.Groups[0].Index;
                var conditionStatementLength = conditionMatch.Groups[0].Length;

                var variableName = CreateVariableName();
                var variableValue = conditionMatch.Groups[1].Value;
                var variableIndex = conditionMatch.Groups[1].Index + conditionStatementIndex;
                conditionStatement = ReplaceCommandVariable(conditionStatement, variableIndex, variableName, variableValue);
                statement.Scope.Variables.Add(new QueryVariable(variableName, variableValue, QueryVariableType.Expression));

                var propertyName = CreateVariableName();
                var propertyValue = conditionMatch.Groups[3].Value;
                var propertyIndex = conditionMatch.Groups[3].Index + conditionStatementIndex;
                conditionStatement = ReplaceCommandVariable(conditionStatement, propertyIndex, propertyName, propertyValue);
                statement.Scope.Variables.Add(new QueryVariable(propertyName, propertyValue, QueryVariableType.Property));

                var valueName = GetVariableName(conditionMatch.Groups[5].Value);
                var valueVariable = statement.Scope.GetVariable(valueName);
                if (valueVariable == null)
                {
                    valueName = CreateVariableName();
                    var valueValue = conditionMatch.Groups[5].Value.Trim('\'');
                    var valueIndex = conditionMatch.Groups[5].Index + conditionStatementIndex + (conditionStatement.Length - originalConditionStatement.Length);
                    conditionStatement = ReplaceCommandVariable(conditionStatement, valueIndex, valueName, conditionMatch.Groups[5].Value);
                    statement.Scope.Variables.Add(new QueryVariable(valueName, valueValue, QueryVariableType.Scalar));
                }

                statement.Command = statement.Command.Replace(originalConditionStatement, conditionStatement);
                conditionGroup.Conditions.Add(new QueryCondition(variableName, valueName, null, ParseOperatorType(conditionMatch.Groups[4].Value)));
            }
            else
            {
                //conditionRegex = @"\[?([^\]\.]+)\]?(?:\.\[?([^\]]*)\]?)?\s(.+)\s(.*)";
                conditionRegex = @"\[?([^\]\s\.]+)\]?(?:\.\[?([^\]\s]*)\]?)?\s([^\s]+)\s\'?([^']*)\'?";
                conditionMatch = Regex.Match(inputStatement, conditionRegex);
                if (conditionMatch.Success)
                {
                    var originalConditionStatement = conditionMatch.Groups[0].Value;
                    var conditionStatement = conditionMatch.Groups[0].Value;
                    var conditionStatementIndex = conditionMatch.Groups[0].Index;
                    var conditionStatementLength = conditionMatch.Groups[0].Length;

                    var variableName = CreateVariableName();
                    var variableValue = conditionMatch.Groups[1].Value;
                    var variableIndex = conditionMatch.Groups[1].Index + conditionStatementIndex;
                    conditionStatement = ReplaceCommandVariable(conditionStatement, variableIndex, variableName, variableValue);
                    statement.Scope.Variables.Add(new QueryVariable(variableName, variableValue, QueryVariableType.Expression));

                    var propertyName = CreateVariableName();
                    var propertyValue = conditionMatch.Groups[2].Value;
                    var propertyIndex = conditionMatch.Groups[2].Index + conditionStatementIndex;
                    conditionStatement = ReplaceCommandVariable(conditionStatement, propertyIndex, propertyName, propertyValue);
                    statement.Scope.Variables.Add(new QueryVariable(propertyName, propertyValue, QueryVariableType.Property));

                    var valueName = GetVariableName(conditionMatch.Groups[4].Value);
                    var valueVariable = statement.Scope.GetVariable(valueName);
                    if (valueVariable == null)
                    {
                        valueName = CreateVariableName();
                        var valueValue = conditionMatch.Groups[4].Value.Trim('\'');
                        var valueIndex = conditionMatch.Groups[4].Index + conditionStatementIndex + (conditionStatement.Length - conditionStatementLength);
                        conditionStatement = ReplaceCommandVariable(conditionStatement, valueIndex, valueName, conditionMatch.Groups[4].Value);
                        statement.Scope.Variables.Add(new QueryVariable(valueName, valueValue, QueryVariableType.Scalar));
                    }                        

                    statement.Command = statement.Command.Replace(originalConditionStatement, conditionStatement);
                    conditionGroup.Conditions.Add(new QueryCondition(variableName, valueName, propertyName, ParseOperatorType(conditionMatch.Groups[3].Value)));
                }
                else // Assume no Operator (typically a ConditionGroup)
                {
                    conditionGroup.Conditions.Add(new QueryCondition(inputStatement.Trim(new char[] { '[', ']' }), "true", null, TrakHoundConditionOperator.EQUALS));
                }
            }
        }


        private static long? ParseStart(string query)
        {
            if (!string.IsNullOrEmpty(query))
            {
                var whereQuery = Regex.Match(query, @"where\s+(.*)").Groups[1].Value;
                if (!string.IsNullOrEmpty(whereQuery))
                {
                    var match = Regex.Match(whereQuery, @"start\s+\'([^']+)\'|start\s+([^\s]+)");

                    var group = GetFirstGroupMatch(match);
                    if (group != null)
                    {
                        var datetime = ProcessFunctions(group.Value).ToDateTime();
                        if (datetime > DateTime.MinValue)
                        {
                            return datetime.ToUnixTime();
                        }
                    }
                }
                else
                {
                    var match = Regex.Match(query, @"start\s+\'([^']+)\'|start\s+([^\s]+)");

                    var group = GetFirstGroupMatch(match);
                    if (group != null)
                    {
                        var datetime = ProcessFunctions(group.Value).ToDateTime();
                        if (datetime > DateTime.MinValue)
                        {
                            return datetime.ToUnixTime();
                        }
                    }
                }
            }

            return null;
        }

        private static long? ParseStop(string query)
        {
            if (!string.IsNullOrEmpty(query))
            {
                var whereQuery = Regex.Match(query, @"where\s+(.*)").Groups[1].Value;
                if (!string.IsNullOrEmpty(whereQuery))
                {
                    var match = Regex.Match(whereQuery, @"stop\s+\'([^']+)\'|stop\s+([^\s]+)");

                    var group = GetFirstGroupMatch(match);
                    if (group != null)
                    {
                        var datetime = ProcessFunctions(group.Value).ToDateTime();
                        if (datetime > DateTime.MinValue)
                        {
                            return datetime.ToUnixTime();
                        }
                    }
                }
                else
                {
                    var match = Regex.Match(query, @"stop\s+\'([^']+)\'|stop\s+([^\s]+)");

                    var group = GetFirstGroupMatch(match);
                    if (group != null)
                    {
                        var datetime = ProcessFunctions(group.Value).ToDateTime();
                        if (datetime > DateTime.MinValue)
                        {
                            return datetime.ToUnixTime();
                        }
                    }
                }           
            }

            return null;
        }

        private static string ParseRange(string query)
        {
            if (!string.IsNullOrEmpty(query))
            {
                var whereQuery = Regex.Match(query, @"where\s+(.*)").Groups[1].Value;
                if (!string.IsNullOrEmpty(whereQuery))
                {
                    var match = Regex.Match(whereQuery, @"range\s+\'([^']+)\'|range\s+([^\s]+)");

                    var group = GetFirstGroupMatch(match);
                    if (group != null)
                    {
                        return group.Value;
                    }
                }
                else
                {
                    var match = Regex.Match(query, @"range\s+\'([^']+)\'|range\s+([^\s]+)");

                    var group = GetFirstGroupMatch(match);
                    if (group != null)
                    {
                        return group.Value;
                    }
                }
            }

            return null;
        }

        private static long? ParseSpan(string query)
        {
            if (!string.IsNullOrEmpty(query))
            {
                var match = Regex.Match(query, @"span\s+([0-9a-zA-Z]+)");

                var group = GetFirstGroupMatch(match);
                if (group != null)
                {
                    return (long)group.Value.ToTimeSpan().TotalNanoseconds;
                }
            }

            return int.MaxValue;
        }

        private static int ParseSkip(string query)
        {
            if (!string.IsNullOrEmpty(query))
            {
                var match = Regex.Match(query, @"skip\s+([0-9]+)");

                var group = GetFirstGroupMatch(match);
                if (group != null)
                {
                    return group.Value.ToInt();
                }
            }

            return 0;
        }

        private static int ParseTake(string query)
        {
            if (!string.IsNullOrEmpty(query))
            {
                var match = Regex.Match(query, @"take\s+([0-9]+)");

                var group = GetFirstGroupMatch(match);
                if (group != null)
                {
                    return group.Value.ToInt();
                }
            }

            return int.MaxValue;
        }

        private static SortOrder? ParseOrder(string query)
        {
            if (!string.IsNullOrEmpty(query))
            {
                var match = Regex.Match(query, @"order\s+([a-zA-z]+)");

                var group = GetFirstGroupMatch(match);
                if (group != null)
                {
                    var value = group.Value;
                    if (!string.IsNullOrEmpty(value))
                    {
                        switch (value.ToLower())
                        {
                            case "asc": return SortOrder.Ascending;
                            case "desc": return SortOrder.Descending;
                        }
                    }
                }
            }

            return null;
        }


        private static void ParseGroupBy(QueryStatement statement)
        {
            if (statement != null)
            {
                var groupBy = new List<string>();

                var groupByMatch = Regex.Match(statement.Command, @"group\sby\s+([^\s]*)\s?$|group\sby\s+(.*)\s+start|group\sby\s+(.*)\s+stop");
                if (groupByMatch.Success)
                {
                    var groupByGroup = GetFirstGroupMatch(groupByMatch);
                    if (groupByGroup.Success)
                    {
                        var groupByGroupsMatches = Regex.Matches(groupByGroup.Value, @"\[?(?:([^\]]*))\]?");
                        foreach (Match groupByGroupsMatch in groupByGroupsMatches)
                        {
                            var groupByGroupsMatchValue = GetFirstGroupMatch(groupByGroupsMatch);
                            if (groupByGroupsMatchValue.Success && !string.IsNullOrEmpty(groupByGroupsMatchValue.Value))
                            {
                                groupBy.Add(groupByGroupsMatchValue.Value);
                            }
                        }
                    }
                }

                statement.GroupBy = groupBy;
            }
        }


        static Group GetFirstGroupMatch(Match match)
        {
            if (match != null)
            {
                for (var i = 1; i < match.Groups.Count; i++)
                {
                    var group = match.Groups[i];
                    if (group.Success) return group;
                }
            }

            return null;
        }

        static TrakHoundConditionOperator ParseOperatorType(string input)
        {
            if (input != null)
            {
                switch (input.ToLower())
                {
                    case "<>": return TrakHoundConditionOperator.NOT_EQUALS;
                    case ">": return TrakHoundConditionOperator.GREATER_THAN;
                    case ">=": return TrakHoundConditionOperator.GREATER_THAN_OR_EQUAL;
                    case "<": return TrakHoundConditionOperator.LESS_THAN;
                    case "<=": return TrakHoundConditionOperator.LESS_THAN_OR_EQUAL;
                    case "like": return TrakHoundConditionOperator.LIKE;
                    case "<-": return TrakHoundConditionOperator.HAS_MEMBER;
                    case "->": return TrakHoundConditionOperator.MEMBER_OF;
                }
            }

            return TrakHoundConditionOperator.EQUALS;
        }

        static string GetVariableName(string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                var match = Regex.Match(input, @"\{([^\{]+)\}");
                if (match.Success)
                {
                    return match.Groups[1].Value;
                }
            }

            return null;
        }

        static string ReplaceCommandVariable(string command, int index, string variableName, string variableValue)
        {
            var output = command;

            if (index >= 0)
            {
                output = output.Remove(index, variableValue.Length);
                output = output.Insert(index, $"[{variableName}]");
                output = output.Replace($"[[{variableName}]]", $"[{variableName}]"); // Remove double brackets
            }

            return output;
        }

        static string CreateVariableName()
        {
            return StringFunctions.RandomString(6);
        }

        static string CreateGroupId()
        {
            return StringFunctions.RandomString(3);
        }

        static string CreateGroupVariableName(string groupId)
        {
            return groupId.ToMD5Hash().Substring(0, 5);
        }

        #endregion

        #region "Functions"

        public static string ProcessFunctions(string input)
        {
            var output = input;

            if (!string.IsNullOrEmpty(input))
            {
                var localRegex = @"local\(\'?([^\)\']+)\'?\)";
                var match = Regex.Match(output, localRegex);
                if (match.Success)
                {
                    var x = match.Groups[1].Value.ToDateTime();
                    output = x.ToUniversalTime().ToString("o");
                }

                var lowerRegex = @"lower\(\'?([^\)\']+)\'?\)";
                match = Regex.Match(input, lowerRegex);
                if (match.Success)
                {
                    output = match.Groups[1].Value?.ToLower();
                }

                var upperRegex = @"upper\(\'?([^\)\']+)\'?\)";
                match = Regex.Match(input, upperRegex);
                if (match.Success)
                {
                    output = match.Groups[1].Value?.ToUpper();
                }

                var datetimeRegex = @"datetime\(\'?([^\)\']+)\'?\)";
                match = Regex.Match(input, datetimeRegex);
                if (match.Success)
                {
                    output = match.Groups[1].Value.ToDateTime().ToString();
                }
            }

            return output;
        }

        private static string GetFunctionTarget(string expression)
        {
            if (!string.IsNullOrEmpty(expression))
            {
                var regex = @"([^\(\)]+)(\(([^()]+)\))";
                var match = Regex.Match(expression, regex);
                if (match.Success)
                {
                    var inner = match.Groups[3].Value;
                    return GetFunctionTarget(inner);
                }
                else
                {
                    return expression;
                }
            }

            return null;
        }

        public static string ReplaceFunctionTarget(string expression, string value)
        {
            if (!string.IsNullOrEmpty(expression))
            {
                var regex = @"[^\(\)]+\(([^()]+)\)";
                var match = Regex.Match(expression, regex);
                if (match.Success)
                {
                    var index = match.Groups[1].Index;
                    var length = match.Groups[1].Length;

                    var output = expression.Remove(index, length);
                    return output.Insert(index, value);
                }


                //var regex = @"[^\(\)]+\(([^()]+)\)";
                ////var regex = @"(?:[^\(\)]+)(?:\(([^()]+)\))";
                //var match = Regex.Match(expression, regex);

                ////return Regex.Replace(expression, regex, $"${{1}}{value}");
                ////return Regex.Replace(expression, regex, $"$&{value}");
                //return Regex.Replace(expression, regex, $"$`{{1}}{value}$'");
            }

            return value;
        }

        #endregion

        #region "Variables"

        private string ReplaceVariables(string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                var matches = Regex.Matches(input, @"\{([^\{]+)\}");
                if (matches != null)
                {
                    var output = input;
                    var indexDiff = 0;

                    foreach (Match match in matches)
                    {
                        if (match.Success)
                        {
                            var variable = Scope.GetVariable(match.Groups[1].Value);
                            if (variable != null)
                            {
                                var variableValue = variable.Value.TrimStart('[').TrimEnd(']');

                                var index = match.Groups[0].Index + indexDiff;
                                output = ReplaceVariableValue(output, index, variable.Name, variable.Value);
                                indexDiff = output.Length - input.Length;
                            }
                        }
                    }

                    return output;
                }
                else
                {
                    return input;
                }
            }

            return null;
        }

        static string ReplaceVariableValue(string input, int index, string pattern, string value)
        {
            var output = input;

            if (index >= 0)
            {
                var p = $"{{{pattern}}}";
                output = output.Remove(index, p.Length);
                output = output.Insert(index, value);
                //output = output.Replace($"[[{pattern}]]", $"[{pattern}]"); // Remove double brackets
            }

            return output;
        }

        #endregion


        private static async Task<bool> IndexExists(ITrakHoundSystemEntitiesClient client, string target)
        {
            if (client != null && target != null)
            {
                var results = await client.Objects.IndexExists(new string[] { target });
                if (!results.IsNullOrEmpty())
                {
                    return results.GetValueOrDefault(target);
                }
            }

            return false;
        }

        private static string FormatValue(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                if (!value.IsNumeric())
                {
                    // Detect Timestamp
                    var timestamp = value.ParseDateTime();
                    if (timestamp != null)
                    {
                        return timestamp.Value.ToUnixTime().ToString();
                    }
                }
            }

            return value;
        }


        private static string GetPropertyValue(object obj, string propertyPath)
        {
            if (obj != null)
            {
                var matches = Regex.Matches(propertyPath, @"(?:([^\[\]\,\.\s]+))");
                if (matches != null)
                {
                    var o = obj;
                    var i = 0;
                    var n = matches.Count;

                    do
                    {
                        o = GetSinglePropertyValue(o, matches[i].Groups[i].Value);
                        if (o == null) break;
                        i++;
                    }
                    while (i < n);

                    return o?.ToString();
                }
            }

            return null;
        }

        private static object GetSinglePropertyValue(object obj, string propertyName)
        {
            if (obj != null)
            {
                var type = obj.GetType();
                var property = type.GetProperty(propertyName);
                if (property != null)
                {
                    return property.GetValue(obj);
                }
            }

            return null;
        }

        private QuerySelectType GetSelectType()
        {
            if (!Select.IsNullOrEmpty())
            {
                var select = Select.FirstOrDefault();
                var selectVariable = Scope.GetVariable(select.Expression);
                if (selectVariable != null)
                {
                    switch (selectVariable.Value)
                    {
                        case ">>": return QuerySelectType.TargetObject;
                        case ">": return QuerySelectType.Target;
                        case "*": return QuerySelectType.Content;
                    }
                }               
            }

            return QuerySelectType.Content; // Need to accommodate Property (custom queries)
            //return QuerySelectType.Property;
        }

        private IEnumerable<string> GetSelectExpressions()
        {
            var expressions = new List<string>();

            if (!Select.IsNullOrEmpty())
            {
                foreach (var select in Select)
                {
                    var selectVariable = Scope.GetVariable(select.Expression);
                    if (selectVariable != null)
                    {
                        if (!string.IsNullOrEmpty(selectVariable.Value))
                        {
                            var targetVariable = Scope.GetVariable(Target);
                            if (targetVariable != null)
                            {
                                expressions.Add(TrakHoundPath.Combine(targetVariable.Value, selectVariable.Value));
                            }

                            //expressions.Add(selectVariable.Value);
                        }
                        else
                        {
                            var targetVariable = Scope.GetVariable(Target);
                            if (targetVariable != null)
                            {
                                expressions.Add(targetVariable.Value);
                            }
                        }
                    }
                }
            }

            return expressions.ToDistinct();
        }
    }
}
