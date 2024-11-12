// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Drivers;
using TrakHound.Routing;

namespace TrakHound.Extensions
{
    internal static class TrakHoundDriverExtensions
    {
        public static bool IsRouteMatch(this ITrakHoundDriver service, string route)
        {
            if (service != null && !string.IsNullOrEmpty(route))
            {
                return TrakHoundRoutes.IsMatch(service.GetType(), route);
            }

            return false;
        }
    }
}
