// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;

namespace TrakHound.Apps
{
    public abstract class TrakHoundAppInjectionServicesBase<TServiceBase> where TServiceBase : class, ITrakHoundAppInjectionService
    {
        private readonly string _scopeId;
        private readonly ITrakHoundAppInjectionServiceManager _serviceManager;


        public TrakHoundAppInjectionServicesBase(ITrakHoundAppInjectionServiceManager serviceManager)
        {
            _scopeId = Guid.NewGuid().ToString();
            _serviceManager = serviceManager;
        }

        public void Dispose()
        {
            _serviceManager.DisposeScope(_scopeId);
        }


        public TService Get<TService>(string appId) where TService : class, ITrakHoundAppInjectionService
        {
            return (TService)Get(appId, typeof(TService));
        }

        public object Get(string appId, Type requestedType)
        {
            if (!string.IsNullOrEmpty(appId))
            {
                return _serviceManager.GetService<TServiceBase>(_scopeId, requestedType);
            }

            return null;
        }
    }
}
