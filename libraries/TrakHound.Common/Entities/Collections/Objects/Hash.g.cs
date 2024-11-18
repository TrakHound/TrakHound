// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace TrakHound.Entities.Collections
{
    public partial class TrakHoundObjectCollection
    {
        private readonly Dictionary<string, ITrakHoundObjectHashEntity> _hashes = new Dictionary<string, ITrakHoundObjectHashEntity>();

        private readonly ListDictionary<string, string> _hashesByObjectUuid = new ListDictionary<string, string>(); // ObjectUuid => Uuid



        public IEnumerable<ITrakHoundObjectHashEntity> Hashes => _hashes.Values;
        public void AddHash(ITrakHoundObjectHashEntity entity)
        {
            if (entity != null)
            {
                var x = _hashes.GetValueOrDefault(entity.Uuid);
                if (x == null || !ObjectExtensions.ByteArraysEqual(x.Hash, entity.Hash))
                {
                    _hashes.Remove(entity.Uuid);
                    _hashes.Add(entity.Uuid, new TrakHoundObjectHashEntity(entity));


                    _hashesByObjectUuid.Add(entity.ObjectUuid, entity.Uuid);

                }
            }
        }

        public void AddHashes(IEnumerable<ITrakHoundObjectHashEntity> entities)
        {
            if (!entities.IsNullOrEmpty())
            {
                foreach (var entity in entities)
                {
                    if (entity.Uuid != null)
                    {                    
                        var x = _hashes.GetValueOrDefault(entity.Uuid);
                        if (x == null || !ObjectExtensions.ByteArraysEqual(x.Hash, entity.Hash))
                        {
                            _hashes.Remove(entity.Uuid);
                            _hashes.Add(entity.Uuid, new TrakHoundObjectHashEntity(entity));


                            _hashesByObjectUuid.Add(entity.ObjectUuid, entity.Uuid);
                        }                   
                    }
                }
            }
        }


        public ITrakHoundObjectHashEntity GetHash(string uuid)
        {
            if (!string.IsNullOrEmpty(uuid))
            {
                return _hashes.GetValueOrDefault(uuid);
            }

            return null;
        }

        public IEnumerable<ITrakHoundObjectHashEntity> GetHashes(IEnumerable<string> uuids)
        {
            if (!uuids.IsNullOrEmpty())
            {
                var entities = new List<ITrakHoundObjectHashEntity>();
                foreach (var uuid in uuids)
                {
                    var entity = _hashes.GetValueOrDefault(uuid);
                    if (entity != null) entities.Add(entity);
                }
                return entities;
            }

            return null;
        }




        public IEnumerable<ITrakHoundObjectHashEntity> QueryHashesByObjectUuid(string objectUuid)
        {
            if (!string.IsNullOrEmpty(objectUuid))
            {
                var uuids = _hashesByObjectUuid.Get(objectUuid);
                if (!uuids.IsNullOrEmpty())
                {
                    var entities = new List<ITrakHoundObjectHashEntity>();
                    foreach (var uuid in uuids)
                    {
                        var entity = _hashes.GetValueOrDefault(uuid);
                        if (entity != null) entities.Add(entity);
                    }
                    return entities;
                }
            }

            return null;
        }




        public IEnumerable<object[]> GetHashArrays()
        {
            var arrays = new List<object[]>();
            foreach (var entity in _hashes.Values) arrays.Add(TrakHoundEntity.ToArray(entity));
            return !arrays.IsNullOrEmpty() ? arrays : null;
        }
    }
}
