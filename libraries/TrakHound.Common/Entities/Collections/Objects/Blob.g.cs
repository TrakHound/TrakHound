// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace TrakHound.Entities.Collections
{
    public partial class TrakHoundObjectCollection
    {
        private readonly Dictionary<string, ITrakHoundObjectBlobEntity> _blobs = new Dictionary<string, ITrakHoundObjectBlobEntity>();

        private readonly Dictionary<string, string> _blobsByObjectUuid = new Dictionary<string, string>(); // ObjectUuid => Uuid



        public IEnumerable<ITrakHoundObjectBlobEntity> Blobs => _blobs.Values;
        public void AddBlob(ITrakHoundObjectBlobEntity entity)
        {
            if (entity != null)
            {
                var x = _blobs.GetValueOrDefault(entity.Uuid);
                if (x == null || !ObjectExtensions.ByteArraysEqual(x.Hash, entity.Hash))
                {
                    _blobs.Remove(entity.Uuid);
                    _blobs.Add(entity.Uuid, new TrakHoundObjectBlobEntity(entity));


                    _blobsByObjectUuid.Remove(entity.ObjectUuid);
                    _blobsByObjectUuid.Add(entity.ObjectUuid, entity.Uuid);
                }
            }
        }

        public void AddBlobs(IEnumerable<ITrakHoundObjectBlobEntity> entities)
        {
            if (!entities.IsNullOrEmpty())
            {
                foreach (var entity in entities)
                {
                    if (entity.Uuid != null)
                    {                    
                        var x = _blobs.GetValueOrDefault(entity.Uuid);
                        if (x == null || !ObjectExtensions.ByteArraysEqual(x.Hash, entity.Hash))
                        {
                            _blobs.Remove(entity.Uuid);
                            _blobs.Add(entity.Uuid, new TrakHoundObjectBlobEntity(entity));


                            _blobsByObjectUuid.Add(entity.ObjectUuid, entity.Uuid);
                        }                   
                    }
                }
            }
        }


        public ITrakHoundObjectBlobEntity GetBlob(string uuid)
        {
            if (!string.IsNullOrEmpty(uuid))
            {
                return _blobs.GetValueOrDefault(uuid);
            }

            return null;
        }

        public IEnumerable<ITrakHoundObjectBlobEntity> GetBlobs(IEnumerable<string> uuids)
        {
            if (!uuids.IsNullOrEmpty())
            {
                var entities = new List<ITrakHoundObjectBlobEntity>();
                foreach (var uuid in uuids)
                {
                    var entity = _blobs.GetValueOrDefault(uuid);
                    if (entity != null) entities.Add(entity);
                }
                return entities;
            }

            return null;
        }




        public ITrakHoundObjectBlobEntity QueryBlobByObjectUuid(string objectUuid)
        {
            if (!string.IsNullOrEmpty(objectUuid))
            {
                var uuid = _blobsByObjectUuid.GetValueOrDefault(objectUuid);
                if (!string.IsNullOrEmpty(uuid))
                {
                    return _blobs.GetValueOrDefault(uuid);
                }
            }

            return null;
        }




        public IEnumerable<object[]> GetBlobArrays()
        {
            var arrays = new List<object[]>();
            foreach (var entity in _blobs.Values) arrays.Add(TrakHoundEntity.ToArray(entity));
            return !arrays.IsNullOrEmpty() ? arrays : null;
        }
    }
}
