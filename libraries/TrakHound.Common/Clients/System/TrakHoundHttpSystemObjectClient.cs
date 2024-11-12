// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrakHound.Entities;
using TrakHound.Http;

namespace TrakHound.Clients
{
    internal partial class TrakHoundHttpSystemObjectClient
    {

        #region "Query"

        public async Task<IEnumerable<ITrakHoundObjectEntity>> Query(string path, long skip = 0, long take = 1000, SortOrder order = SortOrder.Ascending, string routerId = null)
        {
            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectEntity>());
            url = Url.AddQueryParameter(url, "path", path);
            url = Url.AddQueryParameter(url, "skip", skip);
            url = Url.AddQueryParameter(url, "take", take);
            url = Url.AddQueryParameter(url, "order", (int)order);
            url = Url.AddQueryParameter(url, "routerId", BaseClient.GetRouterId(routerId));

            var array = await RestRequest.Get<object[][]>(url);
            return ProcessResponse(array);
        }

        public async Task<IEnumerable<ITrakHoundObjectEntity>> Query(IEnumerable<string> paths, long skip = 0, long take = 1000, SortOrder order = SortOrder.Ascending, string routerId = null)
        {
            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectEntity>());
            url = Url.Combine(url, "path");
            url = Url.AddQueryParameter(url, "skip", skip);
            url = Url.AddQueryParameter(url, "take", take);
            url = Url.AddQueryParameter(url, "order", (int)order);
            url = Url.AddQueryParameter(url, "routerId", BaseClient.GetRouterId(routerId));

