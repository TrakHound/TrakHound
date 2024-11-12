// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Apps
{
    public class TrakHoundSingletonAppInjectionServices : TrakHoundAppInjectionServicesBase<ITrakHoundSingletonAppInjectionService>
    {
        public TrakHoundSingletonAppInjectionServices(ITrakHoundAppInjectionServiceManager serviceManager) : base(serviceManager) { }
    }
}
