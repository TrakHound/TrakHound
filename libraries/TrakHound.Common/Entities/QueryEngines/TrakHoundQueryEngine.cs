// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Clients;
using TrakHound.Clients.Collections;
using TrakHound.Entities.Collections;

namespace TrakHound.Entities.QueryEngines
{
    public class TrakHoundQueryEngine
    {
        private readonly ITrakHoundSystemEntitiesClient _client;


        public TrakHoundQueryEngine() { }

        public TrakHoundQueryEngine(ITrakHoundSystemEntitiesClient client)
        {
            _client = client;
        }


        public async Task<TrakHoundQueryResult[]> ExecuteQuery(string query)
        {
            if (!string.IsNullOrEmpty(query))
            {
                var scope = QueryScope.Create(query);
                return await scope.Execute(_client);
            }

            return null;
        }

        public TrakHoundQueryResult[] ExecuteQuery(string query, TrakHoundEntityCollection collection)
        {
            if (!string.IsNullOrEmpty(query))
            {
                var scope = QueryScope.Create(query);
                return scope.Execute(collection);
            }

            return null;
        }


        public async Task<bool> IsMatch(string query, ITrakHoundEntity entity)
        {
            var collection = await GetEntityModel(entity);

            var collectionClient = new TrakHoundCollectionEntitiesClient(collection);

            //var resultCollection = await collectionClient.Query(query);
            //if (resultCollection != null)
            //{
            //    var matchedEntity = collection.GetTargetEntity(entity.Uuid);
            //    //var matchedEntity = collection.GetEntity(entity.Uuid);
            //    if (matchedEntity != null) return true;
            //}




            //var entityModel = await _client.GetEntityModelClient()

            //var results = await ExecuteQuery(query);
            //if (!results.IsNullOrEmpty())
            //{
            //    //// NEED TO UPDATE THIS TO FILTER BASED ON COLLECTION AND WHERE STATEMENT

            //    ////return true;

            //    //var collection = TrakHoundEntityQueryResult.GetCollection(results);
            //    //if (collection != null)
            //    //{
            //    //    var matchedEntity = collection.GetTargetEntity(entity.Uuid);
            //    //    //var matchedEntity = collection.GetEntity(entity.Uuid);
            //    //    if (matchedEntity != null) return true;
            //    //}
            //}

            return false;
        }

        //public async Task<TrakHoundEntityCollection> Match(string query, ITrakHoundEntity entity)
        //{
        //    var collection = await GetEntityModel(entity);
        //    var collectionClient = new TrakHoundEntitiesCollectionClient(collection);

        //    return (await collectionClient.Query(query))?.Entities;
        //}

        //public async Task<TrakHoundQueryResponse> Match(string query, IEnumerable<ITrakHoundEntity> entities)
        //{
        //    var collection = await GetEntityModel(entities);
        //    collection.Add(entities);

        //    var scope = QueryScope.Create(query);
        //    return scope.Execute(collection);

        //    //var collectionClient = new TrakHoundEntitiesCollectionClient(collection);
        //    //return await collectionClient.Query(query);
        //}

        //public async Task<TrakHoundEntityCollection> Match(string query, IEnumerable<ITrakHoundEntity> entities)
        //{
        //    var collection = await GetEntityModel(entities);
        //    var collectionClient = new TrakHoundEntitiesCollectionClient(collection);

        //    return (await collectionClient.Query(query))?.Entities;
        //    return null;
        //}

        //public async Task<bool> MatchContent(string query, string objectUuid)
        //{
        //    var results = await ExecuteQuery(query);
        //    if (!results.IsNullOrEmpty())
        //    {
        //        var collection = TrakHoundEntityQueryResult.GetCollection(results);
        //        if (collection != null)
        //        {
        //            var matchedEntity = collection.GetEntity(objectUuid);
        //            if (matchedEntity != null) return true;
        //        }
        //    }

        //    return false;
        //}

        public static IEnumerable<string> GetSelectExpressions(string query)
        {
            var scope = QueryScope.Create(query);
            if (scope != null)
            {
                var expressions = new List<string>();
                foreach (var statement in scope.Statements)
                {
                    string targetExpression = null;

                    if (statement.Target != null)
                    {
                        var targetVariable = scope.GetVariable(statement.Target);
                        if (targetVariable != null && targetVariable.Type == QueryVariableType.Expression)
                        {
                            targetExpression = targetVariable.Value;
                            expressions.Add(targetExpression);
                        }
                    }
                }

                return expressions.ToDistinct();
            }

            return null;
        }

