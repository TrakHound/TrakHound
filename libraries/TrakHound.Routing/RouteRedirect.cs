// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using TrakHound.Entities.Filters;

namespace TrakHound.Routing
{
    class RouteRedirect
    {
        public IEnumerable<string> Conditions { get; set; }

        public TrakHoundRedirectConfiguration Configuration { get; set; }

        public IEnumerable<IRouteTarget> Targets { get; set; }

        public IEnumerable<RouteRedirectOptions> Options { get; set; }


        public RouteRedirect(
            TrakHoundRouterConfiguration routerConfiguration,
            TrakHoundRouteConfiguration routeConfiguration,
            TrakHoundRedirectConfiguration redirectConfiguration,
            TrakHoundEntityPatternFilter filter
            )
        {
            if (routeConfiguration != null && redirectConfiguration != null)
            {
                Conditions = redirectConfiguration.Conditions;
                Configuration = redirectConfiguration;

                // Read Targets
                if (!redirectConfiguration.Targets.IsNullOrEmpty())
                {
                    var targets = new List<IRouteTarget>();
                    foreach (var targetConfiguration in redirectConfiguration.Targets)
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
                    Targets = targets;
                }

                var options = new List<RouteRedirectOptions>();
                if (!redirectConfiguration.Options.IsNullOrEmpty())
                {
                    options.AddRange(redirectConfiguration.Options);
                }
                Options = options;
            }
        }


        public IEnumerable<string> GetTargetIds()
        {
            var ids = new List<string>();

            if (!Targets.IsNullOrEmpty())
            {
                foreach (var target in Targets)
                {
                    ids.AddRange(target.GetIds());
                }
            }

            return ids;
        }
    }
}
