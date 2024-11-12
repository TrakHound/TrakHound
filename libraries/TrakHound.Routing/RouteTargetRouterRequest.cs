// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Routing
{
    class RouteTargetRouterRequest<TEntity>
    {
        public IRouteTarget Target { get; set; }

        public TrakHoundRouter Router { get; set; }

        public RouteRequest Request { get; set; }


        public RouteTargetRouterRequest(IRouteTarget target, TrakHoundRouter router, RouteRequest request)
        {
            Target = target;
            Router = router;
            Request = request;
        }
    }
}
