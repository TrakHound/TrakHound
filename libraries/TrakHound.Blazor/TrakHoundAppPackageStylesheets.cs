// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Sections;

namespace TrakHound.Blazor
{
    public class TrakHoundAppPackageStylesheets : ComponentBase
    {
        [Inject]
        public TrakHound.Apps.ITrakHoundAppProvider AppProvider { get; set; }

        [Parameter]
        public string AppId { get; set; }


        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenComponent<SectionContent>(0);
            builder.AddComponentParameter(1, nameof(SectionContent.SectionName), TrakHoundAppHeadOutlet.StylesheetSectionName);
            builder.AddComponentParameter(2, nameof(SectionContent.ChildContent), (RenderFragment)BuildStylesheetLinks);
            builder.CloseComponent();
        }

        private void BuildStylesheetLinks(RenderTreeBuilder builder)
        {
            if (AppProvider != null)
            {
                var stylesheetFiles = AppProvider.GetAppStylesheets(AppId);
                if (!stylesheetFiles.IsNullOrEmpty())
                {
                    var i = 0;
                    foreach (var stylesheetFile in stylesheetFiles)
                    {
                        var stylesheetPath = System.IO.Path.Combine("_packages", stylesheetFile);
                        stylesheetPath = stylesheetPath.Replace('\\', '/');

                        //<link rel="stylesheet" href="@stylesheetPath" />
                        builder.OpenElement(i++, "link");
                        builder.AddAttribute(i++, "rel", "stylesheet");
                        builder.AddAttribute(i++, "href", stylesheetPath);
                        builder.CloseElement();
                    }
                }
            }
        }
    }
}
