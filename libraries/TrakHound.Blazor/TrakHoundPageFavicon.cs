// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Sections;

namespace TrakHound.Blazor
{
    public class TrakHoundPageFavicon : ComponentBase
    {
        [Parameter]
        public string Path { get; set; }


        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenComponent<SectionContent>(0);
            builder.AddComponentParameter(1, nameof(SectionContent.SectionName), TrakHoundHeadOutlet.FaviconSectionName);
            builder.AddComponentParameter(2, nameof(SectionContent.ChildContent), (RenderFragment)BuildFavicon);
            builder.CloseComponent();
        }

        private void BuildFavicon(RenderTreeBuilder builder)
        {
            if (Path != null)
            {
                builder.OpenElement(0, "link");
                builder.AddAttribute(1, "rel", "icon");
                builder.AddAttribute(2, "type", "image/x-icon");
                builder.AddAttribute(3, "href", Path);
                builder.CloseElement();
            }
        }
    }
}
