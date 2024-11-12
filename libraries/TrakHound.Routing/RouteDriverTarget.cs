// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;
using TrakHound.Drivers;
using TrakHound.Entities.Filters;

namespace TrakHound.Routing
{
    class RouteDriverTarget : RouteTarget, IRouteDriverTarget
    {
        public IEnumerable<ITrakHoundDriver> Drivers { get; set; }


        public RouteDriverTarget(
            TrakHoundRouterConfiguration routerConfiguration,
            TrakHoundRouteConfiguration routeConfiguration,
            TrakHoundTargetConfiguration targetConfiguration,
            TrakHoundEntityPatternFilter filter
            ) 
            : base(routerConfiguration, routeConfiguration, targetConfiguration, filter) { }


        public TDriver GetDriver<TDriver>() where TDriver : ITrakHoundDriver
        {
            if (!Drivers.IsNullOrEmpty())
            {
                // Get the Driver that matches the specified IApiDriver (TDriver)
                return (TDriver)Drivers.FirstOrDefault(x => typeof(TDriver).IsAssignableFrom(x.GetType()));
            }

            return default;
        }

        public IEnumerable<TDriver> GetDrivers<TDriver>() where TDriver : ITrakHoundDriver
        {
            var x = new List<TDriver>();

            if (!Drivers.IsNullOrEmpty())
            {
                // Get the Driver(s) that matches the specified ITrakHoundDriver (TDriver)
                var y = Drivers.Where(x => typeof(TDriver).IsAssignableFrom(x.GetType())).OfType<TDriver>();
                if (!y.IsNullOrEmpty()) x.AddRange(y);
            }

            return x;
        }
    }
}
