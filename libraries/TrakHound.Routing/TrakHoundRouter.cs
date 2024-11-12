// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TrakHound.Buffers;
using TrakHound.Clients;
using TrakHound.Drivers;
using TrakHound.Entities;
using TrakHound.Entities.Filters;
using TrakHound.Extensions;
using TrakHound.Logging;
using TrakHound.Routing.Routers;

namespace TrakHound.Routing
{
    public class TrakHoundRouter
    {
        public const string Default = "default";

        private static ITrakHoundLogger _logger = new TrakHoundLogger<TrakHoundRouter>();

        private readonly Dictionary<string, bool?> _routeCache = new Dictionary<string, bool?>();

        private readonly string _id;
        private readonly TrakHoundRouterConfiguration _configuration;
        private readonly ITrakHoundDriverProvider _driverProvider;
        private readonly ITrakHoundBufferProvider _bufferProvider;
        private readonly ITrakHoundClient _client;
        private readonly object _lock = new object();

        private IEnumerable<IRouteTarget> _targets;
        private TrakHoundBlobRouter _blobs;
        private TrakHoundCommandRouter _commands;
        private TrakHoundEntitiesRouter _entities;
        private TrakHoundMessageRouter _messages;
        private TrakHoundMessageQueueRouter _messageQueues;


        public string Id => _id;

        public TrakHoundRouterConfiguration Configuration => _configuration;

        public ITrakHoundClient Client { get; set; }

        public TrakHoundBlobRouter Blobs => _blobs;

        public TrakHoundCommandRouter Commands => _commands;

        /// <summary>
        /// Router used to access TrakHound Entities
        /// </summary>
        public TrakHoundEntitiesRouter Entities => _entities;

        public TrakHoundMessageRouter Messages => _messages;

        public TrakHoundMessageQueueRouter MessageQueues => _messageQueues;

        public ITrakHoundLogger Logger => _logger;


        public TrakHoundRouter(TrakHoundRouterConfiguration configuration, ITrakHoundDriverProvider driverProvider, ITrakHoundBufferProvider bufferProvider)
        {
            _blobs = new TrakHoundBlobRouter(this);
            _commands = new TrakHoundCommandRouter(this);
            _entities = new TrakHoundEntitiesRouter(this, driverProvider, bufferProvider);
            _messages = new TrakHoundMessageRouter(this);
            _messageQueues = new TrakHoundMessageQueueRouter(this);

            _configuration = configuration;
            if (_configuration != null)
            {
                _id = configuration.Id;

                var targets = InitializeTargets(configuration);
                lock (_lock) _targets = targets;
            }
        }


        public void Initialize(IEnumerable<TrakHoundRouter> routers, IEnumerable<ITrakHoundDriver> drivers)
        {
            _entities.Initialize(routers, drivers);
        }


        #region "Mapping"

        public void MapTargets(IEnumerable<TrakHoundRouter> routers, IEnumerable<ITrakHoundDriver> drivers)
        {
            try
            {
                MapTargets(_targets, drivers);
                MapTargets(_targets, routers);
            }
            catch { } // Ignore Collection Modified and Object Disposed Exceptions
        }

        private void MapTargets(IEnumerable<IRouteTarget> targets, IEnumerable<ITrakHoundDriver> drivers)
        {
            if (!targets.IsNullOrEmpty())
            {
                foreach (var target in targets)
                {
                    MapTargets(target, drivers);
                }
            }
        }

        private void MapTargets(IRouteTarget target, IEnumerable<ITrakHoundDriver> drivers)
        {
            if (target != null)
            {
                if (typeof(IRouteDriverTarget).IsAssignableFrom(target.GetType()))
                {
                    // Assign Drivers
                    var driverTarget = ((IRouteDriverTarget)target);
                    driverTarget.Drivers = GetTargetDrivers(driverTarget, drivers);
                }

                if (!target.Redirects.IsNullOrEmpty())
                {
                    foreach (var redirect in target.Redirects)
                    {
                        // Assign Redirect Targets
                        MapTargets(redirect.Targets, drivers);
                    }
                }
            }
        }


        private void MapTargets(IEnumerable<IRouteTarget> targets, IEnumerable<TrakHoundRouter> routers)
        {
            if (!targets.IsNullOrEmpty())
            {
                foreach (var target in targets)
                {
                    MapTargets(target, routers);
                }
            }
        }

        private void MapTargets(IRouteTarget target, IEnumerable<TrakHoundRouter> routers)
        {
            if (target != null && !routers.IsNullOrEmpty())
            {
                // Map Router
                if (typeof(IRouteRouterTarget).IsAssignableFrom(target.GetType()))
                {
                    // Assign Router
                    var routerTarget = (IRouteRouterTarget)target;
                    routerTarget.Router = routers.FirstOrDefault(o => o.Id == target.Id);
                }

                // Map Redirects
                if (!target.Redirects.IsNullOrEmpty())
                {
                    foreach (var redirect in target.Redirects)
                    {
                        // Assign Redirect Targets
                        MapTargets(redirect.Targets, routers);
                    }
                }
            }
        }

        #endregion

        #region "Targets"

