// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Requests;

namespace TrakHound.Clients
{
    public abstract partial class TrakHoundEntitiesClientBase
    {
        private const string _hashesRoute = "hash";


        public async Task<IEnumerable<TrakHoundHash>> GetHashes(string objectPath, string routerId = null)
        {
            if (!string.IsNullOrEmpty(objectPath))
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _hashesRoute);

                var parameters = new Dictionary<string, string>();
                parameters["objectPath"] = objectPath;
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.QueryJson<IEnumerable<TrakHoundHash>>(url, parameters);
            }

            return null;
        }

		public async Task<IEnumerable<TrakHoundHash>> GetHashes(IEnumerable<string> objectPaths, string routerId = null)
		{
			if (!objectPaths.IsNullOrEmpty())
			{
				var url = Url.Combine(ApiRoute, _objectsRoute, _hashesRoute);

                var parameters = new Dictionary<string, string>();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.QueryJson<IEnumerable<TrakHoundHash>>(url, objectPaths, queryParameters: parameters);
			}

			return null;
		}

		public async Task<Dictionary<string, Dictionary<string, string>>> GetHashValues(string objectPath, string routerId = null)
        {
            if (!string.IsNullOrEmpty(objectPath))
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, $"{_hashesRoute}/values");

                var parameters = new Dictionary<string, string>();
                parameters["objectPath"] = objectPath;
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.QueryJson<Dictionary<string, Dictionary<string, string>>>(url, parameters);
            }

            return null;
        }

		public async Task<Dictionary<string, Dictionary<string, string>>> GetHashValues(IEnumerable<string> objectPaths, string routerId = null)
		{
			if (!objectPaths.IsNullOrEmpty())
			{
				var url = Url.Combine(ApiRoute, _objectsRoute, $"{_hashesRoute}/values");

                var parameters = new Dictionary<string, string>();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.QueryJson<Dictionary<string, Dictionary<string, string>>>(url, objectPaths, queryParameters: parameters);
			}

			return null;
		}


		public async Task<ITrakHoundConsumer<IEnumerable<TrakHoundHash>>> SubscribeHashes(string objectPath, string routerId = null)
        {
            if (!string.IsNullOrEmpty(objectPath))
            {
                var route = Url.Combine(ApiRoute, _objectsRoute, _hashesRoute);

                var parameters = new Dictionary<string, string>();
                parameters["objectPath"] = objectPath;
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.SubscribeJson<IEnumerable<TrakHoundHash>>(route, parameters);
            }

            return null;
        }


        public async Task<bool> PublishHash(string objectPath, string key, string value, bool async = false, string routerId = null)
        {
            if (!string.IsNullOrEmpty(objectPath) && !string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _hashesRoute);

                var parameters = new Dictionary<string, string>();
                parameters["objectPath"] = objectPath;
                parameters["async"] = async.ToString();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                var values = new Dictionary<string, string>();
                values.Add(key, value);

                var response = await ApiClient.Publish(url, values, queryParameters: parameters);
                await PublishAfter();
                return response.Success;
            }

            return false;
        }

        public async Task<bool> PublishHash(string objectPath, Dictionary<string, string> values, bool async = false, string routerId = null)
        {
            if (!string.IsNullOrEmpty(objectPath) && !values.IsNullOrEmpty())
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _hashesRoute);

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

        public async Task<bool> PublishHashes(IEnumerable<TrakHoundHashEntry> entries, bool async = false, string routerId = null)
        {
            if (!entries.IsNullOrEmpty())
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, $"{_hashesRoute}/batch");

                var parameters = new Dictionary<string, string>();
                parameters["async"] = async.ToString();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                var response = await ApiClient.Publish(url, entries, queryParameters: parameters);
                await PublishAfter();
                return response.Success;
            }

            return false;
        }


        public async Task<bool> DeleteHash(string objectPath, bool async = false, string routerId = null)
        {
            if (!string.IsNullOrEmpty(objectPath))
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _hashesRoute);

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

        public async Task<bool> DeleteHash(string objectPath, string key, bool async = false, string routerId = null)
        {
            if (!string.IsNullOrEmpty(objectPath) && !string.IsNullOrEmpty(key))
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _hashesRoute);

                var parameters = new Dictionary<string, string>();
                parameters["objectPath"] = objectPath;
                parameters["key"] = key;
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
