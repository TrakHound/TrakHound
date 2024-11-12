// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;

namespace TrakHound.Api
{
    public static class TrakHoundApiSchemaDataType
    {
        public const string Float = "float";
        public const string Long = "long";
        public const string Integer = "integer";
        public const string String = "string";
        public const string Timestamp = "timestamp";


        public static string Get(Type type)
        {
            if (type != null)
            {
                if (type == typeof(double) || type == typeof(decimal))
                {
                    return Float;
                }
                else if (type == typeof(long))
                {
                    return Long;
                }
                else if (type == typeof(int))
                {
                    return Integer;
                }
                else if (type == typeof(string))
                {
                    return String;
                }
                else if (type == typeof(DateTime))
                {
                    return Timestamp;
                }
            }

            return null;
        }
    }
}
