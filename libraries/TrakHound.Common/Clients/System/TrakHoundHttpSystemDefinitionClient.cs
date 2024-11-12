// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Entities;
using TrakHound.Http;

namespace TrakHound.Clients
{
    internal partial class TrakHoundHttpSystemDefinitionClient
    {

        #region "Query"

        public async Task<IEnumerable<ITrakHoundDefinitionEntity>> Query(string pattern, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        {
            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundDefinitionEntity>());
            url = Url.AddQueryParameter(url, "pattern", pattern);
            url = Url.AddQueryParameter(url, "skip", skip);
            url = Url.AddQueryParameter(url, "take", take);
            url = Url.AddQueryParameter(url, "order", (int)sortOrder);
            url = Url.AddQueryParameter(url, "routerId", BaseClient.GetRouterId(routerId));

            var array = await RestRequest.Get<object[][]>(url);
            return ProcessResponse(array);
        }

        #endregion

        #region "Query By Parent"

        public async Task<IEnumerable<ITrakHoundDefinitionEntity>> QueryByParentUuid(string parentUuid, long skip = 0, long take = 1000, SortOrder order = SortOrder.Ascending, string routerId = null)
        {
            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundDefinitionEntity>());
            url = Url.Combine(url, "parent");
            url = Url.AddQueryParameter(url, "skip", skip);
            url = Url.AddQueryParameter(url, "take", take);
            url = Url.AddQueryParameter(url, "order", (int)order);
            url = Url.AddQueryParameter(url, "routerId", BaseClient.GetRouterId(routerId));

            var array = await RestRequest.Post<object[][]>(url, new string[] { parentUuid });
            return ProcessResponse(array);
        }

