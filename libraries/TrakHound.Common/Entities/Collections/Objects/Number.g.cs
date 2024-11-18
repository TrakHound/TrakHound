// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace TrakHound.Entities.Collections
{
    public partial class TrakHoundObjectCollection
    {
        private readonly Dictionary<string, ITrakHoundObjectNumberEntity> _numbers = new Dictionary<string, ITrakHoundObjectNumberEntity>();

        private readonly Dictionary<string, string> _numbersByObjectUuid = new Dictionary<string, string>(); // ObjectUuid => Uuid



        public IEnumerable<ITrakHoundObjectNumberEntity> Numbers => _numbers.Values;
        public void AddNumber(ITrakHoundObjectNumberEntity entity)
        {
            if (entity != null)
            {
                var x = _numbers.GetValueOrDefault(entity.Uuid);
                if (x == null || !ObjectExtensions.ByteArraysEqual(x.Hash, entity.Hash))
                {
                    _numbers.Remove(entity.Uuid);
                    _numbers.Add(entity.Uuid, new TrakHoundObjectNumberEntity(entity));


                    _numbersByObjectUuid.Remove(entity.ObjectUuid);
                    _numbersByObjectUuid.Add(entity.ObjectUuid, entity.Uuid);
                }
            }
        }

        public void AddNumbers(IEnumerable<ITrakHoundObjectNumberEntity> entities)
        {
            if (!entities.IsNullOrEmpty())
            {
                foreach (var entity in entities)
                {
                    if (entity.Uuid != null)
                    {                    
                        var x = _numbers.GetValueOrDefault(entity.Uuid);
                        if (x == null || !ObjectExtensions.ByteArraysEqual(x.Hash, entity.Hash))
                        {
                            _numbers.Remove(entity.Uuid);
                            _numbers.Add(entity.Uuid, new TrakHoundObjectNumberEntity(entity));


                            _numbersByObjectUuid.Add(entity.ObjectUuid, entity.Uuid);
                        }                   
                    }
                }
            }
        }


        public ITrakHoundObjectNumberEntity GetNumber(string uuid)
        {
            if (!string.IsNullOrEmpty(uuid))
            {
                return _numbers.GetValueOrDefault(uuid);
            }

            return null;
        }

        public IEnumerable<ITrakHoundObjectNumberEntity> GetNumbers(IEnumerable<string> uuids)
        {
            if (!uuids.IsNullOrEmpty())
            {
                var entities = new List<ITrakHoundObjectNumberEntity>();
                foreach (var uuid in uuids)
                {
                    var entity = _numbers.GetValueOrDefault(uuid);
                    if (entity != null) entities.Add(entity);
                }
                return entities;
            }

            return null;
        }




        public ITrakHoundObjectNumberEntity QueryNumberByObjectUuid(string objectUuid)
        {
            if (!string.IsNullOrEmpty(objectUuid))
            {
                var uuid = _numbersByObjectUuid.GetValueOrDefault(objectUuid);
                if (!string.IsNullOrEmpty(uuid))
                {
                    return _numbers.GetValueOrDefault(uuid);
                }
            }

            return null;
        }




        public IEnumerable<object[]> GetNumberArrays()
        {
            var arrays = new List<object[]>();
            foreach (var entity in _numbers.Values) arrays.Add(TrakHoundEntity.ToArray(entity));
            return !arrays.IsNullOrEmpty() ? arrays : null;
        }
    }
}
