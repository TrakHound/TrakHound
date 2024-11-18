// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace TrakHound.Entities.Collections
{
    public partial class TrakHoundObjectCollection
    {
        private readonly Dictionary<string, ITrakHoundObjectObservationEntity> _observations = new Dictionary<string, ITrakHoundObjectObservationEntity>();

        private readonly ListDictionary<string, string> _observationsByObjectUuid = new ListDictionary<string, string>(); // ObjectUuid => Uuid



        public IEnumerable<ITrakHoundObjectObservationEntity> Observations => _observations.Values;
        public void AddObservation(ITrakHoundObjectObservationEntity entity)
        {
            if (entity != null)
            {
                var x = _observations.GetValueOrDefault(entity.Uuid);
                if (x == null || !ObjectExtensions.ByteArraysEqual(x.Hash, entity.Hash))
                {
                    _observations.Remove(entity.Uuid);
                    _observations.Add(entity.Uuid, new TrakHoundObjectObservationEntity(entity));


                    _observationsByObjectUuid.Add(entity.ObjectUuid, entity.Uuid);

                }
            }
        }

        public void AddObservations(IEnumerable<ITrakHoundObjectObservationEntity> entities)
        {
            if (!entities.IsNullOrEmpty())
            {
                foreach (var entity in entities)
                {
                    if (entity.Uuid != null)
                    {                    
                        var x = _observations.GetValueOrDefault(entity.Uuid);
                        if (x == null || !ObjectExtensions.ByteArraysEqual(x.Hash, entity.Hash))
                        {
                            _observations.Remove(entity.Uuid);
                            _observations.Add(entity.Uuid, new TrakHoundObjectObservationEntity(entity));


                            _observationsByObjectUuid.Add(entity.ObjectUuid, entity.Uuid);
                        }                   
                    }
                }
            }
        }


        public ITrakHoundObjectObservationEntity GetObservation(string uuid)
        {
            if (!string.IsNullOrEmpty(uuid))
            {
                return _observations.GetValueOrDefault(uuid);
            }

            return null;
        }

        public IEnumerable<ITrakHoundObjectObservationEntity> GetObservations(IEnumerable<string> uuids)
        {
            if (!uuids.IsNullOrEmpty())
            {
                var entities = new List<ITrakHoundObjectObservationEntity>();
                foreach (var uuid in uuids)
                {
                    var entity = _observations.GetValueOrDefault(uuid);
                    if (entity != null) entities.Add(entity);
                }
                return entities;
            }

            return null;
        }




        public IEnumerable<ITrakHoundObjectObservationEntity> QueryObservationsByObjectUuid(string objectUuid)
        {
            if (!string.IsNullOrEmpty(objectUuid))
            {
                var uuids = _observationsByObjectUuid.Get(objectUuid);
                if (!uuids.IsNullOrEmpty())
                {
                    var entities = new List<ITrakHoundObjectObservationEntity>();
                    foreach (var uuid in uuids)
                    {
                        var entity = _observations.GetValueOrDefault(uuid);
                        if (entity != null) entities.Add(entity);
                    }
                    return entities;
                }
            }

            return null;
        }




        public IEnumerable<object[]> GetObservationArrays()
        {
            var arrays = new List<object[]>();
            foreach (var entity in _observations.Values) arrays.Add(TrakHoundEntity.ToArray(entity));
            return !arrays.IsNullOrEmpty() ? arrays : null;
        }
    }
}
