// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Sections;
using TrakHound.Blazor.Services;

namespace TrakHound.Blazor
{
    public class TrakHoundTheme : ComponentBase
    {
        [Inject]
        public TrakHoundThemeService Service { get; set; }

        [Parameter]
        public string Key { get; set; }


        protected override void OnInitialized()
        {
            Service.ThemeChanged += ServiceThemeChanged;
        }

        private async void ServiceThemeChanged(object sender, TrakHoundThemes e)
        {
            await InvokeAsync(StateHasChanged);
        }

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (Service != null) await Service.Load(Key);
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenComponent<SectionContent>(0);
            builder.AddComponentParameter(1, nameof(SectionContent.SectionName), TrakHoundThemeOutlet.ThemeSectionName);
            builder.AddComponentParameter(2, nameof(SectionContent.ChildContent), (RenderFragment)BuildStylesheetLink);
            builder.CloseComponent();
        }

        private void BuildStylesheetLink(RenderTreeBuilder builder)
        {
            if (Service != null)
            {
                var stylesheetPath = "";
                switch (Service.Theme)
                {
                    case TrakHoundThemes.Light: stylesheetPath = "_content/TrakHound.Blazor.Hosting/css/trakhound/trakhound-light.css"; break;
                    case TrakHoundThemes.Dark: stylesheetPath = "_content/TrakHound.Blazor.Hosting/css/trakhound/trakhound-dark.css"; break;
                }

                //<link rel="stylesheet" href="@stylesheetPath" />
                builder.OpenElement(0, "link");
                builder.AddAttribute(1, "rel", "stylesheet");
                builder.AddAttribute(2, "href", stylesheetPath);
                builder.CloseElement();
            }
        }
    }
}
