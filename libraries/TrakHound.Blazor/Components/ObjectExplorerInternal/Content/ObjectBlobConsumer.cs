// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Clients;
using TrakHound.Entities;

namespace TrakHound.Blazor.Components.ObjectExplorerInternal
{
    public class ObjectBlobConsumer : IContentConsumer
    {
        private readonly ITrakHoundClient _client;
        private readonly string _consumerId;
        private ITrakHoundConsumer<IEnumerable<ITrakHoundObjectBlobEntity>> _consumer;


        public event ContentHandler ValueUpdated;


        public ObjectBlobConsumer(ITrakHoundClient client, string consumerId)
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

                var entities = await _client.System.Entities.Objects.Blob.QueryByObjectUuid(objectUuids);
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
                _consumer = await _client.System.Entities.Objects.Blob.SubscribeByObjectUuid(objectUuids, consumerId: _consumerId);
                if (_consumer != null)
                {
                    if (!_consumer.InitialValue.IsNullOrEmpty())
                    {
                        foreach (var entity in _consumer.InitialValue)
                        {
                            UpdateValue(entity);
                        }
                    }

                    _consumer.Received += OnReceived;
                }
            }
        }

        private void OnReceived(object sender, IEnumerable<ITrakHoundObjectBlobEntity> entities)
        {
            foreach (var entity in entities)
            {
                UpdateValue(entity);
            }
        }

        private void UpdateValue(ITrakHoundObjectBlobEntity entity)
        {
            if (entity != null && entity.ObjectUuid != null)
            {
                var value = entity.Size.ToFileSize();

                if (ValueUpdated != null) ValueUpdated.Invoke(entity.ObjectUuid, entity, value, entity.Created);
            }
        }
    }
}
