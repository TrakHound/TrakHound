// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;

namespace TrakHound
{
    public static class UnixDateTime
    {
        public static long Now
        {
            get
            {
                return DateTime.UtcNow.ToUnixTime();
            }
        }

        public static TimeSpan Ago(long ticks)
        {
            return TimeSpan.FromTicks(Now - ticks);
        }
    }


    public static class UnixTimeExtensions
    {
        public static readonly DateTime EpochTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private static readonly long _maxTicks = DateTime.MaxValue.Ticks;


        public static long ToUnixTime(this DateTime d)
        {
            var x = d;
            if (d.Kind == DateTimeKind.Local) x = d.ToUniversalTime();
            var duration = x - EpochTime;
            return duration.Ticks * 100; // Nanoseconds
        }


        public static DateTime ToDateTime(this long unixMilliseconds)
        {
            return FromUnixTime(unixMilliseconds);
        }

        public static DateTime ToLocalDateTime(this long unixMilliseconds)
        {
            return FromUnixTime(unixMilliseconds).ToLocalTime();
        }

        //public static DateTime FromUnixTime(long unixNanoseconds)
        //{
        //    try
        //    {
        //        if (unixNanoseconds <= _maxTicks)
        //        {
        //            return EpochTime.AddTicks(unixNanoseconds / 100);
        //        }
        //        else
        //        {
        //            return DateTime.MaxValue;
        //        }
        //    }
        //    catch { }

        //    return DateTime.MinValue;
        //}

        public static DateTime FromUnixTime(long unixNanoseconds)
        {
            try
            {
                if (unixNanoseconds <= _maxTicks)
                {
                    return EpochTime.AddTicks(unixNanoseconds / 100);
                }
                else
                {
                    return DateTime.MaxValue;
                }
            }
            catch { }

            return DateTime.MinValue;
        }

        //public static DateTime FromUnixTime(long unixTicks)
        //{
        //    try
        //    {
        //        if (unixTicks <= _maxTicks)
        //        {
        //            return EpochTime.AddTicks(unixTicks);
        //        }
        //        else
        //        {
        //            return DateTime.MaxValue;
        //        }
        //    }
        //    catch { }

        //    return DateTime.MinValue;
        //}

        public static DateTime FromUnixTimeSeconds(long unixSeconds)
        {
            try
            {
                return EpochTime.AddSeconds(unixSeconds);
            }
            catch { }

            return DateTime.MinValue;
        }

        public static DateTime FromUnixTimeMilliseconds(long unixMiliseconds)
        {
            try
            {
                return EpochTime.AddMilliseconds(unixMiliseconds);
            }
            catch { }

            return DateTime.MinValue;
        }
    }
}
