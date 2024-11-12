// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Api
{
    class RouteParameter
    {
        public string Name { get; set; }

        public string Value { get; set; }

        public RouteParameter(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
}
