// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Requests;

namespace TrakHound.Clients
{
    public abstract partial class TrakHoundEntitiesClientBase
    {
        private const string _setsRoute = "set";


        public async Task<IEnumerable<TrakHoundSet>> GetSets(string objectPath, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        {
            if (!string.IsNullOrEmpty(objectPath))
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _setsRoute);

                var parameters = new Dictionary<string, string>();
                parameters["objectPath"] = objectPath;
                parameters["skip"] = skip.ToString();
                parameters["take"] = take.ToString();
                parameters["sortOrder"] = ((int)sortOrder).ToString();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.QueryJson<IEnumerable<TrakHoundSet>>(url, parameters);
            }

            return null;
        }

        public async Task<IEnumerable<TrakHoundSet>> GetSets(IEnumerable<string> objectPaths, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        {
            if (!objectPaths.IsNullOrEmpty())
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _setsRoute);

                var parameters = new Dictionary<string, string>();
                parameters["skip"] = skip.ToString();
                parameters["take"] = take.ToString();
                parameters["sortOrder"] = ((int)sortOrder).ToString();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.QueryJson<IEnumerable<TrakHoundSet>>(url, objectPaths, queryParameters: parameters);
            }

            return null;
        }


        public async Task<Dictionary<string, IEnumerable<string>>> GetSetValues(string objectPath, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        {
            if (!string.IsNullOrEmpty(objectPath))
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, $"{_setsRoute}/values");

                var parameters = new Dictionary<string, string>();
                parameters["objectPath"] = objectPath;
                parameters["skip"] = skip.ToString();
                parameters["take"] = take.ToString();
                parameters["sortOrder"] = ((int)sortOrder).ToString();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.QueryJson<Dictionary<string, IEnumerable<string>>>(url, parameters);
            }

            return null;
        }

        public async Task<Dictionary<string, IEnumerable<string>>> GetSetValues(IEnumerable<string> objectPaths, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null)
        {
            if (!objectPaths.IsNullOrEmpty())
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, $"{_setsRoute}/values");

                var parameters = new Dictionary<string, string>();
                parameters["skip"] = skip.ToString();
                parameters["take"] = take.ToString();
                parameters["sortOrder"] = ((int)sortOrder).ToString();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.QueryJson<Dictionary<string, IEnumerable<string>>>(url, objectPaths, queryParameters: parameters);
            }

            return null;
        }


        public async Task<ITrakHoundConsumer<IEnumerable<TrakHoundSet>>> SubscribeSets(string objectPath, string routerId = null)
        {
            if (!string.IsNullOrEmpty(objectPath))
            {
                var route = Url.Combine(ApiRoute, _objectsRoute, _setsRoute);

                var parameters = new Dictionary<string, string>();
                parameters["objectPath"] = objectPath;
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.SubscribeJson<IEnumerable<TrakHoundSet>>(route, parameters);
            }

            return null;
        }

        public async Task<ITrakHoundConsumer<IEnumerable<TrakHoundSet>>> SubscribeSets(IEnumerable<string> objectPaths, string routerId = null)
        {
            if (!objectPaths.IsNullOrEmpty())
            {
                var route = Url.Combine(ApiRoute, _objectsRoute, _setsRoute);

                var parameters = new Dictionary<string, string>();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.SubscribeJson<IEnumerable<TrakHoundSet>>(route, objectPaths, queryParameters: parameters);
            }

            return null;
        }


        public async Task<bool> PublishSet(string objectPath, string value, bool async = false, string routerId = null)
        {
            if (!string.IsNullOrEmpty(objectPath) && !string.IsNullOrEmpty(value))
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _setsRoute);

                var parameters = new Dictionary<string, string>();
                parameters["objectPath"] = objectPath;
                parameters["async"] = async.ToString();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                var values = new string[] { value };

                var response = await ApiClient.Publish(url, values, queryParameters: parameters);
                await PublishAfter();
                return response.Success;
            }

            return false;
        }

        public async Task<bool> PublishSet(string objectPath, IEnumerable<string> values, bool async = false, string routerId = null)
        {
            if (!string.IsNullOrEmpty(objectPath) && !values.IsNullOrEmpty())
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _setsRoute);

                var parameters = new Dictionary<string, string>();
                parameters["objectPath"] = objectPath;
                parameters["async"] = async.ToString();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                var response = await ApiClient.Publish(url, values, queryParameters: parameters);
                await PublishAfter();
                return response.Success;
            }

            return false;
        }

        public async Task<bool> PublishSets(IEnumerable<TrakHoundSetEntry> entries, bool async = false, string routerId = null)
        {
            if (!entries.IsNullOrEmpty())
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, $"{_setsRoute}/batch");

                var parameters = new Dictionary<string, string>();
                parameters["async"] = async.ToString();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                var response = await ApiClient.Publish(url, entries, queryParameters: parameters);
                await PublishAfter();
                return response.Success;
            }

            return false;
        }


        public async Task<bool> DeleteSet(string objectPath, bool async = false, string routerId = null)
        {
            if (!string.IsNullOrEmpty(objectPath))
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _setsRoute);

                var parameters = new Dictionary<string, string>();
                parameters["objectPath"] = objectPath;
                parameters["async"] = async.ToString();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                var response = await ApiClient.Delete(url, parameters);
                await PublishAfter();
                return response.Success;
            }

            return false;
        }

        public async Task<bool> DeleteSet(string objectPath, string value, bool async = false, string routerId = null)
        {
            if (!string.IsNullOrEmpty(objectPath) && !string.IsNullOrEmpty(value))
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _setsRoute);

                var parameters = new Dictionary<string, string>();
                parameters["objectPath"] = objectPath;
                parameters["value"] = value;
                parameters["async"] = async.ToString();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                var response = await ApiClient.Delete(url, parameters);
                await PublishAfter();
                return response.Success;
            }

            return false;
        }
    }
}