        public static IEnumerable<string> GetSubscriptions(string query)
        {
            var scope = QueryScope.Create(query);
            if (scope != null)
            {
                var expressions = new List<string>();
                foreach (var statement in scope.Statements)
                {
                    string targetExpression = null;

                    if (statement.Target != null)
                    {
                        var targetVariable = scope.GetVariable(statement.Target);
                        if (targetVariable != null && targetVariable.Type == QueryVariableType.Expression)
                        {
                            targetExpression = targetVariable.Value?.Trim();
                            expressions.Add(targetExpression);
                        }
                    }

                    if (statement.Select != null)
                    {
                        foreach (var select in statement.Select)
                        {
                            var nameVariable = scope.GetVariable(select.Expression);
                            if (nameVariable != null && nameVariable.Type == QueryVariableType.Expression)
                            {
                                if (nameVariable.Value.StartsWith("/"))
                                {
                                    expressions.Add(nameVariable.Value?.Trim());
                                }
                                else if (targetExpression.EndsWith("*") && nameVariable.Value == "*")
                                {
                                    expressions.Add(targetExpression);
                                }
                                else if (nameVariable.Value == ">")
                                {
                                    expressions.Add(targetExpression);
                                }
                                else
                                {
                                    expressions.Add(TrakHoundPath.Combine(targetExpression, nameVariable.Value?.Trim()));
                                }
                            }
                        }
                    }

                    if (!statement.ConditionGroups.IsNullOrEmpty())
                    {
                        foreach (var conditionGroup in  statement.ConditionGroups)
                        {
                            if (!conditionGroup.Conditions.IsNullOrEmpty())
                            {
                                foreach (var condition in conditionGroup.Conditions)
                                {
                                    var nameVariable = scope.GetVariable(condition.Variable);
                                    if (nameVariable != null && nameVariable.Type == QueryVariableType.Expression)
                                    {
                                        expressions.Add(TrakHoundPath.Combine(targetExpression, nameVariable.Value));
                                    }
                                }
                            }
                        }
                    }
                }

                return expressions.ToDistinct();
            }

            return null;
        }

        //public static IEnumerable<TrakHoundEntitySubscriptionRequest> CreateSubscriptions(string query)
        //{
        //    var scope = QueryScope.Create(query);
        //    if (scope != null)
        //    {
        //        var requests = new List<TrakHoundEntitySubscriptionRequest>();
        //        foreach (var statement in scope.Statements)
        //        {
        //            string targetExpression = null;

        //            if (statement.Target != null)
        //            {
        //                var targetVariable = scope.GetVariable(statement.Target);
        //                if (targetVariable != null && targetVariable.Type == QueryVariableType.Expression)
        //                {
        //                    targetExpression = targetVariable.Value?.Trim();
        //                    requests.Add(targetExpression);
        //                }
        //            }

        //            if (statement.Select != null)
        //            {
        //                foreach (var select in statement.Select)
        //                {
        //                    var nameVariable = scope.GetVariable(select.Expression);
        //                    if (nameVariable != null && nameVariable.Type == QueryVariableType.Expression)
        //                    {
        //                        if (nameVariable.Value.StartsWith("/"))
        //                        {
        //                            requests.Add(nameVariable.Value?.Trim());
        //                        }
        //                        else if (targetExpression.EndsWith("*") && nameVariable.Value == "*")
        //                        {
        //                            requests.Add(targetExpression);
        //                        }
        //                        else if (nameVariable.Value == ">")
        //                        {
        //                            requests.Add(targetExpression);
        //                        }
        //                        else
        //                        {
        //                            requests.Add(TrakHoundPath.Combine(targetExpression, nameVariable.Value?.Trim()));
        //                        }
        //                    }
        //                }
        //            }

        //            if (!statement.ConditionGroups.IsNullOrEmpty())
        //            {
        //                foreach (var conditionGroup in statement.ConditionGroups)
        //                {
        //                    if (!conditionGroup.Conditions.IsNullOrEmpty())
        //                    {
        //                        foreach (var condition in conditionGroup.Conditions)
        //                        {
        //                            var nameVariable = scope.GetVariable(condition.Variable);
        //                            if (nameVariable != null && nameVariable.Type == QueryVariableType.Expression)
        //                            {
        //                                requests.Add(TrakHoundPath.Combine(targetExpression, nameVariable.Value));
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }

        //        return expressions.ToDistinct();
        //    }

        //    return null;
        //}


        //public QueryExecutionPlan GetExecutionPlan(string query)
        //{
        //    var scope = QueryScope.Create(query);
        //    return QueryExecutionPlan.Create(scope);
        //}


