// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Sections;

namespace TrakHound.Blazor
{
    public class TrakHoundHeadOutlet : ComponentBase
    {
        internal const string TitleSectionName = "TrakHound.Page.Title";

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.AddMarkupContent(0, "<!-- TrakHound Page Title -->");
            builder.OpenComponent<SectionOutlet>(1);
            builder.AddComponentParameter(2, nameof(SectionOutlet.SectionName), TitleSectionName);
            builder.CloseComponent();
            builder.AddMarkupContent(3, "<!-- End Package Stylesheets -->");
        }
    }
}
