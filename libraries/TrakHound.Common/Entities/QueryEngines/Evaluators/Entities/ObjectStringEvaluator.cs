// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrakHound.Clients;

namespace TrakHound.Entities.QueryEngines.Evaluators.Entities
{
    public class ObjectStringEvaluator : EntityEvaluator, IEvaluator
    {
        public ObjectStringEvaluator(ConditionObjectQuery query, ITrakHoundObjectEntity conditionObject)
        {
            ContentType = conditionObject.ContentType.ConvertEnum<TrakHoundObjectContentType>();
            //TargetObject = query.TargetObject;
            ConditionObject = conditionObject;
            Query = query;
            ConditionGroup = query.ConditionGroup;
            Condition = query.Condition;
        }


        public bool Evaluate()
        {
            return false;
        }

        public static async Task<IEnumerable<IEvaluatorResult>> Evaluate(QueryStatement statement, ITrakHoundSystemEntitiesClient client, IEnumerable<ObjectStringEvaluator> evaluators)
        {
            var results = new List<IEvaluatorResult>();

            if (statement != null && !statement.Select.IsNullOrEmpty() && client != null && !evaluators.IsNullOrEmpty())
            {
                var entities = await client.Objects.String.QueryByObjectUuid(evaluators.Select(o => o.ConditionObject.Uuid));
                if (!entities.IsNullOrEmpty())
                {
                    var dEntities = entities.ToDistinct();

                    foreach (var entity in dEntities)
                    {
                        var entityEvaluators = evaluators.Where(o => o.ConditionObject.Uuid == entity.ObjectUuid);
                        if (!entityEvaluators.IsNullOrEmpty())
                        {
                            foreach (var entityEvaluator in entityEvaluators)
                            {
                                var variable = statement.Scope.GetVariable(entityEvaluator.Condition.Variable)?.Value;
                                if (variable != null) variable = QueryStatement.ReplaceFunctionTarget(variable, entity.Value);

                                var property = statement.Scope.GetVariable(entityEvaluator.Condition.Property)?.Value;
                                if (property != null) property = QueryStatement.ReplaceFunctionTarget(property, GetEntityProperty(entity, property));

                                var value = statement.Scope.GetVariable(entityEvaluator.Condition.Value)?.Value;

                                var target = property ?? variable;

                                if (ProcessValue(entityEvaluator.Condition.Operator, QueryStatement.ProcessFunctions(target), QueryStatement.ProcessFunctions(value)))
                                //if (ProcessValue(entityEvaluator.Condition.Operator, statement.ProcessFunctions(variable), statement.ProcessFunctions(value)))
                                {
                                    foreach (var select in statement.Select)
                                    {
                                        var selectQuery = statement.Scope.GetVariable(select.Expression);
                                        if (selectQuery != null)
                                        {
                                            var result = new EntityEvaluatorResult();
                                            result.Id = Guid.NewGuid().ToString();
                                            result.ConditionId = entityEvaluator.Condition.Id;
                                            result.GroupId = entityEvaluator.ConditionObject?.Uuid;
                                            result.Query = entityEvaluator.Query;
                                            result.Success = true;
                                            result.Evaluator = entityEvaluator;
                                            result.Entity = entity;
                                            results.Add(result);

                                            //var selectTarget = TrakQL.Match(selectQuery.Value, entityEvaluator.ConditionObject)?.FirstOrDefault();
                                            //if (selectTarget != null)
                                            //{
                                            //    var result = new EntityEvaluatorResult();
                                            //    result.Id = Guid.NewGuid().ToString();
                                            //    result.ConditionId = entityEvaluator.Condition.Id;
                                            //    result.ValueId = selectTarget.Uuid;
                                            //    result.Success = true;
                                            //    result.Evaluator = entityEvaluator;
                                            //    result.Entity = entity;
                                            //    results.Add(result);
                                            //}
                                        }
                                    }
                                }
                                else
                                {
                                    var result = new EntityEvaluatorResult();
                                    result.Id = Guid.NewGuid().ToString();
                                    result.ConditionId = entityEvaluator.Condition.Id;
                                    result.Success = false;
                                    results.Add(result);
                                }
                            }
                        }
                    }
                }
                else
                {
                    //foreach (var entityQuery in observationQueries)
                    //{
                    //    entityResults.Add(new EntityResult(entityQuery, null));
                    //}
                }
            }

            return results;
        }


