// Copyright (c) 2025 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Entities;
using TrakHound.Messages;

namespace TrakHound.Drivers
{
    public interface IMessageBrokerDriver : ITrakHoundDriver
    {
        Task<TrakHoundEntityResponse<TrakHoundMessageBroker>> QueryBrokers();

        Task<TrakHoundEntityResponse<TrakHoundMessageBroker>> QueryBrokersById(IEnumerable<string> brokerIds);
    }
}
