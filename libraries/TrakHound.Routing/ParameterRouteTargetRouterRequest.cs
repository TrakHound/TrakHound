// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Entities;

namespace TrakHound.Routing
{
    class ParameterRouteTargetRouterRequest<TEntity>
    {
        public IRouteTarget Target { get; set; }

        public TrakHoundRouter Router { get; set; }

        public IParameterRouteRequest Request { get; set; }


        public ParameterRouteTargetRouterRequest(IRouteTarget target, TrakHoundRouter router, IParameterRouteRequest request)
        {
            Target = target;
            Router = router;
            Request = request;
        }
    }
}