        //public static async Task<IEnumerable<IEvaluatorResult>> Evaluate(QueryStatement statement, ITrakHoundSystemEntitiesClient client, IEnumerable<ConditionObjectQuery> queries)
        //{
        //    var results = new List<IEvaluatorResult>();

        //    if (statement != null && !statement.Select.IsNullOrEmpty() && client != null && !queries.IsNullOrEmpty())
        //    {
        //        var targetObjectUuids = queries.Select(o => o.ConditionObject?.Uuid).Distinct();

        //        var entities = await client.Objects.String.QueryByObjectUuid(targetObjectUuids);
        //        if (!entities.IsNullOrEmpty())
        //        {
        //            var dEntities = entities.ToDistinct();

        //            foreach (var entity in dEntities)
        //            {
        //                var conditionQueries = queries.Where(o => o.ConditionObject?.Uuid == entity.ObjectUuid);
        //                if (!conditionQueries.IsNullOrEmpty())
        //                {
        //                    foreach (var conditionQuery in conditionQueries)
        //                    {
        //                        var variable = statement.Scope.GetVariable(conditionQuery.Condition.Variable)?.Value;
        //                        if (variable != null) variable = QueryStatement.ReplaceFunctionTarget(variable, entity.Value);

        //                        var property = statement.Scope.GetVariable(conditionQuery.Condition.Property)?.Value;
        //                        if (property != null) property = QueryStatement.ReplaceFunctionTarget(property, GetEntityProperty(entity, property));

        //                        var value = statement.Scope.GetVariable(conditionQuery.Condition.Value)?.Value;

        //                        var target = property ?? variable;

        //                        if (ProcessValue(conditionQuery.Condition.Operator, QueryStatement.ProcessFunctions(target), QueryStatement.ProcessFunctions(value)))
        //                        {
        //                            foreach (var select in statement.Select)
        //                            {
        //                                var selectQuery = statement.Scope.GetVariable(select.Expression);
        //                                if (selectQuery != null)
        //                                {
        //                                    var result = new EntityEvaluatorResult();
        //                                    result.Id = Guid.NewGuid().ToString();
        //                                    result.ConditionId = conditionQuery.Condition.Id;
        //                                    //result.GroupId = entityEvaluator.ConditionObject?.Uuid;
        //                                    result.Query = conditionQuery;
        //                                    result.Success = true;
        //                                    result.Entity = entity;
        //                                    results.Add(result);

        //                                    //var selectTarget = TrakQL.Match(selectQuery.Value, entityEvaluator.ConditionObject)?.FirstOrDefault();
        //                                    //if (selectTarget != null)
        //                                    //{
        //                                    //    var result = new EntityEvaluatorResult();
        //                                    //    result.Id = Guid.NewGuid().ToString();
        //                                    //    result.ConditionId = entityEvaluator.Condition.Id;
        //                                    //    result.ValueId = selectTarget.Uuid;
        //                                    //    result.Success = true;
        //                                    //    result.Evaluator = entityEvaluator;
        //                                    //    result.Entity = entity;
        //                                    //    results.Add(result);
        //                                    //}
        //                                }
        //                            }
        //                        }
        //                        else
        //                        {
        //                            var result = new EntityEvaluatorResult();
        //                            result.Id = Guid.NewGuid().ToString();
        //                            result.ConditionId = conditionQuery.Condition.Id;
        //                            result.Success = false;
        //                            results.Add(result);
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        else
        //        {
        //            //foreach (var entityQuery in observationQueries)
        //            //{
        //            //    entityResults.Add(new EntityResult(entityQuery, null));
        //            //}
        //        }
        //    }

        //    return results;
        //}


        //public static async Task<IEnumerable<IEvaluatorResult>> Evaluate(QueryStatement statement, ITrakHoundSystemEntitiesClient client, IEnumerable<ObjectStringEvaluator> evaluators)
        //{
        //    var results = new List<IEvaluatorResult>();

        //    if (statement != null && !statement.Select.IsNullOrEmpty() && client != null && !evaluators.IsNullOrEmpty())
        //    {
        //        var entities = await client.Objects.String.QueryByObjectUuid(evaluators.Select(o => o.ConditionObject.Uuid));
        //        if (!entities.IsNullOrEmpty())
        //        {
        //            var dEntities = entities.ToDistinct();

        //            foreach (var entity in dEntities)
        //            {
        //                var entityEvaluators = evaluators.Where(o => o.ConditionObject.Uuid == entity.ObjectUuid);
        //                if (!entityEvaluators.IsNullOrEmpty())
        //                {
        //                    foreach (var entityEvaluator in entityEvaluators)
        //                    {
        //                        var variable = statement.Scope.GetVariable(entityEvaluator.Condition.Variable)?.Value;
        //                        if (variable != null) variable = QueryStatement.ReplaceFunctionTarget(variable, entity.Value);

