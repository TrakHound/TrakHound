// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrakHound.Entities;

namespace TrakHound.Clients
{
    internal partial class TrakHoundInstanceSystemDefinitionClient
    {

        #region "Query"

        public async Task<IEnumerable<ITrakHoundDefinitionEntity>> Query(string pattern, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router.Entities.Definitions.Definitions.Query(pattern, skip, take, sortOrder);
                if (response.IsSuccess)
                {
                    return response.Content.ToDistinct();
                }
            }

            return Enumerable.Empty<ITrakHoundDefinitionEntity>();
        }

        #endregion

        #region "Query by Parent"

        public async Task<IEnumerable<ITrakHoundDefinitionEntity>> QueryByParentUuid(string parentUuid, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        {
            if (!string.IsNullOrEmpty(parentUuid))
            {
                var router = BaseClient.GetRouter(routerId);
                if (router != null)
                {
                    var parentUuids = new string[] { parentUuid };
                    var response = await router?.Entities.Definitions.Definitions.QueryByParentUuid(parentUuids, skip, take, sortOrder);
                    if (response.IsSuccess)
                    {
                        return response.Content.ToDistinct();
                    }
                }
            }

            return Enumerable.Empty<ITrakHoundDefinitionEntity>();
        }

        public async Task<IEnumerable<ITrakHoundDefinitionEntity>> QueryByParentUuid(IEnumerable<string> parentUuids, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router?.Entities.Definitions.Definitions.QueryByParentUuid(parentUuids, skip, take, sortOrder);
                if (response.IsSuccess)
                {
                    return response.Content.ToDistinct();
                }
            }

            return Enumerable.Empty<ITrakHoundDefinitionEntity>();
        }

        #endregion

        #region "Query by Child"

        public async Task<IEnumerable<ITrakHoundDefinitionEntity>> QueryByChildUuid(string childUuid, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        {
            if (!string.IsNullOrEmpty(childUuid))
            {
                var router = BaseClient.GetRouter(routerId);
                if (router != null)
                {
                    var childUuids = new string[] { childUuid };
                    var response = await router?.Entities.Definitions.Definitions.QueryByChildUuid(childUuids, skip, take, sortOrder);
                    if (response.IsSuccess)
                    {
                        return response.Content.ToDistinct();
                    }
                }
            }

            return Enumerable.Empty<ITrakHoundDefinitionEntity>();
        }

        public async Task<IEnumerable<ITrakHoundDefinitionEntity>> QueryByChildUuid(IEnumerable<string> childUuids, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router?.Entities.Definitions.Definitions.QueryByChildUuid(childUuids, skip, take, sortOrder);
                if (response.IsSuccess)
                {
                    return response.Content.ToDistinct();
                }
            }

            return Enumerable.Empty<ITrakHoundDefinitionEntity>();
        }

        #endregion

        #region "Query Children by Root"

        public async Task<IEnumerable<ITrakHoundDefinitionEntity>> QueryChildrenByRootUuid(string rootUuid, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        {
            if (!string.IsNullOrEmpty(rootUuid))
            {
                //var router = BaseClient.GetRouter(routerId);
                //if (router != null)
                //{
                //    var rootUuids = new string[] { rootUuid };
                //    var response = await router?.Entities.Definitions.Instances.QueryChildrenByRootUuid(rootUuids, skip, take, sortOrder);
                //    if (response.IsSuccess)
                //    {
                //        return response.Content.ToDistinct();
                //    }
                //}
            }

            return Enumerable.Empty<ITrakHoundDefinitionEntity>();
        }

        public async Task<IEnumerable<ITrakHoundDefinitionEntity>> QueryChildrenByRootUuid(IEnumerable<string> rootUuids, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                //var response = await router?.Entities.Definitions.Instances.QueryChildrenByRootUuid(rootUuids, skip, take, sortOrder);
                //if (response.IsSuccess)
                //{
                //    return response.Content.ToDistinct();
                //}
            }

            return Enumerable.Empty<ITrakHoundDefinitionEntity>();
        }


        public async Task<IEnumerable<ITrakHoundObjectQueryResult>> QueryChildUuidsByRootUuid(string rootUuid, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                //var rootUuids = new string[] { rootUuid };
                //var response = await router?.Entities.Definitions.Instances.QueryChildUuidsByRootUuid(rootUuids, skip, take, sortOrder);
                //if (response.IsSuccess)
                //{
                //    return response.Content;
                //}
            }

