// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrakHound.Entities;

namespace TrakHound.Clients.Collections
{
    internal partial class TrakHoundCollectionObjectClient
    {

        #region "Query"

        public async Task<IEnumerable<ITrakHoundObjectEntity>> Query(string path, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        {
            return _collection.Objects.QueryObjects(path);
        }

        public async Task<IEnumerable<ITrakHoundObjectEntity>> Query(IEnumerable<string> paths, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        {
            if (!paths.IsNullOrEmpty())
            {
                var results = new List<ITrakHoundObjectEntity>();
                foreach (var path in paths)
                {
                    var entityModels = _collection.Objects.QueryObjects(path);
                    if (!entityModels.IsNullOrEmpty()) results.AddRange(entityModels);
                }
                return results;
            }

            return null;
        }

        public async Task<IEnumerable<ITrakHoundObjectQueryResult>> QueryUuids(string path, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        {
            if (!string.IsNullOrEmpty(path))
            {
                var results = new List<ITrakHoundObjectQueryResult>();
                var entities = _collection.Objects.QueryObjects(path);
                if (!entities.IsNullOrEmpty())
                {
                    foreach (var entity in entities) results.Add(new TrakHoundObjectQueryResult(TrakHoundObjectQueryRequestType.Uuid, entity.Namespace, path, entity.Uuid));
                }
                return results;
            }

            return null;
        }

        public async Task<IEnumerable<ITrakHoundObjectQueryResult>> QueryUuids(IEnumerable<string> paths, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        {
            if (!paths.IsNullOrEmpty())
            {
                var results = new List<ITrakHoundObjectQueryResult>();
                foreach (var path in paths)
                {
                    var entities = _collection.Objects.QueryObjects(path);
                    if (!entities.IsNullOrEmpty())
                    {
                        foreach (var entity in entities) results.Add(new TrakHoundObjectQueryResult(TrakHoundObjectQueryRequestType.Uuid, entity.Namespace, path, entity.Uuid));
                    }
                }
                return results;
            }

            return null;
        }


        public async Task<IEnumerable<ITrakHoundObjectEntity>> Query(TrakHoundObjectQueryRequest queryRequest, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        {
            return null;
        }

        public async Task<IEnumerable<ITrakHoundObjectQueryResult>> QueryUuids(TrakHoundObjectQueryRequest queryRequest, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        {
            return null;
        }



        public async Task<IEnumerable<string>> QueryMatch(string query, IEnumerable<string> objectUuids, string routerId = null)
        {
            if (!string.IsNullOrEmpty(query) && !objectUuids.IsNullOrEmpty())
            {
                var objs = _collection.Objects.GetObjects(objectUuids);
                if (!objs.IsNullOrEmpty())
                {
                    //return TrakHoundObjectExpression.Match(query, objs)?.Select(o => o.Uuid);
                }
            }

            return null;
        }

        public async Task<IEnumerable<TrakHoundTargetResult>> QueryMatch(IEnumerable<string> queries, IEnumerable<string> objectUuids, string routerId = null)
        {
            if (!queries.IsNullOrEmpty() && !objectUuids.IsNullOrEmpty())
            {
                var results = new List<TrakHoundTargetResult>();

                foreach (var query in queries)
                {
                    var matches = await QueryMatch(query, objectUuids);
                    if (!matches.IsNullOrEmpty())
                    {
                        foreach (var match in matches)
                        {
                            results.Add(new TrakHoundTargetResult(query, match));
                        }
                    }
                }

                return results;
            }

            return null;
        }

        #region "Query Root"

        public async Task<IEnumerable<ITrakHoundObjectEntity>> QueryRoot(long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        {
            var objects = _collection.Objects.Objects;
            if (!objects.IsNullOrEmpty())
            {
                return objects.Where(o => o.ParentUuid == null);
            }

            return null;
        }

        public async Task<IEnumerable<ITrakHoundObjectEntity>> QueryRoot(string ns, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        {
            if (!string.IsNullOrEmpty(ns))
            {
                return await QueryRoot(new string[] { ns }, skip, take, sortOrder);
            }

            return null;
        }

        public async Task<IEnumerable<ITrakHoundObjectEntity>> QueryRoot(IEnumerable<string> namespaces, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        {
            if (!namespaces.IsNullOrEmpty())
            {
                var objects = _collection.Objects.Objects;
                if (!objects.IsNullOrEmpty())
                {
                    return objects.Where(o => namespaces.Contains(o.Namespace));
                }
            }

            return null;
        }

        public async Task<IEnumerable<ITrakHoundObjectQueryResult>> QueryRootUuids(long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        {
            return await QueryRootUuids(skip, take, sortOrder);
        }

        public async Task<IEnumerable<ITrakHoundObjectQueryResult>> QueryRootUuids(string ns, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        {
            if (!string.IsNullOrEmpty(ns))
            {
                return await QueryRootUuids(new string[] { ns }, skip, take, sortOrder);
            }

            return null;
        }

        public async Task<IEnumerable<ITrakHoundObjectQueryResult>> QueryRootUuids(IEnumerable<string> namespaces, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        {
            var objects = await QueryRootUuids(namespaces, skip, take, sortOrder);
            if (!objects.IsNullOrEmpty())
            {
                var results = new List<ITrakHoundObjectQueryResult>();

                foreach (var obj in objects)
                {
                    results.Add(new TrakHoundObjectQueryResult(TrakHoundObjectQueryRequestType.Uuid, obj.Namespace, obj.Query, obj.Uuid));
                }

                return results;
            }

            return null;
        }

        #endregion

        #region "Query By Parent"

        //public async Task<IEnumerable<ITrakHoundObjectEntityModel>> QueryModelsByParentUuid(string parentUuid, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        //{
        //    if (!string.IsNullOrEmpty(parentUuid))
        //    {
        //        return await QueryModelsByParentUuid(new string[] { parentUuid }, skip, take, sortOrder);
        //    }

        //    return null;
        //}

        //public async Task<IEnumerable<ITrakHoundObjectEntityModel>> QueryModelsByParentUuid(IEnumerable<string> parentUuids, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        //{
        //    var objects = await QueryByParentUuid(parentUuids, skip, take, sortOrder);
        //    if (!objects.IsNullOrEmpty())
        //    {
        //        var models = new List<ITrakHoundObjectEntityModel>();
        //        foreach (var obj in objects)
        //        {
        //            var model = _collection.Objects.GetObjectModel(obj.Uuid);
        //            if (model != null) models.Add(model);
        //        }
        //        return models;
        //    }

        //    return null;
        //}

        //public async Task<IEnumerable<ITrakHoundObjectEntityModel>> QueryPartialsByParentUuid(string parentUuid, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        //{
        //    if (!string.IsNullOrEmpty(parentUuid))
        //    {
        //        return await QueryPartialsByParentUuid(new string[] { parentUuid }, skip, take, sortOrder);
        //    }

        //    return null;
        //}

        //public async Task<IEnumerable<ITrakHoundObjectEntityModel>> QueryPartialsByParentUuid(IEnumerable<string> parentUuids, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        //{
        //    var objects = await QueryByParentUuid(parentUuids, skip, take, sortOrder);
        //    if (!objects.IsNullOrEmpty())
        //    {
        //        var models = new List<ITrakHoundObjectEntityModel>();
        //        foreach (var obj in objects)
        //        {
        //            var model = _collection.Objects.GetObjectPartialModel(obj.Uuid);
        //            if (model != null) models.Add(model);
        //        }
        //        return models;
        //    }

        //    return null;
        //}

        public async Task<IEnumerable<ITrakHoundObjectEntity>> QueryByParentUuid(string parentUuid, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        {
            if (!string.IsNullOrEmpty(parentUuid))
            {
                return await QueryByParentUuid(new string[] { parentUuid }, skip, take, sortOrder);
            }

            return null;
        }

        public async Task<IEnumerable<ITrakHoundObjectEntity>> QueryByParentUuid(IEnumerable<string> parentUuids, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        {
            if (!parentUuids.IsNullOrEmpty())
            {
                var objects = _collection.Objects.Objects;
                if (!objects.IsNullOrEmpty())
                {
                    return objects.Where(o => parentUuids.Contains(o.ParentUuid));
                }
            }

            return null;
        }

        public async Task<IEnumerable<ITrakHoundObjectQueryResult>> QueryUuidsByParentUuid(string parentUuid, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        {
            if (!string.IsNullOrEmpty(parentUuid))
            {
                return await QueryUuidsByParentUuid(new string[] { parentUuid }, skip, take, sortOrder);
            }

            return null;
        }

        public async Task<IEnumerable<ITrakHoundObjectQueryResult>> QueryUuidsByParentUuid(IEnumerable<string> parentUuids, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        {
            var objects = await QueryByParentUuid(parentUuids, skip, take, sortOrder);
            if (!objects.IsNullOrEmpty())
            {
                var results = new List<ITrakHoundObjectQueryResult>();

                foreach (var obj in objects)
                {
                    results.Add(new TrakHoundObjectQueryResult(TrakHoundObjectQueryRequestType.Uuid, null, obj.ParentUuid, obj.Uuid));
                }

                return results;
            }

            return null;
        }

        #endregion

        #region "Query by Children"

        //public async Task<IEnumerable<ITrakHoundObjectEntityModel>> QueryModelsByChildUuid(string childUuid, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        //{
        //    if (!string.IsNullOrEmpty(childUuid))
        //    {
        //        return await QueryModelsByChildUuid(new string[] { childUuid }, skip, take, sortOrder);
        //    }

        //    return null;
        //}

        //public async Task<IEnumerable<ITrakHoundObjectEntityModel>> QueryModelsByChildUuid(IEnumerable<string> childUuids, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        //{
        //    var objects = await QueryByChildUuid(childUuids, skip, take, sortOrder);
        //    if (!objects.IsNullOrEmpty())
        //    {
        //        var models = new List<ITrakHoundObjectEntityModel>();
        //        foreach (var obj in objects)
        //        {
        //            var model = _collection.Objects.GetObjectModel(obj.Uuid);
        //            if (model != null) models.Add(model);
        //        }
        //        return models;
        //    }

        //    return null;
        //}

        //public async Task<IEnumerable<ITrakHoundObjectEntityModel>> QueryPartialsByChildUuid(string childUuid, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        //{
        //    if (!string.IsNullOrEmpty(childUuid))
        //    {
        //        return await QueryPartialsByChildUuid(new string[] { childUuid }, skip, take, sortOrder);
        //    }

        //    return null;
        //}

        //public async Task<IEnumerable<ITrakHoundObjectEntityModel>> QueryPartialsByChildUuid(IEnumerable<string> childUuids, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        //{
        //    var objects = await QueryByChildUuid(childUuids, skip, take, sortOrder);
        //    if (!objects.IsNullOrEmpty())
        //    {
        //        var models = new List<ITrakHoundObjectEntityModel>();
        //        foreach (var obj in objects)
        //        {
        //            var model = _collection.Objects.GetObjectPartialModel(obj.Uuid);
        //            if (model != null) models.Add(model);
        //        }
        //        return models;
        //    }

        //    return null;
        //}

        public async Task<IEnumerable<ITrakHoundObjectEntity>> QueryByChildUuid(string childUuid, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        {
            if (!string.IsNullOrEmpty(childUuid))
            {
                return await QueryByChildUuid(new string[] { childUuid }, skip, take, sortOrder);
            }

            return null;
        }

        public async Task<IEnumerable<ITrakHoundObjectEntity>> QueryByChildUuid(IEnumerable<string> childUuids, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        {
            if (!childUuids.IsNullOrEmpty())
            {
                var objects = _collection.Objects.Objects;
                if (!objects.IsNullOrEmpty())
                {
                    var parentUuids = objects.Select(o => o.ParentUuid).Distinct();

                    return await QueryByParentUuid(parentUuids, skip, take, sortOrder);
                }
            }

            return null;
        }

        public async Task<IEnumerable<ITrakHoundObjectQueryResult>> QueryUuidsByChildUuid(string childUuid, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        {
            if (!string.IsNullOrEmpty(childUuid))
            {
                return await QueryUuidsByChildUuid(new string[] { childUuid }, skip, take, sortOrder);
            }

            return null;
        }

        public async Task<IEnumerable<ITrakHoundObjectQueryResult>> QueryUuidsByChildUuid(IEnumerable<string> childUuids, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        {
            var objects = await QueryByChildUuid(childUuids, skip, take, sortOrder);
            if (!objects.IsNullOrEmpty())
            {
                var parentUuids = objects.Select(o => o.ParentUuid).Distinct();

                return await QueryUuidsByParentUuid(parentUuids, skip, take, sortOrder);
            }

            return null;
        }

        #endregion

        #region "Query Children by Root"

        //public async Task<IEnumerable<ITrakHoundObjectEntityModel>> QueryChildModelsByRootUuid(string rootUuid, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        //{
        //    if (!string.IsNullOrEmpty(rootUuid))
        //    {
        //        return await QueryChildModelsByRootUuid(new string[] { rootUuid }, skip, take, sortOrder);
        //    }

        //    return null;
        //}

        //public async Task<IEnumerable<ITrakHoundObjectEntityModel>> QueryChildModelsByRootUuid(IEnumerable<string> rootUuids, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        //{
        //    var objects = await QueryChildrenByRootUuid(rootUuids, skip, take, sortOrder);
        //    if (!objects.IsNullOrEmpty())
        //    {
        //        var models = new List<ITrakHoundObjectEntityModel>();
        //        foreach (var obj in objects)
        //        {
        //            var model = _collection.Objects.GetObjectModel(obj.Uuid);
        //            if (model != null) models.Add(model);
        //        }
        //        return models;
        //    }

        //    return null;
        //}

        //public async Task<IEnumerable<ITrakHoundObjectEntityModel>> QueryChildPartialsByRootUuid(string rootUuid, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        //{
        //    if (!string.IsNullOrEmpty(rootUuid))
        //    {
        //        return await QueryChildPartialsByRootUuid(new string[] { rootUuid }, skip, take, sortOrder);
        //    }

        //    return null;
        //}

        //public async Task<IEnumerable<ITrakHoundObjectEntityModel>> QueryChildPartialsByRootUuid(IEnumerable<string> rootUuids, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        //{
        //    var objects = await QueryChildrenByRootUuid(rootUuids, skip, take, sortOrder);
        //    if (!objects.IsNullOrEmpty())
        //    {
        //        var models = new List<ITrakHoundObjectEntityModel>();
        //        foreach (var obj in objects)
        //        {
        //            var model = _collection.Objects.GetObjectPartialModel(obj.Uuid);
        //            if (model != null) models.Add(model);
        //        }
        //        return models;
        //    }

        //    return null;
        //}

        public async Task<IEnumerable<ITrakHoundObjectEntity>> QueryChildrenByRootUuid(string rootUuid, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        {
            if (!string.IsNullOrEmpty(rootUuid))
            {
                return await QueryChildrenByRootUuid(new string[] { rootUuid }, skip, take, sortOrder);
            }

            return null;
        }

        public async Task<IEnumerable<ITrakHoundObjectEntity>> QueryChildrenByRootUuid(IEnumerable<string> rootUuids, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        {
            if (!rootUuids.IsNullOrEmpty())
            {
                //var objects = _collection.Objects.GetObjects(rootUuids);
                //if (!objects.IsNullOrEmpty())
                //{
                //    var results = new List<ITrakHoundObjectEntity>();
                //    foreach (var obj in objects)
                //    {
                //        var children = obj.GetAllChildren();
                //        if (!children.IsNullOrEmpty())
                //        {
                //            results.AddRange(children);
                //        }
                //    }
                //    return results;
                //}
            }

            return null;
        }

        public async Task<IEnumerable<ITrakHoundObjectQueryResult>> QueryChildUuidsByRootUuid(string rootUuid, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        {
            if (!string.IsNullOrEmpty(rootUuid))
            {
                return await QueryChildUuidsByRootUuid(new string[] { rootUuid }, skip, take, sortOrder);
            }

            return null;
        }

        public async Task<IEnumerable<ITrakHoundObjectQueryResult>> QueryChildUuidsByRootUuid(IEnumerable<string> rootUuids, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        {
            if (!rootUuids.IsNullOrEmpty())
            {
                var results = new List<ITrakHoundObjectQueryResult>();

                foreach (var rootUuid in rootUuids)
                {
                    var childObjects = await QueryChildrenByRootUuid(rootUuid, skip, take, sortOrder);
                    if (!childObjects.IsNullOrEmpty())
                    {
                        foreach (var obj in childObjects)
                        {
                            results.Add(new TrakHoundObjectQueryResult(TrakHoundObjectQueryRequestType.Uuid, null, rootUuid, obj.Uuid));
                        }
                    }
                }

                return results;
            }

            return null;
        }

        #endregion

        #region "Query Root by Children"

        //public async Task<IEnumerable<ITrakHoundObjectEntityModel>> QueryRootModelsByChildUuid(string childUuid, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        //{
        //    if (!string.IsNullOrEmpty(childUuid))
        //    {
        //        return await QueryRootModelsByChildUuid(new string[] { childUuid }, skip, take, sortOrder);
        //    }

        //    return null;
        //}

        //public async Task<IEnumerable<ITrakHoundObjectEntityModel>> QueryRootModelsByChildUuid(IEnumerable<string> childUuids, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        //{
        //    var objects = await QueryRootByChildUuid(childUuids, skip, take, sortOrder);
        //    if (!objects.IsNullOrEmpty())
        //    {
        //        var models = new List<ITrakHoundObjectEntityModel>();
        //        foreach (var obj in objects)
        //        {
        //            var model = _collection.Objects.GetObjectModel(obj.Uuid);
        //            if (model != null) models.Add(model);
        //        }
        //        return models;
        //    }

        //    return null;
        //}

        //public async Task<IEnumerable<ITrakHoundObjectEntityModel>> QueryRootPartialsByChildUuid(string childUuid, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        //{
        //    if (!string.IsNullOrEmpty(childUuid))
        //    {
        //        return await QueryRootPartialsByChildUuid(new string[] { childUuid }, skip, take, sortOrder);
        //    }

        //    return null;
        //}

        //public async Task<IEnumerable<ITrakHoundObjectEntityModel>> QueryRootPartialsByChildUuid(IEnumerable<string> childUuids, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        //{
        //    var objects = await QueryRootByChildUuid(childUuids, skip, take, sortOrder);
        //    if (!objects.IsNullOrEmpty())
        //    {
        //        var models = new List<ITrakHoundObjectEntityModel>();
        //        foreach (var obj in objects)
        //        {
        //            var model = _collection.Objects.GetObjectPartialModel(obj.Uuid);
        //            if (model != null) models.Add(model);
        //        }
        //        return models;
        //    }

        //    return null;
        //}

        public async Task<IEnumerable<ITrakHoundObjectEntity>> QueryRootByChildUuid(string childUuid, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        {
            if (!string.IsNullOrEmpty(childUuid))
            {
                return await QueryRootByChildUuid(new string[] { childUuid }, skip, take, sortOrder);
            }

            return null;
        }

        public async Task<IEnumerable<ITrakHoundObjectEntity>> QueryRootByChildUuid(IEnumerable<string> childUuids, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        {
            if (!childUuids.IsNullOrEmpty())
            {
                //var objects = _collection.Objects.Objects;
                //if (!objects.IsNullOrEmpty())
                //{
                //    var parentUuids = objects.Select(o => o.ParentUuid).Distinct();

                //    return await QueryRootByParentUuid(parentUuids, skip, take, sortOrder);
                //}
            }

            return null;
        }

        public async Task<IEnumerable<ITrakHoundObjectQueryResult>> QueryRootUuidsByChildUuid(string childUuid, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        {
            if (!string.IsNullOrEmpty(childUuid))
            {
                return await QueryRootUuidsByChildUuid(new string[] { childUuid }, skip, take, sortOrder);
            }

            return null;
        }

        public async Task<IEnumerable<ITrakHoundObjectQueryResult>> QueryRootUuidsByChildUuid(IEnumerable<string> childUuids, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        {
            var objects = await QueryByChildUuid(childUuids, skip, take, sortOrder);
            if (!objects.IsNullOrEmpty())
            {
                //var parentUuids = objects.Select(o => o.ParentUuid).Distinct();

                //return await QueryUuidsByParentUuid(parentUuids, skip, take, sortOrder);
            }

            return null;
        }

        #endregion

        #endregion

        #region "Namespaces"

        public async Task<IEnumerable<string>> ListNamespaces(long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        {
            var objects = _collection.Objects.Objects;
            if (!objects.IsNullOrEmpty())
            {
                return objects.Select(o => o.Namespace).Distinct();
            }

            return null;
        }

        #endregion

        #region "Subscribe"

        public async Task<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectEntity>>> Subscribe(
            IEnumerable<string> paths,
            int interval = 0,
            int take = 1000,
            string consumerId = null,
            string routerId = null)
        {
            return default;
        }

        public async Task<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectEntity>>> SubscribeByUuid(
            IEnumerable<string> uuids,
            int interval = 0,
            int take = 1000,
            string consumerId = null,
            string routerId = null)
        {
            return default;
        }

        #endregion

        #region "Count"

        public async Task<TrakHoundCount> QueryChildCount(string uuid, string routerId = null)
        {
            if (!string.IsNullOrEmpty(uuid))
            {
                var results = await QueryChildCount(new string[] { uuid }, routerId);
                return results?.FirstOrDefault();
            }

            return null;
        }

        public async Task<IEnumerable<TrakHoundCount>> QueryChildCount(IEnumerable<string> uuids, string routerId = null)
        {
            return null;
        }


        public async Task<TrakHoundCount> QueryDescendantCount(string uuid, string routerId = null)
        {
            if (!string.IsNullOrEmpty(uuid))
            {
                var results = await QueryDescendantCount(new string[] { uuid }, routerId);
                return results?.FirstOrDefault();
            }

            return null;
        }

        public async Task<IEnumerable<TrakHoundCount>> QueryDescendantCount(IEnumerable<string> uuids, string routerId = null)
        {
            return null;
        }

        #endregion

        #region "Notify"

        public async Task<ITrakHoundConsumer<IEnumerable<TrakHoundEntityNotification>>> Notify(string path, TrakHoundEntityNotificationType notificationType = TrakHoundEntityNotificationType.All, string routerId = null) => default;

        #endregion

        #region "Index"

        //public async Task<IEnumerable<string>> QueryIndex(string target, TrakHoundIndexQueryType queryType, string query, long skip, long take, SortOrder sortOrder, string routerId = null)
        //{
        //    return null;
        //}

        public async Task<IEnumerable<string>> QueryIndex(IEnumerable<EntityIndexRequest> requests, long skip, long take, SortOrder sortOrder, string routerId = null)
        {
            return null;
        }

        public async Task<bool> UpdateIndex(IEnumerable<EntityIndexPublishRequest> requests, TrakHoundOperationMode mode = TrakHoundOperationMode.Async, string routerId = null)
        {
            return false;
        }

        #endregion

        #region "Delete"

        public async Task<bool> DeleteByRootUuid(string rootUuid, string routerId = null)
        {
            if (!string.IsNullOrEmpty(rootUuid))
            {
                return await DeleteByRootUuid(new string[] { rootUuid });
            }

            return false;
        }

        public async Task<bool> DeleteByRootUuid(IEnumerable<string> rootUuids, string routerId = null)
        {
            if (!rootUuids.IsNullOrEmpty())
            {
                return await DeleteByRootUuid(rootUuids);
            }

            return false;
        }

        #endregion

        #region "Expire"

        public async Task<long> Expire(string pattern, long created, string routerId = null)
        {
            return 0;
        }

        public async Task<IEnumerable<EntityDeleteResult>> Expire(IEnumerable<string> patterns, long created, string routerId = null)
        {
            return null;
        }


        public async Task<long> ExpireByUpdate(string pattern, long lastUpdated, string routerId = null)
        {
            return 0;
        }

        public async Task<IEnumerable<EntityDeleteResult>> ExpireByUpdate(IEnumerable<string> patterns, long lastUpdated, string routerId = null)
        {
            return null;
        }


        public async Task<long> ExpireByAccess(string pattern, long lastAccessed, string routerId = null)
        {
            return 0;
        }

        public async Task<IEnumerable<EntityDeleteResult>> ExpireByAccess(IEnumerable<string> patterns, long lastAccessed, string routerId = null)
        {
            return null;
        }

        #endregion

    }
}
