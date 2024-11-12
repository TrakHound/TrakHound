// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Entities.QueryEngines
{
    public struct ConditionIndexQuery
    {
        public QueryConditionGroup ConditionGroup { get; set; }

        public string Target { get; set; }

        public TrakHoundConditionOperator Operator { get; set; }

        public string Query { get; set; }
    }
}
