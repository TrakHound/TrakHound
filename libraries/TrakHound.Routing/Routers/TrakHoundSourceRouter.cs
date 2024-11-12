// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrakHound.Clients;
using TrakHound.Drivers.Entities;
using TrakHound.Entities;

namespace TrakHound.Routing.Routers
{
    public class TrakHoundSourceRouter : TrakHoundEntityRouter<ITrakHoundSourceEntity>
    {
        private const int _storeQueueInterval = 1000;
        private const int _storeQueueTake = 10000;

        private readonly ITrakHoundClient _client;

        public TrakHoundSourceRouter(TrakHoundRouter router) : base(router) 
        {
            _client = router.Client;
        }


        public async Task<TrakHoundResponse<ITrakHoundSourceEntity>> QueryChain(IEnumerable<string> uuids, string requestId = null)
        {
            if (string.IsNullOrEmpty(requestId)) requestId = Guid.NewGuid().ToString();
            var stpw = System.Diagnostics.Stopwatch.StartNew();

            var uuidChainResponse = await QueryUuidChain(uuids, requestId);
            var uuidChain = uuidChainResponse.Content?.Select(o => o.Uuid);

            var entityResponse = await Read(uuidChain, requestId);

            stpw.Stop();
            return HandleResponse(requestId, entityResponse.Results, entityResponse.Duration);
        }

        public async Task<TrakHoundResponse<ITrakHoundSourceQueryResult>> QueryUuidChain(IEnumerable<string> uuids, string requestId = null)
        {
            // Set Read Driver Function
            Func<ParameterRouteTargetDriverRequest<ISourceQueryDriver, ITrakHoundSourceEntity>, Task<TrakHoundResponse<ITrakHoundSourceQueryResult>>> serviceFunction = async (serviceRequest) =>
            {
                return await serviceRequest.Driver.QueryUuidChain(uuids);
            };

            // Set Read Router Function
            Func<ParameterRouteTargetRouterRequest<ITrakHoundSourceEntity>, Task<TrakHoundResponse<ITrakHoundSourceQueryResult>>> routerFunction = async (routerRequest) =>
            {
                return await routerRequest.Router.Entities.Sources.Sources.QueryUuidChain(uuids, requestId);
            };

            var request = new EntityParameterRouteRequest<ITrakHoundSourceEntity>("Query Uuid Chain", requestId);

            // Run Router Targets
            var response = await ParameterRouteTarget.Run(
                Router.Id,
                request,
                Router.Logger,
                Router.GetTargets<ISourceQueryDriver>(TrakHoundSourceRoutes.SourcesQuery),
                serviceFunction,
                routerFunction
                );

            return HandleResponse(request.Id, response.Results, response.Duration);
        }
    }
}
