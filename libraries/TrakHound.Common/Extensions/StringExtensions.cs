// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using TrakHound.Extensions;

namespace TrakHound
{
    public static class StringFunctions
    {
        private static readonly Encoding _utf8 = Encoding.UTF8;
        private static readonly NameGenerator _nameGenerator = new NameGenerator();

        [ThreadStatic]
        private static MD5 _md5;

        [ThreadStatic]
        private static SHA1 _sha1;

        [ThreadStatic]
        private static SHA256 _sha256;

        [ThreadStatic]
        private static Random _random;

        private static MD5 MD5Algorithm
        {
            get
            {
                if (_md5 == null)
                {
                    _md5 = MD5.Create();
                }
                return _md5;
            }
        }

        private static SHA1 SHA1Algorithm
        {
            get
            {
                if (_sha1 == null)
                {
                    _sha1 = SHA1.Create();
                }
                return _sha1;
            }
        }

        private static SHA256 SHA256Algorithm
        {
            get
            {
                if (_sha256 == null)
                {
                    _sha256 = SHA256.Create();
                }
                return _sha256;
            }
        }

        private static Random Random
        {
            get
            {
                if (_random == null)
                {
                    _random = new Random();
                }

                return _random;
            }
        }


        public static int ToInt(this string s)
        {
            if (!string.IsNullOrEmpty(s) && int.TryParse(s, out var x)) return x;
            else return -1;
        }

        public static long ToLong(this string s)
        {
            if (!string.IsNullOrEmpty(s) && long.TryParse(s, out var x)) return x;
            else return -1;
        }

        public static double ToDouble(this string s)
        {
            if (!string.IsNullOrEmpty(s) && double.TryParse(s, out var x)) return x;
            else return -1;
        }

        public static double ToDouble(this string s, int decimalPlaces = int.MaxValue)
        {
            if (!string.IsNullOrEmpty(s) && double.TryParse(s, out var x))
            {
                return Math.Round(x, decimalPlaces);
            }
            else return -1;
        }

        public static string GetUtf8String(byte[] bytes)
        {
            if (!bytes.IsNullOrEmpty())
            {
                try
                {
                    return System.Text.Encoding.UTF8.GetString(bytes);
                }
                catch { }
            }

            return null;
        }

        public static string GetUtf8String(ReadOnlySpan<byte> bytes)
        {
            if (!bytes.IsEmpty)
            {
                try
                {
                    return System.Text.Encoding.UTF8.GetString(bytes);
                }
                catch { }
            }

            return null;
        }

        public static byte[] ToUtf8Bytes(this string s)
        {
            if (!string.IsNullOrEmpty(s))
            {
                try
                {
                    return System.Text.Encoding.UTF8.GetBytes(s);
                }
                catch { }
            }

            return null;
        }

        public static int ToUtf8Bytes(this string s, byte[] buffer)
        {
            if (!string.IsNullOrEmpty(s))
            {
                try
                {
                    return System.Text.Encoding.UTF8.GetBytes(s, buffer);
                }
                catch { }
            }

            return 0;
        }

