// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.IO;
using System.Threading.Tasks;
using TrakHound.Drivers;

namespace TrakHound.Routing.Routers
{
    public class TrakHoundBlobRouter
    {
        private readonly TrakHoundRouter _router;


        public TrakHoundBlobRouter(TrakHoundRouter router)
        {
            _router = router;
        }


        public async Task<TrakHoundResponse<Stream>> Read(string blobId, string requestId = null)
        {
            // Set Query Driver Command
            Func<ParameterRouteTargetDriverRequest<IBlobReadDriver, Stream>, Task<TrakHoundResponse<Stream>>> serviceCommand = async (serviceRequest) =>
            {
                return await serviceRequest.Driver.Read(blobId);
            };

            // Set Query Router Command
            Func<ParameterRouteTargetRouterRequest<Stream>, Task<TrakHoundResponse<Stream>>> routerCommand = async (routerRequest) =>
            {
                return await routerRequest.Router.Blobs.Read(blobId, routerRequest.Request.Id);
            };


            var request = new ParameterRouteRequest("Read", requestId, blobId);

            // Run Targets
            var response = await ParameterRouteTarget.Run(
                _router.Id,
                request,
                _router.Logger,
                _router.GetTargets<IBlobReadDriver>(TrakHoundBlobRoutes.Read),
                serviceCommand,
                routerCommand
                );

            return new TrakHoundResponse<Stream>(response.Results, response.Duration);
        }

        public async Task<TrakHoundResponse<bool>> Publish(string blobId, Stream content, string requestId = null)
        {
            // Set Query Driver Command
            Func<ParameterRouteTargetDriverRequest<IBlobPublishDriver, bool>, Task<TrakHoundResponse<bool>>> serviceCommand = async (serviceRequest) =>
            {
                return await serviceRequest.Driver.Publish(blobId, content);
            };

            // Set Query Router Command
            Func<ParameterRouteTargetRouterRequest<bool>, Task<TrakHoundResponse<bool>>> routerCommand = async (routerRequest) =>
            {
                return await routerRequest.Router.Blobs.Publish(blobId, content, routerRequest.Request.Id);
            };


            var request = new ParameterRouteRequest("Publish", requestId, blobId);

            // Run Targets
            var response = await ParameterRouteTarget.Run(
                _router.Id,
                request,
                _router.Logger,
                _router.GetTargets<IBlobPublishDriver>(TrakHoundBlobRoutes.Publish),
                serviceCommand,
                routerCommand
                );

            return new TrakHoundResponse<bool>(response.Results, response.Duration);
        }

        public async Task<TrakHoundResponse<bool>> Delete(string blobId, string requestId = null)
        {
            // Set Query Driver Command
            Func<ParameterRouteTargetDriverRequest<IBlobDeleteDriver, bool>, Task<TrakHoundResponse<bool>>> serviceCommand = async (serviceRequest) =>
            {
                return await serviceRequest.Driver.Delete(blobId);
            };

            // Set Query Router Command
            Func<ParameterRouteTargetRouterRequest<bool>, Task<TrakHoundResponse<bool>>> routerCommand = async (routerRequest) =>
            {
                return await routerRequest.Router.Blobs.Delete(blobId, routerRequest.Request.Id);
            };


            var request = new ParameterRouteRequest("Delete", requestId, blobId);

            // Run Targets
            var response = await ParameterRouteTarget.Run(
                _router.Id,
                request,
                _router.Logger,
                _router.GetTargets<IBlobDeleteDriver>(TrakHoundBlobRoutes.Delete),
                serviceCommand,
                routerCommand
                );

            return new TrakHoundResponse<bool>(response.Results, response.Duration);
        }
    }
}
