// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace TrakHound.Entities.Collections
{
    public partial class TrakHoundObjectCollection
    {
        private readonly Dictionary<string, ITrakHoundObjectMessageQueueEntity> _messageQueues = new Dictionary<string, ITrakHoundObjectMessageQueueEntity>();

        private readonly ListDictionary<string, string> _messageQueuesByObjectUuid = new ListDictionary<string, string>(); // ObjectUuid => Uuid



        public IEnumerable<ITrakHoundObjectMessageQueueEntity> MessageQueues => _messageQueues.Values;
        public void AddMessageQueue(ITrakHoundObjectMessageQueueEntity entity)
        {
            if (entity != null)
            {
                var x = _messageQueues.GetValueOrDefault(entity.Uuid);
                if (x == null || !ObjectExtensions.ByteArraysEqual(x.Hash, entity.Hash))
                {
                    _messageQueues.Remove(entity.Uuid);
                    _messageQueues.Add(entity.Uuid, new TrakHoundObjectMessageQueueEntity(entity));


                    _messageQueuesByObjectUuid.Add(entity.ObjectUuid, entity.Uuid);

                }
            }
        }

        public void AddMessageQueues(IEnumerable<ITrakHoundObjectMessageQueueEntity> entities)
        {
            if (!entities.IsNullOrEmpty())
            {
                foreach (var entity in entities)
                {
                    if (entity.Uuid != null)
                    {                    
                        var x = _messageQueues.GetValueOrDefault(entity.Uuid);
                        if (x == null || !ObjectExtensions.ByteArraysEqual(x.Hash, entity.Hash))
                        {
                            _messageQueues.Remove(entity.Uuid);
                            _messageQueues.Add(entity.Uuid, new TrakHoundObjectMessageQueueEntity(entity));


                            _messageQueuesByObjectUuid.Add(entity.ObjectUuid, entity.Uuid);
                        }                   
                    }
                }
            }
        }


        public ITrakHoundObjectMessageQueueEntity GetMessageQueue(string uuid)
        {
            if (!string.IsNullOrEmpty(uuid))
            {
                return _messageQueues.GetValueOrDefault(uuid);
            }

            return null;
        }

        public IEnumerable<ITrakHoundObjectMessageQueueEntity> GetMessageQueues(IEnumerable<string> uuids)
        {
            if (!uuids.IsNullOrEmpty())
            {
                var entities = new List<ITrakHoundObjectMessageQueueEntity>();
                foreach (var uuid in uuids)
                {
                    var entity = _messageQueues.GetValueOrDefault(uuid);
                    if (entity != null) entities.Add(entity);
                }
                return entities;
            }

            return null;
        }




        public IEnumerable<ITrakHoundObjectMessageQueueEntity> QueryMessageQueuesByObjectUuid(string objectUuid)
        {
            if (!string.IsNullOrEmpty(objectUuid))
            {
                var uuids = _messageQueuesByObjectUuid.Get(objectUuid);
                if (!uuids.IsNullOrEmpty())
                {
                    var entities = new List<ITrakHoundObjectMessageQueueEntity>();
                    foreach (var uuid in uuids)
                    {
                        var entity = _messageQueues.GetValueOrDefault(uuid);
                        if (entity != null) entities.Add(entity);
                    }
                    return entities;
                }
            }

            return null;
        }




        public IEnumerable<object[]> GetMessageQueueArrays()
        {
            var arrays = new List<object[]>();
            foreach (var entity in _messageQueues.Values) arrays.Add(TrakHoundEntity.ToArray(entity));
            return !arrays.IsNullOrEmpty() ? arrays : null;
        }
    }
}
