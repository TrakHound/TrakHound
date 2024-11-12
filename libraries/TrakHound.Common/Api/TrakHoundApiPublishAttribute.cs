// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Api
{
    public class TrakHoundApiPublishAttribute : TrakHoundApiEndpointRouteAttribute
    {
        public TrakHoundApiPublishAttribute() 
        {
            Type = TrakHoundApiRouteType.Publish;
        }

        public TrakHoundApiPublishAttribute(string route, string description = null)
        {
            Type = TrakHoundApiRouteType.Publish;
            Route = route;
            Description = description;
        }
    }
}
