// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Drivers;
using TrakHound.Sequences;

namespace TrakHound.Routing.Routers
{
    public class TrakHoundSequenceRouter
    {
        private readonly TrakHoundRouter _router;


        public TrakHoundSequenceRouter(TrakHoundRouter router)
        {
            _router = router;
        }


        public async Task<TrakHoundResponse<TrakHoundSequenceResponse>> Read(IEnumerable<string> sequenceIds, string requestId = null)
        {
            // Set Query Driver Command
            Func<ParameterRouteTargetDriverRequest<ISequenceReadDriver, TrakHoundSequenceResponse>, Task<TrakHoundResponse<TrakHoundSequenceResponse>>> serviceCommand = async (serviceRequest) =>
            {
                return await serviceRequest.Driver.Read(sequenceIds);
            };

            // Set Query Router Command
            Func<ParameterRouteTargetRouterRequest<TrakHoundSequenceResponse>, Task<TrakHoundResponse<TrakHoundSequenceResponse>>> routerCommand = async (routerRequest) =>
            {
                return await routerRequest.Router.Sequences.Read(sequenceIds, routerRequest.Request.Id);
            };


            var request = new ParameterRouteRequest("Read", requestId, sequenceIds);

            // Run Targets
            var response = await ParameterRouteTarget.Run(
                _router.Id,
                request,
                _router.Logger,
                _router.GetTargets<ISequenceReadDriver>(TrakHoundSequenceRoutes.Read),
                serviceCommand,
                routerCommand
                );

            return new TrakHoundResponse<TrakHoundSequenceResponse>(response.Results, response.Duration);
        }

        public async Task<TrakHoundResponse<TrakHoundSequenceResponse>> Publish(string sequenceId, double value, long created, string requestId = null)
        {
            // Set Query Driver Command
            Func<ParameterRouteTargetDriverRequest<ISequencePublishDriver, TrakHoundSequenceResponse>, Task<TrakHoundResponse<TrakHoundSequenceResponse>>> serviceCommand = async (serviceRequest) =>
            {
                return await serviceRequest.Driver.Publish(sequenceId, value, created);
            };

            // Set Query Router Command
            Func<ParameterRouteTargetRouterRequest<TrakHoundSequenceResponse>, Task<TrakHoundResponse<TrakHoundSequenceResponse>>> routerCommand = async (routerRequest) =>
            {
                return await routerRequest.Router.Sequences.Publish(sequenceId, value, created, routerRequest.Request.Id);
            };


            var request = new ParameterRouteRequest("Publish", requestId, sequenceId);

            // Run Targets
            var response = await ParameterRouteTarget.Run(
                _router.Id,
                request,
                _router.Logger,
                _router.GetTargets<ISequencePublishDriver>(TrakHoundSequenceRoutes.Publish),
                serviceCommand,
                routerCommand
                );

            return new TrakHoundResponse<TrakHoundSequenceResponse>(response.Results, response.Duration);
        }

        public async Task<TrakHoundResponse<bool>> Delete(IEnumerable<string> sequenceIds, string requestId = null)
        {
            // Set Query Driver Command
            Func<ParameterRouteTargetDriverRequest<ISequenceDeleteDriver, bool>, Task<TrakHoundResponse<bool>>> serviceCommand = async (serviceRequest) =>
            {
                return await serviceRequest.Driver.Delete(sequenceIds);
            };

            // Set Query Router Command
            Func<ParameterRouteTargetRouterRequest<bool>, Task<TrakHoundResponse<bool>>> routerCommand = async (routerRequest) =>
            {
                return await routerRequest.Router.Sequences.Delete(sequenceIds, routerRequest.Request.Id);
            };


            var request = new ParameterRouteRequest("Delete", requestId, sequenceIds);

            // Run Targets
            var response = await ParameterRouteTarget.Run(
                _router.Id,
                request,
                _router.Logger,
                _router.GetTargets<ISequenceDeleteDriver>(TrakHoundSequenceRoutes.Delete),
                serviceCommand,
                routerCommand
                );

            return new TrakHoundResponse<bool>(response.Results, response.Duration);
        }
    }
}
