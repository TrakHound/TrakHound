// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Routing
{
    struct RouteQueryParameter
    {
        public string Key { get; set; }

        public string Value { get; set; }


        public RouteQueryParameter(string key, object value)
        {
            Key = key;
            Value = value != null ? value.ToString() : null;
        }
    }
}
