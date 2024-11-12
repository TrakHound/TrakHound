// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Threading.Tasks;
using TrakHound.MessageQueues;

namespace TrakHound.Drivers
{
    public interface IMessageQueueSubscribeDriver : ITrakHoundDriver
    {
        Task<TrakHoundResponse<ITrakHoundConsumer<TrakHoundMessageQueueResponse>>> Subscribe(string queue, bool acknowledge);
    }
}
