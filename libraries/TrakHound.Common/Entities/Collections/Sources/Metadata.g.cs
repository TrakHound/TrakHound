// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace TrakHound.Entities.Collections
{
    public partial class TrakHoundSourceCollection
    {
        private readonly Dictionary<string, ITrakHoundSourceMetadataEntity> _metadata = new Dictionary<string, ITrakHoundSourceMetadataEntity>();

        private readonly ListDictionary<string, string> _metadataBySourceUuid = new ListDictionary<string, string>(); // SourceUuid => Uuid



        public IEnumerable<ITrakHoundSourceMetadataEntity> Metadata => _metadata.Values;


        public void AddMetadata(ITrakHoundSourceMetadataEntity entity)
        {
            if (entity != null)
            {
                var x = _metadata.GetValueOrDefault(entity.Uuid);
                if (x == null || !ObjectExtensions.ByteArraysEqual(x.Hash, entity.Hash))
                {
                    _metadata.Remove(entity.Uuid);
                    _metadata.Add(entity.Uuid, new TrakHoundSourceMetadataEntity(entity));


                    _metadataBySourceUuid.Add(entity.SourceUuid, entity.Uuid);

                }
            }
        }

        public void AddMetadata(IEnumerable<ITrakHoundSourceMetadataEntity> entities)
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
                            _metadata.Add(entity.Uuid, new TrakHoundSourceMetadataEntity(entity));


                            _metadataBySourceUuid.Add(entity.SourceUuid, entity.Uuid);
                        }                   
                    }
                }
            }
        }


        public ITrakHoundSourceMetadataEntity GetMetadata(string uuid)
        {
            if (!string.IsNullOrEmpty(uuid))
            {
                return _metadata.GetValueOrDefault(uuid);
            }

            return null;
        }

        public IEnumerable<ITrakHoundSourceMetadataEntity> GetMetadata(IEnumerable<string> uuids)
        {
            if (!uuids.IsNullOrEmpty())
            {
                var entities = new List<ITrakHoundSourceMetadataEntity>();
                foreach (var uuid in uuids)
                {
                    var entity = _metadata.GetValueOrDefault(uuid);
                    if (entity != null) entities.Add(entity);
                }
                return entities;
            }

            return null;
        }




        public IEnumerable<ITrakHoundSourceMetadataEntity> QueryMetadataBySourceUuid(string sourceUuid)
        {
            if (!string.IsNullOrEmpty(sourceUuid))
            {
                var uuids = _metadataBySourceUuid.Get(sourceUuid);
                if (!uuids.IsNullOrEmpty())
                {
                    var entities = new List<ITrakHoundSourceMetadataEntity>();
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
