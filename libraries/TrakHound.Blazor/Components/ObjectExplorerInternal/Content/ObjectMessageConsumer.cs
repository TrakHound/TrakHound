// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Clients;
using TrakHound.Entities;
using TrakHound.Messages;

namespace TrakHound.Blazor.Components.ObjectExplorerInternal
{
    public class ObjectMessageConsumer : IContentConsumer
    {
        private const int _defaultQos = 0;

        private readonly ITrakHoundClient _client;
        private readonly string _consumerId;
        private readonly string _clientId = Guid.NewGuid().ToString();
        private ITrakHoundConsumer<TrakHoundMessageResponse> _consumer;
        private List<ITrakHoundObjectMessageEntity> _entities = new List<ITrakHoundObjectMessageEntity>(); // Topic => ObjectMessageEntity


        public event ContentHandler ValueUpdated;


        public ObjectMessageConsumer(ITrakHoundClient client, string consumerId)
        {
            _client = client;
            _consumerId = consumerId;
        }

        public void Dispose()
        {
            if (_consumer != null) _consumer.Dispose();
        }


        public async Task Load(IEnumerable<ITrakHoundObjectEntity> objects)
        {
            if (_client != null && !objects.IsNullOrEmpty())
            {
                var contentObjs = objects.Where(o => o.ContentType == TrakHoundObjectContentTypes.Message);
                if (!contentObjs.IsNullOrEmpty())
                {
                    var objectUuids = contentObjs.Select(o => o.Uuid).Distinct();

                    await Subscribe(objectUuids);
                }  
            }
        }

        private async Task Subscribe(IEnumerable<string> objectUuids)
        {
            if (_client != null)
            {
                _entities.Clear();

                var entities = await _client.System.Entities.Objects.Message.QueryByObjectUuid(objectUuids);
                if (!entities.IsNullOrEmpty())
                {
                    _entities.AddRange(entities);

                    var brokerIds = entities.Select(o => o.BrokerId).Distinct();
                    foreach (var brokerId in brokerIds)
                    {
                        var brokerEntities = entities.Where(o => o.BrokerId == brokerId);
                        var topics = brokerEntities.Select(o => o.Topic).Distinct();

                        _consumer = await _client.System.Messages.Subscribe(brokerId, _clientId, topics, _defaultQos);
                        if (_consumer != null)
                        {
                            OnReceived(null, _consumer.InitialValue);

                            _consumer.Received += OnReceived;
                        }
                    }
                }
            }
        }

        private void OnReceived(object sender, TrakHoundMessageResponse messageResponse)
        {
            if (!string.IsNullOrEmpty(messageResponse.Topic))
            {
                if (!_entities.IsNullOrEmpty())
                {
                    foreach (var entity in _entities)
                    {
                        if (entity.BrokerId == messageResponse.BrokerId)
                        {
                            var topicMatch = entity.Topic.TrimEnd('#');
                            if (messageResponse.Topic.StartsWith(topicMatch))
                            {
                                UpdateValue(entity, messageResponse);
                            }
                        }
                    }
                }
            }
        }

        private void UpdateValue(ITrakHoundObjectMessageEntity entity, TrakHoundMessageResponse messageResponse)
        {
            if (entity != null && entity.ObjectUuid != null)
            {            
                var content = messageResponse.GetContentUtf8String();

                if (ValueUpdated != null) ValueUpdated.Invoke(entity.ObjectUuid, entity, content, messageResponse.Timestamp);
            }
        }
    }
}
