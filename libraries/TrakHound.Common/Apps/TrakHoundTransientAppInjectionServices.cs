// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Apps
{
    public class TrakHoundTransientAppInjectionServices : TrakHoundAppInjectionServicesBase<ITrakHoundTransientAppInjectionService>
    {
        public TrakHoundTransientAppInjectionServices(ITrakHoundAppInjectionServiceManager serviceManager) : base(serviceManager) { }
    }
}
