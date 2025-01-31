namespace TrakHound.Tables
{
    public struct TrakHoundTableQuery
    {
        public string Column { get; set; }

        public TrakHoundConditionOperator Operator { get; set; }

        public object Value { get; set; }


        public TrakHoundTableQuery() { }

        public TrakHoundTableQuery(string column, TrakHoundConditionOperator conditionOperator, object value) 
        {
            Column = column;
            Operator = conditionOperator;
            Value = value;
        }
    }
}
