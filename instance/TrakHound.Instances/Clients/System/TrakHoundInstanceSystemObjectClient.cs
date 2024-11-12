// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrakHound.Entities;
using TrakHound.Entities.Collections;

namespace TrakHound.Clients
{
    internal partial class TrakHoundInstanceSystemObjectClient
    {

        #region "Query"

        public async Task<IEnumerable<ITrakHoundObjectEntity>> Query(string path, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        {
            if (!string.IsNullOrEmpty(path))
            {
                var router = BaseClient.GetRouter(routerId);
                if (router != null)
                {
                    var paths = new string[] { path };
                    var response = await router?.Entities.Objects.Objects.Query(paths, skip, take);
                    if (response.IsSuccess)
                    {
                        return response.Content.ToDistinct();
                    }
                }
            }

            return Enumerable.Empty<ITrakHoundObjectEntity>();
        }

        public async Task<IEnumerable<ITrakHoundObjectEntity>> Query(IEnumerable<string> paths, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        {
            if (!paths.IsNullOrEmpty())
            {
                var router = BaseClient.GetRouter(routerId);
                if (router != null)
                {
                    var response = await router?.Entities.Objects.Objects.Query(paths, skip, take);
                    if (response.IsSuccess)
                    {
                        return response.Content.ToDistinct();
                    }
                }
            }

            return Enumerable.Empty<ITrakHoundObjectEntity>();
        }

        public async Task<IEnumerable<ITrakHoundObjectQueryResult>> QueryUuids(string path, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        {
            if (!string.IsNullOrEmpty(path))
            {
                var router = BaseClient.GetRouter(routerId);
                if (router != null)
                {
                    var paths = new string[] { path };
                    var response = await router?.Entities.Objects.Objects.QueryUuids(paths, skip, take);
                    if (response.IsSuccess)
                    {
                        return response.Content;
                    }
                }
            }

            return Enumerable.Empty<ITrakHoundObjectQueryResult>();
        }

        //public async Task<IEnumerable<ITrakHoundObjectQueryResult>> QueryUuids(string path, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        //{
        //    var results = await TrakHoundObjectExpression.Evaluate(_baseClient.System.Entities, path, skip, take, sortOrder);
        //    if (!results.IsNullOrEmpty())
        //    {
        //        var queryResults = new List<ITrakHoundObjectQueryResult>();

        //        foreach (var result in results)
        //        {
        //            queryResults.Add(new TrakHoundObjectQueryResult(TrakHoundObjectQueryRequestType.Expression, result.Expression, result.Uuid));
        //        }

        //        return queryResults;
        //    }

        //    return Enumerable.Empty<ITrakHoundObjectQueryResult>();
        //}

        public async Task<IEnumerable<ITrakHoundObjectQueryResult>> QueryUuids(IEnumerable<string> paths, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        {
            if (!paths.IsNullOrEmpty())
            {
                var router = BaseClient.GetRouter(routerId);
                if (router != null)
                {
                    var response = await router?.Entities.Objects.Objects.QueryUuids(paths, skip, take);
                    if (response.IsSuccess)
                    {
                        return response.Content;
                    }
                }
            }

            return Enumerable.Empty<ITrakHoundObjectQueryResult>();
        }

        //public async Task<IEnumerable<ITrakHoundObjectQueryResult>> QueryUuids(IEnumerable<string> paths, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        //{
        //    var results = await TrakHoundObjectExpression.Evaluate(_baseClient.System.Entities, paths, skip, take, sortOrder);
        //    if (!results.IsNullOrEmpty())
        //    {
        //        var queryResults = new List<ITrakHoundObjectQueryResult>();

        //        foreach (var result in results)
        //        {
        //            queryResults.Add(new TrakHoundObjectQueryResult(TrakHoundObjectQueryRequestType.Expression, result.Expression, result.Uuid));
        //        }

        //        return queryResults;
        //    }

        //    return Enumerable.Empty<ITrakHoundObjectQueryResult>();
        //}


        public async Task<IEnumerable<ITrakHoundObjectEntity>> Query(TrakHoundObjectQueryRequest queryRequest, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router?.Entities.Objects.Objects.Query(queryRequest, skip, take, sortOrder);
                if (response.IsSuccess)
                {
                    return response.Content.ToDistinct();
                }
            }

            return Enumerable.Empty<ITrakHoundObjectEntity>();
        }

        public async Task<IEnumerable<ITrakHoundObjectQueryResult>> QueryUuids(TrakHoundObjectQueryRequest queryRequest, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router?.Entities.Objects.Objects.QueryUuidsByQueryRequest(queryRequest, skip, take, sortOrder);
                if (response.IsSuccess)
                {
                    var results = new List<ITrakHoundObjectQueryResult>();

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

            return Enumerable.Empty<ITrakHoundObjectQueryResult>();
        }


        public async Task<IEnumerable<string>> QueryMatch(string query, IEnumerable<string> objectUuids, string routerId = null)
        {
            if (!string.IsNullOrEmpty(query))
            {
                var entities = await ReadByUuid(objectUuids);
                if (!entities.IsNullOrEmpty())
                {
                    var collection = new TrakHoundEntityCollection();
                    collection.Add(entities);

                    // Read Root Objects (to use for Expression Evaluation)
                    collection.Add(await QueryRootByChildUuid(objectUuids));
                    //collection.Add(await QueryRootPartialsByChildUuid(objectUuids));

                    var matches = TrakHoundExpression.Match(query, collection);
                    //var matches = TrakHoundObjectExpression.Match(query, entities);
                    if (!matches.IsNullOrEmpty())
                    {
                        return matches.Select(o => o.Uuid);
                    }
                }
            }

            return Enumerable.Empty<string>();
        }

        public async Task<IEnumerable<TrakHoundTargetResult>> QueryMatch(IEnumerable<string> queries, IEnumerable<string> objectUuids, string routerId = null)
        {
            if (!queries.IsNullOrEmpty())
            {
                var results = new List<TrakHoundTargetResult>();

                var entities = await ReadByUuid(objectUuids);
                if (!entities.IsNullOrEmpty())
                {
                    var collection = new TrakHoundEntityCollection();
                    collection.Add(entities);

                    // Read Root Objects (to use for Expression Evaluation)
                    collection.Add(await QueryRootByChildUuid(objectUuids));
                    //collection.Add(await QueryRootPartialsByChildUuid(objectUuids));

                    foreach (var query in queries.ToList())
                    {
                        var matches = TrakHoundExpression.Match(query, collection);
                        //var matches = TrakHoundObjectExpression.Match(query, entities);
                        if (!matches.IsNullOrEmpty())
                        {
                            foreach (var match in matches)
                            {
                                results.Add(new TrakHoundTargetResult(query, match.Uuid));
                            }
                        }
                    }
                }

                return results;
            }

            return Enumerable.Empty<TrakHoundTargetResult>();
        }

        #endregion

        #region "Query Root"

        public async Task<IEnumerable<ITrakHoundObjectEntity>> QueryRoot(long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router?.Entities.Objects.Objects.QueryRoot(skip, take, sortOrder);
                if (response.IsSuccess)
                {
                    return response.Content.ToDistinct();
                }
            }

            return Enumerable.Empty<ITrakHoundObjectEntity>();
        }

        public async Task<IEnumerable<ITrakHoundObjectEntity>> QueryRoot(string ns, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        {
            if (!string.IsNullOrEmpty(ns))
            {
                var router = BaseClient.GetRouter(routerId);
                if (router != null)
                {
                    var namespaces = new string[] { ns };
                    var response = await router?.Entities.Objects.Objects.QueryRoot(namespaces, skip, take, sortOrder);
                    if (response.IsSuccess)
                    {
                        return response.Content.ToDistinct();
                    }
                }
            }

            return Enumerable.Empty<ITrakHoundObjectEntity>();
        }

        public async Task<IEnumerable<ITrakHoundObjectEntity>> QueryRoot(IEnumerable<string> namespaces, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router?.Entities.Objects.Objects.QueryRoot(namespaces, skip, take, sortOrder);
                if (response.IsSuccess)
                {
                    return response.Content.ToDistinct();
                }
            }

            return Enumerable.Empty<ITrakHoundObjectEntity>();
        }


        public async Task<IEnumerable<ITrakHoundObjectQueryResult>> QueryRootUuids(long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router?.Entities.Objects.Objects.QueryRootUuids(skip, take, sortOrder);
                if (response.IsSuccess)
                {
                    return response.Content;
                }
            }

            return Enumerable.Empty<ITrakHoundObjectQueryResult>();
        }

        public async Task<IEnumerable<ITrakHoundObjectQueryResult>> QueryRootUuids(string ns, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var namespaces = new string[] { ns };
                var response = await router?.Entities.Objects.Objects.QueryRootUuids(namespaces, skip, take, sortOrder);
                if (response.IsSuccess)
                {
                    return response.Content;
                }
            }

            return Enumerable.Empty<ITrakHoundObjectQueryResult>();
        }

        public async Task<IEnumerable<ITrakHoundObjectQueryResult>> QueryRootUuids(IEnumerable<string> namespaces, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router?.Entities.Objects.Objects.QueryRootUuids(namespaces, skip, take, sortOrder);
                if (response.IsSuccess)
                {
                    return response.Content;
                }
            }

            return Enumerable.Empty<ITrakHoundObjectQueryResult>();
        }

