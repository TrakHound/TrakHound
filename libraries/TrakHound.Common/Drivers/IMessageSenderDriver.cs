// Copyright (c) 2025 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Entities;
using TrakHound.Messages;

namespace TrakHound.Drivers
{
    public interface IMessageSenderDriver : ITrakHoundDriver
    {
        Task<TrakHoundEntityResponse<TrakHoundMessageSender>> QuerySenders();

        Task<TrakHoundEntityResponse<TrakHoundMessageSender>> QuerySendersById(IEnumerable<string> senderIds);
    }
}
