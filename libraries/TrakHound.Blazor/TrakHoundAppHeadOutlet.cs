// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Sections;

namespace TrakHound.Blazor
{
    public class TrakHoundAppHeadOutlet : ComponentBase
    {
        internal const string StylesheetSectionName = "TrakHound.App.Package.Stylesheets";

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.AddMarkupContent(4, "<!-- TrakHound App Package Stylesheets -->");
            builder.OpenComponent<SectionOutlet>(5);
            builder.AddComponentParameter(6, nameof(SectionOutlet.SectionName), StylesheetSectionName);
            builder.CloseComponent();
            builder.AddMarkupContent(7, "<!-- End Package Stylesheets -->");
        }
    }
}
