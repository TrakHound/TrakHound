// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Api
{
    public class TrakHoundApiSubscribeAttribute : TrakHoundApiEndpointRouteAttribute
    {
        public TrakHoundApiSubscribeAttribute() 
        {
            Type = TrakHoundApiRouteType.Subscribe;
        }

        public TrakHoundApiSubscribeAttribute(string route, string description = null)
        {
            Type = TrakHoundApiRouteType.Subscribe;
            Route = route;
            Description = description;
        }
    }
}
