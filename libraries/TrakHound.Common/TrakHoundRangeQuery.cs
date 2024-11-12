// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound
{
    public struct TrakHoundRangeQuery
    {
        public string Target { get; set; }

        public long From { get; set; }

        public long To { get; set; }


        public TrakHoundRangeQuery(string target, long from, long to)
        {
            Target = target;
            From = from;
            To = to;
        }


        public static string GetRangeId(TrakHoundRangeQuery query)
        {
            return $"{query.From}:{query.To}";
        }
    }
}
