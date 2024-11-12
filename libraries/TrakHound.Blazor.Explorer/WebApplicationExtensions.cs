// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using TrakHound.Blazor.ExplorerInternal.Services;
using TrakHound.Blazor.Services;

namespace TrakHound.Blazor
{
    public static class WebApplicationExtensions
    {
        public static void AddTrakHoundExplorer(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<ExplorerService>();
            builder.Services.AddScoped<ExplorerTreeViewService>();

            builder.Services.AddSingleton<TrakHoundDownloadService>();

            builder.Services.AddScoped<ContextMenuService>();
            builder.Services.AddScoped<JavascriptService>();
        }
    }
}
