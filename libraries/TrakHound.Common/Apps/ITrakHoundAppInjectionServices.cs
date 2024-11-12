// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;

namespace TrakHound.Apps
{
    public interface ITrakHoundAppInjectionServices : IDisposable
    {
        TService Get<TService>(string appId) where TService : class, ITrakHoundAppInjectionService;

        object Get(string appId, Type serviceType);
    }
}
