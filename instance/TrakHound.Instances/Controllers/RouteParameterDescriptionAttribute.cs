// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;

namespace TrakHound.Controllers.Http
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public class RouteParameterDescriptionAttribute : Attribute
    {
        public string ParameterName { get; set; }

        public string Description { get; set; }


        public RouteParameterDescriptionAttribute(string parameterName, string description)
        {
            ParameterName = parameterName;
            Description = description;
        }
    }
}
