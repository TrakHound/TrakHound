// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;

namespace TrakHound.Controllers.Http
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public class RouteResponseAttribute : Attribute
    {
        public int StatusCode { get; set; }

        public Type ResponseType { get; set; }


        public RouteResponseAttribute(int statusCode)
        {
            StatusCode = statusCode;
        }

        public RouteResponseAttribute(int statusCode, Type responseType)
        {
            StatusCode = statusCode;
            ResponseType = responseType;
        }
    }
}
