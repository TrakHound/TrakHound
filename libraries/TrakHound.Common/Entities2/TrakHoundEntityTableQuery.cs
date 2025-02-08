// Copyright (c) 2025 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Entities
{
    public struct TrakHoundEntityTableQuery
    {
        public string Column { get; set; }

        public TrakHoundConditionOperator Operator { get; set; }

        public object Value { get; set; }


        public TrakHoundEntityTableQuery() { }

        public TrakHoundEntityTableQuery(string column, TrakHoundConditionOperator conditionOperator, object value) 
        {
            Column = column;
            Operator = conditionOperator;
            Value = value;
        }
    }
}