        internal IEnumerable<IRouteTarget> GetTargets<TDriver>(string route)
        {
            var targets = new List<IRouteTarget>();

            lock (_lock)
            {
                if (!string.IsNullOrEmpty(route) && !_targets.IsNullOrEmpty())
                {
                    // Driver Targets
                    foreach (var target in _targets.OfType<IRouteDriverTarget>())
                    {
                        if (target.RouteConfiguration != null && !target.RouteConfiguration.Patterns.IsNullOrEmpty())
                        {
                            foreach (var routePattern in target.RouteConfiguration.Patterns)
                            {
                                if (MatchRoute(routePattern, route))
                                {
                                    targets.Add(target);
                                }
                            }
                        }
                    }

                    // Router Targets
                    foreach (var target in _targets.OfType<IRouteRouterTarget>())
                    {
                        if (target.RouteConfiguration != null && !target.RouteConfiguration.Patterns.IsNullOrEmpty())
                        {
                            foreach (var routePattern in target.RouteConfiguration.Patterns)
                            {
                                if (MatchRoute(routePattern, route))
                                {
                                    targets.Add(target);
                                }
                            }
                        }
                    }
                }
            }

            return targets;
        }

        internal IEnumerable<IRouteTarget> GetTargets<TDriver>(IEnumerable<string> routes)
        {
            var targets = new List<IRouteTarget>();

            if (!routes.IsNullOrEmpty())
            {
                foreach (var route in routes)
                {
                    targets.AddRange(GetTargets<TDriver>(route));
                }
            }

            return targets;
        }

        internal IEnumerable<IRouteTarget> InitializeTargets(TrakHoundRouterConfiguration routerConfiguration)
        {
            var targets = new List<IRouteTarget>();

            if (routerConfiguration != null && !routerConfiguration.Routes.IsNullOrEmpty())
            {
                foreach (var routeConfiguration in routerConfiguration.Routes)
                {
                    // Setup Filter
                    TrakHoundEntityPatternFilter filter = null;
                    if (!routeConfiguration.Filters.IsNullOrEmpty())
                    {
                        var filterQuery = TrakHoundExpression.Convert(routeConfiguration.Filters);
                        if (!string.IsNullOrEmpty(filterQuery))
                        {
                            filter = new TrakHoundEntityPatternFilter(_client.System.Entities);
                            filter.Allow(filterQuery);
                        }
                    }

                    if (!routeConfiguration.Targets.IsNullOrEmpty())
                    {
                        foreach (var targetConfiguration in routeConfiguration.Targets)
                        {
                            if (!string.IsNullOrEmpty(targetConfiguration.Type))
                            {
                                if (targetConfiguration.Type.ToLower() == RouteTargetType.Driver.ToString().ToLower())
                                {
                                    // Add Driver Target
                                    targets.Add(new ParameterRouteDriverTarget(routerConfiguration, routeConfiguration, targetConfiguration, filter));
                                }
                                else if (targetConfiguration.Type.ToLower() == RouteTargetType.Router.ToString().ToLower())
                                {
                                    // Add Router Target
                                    targets.Add(new ParameterRouteRouterTarget(routerConfiguration, routeConfiguration, targetConfiguration, filter));
                                }
                            }
                        }
                    }
                }
            }

            return targets;
        }

        internal IEnumerable<ITrakHoundDriver> GetTargetDrivers(IRouteDriverTarget target, IEnumerable<ITrakHoundDriver> drivers)
        {
            var a = new List<ITrakHoundDriver>();

            if (target != null && !target.RouteConfiguration.Patterns.IsNullOrEmpty() && !drivers.IsNullOrEmpty())
            {
                var x = new List<ITrakHoundDriver>();
                var patterns = target.RouteConfiguration.Patterns.ToList();

                // Get Drivers matching the Route Pattern
                if (!patterns.IsNullOrEmpty())
                {
                    // Allow Patterns
                    var allowPatterns = patterns.Where(o => !o.StartsWith('!'));
                    if (!allowPatterns.IsNullOrEmpty())
                    {
                        foreach (var pattern in allowPatterns)
                        {
                            var targetDrivers = drivers.Where(o => o.IsRouteMatch(pattern));
                            if (!targetDrivers.IsNullOrEmpty())
                            {
                                x.AddRange(targetDrivers);
                            }
                        }
                    }

                    // Deny Patterns
                    var denyPatterns = patterns.Where(o => o.StartsWith('!'));
                    if (!denyPatterns.IsNullOrEmpty())
                    {
                        foreach (var pattern in denyPatterns)
                        {
                            var targetDrivers = drivers.Where(o => o.IsRouteMatch(pattern.Remove(0, 1)));
                            if (!targetDrivers.IsNullOrEmpty())
                            {
                                foreach (var driver in targetDrivers)
                                {
                                    x.RemoveAll(o => o.Id == driver.Id);
                                }
                            }
                        }
                    }
                }

                if (!x.IsNullOrEmpty())
                {
                    // Map to Target ID
                    var targetDrivers = x.Where(o => target.Id == TrakHoundRoutes.Wildcard || o.Configuration.Id == target.Id);
                    if (!targetDrivers.IsNullOrEmpty())
                    {
                        foreach (var driver in targetDrivers)
                        {
                            if (!a.Any(o => o.Id == driver.Id))
                            {
                                a.Add(driver);
                            }
                        }
                    }
                }
            }

            return a;
        }

        private bool MatchRoute(string routePattern, string route)
        {
            if (routePattern == TrakHoundRoutes.Wildcard) return true;
            if (routePattern == route) return true;

            var cacheKey = $"{routePattern}:{route}";

            bool? matched;
            lock (_lock) matched = _routeCache.GetValueOrDefault(cacheKey);
            if (matched == null)
            {
                try
                {
                    var regex = new Regex(routePattern);
                    var match = regex.Match(route);

                    matched = match != null && match.Success;
                    lock (_lock) _routeCache.Add(cacheKey, matched);
                    return matched.Value;
                }
                catch { }
            }
            else
            {
                return matched.Value;
            }

            return false;
        }

        #endregion

    }
}
