// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;

namespace TrakHound.Entities.Collections
{
    public partial class TrakHoundDefinitionCollection
    {
        private readonly TrakHoundEntityCollection _entityCollection;


        public bool IsEmpty 
        {
            get {
                var empty = true;
                if (empty) empty = _definitions.IsNullOrEmpty();
                if (empty) empty = _metadata.IsNullOrEmpty();
                if (empty) empty = _descriptions.IsNullOrEmpty();
                if (empty) empty = _wikis.IsNullOrEmpty();
                return empty;
            }
        }


        public TrakHoundDefinitionCollection(TrakHoundEntityCollection entityCollection)
        {
            _entityCollection = entityCollection;
        }


        public void Add(ITrakHoundEntity entity)
        {
            if (entity != null)
            {
                var type = entity.GetType();

                if (typeof(ITrakHoundDefinitionEntity).IsAssignableFrom(type))
                    AddDefinition((ITrakHoundDefinitionEntity)entity);

                else if (typeof(ITrakHoundDefinitionMetadataEntity).IsAssignableFrom(type))
                    AddMetadata((ITrakHoundDefinitionMetadataEntity)entity);

                else if (typeof(ITrakHoundDefinitionDescriptionEntity).IsAssignableFrom(type))
                    AddDescription((ITrakHoundDefinitionDescriptionEntity)entity);

                else if (typeof(ITrakHoundDefinitionWikiEntity).IsAssignableFrom(type))
                    AddWiki((ITrakHoundDefinitionWikiEntity)entity);

            }
        }

        public IEnumerable<ITrakHoundEntity> GetEntities()
        {   
            var entities = new List<ITrakHoundEntity>();
            if (_definitions.Count > 0) entities.AddRange(_definitions.Values);
            if (_metadata.Count > 0) entities.AddRange(_metadata.Values);
            if (_descriptions.Count > 0) entities.AddRange(_descriptions.Values);
            if (_wikis.Count > 0) entities.AddRange(_wikis.Values);
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

            if (typeof(ITrakHoundDefinitionEntity).IsAssignableFrom(typeof(TEntity)))
                return GetEntities<TEntity>(_definitions.Values);

            else if (typeof(ITrakHoundDefinitionMetadataEntity).IsAssignableFrom(typeof(TEntity)))
                return GetEntities<TEntity>(_metadata.Values);

            else if (typeof(ITrakHoundDefinitionDescriptionEntity).IsAssignableFrom(typeof(TEntity)))
                return GetEntities<TEntity>(_descriptions.Values);

            else if (typeof(ITrakHoundDefinitionWikiEntity).IsAssignableFrom(typeof(TEntity)))
                return GetEntities<TEntity>(_wikis.Values);

            return default;
        }

        public ITrakHoundEntity GetEntity(string uuid)
        {
            if (!string.IsNullOrEmpty(uuid))
            {

                ITrakHoundEntity entity = GetDefinition(uuid);

                if (entity == null) entity = GetMetadata(uuid);

                if (entity == null) entity = GetDescription(uuid);

                if (entity == null) entity = GetWiki(uuid);

                return entity;
            }

            return default;
        }

        public TEntity GetEntity<TEntity>(string uuid) where TEntity : ITrakHoundEntity
        {
            if (!string.IsNullOrEmpty(uuid))
            {

                if (typeof(ITrakHoundDefinitionEntity).IsAssignableFrom(typeof(TEntity)))
                    return (TEntity)GetDefinition(uuid);

                else if (typeof(ITrakHoundDefinitionMetadataEntity).IsAssignableFrom(typeof(TEntity)))
                    return (TEntity)GetMetadata(uuid);

                else if (typeof(ITrakHoundDefinitionDescriptionEntity).IsAssignableFrom(typeof(TEntity)))
                    return (TEntity)GetDescription(uuid);

                else if (typeof(ITrakHoundDefinitionWikiEntity).IsAssignableFrom(typeof(TEntity)))
                    return (TEntity)GetWiki(uuid);

            }

            return default;
        }

        public IEnumerable<ITrakHoundEntity> GetEntityArrays()
        {
            var entities = new List<ITrakHoundEntity>();
            entities.AddRange(_definitions.Values);
            entities.AddRange(_metadata.Values);
            entities.AddRange(_descriptions.Values);
            entities.AddRange(_wikis.Values);
            return entities;
        }

        public void Clear()
        {
            _definitions.Clear();
            _metadata.Clear();
            _descriptions.Clear();
            _wikis.Clear();
        }

        public partial void OnClear();
    }
}
