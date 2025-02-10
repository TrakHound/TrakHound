// Copyright (c) 2025 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Threading.Tasks;
using TrakHound.Entities;
using TrakHound.MessageQueues;

namespace TrakHound.Drivers
{
    public interface IMessageQueuePullDriver : ITrakHoundDriver
    {
        public Task<TrakHoundEntityResponse<TrakHoundMessageQueueResponse>> Pull(string queue, int count, bool acknowledge);

        public Task<TrakHoundEntityResponse<bool>> Acknowledge(string queue, string deliveryId);

        public Task<TrakHoundEntityResponse<bool>> Reject(string queue, string deliveryId);
    }
}
