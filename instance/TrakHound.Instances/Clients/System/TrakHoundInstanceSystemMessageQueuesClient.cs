// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TrakHound.MessageQueues;

namespace TrakHound.Clients
{
    public class TrakHoundInstanceSystemMessageQueuesClient : ITrakHoundSystemMessageQueuesClient
    {
        private readonly TrakHoundInstanceClient _baseClient;


        public TrakHoundInstanceSystemMessageQueuesClient(TrakHoundInstanceClient baseClient)
        {
            _baseClient = baseClient;
        }


        public async Task<TrakHoundMessageQueueResponse> Pull(string queue, bool acknowledge, string routerId = null)
        {
            if (!string.IsNullOrEmpty(queue))
            {
                var router = _baseClient.GetRouter(routerId);
                if (router != null)
                {
                    var response = await router.MessageQueues.Pull(queue, acknowledge);
                    if (response.IsSuccess)
                    {
                        return response.Content.FirstOrDefault();
                    }
                }
            }

            return default;
        }

        public async Task<ITrakHoundConsumer<TrakHoundMessageQueueResponse>> Subscribe(string queue, bool acknowledge, string routerId = null)
        {
            if (!string.IsNullOrEmpty(queue))
            {
                var router = _baseClient.GetRouter(routerId);
                if (router != null)
                {
                    var response = await router.MessageQueues.Subscribe(queue, acknowledge);
                    if (response.IsSuccess)
                    {
                        return response.Content.FirstOrDefault();
                    }
                }
            }

            return default;
        }

        public async Task<bool> Publish(string queue, Stream content, string routerId = null)
        {
            if (!string.IsNullOrEmpty(queue))
            {
                var router = _baseClient.GetRouter(routerId);
                if (router != null)
                {
                    var response = await router.MessageQueues.Publish(queue, content);
                    if (response.IsSuccess)
                    {
                        return response.Content.FirstOrDefault();
                    }
                }
            }

            return default;
        }

        public async Task<bool> Acknowledge(string queue, string deliveryId, string routerId = null)
        {
            if (!string.IsNullOrEmpty(queue) && !string.IsNullOrEmpty(deliveryId))
            {
                var router = _baseClient.GetRouter(routerId);
                if (router != null)
                {
                    var response = await router.MessageQueues.Acknowledge(queue, deliveryId);
                    if (response.IsSuccess)
                    {
                        return response.Content.FirstOrDefault();
                    }
                }
            }

            return default;
        }

        public async Task<bool> Reject(string queue, string deliveryId, string routerId = null)
        {
            if (!string.IsNullOrEmpty(queue) && !string.IsNullOrEmpty(deliveryId))
            {
                var router = _baseClient.GetRouter(routerId);
                if (router != null)
                {
                    var response = await router.MessageQueues.Reject(queue, deliveryId);
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
