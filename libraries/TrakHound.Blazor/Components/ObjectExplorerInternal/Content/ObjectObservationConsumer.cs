// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Clients;
using TrakHound.Entities;

namespace TrakHound.Blazor.Components.ObjectExplorerInternal
{
    public class ObjectObservationConsumer : IContentConsumer
    {
        private readonly ITrakHoundClient _client;
        private readonly string _consumerId;
        private ITrakHoundConsumer<IEnumerable<ITrakHoundObjectObservationEntity>> _consumer;
        //private readonly Dictionary<string, string> _referenceQueue = new Dictionary<string, string>();
        //private readonly DelayEvent _referenceDelay;
        private readonly object _lock = new object();


        public event ContentHandler ValueUpdated;


        public ObjectObservationConsumer(ITrakHoundClient client, string consumerId)
        {
            _client = client;
            _consumerId = consumerId;

            //_referenceDelay = new DelayEvent(1000);
            //_referenceDelay.Elapsed += ReferenceDelayElapsed;
        }

        public void Dispose()
        {
            if (_consumer != null) _consumer.Dispose();
            //_referenceDelay.Dispose();
        }


        public async Task Load(IEnumerable<ITrakHoundObjectEntity> objects)
        {
            if (_client != null && !objects.IsNullOrEmpty())
            {
                var objectUuids = objects.Select(o => o.Uuid).Distinct();

                var entities = await _client.System.Entities.Objects.Observation.LatestByObjectUuid(objectUuids);
                if (!entities.IsNullOrEmpty())
                {
                    foreach (var entity in entities)
                    {
                        UpdateValue(entity);
                    }
                }

                await Subscribe(objectUuids);




                //var contentObjs = objects.Where(o => o.ContentType == TrakHoundObjectContentTypes.Observation);
                //if (!contentObjs.IsNullOrEmpty())
                //{
                //    var objectUuids = contentObjs.Select(o => o.Uuid).Distinct();

                //    var entities = await _client.System.Entities.Objects.Observation.LatestByObjectUuid(objectUuids);
                //    if (!entities.IsNullOrEmpty())
                //    {
                //        foreach (var entity in entities)
                //        {
                //            UpdateValue(entity);
                //        }
                //    }

                //    //await Subscribe(objectUuids);
                //}  
            }
        }

        private async Task Subscribe(IEnumerable<string> objectUuids)
        {
            if (_client != null)
            {
                _consumer = await _client.System.Entities.Objects.Observation.SubscribeByObjectUuid(objectUuids, consumerId: _consumerId);
                if (_consumer != null)
                {
                    _consumer.Received += OnReceived;
                }
            }
        }

        private void OnReceived(object sender, IEnumerable<ITrakHoundObjectObservationEntity> entities)
        {
            foreach (var entity in entities)
            {
                UpdateValue(entity);
            }
        }

        private void UpdateValue(ITrakHoundObjectObservationEntity entity)
        {
            if (entity != null && entity.ObjectUuid != null)
            {
                if (ValueUpdated != null) ValueUpdated.Invoke(entity.ObjectUuid, entity, entity.Value, entity.Timestamp);
            }
        }


        //private async void ReferenceDelayElapsed(object sender, EventArgs e)
        //{
        //    Dictionary<string, string> referenceItems;
        //    lock (_lock)
        //    {
        //        referenceItems = _referenceQueue.ToDictionary(o => o.Key, o => o.Value);
        //        _referenceQueue.Clear();
        //    }

        //    if (_client != null && !referenceItems.IsNullOrEmpty())
        //    {
        //        var targetUuids = referenceItems.Values.Distinct();

        //        var targetObjects = await _client.System.Entities.Objects.ReadPartialModels(targetUuids);
        //        if (!targetObjects.IsNullOrEmpty())
        //        {
        //            var dTargetObjects = targetObjects.ToDictionary(o => o.Uuid);

        //            foreach (var referenceItem in referenceItems)
        //            {
        //                var targetObject = dTargetObjects.GetValueOrDefault(referenceItem.Value);
        //                if (targetObject != null)
        //                {
        //                    if (ValueUpdated != null) ValueUpdated.Invoke(referenceItem.Key, null, targetObject.Path);
        //                }
        //            }
        //        }
        //    }
        //}
    }
}
