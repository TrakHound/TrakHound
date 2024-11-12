// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;

namespace TrakHound.Controllers.Http
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public class EntityRouteResponseAttribute : RouteResponseAttribute
    {
        public EntityRouteResponseAttribute(int statusCode, Type responseType) : base(statusCode, responseType) { }
    }
}
