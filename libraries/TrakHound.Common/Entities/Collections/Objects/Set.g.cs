// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace TrakHound.Entities.Collections
{
    public partial class TrakHoundObjectCollection
    {
        private readonly Dictionary<string, ITrakHoundObjectSetEntity> _sets = new Dictionary<string, ITrakHoundObjectSetEntity>();

        private readonly ListDictionary<string, string> _setsByObjectUuid = new ListDictionary<string, string>(); // ObjectUuid => Uuid



        public IEnumerable<ITrakHoundObjectSetEntity> Sets => _sets.Values;
        public void AddSet(ITrakHoundObjectSetEntity entity)
        {
            if (entity != null)
            {
                var x = _sets.GetValueOrDefault(entity.Uuid);
                if (x == null || !ObjectExtensions.ByteArraysEqual(x.Hash, entity.Hash))
                {
                    _sets.Remove(entity.Uuid);
                    _sets.Add(entity.Uuid, new TrakHoundObjectSetEntity(entity));


                    _setsByObjectUuid.Add(entity.ObjectUuid, entity.Uuid);

                }
            }
        }

        public void AddSets(IEnumerable<ITrakHoundObjectSetEntity> entities)
        {
            if (!entities.IsNullOrEmpty())
            {
                foreach (var entity in entities)
                {
                    if (entity.Uuid != null)
                    {                    
                        var x = _sets.GetValueOrDefault(entity.Uuid);
                        if (x == null || !ObjectExtensions.ByteArraysEqual(x.Hash, entity.Hash))
                        {
                            _sets.Remove(entity.Uuid);
                            _sets.Add(entity.Uuid, new TrakHoundObjectSetEntity(entity));


                            _setsByObjectUuid.Add(entity.ObjectUuid, entity.Uuid);
                        }                   
                    }
                }
            }
        }


        public ITrakHoundObjectSetEntity GetSet(string uuid)
        {
            if (!string.IsNullOrEmpty(uuid))
            {
                return _sets.GetValueOrDefault(uuid);
            }

            return null;
        }

        public IEnumerable<ITrakHoundObjectSetEntity> GetSets(IEnumerable<string> uuids)
        {
            if (!uuids.IsNullOrEmpty())
            {
                var entities = new List<ITrakHoundObjectSetEntity>();
                foreach (var uuid in uuids)
                {
                    var entity = _sets.GetValueOrDefault(uuid);
                    if (entity != null) entities.Add(entity);
                }
                return entities;
            }

            return null;
        }




        public IEnumerable<ITrakHoundObjectSetEntity> QuerySetsByObjectUuid(string objectUuid)
        {
            if (!string.IsNullOrEmpty(objectUuid))
            {
                var uuids = _setsByObjectUuid.Get(objectUuid);
                if (!uuids.IsNullOrEmpty())
                {
                    var entities = new List<ITrakHoundObjectSetEntity>();
                    foreach (var uuid in uuids)
                    {
                        var entity = _sets.GetValueOrDefault(uuid);
                        if (entity != null) entities.Add(entity);
                    }
                    return entities;
                }
            }

            return null;
        }




        public IEnumerable<object[]> GetSetArrays()
        {
            var arrays = new List<object[]>();
            foreach (var entity in _sets.Values) arrays.Add(TrakHoundEntity.ToArray(entity));
            return !arrays.IsNullOrEmpty() ? arrays : null;
        }
    }
}
