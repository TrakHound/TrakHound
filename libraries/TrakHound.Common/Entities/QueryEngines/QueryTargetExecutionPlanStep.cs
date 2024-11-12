// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrakHound.Entities.QueryEngines
{
    public class QueryTargetExecutionPlanStep : QueryExecutionPlanStep
    {
        public string Target { get; set; }
    }
}
