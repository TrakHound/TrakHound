// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;

namespace TrakHound.Entities.QueryEngines
{
    public struct QueryCondition
    {
        public string Id { get; set; }

        public string Variable { get; set; }

        public string Value { get; set; }

        public string Property { get; set; }

        public TrakHoundConditionOperator Operator { get; set; }


        public QueryCondition(string variable, string value, string property = null, TrakHoundConditionOperator operatorType = TrakHoundConditionOperator.EQUALS)
        {
            Id = Guid.NewGuid().ToString();
            Variable = variable;
            Value = value;
            Property = property;
            Operator = operatorType;
        }
    }
}
