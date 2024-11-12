// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;
using TrakHound.Drivers;
using TrakHound.Entities.Filters;

namespace TrakHound.Routing
{
    class ParameterRouteDriverTarget : ParameterRouteTarget, IRouteDriverTarget
    {
        private readonly ListDictionary<string, ITrakHoundDriver> _driverCache = new ListDictionary<string, ITrakHoundDriver>();
        private readonly HashSet<string> _emptyCache = new HashSet<string>();
        private readonly object _lock = new object();


        public IEnumerable<ITrakHoundDriver> Drivers { get; set; }


        public ParameterRouteDriverTarget(
            TrakHoundRouterConfiguration routerConfiguration,
            TrakHoundRouteConfiguration routeConfiguration,
            TrakHoundTargetConfiguration targetConfiguration,
            TrakHoundEntityPatternFilter filter
            ) 
            : base(routerConfiguration, routeConfiguration, targetConfiguration, filter) { }


        public IEnumerable<TDriver> GetDrivers<TDriver>() where TDriver : ITrakHoundDriver
        {
            var x = new List<TDriver>();

            if (!Drivers.IsNullOrEmpty())
            {
                // Check the cache for the matching drivers
                var cacheKey = typeof(TDriver).FullName;
                var cachedDrivers = _driverCache.Get(cacheKey);
                if (cachedDrivers.IsNullOrEmpty())
                {
                    bool empty;
                    lock (_lock) empty = _emptyCache.Contains(cacheKey);
                    if (!empty)
                    {
                        // Get the Driver(s) that matches the specified ITrakHoundDriver (TDriver)
                        var matchedDrivers = Drivers.Where(x => typeof(TDriver).IsAssignableFrom(x.GetType())).OfType<TDriver>();
                        if (!matchedDrivers.IsNullOrEmpty())
                        {
                            foreach (var driver in matchedDrivers)
                            {
                                x.Add(driver);
                                _driverCache.Add(cacheKey, driver);
                            }
                        }
                        else
                        {
                            lock (_lock) _emptyCache.Add(cacheKey);
                        }
                    }
                }
                else
                {
                    foreach (var driver in cachedDrivers)
                    {
                        x.Add((TDriver)driver);
                    }
                }
            }

            return x;
        }
    }
}
