// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Globalization;

namespace TrakHound
{
    public static class DateTimeExtensions
    {
        public static string ToISO8601String(this DateTime dateTime)
        {
            return dateTime.ToString("o");
        }

        public static string ToFormattedString(this TimeSpan ts, bool showMilliseconds = false)
        {
            if (ts.Days > 1) return string.Format("{0:%d} Days {0:hh\\:mm\\:ss}", ts);
            else if (ts.Days > 0) return string.Format("{0:%d} Day {0:hh\\:mm\\:ss}", ts);
            else if (showMilliseconds && ts > TimeSpan.Zero && ts.TotalSeconds < 1) return $"{ts.TotalMilliseconds}ms";
            else return ts.ToString(@"hh\:mm\:ss");
        }

        public static string ToSimpleFormattedString(this TimeSpan ts)
        {
            if (ts.Days > 0) return string.Format("{0:%d}d", ts);
            else if (ts.TotalHours >= 1) return string.Format("{0:%h}h", ts);
            else if (ts.TotalMinutes >= 1) return string.Format("{0:%m}m", ts);
            else if (ts.TotalMinutes < 1) return string.Format("{0:%s}s", ts);
            //else if (ts.TotalMinutes < 1) return string.Format("<1m", ts);
            //else if (ts.TotalSeconds < 10) return "<30s";
            else return "Just Now";
        }

        public static string ToDetailedFormattedString(this TimeSpan ts)
        {
            if (ts.Days > 0) return string.Format("{0:%d}d", ts);
            else if (ts.TotalHours >= 1) return string.Format("{0:%h}h", ts);
            else if (ts.TotalMinutes >= 1) return string.Format("{0:%m}m", ts);
            else if (ts.TotalSeconds >= 10) return string.Format("{0:%s}s", ts);
            else if (ts.TotalSeconds >= 1) return string.Format("{0:%s\\.ff}s", ts);
            else if (ts.TotalMilliseconds > 1) return $"{Math.Round(ts.TotalMilliseconds, 0)}ms";
            else return $"{Math.Round((double)ts.Ticks / 10000, 3)}ms";
        }

        public static int GetWeekOfYear(this DateTime dateTime)
        {
            var ci = new CultureInfo("en-US");
            return ci.Calendar.GetWeekOfYear(dateTime, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
        }
    }
}