        #endregion

        #region "Query by Parent"

        public async Task<IEnumerable<ITrakHoundObjectEntity>> QueryByParentUuid(string parentUuid, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        {
            if (!string.IsNullOrEmpty(parentUuid))
            {
                var router = BaseClient.GetRouter(routerId);
                if (router != null)
                {
                    var parentUuids = new string[] { parentUuid };
                    var response = await router?.Entities.Objects.Objects.QueryByParentUuid(parentUuids, skip, take, sortOrder);
                    if (response.IsSuccess)
                    {
                        return response.Content.ToDistinct();
                    }
                }
            }

            return Enumerable.Empty<ITrakHoundObjectEntity>();
        }

        public async Task<IEnumerable<ITrakHoundObjectEntity>> QueryByParentUuid(IEnumerable<string> parentUuids, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router?.Entities.Objects.Objects.QueryByParentUuid(parentUuids, skip, take, sortOrder);
                if (response.IsSuccess)
                {
                    return response.Content.ToDistinct();
                }
            }

            return Enumerable.Empty<ITrakHoundObjectEntity>();
        }


        public async Task<IEnumerable<ITrakHoundObjectQueryResult>> QueryUuidsByParentUuid(string parentUuid, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var parentUuids = new string[] { parentUuid };
                var response = await router?.Entities.Objects.Objects.QueryUuidsByParentUuid(parentUuids, skip, take, sortOrder);
                if (response.IsSuccess)
                {
                    return response.Content;
                }
            }

