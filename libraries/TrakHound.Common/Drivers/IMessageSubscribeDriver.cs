// Copyright (c) 2025 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Entities;
using TrakHound.Messages;

namespace TrakHound.Drivers
{
    public interface IMessageSubscribeDriver : ITrakHoundDriver
    {
        Task<TrakHoundEntityResponse<ITrakHoundConsumer<TrakHoundMessageResponse>>> Subscribe(string brokerId, string clientId, IEnumerable<string> topics, int qos);
    }
}
