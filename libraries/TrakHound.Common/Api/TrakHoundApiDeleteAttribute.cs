// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Api
{
    public class TrakHoundApiDeleteAttribute : TrakHoundApiEndpointRouteAttribute
    {
        public TrakHoundApiDeleteAttribute() 
        { 
            Type = TrakHoundApiRouteType.Delete;
        }

        public TrakHoundApiDeleteAttribute(string route, string description = null)
        {
            Type = TrakHoundApiRouteType.Delete;
            Route = route;
            Description = description;
        }
    }
}
