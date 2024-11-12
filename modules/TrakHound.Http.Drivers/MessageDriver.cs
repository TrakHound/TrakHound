// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Drivers;
using TrakHound.Messages;

namespace TrakHound.Http.Drivers
{
    public class MessageDriver :
        HttpDriver,
        IMessageBrokerDriver,
        IMessageSenderDriver,
        IMessageSubscribeDriver,
        IMessagePublishDriver
    {
        public MessageDriver() { }

        public MessageDriver(ITrakHoundDriverConfiguration configuration) : base(configuration) { }


        public async Task<TrakHoundResponse<ITrakHoundConsumer<TrakHoundMessageResponse>>> Subscribe(string brokerId, string clientId, IEnumerable<string> topics, int qos)
        {
            var results = new List<TrakHoundResult<ITrakHoundConsumer<TrakHoundMessageResponse>>>();

            if (!string.IsNullOrEmpty(brokerId) && !topics.IsNullOrEmpty())
            {
                var consumer = await Client.System.Messages.Subscribe(brokerId, clientId, topics, qos);
                if (consumer != null)
                {
                    results.Add(new TrakHoundResult<ITrakHoundConsumer<TrakHoundMessageResponse>>(Id, brokerId, TrakHoundResultType.Ok, consumer));
                }
                else
                {
                    results.Add(new TrakHoundResult<ITrakHoundConsumer<TrakHoundMessageResponse>>(Id, brokerId, TrakHoundResultType.NotFound));
                }
            }
            else
            {
                results.Add(new TrakHoundResult<ITrakHoundConsumer<TrakHoundMessageResponse>>(Id, null, TrakHoundResultType.NotFound));
            }

            return new TrakHoundResponse<ITrakHoundConsumer<TrakHoundMessageResponse>>(results);
        }

        public async Task<TrakHoundResponse<bool>> Publish(string brokerId, string topic, Stream content, bool retain, int qos)
        {
            var results = new List<TrakHoundResult<bool>>();

            if (!string.IsNullOrEmpty(brokerId) && !string.IsNullOrEmpty(topic))
            {
                if (await Client.System.Messages.Publish(brokerId, topic, content, retain, qos))
                {
                    results.Add(new TrakHoundResult<bool>(Id, brokerId, TrakHoundResultType.Ok, true));
                }
                else
                {
                    results.Add(new TrakHoundResult<bool>(Id, brokerId, TrakHoundResultType.InternalError));
                }
            }
            else
            {
                results.Add(new TrakHoundResult<bool>(Id, null, TrakHoundResultType.NotFound));
            }

            return new TrakHoundResponse<bool>(results);
        }


        public async Task<TrakHoundResponse<TrakHoundMessageBroker>> QueryBrokers()
        {
            var results = new List<TrakHoundResult<TrakHoundMessageBroker>>();

            var brokers = await Client.System.Messages.QueryBrokers();
            if (!brokers.IsNullOrEmpty())
            {
                foreach (var broker in brokers)
                {
                    results.Add(new TrakHoundResult<TrakHoundMessageBroker>(Id, "All", TrakHoundResultType.Ok, broker));
                }
            }
            else
            {
                results.Add(new TrakHoundResult<TrakHoundMessageBroker>(Id, "All", TrakHoundResultType.NotFound));
            }

            return new TrakHoundResponse<TrakHoundMessageBroker>(results);
        }

        public async Task<TrakHoundResponse<TrakHoundMessageBroker>> QueryBrokersById(IEnumerable<string> brokerIds)
        {
            var results = new List<TrakHoundResult<TrakHoundMessageBroker>>();

            var brokers = await Client.System.Messages.QueryBrokersById(brokerIds);
            if (!brokers.IsNullOrEmpty())
            {
                foreach (var brokerId in brokerIds)
                {
                    var broker = brokers.FirstOrDefault(o => o.Id == brokerId);
                    if (broker != null)
                    {
                        results.Add(new TrakHoundResult<TrakHoundMessageBroker>(Id, brokerId, TrakHoundResultType.Ok, broker));
                    }
                    else
                    {
                        results.Add(new TrakHoundResult<TrakHoundMessageBroker>(Id, brokerId, TrakHoundResultType.NotFound));
                    }
                }
            }
            else
            {
                results.Add(new TrakHoundResult<TrakHoundMessageBroker>(Id, "All", TrakHoundResultType.NotFound));
            }

            return new TrakHoundResponse<TrakHoundMessageBroker>(results);
        }

        public async Task<TrakHoundResponse<TrakHoundMessageSender>> QuerySenders()
        {
            var results = new List<TrakHoundResult<TrakHoundMessageSender>>();

            var brokers = await Client.System.Messages.QuerySenders();
            if (!brokers.IsNullOrEmpty())
            {
                foreach (var broker in brokers)
                {
                    results.Add(new TrakHoundResult<TrakHoundMessageSender>(Id, "All", TrakHoundResultType.Ok, broker));
                }
            }
            else
            {
                results.Add(new TrakHoundResult<TrakHoundMessageSender>(Id, "All", TrakHoundResultType.NotFound));
            }

            return new TrakHoundResponse<TrakHoundMessageSender>(results);
        }

        public async Task<TrakHoundResponse<TrakHoundMessageSender>> QuerySendersById(IEnumerable<string> senderIds)
        {
            var results = new List<TrakHoundResult<TrakHoundMessageSender>>();

            var senders = await Client.System.Messages.QuerySendersById(senderIds);
            if (!senders.IsNullOrEmpty())
            {
                foreach (var senderId in senderIds)
                {
                    var broker = senders.FirstOrDefault(o => o.Id == senderId);
                    if (broker != null)
                    {
                        results.Add(new TrakHoundResult<TrakHoundMessageSender>(Id, senderId, TrakHoundResultType.Ok, broker));
                    }
                    else
                    {
                        results.Add(new TrakHoundResult<TrakHoundMessageSender>(Id, senderId, TrakHoundResultType.NotFound));
                    }
                }
            }
            else
            {
                results.Add(new TrakHoundResult<TrakHoundMessageSender>(Id, "All", TrakHoundResultType.NotFound));
            }

            return new TrakHoundResponse<TrakHoundMessageSender>(results);
        }
    }
}
