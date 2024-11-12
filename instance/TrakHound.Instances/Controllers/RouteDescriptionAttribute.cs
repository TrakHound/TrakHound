// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;

namespace TrakHound.Controllers.Http
{
    public class RouteDescriptionAttribute : Attribute
    {
        public string Description { get; set; }


        public RouteDescriptionAttribute(string description)
        {
            Description = description;
        }
    }
}
