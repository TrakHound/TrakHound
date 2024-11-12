// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrakHound.Requests;

namespace TrakHound.Clients
{
    public abstract partial class TrakHoundEntitiesClientBase
    {
        private const string _vocabulariesRoute = "vocabulary";


        public async Task<TrakHoundVocabulary> GetVocabulary(string objectPath, string routerId = null)
        {
            if (!string.IsNullOrEmpty(objectPath))
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _vocabulariesRoute);

                var parameters = new Dictionary<string, string>();
                parameters["objectPath"] = objectPath;
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return (await ApiClient.QueryJson<IEnumerable<TrakHoundVocabulary>>(url, parameters))?.FirstOrDefault();
            }

            return null;
        }

        public async Task<IEnumerable<TrakHoundVocabulary>> GetVocabularies(string objectPath, string routerId = null)
        {
            if (!string.IsNullOrEmpty(objectPath))
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _vocabulariesRoute);

                var parameters = new Dictionary<string, string>();
                parameters["objectPath"] = objectPath;
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.QueryJson<IEnumerable<TrakHoundVocabulary>>(url, parameters);
            }

            return null;
        }

        public async Task<IEnumerable<TrakHoundVocabulary>> GetVocabularies(IEnumerable<string> objectPaths, string routerId = null)
        {
            if (!objectPaths.IsNullOrEmpty())
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _vocabulariesRoute);

                var parameters = new Dictionary<string, string>();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.QueryJson<IEnumerable<TrakHoundVocabulary>>(url, objectPaths, queryParameters: parameters);
            }

            return null;
        }


        public async Task<bool> PublishVocabulary(string objectPath, string definitionId, bool async = false, string routerId = null)
        {
            if (!string.IsNullOrEmpty(objectPath) && !string.IsNullOrEmpty(definitionId))
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _vocabulariesRoute);

                var parameters = new Dictionary<string, string>();
                parameters["objectPath"] = objectPath;
                parameters["definitionId"] = definitionId;
                parameters["async"] = async.ToString();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                var response = await ApiClient.Publish(url, parameters);
                await PublishAfter();
                return response.Success;
            }

            return false;
        }

        public async Task<bool> PublishVocabularies(IEnumerable<TrakHoundVocabularyEntry> entries, bool async = false, string routerId = null)
        {
            if (!entries.IsNullOrEmpty())
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, $"{_vocabulariesRoute}/batch");

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
