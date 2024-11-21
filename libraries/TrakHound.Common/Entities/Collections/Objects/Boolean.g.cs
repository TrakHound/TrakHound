// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace TrakHound.Entities.Collections
{
    public partial class TrakHoundObjectCollection
    {
        private readonly Dictionary<string, ITrakHoundObjectBooleanEntity> _booleans = new Dictionary<string, ITrakHoundObjectBooleanEntity>();

        private readonly Dictionary<string, string> _booleansByObjectUuid = new Dictionary<string, string>(); // ObjectUuid => Uuid



        public IEnumerable<ITrakHoundObjectBooleanEntity> Booleans => _booleans.Values;
        public void AddBoolean(ITrakHoundObjectBooleanEntity entity)
        {
            if (entity != null)
            {
                var x = _booleans.GetValueOrDefault(entity.Uuid);
                if (x == null || !ObjectExtensions.ByteArraysEqual(x.Hash, entity.Hash))
                {
                    _booleans.Remove(entity.Uuid);
                    _booleans.Add(entity.Uuid, new TrakHoundObjectBooleanEntity(entity));


                    _booleansByObjectUuid.Remove(entity.ObjectUuid);
                    _booleansByObjectUuid.Add(entity.ObjectUuid, entity.Uuid);
                }
            }
        }

        public void AddBooleans(IEnumerable<ITrakHoundObjectBooleanEntity> entities)
        {
            if (!entities.IsNullOrEmpty())
            {
                foreach (var entity in entities)
                {
                    if (entity.Uuid != null)
                    {                    
                        var x = _booleans.GetValueOrDefault(entity.Uuid);
                        if (x == null || !ObjectExtensions.ByteArraysEqual(x.Hash, entity.Hash))
                        {
                            _booleans.Remove(entity.Uuid);
                            _booleans.Add(entity.Uuid, new TrakHoundObjectBooleanEntity(entity));


                            _booleansByObjectUuid.Add(entity.ObjectUuid, entity.Uuid);
                        }                   
                    }
                }
            }
        }


        public ITrakHoundObjectBooleanEntity GetBoolean(string uuid)
        {
            if (!string.IsNullOrEmpty(uuid))
            {
                return _booleans.GetValueOrDefault(uuid);
            }

            return null;
        }

        public IEnumerable<ITrakHoundObjectBooleanEntity> GetBooleans(IEnumerable<string> uuids)
        {
            if (!uuids.IsNullOrEmpty())
            {
                var entities = new List<ITrakHoundObjectBooleanEntity>();
                foreach (var uuid in uuids)
                {
                    var entity = _booleans.GetValueOrDefault(uuid);
                    if (entity != null) entities.Add(entity);
                }
                return entities;
            }

            return null;
        }




        public ITrakHoundObjectBooleanEntity QueryBooleanByObjectUuid(string objectUuid)
        {
            if (!string.IsNullOrEmpty(objectUuid))
            {
                var uuid = _booleansByObjectUuid.GetValueOrDefault(objectUuid);
                if (!string.IsNullOrEmpty(uuid))
                {
                    return _booleans.GetValueOrDefault(uuid);
                }
            }

            return null;
        }




        public IEnumerable<object[]> GetBooleanArrays()
        {
            var arrays = new List<object[]>();
            foreach (var entity in _booleans.Values) arrays.Add(TrakHoundEntity.ToArray(entity));
            return !arrays.IsNullOrEmpty() ? arrays : null;
        }
    }
}
