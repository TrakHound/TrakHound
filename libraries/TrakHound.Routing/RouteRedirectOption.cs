// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Routing
{
    struct RouteRedirectOption<T>
    {
        public RouteRedirectOptions Option { get; set; }

        public string Request { get; set; }

        public string Id { get; set; }

        public T Argument { get; set; }


        public RouteRedirectOption(RouteRedirectOptions option, string request, T argument = default, string id = null)
        {
            Option = option;
            Request = request;
            Id = id;
            if (string.IsNullOrEmpty(Id)) Id = argument != null ? argument.ToString() : null;
            Argument = argument;
        }
    }
}
