// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace TrakHound.Entities.Collections
{
    public partial class TrakHoundObjectCollection
    {
        private readonly Dictionary<string, ITrakHoundObjectStateEntity> _states = new Dictionary<string, ITrakHoundObjectStateEntity>();

        private readonly ListDictionary<string, string> _statesByObjectUuid = new ListDictionary<string, string>(); // ObjectUuid => Uuid



        public IEnumerable<ITrakHoundObjectStateEntity> States => _states.Values;
        public void AddState(ITrakHoundObjectStateEntity entity)
        {
            if (entity != null)
            {
                var x = _states.GetValueOrDefault(entity.Uuid);
                if (x == null || !ObjectExtensions.ByteArraysEqual(x.Hash, entity.Hash))
                {
                    _states.Remove(entity.Uuid);
                    _states.Add(entity.Uuid, new TrakHoundObjectStateEntity(entity));


                    _statesByObjectUuid.Add(entity.ObjectUuid, entity.Uuid);

                }
            }
        }

        public void AddStates(IEnumerable<ITrakHoundObjectStateEntity> entities)
        {
            if (!entities.IsNullOrEmpty())
            {
                foreach (var entity in entities)
                {
                    if (entity.Uuid != null)
                    {                    
                        var x = _states.GetValueOrDefault(entity.Uuid);
                        if (x == null || !ObjectExtensions.ByteArraysEqual(x.Hash, entity.Hash))
                        {
                            _states.Remove(entity.Uuid);
                            _states.Add(entity.Uuid, new TrakHoundObjectStateEntity(entity));


                            _statesByObjectUuid.Add(entity.ObjectUuid, entity.Uuid);
                        }                   
                    }
                }
            }
        }


        public ITrakHoundObjectStateEntity GetState(string uuid)
        {
            if (!string.IsNullOrEmpty(uuid))
            {
                return _states.GetValueOrDefault(uuid);
            }

            return null;
        }

        public IEnumerable<ITrakHoundObjectStateEntity> GetStates(IEnumerable<string> uuids)
        {
            if (!uuids.IsNullOrEmpty())
            {
                var entities = new List<ITrakHoundObjectStateEntity>();
                foreach (var uuid in uuids)
                {
                    var entity = _states.GetValueOrDefault(uuid);
                    if (entity != null) entities.Add(entity);
                }
                return entities;
            }

            return null;
        }




        public IEnumerable<ITrakHoundObjectStateEntity> QueryStatesByObjectUuid(string objectUuid)
        {
            if (!string.IsNullOrEmpty(objectUuid))
            {
                var uuids = _statesByObjectUuid.Get(objectUuid);
                if (!uuids.IsNullOrEmpty())
                {
                    var entities = new List<ITrakHoundObjectStateEntity>();
                    foreach (var uuid in uuids)
                    {
                        var entity = _states.GetValueOrDefault(uuid);
                        if (entity != null) entities.Add(entity);
                    }
                    return entities;
                }
            }

            return null;
        }




        public IEnumerable<object[]> GetStateArrays()
        {
            var arrays = new List<object[]>();
            foreach (var entity in _states.Values) arrays.Add(TrakHoundEntity.ToArray(entity));
            return !arrays.IsNullOrEmpty() ? arrays : null;
        }
    }
}
