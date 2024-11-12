// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrakHound.Clients;
using TrakHound.Drivers;

namespace TrakHound.Entities.QueryEngines.Evaluators.Entities
{
    public class ObjectStatisticEvaluator : EntityEvaluator, IEvaluator
    {
        public ObjectStatisticEvaluator(ConditionObjectQuery query, ITrakHoundObjectEntityModel conditionObject)
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


        public static async Task<IEnumerable<IEvaluatorResult>> Evaluate(QueryStatement statement, ITrakHoundClient client, IEnumerable<ObjectStatisticEvaluator> evaluators)
        {
            var results = new List<IEvaluatorResult>();

            if (statement != null && !statement.Select.IsNullOrEmpty() && client != null && !evaluators.IsNullOrEmpty())
            {
                IEnumerable<ITrakHoundObjectStatisticEntity> entities;

                if (statement.Start > 0 || statement.Stop > 0)
                {
                    var conditionGroups = CreateConditionGroupQueries(statement, evaluators.Where(o => o.ConditionGroup != null).Select(o => o.ConditionGroup));
                    //var conditionGroups = CreateConditionGroupQueries(statement, statement.ConditionGroups);
                    
                    var startUnix = statement.Start ?? 0;
                    if (startUnix < 1) startUnix = 0;

                    var stopUnix = statement.Stop ?? 0;
                    if (stopUnix < 1) stopUnix = long.MaxValue;

                    var rangeQueries = new List<TrakHoundTimeRangeQuery>();
                    foreach (var objectUuid in evaluators.Select(o => o.ConditionObject.Uuid))
                    {
                        rangeQueries.Add(new TrakHoundTimeRangeQuery(objectUuid, startUnix, stopUnix));
                    }

                    entities = await client.Entities.Objects.Statistics.QueryByRange(rangeQueries, statement.Skip, statement.Take);
                    //entities = await client.Entities.Objects.Observations.QueryByObjectUuid(evaluators.Select(o => o.ConditionObject.Uuid), conditionGroups, startUnix, stopUnix);
                    //entities = await client.Entities.Objects.Observations.QueryByObjectUuid(evaluators.Select(o => o.ConditionObject.Uuid), conditionGroups, startUnix, stopUnix);
                    //entities = await client.Entities.Objects.Observations.QueryByObjectUuid(evaluators.Select(o => o.ConditionObject.Uuid), startUnix, stopUnix);
                }
                else
                {
                    entities = await client.Entities.Objects.Statistics.LatestByObjectUuid(evaluators.Select(o => o.ConditionObject.Uuid));
                }

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

                                if (ProcessValue(entityEvaluator.Condition.Operator, QueryStatement.ProcessFunctions(entity.Value), QueryStatement.ProcessFunctions(value)))
                                {
                                    if (!entityEvaluator.ConditionGroup.GroupBy.IsNullOrEmpty())
                                    {
                                        foreach (var groupBy in entityEvaluator.ConditionGroup.GroupBy)
                                        {
                                            var result = new EntityEvaluatorResult();
                                            result.Id = Guid.NewGuid().ToString();
                                            result.ConditionId = entityEvaluator.Condition.Id;
                                            result.GroupId = groupBy;
                                            result.Success = true;
                                            result.Evaluator = entityEvaluator;
                                            result.Entity = entity;
                                            results.Add(result);
                                        }
                                    }
                                    else
                                    {
                                        var result = new EntityEvaluatorResult();
                                        result.Id = Guid.NewGuid().ToString();
                                        result.ConditionId = entityEvaluator.Condition.Id;
                                        result.GroupId = "[GLOBAL]";
                                        result.Success = true;
                                        result.Evaluator = entityEvaluator;
                                        result.Entity = entity;
                                        results.Add(result);
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

        private static bool ProcessValue(TrakHoundConditionOperator operatorType, string target, string pattern)
        {
            switch (operatorType)
            {
                case TrakHoundConditionOperator.EQUALS: return target == pattern;
                case TrakHoundConditionOperator.NOT_EQUALS: return target != pattern;
                case TrakHoundConditionOperator.GREATER_THAN: return target.ToDouble() > pattern.ToDouble();
                case TrakHoundConditionOperator.GREATER_THAN_OR_EQUAL: return target.ToDouble() >= pattern.ToDouble();
                case TrakHoundConditionOperator.LESS_THAN: return target.ToDouble() < pattern.ToDouble();
                case TrakHoundConditionOperator.LESS_THAN_OR_EQUAL: return target.ToDouble() <= pattern.ToDouble();
                case TrakHoundConditionOperator.LIKE: return target.Contains(pattern.Trim('%'));
            }

            return false;
        }

        private static IEnumerable<TrakHoundConditionGroupQuery> CreateConditionGroupQueries(QueryStatement statement, IEnumerable<QueryConditionGroup> inputConditionGroups)
        {
            if (!inputConditionGroups.IsNullOrEmpty())
            {
                var outputConditionGroups = new List<TrakHoundConditionGroupQuery>();

                foreach (var inputConditionGroup in inputConditionGroups)
                {
                    outputConditionGroups.Add(CreateConditionGroupQuery(statement, inputConditionGroup));
                }

                return outputConditionGroups;
            }

            return null;
        }

        private static TrakHoundConditionGroupQuery CreateConditionGroupQuery(QueryStatement statement, QueryConditionGroup inputConditionGroup)
        {
            var outputConditionGroup = new TrakHoundConditionGroupQuery();
            outputConditionGroup.GroupOperator = inputConditionGroup.GroupOperator;

            var subConditionGroups = new List<TrakHoundConditionGroupQuery>();
            var conditions = new List<Drivers.TrakHoundConditionQuery>();

            if (!inputConditionGroup.Conditions.IsNullOrEmpty())
            {
                foreach (var inputCondition in inputConditionGroup.Conditions)
                {
                    var nameVariable = statement.Scope.GetVariable(inputCondition.Variable);
                    var propertyVariable = statement.Scope.GetVariable(inputCondition.Property);
                    var valueVariable = statement.Scope.GetVariable(inputCondition.Value);

                    if (nameVariable != null)
                    {
                        if (nameVariable.Type == QueryVariableType.ConditionGroup)
                        {
                            var conditionGroup = statement.ConditionGroups?.FirstOrDefault(o => o.GroupId == nameVariable.Value);
                            if (conditionGroup != null && conditionGroup.GroupId != inputConditionGroup.GroupId)
                            {
                                subConditionGroups.Add(CreateConditionGroupQuery(statement, conditionGroup));
                            }
                        }
                        else
                        {
                            var target = propertyVariable != null && !string.IsNullOrEmpty(propertyVariable.Value) ? propertyVariable.Value : "value";
                            //var target = inputCondition.Property != null ? inputCondition.Property : "value";

                            conditions.Add(new Drivers.TrakHoundConditionQuery(target, inputCondition.Operator, valueVariable.Value));
                            //conditions.Add(new Drivers.TrakHoundConditionQuery(nameVariable.Value, inputCondition.Operator, valueVariable.Value));
                        }
                    }
                }
            }

            outputConditionGroup.ConditionGroups = subConditionGroups;
            outputConditionGroup.Conditions = conditions;

            return outputConditionGroup;
        }
    }
}
