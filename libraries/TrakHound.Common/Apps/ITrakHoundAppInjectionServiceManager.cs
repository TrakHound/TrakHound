// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;

namespace TrakHound.Apps
{
    public interface ITrakHoundAppInjectionServiceManager
    {
        IServiceProvider ServiceProvider { get; }


        object GetService(string scopeId, Type serviceType);

        object GetService<TServiceType>(string scopeId, Type serviceType) where TServiceType : ITrakHoundAppInjectionService;

        IEnumerable<TrakHoundAppInjectionServiceConfiguration> GetServiceConfigurations<TServiceType>() where TServiceType : ITrakHoundAppInjectionService;

        IEnumerable<TrakHoundAppInjectionServiceConfiguration> GetRequiredServiceConfigurations<TServiceType>() where TServiceType : ITrakHoundAppInjectionService;


        void RegisterService(string scopeId, object service);

        void RegisterService<TServiceType>(string scopeId, object service) where TServiceType : ITrakHoundAppInjectionService;


        void DisposeScope(string scopeId);
    }
}
