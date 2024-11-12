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
    public partial class TrakHoundDefinitionWikiRouter : TrakHoundEntityRouter<ITrakHoundDefinitionWikiEntity>
    {
        public TrakHoundDefinitionWikiRouter(TrakHoundRouter router) : base(router) { }


        public async Task<TrakHoundResponse<ITrakHoundDefinitionWikiEntity>> QueryByDefinitionUuid(IEnumerable<string> definitionUuids, string requestId = null)
        {
            // Set Query Driver Function
            Func<ParameterRouteTargetDriverRequest<IDefinitionWikiQueryDriver, ITrakHoundDefinitionWikiEntity>, Task<TrakHoundResponse<ITrakHoundDefinitionWikiEntity>>> serviceFunction = async (serviceRequest) =>
            {
                return await serviceRequest.Driver.Query(serviceRequest.Request.Queries.Select(o => o.Query));
            };

            // Set Query Router Function
            Func<ParameterRouteTargetRouterRequest<ITrakHoundDefinitionWikiEntity>, Task<TrakHoundResponse<ITrakHoundDefinitionWikiEntity>>> routerFunction = async (routerRequest) =>
            {
                return await routerRequest.Router.Entities.Definitions.Wikis.QueryByDefinitionUuid(routerRequest.Request.Queries.Select(o => o.Query), routerRequest.Request.Id);
            };

            var request = new EntityParameterRouteRequest<ITrakHoundDefinitionWikiEntity>("Query", requestId, definitionUuids);

            // Run Targets
            var response = await ParameterRouteTarget.Run(
                Router.Id,
                request,
                Router.Logger,
                Router.GetTargets<IDefinitionWikiQueryDriver>(TrakHoundDefinitionRoutes.WikisQuery),
                serviceFunction,
                routerFunction
                );

            if (response.IsSuccess)
            {
                // Process Options
                await ProcessOptions(requestId, response.Options);
            }

            return HandleResponse(request.Id, response.Results, response.Duration);
        }
    }
}
