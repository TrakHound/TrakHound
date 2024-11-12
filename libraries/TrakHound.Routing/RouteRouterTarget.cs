// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Entities.Filters;

namespace TrakHound.Routing
{
    class RouteRouterTarget : RouteTarget, IRouteRouterTarget
    {
        public TrakHoundRouter Router { get; set; }


        public RouteRouterTarget(
            TrakHoundRouterConfiguration routerConfiguration,
            TrakHoundRouteConfiguration routeConfiguration,
            TrakHoundTargetConfiguration targetConfiguration,
            TrakHoundEntityPatternFilter filter
            )
            : base (routerConfiguration, routeConfiguration, targetConfiguration, filter) { }
    }
}