        public static DateTime ToDateTime(this string s)
        {
            if (!string.IsNullOrEmpty(s))
            {
                // Detect if Unix Timestamp
                if (s.IsNumeric())
                {
                    long.TryParse(s, out var n);
                    if (n > 9999999999999) // Nanoseconds
                    {
                        return UnixTimeExtensions.FromUnixTime(n);
                    }
                    else if (n > 9999999999) // Milliseconds
                    {
                        return UnixTimeExtensions.FromUnixTimeMilliseconds(n);
                    }
                    else
                    {
                        return UnixTimeExtensions.FromUnixTimeSeconds(n);
                    }
                }
                else
                {
                    if (DateTime.TryParse(s, null, DateTimeStyles.AssumeLocal, out var x))
                    {
                        return x;
                    }
                    else
                    {
                        var now = DateTime.UtcNow;
                        var today = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0, DateTimeKind.Local);
                        var week = GetStartOfWeek(today, DayOfWeek.Sunday); // Monday?
                        var month = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Local);
                        var year = new DateTime(now.Year, 1, 1, 0, 0, 0, DateTimeKind.Local);

                        if (s.ToLower() == "now") return now;
                        else if (s.ToLower() == "today") return today;
                        else if (s.ToLower() == "noon") return today.AddHours(12);

                        else if (s.ToLower() == "yesterday") return today.AddDays(-1);
                        else if (s.ToLower() == "tomorrow") return today.AddDays(1);

                        else if (s.ToLower() == "sunday") return week;
                        else if (s.ToLower() == "monday") return week.AddDays(1);
                        else if (s.ToLower() == "tuesday") return week.AddDays(2);
                        else if (s.ToLower() == "wednesday") return week.AddDays(3);
                        else if (s.ToLower() == "thursday") return week.AddDays(4);
                        else if (s.ToLower() == "friday") return week.AddDays(5);
                        else if (s.ToLower() == "saturday") return week.AddDays(6);

                        else if (s.ToLower() == "next-sunday") return week.AddDays(7);
                        else if (s.ToLower() == "next-monday") return week.AddDays(7).AddDays(1);
                        else if (s.ToLower() == "next-tuesday") return week.AddDays(7).AddDays(2);
                        else if (s.ToLower() == "next-wednesday") return week.AddDays(7).AddDays(3);
                        else if (s.ToLower() == "next-thursday") return week.AddDays(7).AddDays(4);
                        else if (s.ToLower() == "next-friday") return week.AddDays(7).AddDays(5);
                        else if (s.ToLower() == "next-saturday") return week.AddDays(7).AddDays(6);

                        else if (s.ToLower() == "last-sunday") return week.AddDays(-7);
                        else if (s.ToLower() == "last-monday") return week.AddDays(-7).AddDays(1);
                        else if (s.ToLower() == "last-tuesday") return week.AddDays(-7).AddDays(2);
                        else if (s.ToLower() == "last-wednesday") return week.AddDays(-7).AddDays(3);
                        else if (s.ToLower() == "last-thursday") return week.AddDays(-7).AddDays(4);
                        else if (s.ToLower() == "last-friday") return week.AddDays(-7).AddDays(5);
                        else if (s.ToLower() == "last-saturday") return week.AddDays(-7).AddDays(6);

                        else if (s.ToLower() == "this-week") return week;
                        else if (s.ToLower() == "this-month") return month;
                        else if (s.ToLower() == "this-year") return year;
                        else if (s.ToLower() == "next-week") return week.AddDays(7);
                        else if (s.ToLower() == "next-month") return month.AddMonths(1);
                        else if (s.ToLower() == "next-year") return year.AddYears(1);
                        else if (s.ToLower() == "last-week") return week.AddDays(-7);
                        else if (s.ToLower() == "last-month") return month.AddMonths(-1);
                        else if (s.ToLower() == "last-year") return year.AddYears(-1);

                        else
                        {
                            var match = Regex.Match(s, @"(.+)-ago");
                            if (match.Success)
                            {
                                for (var i = 1; i < match.Groups.Count; i++)
                                {
                                    if (match.Groups[i].Success)
                                    {
                                        var value = match.Groups[i].Value;
                                        var duration = ToTimeSpan(value).Negate();
                                        return now.Add(duration);
                                    }
                                }
                            }
                        }
                    }           
                }
            }
            
