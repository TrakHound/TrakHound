// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Blazor.ExplorerInternal.Components
{
    public struct BreadcrumbItem
    {
        public string Display { get; set; }

        public string Link { get; set; }


        public BreadcrumbItem(string display, string link)
        {
            Display = display;
            Link = link;
        }
    }
}
