// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TrakHound.Messages;

namespace TrakHound.Clients
{
    public interface ITrakHoundSystemMessagesClient
    {
        Task<IEnumerable<TrakHoundMessageBroker>> QueryBrokers(string routerId = null);

        Task<IEnumerable<TrakHoundMessageBroker>> QueryBrokersById(IEnumerable<string> brokerIds, string routerId = null);

        Task<IEnumerable<TrakHoundMessageSender>> QuerySenders(string routerId = null);

        Task<IEnumerable<TrakHoundMessageSender>> QuerySendersById(IEnumerable<string> senderIds, string routerId = null);


        Task<ITrakHoundConsumer<TrakHoundMessageResponse>> Subscribe(string brokerId, string clientId, IEnumerable<string> topics, int qos, string routerId = null);

        Task<bool> Publish(string brokerId, string topic, Stream content, bool retain = false, int qos = 0, string routerId = null);
    }
}
