// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Clients;
using TrakHound.Entities;

namespace TrakHound.Blazor.Components.ObjectExplorerInternal
{
    public class ObjectReferenceConsumer : IContentConsumer
    {
        private readonly ITrakHoundClient _client;
        private readonly string _consumerId;
        private ITrakHoundConsumer<IEnumerable<ITrakHoundObjectReferenceEntity>> _consumer;


        public event ContentHandler ValueUpdated;


        public ObjectReferenceConsumer(ITrakHoundClient client, string consumerId)
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
                var contentObjs = objects.Where(o => o.ContentType == TrakHoundObjectContentTypes.Reference);
                if (!contentObjs.IsNullOrEmpty())
                {
                    var objectUuids = contentObjs.Select(o => o.Uuid).Distinct();

                    var entities = await _client.System.Entities.Objects.Reference.QueryByObjectUuid(objectUuids);
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
        }

        private async Task Subscribe(IEnumerable<string> objectUuids)
        {
            if (_client != null)
            {
                _consumer = await _client.System.Entities.Objects.Reference.SubscribeByObjectUuid(objectUuids, consumerId: _consumerId);
                if (_consumer != null)
                {
                    _consumer.Received += OnReceived;
                }
            }
        }

        private async void OnReceived(object sender, IEnumerable<ITrakHoundObjectReferenceEntity> entities)
        {
            foreach (var entity in entities)
            {
                await UpdateValue(entity);
            }
        }

        private async Task UpdateValue(ITrakHoundObjectReferenceEntity entity)
        {
            if (entity != null && entity.ObjectUuid != null)
            {
                var target = await _client.System.Entities.Objects.ReadByUuid(entity.TargetUuid);
                if (target != null)
                {
                    if (ValueUpdated != null) ValueUpdated.Invoke(entity.ObjectUuid, entity, target.Path, entity.Created);
                }
            }
        }
    }
}
