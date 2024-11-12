// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrakHound.Entities;

namespace TrakHound.Clients.Collections
{
    internal partial class TrakHoundCollectionDefinitionClient
    {

        #region "Read Partial Models"

        //public async Task<ITrakHoundDefinitionEntityModel> ReadPartialModel(string uuid, string routerId = null)
        //{
        //    return _collection.Definitions.GetDefinitionModel(uuid);
        //}

        //public async Task<IEnumerable<ITrakHoundDefinitionEntityModel>> ReadPartialModels(IEnumerable<string> uuids, string routerId = null)
        //{
        //    if (!uuids.IsNullOrEmpty())
        //    {
        //        var entities = new List<ITrakHoundDefinitionEntityModel>();
        //        foreach (var uuid in uuids)
        //        {
        //            var entity = _collection.Definitions.GetDefinitionModel(uuid);
        //            if (entity != null) entities.Add(entity);
        //        }
        //        return entities;
        //    }

        //    return null;
        //}

        #endregion

        #region "Query"

        //public async Task<IEnumerable<ITrakHoundDefinitionEntityModel>> QueryModels(string pattern, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null) => default;

        //public async Task<IEnumerable<ITrakHoundDefinitionEntityModel>> QueryPartialModels(string pattern, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null) => default;

        public async Task<IEnumerable<ITrakHoundDefinitionEntity>> Query(string pattern, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null) => default;

        #endregion

        #region "Query By Parent"

        //public async Task<IEnumerable<ITrakHoundDefinitionEntityModel>> QueryModelsByParentUuid(string parentUuid, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        //{
        //    if (!string.IsNullOrEmpty(parentUuid))
        //    {
        //        return await QueryModelsByParentUuid(new string[] { parentUuid }, skip, take, sortOrder);
        //    }

        //    return null;
        //}

        //public async Task<IEnumerable<ITrakHoundDefinitionEntityModel>> QueryModelsByParentUuid(IEnumerable<string> parentUuids, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        //{
        //    var objects = await QueryByParentUuid(parentUuids, skip, take, sortOrder);
        //    if (!objects.IsNullOrEmpty())
        //    {
        //        var models = new List<ITrakHoundDefinitionEntityModel>();
        //        foreach (var obj in objects)
        //        {
        //            var model = _collection.Definitions.GetDefinitionModel(obj.Uuid);
        //            if (model != null) models.Add(model);
        //        }
        //        return models;
        //    }

        //    return null;
        //}

        //public async Task<IEnumerable<ITrakHoundDefinitionEntityModel>> QueryPartialsByParentUuid(string parentUuid, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        //{
        //    if (!string.IsNullOrEmpty(parentUuid))
        //    {
        //        return await QueryPartialsByParentUuid(new string[] { parentUuid }, skip, take, sortOrder);
        //    }

        //    return null;
        //}

        //public async Task<IEnumerable<ITrakHoundDefinitionEntityModel>> QueryPartialsByParentUuid(IEnumerable<string> parentUuids, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        //{
        //    var objects = await QueryByParentUuid(parentUuids, skip, take, sortOrder);
        //    if (!objects.IsNullOrEmpty())
        //    {
        //        var models = new List<ITrakHoundDefinitionEntityModel>();
        //        foreach (var obj in objects)
        //        {
        //            var model = _collection.Definitions.GetDefinitionPartialModel(obj.Uuid);
        //            if (model != null) models.Add(model);
        //        }
        //        return models;
        //    }

        //    return null;
        //}

