// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace TrakHound.Entities.Collections
{
    public partial class TrakHoundObjectCollection
    {
        private readonly Dictionary<string, ITrakHoundObjectTimestampEntity> _timestamps = new Dictionary<string, ITrakHoundObjectTimestampEntity>();

        private readonly Dictionary<string, string> _timestampsByObjectUuid = new Dictionary<string, string>(); // ObjectUuid => Uuid



        public IEnumerable<ITrakHoundObjectTimestampEntity> Timestamps => _timestamps.Values;


        public void AddTimestamp(ITrakHoundObjectTimestampEntity entity)
        {
            if (entity != null)
            {
                var x = _timestamps.GetValueOrDefault(entity.Uuid);
                if (x == null || !ObjectExtensions.ByteArraysEqual(x.Hash, entity.Hash))
                {
                    _timestamps.Remove(entity.Uuid);
                    _timestamps.Add(entity.Uuid, new TrakHoundObjectTimestampEntity(entity));


                    _timestampsByObjectUuid.Remove(entity.ObjectUuid);
                    _timestampsByObjectUuid.Add(entity.ObjectUuid, entity.Uuid);
                }
            }
        }

        public void AddTimestamps(IEnumerable<ITrakHoundObjectTimestampEntity> entities)
        {
            if (!entities.IsNullOrEmpty())
            {
                foreach (var entity in entities)
                {
                    if (entity.Uuid != null)
                    {                    
                        var x = _timestamps.GetValueOrDefault(entity.Uuid);
                        if (x == null || !ObjectExtensions.ByteArraysEqual(x.Hash, entity.Hash))
                        {
                            _timestamps.Remove(entity.Uuid);
                            _timestamps.Add(entity.Uuid, new TrakHoundObjectTimestampEntity(entity));


                            _timestampsByObjectUuid.Add(entity.ObjectUuid, entity.Uuid);
                        }                   
                    }
                }
            }
        }


        public ITrakHoundObjectTimestampEntity GetTimestamp(string uuid)
        {
            if (!string.IsNullOrEmpty(uuid))
            {
                return _timestamps.GetValueOrDefault(uuid);
            }

            return null;
        }

        public IEnumerable<ITrakHoundObjectTimestampEntity> GetTimestamps(IEnumerable<string> uuids)
        {
            if (!uuids.IsNullOrEmpty())
            {
                var entities = new List<ITrakHoundObjectTimestampEntity>();
                foreach (var uuid in uuids)
                {
                    var entity = _timestamps.GetValueOrDefault(uuid);
                    if (entity != null) entities.Add(entity);
                }
                return entities;
            }

            return null;
        }




        public ITrakHoundObjectTimestampEntity QueryTimestampByObjectUuid(string objectUuid)
        {
            if (!string.IsNullOrEmpty(objectUuid))
            {
                var uuid = _timestampsByObjectUuid.GetValueOrDefault(objectUuid);
                if (!string.IsNullOrEmpty(uuid))
                {
                    return _timestamps.GetValueOrDefault(uuid);
                }
            }

            return null;
        }




        public IEnumerable<object[]> GetTimestampArrays()
        {
            var arrays = new List<object[]>();
            foreach (var entity in _timestamps.Values) arrays.Add(TrakHoundEntity.ToArray(entity));
            return !arrays.IsNullOrEmpty() ? arrays : null;
        }
    }
}
