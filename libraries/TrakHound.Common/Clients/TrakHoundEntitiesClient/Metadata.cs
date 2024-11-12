// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Requests;

namespace TrakHound.Clients
{
    public abstract partial class TrakHoundEntitiesClientBase
    {
        private const string _metadataRoute = "metadata";


        public async Task<IEnumerable<TrakHoundMetadata>> GetMetadata(string objectPath, string routerId = null)
        {
            if (!string.IsNullOrEmpty(objectPath))
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _metadataRoute);

                var parameters = new Dictionary<string, string>();
                parameters["entityUuid"] = objectPath;
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.QueryJson<IEnumerable<TrakHoundMetadata>>(url, parameters);
            }

            return null;
        }

        public async Task<IEnumerable<TrakHoundMetadata>> GetMetadata(IEnumerable<string> objectPaths, string routerId = null)
        {
            if (!objectPaths.IsNullOrEmpty())
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _metadataRoute);

                var parameters = new Dictionary<string, string>();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.QueryJson<IEnumerable<TrakHoundMetadata>>(url, objectPaths, queryParameters: parameters);
            }

            return null;
        }


        public async Task<bool> PublishMetadata(
            string entityUuid,
            string name,
            string value,
            string definitionId = null,
            string valueDefinitionId = null,
            bool async = false,
            string routerId = null
            )
        {
            if (!string.IsNullOrEmpty(entityUuid) && !string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(value))
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _metadataRoute);

                var parameters = new Dictionary<string, string>();
                parameters["entityUuid"] = entityUuid;
                parameters["name"] = name;
                parameters["value"] = value;
                parameters["definitionId"] = definitionId;
                parameters["valueDefinitionId"] = valueDefinitionId;
                parameters["async"] = async.ToString();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                var response = await ApiClient.Publish(url, parameters);
                await PublishAfter();
                return response.Success;
            }

            return false;
        }

        public async Task<bool> PublishMetadata(IEnumerable<TrakHoundMetadataEntry> entries, bool async = false, string routerId = null)
        {
            if (!entries.IsNullOrEmpty())
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, $"{_metadataRoute}/batch");

                var parameters = new Dictionary<string, string>();
                parameters["async"] = async.ToString();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                var response = await ApiClient.Publish(url, entries, queryParameters: parameters);
                await PublishAfter();
                return response.Success;
            }

            return false;
        }


        public async Task<bool> DeleteMetadata(string entityUuid, string routerId = null)
        {
            if (!string.IsNullOrEmpty(entityUuid))
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _metadataRoute);

                var parameters = new Dictionary<string, string>();
                parameters["entityUuid"] = entityUuid;
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                var response = await ApiClient.Delete(url, parameters);
                return response.Success;
            }

            return false;
        }

        public async Task<bool> DeleteMetadata(string entityUuid, string name, string routerId = null)
        {
            if (!string.IsNullOrEmpty(entityUuid))
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _metadataRoute);

                var parameters = new Dictionary<string, string>();
                parameters["entityUuid"] = entityUuid;
                parameters["name"] = name;
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                var response = await ApiClient.Delete(url, parameters);
                return response.Success;
            }

            return false;
        }
    }
}
