// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using TrakHound.Clients;
using TrakHound.Clients.Collections;

namespace TrakHound.Entities.Collections
{
    public partial class TrakHoundEntityCollection
    {
        private readonly TrakHoundCollectionEntitiesClient _client;
        private readonly HashSet<string> _targetUuids = new HashSet<string>();
        private readonly TrakHoundObjectCollection _objects;
        private readonly TrakHoundDefinitionCollection _definitions;
        private readonly TrakHoundSourceCollection _sources;


        public ITrakHoundSystemEntitiesClient Client => _client;

        public IEnumerable<string> TargetUuids => _targetUuids;


        public TrakHoundObjectCollection Objects => _objects;

        public TrakHoundDefinitionCollection Definitions => _definitions;

        public TrakHoundSourceCollection Sources => _sources;


        public TrakHoundEntityCollection()
        {
            _client = new TrakHoundCollectionEntitiesClient(this);
            _objects = new TrakHoundObjectCollection(this);
            _definitions = new TrakHoundDefinitionCollection(this);
            _sources = new TrakHoundSourceCollection(this);
        }

        public static TrakHoundEntityCollection FromJson(string json)
        {
            return null;
        }


        #region "Add"

        public void AddTargetId(string targetId)
        {
            if (!string.IsNullOrEmpty(targetId))
            {
                if (!_targetUuids.Contains(targetId))
                {
                    _targetUuids.Add(targetId);
                }
            }
        }

        public void AddTargetIds(IEnumerable<string> targetIds)
        {
            if (!targetIds.IsNullOrEmpty())
            {
                foreach (var targetId in targetIds) AddTargetId(targetId);
            }
        }


        public void Add(TrakHoundEntityCollection collection)
        {
            if (collection != null)
            {
                if (!collection.TargetUuids.IsNullOrEmpty())
                {
                    AddTargetIds(collection.TargetUuids);
                }

                var entities = collection.GetEntities();
                if (!entities.IsNullOrEmpty())
                {
                    foreach (var entity in entities)
                    {
                        Add(entity, false);
                    }
                }
            }
        }

        public void Add<TEntity>(TEntity entity, bool addAsTarget = true) where TEntity : ITrakHoundEntity
        {
            if (entity != null)
            {
                if (addAsTarget)
                {
                    AddTarget(entity);
                }

                AddEntity(entity);
            }
        }

        public void Add(IEnumerable<ITrakHoundEntity> entities, bool addAsTarget = true)
        {
            if (!entities.IsNullOrEmpty())
            {
                foreach (var entity in entities)
                {
                    if (entity != null)
                    {
                        if (addAsTarget)
                        {
                            AddTarget(entity);
                        }

                        AddEntity(entity);
                    }
                }
            }
        }


        private void AddEntity<TEntity>(TEntity entity, bool addAsTarget = false) where TEntity : ITrakHoundEntity
        {
            if (entity != null)
            {
                if (addAsTarget) AddTargetId(entity.Uuid);

                if (TrakHoundObjectsEntity.IsObjectsEntity(entity)) _objects.Add(entity);
                else if (TrakHoundDefinitionsEntity.IsDefinitionsEntity(entity)) _definitions.Add(entity);
                else if (TrakHoundSourcesEntity.IsSourcesEntity(entity)) _sources.Add(entity);
            }
        }

        private void AddEntities(IEnumerable<ITrakHoundEntity> entities)
        {
            if (!entities.IsNullOrEmpty())
            {
                foreach (var entity in entities) AddEntity(entity);
            }
        }

        #endregion

        #region "Get"

        public IEnumerable<ITrakHoundEntity> GetEntities()
        {
            var entities = new List<ITrakHoundEntity>();
            entities.AddRange(_sources.GetEntities());
            entities.AddRange(_definitions.GetEntities());
            entities.AddRange(_objects.GetEntities());
            return entities;
        }

        public IEnumerable<TEntity> GetEntities<TEntity>() where TEntity : ITrakHoundEntity
        {
            if (TrakHoundSourcesEntity.IsSourcesEntity<TEntity>()) return _sources.GetEntities<TEntity>();
            else if (TrakHoundDefinitionsEntity.IsDefinitionsEntity<TEntity>()) return _definitions.GetEntities<TEntity>();
            else if (TrakHoundObjectsEntity.IsObjectsEntity<TEntity>()) return _objects.GetEntities<TEntity>();

            return default;
        }

        public ITrakHoundEntity GetEntity(string uuid)
        {
            if (!string.IsNullOrEmpty(uuid))
            {
                var entity = _sources.GetEntity(uuid);
                if (entity == null) entity = _definitions.GetEntity(uuid);
                if (entity == null) entity = _objects.GetEntity(uuid);
                return entity;
            }

            return default;
        }

        public TEntity GetEntity<TEntity>(string uuid) where TEntity : ITrakHoundEntity
        {
            if (!string.IsNullOrEmpty(uuid))
            {
                if (TrakHoundSourcesEntity.IsSourcesEntity<TEntity>()) return _sources.GetEntity<TEntity>(uuid);
                else if (TrakHoundDefinitionsEntity.IsDefinitionsEntity<TEntity>()) return _definitions.GetEntity<TEntity>(uuid);
                else if (TrakHoundObjectsEntity.IsObjectsEntity<TEntity>()) return _objects.GetEntity<TEntity>(uuid);
            }

            return default;
        }


        public IEnumerable<ITrakHoundEntity> GetTargetEntities()
        {
            var results = new List<ITrakHoundEntity>();

            if (!TargetUuids.IsNullOrEmpty())
            {
                foreach (var targetUuid in TargetUuids)
                {
                    var entity = _sources.GetEntity(targetUuid);
                    if (entity == null) entity = _definitions.GetEntity(targetUuid);
                    if (entity == null) entity = _objects.GetEntity(targetUuid);
                    if (entity != null) results.Add(entity);
                }
            }

            return results;
        }

        public ITrakHoundEntity GetTargetEntity(string uuid)
        {
            
            if (!string.IsNullOrEmpty(uuid))
            {
                ITrakHoundEntity entity = null;

                if (TargetUuids != null && TargetUuids.Contains(uuid))
                {
                    entity = _sources.GetEntity(uuid);
                    if (entity == null) entity = _definitions.GetEntity(uuid);
                    if (entity == null) entity = _objects.GetEntity(uuid);
                    return entity;
                }
            }

            return default;
        }

        #endregion

        #region "Count"

        public int TargetCount()
        {
            var count = 0;

            if (!TargetUuids.IsNullOrEmpty())
            {
                foreach (var targetUuid in TargetUuids)
                {
                    count += 1;
                }
            }

            return count;
        }

        #endregion


        public void Clear()
        {
            _objects.Clear();
            _definitions.Clear();
            _sources.Clear();

            _targetUuids.Clear();
        }
    }
}