        public async Task<IEnumerable<ITrakHoundDefinitionEntity>> QueryByParentUuid(IEnumerable<string> parentUuids, long skip = 0, long take = 1000, SortOrder order = SortOrder.Ascending, string routerId = null)
        {
            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundDefinitionEntity>());
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
            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundDefinitionEntity>());
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
            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundDefinitionEntity>());
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

        public async Task<IEnumerable<ITrakHoundDefinitionEntity>> QueryByChildUuid(string childUuid, long skip = 0, long take = 1000, SortOrder order = SortOrder.Ascending, string routerId = null)
        {
            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundDefinitionEntity>());
            url = Url.Combine(url, "child");
            url = Url.AddQueryParameter(url, "skip", skip);
            url = Url.AddQueryParameter(url, "take", take);
            url = Url.AddQueryParameter(url, "order", (int)order);
            url = Url.AddQueryParameter(url, "routerId", BaseClient.GetRouterId(routerId));

            var array = await RestRequest.Post<object[][]>(url, new string[] { childUuid });
            return ProcessResponse(array);
        }

        public async Task<IEnumerable<ITrakHoundDefinitionEntity>> QueryByChildUuid(IEnumerable<string> childUuids, long skip = 0, long take = 1000, SortOrder order = SortOrder.Ascending, string routerId = null)
        {
            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundDefinitionEntity>());
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
            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundDefinitionEntity>());
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
            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundDefinitionEntity>());
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

        public async Task<IEnumerable<ITrakHoundDefinitionEntity>> QueryChildrenByRootUuid(string rootUuid, long skip = 0, long take = 1000, SortOrder order = SortOrder.Ascending, string routerId = null)
        {
            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundDefinitionEntity>());
            url = Url.Combine(url, "root/children");
            url = Url.AddQueryParameter(url, "skip", skip);
            url = Url.AddQueryParameter(url, "take", take);
            url = Url.AddQueryParameter(url, "order", (int)order);
            url = Url.AddQueryParameter(url, "routerId", BaseClient.GetRouterId(routerId));

            var array = await RestRequest.Post<object[][]>(url, new string[] { rootUuid });
            return ProcessResponse(array);
        }

        public async Task<IEnumerable<ITrakHoundDefinitionEntity>> QueryChildrenByRootUuid(IEnumerable<string> rootUuids, long skip = 0, long take = 1000, SortOrder order = SortOrder.Ascending, string routerId = null)
        {
            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundDefinitionEntity>());
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
            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundDefinitionEntity>());
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
            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundDefinitionEntity>());
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

        public async Task<IEnumerable<ITrakHoundDefinitionEntity>> QueryRootByChildUuid(string childUuid, long skip = 0, long take = 1000, SortOrder order = SortOrder.Ascending, string routerId = null)
        {
            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundDefinitionEntity>());
            url = Url.Combine(url, "child/root");
            url = Url.AddQueryParameter(url, "skip", skip);
            url = Url.AddQueryParameter(url, "take", take);
            url = Url.AddQueryParameter(url, "order", (int)order);
            url = Url.AddQueryParameter(url, "routerId", BaseClient.GetRouterId(routerId));

            var array = await RestRequest.Post<object[][]>(url, new string[] { childUuid });
            return ProcessResponse(array);
        }

        public async Task<IEnumerable<ITrakHoundDefinitionEntity>> QueryRootByChildUuid(IEnumerable<string> childUuids, long skip = 0, long take = 1000, SortOrder order = SortOrder.Ascending, string routerId = null)
        {
            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundDefinitionEntity>());
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
            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundDefinitionEntity>());
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
            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundDefinitionEntity>());
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

        #region "Query by Type"

        public async Task<IEnumerable<ITrakHoundDefinitionEntity>> QueryByType(string type, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        {
            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundDefinitionEntity>());
            url = Url.Combine(url, "type");
            url = Url.AddQueryParameter(url, "skip", skip);
            url = Url.AddQueryParameter(url, "take", take);
            url = Url.AddQueryParameter(url, "order", (int)sortOrder);
            url = Url.AddQueryParameter(url, "routerId", BaseClient.GetRouterId(routerId));

            var array = await RestRequest.Post<object[][]>(url, new string[] { type });
            return ProcessResponse(array);
        }

        public async Task<IEnumerable<ITrakHoundDefinitionEntity>> QueryByType(IEnumerable<string> types, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        {
            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundDefinitionEntity>());
            url = Url.Combine(url, "type");
            url = Url.AddQueryParameter(url, "skip", skip);
            url = Url.AddQueryParameter(url, "take", take);
            url = Url.AddQueryParameter(url, "order", (int)sortOrder);
            url = Url.AddQueryParameter(url, "routerId", BaseClient.GetRouterId(routerId));

            var array = await RestRequest.Post<object[][]>(url, types);
            return ProcessResponse(array);
        }


        public async Task<IEnumerable<ITrakHoundDefinitionQueryResult>> QueryIdsByType(string type, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        {
            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundDefinitionEntity>());
            url = Url.Combine(url, "ids");
            url = Url.Combine(url, "type");
            url = Url.AddQueryParameter(url, "skip", skip);
            url = Url.AddQueryParameter(url, "take", take);
            url = Url.AddQueryParameter(url, "order", (int)sortOrder);
            url = Url.AddQueryParameter(url, "routerId", BaseClient.GetRouterId(routerId));

            var httpResults = await RestRequest.Post<IEnumerable<TrakHoundHttpDefinitionQueryResult>>(url, new string[] { type });
            if (!httpResults.IsNullOrEmpty())
            {
                var results = new List<ITrakHoundDefinitionQueryResult>();

                foreach (var httpResult in httpResults)
                {
                    results.Add(httpResult.ToQueryResult());
                }

                return results;
            }

            return null;
        }

        public async Task<IEnumerable<ITrakHoundDefinitionQueryResult>> QueryIdsByType(IEnumerable<string> types, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        {
            var url = Url.Combine(BaseUrl, TrakHoundHttp.GetEntityPath<ITrakHoundDefinitionEntity>());
            url = Url.Combine(url, "ids");
            url = Url.Combine(url, "type");
            url = Url.AddQueryParameter(url, "skip", skip);
            url = Url.AddQueryParameter(url, "take", take);
            url = Url.AddQueryParameter(url, "order", (int)sortOrder);
            url = Url.AddQueryParameter(url, "routerId", BaseClient.GetRouterId(routerId));

            var httpResults = await RestRequest.Post<IEnumerable<TrakHoundHttpDefinitionQueryResult>>(url, types);
            if (!httpResults.IsNullOrEmpty())
            {
                var results = new List<ITrakHoundDefinitionQueryResult>();

                foreach (var httpResult in httpResults)
                {
                    results.Add(httpResult.ToQueryResult());
                }

                return results;
            }

            return null;
        }

        #endregion

    }
}
