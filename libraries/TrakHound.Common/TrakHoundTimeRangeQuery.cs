// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound
{
    public struct TrakHoundTimeRangeQuery
    {
        public string Target { get; set; }

        public long From { get; set; }

        public long To { get; set; }

        public long Span { get; set; }


        public TrakHoundTimeRangeQuery(string target, long from, long to, long span)
        {
            Target = target;
            From = from;
            To = to;
            Span = span;
        }


        public static string GetRangeId(TrakHoundTimeRangeQuery query)
        {
            return $"{query.From}:{query.To}:{query.Span}";
        }
    }
}
