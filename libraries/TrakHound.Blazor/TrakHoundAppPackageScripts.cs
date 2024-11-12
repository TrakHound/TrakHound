// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Sections;

namespace TrakHound.Blazor
{
    public class TrakHoundAppPackageScripts : ComponentBase
    {
        [Inject]
        public TrakHound.Apps.ITrakHoundAppProvider AppProvider { get; set; }

        [Parameter]
        public string AppId { get; set; }


        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenComponent<SectionContent>(1);
            builder.AddComponentParameter(2, nameof(SectionContent.SectionName), TrakHoundAppBodyOutlet.ScriptSectionName);
            builder.AddComponentParameter(3, nameof(SectionContent.ChildContent), (RenderFragment)BuildScriptReferences);
            builder.CloseComponent();
        }

        private void BuildScriptReferences(RenderTreeBuilder builder)
        {
            if (AppProvider != null)
            {
                var scriptFiles = AppProvider.GetAppScripts(AppId);
                if (!scriptFiles.IsNullOrEmpty())
                {
                    var i = 0;
                    foreach (var scriptFile in scriptFiles)
                    {
                        var scriptPath = System.IO.Path.Combine("_packages", scriptFile);
                        scriptPath = scriptPath.Replace('\\', '/');

                        //<script src="@scriptPath"></script>
                        builder.OpenElement(i++, "script");
                        builder.AddAttribute(i++, "src", scriptPath);
                        builder.CloseElement();
                    }
                }
            }
        }
    }
}
