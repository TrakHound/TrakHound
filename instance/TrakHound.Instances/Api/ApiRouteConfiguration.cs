// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using TrakHound.Packages;

namespace TrakHound.Api
{
    struct ApiRouteConfiguration
    {
        public string Route { get; set; }

        public TrakHoundPackage Package { get; set; }

        public string PackageReadMe { get; set; }

        public ITrakHoundApiConfiguration Configuration { get; set; }

        public IEnumerable<ITrakHoundApiController> Controllers { get; set; }


        public ApiRouteConfiguration(string route, TrakHoundPackage package, ITrakHoundApiConfiguration configuration, IEnumerable<ITrakHoundApiController> controllers, string packageReadMe = null)
        {
            Route = route;
            Package= package;
            Configuration= configuration;
            Controllers = controllers;
            PackageReadMe = packageReadMe;
        }
    }
}
