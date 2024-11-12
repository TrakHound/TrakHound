// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrakHound.Clients;

namespace TrakHound.Entities.QueryEngines.Evaluators.Entities
{
    public class ObjectDurationEvaluator : EntityEvaluator, IEvaluator
    {
        public ObjectDurationEvaluator(ConditionObjectQuery query, ITrakHoundObjectEntityModel conditionObject) 
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


        public static async Task<IEnumerable<IEvaluatorResult>> Evaluate(QueryStatement statement, ITrakHoundSystemEntitiesClient client, IEnumerable<ObjectDurationEvaluator> evaluators)
        {
            var results = new List<IEvaluatorResult>();

            if (statement != null && !statement.Select.IsNullOrEmpty() && client != null && !evaluators.IsNullOrEmpty())
            {
                var entities = await client.Objects.Duration.QueryByObjectUuid(evaluators.Select(o => o.ConditionObject.Uuid));
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
                                var value = statement.Scope.GetVariable(entityEvaluator.Condition.Value)?.Value;

                                if (ProcessValue(entityEvaluator.Condition.Operator, entity.Value, QueryStatement.ProcessFunctions(value)))
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

        private static bool ProcessValue(TrakHoundConditionOperator operatorType, ulong target, string pattern)
        {
            var value = pattern.ToTimeSpan().Ticks.ToULong();

            switch (operatorType)
            {
                case TrakHoundConditionOperator.EQUALS: return target == value;
                case TrakHoundConditionOperator.NOT_EQUALS: return target != value;
                case TrakHoundConditionOperator.GREATER_THAN: return target > value;
                case TrakHoundConditionOperator.GREATER_THAN_OR_EQUAL: return target >= value;
                case TrakHoundConditionOperator.LESS_THAN: return target < value;
                case TrakHoundConditionOperator.LESS_THAN_OR_EQUAL: return target <= value;
            }

            return false;
        }
    }
}
