// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace TrakHound.Entities.Collections
{
    public partial class TrakHoundObjectCollection
    {
        private readonly Dictionary<string, ITrakHoundObjectStatisticEntity> _statistics = new Dictionary<string, ITrakHoundObjectStatisticEntity>();

        private readonly ListDictionary<string, string> _statisticsByObjectUuid = new ListDictionary<string, string>(); // ObjectUuid => Uuid



        public IEnumerable<ITrakHoundObjectStatisticEntity> Statistics => _statistics.Values;


        public void AddStatistic(ITrakHoundObjectStatisticEntity entity)
        {
            if (entity != null)
            {
                var x = _statistics.GetValueOrDefault(entity.Uuid);
                if (x == null || !ObjectExtensions.ByteArraysEqual(x.Hash, entity.Hash))
                {
                    _statistics.Remove(entity.Uuid);
                    _statistics.Add(entity.Uuid, new TrakHoundObjectStatisticEntity(entity));


                    _statisticsByObjectUuid.Add(entity.ObjectUuid, entity.Uuid);

                }
            }
        }

        public void AddStatistics(IEnumerable<ITrakHoundObjectStatisticEntity> entities)
        {
            if (!entities.IsNullOrEmpty())
            {
                foreach (var entity in entities)
                {
                    if (entity.Uuid != null)
                    {                    
                        var x = _statistics.GetValueOrDefault(entity.Uuid);
                        if (x == null || !ObjectExtensions.ByteArraysEqual(x.Hash, entity.Hash))
                        {
                            _statistics.Remove(entity.Uuid);
                            _statistics.Add(entity.Uuid, new TrakHoundObjectStatisticEntity(entity));


                            _statisticsByObjectUuid.Add(entity.ObjectUuid, entity.Uuid);
                        }                   
                    }
                }
            }
        }


        public ITrakHoundObjectStatisticEntity GetStatistic(string uuid)
        {
            if (!string.IsNullOrEmpty(uuid))
            {
                return _statistics.GetValueOrDefault(uuid);
            }

            return null;
        }

        public IEnumerable<ITrakHoundObjectStatisticEntity> GetStatistics(IEnumerable<string> uuids)
        {
            if (!uuids.IsNullOrEmpty())
            {
                var entities = new List<ITrakHoundObjectStatisticEntity>();
                foreach (var uuid in uuids)
                {
                    var entity = _statistics.GetValueOrDefault(uuid);
                    if (entity != null) entities.Add(entity);
                }
                return entities;
            }

            return null;
        }




        public IEnumerable<ITrakHoundObjectStatisticEntity> QueryStatisticsByObjectUuid(string objectUuid)
        {
            if (!string.IsNullOrEmpty(objectUuid))
            {
                var uuids = _statisticsByObjectUuid.Get(objectUuid);
                if (!uuids.IsNullOrEmpty())
                {
                    var entities = new List<ITrakHoundObjectStatisticEntity>();
                    foreach (var uuid in uuids)
                    {
                        var entity = _statistics.GetValueOrDefault(uuid);
                        if (entity != null) entities.Add(entity);
                    }
                    return entities;
                }
            }

            return null;
        }




        public IEnumerable<object[]> GetStatisticArrays()
        {
            var arrays = new List<object[]>();
            foreach (var entity in _statistics.Values) arrays.Add(TrakHoundEntity.ToArray(entity));
            return !arrays.IsNullOrEmpty() ? arrays : null;
        }
    }
}
