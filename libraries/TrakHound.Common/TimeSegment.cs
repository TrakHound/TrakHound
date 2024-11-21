// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;

namespace TrakHound
{
    public struct TimeSegment
    {
        public DateTime From { get; set; }

        public DateTime To { get; set; }


        public TimeSegment(DateTime from, DateTime to)
        {
            From = from;
            To = to;
        }


        public static List<TimeSegment> GetSegments(DateTime from, DateTime to, int increment)
        {
            var objs = new List<TimeSegment>();

            var bottom = GetSegmentBottom(from, increment);
            var top = GetSegmentTop(to, increment);
            if (GetSegmentBottom(to, increment) == to) top = top.AddSeconds(increment * -1);
            var ts = from;

            while (ts < top)
            {
                var b = GetSegmentBottom(ts, increment);
                var t = GetSegmentTop(ts, increment);

                if (b < top)
                {
                    var seg = new TimeSegment(b, t);
                    objs.Add(seg);
                }

                ts = t;
            }

            return objs;
        }

        public static DateTime GetSegmentBottom(DateTime ts, int increment)
        {
            var y = ts.Year;
            var m = ts.Month;
            var d = ts.Day;
            var h = ts.Hour;
            var mm = ts.Minute;
            var s = ts.Second;
            var ms = ts.Millisecond;

            var b = new DateTime(y, m, d, h, 0, 0, ts.Kind);
            var seg = b.AddSeconds(increment);
            while (ts >= seg)
            {
                seg = seg.AddSeconds(increment);
            }

            seg = seg.AddSeconds(increment * -1);

            return seg;
        }

        public static DateTime GetSegmentTop(DateTime ts, int increment)
        {
            var y = ts.Year;
            var m = ts.Month;
            var d = ts.Day;
            var h = ts.Hour;
            var mm = ts.Minute;
            var s = ts.Second;
            var ms = ts.Millisecond;

            var b = new DateTime(y, m, d, h, 0, 0, ts.Kind);
            b = b.AddHours(1);

            var seg = b.AddSeconds(increment * -1);
            while (ts < seg)
            {
                seg = seg.AddSeconds(increment * -1);
            }

            seg = seg.AddSeconds(increment);

            return seg;
        }
    }
}
