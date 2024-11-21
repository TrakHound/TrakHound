// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Clients;
using TrakHound.Entities;

namespace TrakHound.Blazor.Components.ObjectExplorerInternal
{
    public class ObjectStateConsumer : IContentConsumer
    {
        private readonly ITrakHoundClient _client;
        private readonly string _consumerId;
        private ITrakHoundConsumer<IEnumerable<ITrakHoundObjectStateEntity>> _consumer;


        public event ContentHandler ValueUpdated;


        public ObjectStateConsumer(ITrakHoundClient client, string consumerId)
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

                var entities = await _client.System.Entities.Objects.State.LatestByObjectUuid(objectUuids);
                if (!entities.IsNullOrEmpty())
                {
                    foreach (var entity in entities)
                    {
                        await UpdateValue(entity);
                    }
                }

                await Subscribe(objectUuids);
            }
        }

        private async Task Subscribe(IEnumerable<string> objectUuids)
        {
            if (_client != null)
            {
                _consumer = await _client.System.Entities.Objects.State.SubscribeByObjectUuid(objectUuids, consumerId: _consumerId);
                if (_consumer != null)
                {
                    _consumer.Received += OnReceived;
                }
            }
        }

        private async void OnReceived(object sender, IEnumerable<ITrakHoundObjectStateEntity> entities)
        {
            foreach (var entity in entities)
            {
                await UpdateValue(entity);
            }
        }

        private async Task UpdateValue(ITrakHoundObjectStateEntity entity)
        {
            if (entity != null && entity.ObjectUuid != null)
            {
                var definition = await _client.System.Entities.Definitions.ReadByUuid(entity.DefinitionUuid);
                if (definition != null)
                {
                    if (ValueUpdated != null) ValueUpdated.Invoke(entity.ObjectUuid, entity, definition.Type, entity.Created);
                }
            }
        }
    }
}
