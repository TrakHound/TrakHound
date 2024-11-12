// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using TrakHound.Entities.Filters;

namespace TrakHound.Routing
{
    interface IRouteTarget
    {
        string Id { get; }

        TrakHoundRouteConfiguration RouteConfiguration { get; }

        TrakHoundTargetConfiguration TargetConfiguration { get; }

        RouteTargetType Type { get; }

        IEnumerable<RouteRedirect> Redirects { get; }

        TrakHoundEntityPatternFilter Filter { get; }


        IEnumerable<string> GetIds();
    }
}
