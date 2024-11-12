// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace TrakHound.Entities.Collections
{
    public partial class TrakHoundObjectCollection
    {
        private readonly Dictionary<string, ITrakHoundObjectLogEntity> _logs = new Dictionary<string, ITrakHoundObjectLogEntity>();

        private readonly ListDictionary<string, string> _logsByObjectUuid = new ListDictionary<string, string>(); // ObjectUuid => Uuid



        public IEnumerable<ITrakHoundObjectLogEntity> Logs => _logs.Values;


        public void AddLog(ITrakHoundObjectLogEntity entity)
        {
            if (entity != null)
            {
                var x = _logs.GetValueOrDefault(entity.Uuid);
                if (x == null || !ObjectExtensions.ByteArraysEqual(x.Hash, entity.Hash))
                {
                    _logs.Remove(entity.Uuid);
                    _logs.Add(entity.Uuid, new TrakHoundObjectLogEntity(entity));


                    _logsByObjectUuid.Add(entity.ObjectUuid, entity.Uuid);

                }
            }
        }

        public void AddLogs(IEnumerable<ITrakHoundObjectLogEntity> entities)
        {
            if (!entities.IsNullOrEmpty())
            {
                foreach (var entity in entities)
                {
                    if (entity.Uuid != null)
                    {                    
                        var x = _logs.GetValueOrDefault(entity.Uuid);
                        if (x == null || !ObjectExtensions.ByteArraysEqual(x.Hash, entity.Hash))
                        {
                            _logs.Remove(entity.Uuid);
                            _logs.Add(entity.Uuid, new TrakHoundObjectLogEntity(entity));


                            _logsByObjectUuid.Add(entity.ObjectUuid, entity.Uuid);
                        }                   
                    }
                }
            }
        }


        public ITrakHoundObjectLogEntity GetLog(string uuid)
        {
            if (!string.IsNullOrEmpty(uuid))
            {
                return _logs.GetValueOrDefault(uuid);
            }

            return null;
        }

        public IEnumerable<ITrakHoundObjectLogEntity> GetLogs(IEnumerable<string> uuids)
        {
            if (!uuids.IsNullOrEmpty())
            {
                var entities = new List<ITrakHoundObjectLogEntity>();
                foreach (var uuid in uuids)
                {
                    var entity = _logs.GetValueOrDefault(uuid);
                    if (entity != null) entities.Add(entity);
                }
                return entities;
            }

            return null;
        }




        public IEnumerable<ITrakHoundObjectLogEntity> QueryLogsByObjectUuid(string objectUuid)
        {
            if (!string.IsNullOrEmpty(objectUuid))
            {
                var uuids = _logsByObjectUuid.Get(objectUuid);
                if (!uuids.IsNullOrEmpty())
                {
                    var entities = new List<ITrakHoundObjectLogEntity>();
                    foreach (var uuid in uuids)
                    {
                        var entity = _logs.GetValueOrDefault(uuid);
                        if (entity != null) entities.Add(entity);
                    }
                    return entities;
                }
            }

            return null;
        }




        public IEnumerable<object[]> GetLogArrays()
        {
            var arrays = new List<object[]>();
            foreach (var entity in _logs.Values) arrays.Add(TrakHoundEntity.ToArray(entity));
            return !arrays.IsNullOrEmpty() ? arrays : null;
        }
    }
}
