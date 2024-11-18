// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace TrakHound.Entities.Collections
{
    public partial class TrakHoundObjectCollection
    {
        private readonly Dictionary<string, ITrakHoundObjectEventEntity> _events = new Dictionary<string, ITrakHoundObjectEventEntity>();

        private readonly ListDictionary<string, string> _eventsByObjectUuid = new ListDictionary<string, string>(); // ObjectUuid => Uuid



        public IEnumerable<ITrakHoundObjectEventEntity> Events => _events.Values;
        public void AddEvent(ITrakHoundObjectEventEntity entity)
        {
            if (entity != null)
            {
                var x = _events.GetValueOrDefault(entity.Uuid);
                if (x == null || !ObjectExtensions.ByteArraysEqual(x.Hash, entity.Hash))
                {
                    _events.Remove(entity.Uuid);
                    _events.Add(entity.Uuid, new TrakHoundObjectEventEntity(entity));


                    _eventsByObjectUuid.Add(entity.ObjectUuid, entity.Uuid);

                }
            }
        }

        public void AddEvents(IEnumerable<ITrakHoundObjectEventEntity> entities)
        {
            if (!entities.IsNullOrEmpty())
            {
                foreach (var entity in entities)
                {
                    if (entity.Uuid != null)
                    {                    
                        var x = _events.GetValueOrDefault(entity.Uuid);
                        if (x == null || !ObjectExtensions.ByteArraysEqual(x.Hash, entity.Hash))
                        {
                            _events.Remove(entity.Uuid);
                            _events.Add(entity.Uuid, new TrakHoundObjectEventEntity(entity));


                            _eventsByObjectUuid.Add(entity.ObjectUuid, entity.Uuid);
                        }                   
                    }
                }
            }
        }


        public ITrakHoundObjectEventEntity GetEvent(string uuid)
        {
            if (!string.IsNullOrEmpty(uuid))
            {
                return _events.GetValueOrDefault(uuid);
            }

            return null;
        }

        public IEnumerable<ITrakHoundObjectEventEntity> GetEvents(IEnumerable<string> uuids)
        {
            if (!uuids.IsNullOrEmpty())
            {
                var entities = new List<ITrakHoundObjectEventEntity>();
                foreach (var uuid in uuids)
                {
                    var entity = _events.GetValueOrDefault(uuid);
                    if (entity != null) entities.Add(entity);
                }
                return entities;
            }

            return null;
        }




        public IEnumerable<ITrakHoundObjectEventEntity> QueryEventsByObjectUuid(string objectUuid)
        {
            if (!string.IsNullOrEmpty(objectUuid))
            {
                var uuids = _eventsByObjectUuid.Get(objectUuid);
                if (!uuids.IsNullOrEmpty())
                {
                    var entities = new List<ITrakHoundObjectEventEntity>();
                    foreach (var uuid in uuids)
                    {
                        var entity = _events.GetValueOrDefault(uuid);
                        if (entity != null) entities.Add(entity);
                    }
                    return entities;
                }
            }

            return null;
        }




        public IEnumerable<object[]> GetEventArrays()
        {
            var arrays = new List<object[]>();
            foreach (var entity in _events.Values) arrays.Add(TrakHoundEntity.ToArray(entity));
            return !arrays.IsNullOrEmpty() ? arrays : null;
        }
    }
}