        //                        var property = statement.Scope.GetVariable(entityEvaluator.Condition.Property)?.Value;
        //                        if (property != null) property = QueryStatement.ReplaceFunctionTarget(property, GetEntityProperty(entity, property));

        //                        var value = statement.Scope.GetVariable(entityEvaluator.Condition.Value)?.Value;

        //                        var target = property ?? variable;

        //                        if (ProcessValue(entityEvaluator.Condition.Operator, QueryStatement.ProcessFunctions(target), QueryStatement.ProcessFunctions(value)))
        //                        //if (ProcessValue(entityEvaluator.Condition.Operator, statement.ProcessFunctions(variable), statement.ProcessFunctions(value)))
        //                        {
        //                            foreach (var select in statement.Select)
        //                            {
        //                                var selectQuery = statement.Scope.GetVariable(select.Expression);
        //                                if (selectQuery != null)
        //                                {
        //                                    var result = new EntityEvaluatorResult();
        //                                    result.Id = Guid.NewGuid().ToString();
        //                                    result.ConditionId = entityEvaluator.Condition.Id;
        //                                    result.GroupId = entityEvaluator.ConditionObject?.Uuid;
        //                                    result.Success = true;
        //                                    result.Evaluator = entityEvaluator;
        //                                    result.Entity = entity;
        //                                    results.Add(result);

        //                                    //var selectTarget = TrakQL.Match(selectQuery.Value, entityEvaluator.ConditionObject)?.FirstOrDefault();
        //                                    //if (selectTarget != null)
        //                                    //{
        //                                    //    var result = new EntityEvaluatorResult();
        //                                    //    result.Id = Guid.NewGuid().ToString();
        //                                    //    result.ConditionId = entityEvaluator.Condition.Id;
        //                                    //    result.ValueId = selectTarget.Uuid;
        //                                    //    result.Success = true;
        //                                    //    result.Evaluator = entityEvaluator;
        //                                    //    result.Entity = entity;
        //                                    //    results.Add(result);
        //                                    //}
        //                                }
        //                            }
        //                        }
        //                        else
        //                        {
        //                            var result = new EntityEvaluatorResult();
        //                            result.Id = Guid.NewGuid().ToString();
        //                            result.ConditionId = entityEvaluator.Condition.Id;
        //                            result.Success = false;
        //                            results.Add(result);
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        else
        //        {
        //            //foreach (var entityQuery in observationQueries)
        //            //{
        //            //    entityResults.Add(new EntityResult(entityQuery, null));
        //            //}
        //        }
        //    }

        //    return results;
        //}

        private static bool ProcessValue(TrakHoundConditionOperator operatorType, string target, string pattern)
        //private static bool ProcessValue(OperatorType operatorType, string target, Type targetType, string pattern)
        {
            //var type = targetType ?? typeof(string);

            //var value = pattern;
            //if (type == typeof(DateTime)) value = value.ToDateTime()

            switch (operatorType)
            {
                case TrakHoundConditionOperator.EQUALS: return target == pattern;
                case TrakHoundConditionOperator.NOT_EQUALS: return target != pattern;
                case TrakHoundConditionOperator.GREATER_THAN: return target.ToDouble() > pattern.ToDouble();
                case TrakHoundConditionOperator.LESS_THAN: return target.ToDouble() < pattern.ToDouble();
                case TrakHoundConditionOperator.LIKE: return target.Contains(pattern.Trim('%'));
            }

            return false;
        }

        private static string GetEntityProperty(ITrakHoundObjectStringEntity entity, string propertyName)
        {
            if (entity != null && propertyName != null)
            {
                var properties = entity.GetType().GetProperties();
                var property = properties.FirstOrDefault(o => o.Name.ToLower() == propertyName.ToLower());
                if (property != null)
                {
                    return property.GetValue(entity)?.ToString();
                }
            }

            return null;
        }

        private static Type GetEntityPropertyType(ITrakHoundObjectStringEntity entity, string propertyName)
        {
            if (entity != null && propertyName != null)
            {
                var properties = entity.GetType().GetProperties();
                var property = properties.FirstOrDefault(o => o.Name.ToLower() == propertyName.ToLower());
                if (property != null)
                {
                    return property.PropertyType;
                }
            }

            return null;
        }
    }
}
