// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace TrakHound.Entities.Collections
{
    public partial class TrakHoundObjectCollection
    {
        private readonly Dictionary<string, ITrakHoundObjectDurationEntity> _durations = new Dictionary<string, ITrakHoundObjectDurationEntity>();

        private readonly Dictionary<string, string> _durationsByObjectUuid = new Dictionary<string, string>(); // ObjectUuid => Uuid



        public IEnumerable<ITrakHoundObjectDurationEntity> Durations => _durations.Values;


        public void AddDuration(ITrakHoundObjectDurationEntity entity)
        {
            if (entity != null)
            {
                var x = _durations.GetValueOrDefault(entity.Uuid);
                if (x == null || !ObjectExtensions.ByteArraysEqual(x.Hash, entity.Hash))
                {
                    _durations.Remove(entity.Uuid);
                    _durations.Add(entity.Uuid, new TrakHoundObjectDurationEntity(entity));


                    _durationsByObjectUuid.Remove(entity.ObjectUuid);
                    _durationsByObjectUuid.Add(entity.ObjectUuid, entity.Uuid);
                }
            }
        }

        public void AddDurations(IEnumerable<ITrakHoundObjectDurationEntity> entities)
        {
            if (!entities.IsNullOrEmpty())
            {
                foreach (var entity in entities)
                {
                    if (entity.Uuid != null)
                    {                    
                        var x = _durations.GetValueOrDefault(entity.Uuid);
                        if (x == null || !ObjectExtensions.ByteArraysEqual(x.Hash, entity.Hash))
                        {
                            _durations.Remove(entity.Uuid);
                            _durations.Add(entity.Uuid, new TrakHoundObjectDurationEntity(entity));


                            _durationsByObjectUuid.Add(entity.ObjectUuid, entity.Uuid);
                        }                   
                    }
                }
            }
        }


        public ITrakHoundObjectDurationEntity GetDuration(string uuid)
        {
            if (!string.IsNullOrEmpty(uuid))
            {
                return _durations.GetValueOrDefault(uuid);
            }

            return null;
        }

        public IEnumerable<ITrakHoundObjectDurationEntity> GetDurations(IEnumerable<string> uuids)
        {
            if (!uuids.IsNullOrEmpty())
            {
                var entities = new List<ITrakHoundObjectDurationEntity>();
                foreach (var uuid in uuids)
                {
                    var entity = _durations.GetValueOrDefault(uuid);
                    if (entity != null) entities.Add(entity);
                }
                return entities;
            }

            return null;
        }




        public ITrakHoundObjectDurationEntity QueryDurationByObjectUuid(string objectUuid)
        {
            if (!string.IsNullOrEmpty(objectUuid))
            {
                var uuid = _durationsByObjectUuid.GetValueOrDefault(objectUuid);
                if (!string.IsNullOrEmpty(uuid))
                {
                    return _durations.GetValueOrDefault(uuid);
                }
            }

            return null;
        }




        public IEnumerable<object[]> GetDurationArrays()
        {
            var arrays = new List<object[]>();
            foreach (var entity in _durations.Values) arrays.Add(TrakHoundEntity.ToArray(entity));
            return !arrays.IsNullOrEmpty() ? arrays : null;
        }
    }
}
