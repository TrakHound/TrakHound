// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using TrakHound.Apps;
using TrakHound.Blazor.Apps;
using TrakHound.Blazor.Routing;
using TrakHound.Instances;
using TrakHound.Modules;

namespace TrakHound.Blazor
{
    public static class ServiceCollectionExtensions
    {
        public static void AddTrakHoundHosting(this IServiceCollection services, ITrakHoundInstance instance)
        {
            instance.ConfigurationProfile.Load<TrakHoundAppConfiguration>(TrakHoundAppConfiguration.ConfigurationCategory);

            services.AddSingleton(instance.PackageManager);

            var contextId = Guid.NewGuid().ToString();
            var context = new TrakHoundModuleContext(contextId);
            services.AddSingleton<ITrakHoundModuleProvider>(instance.ModuleProvider);

            var injectionServiceManager = new TrakHoundAppInjectionServiceManager(instance.ConfigurationProfile, instance.PackageManager, instance.ModuleProvider, context, instance.ClientProvider, instance.VolumeProvider);
            services.AddSingleton<ITrakHoundAppInjectionServiceManager>(injectionServiceManager);

            services.AddTransient<TrakHoundTransientAppInjectionServices>();
            services.AddScoped<TrakHoundScopedAppInjectionServices>();
            services.AddSingleton<TrakHoundSingletonAppInjectionServices>();
            services.AddTransient<ITrakHoundAppInjectionServices, TrakHoundAppInjectionServices>();


            var routeManager = new TrakHoundPageRouteManager<ComponentBase>(instance.Configuration.BasePath, instance.ConfigurationProfile, instance.ModuleProvider, "app", context, instance.ClientProvider, instance.VolumeProvider);
            services.AddSingleton<ITrakHoundPageRouteManager>(routeManager);


            //var appService = new TrakHoundAppService(instance, instance.ConfigurationProfile, routeManager, instance.PackageManager, instance.ModuleProvider, context);
            var appService = new TrakHoundAppService(instance, instance.ConfigurationProfile, routeManager, instance.PackageManager, instance.ModuleProvider);
            services.AddSingleton<ITrakHoundAppProvider>(appService);
        }
    }
}
