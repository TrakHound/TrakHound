// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Components;
using TrakHound.Clients;
using TrakHound.Logging;
using TrakHound.Packages;
using TrakHound.Security;
using TrakHound.Volumes;

namespace TrakHound.Blazor.Routing
{
    public class TrakHoundRouteData
    {
        public ITrakHoundPageRouteManager RouteManager { get; set; }

        public string AppId { get; set; }

        public string AppName { get; set; }

        public ITrakHoundClient Client { get; set; }

        public TrakHoundPackage Package { get; set; }

        public ITrakHoundVolume Volume { get; set; }

        public ITrakHoundLogger Logger { get; set; }

        public ITrakHoundSession Session { get; set; }

        public string BaseUrl { get; set; }

        public string BasePath { get; set; }

        public string BaseLocation { get; set; }

        public RouteData Data { get; set; }
    }
}
