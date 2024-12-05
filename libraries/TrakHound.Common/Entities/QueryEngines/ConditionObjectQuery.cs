// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Entities.QueryEngines
{
    public struct ConditionObjectQuery
    {
        //public ITrakHoundObjectEntity TargetObject { get; set; }

        public string TargetObjectUuid { get; set; }

        public ITrakHoundObjectEntity ConditionObject { get; set; }

        public QueryConditionGroup ConditionGroup { get; set; }

        public QueryCondition Condition { get; set; }

        public string Query { get; set; }
    }
}
