// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrakHound.Requests;

namespace TrakHound.Clients
{
    public abstract partial class TrakHoundEntitiesClientBase
    {
        private const string _statesRoute = "state";


        public async Task<IEnumerable<TrakHoundState>> GetStates(string objectPath, int skip = _defaultSkip, int take = _defaultTake, SortOrder sortOrder = _defaultSortOrder, bool includePrevious = false, string routerId = null)
        {
            if (!string.IsNullOrEmpty(objectPath))
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _statesRoute);

                var parameters = new Dictionary<string, string>();
                parameters["objectPath"] = objectPath;
                parameters["start"] = "1";
                parameters["stop"] = "now";
                parameters["skip"] = skip.ToString();
                parameters["take"] = take.ToString();
                parameters["sortOrder"] = ((int)sortOrder).ToString();
                parameters["includePrevious"] = includePrevious.ToString();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.QueryJson<IEnumerable<TrakHoundState>>(url, parameters);
            }

            return null;
        }

        public async Task<IEnumerable<TrakHoundState>> GetStates(string objectPath, DateTime start, DateTime stop, int skip = _defaultSkip, int take = _defaultTake, SortOrder sortOrder = _defaultSortOrder, bool includePrevious = false, string routerId = null)
        {
            if (!string.IsNullOrEmpty(objectPath) && start > DateTime.MinValue && stop > DateTime.MinValue)
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _statesRoute);

                var parameters = new Dictionary<string, string>();
                parameters["objectPath"] = objectPath;
                parameters["start"] = start.ToUnixTime().ToString();
                parameters["stop"] = stop.ToUnixTime().ToString();
                parameters["skip"] = skip.ToString();
                parameters["take"] = take.ToString();
                parameters["sortOrder"] = ((int)sortOrder).ToString();
                parameters["includePrevious"] = includePrevious.ToString();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.QueryJson<IEnumerable<TrakHoundState>>(url, parameters);
            }

            return null;
        }

        public async Task<IEnumerable<TrakHoundState>> GetStates(string objectPath, string startExpression, string stopExpression, int skip = _defaultSkip, int take = _defaultTake, SortOrder sortOrder = _defaultSortOrder, bool includePrevious = false, string routerId = null)
        {
            if (!string.IsNullOrEmpty(objectPath) && !string.IsNullOrEmpty(startExpression) && !string.IsNullOrEmpty(stopExpression))
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _statesRoute);

                var parameters = new Dictionary<string, string>();
                parameters["objectPath"] = objectPath;
                parameters["start"] = startExpression;
                parameters["stop"] = stopExpression;
                parameters["skip"] = skip.ToString();
                parameters["take"] = take.ToString();
                parameters["sortOrder"] = ((int)sortOrder).ToString();
                parameters["includePrevious"] = includePrevious.ToString();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.QueryJson<IEnumerable<TrakHoundState>>(url, parameters);
            }

            return null;
        }


        public async Task<TrakHoundState> GetLatestState(string objectPath, string routerId = null)
        {
            if (!string.IsNullOrEmpty(objectPath))
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _statesRoute);

                var parameters = new Dictionary<string, string>();
                parameters["objectPath"] = objectPath;
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return (await ApiClient.QueryJson<IEnumerable<TrakHoundState>>(url, parameters))?.FirstOrDefault();
            }

            return null;
        }

        public async Task<IEnumerable<TrakHoundState>> GetLatestStates(string objectPath, string routerId = null)
        {
            if (!string.IsNullOrEmpty(objectPath))
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _statesRoute);

                var parameters = new Dictionary<string, string>();
                parameters["objectPath"] = objectPath;
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.QueryJson<IEnumerable<TrakHoundState>>(url, parameters);
            }

            return null;
        }

        public async Task<IEnumerable<TrakHoundState>> GetLatestStates(IEnumerable<string> objectPaths, string routerId = null)
        {
            if (!objectPaths.IsNullOrEmpty())
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _statesRoute);

                var parameters = new Dictionary<string, string>();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.QueryJson<IEnumerable<TrakHoundState>>(url, objectPaths, queryParameters: parameters);
            }

            return null;
        }


        public async Task<ITrakHoundConsumer<IEnumerable<TrakHoundState>>> SubscribeStates(string objectPath, string routerId = null)
        {
            if (!string.IsNullOrEmpty(objectPath))
            {
                var route = Url.Combine(ApiRoute, _objectsRoute, _statesRoute);

                var parameters = new Dictionary<string, string>();
                parameters["objectPath"] = objectPath;
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.SubscribeJson<IEnumerable<TrakHoundState>>(route, parameters);
            }

            return null;
        }

        public async Task<ITrakHoundConsumer<IEnumerable<TrakHoundState>>> SubscribeStates(IEnumerable<string> objectPaths, string routerId = null)
        {
            if (!objectPaths.IsNullOrEmpty())
            {
                var route = Url.Combine(ApiRoute, _objectsRoute, _statesRoute);

                var parameters = new Dictionary<string, string>();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.SubscribeJson<IEnumerable<TrakHoundState>>(route, objectPaths, queryParameters: parameters);
            }

            return null;
        }


        public async Task<bool> PublishState(string objectPath, string definitionUuid, int ttl = 0, DateTime? timestamp = null, bool async = false, string routerId = null)
        {
            if (!string.IsNullOrEmpty(objectPath) && !string.IsNullOrEmpty(definitionUuid))
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _statesRoute);

                var parameters = new Dictionary<string, string>();
                parameters["objectPath"] = objectPath;
                parameters["definition"] = definitionUuid;
                if (ttl > 0) parameters["ttl"] = ttl.ToString();
                if (timestamp != null) parameters["timestamp"] = timestamp.Value.ToUnixTime().ToString();
                parameters["async"] = async.ToString();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                var response = await ApiClient.Publish(url, parameters);
                await PublishAfter();
                return response.Success;
            }

            return false;
        }

        public async Task<bool> PublishStates(IEnumerable<TrakHoundStateEntry> entries, bool async = false, string routerId = null)
        {
            if (!entries.IsNullOrEmpty())
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, $"{_statesRoute}/batch");

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
