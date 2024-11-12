// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Drivers;
using TrakHound.Entities;

namespace TrakHound.Routing
{
    class ParameterRouteTargetDriverRequest<TDriver, TEntity> where TDriver : ITrakHoundDriver
    {
        public IRouteTarget Target { get; set; }

        public TDriver Driver { get; set; }

        public IParameterRouteRequest Request { get; set; }


        public ParameterRouteTargetDriverRequest(IRouteTarget target, TDriver driver, IParameterRouteRequest request)
        {
            Target = target;
            Driver = driver;
            Request = request;
        }
    }
}
