// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace TrakHound.Entities.Collections
{
    public partial class TrakHoundObjectCollection
    {
        private readonly Dictionary<string, ITrakHoundObjectMessageEntity> _messages = new Dictionary<string, ITrakHoundObjectMessageEntity>();

        private readonly ListDictionary<string, string> _messagesByObjectUuid = new ListDictionary<string, string>(); // ObjectUuid => Uuid



        public IEnumerable<ITrakHoundObjectMessageEntity> Messages => _messages.Values;


        public void AddMessage(ITrakHoundObjectMessageEntity entity)
        {
            if (entity != null)
            {
                var x = _messages.GetValueOrDefault(entity.Uuid);
                if (x == null || !ObjectExtensions.ByteArraysEqual(x.Hash, entity.Hash))
                {
                    _messages.Remove(entity.Uuid);
                    _messages.Add(entity.Uuid, new TrakHoundObjectMessageEntity(entity));


                    _messagesByObjectUuid.Add(entity.ObjectUuid, entity.Uuid);

                }
            }
        }

        public void AddMessages(IEnumerable<ITrakHoundObjectMessageEntity> entities)
        {
            if (!entities.IsNullOrEmpty())
            {
                foreach (var entity in entities)
                {
                    if (entity.Uuid != null)
                    {                    
                        var x = _messages.GetValueOrDefault(entity.Uuid);
                        if (x == null || !ObjectExtensions.ByteArraysEqual(x.Hash, entity.Hash))
                        {
                            _messages.Remove(entity.Uuid);
                            _messages.Add(entity.Uuid, new TrakHoundObjectMessageEntity(entity));


                            _messagesByObjectUuid.Add(entity.ObjectUuid, entity.Uuid);
                        }                   
                    }
                }
            }
        }


        public ITrakHoundObjectMessageEntity GetMessage(string uuid)
        {
            if (!string.IsNullOrEmpty(uuid))
            {
                return _messages.GetValueOrDefault(uuid);
            }

            return null;
        }

        public IEnumerable<ITrakHoundObjectMessageEntity> GetMessages(IEnumerable<string> uuids)
        {
            if (!uuids.IsNullOrEmpty())
            {
                var entities = new List<ITrakHoundObjectMessageEntity>();
                foreach (var uuid in uuids)
                {
                    var entity = _messages.GetValueOrDefault(uuid);
                    if (entity != null) entities.Add(entity);
                }
                return entities;
            }

            return null;
        }




        public IEnumerable<ITrakHoundObjectMessageEntity> QueryMessagesByObjectUuid(string objectUuid)
        {
            if (!string.IsNullOrEmpty(objectUuid))
            {
                var uuids = _messagesByObjectUuid.Get(objectUuid);
                if (!uuids.IsNullOrEmpty())
                {
                    var entities = new List<ITrakHoundObjectMessageEntity>();
                    foreach (var uuid in uuids)
                    {
                        var entity = _messages.GetValueOrDefault(uuid);
                        if (entity != null) entities.Add(entity);
                    }
                    return entities;
                }
            }

            return null;
        }




        public IEnumerable<object[]> GetMessageArrays()
        {
            var arrays = new List<object[]>();
            foreach (var entity in _messages.Values) arrays.Add(TrakHoundEntity.ToArray(entity));
            return !arrays.IsNullOrEmpty() ? arrays : null;
        }
    }
}
