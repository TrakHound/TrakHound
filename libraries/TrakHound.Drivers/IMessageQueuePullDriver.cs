// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Threading.Tasks;
using TrakHound.MessageQueues;

namespace TrakHound.Drivers
{
    public interface IMessageQueuePullDriver : ITrakHoundDriver
    {
        public Task<TrakHoundResponse<TrakHoundMessageQueueResponse>> Pull(string queue, bool acknowledge);

        public Task<TrakHoundResponse<bool>> Acknowledge(string queue, string deliveryId);

        public Task<TrakHoundResponse<bool>> Reject(string queue, string deliveryId);
    }
}