            var array = await RestRequest.Post<object[][]>(url, paths);
            return ProcessResponse(array);
        }

        public async Task<IEnumerable<ITrakHoundObjectQueryResult>> QueryUuids(string path, long skip = 0, long take = 1000, SortOrder order = SortOrder.Ascending, string routerId = null)
        {
            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectEntity>());
            url = Url.Combine(url, "ids");
            url = Url.AddQueryParameter(url, "path", path);
            url = Url.AddQueryParameter(url, "skip", skip);
            url = Url.AddQueryParameter(url, "take", take);
            url = Url.AddQueryParameter(url, "order", (int)order);
            url = Url.AddQueryParameter(url, "routerId", BaseClient.GetRouterId(routerId));

            var httpResults = await RestRequest.Get<IEnumerable<TrakHoundHttpObjectQueryResult>>(url);
            if (!httpResults.IsNullOrEmpty())
            {
                var results = new List<ITrakHoundObjectQueryResult>();

                foreach (var httpResult in httpResults)
                {
                    results.Add(httpResult.ToQueryResult());
                }

                return results;
            }

            return null;
        }

        public async Task<IEnumerable<ITrakHoundObjectQueryResult>> QueryUuids(IEnumerable<string> paths, long skip = 0, long take = 1000, SortOrder order = SortOrder.Ascending, string routerId = null)
        {
            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectEntity>());
            url = Url.Combine(url, "ids");
            url = Url.Combine(url, "path");
            url = Url.AddQueryParameter(url, "skip", skip);
            url = Url.AddQueryParameter(url, "take", take);
            url = Url.AddQueryParameter(url, "order", (int)order);
            url = Url.AddQueryParameter(url, "routerId", BaseClient.GetRouterId(routerId));

            var httpResults = await RestRequest.Post<IEnumerable<TrakHoundHttpObjectQueryResult>>(url, paths);
            if (!httpResults.IsNullOrEmpty())
            {
                var results = new List<ITrakHoundObjectQueryResult>();

                foreach (var httpResult in httpResults)
                {
                    results.Add(httpResult.ToQueryResult());
                }

                return results;
            }

            return null;
        }


        public async Task<IEnumerable<string>> QueryMatch(string query, IEnumerable<string> objectUuids, string routerId = null)
        {
            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectEntity>());
            url = Url.Combine(url, "match");
            url = Url.AddQueryParameter(url, "q", query);
            url = Url.AddQueryParameter(url, "routerId", BaseClient.GetRouterId(routerId));

            return await RestRequest.Post<string[]>(url, objectUuids);
        }

        public async Task<IEnumerable<TrakHoundTargetResult>> QueryMatch(IEnumerable<string> queries, IEnumerable<string> objectUuids, string routerId = null)
        {
            //var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectEntity>());
            //url = Url.Combine(url, "match");
            //url = Url.AddQueryParameter(url, "routerId", BaseClient.GetRouterId(routerId));

            //return await RestRequest.Post<IEnumerable<TrakHoundMatchQueryResult>>(url, objectUuids);

            return Enumerable.Empty<TrakHoundTargetResult>();
        }

        #endregion

        #region "Query Root"

        public async Task<IEnumerable<ITrakHoundObjectEntity>> QueryRoot(long skip = 0, long take = 1000, SortOrder order = SortOrder.Ascending, string routerId = null)
        {
            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectEntity>());
            url = Url.Combine(url, "root");
            url = Url.AddQueryParameter(url, "skip", skip);
            url = Url.AddQueryParameter(url, "take", take);
            url = Url.AddQueryParameter(url, "order", (int)order);
            url = Url.AddQueryParameter(url, "routerId", BaseClient.GetRouterId(routerId));

            var array = await RestRequest.Get<object[][]>(url);
            return ProcessResponse(array);
        }

        public async Task<IEnumerable<ITrakHoundObjectEntity>> QueryRoot(string ns, long skip = 0, long take = 1000, SortOrder order = SortOrder.Ascending, string routerId = null)
        {
            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectEntity>());
            url = Url.Combine(url, "root");
            url = Url.AddQueryParameter(url, "skip", skip);
            url = Url.AddQueryParameter(url, "take", take);
            url = Url.AddQueryParameter(url, "order", (int)order);
            url = Url.AddQueryParameter(url, "routerId", BaseClient.GetRouterId(routerId));

            var namespaces = !string.IsNullOrEmpty(ns) ? new string[] { ns } : null;

            var array = await RestRequest.Post<object[][]>(url, namespaces);
            return ProcessResponse(array);
        }

        public async Task<IEnumerable<ITrakHoundObjectEntity>> QueryRoot(IEnumerable<string> namespaces, long skip = 0, long take = 1000, SortOrder order = SortOrder.Ascending, string routerId = null)
        {
            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectEntity>());
            url = Url.Combine(url, "root");
            url = Url.AddQueryParameter(url, "skip", skip);
            url = Url.AddQueryParameter(url, "take", take);
            url = Url.AddQueryParameter(url, "order", (int)order);
            url = Url.AddQueryParameter(url, "routerId", BaseClient.GetRouterId(routerId));

            var array = await RestRequest.Post<object[][]>(url, namespaces);
            return ProcessResponse(array);
        }


        public async Task<IEnumerable<ITrakHoundObjectQueryResult>> QueryRootUuids(long skip = 0, long take = 1000, SortOrder order = SortOrder.Ascending, string routerId = null)
        {
            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectEntity>());
            url = Url.Combine(url, "ids");
            url = Url.Combine(url, "root");
            url = Url.AddQueryParameter(url, "skip", skip);
            url = Url.AddQueryParameter(url, "take", take);
            url = Url.AddQueryParameter(url, "order", (int)order);
            url = Url.AddQueryParameter(url, "routerId", BaseClient.GetRouterId(routerId));

            var httpResults = await RestRequest.Get<IEnumerable<TrakHoundHttpObjectQueryResult>>(url);
            if (!httpResults.IsNullOrEmpty())
            {
                var results = new List<ITrakHoundObjectQueryResult>();

                foreach (var httpResult in httpResults)
                {
                    results.Add(httpResult.ToQueryResult());
                }

                return results;
            }

            return null;
        }

        public async Task<IEnumerable<ITrakHoundObjectQueryResult>> QueryRootUuids(string ns, long skip = 0, long take = 1000, SortOrder order = SortOrder.Ascending, string routerId = null)
        {
            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectEntity>());
            url = Url.Combine(url, "ids");
            url = Url.Combine(url, "root");
            url = Url.AddQueryParameter(url, "skip", skip);
            url = Url.AddQueryParameter(url, "take", take);
            url = Url.AddQueryParameter(url, "order", (int)order);
            url = Url.AddQueryParameter(url, "routerId", BaseClient.GetRouterId(routerId));

            var namespaces = !string.IsNullOrEmpty(ns) ? new string[] { ns } : null;
            
            var httpResults = await RestRequest.Post<IEnumerable<TrakHoundHttpObjectQueryResult>>(url, namespaces);
            if (!httpResults.IsNullOrEmpty())
            {
                var results = new List<ITrakHoundObjectQueryResult>();

                foreach (var httpResult in httpResults)
                {
                    results.Add(httpResult.ToQueryResult());
                }

                return results;
            }

            return null;
        }

        public async Task<IEnumerable<ITrakHoundObjectQueryResult>> QueryRootUuids(IEnumerable<string> namespaces = null, long skip = 0, long take = 1000, SortOrder order = SortOrder.Ascending, string routerId = null)
        {
            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectEntity>());
            url = Url.Combine(url, "ids");
            url = Url.Combine(url, "root");
            url = Url.AddQueryParameter(url, "skip", skip);
            url = Url.AddQueryParameter(url, "take", take);
            url = Url.AddQueryParameter(url, "order", (int)order);
            url = Url.AddQueryParameter(url, "routerId", BaseClient.GetRouterId(routerId));

            var httpResults = await RestRequest.Post<IEnumerable<TrakHoundHttpObjectQueryResult>>(url, namespaces);
            if (!httpResults.IsNullOrEmpty())
            {
                var results = new List<ITrakHoundObjectQueryResult>();

                foreach (var httpResult in httpResults)
                {
                    results.Add(httpResult.ToQueryResult());
                }

                return results;
            }

            return null;
        }

        #endregion

        #region "Query By Parent"

        public async Task<IEnumerable<ITrakHoundObjectEntity>> QueryByParentUuid(string parentUuid, long skip = 0, long take = 1000, SortOrder order = SortOrder.Ascending, string routerId = null)
        {
            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectEntity>());
            url = Url.Combine(url, "parent");
            url = Url.AddQueryParameter(url, "skip", skip);
            url = Url.AddQueryParameter(url, "take", take);
            url = Url.AddQueryParameter(url, "order", (int)order);
            url = Url.AddQueryParameter(url, "routerId", BaseClient.GetRouterId(routerId));

            var array = await RestRequest.Post<object[][]>(url, new string[] { parentUuid });
            return ProcessResponse(array);
        }

        public async Task<IEnumerable<ITrakHoundObjectEntity>> QueryByParentUuid(IEnumerable<string> parentUuids, long skip = 0, long take = 1000, SortOrder order = SortOrder.Ascending, string routerId = null)
        {
            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectEntity>());
            url = Url.Combine(url, "parent");
            url = Url.AddQueryParameter(url, "skip", skip);
            url = Url.AddQueryParameter(url, "take", take);
            url = Url.AddQueryParameter(url, "order", (int)order);
            url = Url.AddQueryParameter(url, "routerId", BaseClient.GetRouterId(routerId));

            var array = await RestRequest.Post<object[][]>(url, parentUuids);
            return ProcessResponse(array);
        }


        public async Task<IEnumerable<ITrakHoundObjectQueryResult>> QueryUuidsByParentUuid(string parentUuid, long skip = 0, long take = 1000, SortOrder order = SortOrder.Ascending, string routerId = null)
        {
            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectEntity>());
            url = Url.Combine(url, "ids");
            url = Url.Combine(url, "parent");
            url = Url.AddQueryParameter(url, "skip", skip);
            url = Url.AddQueryParameter(url, "take", take);
            url = Url.AddQueryParameter(url, "order", (int)order);
            url = Url.AddQueryParameter(url, "routerId", BaseClient.GetRouterId(routerId));

            var httpResults = await RestRequest.Post<IEnumerable<TrakHoundHttpObjectQueryResult>>(url, new string[] { parentUuid });
            if (!httpResults.IsNullOrEmpty())
            {
                var results = new List<ITrakHoundObjectQueryResult>();

                foreach (var httpResult in httpResults)
                {
                    results.Add(httpResult.ToQueryResult());
                }

                return results;
            }

            return null;
        }

        public async Task<IEnumerable<ITrakHoundObjectQueryResult>> QueryUuidsByParentUuid(IEnumerable<string> parentUuids, long skip = 0, long take = 1000, SortOrder order = SortOrder.Ascending, string routerId = null)
        {
            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectEntity>());
            url = Url.Combine(url, "ids");
            url = Url.Combine(url, "parent");
            url = Url.AddQueryParameter(url, "skip", skip);
            url = Url.AddQueryParameter(url, "take", take);
            url = Url.AddQueryParameter(url, "order", (int)order);
            url = Url.AddQueryParameter(url, "routerId", BaseClient.GetRouterId(routerId));

            var httpResults = await RestRequest.Post<IEnumerable<TrakHoundHttpObjectQueryResult>>(url, parentUuids);
            if (!httpResults.IsNullOrEmpty())
            {
                var results = new List<ITrakHoundObjectQueryResult>();

                foreach (var httpResult in httpResults)
                {
                    results.Add(httpResult.ToQueryResult());
                }

                return results;
            }

            return null;
        }

        #endregion

        #region "Query by Child"

        public async Task<IEnumerable<ITrakHoundObjectEntity>> QueryByChildUuid(string childUuid, long skip = 0, long take = 1000, SortOrder order = SortOrder.Ascending, string routerId = null)
        {
            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectEntity>());
            url = Url.Combine(url, "child");
            url = Url.AddQueryParameter(url, "skip", skip);
            url = Url.AddQueryParameter(url, "take", take);
            url = Url.AddQueryParameter(url, "order", (int)order);
            url = Url.AddQueryParameter(url, "routerId", BaseClient.GetRouterId(routerId));

            var array = await RestRequest.Post<object[][]>(url, new string[] { childUuid });
            return ProcessResponse(array);
        }

        public async Task<IEnumerable<ITrakHoundObjectEntity>> QueryByChildUuid(IEnumerable<string> childUuids, long skip = 0, long take = 1000, SortOrder order = SortOrder.Ascending, string routerId = null)
        {
            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectEntity>());
            url = Url.Combine(url, "child");
            url = Url.AddQueryParameter(url, "skip", skip);
            url = Url.AddQueryParameter(url, "take", take);
            url = Url.AddQueryParameter(url, "order", (int)order);
            url = Url.AddQueryParameter(url, "routerId", BaseClient.GetRouterId(routerId));

            var array = await RestRequest.Post<object[][]>(url, childUuids);
            return ProcessResponse(array);
        }


        public async Task<IEnumerable<ITrakHoundObjectQueryResult>> QueryUuidsByChildUuid(string childUuid, long skip = 0, long take = 1000, SortOrder order = SortOrder.Ascending, string routerId = null)
        {
            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectEntity>());
            url = Url.Combine(url, "ids");
            url = Url.Combine(url, "child");
            url = Url.AddQueryParameter(url, "skip", skip);
            url = Url.AddQueryParameter(url, "take", take);
            url = Url.AddQueryParameter(url, "order", (int)order);
            url = Url.AddQueryParameter(url, "routerId", BaseClient.GetRouterId(routerId));

            var httpResults = await RestRequest.Post<IEnumerable<TrakHoundHttpObjectQueryResult>>(url, new string[] { childUuid });
            if (!httpResults.IsNullOrEmpty())
            {
                var results = new List<ITrakHoundObjectQueryResult>();

                foreach (var httpResult in httpResults)
                {
                    results.Add(httpResult.ToQueryResult());
                }

                return results;
            }

            return null;
        }

        public async Task<IEnumerable<ITrakHoundObjectQueryResult>> QueryUuidsByChildUuid(IEnumerable<string> childUuids, long skip = 0, long take = 1000, SortOrder order = SortOrder.Ascending, string routerId = null)
        {
            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectEntity>());
            url = Url.Combine(url, "ids");
            url = Url.Combine(url, "child");
            url = Url.AddQueryParameter(url, "skip", skip);
            url = Url.AddQueryParameter(url, "take", take);
            url = Url.AddQueryParameter(url, "order", (int)order);
            url = Url.AddQueryParameter(url, "routerId", BaseClient.GetRouterId(routerId));

            var httpResults = await RestRequest.Post<IEnumerable<TrakHoundHttpObjectQueryResult>>(url, childUuids);
            if (!httpResults.IsNullOrEmpty())
            {
                var results = new List<ITrakHoundObjectQueryResult>();

                foreach (var httpResult in httpResults)
                {
                    results.Add(httpResult.ToQueryResult());
                }

                return results;
            }

            return null;
        }

        #endregion

        #region "Query Children By Root"

        public async Task<IEnumerable<ITrakHoundObjectEntity>> QueryChildrenByRootUuid(string rootUuid, long skip = 0, long take = 1000, SortOrder order = SortOrder.Ascending, string routerId = null)
        {
            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectEntity>());
            url = Url.Combine(url, "root/children");
            url = Url.AddQueryParameter(url, "skip", skip);
            url = Url.AddQueryParameter(url, "take", take);
            url = Url.AddQueryParameter(url, "order", (int)order);
            url = Url.AddQueryParameter(url, "routerId", BaseClient.GetRouterId(routerId));

            var array = await RestRequest.Post<object[][]>(url, new string[] { rootUuid });
            return ProcessResponse(array);
        }

        public async Task<IEnumerable<ITrakHoundObjectEntity>> QueryChildrenByRootUuid(IEnumerable<string> rootUuids, long skip = 0, long take = 1000, SortOrder order = SortOrder.Ascending, string routerId = null)
        {
            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectEntity>());
            url = Url.Combine(url, "root/children");
            url = Url.AddQueryParameter(url, "skip", skip);
            url = Url.AddQueryParameter(url, "take", take);
            url = Url.AddQueryParameter(url, "order", (int)order);
            url = Url.AddQueryParameter(url, "routerId", BaseClient.GetRouterId(routerId));

            var array = await RestRequest.Post<object[][]>(url, rootUuids);
            return ProcessResponse(array);
        }


        public async Task<IEnumerable<ITrakHoundObjectQueryResult>> QueryChildUuidsByRootUuid(string rootUuid, long skip = 0, long take = 1000, SortOrder order = SortOrder.Ascending, string routerId = null)
        {
            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectEntity>());
            url = Url.Combine(url, "ids");
            url = Url.Combine(url, "root/children");
            url = Url.AddQueryParameter(url, "skip", skip);
            url = Url.AddQueryParameter(url, "take", take);
            url = Url.AddQueryParameter(url, "order", (int)order);
            url = Url.AddQueryParameter(url, "routerId", BaseClient.GetRouterId(routerId));

            var httpResults = await RestRequest.Post<IEnumerable<TrakHoundHttpObjectQueryResult>>(url, new string[] { rootUuid });
            if (!httpResults.IsNullOrEmpty())
            {
                var results = new List<ITrakHoundObjectQueryResult>();

                foreach (var httpResult in httpResults)
                {
                    results.Add(httpResult.ToQueryResult());
                }

                return results;
            }

            return null;
        }

        public async Task<IEnumerable<ITrakHoundObjectQueryResult>> QueryChildUuidsByRootUuid(IEnumerable<string> rootUuids, long skip = 0, long take = 1000, SortOrder order = SortOrder.Ascending, string routerId = null)
        {
            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectEntity>());
            url = Url.Combine(url, "ids");
            url = Url.Combine(url, "root/children");
            url = Url.AddQueryParameter(url, "skip", skip);
            url = Url.AddQueryParameter(url, "take", take);
            url = Url.AddQueryParameter(url, "order", (int)order);
            url = Url.AddQueryParameter(url, "routerId", BaseClient.GetRouterId(routerId));

            var httpResults = await RestRequest.Post<IEnumerable<TrakHoundHttpObjectQueryResult>>(url, rootUuids);
            if (!httpResults.IsNullOrEmpty())
            {
                var results = new List<ITrakHoundObjectQueryResult>();

                foreach (var httpResult in httpResults)
                {
                    results.Add(httpResult.ToQueryResult());
                }

                return results;
            }

            return null;
        }

        #endregion

        #region "Query Root by Children"

        public async Task<IEnumerable<ITrakHoundObjectEntity>> QueryRootByChildUuid(string childUuid, long skip = 0, long take = 1000, SortOrder order = SortOrder.Ascending, string routerId = null)
        {
            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectEntity>());
            url = Url.Combine(url, "child/root");
            url = Url.AddQueryParameter(url, "skip", skip);
            url = Url.AddQueryParameter(url, "take", take);
            url = Url.AddQueryParameter(url, "order", (int)order);
            url = Url.AddQueryParameter(url, "routerId", BaseClient.GetRouterId(routerId));

            var array = await RestRequest.Post<object[][]>(url, new string[] { childUuid });
            return ProcessResponse(array);
        }

        public async Task<IEnumerable<ITrakHoundObjectEntity>> QueryRootByChildUuid(IEnumerable<string> childUuids, long skip = 0, long take = 1000, SortOrder order = SortOrder.Ascending, string routerId = null)
        {
            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectEntity>());
            url = Url.Combine(url, "child/root");
            url = Url.AddQueryParameter(url, "skip", skip);
            url = Url.AddQueryParameter(url, "take", take);
            url = Url.AddQueryParameter(url, "order", (int)order);
            url = Url.AddQueryParameter(url, "routerId", BaseClient.GetRouterId(routerId));

            var array = await RestRequest.Post<object[][]>(url, childUuids);
            return ProcessResponse(array);
        }


        public async Task<IEnumerable<ITrakHoundObjectQueryResult>> QueryRootUuidsByChildUuid(string childUuid, long skip = 0, long take = 1000, SortOrder order = SortOrder.Ascending, string routerId = null)
        {
            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectEntity>());
            url = Url.Combine(url, "ids");
            url = Url.Combine(url, "child/root");
            url = Url.AddQueryParameter(url, "skip", skip);
            url = Url.AddQueryParameter(url, "take", take);
            url = Url.AddQueryParameter(url, "order", (int)order);
            url = Url.AddQueryParameter(url, "routerId", BaseClient.GetRouterId(routerId));

            var httpResults = await RestRequest.Post<IEnumerable<TrakHoundHttpObjectQueryResult>>(url, new string[] { childUuid });
            if (!httpResults.IsNullOrEmpty())
            {
                var results = new List<ITrakHoundObjectQueryResult>();

                foreach (var httpResult in httpResults)
                {
                    results.Add(httpResult.ToQueryResult());
                }

                return results;
            }

            return null;
        }

        public async Task<IEnumerable<ITrakHoundObjectQueryResult>> QueryRootUuidsByChildUuid(IEnumerable<string> childUuids, long skip = 0, long take = 1000, SortOrder order = SortOrder.Ascending, string routerId = null)
        {
            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectEntity>());
            url = Url.Combine(url, "ids");
            url = Url.Combine(url, "child/root");
            url = Url.AddQueryParameter(url, "skip", skip);
            url = Url.AddQueryParameter(url, "take", take);
            url = Url.AddQueryParameter(url, "order", (int)order);
            url = Url.AddQueryParameter(url, "routerId", BaseClient.GetRouterId(routerId));

            var httpResults = await RestRequest.Post<IEnumerable<TrakHoundHttpObjectQueryResult>>(url, childUuids);
            if (!httpResults.IsNullOrEmpty())
            {
                var results = new List<ITrakHoundObjectQueryResult>();

                foreach (var httpResult in httpResults)
                {
                    results.Add(httpResult.ToQueryResult());
                }

                return results;
            }

            return null;
        }

        #endregion

        #region "Query by Request"

        public async Task<IEnumerable<ITrakHoundObjectEntity>> Query(TrakHoundObjectQueryRequest queryRequest, long skip = 0, long take = 1000, SortOrder order = SortOrder.Ascending, string routerId = null)
        {
            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectEntity>());
            url = Url.Combine(url, "query");
            url = Url.AddQueryParameter(url, "skip", skip);
            url = Url.AddQueryParameter(url, "take", take);
            url = Url.AddQueryParameter(url, "order", (int)order);
            url = Url.AddQueryParameter(url, "routerId", BaseClient.GetRouterId(routerId));

            var httpQueryRequest = new TrakHoundHttpObjectQueryRequest();
            httpQueryRequest.Type = queryRequest.Type.ToString();
            httpQueryRequest.Queries = queryRequest.Queries;
            httpQueryRequest.ParentLevel = queryRequest.ParentLevel;
            httpQueryRequest.ParentUuids = queryRequest.ParentUuids;

            var array = await RestRequest.Post<object[][]>(url, httpQueryRequest);
            return ProcessResponse(array);
        }

        public async Task<IEnumerable<ITrakHoundObjectQueryResult>> QueryUuids(TrakHoundObjectQueryRequest queryRequest, long skip = 0, long take = 1000, SortOrder order = SortOrder.Ascending, string routerId = null)
        {
            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectEntity>());
            url = Url.Combine(url, "ids");
            url = Url.Combine(url, "query");
            url = Url.AddQueryParameter(url, "skip", skip);
            url = Url.AddQueryParameter(url, "take", take);
            url = Url.AddQueryParameter(url, "order", (int)order);
            url = Url.AddQueryParameter(url, "routerId", BaseClient.GetRouterId(routerId));

            var httpQueryRequest = new TrakHoundHttpObjectQueryRequest();
            httpQueryRequest.Type = queryRequest.Type.ToString();
            httpQueryRequest.Queries = queryRequest.Queries;
            httpQueryRequest.ParentLevel = queryRequest.ParentLevel;
            httpQueryRequest.ParentUuids = queryRequest.ParentUuids;

            var httpResults = await RestRequest.Post<IEnumerable<TrakHoundHttpObjectQueryResult>>(url, httpQueryRequest);
            if (!httpResults.IsNullOrEmpty())
            {
                var results = new List<ITrakHoundObjectQueryResult>();

                foreach (var httpResult in httpResults)
                {
                    results.Add(httpResult.ToQueryResult());
                }

                return results;
            }

            return null;
        }

        #endregion

        #region "Namespaces"

        public async Task<IEnumerable<string>> ListNamespaces(long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        {
            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectEntity>());
            url = Url.Combine(url, "namespaces");
            url = Url.AddQueryParameter(url, "routerId", BaseClient.GetRouterId(routerId));

            return await RestRequest.Get<IEnumerable<string>>(url);
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
            var route = "";
            route = Url.Combine(route, "path/subscribe");
            route = Url.AddQueryParameter(route, "interval", (int)interval);
            route = Url.AddQueryParameter(route, "take", (int)take);
            route = Url.AddQueryParameter(route, "consumerId", (string)consumerId);
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));
            route = Url.AddQueryParameter(route, "requestBody", "true");

            var url = TrakHoundHttp.GetEntityPath<ITrakHoundObjectEntity>();
            url = Url.Combine(url, route);

            var consumer = new TrakHoundEntityListClientConsumer<ITrakHoundObjectEntity>(BaseClient.ClientConfiguration, url, paths);
            consumer.Subscribe();
            return consumer;
        }

        public async Task<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectEntity>>> SubscribeByUuid(
            IEnumerable<string> uuids,
            int interval = 0,
            int take = 1000,
            string consumerId = null,
            string routerId = null)
        {
            var route = "";
            route = Url.Combine(route, "subscribe");
            route = Url.AddQueryParameter(route, "interval", (int)interval);
            route = Url.AddQueryParameter(route, "take", (int)take);
            route = Url.AddQueryParameter(route, "consumerId", (string)consumerId);
            route = Url.AddQueryParameter(route, "routerId", BaseClient.GetRouterId(routerId));
            route = Url.AddQueryParameter(route, "requestBody", "true");

            var url = TrakHoundHttp.GetEntityPath<ITrakHoundObjectEntity>();
            url = Url.Combine(url, route);

            var consumer = new TrakHoundEntityListClientConsumer<ITrakHoundObjectEntity>(BaseClient.ClientConfiguration, url, uuids);
            consumer.Subscribe();
            return consumer;
        }

        #endregion

        #region "Count"

        public async Task<TrakHoundCount> QueryChildCount(string uuid, string routerId = null)
        {
            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectEntity>());
            url = Url.Combine(url, "count/child");
            url = Url.AddQueryParameter(url, "routerId", BaseClient.GetRouterId(routerId));

            var array = await RestRequest.Post<TrakHoundHttpCountResponse>(url, new string[] { uuid });
            return ProcessResponse(array);
        }

        public async Task<IEnumerable<TrakHoundCount>> QueryChildCount(IEnumerable<string> uuids, string routerId = null)
        {
            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectEntity>());
            url = Url.Combine(url, "count/child");
            url = Url.AddQueryParameter(url, "routerId", BaseClient.GetRouterId(routerId));

            var array = await RestRequest.Post<IEnumerable<TrakHoundHttpCountResponse>>(url, uuids);
            return ProcessResponse(array);
        }

        #endregion

        #region "Notify"

        public async Task<ITrakHoundConsumer<IEnumerable<TrakHoundEntityNotification>>> Notify(string path, TrakHoundEntityNotificationType notificationType = TrakHoundEntityNotificationType.All, string routerId = null)
        {
            var url = TrakHoundHttp.GetEntityPath<ITrakHoundObjectEntity>();
            url = Url.Combine(url, "notify");
            url = Url.AddQueryParameter(url, "path", path);
            url = Url.AddQueryParameter(url, "type", notificationType.ToString());
            url = Url.AddQueryParameter(url, "routerId", BaseClient.GetRouterId(routerId));

            var consumer = new TrakHoundNotificationClientConsumer(BaseClient.ClientConfiguration, url);
            consumer.Subscribe();
            return consumer;
        }

        #endregion

        #region "Index"

        public async Task<IEnumerable<string>> QueryIndex(IEnumerable<EntityIndexRequest> requests, long skip, long take, SortOrder sortOrder, string routerId = null)
        {
            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectEntity>());
            url = Url.Combine(url, "index/request");
            url = Url.AddQueryParameter(url, "skip", skip);
            url = Url.AddQueryParameter(url, "take", take);
            url = Url.AddQueryParameter(url, "sortOrder", (int)sortOrder);
            url = Url.AddQueryParameter(url, "routerId", BaseClient.GetRouterId(routerId));

            return await RestRequest.Post<IEnumerable<string>>(url, TrakHoundHttpEntityIndexRequest.Create(requests));
        }

        public async Task<bool> UpdateIndex(IEnumerable<EntityIndexPublishRequest> requests, TrakHoundOperationMode mode = TrakHoundOperationMode.Async, string routerId = null)
        {
            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectEntity>());
            url = Url.Combine(url, "index");
            url = Url.AddQueryParameter(url, "routerId", BaseClient.GetRouterId(routerId));
            if (mode == TrakHoundOperationMode.Sync) url = Url.AddQueryParameter(url, "async", false);

            var response = await RestRequest.PostResponse(url, TrakHoundHttpEntityIndexPublishRequest.Create(requests));
            return response.StatusCode >= 200 && response.StatusCode < 300;
        }

        #endregion

        #region "Delete"

        public async Task<bool> DeleteByRootUuid(string rootUuid, string routerId = null)
        {
            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectEntity>());
            url = Url.Combine(url, "root/children");
            url = Url.Combine(url, rootUuid);
            url = Url.AddQueryParameter(url, "routerId", BaseClient.GetRouterId(routerId));

            return await RestRequest.Delete(url);
        }

        public async Task<bool> DeleteByRootUuid(IEnumerable<string> rootUuids, string routerId = null)
        {
            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectEntity>());
            url = Url.Combine(url, "root/children/delete");
            url = Url.AddQueryParameter(url, "routerId", BaseClient.GetRouterId(routerId));

            var response = await RestRequest.PostResponse(url, rootUuids);
            return response.StatusCode == 200;
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
            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectEntity>());
            url = Url.Combine(url, "expire");
            url = Url.Combine(url, "pattern");
            url = Url.AddQueryParameter(url, "created", created);
            url = Url.AddQueryParameter(url, "routerId", BaseClient.GetRouterId(routerId));

            var results = await RestRequest.Post<IEnumerable<TrakHoundHttpDeleteResult>>(url, patterns);
            if (!results.IsNullOrEmpty())
            {
                return results.ToResults();
            }

            return default;
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
            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectEntity>());
            url = Url.Combine(url, "expire");
            url = Url.Combine(url, "update");
            url = Url.Combine(url, "pattern");
            url = Url.AddQueryParameter(url, "lastUpdated", lastUpdated);
            url = Url.AddQueryParameter(url, "routerId", BaseClient.GetRouterId(routerId));

            var results = await RestRequest.Post<IEnumerable<TrakHoundHttpDeleteResult>>(url, patterns);
            if (!results.IsNullOrEmpty())
            {
                return results.ToResults();
            }

            return default;
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
            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundObjectEntity>());
            url = Url.Combine(url, "expire");
            url = Url.Combine(url, "access");
            url = Url.Combine(url, "pattern");
            url = Url.AddQueryParameter(url, "lastAccessed", lastAccessed);
            url = Url.AddQueryParameter(url, "routerId", BaseClient.GetRouterId(routerId));

            var results = await RestRequest.Post<IEnumerable<TrakHoundHttpDeleteResult>>(url, patterns);
            if (!results.IsNullOrEmpty())
            {
                return results.ToResults();
            }

            return default;
        }

        #endregion

    }
}
