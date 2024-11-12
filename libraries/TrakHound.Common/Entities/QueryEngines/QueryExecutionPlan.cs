// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;

namespace TrakHound.Entities.QueryEngines
{
    public class QueryExecutionPlan
    {
        public string Id { get; set; }

        public IEnumerable<IQueryExecutionPlanStep> Steps { get; set; }


        public static QueryExecutionPlan Create(QueryStatement statement)
        {
            if (statement != null)
            {
                var plan = new QueryExecutionPlan();
                var steps = new List<IQueryExecutionPlanStep>();

                var index = 0;

                steps.AddRange(CreateTarget(statement, ref index));
                steps.AddRange(CreateConditionGroups(statement, ref index));

                plan.Steps = steps;
                return plan;
            }

            return null;
        }

        public static IEnumerable<IQueryExecutionPlanStep> CreateTarget(QueryStatement statement, ref int index)
        {
            var steps = new List<IQueryExecutionPlanStep>();

            if (!string.IsNullOrEmpty(statement.Target))
            {
                var targetVariable = statement.Scope.GetVariable(statement.Target);
                if (targetVariable != null)
                {
                    var step = new QueryTargetExecutionPlanStep();
                    step.Index = index;
                    step.Id = Guid.NewGuid().ToString();
                    step.Target = targetVariable.Value;

                    steps.Add(step);
                    index++;
                }
            }

            return steps;
        }

        public static IEnumerable<IQueryExecutionPlanStep> CreateConditionGroups(QueryStatement statement, ref int index)
        {
            var steps = new List<IQueryExecutionPlanStep>();

            if (statement != null && !statement.ConditionGroups.IsNullOrEmpty())
            {
                var oConditionGroups = statement.ConditionGroups.OrderBy(o => o.Order);
                foreach (var conditionGroup in oConditionGroups)
                {
                    var step = new QueryConditionGroupExecutionPlanStep();
                    step.Index = index;
                    step.Id = Guid.NewGuid().ToString();
                    step.GroupOperator = conditionGroup.GroupOperator;
                    step.Conditions = conditionGroup.Conditions;

                    steps.Add(step);
                    index++;
                }
            }

            return steps;
        }
    }
}