            return Enumerable.Empty<ITrakHoundObjectQueryResult>();
        }

        public async Task<IEnumerable<ITrakHoundObjectQueryResult>> QueryChildUuidsByRootUuid(IEnumerable<string> rootUuids, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                //var response = await router?.Entities.Definitions.Instances.QueryChildUuidsByRootUuid(rootUuids, skip, take, sortOrder);
                //if (response.IsSuccess)
                //{
                //    return response.Content;
                //}
            }

            return Enumerable.Empty<ITrakHoundObjectQueryResult>();
        }

        #endregion

        #region "Query Root by Child"

        public async Task<IEnumerable<ITrakHoundDefinitionEntity>> QueryRootByChildUuid(string childUuid, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        {
            if (!string.IsNullOrEmpty(childUuid))
            {
                var router = BaseClient.GetRouter(routerId);
                if (router != null)
                {
                    //var childUuids = new string[] { childUuid };
                    //var response = await router?.Entities.Definitions.Instances.QueryRootByChildUuid(childUuids, skip, take, sortOrder);
                    //if (response.IsSuccess)
                    //{
                    //    return response.Content.ToDistinct();
                    //}
                }
            }

            return Enumerable.Empty<ITrakHoundDefinitionEntity>();
        }

        public async Task<IEnumerable<ITrakHoundDefinitionEntity>> QueryRootByChildUuid(IEnumerable<string> childUuids, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                //var response = await router?.Entities.Definitions.Instances.QueryRootByChildUuid(childUuids, skip, take, sortOrder);
                //if (response.IsSuccess)
                //{
                //    return response.Content.ToDistinct();
                //}
            }

            return Enumerable.Empty<ITrakHoundDefinitionEntity>();
        }


        public async Task<IEnumerable<ITrakHoundObjectQueryResult>> QueryRootUuidsByChildUuid(string childUuid, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                //var childUuids = new string[] { childUuid };
                //var response = await router?.Entities.Definitions.Instances.QueryRootUuidsByChildUuid(childUuids, skip, take, sortOrder);
                //if (response.IsSuccess)
                //{
                //    return response.Content;
                //}
            }

            return Enumerable.Empty<ITrakHoundObjectQueryResult>();
        }

        public async Task<IEnumerable<ITrakHoundObjectQueryResult>> QueryRootUuidsByChildUuid(IEnumerable<string> childUuids, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                //var response = await router?.Entities.Definitions.Instances.QueryRootUuidsByChildUuid(childUuids, skip, take, sortOrder);
                //if (response.IsSuccess)
                //{
                //    return response.Content;
                //}
            }

            return Enumerable.Empty<ITrakHoundObjectQueryResult>();
        }

        #endregion

        #region "Query by Type"

        public async Task<IEnumerable<ITrakHoundDefinitionEntity>> QueryByType(string type, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        {
            if (!string.IsNullOrEmpty(type))
            {
                var router = BaseClient.GetRouter(routerId);
                if (router != null)
                {
                    var types = new string[] { type };
                    var response = await router?.Entities.Definitions.Definitions.QueryByType(types, skip, take, sortOrder);
                    if (response.IsSuccess)
                    {
                        return response.Content.ToDistinct();
                    }
                }
            }

            return Enumerable.Empty<ITrakHoundDefinitionEntity>();
        }

        public async Task<IEnumerable<ITrakHoundDefinitionEntity>> QueryByType(IEnumerable<string> types, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router.Entities.Definitions.Definitions.QueryByType(types, skip, take, sortOrder);
                if (response.IsSuccess)
                {
                    return response.Content.ToDistinct();
                }
            }

            return Enumerable.Empty<ITrakHoundDefinitionEntity>();
        }


        public async Task<IEnumerable<ITrakHoundDefinitionQueryResult>> QueryIdsByType(string type, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        {
            if (!string.IsNullOrEmpty(type))
            {
                var router = BaseClient.GetRouter(routerId);
                if (router != null)
                {
                    var types = new string[] { type };
                    var response = await router?.Entities.Definitions.Definitions.QueryIdsByType(types, skip, take, sortOrder);
                    if (response.IsSuccess)
                    {
                        var results = new List<ITrakHoundDefinitionQueryResult>();

                        foreach (var result in response.Results)
                        {
                            if (result.Content != null)
                            {
                                results.Add(result.Content);
                            }
                        }

                        return results;
                    }
                }
            }

            return Enumerable.Empty<ITrakHoundDefinitionQueryResult>();
        }

        public async Task<IEnumerable<ITrakHoundDefinitionQueryResult>> QueryIdsByType(IEnumerable<string> types, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router.Entities.Definitions.Definitions.QueryIdsByType(types, skip, take, sortOrder);
                if (response.IsSuccess)
                {
                    var results = new List<ITrakHoundDefinitionQueryResult>();

                    foreach (var result in response.Results)
                    {
                        if (result.Content != null)
                        {
                            results.Add(result.Content);
                        }
                    }

                    return results;
                }
            }

            return Enumerable.Empty<ITrakHoundDefinitionQueryResult>();
        }

        #endregion

    }
}
