// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Clients;
using TrakHound.Entities;

namespace TrakHound.Blazor.Components.ObjectExplorerInternal
{
    public class ObjectAssignmentConsumer : IContentConsumer
    {
        private readonly ITrakHoundClient _client;
        private readonly string _consumerId;
        private ITrakHoundConsumer<IEnumerable<ITrakHoundObjectAssignmentEntity>> _consumer;


        public event ContentHandler ValueUpdated;


        public ObjectAssignmentConsumer(ITrakHoundClient client, string consumerId)
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
                var contentObjs = objects.Where(o => o.ContentType == TrakHoundObjectContentTypes.Assignment);
                if (!contentObjs.IsNullOrEmpty())
                {
                    var objectUuids = contentObjs.Select(o => o.Uuid).Distinct();

                    var entities = await _client.System.Entities.Objects.Assignment.CurrentByAssigneeUuid(objectUuids);
                    if (!entities.IsNullOrEmpty())
                    {
                        foreach (var entity in entities)
                        {
                            await UpdateValue(entity);
                        }
                    }
                    else
                    {
                        foreach (var objectUuid in objectUuids)
                        {
                            await UpdateValue(null);
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
                _consumer = await _client.System.Entities.Objects.Assignment.SubscribeByAssigneeUuid(objectUuids, consumerId: _consumerId);
                if (_consumer != null)
                {
                    if (!_consumer.InitialValue.IsNullOrEmpty())
                    {
                        foreach (var entity in _consumer.InitialValue)
                        {
                            await UpdateValue(entity);
                        }
                    }

                    _consumer.Received += OnReceived;
                }
            }
        }

        private async void OnReceived(object sender, IEnumerable<ITrakHoundObjectAssignmentEntity> entities)
        {
            foreach (var entity in entities)
            {
                await UpdateValue(entity);
            }
        }

        private async Task UpdateValue(ITrakHoundObjectAssignmentEntity entity)
        {
            if (entity != null && entity.AssigneeUuid != null)
            {
                var member = await _client.System.Entities.Objects.ReadByUuid(entity.MemberUuid);
                if (member != null)
                {
                    var value = entity.RemoveTimestamp > 0 ? null : member.Path;

                    if (ValueUpdated != null) ValueUpdated.Invoke(entity.AssigneeUuid, entity, value, entity.Created);
                }
            }
        }
    }
}
