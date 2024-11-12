// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TrakHound.Messages;

namespace TrakHound.Clients
{
    public class TrakHoundInstanceSystemMessagesClient : ITrakHoundSystemMessagesClient
    {
        private readonly TrakHoundInstanceClient _baseClient;


        public TrakHoundInstanceSystemMessagesClient(TrakHoundInstanceClient baseClient)
        {
            _baseClient = baseClient;
        }


        public async Task<IEnumerable<TrakHoundMessageBroker>> QueryBrokers(string routerId = null)
        {
            var router = _baseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router.Messages.QueryBrokers();
                if (response.IsSuccess)
                {
                    return response.Content;
                }
            }

            return default;
        }

        public async Task<IEnumerable<TrakHoundMessageBroker>> QueryBrokersById(IEnumerable<string> brokerIds, string routerId = null)
        {
            if (!brokerIds.IsNullOrEmpty())
            {
                var router = _baseClient.GetRouter(routerId);
                if (router != null)
                {
                    var response = await router.Messages.QueryBrokersById(brokerIds);
                    if (response.IsSuccess)
                    {
                        return response.Content;
                    }
                }
            }

            return default;
        }

        public async Task<IEnumerable<TrakHoundMessageSender>> QuerySenders(string routerId = null)
        {
            var router = _baseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router.Messages.QuerySenders();
                if (response.IsSuccess)
                {
                    return response.Content;
                }
            }

            return default;
        }

        public async Task<IEnumerable<TrakHoundMessageSender>> QuerySendersById(IEnumerable<string> senderIds, string routerId = null)
        {
            if (!senderIds.IsNullOrEmpty())
            {
                var router = _baseClient.GetRouter(routerId);
                if (router != null)
                {
                    var response = await router.Messages.QuerySendersById(senderIds);
                    if (response.IsSuccess)
                    {
                        return response.Content;
                    }
                }
            }

            return default;
        }


        public async Task<ITrakHoundConsumer<TrakHoundMessageResponse>> Subscribe(string brokerId, string clientId, IEnumerable<string> topics, int qos, string routerId = null)
        {
            if (!topics.IsNullOrEmpty())
            {
                var router = _baseClient.GetRouter(routerId);
                if (router != null)
                {
                    var response = await router.Messages.Subscribe(brokerId, clientId, topics, qos);
                    if (response.IsSuccess)
                    {
                        return response.Content.FirstOrDefault();
                    }
                }
            }

            return default;
        }

        public async Task<bool> Publish(string brokerId, string topic, Stream content, bool retain = false, int qos = 0, string routerId = null)
        {
            if (!string.IsNullOrEmpty(topic))
            {
                var router = _baseClient.GetRouter(routerId);
                if (router != null)
                {
                    var response = await router.Messages.Publish(brokerId, topic, content, retain, qos);
                    if (response.IsSuccess)
                    {
                        return response.Content.FirstOrDefault();
                    }
                }
            }

            return default;
        }
    }
}
