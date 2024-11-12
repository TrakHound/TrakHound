// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.IO;
using System.Threading.Tasks;
using TrakHound.Drivers;
using TrakHound.MessageQueues;

namespace TrakHound.Routing.Routers
{
    public class TrakHoundMessageQueueRouter
    {
        private readonly TrakHoundRouter _router;


        public TrakHoundMessageQueueRouter(TrakHoundRouter router)
        {
            _router = router;
        }


        public async Task<TrakHoundResponse<TrakHoundMessageQueueResponse>> Pull(string queue, bool acknowledge, string requestId = null)
        {
            // Set Query Driver Command
            Func<ParameterRouteTargetDriverRequest<IMessageQueuePullDriver, TrakHoundMessageQueueResponse>, Task<TrakHoundResponse<TrakHoundMessageQueueResponse>>> serviceCommand = async (serviceRequest) =>
            {
                return await serviceRequest.Driver.Pull(queue, acknowledge);
            };

            // Set Query Router Command
            Func<ParameterRouteTargetRouterRequest<TrakHoundMessageQueueResponse>, Task<TrakHoundResponse<TrakHoundMessageQueueResponse>>> routerCommand = async (routerRequest) =>
            {
                return await routerRequest.Router.MessageQueues.Pull(queue, acknowledge, routerRequest.Request.Id);
            };


            var request = new ParameterRouteRequest("Pull", requestId, queue);

            // Run Targets
            var response = await ParameterRouteTarget.Run(
                _router.Id,
                request,
                _router.Logger,
                _router.GetTargets<IMessageQueuePullDriver>(TrakHoundMessageQueueRoutes.Pull),
                serviceCommand,
                routerCommand
                );

            return new TrakHoundResponse<TrakHoundMessageQueueResponse>(response.Results, response.Duration);
        }

        public async Task<TrakHoundResponse<ITrakHoundConsumer<TrakHoundMessageQueueResponse>>> Subscribe(string queue, bool acknowledge, string requestId = null)
        {
            // Set Query Driver Command
            Func<ParameterRouteTargetDriverRequest<IMessageQueueSubscribeDriver, ITrakHoundConsumer<TrakHoundMessageQueueResponse>>, Task<TrakHoundResponse<ITrakHoundConsumer<TrakHoundMessageQueueResponse>>>> serviceCommand = async (serviceRequest) =>
            {
                return await serviceRequest.Driver.Subscribe(queue, acknowledge);
            };

            // Set Query Router Command
            Func<ParameterRouteTargetRouterRequest<ITrakHoundConsumer<TrakHoundMessageQueueResponse>>, Task<TrakHoundResponse<ITrakHoundConsumer<TrakHoundMessageQueueResponse>>>> routerCommand = async (routerRequest) =>
            {
                return await routerRequest.Router.MessageQueues.Subscribe(queue, acknowledge, routerRequest.Request.Id);
            };


            var request = new ParameterRouteRequest("Subscribe", requestId, queue);

            // Run Targets
            var response = await ParameterRouteTarget.Run(
                _router.Id,
                request,
                _router.Logger,
                _router.GetTargets<IMessageQueueSubscribeDriver>(TrakHoundMessageQueueRoutes.Subscribe),
                serviceCommand,
                routerCommand
                );

            return new TrakHoundResponse<ITrakHoundConsumer<TrakHoundMessageQueueResponse>>(response.Results, response.Duration);
        }

        public async Task<TrakHoundResponse<bool>> Publish(string queue, Stream content, string requestId = null)
        {
            // Set Query Driver Command
            Func<ParameterRouteTargetDriverRequest<IMessageQueuePublishDriver, bool>, Task<TrakHoundResponse<bool>>> serviceCommand = async (serviceRequest) =>
            {
                return await serviceRequest.Driver.Publish(queue, content);
            };

            // Set Query Router Command
            Func<ParameterRouteTargetRouterRequest<bool>, Task<TrakHoundResponse<bool>>> routerCommand = async (routerRequest) =>
            {
                return await routerRequest.Router.MessageQueues.Publish(queue, content, routerRequest.Request.Id);
            };


            var request = new ParameterRouteRequest("Publish", requestId, queue);

            // Run Targets
            var response = await ParameterRouteTarget.Run(
                _router.Id,
                request,
                _router.Logger,
                _router.GetTargets<IMessageQueuePublishDriver>(TrakHoundMessageQueueRoutes.Publish),
                serviceCommand,
                routerCommand
                );

            return new TrakHoundResponse<bool>(response.Results, response.Duration);
        }

        public async Task<TrakHoundResponse<bool>> Acknowledge(string queue, string deliveryId, string requestId = null)
        {
            // Set Query Driver Command
            Func<ParameterRouteTargetDriverRequest<IMessageQueuePullDriver, bool>, Task<TrakHoundResponse<bool>>> serviceCommand = async (serviceRequest) =>
            {
                return await serviceRequest.Driver.Acknowledge(queue, deliveryId);
            };

            // Set Query Router Command
            Func<ParameterRouteTargetRouterRequest<bool>, Task<TrakHoundResponse<bool>>> routerCommand = async (routerRequest) =>
            {
                return await routerRequest.Router.MessageQueues.Acknowledge(queue, deliveryId, routerRequest.Request.Id);
            };


            var request = new ParameterRouteRequest("Acknowledge", requestId, queue);

            // Run Targets
            var response = await ParameterRouteTarget.Run(
                _router.Id,
                request,
                _router.Logger,
                _router.GetTargets<IMessageQueuePullDriver>(TrakHoundMessageQueueRoutes.Pull),
                serviceCommand,
                routerCommand
                );

            return new TrakHoundResponse<bool>(response.Results, response.Duration);
        }

        public async Task<TrakHoundResponse<bool>> Reject(string queue, string deliveryId, string requestId = null)
        {
            // Set Query Driver Command
            Func<ParameterRouteTargetDriverRequest<IMessageQueuePullDriver, bool>, Task<TrakHoundResponse<bool>>> serviceCommand = async (serviceRequest) =>
            {
                return await serviceRequest.Driver.Reject(queue, deliveryId);
            };

            // Set Query Router Command
            Func<ParameterRouteTargetRouterRequest<bool>, Task<TrakHoundResponse<bool>>> routerCommand = async (routerRequest) =>
            {
                return await routerRequest.Router.MessageQueues.Reject(queue, deliveryId, routerRequest.Request.Id);
            };


            var request = new ParameterRouteRequest("Reject", requestId, queue);

            // Run Targets
            var response = await ParameterRouteTarget.Run(
                _router.Id,
                request,
                _router.Logger,
                _router.GetTargets<IMessageQueuePullDriver>(TrakHoundMessageQueueRoutes.Pull),
                serviceCommand,
                routerCommand
                );

            return new TrakHoundResponse<bool>(response.Results, response.Duration);
        }
    }
}
