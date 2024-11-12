// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TrakHound.Drivers;
using TrakHound.Messages;

namespace TrakHound.Routing.Routers
{
    public class TrakHoundMessageRouter
    {
        private readonly TrakHoundRouter _router;


        public TrakHoundMessageRouter(TrakHoundRouter router)
        {
            _router = router;
        }


        public async Task<TrakHoundResponse<TrakHoundMessageBroker>> QueryBrokers(string requestId = null)
        {
            // Set Query Driver Command
            Func<ParameterRouteTargetDriverRequest<IMessageBrokerDriver, TrakHoundMessageBroker>, Task<TrakHoundResponse<TrakHoundMessageBroker>>> serviceCommand = async (serviceRequest) =>
            {
                return await serviceRequest.Driver.QueryBrokers();
            };

            // Set Query Router Command
            Func<ParameterRouteTargetRouterRequest<TrakHoundMessageBroker>, Task<TrakHoundResponse<TrakHoundMessageBroker>>> routerCommand = async (routerRequest) =>
            {
                return await routerRequest.Router.Messages.QueryBrokers(routerRequest.Request.Id);
            };


            var request = new ParameterRouteRequest("Query Brokers", requestId);

            // Run Targets
            var response = await ParameterRouteTarget.Run(
                _router.Id,
                request,
                _router.Logger,
                _router.GetTargets<IMessageBrokerDriver>(TrakHoundMessageRoutes.BrokersQuery),
                serviceCommand,
                routerCommand
                );

            return new TrakHoundResponse<TrakHoundMessageBroker>(response.Results, response.Duration);
        }

        public async Task<TrakHoundResponse<TrakHoundMessageBroker>> QueryBrokersById(IEnumerable<string> brokerIds, string requestId = null)
        {
            // Set Query Driver Command
            Func<ParameterRouteTargetDriverRequest<IMessageBrokerDriver, TrakHoundMessageBroker>, Task<TrakHoundResponse<TrakHoundMessageBroker>>> serviceCommand = async (serviceRequest) =>
            {
                return await serviceRequest.Driver.QueryBrokersById(brokerIds);
            };

            // Set Query Router Command
            Func<ParameterRouteTargetRouterRequest<TrakHoundMessageBroker>, Task<TrakHoundResponse<TrakHoundMessageBroker>>> routerCommand = async (routerRequest) =>
            {
                return await routerRequest.Router.Messages.QueryBrokersById(brokerIds, routerRequest.Request.Id);
            };


            var request = new ParameterRouteRequest("Query Brokers", requestId, brokerIds);

            // Run Targets
            var response = await ParameterRouteTarget.Run(
                _router.Id,
                request,
                _router.Logger,
                _router.GetTargets<IMessageBrokerDriver>(TrakHoundMessageRoutes.BrokersQuery),
                serviceCommand,
                routerCommand
                );

            return new TrakHoundResponse<TrakHoundMessageBroker>(response.Results, response.Duration);
        }

        public async Task<TrakHoundResponse<TrakHoundMessageSender>> QuerySenders(string requestId = null)
        {
            // Set Query Driver Command
            Func<ParameterRouteTargetDriverRequest<IMessageSenderDriver, TrakHoundMessageSender>, Task<TrakHoundResponse<TrakHoundMessageSender>>> serviceCommand = async (serviceRequest) =>
            {
                return await serviceRequest.Driver.QuerySenders();
            };

            // Set Query Router Command
            Func<ParameterRouteTargetRouterRequest<TrakHoundMessageSender>, Task<TrakHoundResponse<TrakHoundMessageSender>>> routerCommand = async (routerRequest) =>
            {
                return await routerRequest.Router.Messages.QuerySenders(routerRequest.Request.Id);
            };


            var request = new ParameterRouteRequest("Query Senders", requestId);

            // Run Targets
            var response = await ParameterRouteTarget.Run(
                _router.Id,
                request,
                _router.Logger,
                _router.GetTargets<IMessageSenderDriver>(TrakHoundMessageRoutes.SendersQuery),
                serviceCommand,
                routerCommand
                );

            return new TrakHoundResponse<TrakHoundMessageSender>(response.Results, response.Duration);
        }

