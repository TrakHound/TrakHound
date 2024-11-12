// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TrakHound.Http;
using TrakHound.Messages;

namespace TrakHound.Clients
{
    internal class TrakHoundHttpSystemMessagesClient : ITrakHoundSystemMessagesClient
    {
        private readonly TrakHoundHttpClient _baseClient;


        public string BaseUrl => _baseClient != null ? _baseClient.HttpBaseUrl : null;

        public string RouterId => _baseClient != null ? _baseClient.RouterId : null;


        public TrakHoundHttpSystemMessagesClient(TrakHoundHttpClient baseClient)
        {
            _baseClient = baseClient;
        }


        public async Task<IEnumerable<TrakHoundMessageBroker>> QueryBrokers(string routerId = null)
        {
            var url = Url.Combine(BaseUrl, HttpConstants.MessagesPrefix);
            url = Url.Combine(url, "brokers");
            url = Url.AddQueryParameter(url, "routerId", _baseClient.GetRouterId(routerId));

            return await RestRequest.Get<IEnumerable<TrakHoundMessageBroker>>(url);
        }

        public async Task<IEnumerable<TrakHoundMessageBroker>> QueryBrokersById(IEnumerable<string> brokerIds, string routerId = null)
        {
            var url = Url.Combine(BaseUrl, HttpConstants.MessagesPrefix);
            url = Url.Combine(url, "brokers");
            url = Url.AddQueryParameter(url, "routerId", _baseClient.GetRouterId(routerId));

            return await RestRequest.Post<IEnumerable<TrakHoundMessageBroker>>(url, brokerIds);
        }

        public async Task<IEnumerable<TrakHoundMessageSender>> QuerySenders(string routerId = null)
        {
            var url = Url.Combine(BaseUrl, HttpConstants.MessagesPrefix);
            url = Url.Combine(url, "senders");
            url = Url.AddQueryParameter(url, "routerId", _baseClient.GetRouterId(routerId));

            return await RestRequest.Get<IEnumerable<TrakHoundMessageSender>>(url);
        }

        public async Task<IEnumerable<TrakHoundMessageSender>> QuerySendersById(IEnumerable<string> senderIds, string routerId = null)
        {
            var url = Url.Combine(BaseUrl, HttpConstants.MessagesPrefix);
            url = Url.Combine(url, "senders");
            url = Url.AddQueryParameter(url, "routerId", _baseClient.GetRouterId(routerId));

            return await RestRequest.Post<IEnumerable<TrakHoundMessageSender>>(url, senderIds);
        }


        public async Task<ITrakHoundConsumer<TrakHoundMessageResponse>> Subscribe(string brokerId, string clientId, IEnumerable<string> topics, int qos, string routerId = null)
        {
            var url = HttpConstants.MessagesPrefix;
            url = Url.Combine(url, "subscribe");
            url = Url.AddQueryParameter(url, "requestBody", "true");
            url = Url.AddQueryParameter(url, "brokerId", brokerId);
            url = Url.AddQueryParameter(url, "clientId", clientId);
            url = Url.AddQueryParameter(url, "qos", qos);
            url = Url.AddQueryParameter(url, "routerId", _baseClient.GetRouterId(routerId));

            var consumer = new TrakHoundMessageClientConsumer(_baseClient.ClientConfiguration, url, topics);
            consumer.Subscribe();
            return consumer;
        }

        public async Task<bool> Publish(string brokerId, string topic, Stream content, bool retain = false, int qos = 0, string routerId = null)
        {
            var url = Url.Combine(BaseUrl, HttpConstants.MessagesPrefix);
            url = Url.Combine(url, "publish");
            url = Url.AddQueryParameter(url, "brokerId", brokerId);
            url = Url.AddQueryParameter(url, "topic", topic);
            url = Url.AddQueryParameter(url, "retain", retain.ToString());
            url = Url.AddQueryParameter(url, "qos", qos.ToString());
            url = Url.AddQueryParameter(url, "routerId", _baseClient.GetRouterId(routerId));

            var httpResponse = await RestRequest.PostResponse(url, content);
            return httpResponse.StatusCode >= 200 && httpResponse.StatusCode < 300;
        }
    }
}
