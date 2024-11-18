// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace TrakHound.Entities.Collections
{
    public partial class TrakHoundDefinitionCollection
    {
        private readonly Dictionary<string, ITrakHoundDefinitionMetadataEntity> _metadata = new Dictionary<string, ITrakHoundDefinitionMetadataEntity>();

        private readonly ListDictionary<string, string> _metadataByDefinitionUuid = new ListDictionary<string, string>(); // DefinitionUuid => Uuid



        public IEnumerable<ITrakHoundDefinitionMetadataEntity> Metadata => _metadata.Values;
        public void AddMetadata(ITrakHoundDefinitionMetadataEntity entity)
        {
            if (entity != null)
            {
                var x = _metadata.GetValueOrDefault(entity.Uuid);
                if (x == null || !ObjectExtensions.ByteArraysEqual(x.Hash, entity.Hash))
                {
                    _metadata.Remove(entity.Uuid);
                    _metadata.Add(entity.Uuid, new TrakHoundDefinitionMetadataEntity(entity));


                    _metadataByDefinitionUuid.Add(entity.DefinitionUuid, entity.Uuid);

                }
            }
        }

        public void AddMetadata(IEnumerable<ITrakHoundDefinitionMetadataEntity> entities)
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
                            _metadata.Add(entity.Uuid, new TrakHoundDefinitionMetadataEntity(entity));


                            _metadataByDefinitionUuid.Add(entity.DefinitionUuid, entity.Uuid);
                        }                   
                    }
                }
            }
        }


        public ITrakHoundDefinitionMetadataEntity GetMetadata(string uuid)
        {
            if (!string.IsNullOrEmpty(uuid))
            {
                return _metadata.GetValueOrDefault(uuid);
            }

            return null;
        }

        public IEnumerable<ITrakHoundDefinitionMetadataEntity> GetMetadata(IEnumerable<string> uuids)
        {
            if (!uuids.IsNullOrEmpty())
            {
                var entities = new List<ITrakHoundDefinitionMetadataEntity>();
                foreach (var uuid in uuids)
                {
                    var entity = _metadata.GetValueOrDefault(uuid);
                    if (entity != null) entities.Add(entity);
                }
                return entities;
            }

            return null;
        }




        public IEnumerable<ITrakHoundDefinitionMetadataEntity> QueryMetadataByDefinitionUuid(string definitionUuid)
        {
            if (!string.IsNullOrEmpty(definitionUuid))
            {
                var uuids = _metadataByDefinitionUuid.Get(definitionUuid);
                if (!uuids.IsNullOrEmpty())
                {
                    var entities = new List<ITrakHoundDefinitionMetadataEntity>();
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
