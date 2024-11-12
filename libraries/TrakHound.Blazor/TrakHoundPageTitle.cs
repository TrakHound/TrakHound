// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Sections;

namespace TrakHound.Blazor
{
    public class TrakHoundPageTitle : ComponentBase
    {
        [Parameter]
        public RenderFragment ChildContent { get; set; }


        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenComponent<SectionContent>(0);
            builder.AddComponentParameter(1, nameof(SectionContent.SectionName), TrakHoundHeadOutlet.TitleSectionName);
            builder.AddComponentParameter(2, nameof(SectionContent.ChildContent), (RenderFragment)BuildTitle);
            builder.CloseComponent();
        }

        private void BuildTitle(RenderTreeBuilder builder)
        {
            if (ChildContent != null)
            {
                builder.OpenElement(0, "title");
                builder.AddContent(1, ChildContent);
                builder.CloseElement();
            }
        }
    }
}
