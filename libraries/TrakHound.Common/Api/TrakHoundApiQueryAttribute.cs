// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Api
{
    public class TrakHoundApiQueryAttribute : TrakHoundApiEndpointRouteAttribute
    {
        public TrakHoundApiQueryAttribute() 
        {
            Type = TrakHoundApiRouteType.Query;
        }

        public TrakHoundApiQueryAttribute(string route, string description = null)
        {
            Type = TrakHoundApiRouteType.Query;
            Route = route;
            Description = description;
        }
    }
}
