// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrakHound.Entities.QueryEngines
{
    public abstract class QueryExecutionPlanStep : IQueryExecutionPlanStep
    {
        public int Index { get; set; }

        public string Id { get; set; }

        public virtual string Description { get; set; }
    }
}