        public async Task<TrakHoundResponse<TrakHoundMessageSender>> QuerySendersById(IEnumerable<string> senderIds, string requestId = null)
        {
            // Set Query Driver Command
            Func<ParameterRouteTargetDriverRequest<IMessageSenderDriver, TrakHoundMessageSender>, Task<TrakHoundResponse<TrakHoundMessageSender>>> serviceCommand = async (serviceRequest) =>
            {
                return await serviceRequest.Driver.QuerySendersById(senderIds);
            };

            // Set Query Router Command
            Func<ParameterRouteTargetRouterRequest<TrakHoundMessageSender>, Task<TrakHoundResponse<TrakHoundMessageSender>>> routerCommand = async (routerRequest) =>
            {
                return await routerRequest.Router.Messages.QuerySendersById(senderIds, routerRequest.Request.Id);
            };


            var request = new ParameterRouteRequest("Query Senders", requestId, senderIds);

            // Run Targets
            var response = await ParameterRouteTarget.Run(
                _router.Id,
                request,
                _router.Logger,
                _router.GetTargets<IMessageSenderDriver>(TrakHoundMessageRoutes.SendersQuery),
                serviceCommand,
                routerCommand
                );

            return new TrakHoundResponse<TrakHoundMessageSender>(response.Results, response.Duration);
        }


        public async Task<TrakHoundResponse<ITrakHoundConsumer<TrakHoundMessageResponse>>> Subscribe(string brokerId, string clientId, IEnumerable<string> topics, int qos, string requestId = null)
        {
            // Set Query Driver Command
            Func<ParameterRouteTargetDriverRequest<IMessageSubscribeDriver, ITrakHoundConsumer<TrakHoundMessageResponse>>, Task<TrakHoundResponse<ITrakHoundConsumer<TrakHoundMessageResponse>>>> serviceCommand = async (serviceRequest) =>
            {
                return await serviceRequest.Driver.Subscribe(brokerId, clientId, topics, qos);
            };

            // Set Query Router Command
            Func<ParameterRouteTargetRouterRequest<ITrakHoundConsumer<TrakHoundMessageResponse>>, Task<TrakHoundResponse<ITrakHoundConsumer<TrakHoundMessageResponse>>>> routerCommand = async (routerRequest) =>
            {
                return await routerRequest.Router.Messages.Subscribe(brokerId, clientId, topics, qos, routerRequest.Request.Id);
            };


            var request = new ParameterRouteRequest("Subscribe", requestId, topics);

            // Run Targets
            var response = await ParameterRouteTarget.Run(
                _router.Id,
                request,
                _router.Logger,
                _router.GetTargets<IMessageSubscribeDriver>(TrakHoundMessageRoutes.Subscribe),
                serviceCommand,
                routerCommand
                );

            return new TrakHoundResponse<ITrakHoundConsumer<TrakHoundMessageResponse>>(response.Results, response.Duration);
        }

        public async Task<TrakHoundResponse<bool>> Publish(string brokerId, string topic, Stream content, bool retain, int qos, string requestId = null)
        {
            // Set Query Driver Command
            Func<ParameterRouteTargetDriverRequest<IMessagePublishDriver, bool>, Task<TrakHoundResponse<bool>>> serviceCommand = async (serviceRequest) =>
            {
                return await serviceRequest.Driver.Publish(brokerId, topic, content, retain, qos);
            };

            // Set Query Router Command
            Func<ParameterRouteTargetRouterRequest<bool>, Task<TrakHoundResponse<bool>>> routerCommand = async (routerRequest) =>
            {
                return await routerRequest.Router.Messages.Publish(brokerId, topic, content, retain, qos, routerRequest.Request.Id);
            };


            var request = new ParameterRouteRequest("Publish", requestId, topic);

            // Run Targets
            var response = await ParameterRouteTarget.Run(
                _router.Id,
                request,
                _router.Logger,
                _router.GetTargets<IMessagePublishDriver>(TrakHoundMessageRoutes.Publish),
                serviceCommand,
                routerCommand
                );

            return new TrakHoundResponse<bool>(response.Results, response.Duration);
        }
    }
}
