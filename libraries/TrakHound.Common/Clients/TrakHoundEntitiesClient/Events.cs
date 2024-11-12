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
        private const string _eventsRoute = "event";


        public async Task<IEnumerable<TrakHoundEvent>> GetEvents(string objectPath, int skip = _defaultSkip, int take = _defaultTake, SortOrder sortOrder = _defaultSortOrder, string routerId = null)
        {
            if (!string.IsNullOrEmpty(objectPath))
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _eventsRoute);

                var parameters = new Dictionary<string, string>();
                parameters["objectPath"] = objectPath;
                parameters["start"] = "1";
                parameters["stop"] = "now";
                parameters["skip"] = skip.ToString();
                parameters["take"] = take.ToString();
                parameters["sortOrder"] = ((int)sortOrder).ToString();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.QueryJson<IEnumerable<TrakHoundEvent>>(url, parameters);
            }

            return null;
        }

        public async Task<IEnumerable<TrakHoundEvent>> GetEvents(string objectPath, DateTime start, DateTime stop, int skip = _defaultSkip, int take = _defaultTake, SortOrder sortOrder = _defaultSortOrder, string routerId = null)
        {
            if (!string.IsNullOrEmpty(objectPath) && start > DateTime.MinValue && stop > DateTime.MinValue)
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _eventsRoute);

                var parameters = new Dictionary<string, string>();
                parameters["objectPath"] = objectPath;
                parameters["start"] = start.ToUnixTime().ToString();
                parameters["stop"] = stop.ToUnixTime().ToString();
                parameters["skip"] = skip.ToString();
                parameters["take"] = take.ToString();
                parameters["sortOrder"] = ((int)sortOrder).ToString();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.QueryJson<IEnumerable<TrakHoundEvent>>(url, parameters);
            }

            return null;
        }

        public async Task<IEnumerable<TrakHoundEvent>> GetEvents(string objectPath, string startExpression, string stopExpression, int skip = _defaultSkip, int take = _defaultTake, SortOrder sortOrder = _defaultSortOrder, string routerId = null)
        {
            if (!string.IsNullOrEmpty(objectPath) && !string.IsNullOrEmpty(startExpression) && !string.IsNullOrEmpty(stopExpression))
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _eventsRoute);

                var parameters = new Dictionary<string, string>();
                parameters["objectPath"] = objectPath;
                parameters["start"] = startExpression;
                parameters["stop"] = stopExpression;
                parameters["skip"] = skip.ToString();
                parameters["take"] = take.ToString();
                parameters["sortOrder"] = ((int)sortOrder).ToString();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.QueryJson<IEnumerable<TrakHoundEvent>>(url, parameters);
            }

            return null;
        }


        public async Task<TrakHoundEvent> GetLatestEvent(string objectPath, string routerId = null)
        {
            if (!string.IsNullOrEmpty(objectPath))
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _eventsRoute);

                var parameters = new Dictionary<string, string>();
                parameters["objectPath"] = objectPath;
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return (await ApiClient.QueryJson<IEnumerable<TrakHoundEvent>>(url, parameters))?.FirstOrDefault();
            }

            return null;
        }

        public async Task<IEnumerable<TrakHoundEvent>> GetLatestEvents(string objectPath, string routerId = null)
        {
            if (!string.IsNullOrEmpty(objectPath))
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _eventsRoute);

                var parameters = new Dictionary<string, string>();
                parameters["objectPath"] = objectPath;
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.QueryJson<IEnumerable<TrakHoundEvent>>(url, parameters);
            }

            return null;
        }

        public async Task<IEnumerable<TrakHoundEvent>> GetLatestEvents(IEnumerable<string> objectPaths, string routerId = null)
        {
            if (!objectPaths.IsNullOrEmpty())
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _eventsRoute);

                var parameters = new Dictionary<string, string>();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.QueryJson<IEnumerable<TrakHoundEvent>>(url, objectPaths, queryParameters: parameters);
            }

            return null;
        }


        public async Task<ITrakHoundConsumer<IEnumerable<TrakHoundEvent>>> SubscribeEvents(string objectPath, string routerId = null)
        {
            if (!string.IsNullOrEmpty(objectPath))
            {
                var route = Url.Combine(ApiRoute, _objectsRoute, _eventsRoute);

                var parameters = new Dictionary<string, string>();
                parameters["objectPath"] = objectPath;
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.SubscribeJson<IEnumerable<TrakHoundEvent>>(route, parameters);
            }

            return null;
        }

        public async Task<ITrakHoundConsumer<IEnumerable<TrakHoundEvent>>> SubscribeEvents(IEnumerable<string> objectPaths, string routerId = null)
        {
            if (!objectPaths.IsNullOrEmpty())
            {
                var route = Url.Combine(ApiRoute, _objectsRoute, _eventsRoute);

                var parameters = new Dictionary<string, string>();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.SubscribeJson<IEnumerable<TrakHoundEvent>>(route, objectPaths, queryParameters: parameters);
            }

            return null;
        }


        public async Task<bool> PublishEvent(string objectPath, string targetPath, DateTime? timestamp = null, bool async = false, string routerId = null)
        {
            if (!string.IsNullOrEmpty(objectPath) && !string.IsNullOrEmpty(targetPath))
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _eventsRoute);

                var parameters = new Dictionary<string, string>();
                parameters["objectPath"] = objectPath;
                parameters["targetPath"] = targetPath;
                if (timestamp != null) parameters["timestamp"] = timestamp.Value.ToUnixTime().ToString();
                parameters["async"] = async.ToString();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                var response = await ApiClient.Publish(url, parameters);
                await PublishAfter();
                return response.Success;
            }

            return false;
        }

        public async Task<bool> PublishEvents(IEnumerable<TrakHoundEventEntry> entries, bool async = false, string routerId = null)
        {
            if (!entries.IsNullOrEmpty())
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, $"{_eventsRoute}/batch");

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
