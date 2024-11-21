// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace TrakHound.Entities.Collections
{
    public partial class TrakHoundObjectCollection
    {
        private readonly Dictionary<string, ITrakHoundObjectMetadataEntity> _metadata = new Dictionary<string, ITrakHoundObjectMetadataEntity>();

        private readonly ListDictionary<string, string> _metadataByEntityUuid = new ListDictionary<string, string>(); // EntityUuid => Uuid



        public IEnumerable<ITrakHoundObjectMetadataEntity> Metadata => _metadata.Values;
        public void AddMetadata(ITrakHoundObjectMetadataEntity entity)
        {
            if (entity != null)
            {
                var x = _metadata.GetValueOrDefault(entity.Uuid);
                if (x == null || !ObjectExtensions.ByteArraysEqual(x.Hash, entity.Hash))
                {
                    _metadata.Remove(entity.Uuid);
                    _metadata.Add(entity.Uuid, new TrakHoundObjectMetadataEntity(entity));


                    _metadataByEntityUuid.Add(entity.EntityUuid, entity.Uuid);

                }
            }
        }

        public void AddMetadata(IEnumerable<ITrakHoundObjectMetadataEntity> entities)
        {
            if (!entities.IsNullOrEmpty())
            {
                foreach (var entity in entities)
                {
                    if (entity.Uuid != null)
                    {                    
                        var x = _metadata.GetValueOrDefault(entity.Uuid);
                        if (x == null || !ObjectExtensions.ByteArraysEqual(x.Hash, entity.Hash))
                        {
                            _metadata.Remove(entity.Uuid);
                            _metadata.Add(entity.Uuid, new TrakHoundObjectMetadataEntity(entity));


                            _metadataByEntityUuid.Add(entity.EntityUuid, entity.Uuid);
                        }                   
                    }
                }
            }
        }


        public ITrakHoundObjectMetadataEntity GetMetadata(string uuid)
        {
            if (!string.IsNullOrEmpty(uuid))
            {
                return _metadata.GetValueOrDefault(uuid);
            }

            return null;
        }

        public IEnumerable<ITrakHoundObjectMetadataEntity> GetMetadata(IEnumerable<string> uuids)
        {
            if (!uuids.IsNullOrEmpty())
            {
                var entities = new List<ITrakHoundObjectMetadataEntity>();
                foreach (var uuid in uuids)
                {
                    var entity = _metadata.GetValueOrDefault(uuid);
                    if (entity != null) entities.Add(entity);
                }
                return entities;
            }

            return null;
        }




        public IEnumerable<ITrakHoundObjectMetadataEntity> QueryMetadataByEntityUuid(string entityUuid)
        {
            if (!string.IsNullOrEmpty(entityUuid))
            {
                var uuids = _metadataByEntityUuid.Get(entityUuid);
                if (!uuids.IsNullOrEmpty())
                {
                    var entities = new List<ITrakHoundObjectMetadataEntity>();
                    foreach (var uuid in uuids)
                    {
                        var entity = _metadata.GetValueOrDefault(uuid);
                        if (entity != null) entities.Add(entity);
                    }
                    return entities;
                }
            }

            return null;
        }




        public IEnumerable<object[]> GetMetadataArrays()
        {
            var arrays = new List<object[]>();
            foreach (var entity in _metadata.Values) arrays.Add(TrakHoundEntity.ToArray(entity));
            return !arrays.IsNullOrEmpty() ? arrays : null;
        }
    }
}
