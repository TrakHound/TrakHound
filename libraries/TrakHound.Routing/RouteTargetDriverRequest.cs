// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Drivers;

namespace TrakHound.Routing
{
    class RouteTargetDriverRequest<TDriver, TEntity> where TDriver : ITrakHoundDriver
    {
        public IRouteTarget Target { get; set; }

        public TDriver Driver { get; set; }

        public RouteRequest Request { get; set; }


        public RouteTargetDriverRequest(IRouteTarget target, TDriver driver, RouteRequest request)
        {
            Target = target;
            Driver = driver;
            Request = request;
        }
    }
}
