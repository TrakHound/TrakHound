// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace TrakHound.Entities.Collections
{
    public partial class TrakHoundSourceCollection
    {
        private readonly Dictionary<string, ITrakHoundSourceEntity> _sources = new Dictionary<string, ITrakHoundSourceEntity>();


        public IEnumerable<ITrakHoundSourceEntity> Sources => _sources.Values;
        public void AddSource(ITrakHoundSourceEntity entity)
        {
            if (entity != null)
            {
                var x = _sources.GetValueOrDefault(entity.Uuid);
                if (x == null || !ObjectExtensions.ByteArraysEqual(x.Hash, entity.Hash))
                {
                    _sources.Remove(entity.Uuid);
                    _sources.Add(entity.Uuid, new TrakHoundSourceEntity(entity));


                    OnAddSource(entity);
                }
            }
        }

        public void AddSources(IEnumerable<ITrakHoundSourceEntity> entities)
        {
            if (!entities.IsNullOrEmpty())
            {
                foreach (var entity in entities)
                {
                    if (entity.Uuid != null)
                    {                    
                        var x = _sources.GetValueOrDefault(entity.Uuid);
                        if (x == null || !ObjectExtensions.ByteArraysEqual(x.Hash, entity.Hash))
                        {
                            _sources.Remove(entity.Uuid);
                            _sources.Add(entity.Uuid, new TrakHoundSourceEntity(entity));


                            OnAddSource(entity);
                        }                   
                    }
                }
            }
        }


        public ITrakHoundSourceEntity GetSource(string uuid)
        {
            if (!string.IsNullOrEmpty(uuid))
            {
                return _sources.GetValueOrDefault(uuid);
            }

            return null;
        }

        public IEnumerable<ITrakHoundSourceEntity> GetSources(IEnumerable<string> uuids)
        {
            if (!uuids.IsNullOrEmpty())
            {
                var entities = new List<ITrakHoundSourceEntity>();
                foreach (var uuid in uuids)
                {
                    var entity = _sources.GetValueOrDefault(uuid);
                    if (entity != null) entities.Add(entity);
                }
                return entities;
            }

            return null;
        }



        public IEnumerable<object[]> GetSourceArrays()
        {
            var arrays = new List<object[]>();
            foreach (var entity in _sources.Values) arrays.Add(TrakHoundEntity.ToArray(entity));
            return !arrays.IsNullOrEmpty() ? arrays : null;
        }
    }
}
