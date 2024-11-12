// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.IO;

namespace TrakHound.Api
{
    public class FromBodyAttribute : Attribute 
    {
        public string ContentType { get; set; }


        public FromBodyAttribute() { }

        public FromBodyAttribute(string contentType)
        {
            ContentType = contentType;
        }


        public static string GetDefaultContentType(Type type)
        {
            if (type == typeof(string)) return "text/plain";
            if (type == typeof(byte[])) return "application/octet-stream";
            if (type == typeof(Stream)) return "application/octet-stream";

            return "application/json";
        }
    }
}
