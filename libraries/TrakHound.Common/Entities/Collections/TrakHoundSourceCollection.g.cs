// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;

namespace TrakHound.Entities.Collections
{
    public partial class TrakHoundSourceCollection
    {
        private readonly TrakHoundEntityCollection _entityCollection;


        public bool IsEmpty 
        {
            get {
                var empty = true;
                if (empty) empty = _sources.IsNullOrEmpty();
                if (empty) empty = _metadata.IsNullOrEmpty();
                return empty;
            }
        }


        public TrakHoundSourceCollection(TrakHoundEntityCollection entityCollection)
        {
            _entityCollection = entityCollection;
        }


        public void Add(ITrakHoundEntity entity)
        {
            if (entity != null)
            {
                var type = entity.GetType();

                if (typeof(ITrakHoundSourceEntity).IsAssignableFrom(type))
                    AddSource((ITrakHoundSourceEntity)entity);

                else if (typeof(ITrakHoundSourceMetadataEntity).IsAssignableFrom(type))
                    AddMetadata((ITrakHoundSourceMetadataEntity)entity);

            }
        }

        public IEnumerable<ITrakHoundEntity> GetEntities()
        {   
            var entities = new List<ITrakHoundEntity>();
            if (_sources.Count > 0) entities.AddRange(_sources.Values);
            if (_metadata.Count > 0) entities.AddRange(_metadata.Values);
            return entities;
        }

        private IEnumerable<TEntity> GetEntities<TEntity>(IEnumerable<ITrakHoundEntity> entities) where TEntity : ITrakHoundEntity
        {
            if (!entities.IsNullOrEmpty())
            {
                var x = new List<TEntity>();
                foreach (var entity in entities) x.Add((TEntity)entity);
                return x;
            }

            return null;
        }
        
        public IEnumerable<TEntity> GetEntities<TEntity>() where TEntity : ITrakHoundEntity
        {

            if (typeof(ITrakHoundSourceEntity).IsAssignableFrom(typeof(TEntity)))
                return GetEntities<TEntity>(_sources.Values);

            else if (typeof(ITrakHoundSourceMetadataEntity).IsAssignableFrom(typeof(TEntity)))
                return GetEntities<TEntity>(_metadata.Values);

            return default;
        }

        public ITrakHoundEntity GetEntity(string uuid)
        {
            if (!string.IsNullOrEmpty(uuid))
            {

                ITrakHoundEntity entity = GetSource(uuid);

                if (entity == null) entity = GetMetadata(uuid);

                return entity;
            }

            return default;
        }

        public TEntity GetEntity<TEntity>(string uuid) where TEntity : ITrakHoundEntity
        {
            if (!string.IsNullOrEmpty(uuid))
            {

                if (typeof(ITrakHoundSourceEntity).IsAssignableFrom(typeof(TEntity)))
                    return (TEntity)GetSource(uuid);

                else if (typeof(ITrakHoundSourceMetadataEntity).IsAssignableFrom(typeof(TEntity)))
                    return (TEntity)GetMetadata(uuid);

            }

            return default;
        }

        public IEnumerable<ITrakHoundEntity> GetEntityArrays()
        {
            var entities = new List<ITrakHoundEntity>();
            entities.AddRange(_sources.Values);
            entities.AddRange(_metadata.Values);
            return entities;
        }

        public void Clear()
        {
            _sources.Clear();
            _metadata.Clear();
        }

        public partial void OnClear();
    }
}
