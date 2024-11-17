// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace TrakHound.Entities.Collections
{
    public partial class TrakHoundObjectCollection
    {
        private readonly ListDictionary<string, ITrakHoundObjectEntity> _objectExpressions = new ListDictionary<string, ITrakHoundObjectEntity>();
        private readonly HashSet<string> _emptyObjectExpressions = new HashSet<string>();
        private readonly ListDictionary<string, string> _objectParents = new ListDictionary<string, string>(); // ParentUuid => Uuid[]
        private readonly ListDictionary<string, string> _metadataEntities = new ListDictionary<string, string>(); // EntityUuid => Uuid[]


        public int TotalCount()
        {
            var count = 0;

            count += _objects.Values.Count;
            count += _metadata.Values.Count;
            count += _observations.Values.Count;

            return count;
        }

        public int GetComponentTargetCount(string entityClass)
        {
            if (!string.IsNullOrEmpty(entityClass))
            {
                switch (entityClass.ConvertEnum<TrakHoundObjectsEntityClass>())
                {
                    case TrakHoundObjectsEntityClass.Observation: return _observations.Count;
                }
            }

            return 0;
        }


        public IEnumerable<ITrakHoundObjectEntity> QueryObjects(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                IEnumerable<ITrakHoundObjectEntity> entities = null;

                switch (TrakHoundPath.GetType(path))
                {
                    case TrakHoundPathType.Absolute:

                        var uuid = TrakHoundPath.GetUuid(path);
                        if (uuid != null)
                        {
                            var entity = GetObject(uuid);
                            if (entity != null)
                            {
                                entities = new ITrakHoundObjectEntity[] { entity };
                            }
                        }
                        break;

                    case TrakHoundPathType.Expression:

                        entities = _objectExpressions.Get(path);
                        if (entities == null)
                        {
                            if (!_emptyObjectExpressions.Contains(path))
                            {
                                entities = TrakHoundExpression.Match(path, _entityCollection);
                                if (!entities.IsNullOrEmpty())
                                {
                                    _objectExpressions.Add(path, entities);
                                }
                                else
                                {
                                    _emptyObjectExpressions.Add(path);
                                }
                            }
                        }
                        break;
                }

                return entities;
            }

            return null;
        }

        public IEnumerable<ITrakHoundObjectEntity> QueryObjects(IEnumerable<string> paths)
        {
            if (!paths.IsNullOrEmpty())
            {
                var entities = new List<ITrakHoundObjectEntity>();
                foreach (var path in paths)
                {
                    var pathEntities = QueryObjects(path);
                    if (!pathEntities.IsNullOrEmpty()) entities.AddRange(pathEntities);
                }
                return entities;
            }

            return null;
        }


        public IEnumerable<ITrakHoundObjectEntity> QueryObjectsByParentUuid(string parentUuid)
        {
            if (!string.IsNullOrEmpty(parentUuid))
            {
                var childUuids = _objectParents.Get(parentUuid);
                if (!childUuids.IsNullOrEmpty())
                {
                    var childObjs = new List<ITrakHoundObjectEntity>();
                    foreach (var childUuid in childUuids)
                    {
                        var childObj = GetObject(childUuid);
                        if (childObj != null) childObjs.Add(childObj);
                    }
                    return childObjs;
                }
            }

            return null;
        }

        public IEnumerable<ITrakHoundObjectEntity> QueryObjectsByParentUuid(IEnumerable<string> parentUuids)
        {
            if (!parentUuids.IsNullOrEmpty())
            {
                var childObjs = new List<ITrakHoundObjectEntity>();
                foreach (var parentUuid in parentUuids)
                {
                    var objs = QueryObjectsByParentUuid(parentUuid);
                    if (!objs.IsNullOrEmpty()) childObjs.AddRange(objs);
                }
                return childObjs;
            }

            return null;
        }

        public void AddObject(ITrakHoundObjectEntity entity)
        {
            if (entity != null)
            {
                var x = _objects.GetValueOrDefault(entity.Uuid);
                if (x == null || (entity.Priority >= x.Priority && !ObjectExtensions.ByteArraysEqual(x.Hash, entity.Hash)))
                {
                    _objects.Remove(entity.Uuid);
                    _objects.Add(entity.Uuid, new TrakHoundObjectEntity(entity));

                    if (!string.IsNullOrEmpty(entity.ParentUuid))
                    {
                        _objectParents.Add(entity.ParentUuid, entity.Uuid);
                    }
                }
            }
        }

        public void AddObjects(IEnumerable<ITrakHoundObjectEntity> entities)
        {
            if (!entities.IsNullOrEmpty())
            {
                foreach (var entity in entities)
                {
                    if (entity.Uuid != null)
                    {
                        var x = _objects.GetValueOrDefault(entity.Uuid);
                        if (x == null || (entity.Priority >= x.Priority && !ObjectExtensions.ByteArraysEqual(x.Hash, entity.Hash)))
                        {
                            _objects.Remove(entity.Uuid);
                            _objects.Add(entity.Uuid, new TrakHoundObjectEntity(entity));

                            if (!string.IsNullOrEmpty(entity.ParentUuid))
                            {
                                _objectParents.Add(entity.ParentUuid, entity.Uuid);
                            }
                        }
                    }
                }
            }
        }

        //public void OnAddObject(ITrakHoundObjectEntity entity) 
        //{ 
        //    if (!string.IsNullOrEmpty(entity.ParentUuid))
        //    {
        //        _objectParents.Add(entity.ParentUuid, entity.Uuid);
        //    }
        //}

        public partial void OnClear()
        {
            _objectExpressions.Clear();
            _emptyObjectExpressions.Clear();
            _objectParents.Clear();
            _metadataEntities.Clear();
        }
    }
}
