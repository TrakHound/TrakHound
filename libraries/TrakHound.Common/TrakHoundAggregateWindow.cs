// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound
{
    public class TrakHoundAggregateWindow
    {
        public string Target { get; set; }

        public long Start { get; set; }

        public long End { get; set; }

        public double Value { get; set; }


        public TrakHoundAggregateWindow(string target, long start, long end, double value)
        {
            Target = target;
            Start = start;
            End = end;
            Value = value;
        }
    }
}