            return DateTime.MinValue;
        }

        public static DateTime? ParseDateTime(this string s)
        {
            if (!string.IsNullOrEmpty(s))
            {
                // Detect if Unix Timestamp
                if (s.IsNumeric())
                {
                    long.TryParse(s, out var n);
                    if (n > 9999999999999) // Nanoseconds
                    {
                        return UnixTimeExtensions.FromUnixTime(n);
                    }
                    else if (n > 9999999999) // Milliseconds
                    {
                        return UnixTimeExtensions.FromUnixTimeMilliseconds(n);
                    }
                    else
                    {
                        return UnixTimeExtensions.FromUnixTimeSeconds(n);
                    }
                }
                else
                {
                    if (DateTime.TryParse(s, null, DateTimeStyles.AssumeLocal, out var x))
                    {
                        return x;
                    }
                    else
                    {
                        var now = DateTime.UtcNow;
                        var today = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0, DateTimeKind.Local);
                        var week = GetStartOfWeek(today, DayOfWeek.Sunday); // Monday?
                        var month = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Local);
                        var year = new DateTime(now.Year, 1, 1, 0, 0, 0, DateTimeKind.Local);

                        if (s.ToLower() == "now") return now;
                        else if (s.ToLower() == "today") return today;
                        else if (s.ToLower() == "noon") return today.AddHours(12);

                        else if (s.ToLower() == "yesterday") return today.AddDays(-1);
                        else if (s.ToLower() == "tomorrow") return today.AddDays(1);

                        else if (s.ToLower() == "sunday") return week;
                        else if (s.ToLower() == "monday") return week.AddDays(1);
                        else if (s.ToLower() == "tuesday") return week.AddDays(2);
                        else if (s.ToLower() == "wednesday") return week.AddDays(3);
                        else if (s.ToLower() == "thursday") return week.AddDays(4);
                        else if (s.ToLower() == "friday") return week.AddDays(5);
                        else if (s.ToLower() == "saturday") return week.AddDays(6);

                        else if (s.ToLower() == "next-sunday") return week.AddDays(7);
                        else if (s.ToLower() == "next-monday") return week.AddDays(7).AddDays(1);
                        else if (s.ToLower() == "next-tuesday") return week.AddDays(7).AddDays(2);
                        else if (s.ToLower() == "next-wednesday") return week.AddDays(7).AddDays(3);
                        else if (s.ToLower() == "next-thursday") return week.AddDays(7).AddDays(4);
                        else if (s.ToLower() == "next-friday") return week.AddDays(7).AddDays(5);
                        else if (s.ToLower() == "next-saturday") return week.AddDays(7).AddDays(6);

                        else if (s.ToLower() == "last-sunday") return week.AddDays(-7);
                        else if (s.ToLower() == "last-monday") return week.AddDays(-7).AddDays(1);
                        else if (s.ToLower() == "last-tuesday") return week.AddDays(-7).AddDays(2);
                        else if (s.ToLower() == "last-wednesday") return week.AddDays(-7).AddDays(3);
                        else if (s.ToLower() == "last-thursday") return week.AddDays(-7).AddDays(4);
                        else if (s.ToLower() == "last-friday") return week.AddDays(-7).AddDays(5);
                        else if (s.ToLower() == "last-saturday") return week.AddDays(-7).AddDays(6);

                        else if (s.ToLower() == "this-week") return week;
                        else if (s.ToLower() == "this-month") return month;
                        else if (s.ToLower() == "this-year") return year;
                        else if (s.ToLower() == "next-week") return week.AddDays(7);
                        else if (s.ToLower() == "next-month") return month.AddMonths(1);
                        else if (s.ToLower() == "next-year") return year.AddYears(1);
                        else if (s.ToLower() == "last-week") return week.AddDays(-7);
                        else if (s.ToLower() == "last-month") return month.AddMonths(-1);
                        else if (s.ToLower() == "last-year") return year.AddYears(-1);

                        else
                        {
                            var match = Regex.Match(s, @"(.+)-ago");
                            if (match.Success)
                            {
                                for (var i = 1; i < match.Groups.Count; i++)
                                {
                                    if (match.Groups[i].Success)
                                    {
                                        var value = match.Groups[i].Value;
                                        var duration = ToTimeSpan(value).Negate();
                                        return now.Add(duration);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return null;
        }

        public static DateTime GetStartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }


        public static TimeSpan ToTimeSpan(this string s)
        {
            if (!string.IsNullOrEmpty(s))
            {
                // Detect if Nanoseconds
                if (s.IsNumeric())
                {
                    if (long.TryParse(s, out var n))
                    {
                        return TimeSpan.FromTicks(n / 100);
                    }
                }
                else
                {
                    if (TimeSpan.TryParse(s, out var x))
                    {
                        return x;
                    }
                    else
                    {
                        var ts = TimeSpan.Zero;

                        // Year
                        if (s.ToLower() == "year") ts = TimeSpan.FromDays(365);

                        // Years
                        var match = Regex.Match(s, @"([0-9]+) [Yy]ears?");
                        if (match.Success) ts = ts.Add(TimeSpan.FromDays(365 * GetTimeSpanDoubleValue(match.Groups[1].Value)));

                        // Years
                        match = Regex.Match(s, @"([0-9]+)y\b");
                        if (match.Success) ts = ts.Add(TimeSpan.FromDays(365 * GetTimeSpanDoubleValue(match.Groups[1].Value)));

                        // Month
                        if (s.ToLower() == "month") ts = TimeSpan.FromDays(30);

                        // Months
                        match = Regex.Match(s, @"([0-9]+) [Mm]onths?");
                        if (match.Success) ts = ts.Add(TimeSpan.FromDays(30 * GetTimeSpanDoubleValue(match.Groups[1].Value)));

                        // Weeks
                        match = Regex.Match(s, @"([0-9]+) [W]eeks?");
                        if (match.Success) ts = ts.Add(TimeSpan.FromDays(7 * GetTimeSpanDoubleValue(match.Groups[1].Value)));

                        // Day
                        if (s.ToLower() == "day") ts = TimeSpan.FromDays(1);

                        // Days
                        match = Regex.Match(s, @"([0-9]+) [Dd]ays?");
                        if (match.Success) ts = ts.Add(TimeSpan.FromDays(GetTimeSpanDoubleValue(match.Groups[1].Value)));

                        // Days
                        match = Regex.Match(s, @"([0-9]+)d\b");
                        if (match.Success) ts = ts.Add(TimeSpan.FromDays(GetTimeSpanDoubleValue(match.Groups[1].Value)));

                        // Hour
                        if (s.ToLower() == "hour") ts = TimeSpan.FromHours(1);

                        // Hours
                        match = Regex.Match(s, @"([0-9]+) [Hh]ours?");
                        if (match.Success) ts = ts.Add(TimeSpan.FromHours(GetTimeSpanDoubleValue(match.Groups[1].Value)));

                        // Hours
                        match = Regex.Match(s, @"([0-9]+)h\b");
                        if (match.Success) ts = ts.Add(TimeSpan.FromHours(GetTimeSpanDoubleValue(match.Groups[1].Value)));

                        // Minute
                        if (s.ToLower() == "minute") ts = TimeSpan.FromMinutes(1);

                        // Minutes
                        match = Regex.Match(s, @"([0-9]+) [Mm]inutes?");
                        if (match.Success) ts = ts.Add(TimeSpan.FromMinutes(GetTimeSpanDoubleValue(match.Groups[1].Value)));

                        // Minutes
                        match = Regex.Match(s, @"([0-9]+)m\b");
                        if (match.Success) ts = ts.Add(TimeSpan.FromMinutes(GetTimeSpanDoubleValue(match.Groups[1].Value)));

                        // Second
                        if (s.ToLower() == "second") ts = TimeSpan.FromSeconds(1);

                        // Seconds
                        match = Regex.Match(s, @"([0-9]+) [Ss]econds?");
                        if (match.Success) ts = ts.Add(TimeSpan.FromSeconds(GetTimeSpanDoubleValue(match.Groups[1].Value)));

                        // Seconds
                        match = Regex.Match(s, @"([0-9]+)s\b");
                        if (match.Success) ts = ts.Add(TimeSpan.FromSeconds(GetTimeSpanDoubleValue(match.Groups[1].Value)));

                        // Milliseconds
                        match = Regex.Match(s, @"([0-9]+) [Mm]illiseconds?");
                        if (match.Success) ts = ts.Add(TimeSpan.FromMilliseconds(GetTimeSpanDoubleValue(match.Groups[1].Value)));

                        // Milliseconds
                        match = Regex.Match(s, @"([0-9]+)ms\b");
                        if (match.Success) ts = ts.Add(TimeSpan.FromMilliseconds(GetTimeSpanDoubleValue(match.Groups[1].Value)));

                        // Microseconds
                        match = Regex.Match(s, @"([0-9]+) [Mm]icroseconds?");
                        if (match.Success) ts = ts.Add(TimeSpan.FromTicks(10 * GetTimeSpanLongValue(match.Groups[1].Value)));

                        // Microseconds
                        match = Regex.Match(s, @"([0-9]+)µs\b");
                        if (match.Success) ts = ts.Add(TimeSpan.FromTicks(10 * GetTimeSpanLongValue(match.Groups[1].Value)));

                        // Ticks
                        match = Regex.Match(s, @"([0-9]+) [Tt]icks?");
                        if (match.Success) ts = ts.Add(TimeSpan.FromTicks(GetTimeSpanLongValue(match.Groups[1].Value)));

                        return ts;
                    }
                }
            }
            
            return TimeSpan.Zero;
        }

        private static double GetTimeSpanDoubleValue(string s)
        {
            if (s != null)
            {
                if (double.TryParse(s, out var value))
                {
                    return value;
                }
            }

            return 0;
        }

        private static long GetTimeSpanLongValue(string s)
        {
            if (s != null)
            {
                if (long.TryParse(s, out var value))
                {
                    return value;
                }
            }

            return 0;
        }


        public static Version ToVersion(this string s)
        {
            if (!string.IsNullOrEmpty(s) && Version.TryParse(s, out var x)) return x;
            else return null;
        }

        public static string ToPascalCase(this string s)
        {
            if (!string.IsNullOrEmpty(s))
            {
                var parts = s.SplitOnWord();
                if (!parts.IsNullOrEmpty())
                {
                    var sb = new StringBuilder();
                    for (var i = 0; i <= parts.Count() - 1; i++)
                    {
                        sb.Append(parts[i].UppercaseFirstCharacter());
                    }
                    return sb.ToString();
                }
            }

            return null;
        }

        public static string ToTitleCase(this string s)
        {
            if (!string.IsNullOrEmpty(s))
            {
                var parts = s.SplitOnWord();
                if (!parts.IsNullOrEmpty())
                {
                    var sb = new StringBuilder();
                    for (var i = 0; i <= parts.Count() - 1; i++)
                    {
                        sb.Append(parts[i].UppercaseFirstCharacter());
                    }
                    return sb.ToString();
                }
            }

            return null;
        }

        public static string ToCamelCase(this string s)
        {
            if (!string.IsNullOrEmpty(s))
            {
                var parts = s.SplitOnWord();
                if (!parts.IsNullOrEmpty())
                {
                    var sb = new StringBuilder();
                    for (var i = 0; i <= parts.Count() - 1; i++)
                    {
                        if (i > 0) sb.Append(parts[i].UppercaseFirstCharacter());
                        else sb.Append(parts[i].ToLower());
                    }
                    return sb.ToString();
                }
            }

            return null;
        }

        public static string ToSnakeCase(this string s, bool splitOnUppercase = true)
        {
            if (!string.IsNullOrEmpty(s))
            {
                if (s != s.ToUpper() && !s.Contains('_') && !s.Contains('-') && !s.Contains('.'))
                {
                    var parts = s.SplitOnWord();

                    var a = new List<string>();
                    if (!parts.IsNullOrEmpty()) foreach (var part in parts) a.Add(part.Trim());
                    if (!a.IsNullOrEmpty())
                    {
                        string x = string.Join("_", a);
                        return x.ToLower();
                    }
                }
                else return s.ToLower();
            }

            return null;
        }

        public static string ToKebabCase(this string s, bool splitOnUppercase = true)
        {
            if (!string.IsNullOrEmpty(s))
            {
                if (s != s.ToUpper() && !s.Contains('_') && !s.Contains('-') && !s.Contains('.'))
                {
                    var parts = s.SplitOnWord();

                    var a = new List<string>();
                    if (!parts.IsNullOrEmpty()) foreach (var part in parts) a.Add(part.Trim());
                    if (!a.IsNullOrEmpty())
                    {
                        string x = string.Join("-", a);
                        return x.ToLower();
                    }
                }
                else return s.ToLower();
            }

            return null;
        }

        public static string[] SplitOnWord(this string s)
        {
            if (!string.IsNullOrEmpty(s))
            {
                string[] parts;

                if (s.Contains(' '))
                {
                    // Split string by empty space
                    parts = s.Split(' ');
                }
                else if (s.Contains('_'))
                {
                    // Split string by underscore
                    parts = s.Split('_');
                }
                else if (s.Contains('-'))
                {
                    // Split string by dash
                    parts = s.Split('-');
                }
                else
                {
                    // Split string by Uppercase characters
                    parts = SplitOnUppercase(s);
                }

                return parts;
            }

            return null;
        }

        public static string[] SplitOnUppercase(this string s)
        {
            if (!string.IsNullOrEmpty(s))
            {
                if (s != s.ToUpper())
                {
                    var p = "";
                    var x = 0;
                    for (var i = 0; i < s.Length; i++)
                    {
                        if (i > 0 && char.IsUpper(s[i]))
                        {
                            p += s.Substring(x, i - x) + " ";
                            x = i;
                        }

                        if (i == s.Length - 1)
                        {
                            p += s.Substring(x);
                        }
                    }
                    return p.Split(' ');
                }
                else return new string[] { s };
            }

            return null;
        }


        public static string UppercaseFirstCharacter(this string s)
        {
            if (s == null) return null;

            if (s.Length > 1)
            {
                var sb = new StringBuilder(s.Length);
                for (var i = 0; i <= s.Length - 1; i++)
                {
                    if (i == 0) sb.Append(char.ToUpper(s[i]));
                    else sb.Append(char.ToLower(s[i]));
                }
                return sb.ToString();
            }

            return s.ToUpper();
        }

        public static string LowercaseFirstCharacter(this string s)
        {
            if (s == null) return null;

            if (s.Length > 1)
            {
                var sb = new StringBuilder(s.Length);
                for (var i = 0; i <= s.Length - 1; i++)
                {
                    if (i == 0) sb.Append(char.ToLower(s[i]));
                    else sb.Append(s[i]);
                }
                return sb.ToString();
            }

            return s.ToUpper();
        }

        public static string ToUnderscoreUpper(this string s, bool splitOnUppercase = true)
        {
            if (!string.IsNullOrEmpty(s))
            {
                if (s != s.ToUpper())
                {
                    var parts = s.SplitOnWord();

                    var a = new List<string>();
                    if (!parts.IsNullOrEmpty()) foreach (var part in parts) a.Add(part.Trim());
                    if (!a.IsNullOrEmpty())
                    {
                        string x = string.Join("_", a);
                        return x.ToUpper();
                    }
                }
                else return s.ToUpper();
            }

            return null;
        }


        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[Random.Next(s.Length)]).ToArray());
        }

        public static string GetRandomName()
        {
            return _nameGenerator.GetRandomName();
        }


        public static ulong GetUInt64Hash(this string text, HashAlgorithm hasher)
        {
            using (hasher)
            {
                var bytes = hasher.ComputeHash(Encoding.Default.GetBytes(text));
                Array.Resize(ref bytes, bytes.Length + bytes.Length % 8); //make multiple of 8 if hash is not, for exampel SHA1 creates 20 bytes. 
                return Enumerable.Range(0, bytes.Length / 8) // create a counter for de number of 8 bytes in the bytearray
                    .Select(i => BitConverter.ToUInt64(bytes, i * 8)) // combine 8 bytes at a time into a integer
                    .Aggregate((x, y) => x ^ y); //xor the bytes together so you end up with a ulong (64-bit int)
            }
        }

        public static string ToFileSize(this long byteCount)
        {
            string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" }; //Longs run out around EB
            if (byteCount == 0)
                return "0" + suf[0];
            long bytes = Math.Abs(byteCount);
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return (Math.Sign(byteCount) * num).ToString() + suf[place];
        }

        public static string ToFileSize(this decimal byteCount)
        {
            var x = (long)byteCount;
            return x.ToFileSize();
        }


        public static bool IsHtml(this string s)
        {
            if (!string.IsNullOrEmpty(s))
            {
                var regex = new Regex(@"<\s*([^ >]+)[^>]*>.*?<\s*/\s*\1\s*>");
                return regex.IsMatch(s);
            }

            return false;
        }

        public static T ConvertEnum<T>(this object value)
        {
            if (value != null)
            {
                if (Enum.TryParse(typeof(T), value.ToString(), true, out var result))
                {
                    return (T)result;
                }
            }

            return default;
        }

        public static T ConvertEnum<T>(this string value)
        {
            if (value != null)
            {
                if (Enum.TryParse(typeof(T), value.ToString(), true, out var result))
                {
                    return (T)result;
                }
            }

            return default;
        }


        public static string ToWildcardMinor(this Version version)
        {
            if (version != null)
            {
                return $"{version.Major}.*";
            }

            return null;
        }

        public static string ToWildcardBuild(this Version version)
        {
            if (version != null)
            {
                return $"{version.Major}.{version.Minor}.*";
            }

            return null;
        }

        public static bool MatchVersion(string targetVersion, string matchVersion)
        {
            if (!string.IsNullOrEmpty(targetVersion) && !string.IsNullOrEmpty(matchVersion))
            {
                if (matchVersion.Contains('*'))
                {
                    var versionParts = matchVersion.Split('.');
                    if (!versionParts.IsNullOrEmpty())
                    {
                        var match = false;
                        var mVersion = new Version();
                        var pVersion = targetVersion.ToVersion();

                        for (var i = 0; i < versionParts.Length; i++)
                        {
                            if (versionParts[i] != "*")
                            {
                                switch (i)
                                {
                                    case 0: mVersion = new Version(versionParts[i].ToInt(), 0, 0); break;
                                    case 1: mVersion = new Version(mVersion.Major, versionParts[i].ToInt(), 0); break;
                                }
                            }

                            match = pVersion >= mVersion;
                        }

                        return match;
                    }
                }
                else
                {
                    return targetVersion == matchVersion;
                }
            }

            return false;
        }

        public static byte[] ConvertHexidecimalToBytes(string hex)
        {
            if (hex != null)
            {
                try
                {
                    if (hex.Length % 2 == 1) throw new Exception("The binary key cannot have an odd number of digits");

                    byte[] arr = new byte[hex.Length >> 1];

                    for (int i = 0; i < hex.Length >> 1; ++i)
                    {
                        arr[i] = (byte)((GetHexVal(hex[i << 1]) << 4) + (GetHexVal(hex[(i << 1) + 1])));
                    }

                    return arr;
                }
                catch { }
            }

            return null;
        }

        private static int GetHexVal(char hex)
        {
            int val = (int)hex;
            //For uppercase A-F letters:
            //return val - (val < 58 ? 48 : 55);
            //For lowercase a-f letters:
            //return val - (val < 58 ? 48 : 87);
            //Or the two combined, but a bit slower:
            return val - (val < 58 ? 48 : (val < 97 ? 55 : 87));
        }


        #region "MD5"

        public static string ToMD5Hash(this string s)
        {
            try
            {
                var hash = MD5Algorithm.ComputeHash(_utf8.GetBytes(s));
                return string.Concat(hash.Select(b => b.ToString("x2")));
            }
            catch { }

            return null;
        }

        public static string ToMD5Hash(this byte[] bytes)
        {
            if (bytes != null)
            {
                try
                {
                    var hash = MD5Algorithm.ComputeHash(bytes);
                    return string.Concat(hash.Select(b => b.ToString("x2")));
                }
                catch { }
            }

            return null;
        }

        public static string ToMD5HashString(this byte[] hashBytes)
        {
            if (hashBytes != null)
            {
                try
                {
                    return string.Concat(hashBytes.Select(b => b.ToString("x2")));
                }
                catch { }
            }

            return null;
        }

        public static byte[] ToMD5HashBytes(this string s)
        {
            try
            {
                return MD5Algorithm.ComputeHash(_utf8.GetBytes(s));
            }
            catch { }

            return null;
        }

        public static byte[] ToMD5HashBytes(this byte[] bytes)
        {
            if (bytes != null)
            {
                try
                {
                    return MD5Algorithm.ComputeHash(bytes);
                }
                catch { }
            }
            return null;
        }

        public static string ToMD5Hash(string[] lines)
        {
            if (lines != null && lines.Length > 0)
            {
                var x1 = lines[0];
                var h = x1.ToMD5Hash();

                for (int i = 1; i < lines.Length; i++)
                {
                    x1 = lines[i].ToMD5Hash();
                    x1 = h + x1;
                    h = x1.ToMD5Hash();
                }

                return h;
            }

            return null;
        }

        public static byte[] ToMD5HashBytes(byte[][] hashBytes)
        {
            if (hashBytes != null && hashBytes.Length > 0)
            {
                var x1 = hashBytes[0];
                var x2 = x1;
                byte[] a1;

                for (int i = 1; i < hashBytes.Length; i++)
                {
                    x2 = hashBytes[i];
                    if (x2 != null)
                    {
                        a1 = new byte[x1.Length + x2.Length];
                        Array.Copy(x1, 0, a1, 0, x1.Length);
                        Array.Copy(x2, 0, a1, x1.Length, x2.Length);

                        x1 = a1.ToMD5HashBytes();
                    }
                }

                return x2;
            }

            return null;
        }

        #endregion

        #region "SHA1"

        public static string ToSHA1Hash(this string s)
        {
            try
            {
                var hash = SHA1Algorithm.ComputeHash(_utf8.GetBytes(s));
                return string.Concat(hash.Select(b => b.ToString("x2")));
            }
            catch { }

            return null;
        }

        public static string ToSHA1Hash(this byte[] bytes)
        {
            if (bytes != null)
            {
                try
                {
                    var hash = SHA1Algorithm.ComputeHash(bytes);
                    return string.Concat(hash.Select(b => b.ToString("x2")));
                }
                catch { }
            }

            return null;
        }

        public static string ToSHA1HashString(this byte[] hashBytes)
        {
            if (hashBytes != null)
            {
                try
                {
                    return string.Concat(hashBytes.Select(b => b.ToString("x2")));
                }
                catch { }
            }

            return null;
        }

        public static byte[] ToSHA1HashBytes(this string s)
        {
            try
            {
                return SHA1Algorithm.ComputeHash(_utf8.GetBytes(s));
            }
            catch { }

            return null;
        }

        public static byte[] ToSHA1HashBytes(this byte[] bytes)
        {
            if (bytes != null)
            {
                try
                {
                    return SHA1Algorithm.ComputeHash(bytes);
                }
                catch { }
            }
            return null;
        }

        public static string ToSHA1Hash(string[] lines)
        {
            if (lines != null && lines.Length > 0)
            {
                var x1 = lines[0];
                var h = x1.ToSHA1Hash();

                for (int i = 1; i < lines.Length; i++)
                {
                    x1 = lines[i].ToSHA1Hash();
                    x1 = h + x1;
                    h = x1.ToSHA1Hash();
                }

                return h;
            }

            return null;
        }

        public static byte[] ToSHA1HashBytes(byte[][] hashBytes)
        {
            if (hashBytes != null && hashBytes.Length > 0)
            {
                var x1 = hashBytes[0];
                var x2 = x1;
                byte[] a1;

                for (int i = 1; i < hashBytes.Length; i++)
                {
                    x2 = hashBytes[i];
                    if (x2 != null)
                    {
                        a1 = new byte[x1.Length + x2.Length];
                        Array.Copy(x1, 0, a1, 0, x1.Length);
                        Array.Copy(x2, 0, a1, x1.Length, x2.Length);

                        x1 = a1.ToSHA1HashBytes();
                    }
                }

                return x2;
            }

            return null;
        }

        #endregion

        #region "SHA256"

        public static string ToSHA256Hash(this string s)
        {
            try
            {
                var hash = SHA256.HashData(_utf8.GetBytes(s));
                //var hash = SHA256Algorithm.ComputeHash(_utf8.GetBytes(s));
                return string.Concat(hash.Select(b => b.ToString("x2")));
            }
            catch { }

            return null;
        }

        //public static string ToSHA256Hash(this string s)
        //{
        //    try
        //    {
        //        var hash = SHA256Algorithm.ComputeHash(_utf8.GetBytes(s));
        //        return string.Concat(hash.Select(b => b.ToString("x2")));
        //    }
        //    catch { }

        //    return null;
        //}

        public static string ToSHA256Hash(this byte[] bytes)
        {
            if (bytes != null)
            {
                try
                {
                    var hash = SHA256.HashData(bytes);
                    return string.Concat(hash.Select(b => b.ToString("x2")));
                }
                catch { }
            }

            return null;
        }

        //public static string ToSHA256Hash(this byte[] bytes)
        //{
        //    if (bytes != null)
        //    {
        //        try
        //        {
        //            var hash = SHA256Algorithm.ComputeHash(bytes);
        //            return string.Concat(hash.Select(b => b.ToString("x2")));
        //        }
        //        catch { }
        //    }

        //    return null;
        //}

        public static string ToSHA256HashString(this byte[] hashBytes)
        {
            if (hashBytes != null)
            {
                try
                {
                    return string.Concat(hashBytes.Select(b => b.ToString("x2")));
                }
                catch { }
            }

            return null;
        }

        public static byte[] ToSHA256HashBytes(this string s)
        {
            try
            {
                return SHA256Algorithm.ComputeHash(_utf8.GetBytes(s));
            }
            catch { }

            return null;
        }

        public static byte[] ToSHA256HashBytes(this byte[] bytes)
        {
            if (bytes != null)
            {
                return SHA256.HashData(bytes);
            }
            return null;
        }

        //public static byte[] ToSHA256HashBytes(this byte[] bytes)
        //{
        //    if (bytes != null)
        //    {
        //        try
        //        {
        //            return SHA256Algorithm.ComputeHash(bytes);
        //        }
        //        catch { }
        //    }
        //    return null;
        //}

        public static string ToSHA256Hash(string[] lines)
        {
            if (lines != null && lines.Length > 0)
            {
                var x1 = lines[0];
                var h = x1.ToSHA256Hash();

                for (int i = 1; i < lines.Length; i++)
                {
                    x1 = lines[i].ToSHA256Hash();
                    x1 = h + x1;
                    h = x1.ToSHA256Hash();
                }

                return h;
            }

            return null;
        }

        public static byte[] ToSHA256HashBytes(byte[][] hashBytes)
        {
            if (hashBytes != null && hashBytes.Length > 0)
            {
                var x1 = hashBytes[0];
                var x2 = x1;
                byte[] a1;

                for (int i = 1; i < hashBytes.Length; i++)
                {
                    x2 = hashBytes[i];
                    if (x2 != null)
                    {
                        a1 = new byte[x1.Length + x2.Length];
                        Array.Copy(x1, 0, a1, 0, x1.Length);
                        Array.Copy(x2, 0, a1, x1.Length, x2.Length);

                        x1 = a1.ToSHA256HashBytes();
                    }
                }

                return x1;
                //return x2;
            }

            return null;
        }

        #endregion

    }
}
