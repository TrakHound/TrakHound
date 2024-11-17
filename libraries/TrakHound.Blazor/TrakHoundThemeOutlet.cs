// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Sections;

namespace TrakHound.Blazor
{
    public class TrakHoundThemeOutlet : ComponentBase
    {
        internal const string ThemeSectionName = "TrakHound.Theme.Stylesheets";


        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.AddMarkupContent(0, "<!-- TrakHound Theme -->");
            builder.OpenComponent<SectionOutlet>(1);
            builder.AddComponentParameter(2, nameof(SectionOutlet.SectionName), ThemeSectionName);
            builder.CloseComponent();
            builder.AddMarkupContent(3, "<!-- End Theme -->");
        }
    }
}
