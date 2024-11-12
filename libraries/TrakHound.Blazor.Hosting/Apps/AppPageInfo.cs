// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Blazor.Apps
{
    class AppPageInfo
    {
        public string PageRoute { get; set; }

        public string PageDescription { get; set; }

        public Type PageType { get; set; }


        public AppPageInfo(string pageRoute, Type pageType, string pageDescription = null)
        {
            PageRoute = pageRoute;
            PageDescription = pageDescription;
            PageType = pageType;
        }
    }
}
