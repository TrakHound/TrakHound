// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrakHound.Clients;

namespace TrakHound.Entities.QueryEngines.Evaluators
{
    public class ConditionGroupEvaluator
    {
        private readonly QueryConditionGroup _conditionGroup;


        public ConditionGroupEvaluator(QueryConditionGroup conditionGroup)
        {
            _conditionGroup = conditionGroup;
        }


        public async Task<IEnumerable<IEvaluatorResult>> Evaluate(QueryStatement statement, ITrakHoundSystemEntitiesClient client, IEnumerable<IEvaluatorResult> evaluatorResults)
        {
            var results = new List<IEvaluatorResult>();

            if (statement != null && _conditionGroup != null && !_conditionGroup.Conditions.IsNullOrEmpty() && !evaluatorResults.IsNullOrEmpty())
            {
                switch (_conditionGroup.GroupOperator)
                {
                    case TrakHoundConditionGroupOperator.AND: results.AddRange(EvaluateAnd(statement, evaluatorResults)); break;
                    case TrakHoundConditionGroupOperator.OR: results.AddRange(EvaluateOr(statement, evaluatorResults)); break;
                    //case TrakHoundConditionGroupOperator.NONE: results.AddRange(EvaluateOr(statement, evaluatorResults)); break;
                }
            }

            return results;
        }

        private IEnumerable<IEvaluatorResult> EvaluateAnd(QueryStatement statement, IEnumerable<IEvaluatorResult> evaluatorResults)
        {
            var results = new List<IEvaluatorResult>();

            if (!evaluatorResults.IsNullOrEmpty())
            {
                var matchedResults = new ListDictionary<string, IEvaluatorResult>();
                var keys = new List<string>();

                foreach (var condition in _conditionGroup.Conditions)
                {
                    var conditionEvaluatorResults = evaluatorResults.Where(o => o.ConditionId == condition.Id);
                    if (!conditionEvaluatorResults.IsNullOrEmpty())
                    {
                        foreach (var conditionEvaluatorResult in conditionEvaluatorResults)
                        {
                            if (conditionEvaluatorResult.Success)
                            {
                                matchedResults.Add(condition.Id, conditionEvaluatorResult);
                                keys.Add(condition.Id);
                            }
                        }
                    }
                }

                if (matchedResults.Count > 0)
                {
                    var dKeys = keys.Distinct();
                    var resultCounts = new Dictionary<string, int>();
                    var valueResults = new List<IEvaluatorResult>();

                    var matchCount = 1;
                    //var matchCount = _conditionGroup.Conditions.Count();

                    foreach (var key in dKeys)
                    {
                        var keyResults = matchedResults.Get(key);
                        if (!keyResults.IsNullOrEmpty())
                        {
                            foreach (var keyResult in keyResults)
                            {
                                var count = 0;
                                if (resultCounts.ContainsKey(keyResult.GroupId)) count = resultCounts[keyResult.GroupId];
                                resultCounts.Remove(keyResult.GroupId);
                                resultCounts.Add(keyResult.GroupId, count + 1);

                                valueResults.Add(keyResult);
                            }
                        }
                    }

                    foreach (var resultCount in resultCounts)
                    {
                        if (resultCount.Value >= matchCount)
                        {
                            results.AddRange(valueResults.Where(o => o.GroupId == resultCount.Key));
                        }
                    }
                }
            }

            return results;
        }

        private IEnumerable<IEvaluatorResult> EvaluateOr(QueryStatement statement, IEnumerable<IEvaluatorResult> evaluatorResults)
        {
            var results = new List<IEvaluatorResult>();

            if (!evaluatorResults.IsNullOrEmpty())
            {
                var matchedResults = new List<IEvaluatorResult>();

                foreach (var condition in _conditionGroup.Conditions)
                {
                    var conditionEvaluatorResults = evaluatorResults.Where(o => o.ConditionId == condition.Id);
                    if (!conditionEvaluatorResults.IsNullOrEmpty())
                    {
                        foreach (var conditionEvaluatorResult in conditionEvaluatorResults)
                        {
                            if (conditionEvaluatorResult.Success)
                            {
                                matchedResults.Add(conditionEvaluatorResult);
                            }
                        }
                    }
                }

                results.AddRange(matchedResults);
            }

            return results;
        }
    }
}
