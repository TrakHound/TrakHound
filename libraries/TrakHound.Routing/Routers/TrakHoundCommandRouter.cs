// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Commands;
using TrakHound.Drivers;

namespace TrakHound.Routing.Routers
{
    public class TrakHoundCommandRouter
    {
        private readonly TrakHoundRouter _router;


        public TrakHoundCommandRouter(TrakHoundRouter router)
        {
            _router = router;
        }


        public async Task<TrakHoundResponse<TrakHoundCommandResponse>> Run(string commandId, IReadOnlyDictionary<string, string> parameters = null, string requestId = null)
        {
            // Set Query Driver Command
            Func<ParameterRouteTargetDriverRequest<ICommandDriver, TrakHoundCommandResponse>, Task<TrakHoundResponse<TrakHoundCommandResponse>>> serviceCommand = async (serviceRequest) =>
            {
                return await serviceRequest.Driver.Run(commandId, parameters);
            };

            // Set Query Router Command
            Func<ParameterRouteTargetRouterRequest<TrakHoundCommandResponse>, Task<TrakHoundResponse<TrakHoundCommandResponse>>> routerCommand = async (routerRequest) =>
            {
                return await routerRequest.Router.Commands.Run(commandId, parameters, routerRequest.Request.Id);
            };


            var request = new ParameterRouteRequest("Run", requestId, commandId);

            // Run Targets
            var response = await ParameterRouteTarget.Run(
                _router.Id,
                request,
                _router.Logger,
                _router.GetTargets<ICommandDriver>(TrakHoundCommandRoutes.Run),
                serviceCommand,
                routerCommand
                );

            return new TrakHoundResponse<TrakHoundCommandResponse>(response.Results, response.Duration);
        }
    }
}
