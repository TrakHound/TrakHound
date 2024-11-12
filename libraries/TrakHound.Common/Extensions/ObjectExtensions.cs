// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;

namespace TrakHound
{
    public static class ObjectExtensions
    {
        public static bool IsNumeric(this object value)
        {
            if (value != null) return double.TryParse(value.ToString(), out _);
            return false;
        }

        public static bool ToBoolean(this object o)
        {
            var s = o?.ToString();
            if (!string.IsNullOrEmpty(s) && bool.TryParse(s, out var x)) return x;
            else return false;
        }

        public static byte ToByte(this object o)
        {
            var s = o?.ToString();
            if (!string.IsNullOrEmpty(s) && byte.TryParse(s, out var x)) return x;
            else return 0;
        }

        public static int ToInt(this object o)
        {
            var s = o?.ToString();
            if (!string.IsNullOrEmpty(s) && int.TryParse(s, out var x)) return x;
            else return -1;
        }

        public static uint ToUInt(this uint o)
        {
            if (o > 0) return (uint)o;
            else return 0;
        }

        public static uint ToUInt(this object o)
        {
            var s = o?.ToString();
            if (!string.IsNullOrEmpty(s) && uint.TryParse(s, out var x)) return x;
            else return 0;
        }

        public static long ToLong(this object o)
        {
            var s = o?.ToString();
            if (!string.IsNullOrEmpty(s) && long.TryParse(s, out var x)) return x;
            else return -1;
        }

        public static ulong ToULong(this long o)
        {
            if (o > 0) return (ulong)o;
            else return 0;
        }

        public static ulong ToULong(this double o)
        {
            var s = o.ToString("F0");
            if (!string.IsNullOrEmpty(s) && ulong.TryParse(s, out var x)) return x;
            else return 0;
        }

        public static ulong ToULong(this object o)
        {
            var s = o?.ToString();
            if (!string.IsNullOrEmpty(s) && ulong.TryParse(s, out var x)) return x;
            else return 0;
        }

        public static double ToDouble(this object o)
        {
            var s = o?.ToString();
            if (!string.IsNullOrEmpty(s) && double.TryParse(s, out var x)) return x;
            else return -1;
        }

        public static double ToDouble(this object o, int decimalPlaces = int.MaxValue)
        {
            var s = o?.ToString();
            if (!string.IsNullOrEmpty(s) && double.TryParse(s, out var x))
            {
                return Math.Round(x, decimalPlaces);
            }
            else return -1;
        }

        public static SortOrder ToSortOrder(this object o)
        {
            if (o != null)
            {
                if (o.IsNumeric())
                {
                    return (SortOrder)(o.ToInt());
                }
                else
                {
                    var s = o.ToString().ToLower();
                    switch (s)
                    {
                        case SortOrderAbbreviations.Ascending: return SortOrder.Ascending;
                        case SortOrderAbbreviations.Descending: return SortOrder.Descending;
                    }
                }
            }

            return SortOrder.Ascending;
        }

        public static string ToJson(this object o, bool indented = false)
        {
            return Json.Convert(o, null, indented);
        }

        public static int GetDigitCount(this double num)
        {
            int digits = 0;
            while (num >= 1)
            {
                digits++;
                num /= 10;
            }
            return digits;
        }

        public static int GetDigitCountAfterDecimal(this double value)
        {
            bool start = false;
            int count = 0;
            foreach (var s in value.ToString())
            {
                if (s == '.')
                {
                    start = true;
                }
                else if (start)
                {
                    count++;
                }
            }

            return count;
        }

        public static T ConvertEnum<T>(object value)
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

        public static bool ByteArraysEqual(byte[] b1, byte[] b2)
        {
            if (b1 == b2) return true;
            if (b1 == null || b2 == null) return false;
            if (b1.Length != b2.Length) return false;
            for (int i = 0; i < b1.Length; i++)
            {
                if (b1[i] != b2[i]) return false;
            }
            return true;
        }
    }
}
