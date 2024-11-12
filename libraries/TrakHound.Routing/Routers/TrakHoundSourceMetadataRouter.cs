// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrakHound.Drivers.Entities;
using TrakHound.Entities;

namespace TrakHound.Routing.Routers
{
    public class TrakHoundSourceMetadataRouter : TrakHoundEntityRouter<ITrakHoundSourceMetadataEntity>
    {
        public TrakHoundSourceMetadataRouter(TrakHoundRouter router) : base(router) { }


        public async Task<TrakHoundResponse<ITrakHoundSourceMetadataEntity>> QueryBySourceUuid(IEnumerable<string> sourceUuids, string requestId = null)
        {
            // Set Query Driver Function
            Func<ParameterRouteTargetDriverRequest<ISourceMetadataQueryDriver, ITrakHoundSourceMetadataEntity>, Task<TrakHoundResponse<ITrakHoundSourceMetadataEntity>>> serviceFunction = async (serviceRequest) =>
            {
                return await serviceRequest.Driver.Query(serviceRequest.Request.Queries.Select(o => o.Query));
            };

            // Set Query Router Function
            Func<ParameterRouteTargetRouterRequest<ITrakHoundSourceMetadataEntity>, Task<TrakHoundResponse<ITrakHoundSourceMetadataEntity>>> routerFunction = async (routerRequest) =>
            {
                return await routerRequest.Router.Entities.Sources.Metadata.QueryBySourceUuid(routerRequest.Request.Queries.Select(o => o.Query), routerRequest.Request.Id);
            };

            var request = new EntityParameterRouteRequest<ITrakHoundSourceMetadataEntity>("Query", requestId, sourceUuids);

            // Run Targets
            var response = await ParameterRouteTarget.Run(
                Router.Id,
                request,
                Router.Logger,
                Router.GetTargets<ISourceMetadataQueryDriver>(TrakHoundSourceRoutes.MetadataQuery),
                serviceFunction,
                routerFunction
                );

            // Process Options
            await ProcessOptions(request.Id, response.Options);

            return HandleResponse(request.Id, response.Results, response.Duration);
        }
    }
}
