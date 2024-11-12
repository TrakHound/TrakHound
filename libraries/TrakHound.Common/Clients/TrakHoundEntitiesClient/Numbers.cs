// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrakHound.Entities;
using TrakHound.Requests;

namespace TrakHound.Clients
{
    public abstract partial class TrakHoundEntitiesClientBase
    {
        private const string _numbersRoute = "number";


        public async Task<TrakHoundNumber> GetNumber(string objectPath, string routerId = null)
        {
            if (!string.IsNullOrEmpty(objectPath))
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _numbersRoute);

                var parameters = new Dictionary<string, string>();
                parameters["objectPath"] = objectPath;
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return (await ApiClient.QueryJson<IEnumerable<TrakHoundNumber>>(url, parameters))?.FirstOrDefault();
            }

            return null;
        }

        public async Task<string> GetNumberValue(string objectPath, string routerId = null)
        {
            if (!string.IsNullOrEmpty(objectPath))
            {
                var values = await GetNumberValues(objectPath, routerId);
                if (!values.IsNullOrEmpty())
                {
                    return values.GetValueOrDefault(objectPath);
                }
            }

            return null;
        }

        public async Task<T> GetNumberValue<T>(string objectPath, string routerId = null)
        {
            var value = await GetNumberValue(objectPath, routerId);
            if (value != null)
            {
                try
                {
                    return (T)Convert.ChangeType(value, typeof(T));
                }
                catch { }
            }

            return default;
        }

        public async Task<IEnumerable<TrakHoundNumber>> GetNumbers(string objectPath, string routerId = null)
        {
            if (!string.IsNullOrEmpty(objectPath))
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _numbersRoute);

                var parameters = new Dictionary<string, string>();
                parameters["objectPath"] = objectPath;
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.QueryJson<IEnumerable<TrakHoundNumber>>(url, parameters);
            }

            return null;
        }

        public async Task<IEnumerable<TrakHoundNumber>> GetNumbers(IEnumerable<string> objectPaths, string routerId = null)
        {
            if (!objectPaths.IsNullOrEmpty())
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _numbersRoute);

                var parameters = new Dictionary<string, string>();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.QueryJson<IEnumerable<TrakHoundNumber>>(url, objectPaths, queryParameters: parameters);
            }

            return null;
        }

        public async Task<Dictionary<string, string>> GetNumberValues(string objectPath, string routerId = null)
        {
            if (!string.IsNullOrEmpty(objectPath))
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, $"{_numbersRoute}/values");

                var parameters = new Dictionary<string, string>();
                parameters["objectPath"] = objectPath;
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.QueryJson<Dictionary<string, string>>(url, parameters);
            }

            return null;
        }

        public async Task<Dictionary<string, string>> GetNumberValues(IEnumerable<string> objectPaths, string routerId = null)
        {
            if (!objectPaths.IsNullOrEmpty())
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, $"{_numbersRoute}/values");

                var parameters = new Dictionary<string, string>();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.QueryJson<Dictionary<string, string>>(url, objectPaths, queryParameters: parameters);
            }

            return null;
        }


        public async Task<ITrakHoundConsumer<IEnumerable<TrakHoundNumber>>> SubscribeNumbers(string objectPath, string routerId = null)
        {
            if (!string.IsNullOrEmpty(objectPath))
            {
                var route = Url.Combine(ApiRoute, _objectsRoute, _numbersRoute);

                var parameters = new Dictionary<string, string>();
                parameters["objectPath"] = objectPath;
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.SubscribeJson<IEnumerable<TrakHoundNumber>>(route, parameters);
            }

            return null;
        }


        public async Task<bool> PublishNumber(string objectPath, string value, TrakHoundNumberDataType dataType = TrakHoundNumberDataType.Float, bool async = false, string routerId = null)
        {
            if (!string.IsNullOrEmpty(objectPath) && !string.IsNullOrEmpty(value))
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _numbersRoute);

                var parameters = new Dictionary<string, string>();
                parameters["objectPath"] = objectPath;
                parameters["dataType"] = ((int)dataType).ToString();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                var requestBody = System.Text.Encoding.UTF8.GetBytes(value);

                var response = await ApiClient.Publish(url, requestBody, "text/plain", parameters);
                await PublishAfter();
                return response.Success;
            }

            return false;
        }
        public async Task<bool> PublishNumber(string objectPath, byte value, bool async = false, string routerId = null) => await PublishNumber(objectPath, value.ToString(), TrakHoundNumberDataType.Byte, async, routerId);
        public async Task<bool> PublishNumber(string objectPath, short value, bool async = false, string routerId = null) => await PublishNumber(objectPath, value.ToString(), TrakHoundNumberDataType.Int16, async, routerId);
        public async Task<bool> PublishNumber(string objectPath, int value, bool async = false, string routerId = null) => await PublishNumber(objectPath, value.ToString(), TrakHoundNumberDataType.Int32, async, routerId);
        public async Task<bool> PublishNumber(string objectPath, long value, bool async = false, string routerId = null) => await PublishNumber(objectPath, value.ToString(), TrakHoundNumberDataType.Int64, async, routerId);
        public async Task<bool> PublishNumber(string objectPath, double value, bool async = false, string routerId = null) => await PublishNumber(objectPath, value.ToString(), TrakHoundNumberDataType.Float, async, routerId);
        public async Task<bool> PublishNumber(string objectPath, decimal value, bool async = false, string routerId = null) => await PublishNumber(objectPath, value.ToString(), TrakHoundNumberDataType.Decimal, async, routerId);

        public async Task<bool> PublishNumbers(IEnumerable<TrakHoundNumberEntry> entries, bool async = false, string routerId = null)
        {
            if (!entries.IsNullOrEmpty())
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, $"{_numbersRoute}/batch");

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
