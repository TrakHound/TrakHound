// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;

namespace TrakHound
{
    public struct Time
    {
        public int Hour { get; set; }

        public int Minute { get; set; }

        public int Second { get; set; }


        public Time(int hour, int minute, int second)
        {
            Hour = hour;
            Minute = minute;
            Second = second;
        }

        public Time(DateTime ts)
        {
            Hour = ts.Hour;
            Minute = ts.Minute;
            Second = ts.Second;
        }

        public Time(string s)
        {
            Hour = 0;
            Minute = 0;
            Second = 0;

            if (!string.IsNullOrEmpty(s))
            {
                if (DateTime.TryParse(s, out DateTime ts))
                {
                    Hour = ts.Hour;
                    Minute = ts.Minute;
                    Second = ts.Second;
                }
            }
        }


        public override string ToString()
        {
            var m = DateTime.MinValue;
            var d = new DateTime(m.Year, m.Month, m.Day, Hour, Minute, Second);

            return d.ToString("t");
        }
    }
}
