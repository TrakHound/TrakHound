// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Entities.QueryEngines
{
    public class QuerySelect
    {
        public int Index { get; set; }

        public string Expression { get; set; }

        public string Property { get; set; }

        public string Alias { get; set; }

        public QuerySelectType Type { get; set; }


        public QuerySelect(int index, QuerySelectType type, string expression, string property = null, string alias = null)
        {
            Index = index;
            Expression = expression;
            Property = property;
            Alias = alias;
            Type = type;
        }
    }
}
