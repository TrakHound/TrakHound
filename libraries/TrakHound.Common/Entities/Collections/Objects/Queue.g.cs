// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace TrakHound.Entities.Collections
{
    public partial class TrakHoundObjectCollection
    {
        private readonly Dictionary<string, ITrakHoundObjectQueueEntity> _queues = new Dictionary<string, ITrakHoundObjectQueueEntity>();

        private readonly ListDictionary<string, string> _queuesByQueueUuid = new ListDictionary<string, string>(); // QueueUuid => Uuid


        private readonly ListDictionary<string, string> _queuesByMemberUuid = new ListDictionary<string, string>(); // MemberUuid => Uuid



        public IEnumerable<ITrakHoundObjectQueueEntity> Queues => _queues.Values;


        public void AddQueue(ITrakHoundObjectQueueEntity entity)
        {
            if (entity != null)
            {
                var x = _queues.GetValueOrDefault(entity.Uuid);
                if (x == null || !ObjectExtensions.ByteArraysEqual(x.Hash, entity.Hash))
                {
                    _queues.Remove(entity.Uuid);
                    _queues.Add(entity.Uuid, new TrakHoundObjectQueueEntity(entity));


                    _queuesByQueueUuid.Add(entity.QueueUuid, entity.Uuid);

                    _queuesByMemberUuid.Add(entity.MemberUuid, entity.Uuid);

                }
            }
        }

        public void AddQueues(IEnumerable<ITrakHoundObjectQueueEntity> entities)
        {
            if (!entities.IsNullOrEmpty())
            {
                foreach (var entity in entities)
                {
                    if (entity.Uuid != null)
                    {                    
                        var x = _queues.GetValueOrDefault(entity.Uuid);
                        if (x == null || !ObjectExtensions.ByteArraysEqual(x.Hash, entity.Hash))
                        {
                            _queues.Remove(entity.Uuid);
                            _queues.Add(entity.Uuid, new TrakHoundObjectQueueEntity(entity));


                            _queuesByQueueUuid.Add(entity.QueueUuid, entity.Uuid);
                            _queuesByMemberUuid.Add(entity.MemberUuid, entity.Uuid);
                        }                   
                    }
                }
            }
        }


        public ITrakHoundObjectQueueEntity GetQueue(string uuid)
        {
            if (!string.IsNullOrEmpty(uuid))
            {
                return _queues.GetValueOrDefault(uuid);
            }

            return null;
        }

        public IEnumerable<ITrakHoundObjectQueueEntity> GetQueues(IEnumerable<string> uuids)
        {
            if (!uuids.IsNullOrEmpty())
            {
                var entities = new List<ITrakHoundObjectQueueEntity>();
                foreach (var uuid in uuids)
                {
                    var entity = _queues.GetValueOrDefault(uuid);
                    if (entity != null) entities.Add(entity);
                }
                return entities;
            }

            return null;
        }




        public IEnumerable<ITrakHoundObjectQueueEntity> QueryQueuesByQueueUuid(string queueUuid)
        {
            if (!string.IsNullOrEmpty(queueUuid))
            {
                var uuids = _queuesByQueueUuid.Get(queueUuid);
                if (!uuids.IsNullOrEmpty())
                {
                    var entities = new List<ITrakHoundObjectQueueEntity>();
                    foreach (var uuid in uuids)
                    {
                        var entity = _queues.GetValueOrDefault(uuid);
                        if (entity != null) entities.Add(entity);
                    }
                    return entities;
                }
            }

            return null;
        }





        public IEnumerable<ITrakHoundObjectQueueEntity> QueryQueuesByMemberUuid(string memberUuid)
        {
            if (!string.IsNullOrEmpty(memberUuid))
            {
                var uuids = _queuesByMemberUuid.Get(memberUuid);
                if (!uuids.IsNullOrEmpty())
                {
                    var entities = new List<ITrakHoundObjectQueueEntity>();
                    foreach (var uuid in uuids)
                    {
                        var entity = _queues.GetValueOrDefault(uuid);
                        if (entity != null) entities.Add(entity);
                    }
                    return entities;
                }
            }

            return null;
        }




        public IEnumerable<object[]> GetQueueArrays()
        {
            var arrays = new List<object[]>();
            foreach (var entity in _queues.Values) arrays.Add(TrakHoundEntity.ToArray(entity));
            return !arrays.IsNullOrEmpty() ? arrays : null;
        }
    }
}
