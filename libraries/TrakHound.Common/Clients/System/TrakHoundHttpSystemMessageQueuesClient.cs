// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.IO;
using System.Threading.Tasks;
using TrakHound.Http;
using TrakHound.MessageQueues;

namespace TrakHound.Clients
{
    internal class TrakHoundHttpSystemMessageQueuesClient : ITrakHoundSystemMessageQueuesClient
    {
        private readonly TrakHoundHttpClient _baseClient;


        public string BaseUrl => _baseClient != null ? _baseClient.HttpBaseUrl : null;

        public string RouterId => _baseClient != null ? _baseClient.RouterId : null;


        public TrakHoundHttpSystemMessageQueuesClient(TrakHoundHttpClient baseClient)
        {
            _baseClient = baseClient;
        }


        public async Task<TrakHoundMessageQueueResponse> Pull(string queue, bool acknowledge, string routerId = null)
        {
            var url = Url.Combine(BaseUrl, HttpConstants.MessageQueuesPrefix);
            url = Url.Combine(url, "pull");
            url = Url.AddQueryParameter(url, "acknowledge", acknowledge.ToString());
            url = Url.AddQueryParameter(url, "routerId", _baseClient.GetRouterId(routerId));

            var httpResponse = await RestRequest.GetResponse(url);
            return TrakHoundHttpMessageQueueResponse.Parse(httpResponse.Content);
        }

        public async Task<ITrakHoundConsumer<TrakHoundMessageQueueResponse>> Subscribe(string queue, bool acknowledge, string routerId = null)
        {
            var url = HttpConstants.MessageQueuesPrefix;
            url = Url.Combine(url, "subscribe");
            url = Url.AddQueryParameter(url, "requestBody", "true");
            url = Url.AddQueryParameter(url, "acknowledge", acknowledge.ToString());
            url = Url.AddQueryParameter(url, "routerId", _baseClient.GetRouterId(routerId));

            var consumer = new TrakHoundMessageQueueClientConsumer(_baseClient.ClientConfiguration, url, queue);
            consumer.Subscribe();
            return consumer;
        }

        public async Task<bool> Publish(string queue, Stream content, string routerId = null)
        {
            var url = Url.Combine(BaseUrl, HttpConstants.MessageQueuesPrefix);
            url = Url.Combine(url, "publish");
            url = Url.AddQueryParameter(url, "queue", queue);
            url = Url.AddQueryParameter(url, "routerId", _baseClient.GetRouterId(routerId));

            var httpResponse = await RestRequest.PostResponse(url, content);
            return httpResponse.StatusCode >= 200 && httpResponse.StatusCode < 300;
        }

        public async Task<bool> Acknowledge(string queue, string deliveryId, string routerId = null)
        {
            var url = Url.Combine(BaseUrl, HttpConstants.MessageQueuesPrefix);
            url = Url.Combine(url, "acknowledge");
            url = Url.AddQueryParameter(url, "deliveryId", deliveryId);
            url = Url.AddQueryParameter(url, "routerId", _baseClient.GetRouterId(routerId));

            var httpResponse = await RestRequest.PutResponse(url);
            return httpResponse.StatusCode >= 200 && httpResponse.StatusCode < 300;
        }

        public async Task<bool> Reject(string queue, string deliveryId, string routerId = null)
        {
            var url = Url.Combine(BaseUrl, HttpConstants.MessageQueuesPrefix);
            url = Url.Combine(url, "reject");
            url = Url.AddQueryParameter(url, "deliveryId", deliveryId);
            url = Url.AddQueryParameter(url, "routerId", _baseClient.GetRouterId(routerId));

            var httpResponse = await RestRequest.PutResponse(url);
            return httpResponse.StatusCode >= 200 && httpResponse.StatusCode < 300;
        }
    }
}
