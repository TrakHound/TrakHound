// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;

namespace TrakHound.Apps
{
    public class TrakHoundAppInjectionServices : ITrakHoundAppInjectionServices
    {
        private readonly TrakHoundTransientAppInjectionServices _transientServices;
        private readonly TrakHoundScopedAppInjectionServices _scopedServices;
        private readonly TrakHoundSingletonAppInjectionServices _singletonServices;


        public TrakHoundAppInjectionServices(TrakHoundTransientAppInjectionServices transientServices, TrakHoundScopedAppInjectionServices scopedServices, TrakHoundSingletonAppInjectionServices singletonServices)
        {
            _transientServices = transientServices;
            _scopedServices = scopedServices;
            _singletonServices = singletonServices;
        }

        public void Dispose()
        {
            _transientServices.Dispose();
            _scopedServices.Dispose();
            _singletonServices.Dispose();
        }


        public TService Get<TService>(string appId) where TService : class, ITrakHoundAppInjectionService
        {
            try
            {
                return (TService)Get(appId, typeof(TService));
            }
            catch { }

            return null;
        }

        public object Get(string appId, Type serviceType)
        {
            try
            {
                object service = null;

                if (typeof(ITrakHoundTransientAppInjectionService).IsAssignableFrom(serviceType)) service = _transientServices.Get(appId, serviceType);
                if (typeof(ITrakHoundScopedAppInjectionService).IsAssignableFrom(serviceType)) service = _scopedServices.Get(appId, serviceType);
                if (typeof(ITrakHoundSingletonAppInjectionService).IsAssignableFrom(serviceType)) service = _singletonServices.Get(appId, serviceType);

                return service;
            }
            catch { }

            return null;
        }
    }
}
