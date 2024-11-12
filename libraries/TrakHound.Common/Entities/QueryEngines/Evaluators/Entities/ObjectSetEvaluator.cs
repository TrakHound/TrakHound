// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrakHound.Clients;

namespace TrakHound.Entities.QueryEngines.Evaluators.Entities
{
    public class ObjectSetEvaluator : EntityEvaluator, IEvaluator
    {
        public ObjectSetEvaluator(ConditionObjectQuery query, ITrakHoundObjectEntityModel conditionObject)
        {
            ContentType = conditionObject.ContentType.ConvertEnum<TrakHoundObjectContentType>();
            TargetObject = query.TargetObject;
            ConditionObject = conditionObject;
            ConditionGroup = query.ConditionGroup;
            Condition = query.Condition;
        }


        public bool Evaluate()
        {
            return false;
        }


        public static async Task<IEnumerable<IEvaluatorResult>> Evaluate(QueryStatement statement, ITrakHoundSystemEntitiesClient client, IEnumerable<ObjectSetEvaluator> evaluators)
        {
            var results = new List<IEvaluatorResult>();

            if (statement != null && !statement.Select.IsNullOrEmpty() && client != null && !evaluators.IsNullOrEmpty())
            {
                var entities = await client.Objects.Set.QueryByObjectUuid(evaluators.Select(o => o.ConditionObject.Uuid));
                // Should be able to query by member

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

                                var value = statement.Scope.GetVariable(entityEvaluator.Condition.Value)?.Value;

                                if (entityEvaluator.Condition.Operator == TrakHoundConditionOperator.HAS_MEMBER && QueryStatement.ProcessFunctions(variable) == QueryStatement.ProcessFunctions(value))
                                {
                                    foreach (var select in statement.Select)
                                    {
                                        var selectQuery = statement.Scope.GetVariable(select.Expression);
                                        if (selectQuery != null)
                                        {
                                            var selectTarget = TrakHoundObjectExpression.Match(selectQuery.Value, entityEvaluator.ConditionObject)?.FirstOrDefault();
                                            if (selectTarget != null)
                                            {
                                                var result = new EntityEvaluatorResult();
                                                result.Id = Guid.NewGuid().ToString();
                                                result.ConditionId = entityEvaluator.Condition.Id;
                                                result.GroupId = selectTarget.Uuid;
                                                result.Success = true;
                                                result.Evaluator = entityEvaluator;
                                                result.Entity = entity;
                                                results.Add(result);
                                            }
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

        //private static bool ProcessValue(OperatorType operatorType, string target, string pattern)
        //{
        //    switch (operatorType)
        //    {
        //        case OperatorType.EQUALS: return target == pattern;
        //        case OperatorType.NOT_EQUALS: return target != pattern;
        //        case OperatorType.GREATER_THAN: return target.ToDouble() > pattern.ToDouble();
        //        case OperatorType.LESS_THAN: return target.ToDouble() < pattern.ToDouble();
        //    }

        //    return false;
        //}
    }
}
