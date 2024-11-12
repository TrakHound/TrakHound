// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;

namespace TrakHound.Api
{
    public class TrakHoundApiControllerAttribute : Attribute
    {
        public string Route { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }


        public TrakHoundApiControllerAttribute(string route, string name = null, string description = null)
        {
            Route = route;
            Name = name;
            Description = description;
        }
    }
}
