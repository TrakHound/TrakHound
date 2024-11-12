// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrakHound.Clients;
using TrakHound.Entities.Collections;

namespace TrakHound.Entities.QueryEngines.Evaluators
{
    public class ConditionEvaluator
    {
        private readonly QueryCondition _condition;


        public ConditionEvaluator(QueryCondition condition)
        {
            _condition = condition;
        }


        //public async Task<IEnumerable<IEvaluatorResult>> Evaluate(ITrakHoundClient client, TrakHoundEntityCollection collection)
        //{
        //    var results = new List<IEvaluatorResult>();

        //    if (_conditionGroup != null && !_conditionGroup.Conditions.IsNullOrEmpty())
        //    {
        //        foreach (var condition in _conditionGroup.Conditions)
        //        {

        //        }
        //    }

        //    return results;
        //}

        //public bool Evaluate()
        //{
        //    return false;
        //}
    }
}
