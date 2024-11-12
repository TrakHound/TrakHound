// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Apps;
using TrakHound.Packages;

namespace TrakHound.Blazor.Apps
{
    class AppInfo
    {
        public string Route { get; set; }

        public ITrakHoundAppConfiguration Configuration { get; set; }

        public TrakHoundPackage Package { get; set; }

        public string PackageReadMe { get; set; }

        public IEnumerable<AppPageInfo> PageInfos = new List<AppPageInfo>();


        public AppInfo(string route, ITrakHoundAppConfiguration configuration, TrakHoundPackage package, IEnumerable<AppPageInfo> pageInfos = null, string packageReadMe = null)
        {
            Route = route;
            Configuration = configuration;
            Package= package;
            PageInfos = pageInfos;
            PackageReadMe = packageReadMe;
        }
    }
}
