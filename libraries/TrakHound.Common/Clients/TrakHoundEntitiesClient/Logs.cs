// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Requests;

namespace TrakHound.Clients
{
    public abstract partial class TrakHoundEntitiesClientBase
    {
        private const string _logsRoute = "log";


        public async Task<IEnumerable<TrakHoundLog>> GetLogs(string objectPath, TrakHoundLogLevel minimumLevel, int skip = _defaultSkip, int take = _defaultTake, SortOrder sortOrder = _defaultSortOrder, string routerId = null)
        {
            if (!string.IsNullOrEmpty(objectPath))
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _logsRoute);

                var parameters = new Dictionary<string, string>();
                parameters["objectPath"] = objectPath;
                parameters["level"] = minimumLevel.ToString();
                parameters["skip"] = skip.ToString();
                parameters["take"] = take.ToString();
                parameters["sortOrder"] = ((int)sortOrder).ToString();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.QueryJson<IEnumerable<TrakHoundLog>>(url, parameters);
            }

            return null;
        }

        public async Task<IEnumerable<TrakHoundLog>> GetLogs(string objectPath, TrakHoundLogLevel minimumLevel, DateTime start, DateTime stop, int skip = _defaultSkip, int take = _defaultTake, SortOrder sortOrder = _defaultSortOrder, string routerId = null)
        {
            if (!string.IsNullOrEmpty(objectPath))
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _logsRoute);

                var parameters = new Dictionary<string, string>();
                parameters["objectPath"] = objectPath;
                parameters["level"] = minimumLevel.ToString();
                parameters["start"] = start.ToUnixTime().ToString();
                parameters["stop"] = stop.ToUnixTime().ToString();
                parameters["skip"] = skip.ToString();
                parameters["take"] = take.ToString();
                parameters["sortOrder"] = ((int)sortOrder).ToString();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.QueryJson<IEnumerable<TrakHoundLog>>(url, parameters);
            }

            return null;
        }

        public async Task<IEnumerable<TrakHoundLog>> GetLogs(string objectPath, TrakHoundLogLevel minimumLevel, string startExpression, string stopExpression, int skip = _defaultSkip, int take = _defaultTake, SortOrder sortOrder = _defaultSortOrder, string routerId = null)
        {
            if (!string.IsNullOrEmpty(objectPath) && !string.IsNullOrEmpty(startExpression) && !string.IsNullOrEmpty(stopExpression))
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _logsRoute);

                var parameters = new Dictionary<string, string>();
                parameters["objectPath"] = objectPath;
                parameters["level"] = minimumLevel.ToString();
                parameters["start"] = startExpression;
                parameters["stop"] = stopExpression;
                parameters["skip"] = skip.ToString();
                parameters["take"] = take.ToString();
                parameters["sortOrder"] = ((int)sortOrder).ToString();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.QueryJson<IEnumerable<TrakHoundLog>>(url, parameters);
            }

            return null;
        }


        public async Task<Dictionary<string, IEnumerable<TrakHoundLogValue>>> GetLogValues(string objectPath, TrakHoundLogLevel minimumLevel, DateTime start, DateTime stop, int skip = _defaultSkip, int take = _defaultTake, SortOrder sortOrder = _defaultSortOrder, string routerId = null)
        {
            if (!string.IsNullOrEmpty(objectPath) && start > DateTime.MinValue && stop > DateTime.MinValue)
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _logsRoute);
                url = Url.Combine(url, "values");

                var parameters = new Dictionary<string, string>();
                parameters["objectPath"] = objectPath;
                parameters["level"] = minimumLevel.ToString();
                parameters["start"] = start.ToUnixTime().ToString();
                parameters["stop"] = stop.ToUnixTime().ToString();
                parameters["skip"] = skip.ToString();
                parameters["take"] = take.ToString();
                parameters["sortOrder"] = ((int)sortOrder).ToString();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.QueryJson<Dictionary<string, IEnumerable<TrakHoundLogValue>>>(url, parameters);
            }

            return null;
        }

        public async Task<Dictionary<string, IEnumerable<TrakHoundLogValue>>> GetLogValues(string objectPath, TrakHoundLogLevel minimumLevel, string startExpression, string stopExpression, int skip = _defaultSkip, int take = _defaultTake, SortOrder sortOrder = _defaultSortOrder, string routerId = null)
        {
            if (!string.IsNullOrEmpty(objectPath) && !string.IsNullOrEmpty(startExpression) && !string.IsNullOrEmpty(stopExpression))
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _logsRoute);
                url = Url.Combine(url, "values");

                var parameters = new Dictionary<string, string>();
                parameters["objectPath"] = objectPath;
                parameters["level"] = minimumLevel.ToString();
                parameters["start"] = startExpression;
                parameters["stop"] = stopExpression;
                parameters["skip"] = skip.ToString();
                parameters["take"] = take.ToString();
                parameters["sortOrder"] = ((int)sortOrder).ToString();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.QueryJson<Dictionary<string, IEnumerable<TrakHoundLogValue>>>(url, parameters);
            }

            return null;
        }


        public async Task<ITrakHoundConsumer<IEnumerable<TrakHoundLog>>> SubscribeLogs(string objectPath, TrakHoundLogLevel minimumLevel, string routerId = null)
        {
            if (!string.IsNullOrEmpty(objectPath))
            {
                var route = Url.Combine(ApiRoute, _objectsRoute, _logsRoute);

                var parameters = new Dictionary<string, string>();
                parameters["objectPath"] = objectPath;
                parameters["level"] = minimumLevel.ToString();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.SubscribeJson<IEnumerable<TrakHoundLog>>(route, parameters);
            }

            return null;
        }


        public async Task<bool> PublishLog(string objectPath, TrakHoundLogLevel logLevel, string message, string code = null, DateTime? timestamp = null, bool async = false, string routerId = null)
        {
            if (!string.IsNullOrEmpty(objectPath) && !string.IsNullOrEmpty(message))
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _logsRoute);

                var parameters = new Dictionary<string, string>();
                parameters["objectPath"] = objectPath;
                parameters["level"] = logLevel.ToString();
                parameters["message"] = message;
                parameters["code"] = code;
                if (timestamp != null) parameters["timestamp"] = timestamp.Value.ToUnixTime().ToString();
                parameters["async"] = async.ToString();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                var response = await ApiClient.Publish(url, parameters);
                await PublishAfter();
                return response.Success;
            }

            return false;
        }

        public async Task<bool> PublishLogs(IEnumerable<TrakHoundLogEntry> entries, bool async = false, string routerId = null)
        {
            if (!entries.IsNullOrEmpty())
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, $"{_logsRoute}/batch");

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
