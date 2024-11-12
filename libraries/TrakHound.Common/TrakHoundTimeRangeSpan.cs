// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;

namespace TrakHound
{
    public struct TrakHoundTimeRangeSpan
    {
        public string Target { get; set; }

        public long Span { get; set; }


        public TrakHoundTimeRangeSpan(string target, TimeSpan span)
        {
            Target = target;
            Span = (long)span.TotalNanoseconds;
        }

        public TrakHoundTimeRangeSpan(string target, long spanTicks) 
        { 
            Target = target;
            Span = spanTicks;
        }

        public static long Get(TimeSpan span)
        {
            return (long)span.TotalNanoseconds;
        }
    }
}