        public async Task<IEnumerable<ITrakHoundDefinitionEntity>> QueryByParentUuid(string parentUuid, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        {
            if (!string.IsNullOrEmpty(parentUuid))
            {
                return await QueryByParentUuid(new string[] { parentUuid }, skip, take, sortOrder);
            }

            return null;
        }

        public async Task<IEnumerable<ITrakHoundDefinitionEntity>> QueryByParentUuid(IEnumerable<string> parentUuids, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        {
            if (!parentUuids.IsNullOrEmpty())
            {
                var definitions = _collection.Definitions.Definitions;
                if (!definitions.IsNullOrEmpty())
                {
                    return definitions.Where(o => parentUuids.Contains(o.ParentUuid));
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

        //public async Task<IEnumerable<ITrakHoundDefinitionEntityModel>> QueryModelsByChildUuid(string childUuid, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        //{
        //    if (!string.IsNullOrEmpty(childUuid))
        //    {
        //        return await QueryModelsByChildUuid(new string[] { childUuid }, skip, take, sortOrder);
        //    }

        //    return null;
        //}

        //public async Task<IEnumerable<ITrakHoundDefinitionEntityModel>> QueryModelsByChildUuid(IEnumerable<string> childUuids, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        //{
        //    var objects = await QueryByChildUuid(childUuids, skip, take, sortOrder);
        //    if (!objects.IsNullOrEmpty())
        //    {
        //        var models = new List<ITrakHoundDefinitionEntityModel>();
        //        foreach (var obj in objects)
        //        {
        //            var model = _collection.Definitions.GetDefinitionModel(obj.Uuid);
        //            if (model != null) models.Add(model);
        //        }
        //        return models;
        //    }

        //    return null;
        //}

        //public async Task<IEnumerable<ITrakHoundDefinitionEntityModel>> QueryPartialsByChildUuid(string childUuid, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        //{
        //    if (!string.IsNullOrEmpty(childUuid))
        //    {
        //        return await QueryPartialsByChildUuid(new string[] { childUuid }, skip, take, sortOrder);
        //    }

        //    return null;
        //}

        //public async Task<IEnumerable<ITrakHoundDefinitionEntityModel>> QueryPartialsByChildUuid(IEnumerable<string> childUuids, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        //{
        //    var objects = await QueryByChildUuid(childUuids, skip, take, sortOrder);
        //    if (!objects.IsNullOrEmpty())
        //    {
        //        var models = new List<ITrakHoundDefinitionEntityModel>();
        //        foreach (var obj in objects)
        //        {
        //            var model = _collection.Definitions.GetDefinitionPartialModel(obj.Uuid);
        //            if (model != null) models.Add(model);
        //        }
        //        return models;
        //    }

        //    return null;
        //}

        public async Task<IEnumerable<ITrakHoundDefinitionEntity>> QueryByChildUuid(string childUuid, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        {
            if (!string.IsNullOrEmpty(childUuid))
            {
                return await QueryByChildUuid(new string[] { childUuid }, skip, take, sortOrder);
            }

            return null;
        }

        public async Task<IEnumerable<ITrakHoundDefinitionEntity>> QueryByChildUuid(IEnumerable<string> childUuids, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
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

        //public async Task<IEnumerable<ITrakHoundDefinitionEntityModel>> QueryChildModelsByRootUuid(string rootUuid, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        //{
        //    if (!string.IsNullOrEmpty(rootUuid))
        //    {
        //        return await QueryChildModelsByRootUuid(new string[] { rootUuid }, skip, take, sortOrder);
        //    }

        //    return null;
        //}

        //public async Task<IEnumerable<ITrakHoundDefinitionEntityModel>> QueryChildModelsByRootUuid(IEnumerable<string> rootUuids, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        //{
        //    var objects = await QueryChildrenByRootUuid(rootUuids, skip, take, sortOrder);
        //    if (!objects.IsNullOrEmpty())
        //    {
        //        var models = new List<ITrakHoundDefinitionEntityModel>();
        //        foreach (var obj in objects)
        //        {
        //            var model = _collection.Definitions.GetDefinitionModel(obj.Uuid);
        //            if (model != null) models.Add(model);
        //        }
        //        return models;
        //    }

        //    return null;
        //}

        //public async Task<IEnumerable<ITrakHoundDefinitionEntityModel>> QueryChildPartialsByRootUuid(string rootUuid, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        //{
        //    if (!string.IsNullOrEmpty(rootUuid))
        //    {
        //        return await QueryChildPartialsByRootUuid(new string[] { rootUuid }, skip, take, sortOrder);
        //    }

        //    return null;
        //}

        //public async Task<IEnumerable<ITrakHoundDefinitionEntityModel>> QueryChildPartialsByRootUuid(IEnumerable<string> rootUuids, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        //{
        //    var objects = await QueryChildrenByRootUuid(rootUuids, skip, take, sortOrder);
        //    if (!objects.IsNullOrEmpty())
        //    {
        //        var models = new List<ITrakHoundDefinitionEntityModel>();
        //        foreach (var obj in objects)
        //        {
        //            var model = _collection.Definitions.GetDefinitionPartialModel(obj.Uuid);
        //            if (model != null) models.Add(model);
        //        }
        //        return models;
        //    }

        //    return null;
        //}

        public async Task<IEnumerable<ITrakHoundDefinitionEntity>> QueryChildrenByRootUuid(string rootUuid, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        {
            if (!string.IsNullOrEmpty(rootUuid))
            {
                return await QueryChildrenByRootUuid(new string[] { rootUuid }, skip, take, sortOrder);
            }

            return null;
        }

        public async Task<IEnumerable<ITrakHoundDefinitionEntity>> QueryChildrenByRootUuid(IEnumerable<string> rootUuids, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        {
            //if (!rootUuids.IsNullOrEmpty())
            //{
            //    var objects = _collection.Definitions.GetDefinitionModels(rootUuids);
            //    if (!objects.IsNullOrEmpty())
            //    {
            //        var results = new List<ITrakHoundDefinitionEntity>();
            //        foreach (var obj in objects)
            //        {
            //            var children = obj.GetAllChildren();
            //            if (!children.IsNullOrEmpty())
            //            {
            //                results.AddRange(children);
            //            }
            //        }
            //        return results;
            //    }
            //}

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

        //public async Task<IEnumerable<ITrakHoundDefinitionEntityModel>> QueryRootModelsByChildUuid(string childUuid, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        //{
        //    if (!string.IsNullOrEmpty(childUuid))
        //    {
        //        return await QueryRootModelsByChildUuid(new string[] { childUuid }, skip, take, sortOrder);
        //    }

        //    return null;
        //}

        //public async Task<IEnumerable<ITrakHoundDefinitionEntityModel>> QueryRootModelsByChildUuid(IEnumerable<string> childUuids, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        //{
        //    var objects = await QueryRootByChildUuid(childUuids, skip, take, sortOrder);
        //    if (!objects.IsNullOrEmpty())
        //    {
        //        var models = new List<ITrakHoundDefinitionEntityModel>();
        //        foreach (var obj in objects)
        //        {
        //            var model = _collection.Definitions.GetDefinitionModel(obj.Uuid);
        //            if (model != null) models.Add(model);
        //        }
        //        return models;
        //    }

        //    return null;
        //}

        //public async Task<IEnumerable<ITrakHoundDefinitionEntityModel>> QueryRootPartialsByChildUuid(string childUuid, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        //{
        //    if (!string.IsNullOrEmpty(childUuid))
        //    {
        //        return await QueryRootPartialsByChildUuid(new string[] { childUuid }, skip, take, sortOrder);
        //    }

        //    return null;
        //}

        //public async Task<IEnumerable<ITrakHoundDefinitionEntityModel>> QueryRootPartialsByChildUuid(IEnumerable<string> childUuids, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        //{
        //    var objects = await QueryRootByChildUuid(childUuids, skip, take, sortOrder);
        //    if (!objects.IsNullOrEmpty())
        //    {
        //        var models = new List<ITrakHoundDefinitionEntityModel>();
        //        foreach (var obj in objects)
        //        {
        //            var model = _collection.Definitions.GetDefinitionPartialModel(obj.Uuid);
        //            if (model != null) models.Add(model);
        //        }
        //        return models;
        //    }

        //    return null;
        //}

        public async Task<IEnumerable<ITrakHoundDefinitionEntity>> QueryRootByChildUuid(string childUuid, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        {
            if (!string.IsNullOrEmpty(childUuid))
            {
                return await QueryRootByChildUuid(new string[] { childUuid }, skip, take, sortOrder);
            }

            return null;
        }

        public async Task<IEnumerable<ITrakHoundDefinitionEntity>> QueryRootByChildUuid(IEnumerable<string> childUuids, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
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

        #region "Query by Type"

        //public async Task<IEnumerable<ITrakHoundDefinitionEntityModel>> QueryModelsByType(string type, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null) => default;
        //public async Task<IEnumerable<ITrakHoundDefinitionEntityModel>> QueryModelsByType(IEnumerable<string> types, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null) => default;

        //public async Task<IEnumerable<ITrakHoundDefinitionEntityModel>> QueryPartialModelsByType(string type, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null) => default;
        //public async Task<IEnumerable<ITrakHoundDefinitionEntityModel>> QueryPartialModelsByType(IEnumerable<string> types, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null) => default;

        public async Task<IEnumerable<ITrakHoundDefinitionEntity>> QueryByType(string type, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null) => default;
        public async Task<IEnumerable<ITrakHoundDefinitionEntity>> QueryByType(IEnumerable<string> types, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null) => default;

        public async Task<IEnumerable<ITrakHoundDefinitionQueryResult>> QueryIdsByType(string type, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null) => default;
        public async Task<IEnumerable<ITrakHoundDefinitionQueryResult>> QueryIdsByType(IEnumerable<string> types, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null) => default;

        #endregion

    }
}
