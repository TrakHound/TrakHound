// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Requests;

namespace TrakHound.Clients
{
    public abstract partial class TrakHoundEntitiesClientBase
    {
        private const string _definitionsRoute = "definitions";


        public async Task<TrakHoundDefinition> GetDefinition(string definitionId, string routerId = null)
        {
            if (!string.IsNullOrEmpty(definitionId))
            {
                var url = Url.Combine(ApiRoute, _definitionsRoute, definitionId);

                var parameters = new Dictionary<string, string>();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.QueryJson<TrakHoundDefinition>(url, queryParameters: parameters);
            }

            return null;
        }

        public async Task<TrakHoundDefinition> GetDefinitionByUuid(string definitionUuid, string routerId = null)
        {
            if (!string.IsNullOrEmpty(definitionUuid))
            {
                var url = Url.Combine(ApiRoute, _definitionsRoute, "uuid", definitionUuid);

                var parameters = new Dictionary<string, string>();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.QueryJson<TrakHoundDefinition>(url, queryParameters: parameters);
            }

            return null;
        }

        public async Task<IEnumerable<TrakHoundDefinition>> GetDefinitions(IEnumerable<string> definitionIds, string routerId = null)
        {
            if (!definitionIds.IsNullOrEmpty())
            {
                var url = Url.Combine(ApiRoute, _definitionsRoute);

                var parameters = new Dictionary<string, string>();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.QueryJson<IEnumerable<TrakHoundDefinition>>(url, definitionIds, queryParameters: parameters);
            }

            return null;
        }

        public async Task<IEnumerable<TrakHoundDefinition>> GetParentDefinitions(string definitionId, string routerId = null)
        {
            if (!string.IsNullOrEmpty(definitionId))
            {
                var url = Url.Combine(ApiRoute, _definitionsRoute, definitionId, "parents");
                
                var parameters = new Dictionary<string, string>();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.QueryJson<IEnumerable<TrakHoundDefinition>>(url, queryParameters: parameters);
            }

            return null;
        }

        public async Task<IEnumerable<TrakHoundDefinition>> GetParentDefinitionsByUuid(string definitionUuid, string routerId = null)
        {
            if (!string.IsNullOrEmpty(definitionUuid))
            {
                var url = Url.Combine(ApiRoute, _definitionsRoute, "uuid", definitionUuid, "parents");

                var parameters = new Dictionary<string, string>();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.QueryJson<IEnumerable<TrakHoundDefinition>>(url, queryParameters: parameters);
            }

            return null;
        }


        public async Task<IEnumerable<TrakHoundDefinition>> QueryDefinitions(string pattern, string routerId = null)
        {
            if (!string.IsNullOrEmpty(pattern))
            {
                var url = Url.Combine(ApiRoute, _definitionsRoute, "query");

                var parameters = new Dictionary<string, string>();
                parameters["pattern"] = pattern;
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.QueryJson<IEnumerable<TrakHoundDefinition>>(url, parameters);
            }

            return null;
        }


        public async Task<bool> PublishDefinition(string id, string description = null, string parentId = null, bool async = false, string routerId = null)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var url = Url.Combine(ApiRoute, _definitionsRoute);

                var parameters = new Dictionary<string, string>();
                parameters["id"] = id;
                parameters["description"] = description;
                parameters["parentId"] = parentId;
                parameters["async"] = async.ToString();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                var response = await ApiClient.Publish(url, parameters);
                await PublishAfter();
                return response.Success;
            }

            return false;
        }

        public async Task<bool> PublishDefinitions(IEnumerable<TrakHoundDefinitionEntry> entries, bool async = false, string routerId = null)
        {
            if (!entries.IsNullOrEmpty())
            {
                var url = Url.Combine(ApiRoute, _definitionsRoute, "batch");

                var parameters = new Dictionary<string, string>();
                parameters["async"] = async.ToString();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                var response = await ApiClient.Publish(url, entries, queryParameters: parameters);
                await PublishAfter();
                return response.Success;
            }

            return false;
        }
    }
}
