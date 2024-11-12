// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;

namespace TrakHound
{
    public struct TrakHoundConditionGroupQuery
    {
        public string GroupId { get; set; }

        public TrakHoundConditionGroupOperator GroupOperator { get; set; }

        public IEnumerable<TrakHoundConditionGroupQuery> ConditionGroups { get; set; }

        public IEnumerable<TrakHoundConditionQuery> Conditions { get; set; }
    }
}
