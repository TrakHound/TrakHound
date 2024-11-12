// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.IO;
using System.Threading.Tasks;
using TrakHound.MessageQueues;

namespace TrakHound.Clients
{
    public interface ITrakHoundSystemMessageQueuesClient
    {
        Task<TrakHoundMessageQueueResponse> Pull(string queue, bool acknowledge = true, string routerId = null);

        Task<ITrakHoundConsumer<TrakHoundMessageQueueResponse>> Subscribe(string queue, bool acknowledge, string routerId = null);

        Task<bool> Publish(string queue, Stream content, string routerId = null);

        Task<bool> Acknowledge(string queue, string deliveryId, string routerId = null);

        Task<bool> Reject(string queue, string deliveryId, string routerId = null);
    }
}
