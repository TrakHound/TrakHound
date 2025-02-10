// Copyright (c) 2025 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Threading.Tasks;
using TrakHound.Entities;
using TrakHound.MessageQueues;

namespace TrakHound.Drivers
{
    public interface IMessageQueueSubscribeDriver : ITrakHoundDriver
    {
        Task<TrakHoundEntityResponse<ITrakHoundConsumer<TrakHoundMessageQueueResponse>>> Subscribe(string queue, bool acknowledge);
    }
}
