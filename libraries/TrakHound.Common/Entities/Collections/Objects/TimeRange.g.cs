// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace TrakHound.Entities.Collections
{
    public partial class TrakHoundObjectCollection
    {
        private readonly Dictionary<string, ITrakHoundObjectTimeRangeEntity> _timeRanges = new Dictionary<string, ITrakHoundObjectTimeRangeEntity>();

        private readonly Dictionary<string, string> _timeRangesByObjectUuid = new Dictionary<string, string>(); // ObjectUuid => Uuid



        public IEnumerable<ITrakHoundObjectTimeRangeEntity> TimeRanges => _timeRanges.Values;


        public void AddTimeRange(ITrakHoundObjectTimeRangeEntity entity)
        {
            if (entity != null)
            {
                var x = _timeRanges.GetValueOrDefault(entity.Uuid);
                if (x == null || !ObjectExtensions.ByteArraysEqual(x.Hash, entity.Hash))
                {
                    _timeRanges.Remove(entity.Uuid);
                    _timeRanges.Add(entity.Uuid, new TrakHoundObjectTimeRangeEntity(entity));


                    _timeRangesByObjectUuid.Remove(entity.ObjectUuid);
                    _timeRangesByObjectUuid.Add(entity.ObjectUuid, entity.Uuid);
                }
            }
        }

        public void AddTimeRanges(IEnumerable<ITrakHoundObjectTimeRangeEntity> entities)
        {
            if (!entities.IsNullOrEmpty())
            {
                foreach (var entity in entities)
                {
                    if (entity.Uuid != null)
                    {                    
                        var x = _timeRanges.GetValueOrDefault(entity.Uuid);
                        if (x == null || !ObjectExtensions.ByteArraysEqual(x.Hash, entity.Hash))
                        {
                            _timeRanges.Remove(entity.Uuid);
                            _timeRanges.Add(entity.Uuid, new TrakHoundObjectTimeRangeEntity(entity));


                            _timeRangesByObjectUuid.Add(entity.ObjectUuid, entity.Uuid);
                        }                   
                    }
                }
            }
        }


        public ITrakHoundObjectTimeRangeEntity GetTimeRange(string uuid)
        {
            if (!string.IsNullOrEmpty(uuid))
            {
                return _timeRanges.GetValueOrDefault(uuid);
            }

            return null;
        }

        public IEnumerable<ITrakHoundObjectTimeRangeEntity> GetTimeRanges(IEnumerable<string> uuids)
        {
            if (!uuids.IsNullOrEmpty())
            {
                var entities = new List<ITrakHoundObjectTimeRangeEntity>();
                foreach (var uuid in uuids)
                {
                    var entity = _timeRanges.GetValueOrDefault(uuid);
                    if (entity != null) entities.Add(entity);
                }
                return entities;
            }

            return null;
        }




        public ITrakHoundObjectTimeRangeEntity QueryTimeRangeByObjectUuid(string objectUuid)
        {
            if (!string.IsNullOrEmpty(objectUuid))
            {
                var uuid = _timeRangesByObjectUuid.GetValueOrDefault(objectUuid);
                if (!string.IsNullOrEmpty(uuid))
                {
                    return _timeRanges.GetValueOrDefault(uuid);
                }
            }

            return null;
        }




        public IEnumerable<object[]> GetTimeRangeArrays()
        {
            var arrays = new List<object[]>();
            foreach (var entity in _timeRanges.Values) arrays.Add(TrakHoundEntity.ToArray(entity));
            return !arrays.IsNullOrEmpty() ? arrays : null;
        }
    }
}
