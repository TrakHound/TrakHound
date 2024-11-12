// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;

namespace TrakHound
{
    public class UnixTimeSegment
    {
        public long From { get; set; }

        public long To { get; set; }


        public UnixTimeSegment(long from, long to)
        {
            From = from;
            To = to;
        }


        public static List<UnixTimeSegment> GetSegments(long from, long to, int increment)
        {
            var objs = new List<UnixTimeSegment>();

            var bottom = GetSegmentBottom(from, increment);
            var top = GetSegmentTop(to, increment);
            if (GetSegmentBottom(to, increment) == to) top = top - (increment * 1000);
            var ts = from;

            while (ts < top)
            {
                var b = GetSegmentBottom(ts, increment);
                var t = GetSegmentTop(ts, increment);

                if (b < top)
                {
                    var seg = new UnixTimeSegment(b, t);
                    objs.Add(seg);
                }

                ts = t;
            }

            return objs;
        }

        public static long GetSegmentBottom(long uts, int increment)
        {
            var ts = uts.ToDateTime();

            var y = ts.Year;
            var m = ts.Month;
            var d = ts.Day;
            var h = ts.Hour;
            var mm = ts.Minute;
            var s = ts.Second;
            var ms = ts.Millisecond;

            var b = new DateTime(y, m, d, h, 0, 0);
            var seg = b.AddSeconds(increment);
            while (ts >= seg)
            {
                seg = seg.AddSeconds(increment);
            }

            seg = seg.AddSeconds(increment * -1);

            return seg.ToUnixTime();
        }

        public static long GetSegmentTop(long uts, int increment)
        {
            var ts = uts.ToDateTime();

            var y = ts.Year;
            var m = ts.Month;
            var d = ts.Day;
            var h = ts.Hour;
            var mm = ts.Minute;
            var s = ts.Second;
            var ms = ts.Millisecond;

            var b = new DateTime(y, m, d, h, 0, 0);
            b = b.AddHours(1);

            var seg = b.AddSeconds(increment * -1);
            while (ts < seg)
            {
                seg = seg.AddSeconds(increment * -1);
            }

            seg = seg.AddSeconds(increment);

            return seg.ToUnixTime();
        }
    }
}
