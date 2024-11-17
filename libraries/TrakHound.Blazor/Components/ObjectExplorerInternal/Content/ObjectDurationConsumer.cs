// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Clients;
using TrakHound.Entities;

namespace TrakHound.Blazor.Components.ObjectExplorerInternal
{
    public class ObjectDurationConsumer : IContentConsumer
    {
        private readonly ITrakHoundClient _client;
        private readonly string _consumerId;
        private ITrakHoundConsumer<IEnumerable<ITrakHoundObjectDurationEntity>> _consumer;


        public event ContentHandler ValueUpdated;


        public ObjectDurationConsumer(ITrakHoundClient client, string consumerId)
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
                var objectUuids = objects.Select(o => o.Uuid).Distinct();

                var entities = await _client.System.Entities.Objects.Duration.QueryByObjectUuid(objectUuids);
                if (!entities.IsNullOrEmpty())
                {
                    foreach (var entity in entities)
                    {
                        UpdateValue(entity);
                    }
                }

                await Subscribe(objectUuids);
            }
        }

        private async Task Subscribe(IEnumerable<string> objectUuids)
        {
            if (_client != null)
            {
                _consumer = await _client.System.Entities.Objects.Duration.SubscribeByObjectUuid(objectUuids, consumerId: _consumerId);
                if (_consumer != null)
                {
                    _consumer.Received += OnReceived;
                }
            }
        }

        private void OnReceived(object sender, IEnumerable<ITrakHoundObjectDurationEntity> entities)
        {
            foreach (var entity in entities)
            {
                UpdateValue(entity);
            }
        }

        private void UpdateValue(ITrakHoundObjectDurationEntity entity)
        {
            if (entity != null && entity.ObjectUuid != null)
            {
                if (ValueUpdated != null) ValueUpdated.Invoke(entity.ObjectUuid, entity, entity.Value.ToString(), entity.Created);
            }
        }
    }
}
