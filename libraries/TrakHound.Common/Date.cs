// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Globalization;

namespace TrakHound
{
    public struct Date
    {
        public int Year { get; }

        public int Month { get; }

        public int Day { get; }

        public int Week { get; }

        public DayOfWeek DayOfWeek { get; }


        public Date(int year, int month, int day)
        {
            Year = year;
            Month = month;
            Day = day;
            Week = 0;
            DayOfWeek = 0;

            if (year > 0 && month > 0 && day > 0)
            {
                var ts = new DateTime(year, month, day);
                Calendar cal = new CultureInfo("en-US").Calendar;
                Week = cal.GetWeekOfYear(ts, CalendarWeekRule.FirstDay, DayOfWeek.Monday);

                DayOfWeek = ts.DayOfWeek;
            }
        }

        public Date(DateTime ts)
        {
            Year = ts.Year;
            Month = ts.Month;
            Day = ts.Day;

            Calendar cal = new CultureInfo("en-US").Calendar;
            Week = cal.GetWeekOfYear(ts, CalendarWeekRule.FirstDay, DayOfWeek.Monday);

            DayOfWeek = ts.DayOfWeek;
        }

        public Date(string s)
        {
            Year = 1;
            Month = 1;
            Day = 1;
            Week = 0;
            DayOfWeek = 0;

            if (!string.IsNullOrEmpty(s))
            {
                if (DateTime.TryParse(s, out DateTime ts))
                {
                    Year = ts.Year;
                    Month = ts.Month;
                    Day = ts.Day;

                    Calendar cal = new CultureInfo("en-US").Calendar;
                    Week = cal.GetWeekOfYear(ts, CalendarWeekRule.FirstDay, DayOfWeek.Monday);

                    DayOfWeek = ts.DayOfWeek;
                }
            }
        }

        public override string ToString()
        {
            return $"{Year}-{Month}-{Day}";
        }
    }

    public static class DateExtensions
    {
        public static Date ToDate(this DateTime dateTime)
        {
            return new Date(dateTime.Year, dateTime.Month, dateTime.Day);
        }
    }
}
