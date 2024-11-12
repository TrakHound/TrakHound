// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace TrakHound.Entities.QueryEngines
{
    public class QueryConditionGroup
    {
        public string GroupId { get; set; }

        public int Order { get; set; }

        public TrakHoundConditionGroupOperator GroupOperator { get; set; }

        public List<QueryCondition> Conditions { get; set; }

        public List<string> GroupBy { get; set; }


        public QueryConditionGroup(string groupId, TrakHoundConditionGroupOperator groupOperator, int order = 0)
        {
            GroupId = groupId;
            Order = order;
            GroupOperator = groupOperator;
            Conditions = new List<QueryCondition>();
            GroupBy = new List<string>();
        }
    }
}