        public async Task<TrakHoundEntityCollection> GetEntityModel(ITrakHoundEntity entity)
        {
            var collection = new TrakHoundEntityCollection();

            if (entity != null)
            {
                var entityClass = entity.Class.ConvertEnum<TrakHoundObjectsEntityClass>();
                switch (entityClass)
                {
                    case TrakHoundObjectsEntityClass.Assignment: collection.Add(await _client.Objects.Assignment.ReadByUuid(entity.Uuid)); break;
                    case TrakHoundObjectsEntityClass.Boolean: collection.Add(await _client.Objects.Boolean.ReadByUuid(entity.Uuid)); break;
                    case TrakHoundObjectsEntityClass.Duration: collection.Add(await _client.Objects.Duration.ReadByUuid(entity.Uuid)); break;
                    case TrakHoundObjectsEntityClass.Event: collection.Add(await _client.Objects.Event.ReadByUuid(entity.Uuid)); break;
                    //case TrakHoundObjectsEntityClass.Feed: collection.Add(await _client.Objects.Feed.ReadByUuid(entity.Uuid)); break;
                    case TrakHoundObjectsEntityClass.Group: collection.Add(await _client.Objects.Group.ReadByUuid(entity.Uuid)); break;
                    case TrakHoundObjectsEntityClass.Hash: collection.Add(await _client.Objects.Hash.ReadByUuid(entity.Uuid)); break;
                    case TrakHoundObjectsEntityClass.Number: collection.Add(await _client.Objects.Number.ReadByUuid(entity.Uuid)); break;
                    case TrakHoundObjectsEntityClass.Observation: collection.Add(await _client.Objects.Observation.ReadByUuid(entity.Uuid)); break;
                    case TrakHoundObjectsEntityClass.Set: collection.Add(await _client.Objects.Set.ReadByUuid(entity.Uuid)); break;
                    case TrakHoundObjectsEntityClass.State: collection.Add(await _client.Objects.State.ReadByUuid(entity.Uuid)); break;
                    case TrakHoundObjectsEntityClass.Statistic: collection.Add(await _client.Objects.Statistic.ReadByUuid(entity.Uuid)); break;
                    case TrakHoundObjectsEntityClass.String: collection.Add(await _client.Objects.String.ReadByUuid(entity.Uuid)); break;
                    case TrakHoundObjectsEntityClass.Timestamp: collection.Add(await _client.Objects.Timestamp.ReadByUuid(entity.Uuid)); break;
                    //case TrakHoundObjectsEntityClass.Wiki: collection.Add(await _client.Objects.Wiki.ReadByUuid(entity.Uuid)); break;
                }
            }

            return collection;
        }

        public async Task<TrakHoundEntityCollection> GetEntityModel(IEnumerable<ITrakHoundEntity> entities)
        {
            var collection = new TrakHoundEntityCollection();

            if (!entities.IsNullOrEmpty())
            {
                var objectUuids = new List<string>();

                foreach (var entity in entities)
                {
                    var entityClass = entity.Class.ConvertEnum<TrakHoundObjectsEntityClass>();
                    switch (entityClass)
                    {
                        //case TrakHoundObjectsEntityClass.Assignment: assignmentObjectUuids.Add(entity.Uuid); break;
                        case TrakHoundObjectsEntityClass.Boolean: objectUuids.Add(((ITrakHoundObjectBooleanEntity)entity).ObjectUuid); break;
                        case TrakHoundObjectsEntityClass.Duration: objectUuids.Add(((ITrakHoundObjectDurationEntity)entity).ObjectUuid); break;
                        //case TrakHoundObjectsEntityClass.Event: eventObjectUuids.Add(entity.Uuid); break;
                        //case TrakHoundObjectsEntityClass.Feed: objectUuids.Add(((ITrakHoundObjectFeedEntity)entity).ObjectUuid); break;
                        //case TrakHoundObjectsEntityClass.Group: groupObjectUuids.Add(entity.Uuid); break;
                        case TrakHoundObjectsEntityClass.Hash: objectUuids.Add(((ITrakHoundObjectHashEntity)entity).ObjectUuid); break;
                        case TrakHoundObjectsEntityClass.Message: objectUuids.Add(((ITrakHoundObjectMessageEntity)entity).ObjectUuid); break;
                        case TrakHoundObjectsEntityClass.Number: objectUuids.Add(((ITrakHoundObjectNumberEntity)entity).ObjectUuid); break;
                        case TrakHoundObjectsEntityClass.Observation: objectUuids.Add(((ITrakHoundObjectObservationEntity)entity).ObjectUuid); break;
                        case TrakHoundObjectsEntityClass.Set: objectUuids.Add(((ITrakHoundObjectSetEntity)entity).ObjectUuid); break;
                        case TrakHoundObjectsEntityClass.State: objectUuids.Add(((ITrakHoundObjectStateEntity)entity).ObjectUuid); break;
                        //case TrakHoundObjectsEntityClass.Statistic: statisticObjectUuids.Add(entity.Uuid); break;
                        case TrakHoundObjectsEntityClass.String: objectUuids.Add(((ITrakHoundObjectStringEntity)entity).ObjectUuid); break;
                        case TrakHoundObjectsEntityClass.Timestamp: objectUuids.Add(((ITrakHoundObjectTimestampEntity)entity).ObjectUuid); break;
                        //case TrakHoundObjectsEntityClass.Wiki: wikiObjectUuids.Add(entity.Uuid); break;
                    }
                }

                collection.Add(await _client.Objects.ReadByUuid(objectUuids));
                collection.Add(await _client.Objects.QueryRootByChildUuid(objectUuids));
            }

            return collection;
        }
    }
}
