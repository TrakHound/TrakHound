// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;

namespace TrakHound.Api
{
    public class FromRouteAttribute : Attribute
    {
        public string ParameterName { get; set; }


        public FromRouteAttribute() { }

        public FromRouteAttribute(string name)
        {
            ParameterName = name;
        }
    }
}