            return Enumerable.Empty<ITrakHoundObjectQueryResult>();
        }

        public async Task<IEnumerable<ITrakHoundObjectQueryResult>> QueryUuidsByParentUuid(IEnumerable<string> parentUuids, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router?.Entities.Objects.Objects.QueryUuidsByParentUuid(parentUuids, skip, take, sortOrder);
                if (response.IsSuccess)
                {
                    return response.Content;
                }
            }

            return Enumerable.Empty<ITrakHoundObjectQueryResult>();
        }

        #endregion

        #region "Query by Child"

        public async Task<IEnumerable<ITrakHoundObjectEntity>> QueryByChildUuid(string childUuid, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        {
            if (!string.IsNullOrEmpty(childUuid))
            {
                var router = BaseClient.GetRouter(routerId);
                if (router != null)
                {
                    var childUuids = new string[] { childUuid };
                    var response = await router?.Entities.Objects.Objects.QueryByChildUuid(childUuids, skip, take, sortOrder);
                    if (response.IsSuccess)
                    {
                        return response.Content.ToDistinct();
                    }
                }
            }

            return Enumerable.Empty<ITrakHoundObjectEntity>();
        }

        public async Task<IEnumerable<ITrakHoundObjectEntity>> QueryByChildUuid(IEnumerable<string> childUuids, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router?.Entities.Objects.Objects.QueryByChildUuid(childUuids, skip, take, sortOrder);
                if (response.IsSuccess)
                {
                    return response.Content.ToDistinct();
                }
            }

            return Enumerable.Empty<ITrakHoundObjectEntity>();
        }


        public async Task<IEnumerable<ITrakHoundObjectQueryResult>> QueryUuidsByChildUuid(string childUuid, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var childUuids = new string[] { childUuid };
                var response = await router?.Entities.Objects.Objects.QueryUuidsByChildUuid(childUuids, skip, take, sortOrder);
                if (response.IsSuccess)
                {
                    return response.Content;
                }
            }

            return Enumerable.Empty<ITrakHoundObjectQueryResult>();
        }

        public async Task<IEnumerable<ITrakHoundObjectQueryResult>> QueryUuidsByChildUuid(IEnumerable<string> childUuids, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router?.Entities.Objects.Objects.QueryUuidsByChildUuid(childUuids, skip, take, sortOrder);
                if (response.IsSuccess)
                {
                    return response.Content;
                }
            }

            return Enumerable.Empty<ITrakHoundObjectQueryResult>();
        }

        #endregion

        #region "Query Children by Root"

        public async Task<IEnumerable<ITrakHoundObjectEntity>> QueryChildrenByRootUuid(string rootUuid, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        {
            if (!string.IsNullOrEmpty(rootUuid))
            {
                var router = BaseClient.GetRouter(routerId);
                if (router != null)
                {
                    var rootUuids = new string[] { rootUuid };
                    var response = await router?.Entities.Objects.Objects.QueryChildrenByRootUuid(rootUuids, skip, take, sortOrder);
                    if (response.IsSuccess)
                    {
                        return response.Content.ToDistinct();
                    }
                }
            }

            return Enumerable.Empty<ITrakHoundObjectEntity>();
        }

        public async Task<IEnumerable<ITrakHoundObjectEntity>> QueryChildrenByRootUuid(IEnumerable<string> rootUuids, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router?.Entities.Objects.Objects.QueryChildrenByRootUuid(rootUuids, skip, take, sortOrder);
                if (response.IsSuccess)
                {
                    return response.Content.ToDistinct();
                }
            }

            return Enumerable.Empty<ITrakHoundObjectEntity>();
        }


        public async Task<IEnumerable<ITrakHoundObjectQueryResult>> QueryChildUuidsByRootUuid(string rootUuid, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var rootUuids = new string[] { rootUuid };
                var response = await router?.Entities.Objects.Objects.QueryChildUuidsByRootUuid(rootUuids, skip, take, sortOrder);
                if (response.IsSuccess)
                {
                    return response.Content;
                }
            }

            return Enumerable.Empty<ITrakHoundObjectQueryResult>();
        }

        public async Task<IEnumerable<ITrakHoundObjectQueryResult>> QueryChildUuidsByRootUuid(IEnumerable<string> rootUuids, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router?.Entities.Objects.Objects.QueryChildUuidsByRootUuid(rootUuids, skip, take, sortOrder);
                if (response.IsSuccess)
                {
                    return response.Content;
                }
            }

            return Enumerable.Empty<ITrakHoundObjectQueryResult>();
        }

        #endregion

        #region "Query Root by Child"

        public async Task<IEnumerable<ITrakHoundObjectEntity>> QueryRootByChildUuid(string childUuid, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        {
            if (!string.IsNullOrEmpty(childUuid))
            {
                var router = BaseClient.GetRouter(routerId);
                if (router != null)
                {
                    var childUuids = new string[] { childUuid };
                    var response = await router?.Entities.Objects.Objects.QueryRootByChildUuid(childUuids, skip, take, sortOrder);
                    if (response.IsSuccess)
                    {
                        return response.Content.ToDistinct();
                    }
                }
            }

            return Enumerable.Empty<ITrakHoundObjectEntity>();
        }

        public async Task<IEnumerable<ITrakHoundObjectEntity>> QueryRootByChildUuid(IEnumerable<string> childUuids, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router?.Entities.Objects.Objects.QueryRootByChildUuid(childUuids, skip, take, sortOrder);
                if (response.IsSuccess)
                {
                    return response.Content.ToDistinct();
                }
            }

            return Enumerable.Empty<ITrakHoundObjectEntity>();
        }


        public async Task<IEnumerable<ITrakHoundObjectQueryResult>> QueryRootUuidsByChildUuid(string childUuid, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var childUuids = new string[] { childUuid };
                var response = await router?.Entities.Objects.Objects.QueryRootUuidsByChildUuid(childUuids, skip, take, sortOrder);
                if (response.IsSuccess)
                {
                    return response.Content;
                }
            }

            return Enumerable.Empty<ITrakHoundObjectQueryResult>();
        }

        public async Task<IEnumerable<ITrakHoundObjectQueryResult>> QueryRootUuidsByChildUuid(IEnumerable<string> childUuids, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router?.Entities.Objects.Objects.QueryRootUuidsByChildUuid(childUuids, skip, take, sortOrder);
                if (response.IsSuccess)
                {
                    return response.Content;
                }
            }

            return Enumerable.Empty<ITrakHoundObjectQueryResult>();
        }

        #endregion

        #region "Namespaces"

        public async Task<IEnumerable<string>> ListNamespaces(long skip = 0, long take = 0, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router?.Entities.Objects.Objects.ListNamespaces(skip, take, sortOrder);
                if (response.IsSuccess)
                {
                    return response.Content;
                }
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
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router.Entities.Objects.Objects.Subscribe(paths);
                if (response.IsSuccess)
                {
                    return new TrakHoundConsumer<IEnumerable<ITrakHoundObjectEntity>>(response.Content);
                }
            }

            return null;

        }

        public async Task<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectEntity>>> SubscribeByUuid(
            IEnumerable<string> uuids,
            int interval = 0,
            int take = 1000,
            string consumerId = null,
            string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router.Entities.Objects.Objects.SubscribeByUuid(uuids);
                if (response.IsSuccess)
                {
                    return new TrakHoundConsumer<IEnumerable<ITrakHoundObjectEntity>>(response.Content);
                }
            }

            return null;

        }

        #endregion

        #region "Count"

        public async Task<TrakHoundCount> QueryChildCount(string uuid, string routerId = null)
        {
            if (!string.IsNullOrEmpty(uuid))
            {
                var router = BaseClient.GetRouter(routerId);
                if (router != null)
                {
                    var uuids = new string[] { uuid };
                    var response = await router?.Entities.Objects.Objects.QueryChildCount(uuids);
                    if (response.IsSuccess)
                    {
                        return response.Content?.FirstOrDefault();
                    }
                }
            }

            return null;
        }

        public async Task<IEnumerable<TrakHoundCount>> QueryChildCount(IEnumerable<string> uuids, string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router?.Entities.Objects.Objects.QueryChildCount(uuids);
                if (response.IsSuccess)
                {
                    return response.Content;
                }
            }

            return Enumerable.Empty<TrakHoundCount>();
        }

        #endregion

        #region "Notify"

        public async Task<ITrakHoundConsumer<IEnumerable<TrakHoundEntityNotification>>> Notify(string path, TrakHoundEntityNotificationType notificationType = TrakHoundEntityNotificationType.All, string routerId = null)
        {
            if (!string.IsNullOrEmpty(path))
            {
                var router = BaseClient.GetRouter(routerId);
                if (router != null)
                {
                    var response = await router?.Entities.Objects.Objects.Notify(path, notificationType);
                    if (response.IsSuccess)
                    {
                        return new TrakHoundConsumer<IEnumerable<TrakHoundEntityNotification>>(response.Content);
                    }
                }
            }

            return null;
        }

        #endregion

        #region "Index"

        public async Task<IEnumerable<string>> QueryIndex(IEnumerable<EntityIndexRequest> requests, long skip, long take, SortOrder sortOrder, string routerId = null)
        {
            if (!requests.IsNullOrEmpty())
            {
                var router = BaseClient.GetRouter(routerId);
                if (router != null)
                {
                    var response = await router?.Entities.Objects.Objects.QueryIndex(requests, skip, take, sortOrder);
                    if (response.IsSuccess)
                    {
                        return response.Content;
                    }
                }
            }

            return null;
        }

        public async Task<bool> UpdateIndex(IEnumerable<EntityIndexPublishRequest> requests, TrakHoundOperationMode mode = TrakHoundOperationMode.Async, string routerId = null)
        {
            if (!requests.IsNullOrEmpty())
            {
                var router = BaseClient.GetRouter(routerId);
                if (router != null)
                {
                    var response = await router?.Entities.Objects.Objects.UpdateIndex(requests, mode);
                    return response.IsSuccess;
                }
            }

            return false;
        }

        #endregion

        #region "Delete"

        public async Task<bool> DeleteByRootUuid(string rootUuid, string routerId = null)
        {
            if (!string.IsNullOrEmpty(rootUuid))
            {
                var router = BaseClient.GetRouter(routerId);
                if (router != null)
                {
                    var rootUuids = new string[] { rootUuid };
                    var response = await router?.Entities.Objects.Objects.DeleteByRootUuid(rootUuids);
                    return response.IsSuccess;
                }
            }

            return false;
        }

        public async Task<bool> DeleteByRootUuid(IEnumerable<string> rootUuids, string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router?.Entities.Objects.Objects.DeleteByRootUuid(rootUuids);
                return response.IsSuccess;
            }

            return false;
        }

        #endregion

        #region "Expire"

        public async Task<long> Expire(string pattern, long created, string routerId = null)
        {
            if (!string.IsNullOrEmpty(pattern))
            {
                var patterns = new string[] { pattern };
                var results = await Expire(patterns, created);
                if (!results.IsNullOrEmpty())
                {
                    return results.FirstOrDefault().Count;
                }
            }

            return 0;
        }

        public async Task<IEnumerable<EntityDeleteResult>> Expire(IEnumerable<string> patterns, long created, string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router?.Entities.Objects.Objects.Expire(patterns, created);
                return response.Content;
            }

            return null;
        }


        public async Task<long> ExpireByUpdate(string pattern, long lastUpdated, string routerId = null)
        {
            if (!string.IsNullOrEmpty(pattern))
            {
                var patterns = new string[] { pattern };
                var results = await ExpireByUpdate(patterns, lastUpdated);
                if (!results.IsNullOrEmpty())
                {
                    return results.FirstOrDefault().Count;
                }
            }

            return 0;
        }

        public async Task<IEnumerable<EntityDeleteResult>> ExpireByUpdate(IEnumerable<string> patterns, long lastUpdated, string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router?.Entities.Objects.Objects.ExpireByUpdate(patterns, lastUpdated);
                return response.Content;
            }

            return null;
        }


        public async Task<long> ExpireByAccess(string pattern, long lastAccessed, string routerId = null)
        {
            if (!string.IsNullOrEmpty(pattern))
            {
                var patterns = new string[] { pattern };
                var results = await ExpireByAccess(patterns, lastAccessed);
                if (!results.IsNullOrEmpty())
                {
                    return results.FirstOrDefault().Count;
                }
            }

            return 0;
        }

        public async Task<IEnumerable<EntityDeleteResult>> ExpireByAccess(IEnumerable<string> patterns, long lastAccessed, string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router?.Entities.Objects.Objects.ExpireByAccess(patterns, lastAccessed);
                return response.Content;
            }

            return null;
        }

        #endregion

    }
}
