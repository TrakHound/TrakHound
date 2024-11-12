// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TrakHound.Routing
{
    // Read
    // - Absolute
    // - Query
    // - Latest
    // - Current
    // - Subscribe

    // Write
    // - Publish
    // - Update
    // - Send
    // - Expire
    // - Delete
    // - Empty


    public static class TrakHoundRoutes
    {
        public const string Wildcard = "*";

        private static Dictionary<Type, string> _routeCache;


        public static Dictionary<Type, string> GetRoutes()
        {
            if (_routeCache == null)
            {
                var routes = new Dictionary<Type, string>();

                foreach (var route in Routers.TrakHoundBlobRoutes._routes) routes.Add(route.Key, route.Value);
                foreach (var route in Routers.TrakHoundCommandRoutes._routes) routes.Add(route.Key, route.Value);
                foreach (var route in Routers.TrakHoundMessageRoutes._routes) routes.Add(route.Key, route.Value);
                foreach (var route in Routers.TrakHoundMessageQueueRoutes._routes) routes.Add(route.Key, route.Value);
                foreach (var route in Routers.TrakHoundSourceRoutes._routes) routes.Add(route.Key, route.Value);
                foreach (var route in Routers.TrakHoundDefinitionRoutes._routes) routes.Add(route.Key, route.Value);
                foreach (var route in Routers.TrakHoundObjectRoutes._routes) routes.Add(route.Key, route.Value);

                _routeCache = routes;
            }

            return _routeCache;
        }


        public static string Get(Type type)
        {
            if (type != null)
            {
                GetRoutes().TryGetValue(type, out var route);
                if (route != null) return route;

                var interfaceTypes = type.GetInterfaces();
                if (!interfaceTypes.IsNullOrEmpty())
                {
                    foreach (var interfaceType in interfaceTypes)
                    {
                        GetRoutes().TryGetValue(interfaceType, out route);
                        if (route != null) return route;
                    }
                }
            }

            return null;
        }


        public static IEnumerable<string> Get(IEnumerable<Type> types)
        {
            var routes = new List<string>();

            if (!types.IsNullOrEmpty())
            {
                foreach (var type in types)
                {
                    GetRoutes().TryGetValue(type, out var route);
                    if (route != null) routes.Add(route);

                    var interfaceTypes = type.GetInterfaces();
                    if (!interfaceTypes.IsNullOrEmpty())
                    {
                        foreach (var interfaceType in interfaceTypes)
                        {
                            GetRoutes().TryGetValue(interfaceType, out route);
                            if (route != null) routes.Add(route);
                        }
                    }
                }
            }

            return routes;
        }


        public static bool IsMatch(string pattern, string route)
        {
            if (route == Wildcard) return true;
            if (pattern == route) return true;

            try
            {
                var regex = new Regex(route);
                var match = regex.Match(pattern);

                if (match != null && match.Success) return true;
            }
            catch { }

            return false;
        }

        public static bool IsMatch(Type type, string route)
        {
            var types = new List<Type> { type };
            var typeRoutes = Get(types);
            if (!typeRoutes.IsNullOrEmpty())
            {
                foreach (var typeRoute in typeRoutes)
                {
                    if (route == Wildcard) return true;
                    if (typeRoute == route) return true;

                    try
                    {
                        var regex = new Regex(route);
                        var match = regex.Match(typeRoute);

                        if (match != null && match.Success) return true;
                    }
                    catch { }
                }
            }

            return false;
        }
    }
}
