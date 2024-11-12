// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;

namespace TrakHound
{
    public struct TrakHoundConditionQuery
    {
        public string ConditionId { get; set; }

        public TrakHoundConditionOperator Operator { get; set; } 

        public string Target { get; set; }

        public string Pattern { get; set; }


        public TrakHoundConditionQuery(string target, TrakHoundConditionOperator conditionOperator, string pattern, string conditionId = null)
        {
            Target = target;
            Operator = conditionOperator;
            Pattern = pattern;
            ConditionId = conditionId ?? Guid.NewGuid().ToString();
        }
    }
}
