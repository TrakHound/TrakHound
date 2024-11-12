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
        private const string _assignmentsRoute = "assignment";


        public async Task<IEnumerable<TrakHoundAssignment>> GetAssignments(string assigneePath, int skip = _defaultSkip, int take = _defaultTake, SortOrder sortOrder = _defaultSortOrder, string routerId = null)
        {
            if (!string.IsNullOrEmpty(assigneePath))
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _assignmentsRoute);

                var parameters = new Dictionary<string, string>();
                parameters["assigneePath"] = assigneePath;
                parameters["start"] = 1.ToString();
                parameters["stop"] = "now";
                parameters["skip"] = skip.ToString();
                parameters["take"] = take.ToString();
                parameters["sortOrder"] = ((int)sortOrder).ToString();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.QueryJson<IEnumerable<TrakHoundAssignment>>(url, parameters);
            }

            return null;
        }

        public async Task<IEnumerable<TrakHoundAssignment>> GetAssignments(IEnumerable<string> assigneePaths, int skip = _defaultSkip, int take = _defaultTake, SortOrder sortOrder = _defaultSortOrder, string routerId = null)
        {
            if (!assigneePaths.IsNullOrEmpty())
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _assignmentsRoute);

                var parameters = new Dictionary<string, string>();
                parameters["start"] = 1.ToString();
                parameters["stop"] = "now";
                parameters["skip"] = skip.ToString();
                parameters["take"] = take.ToString();
                parameters["sortOrder"] = ((int)sortOrder).ToString();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.QueryJson<IEnumerable<TrakHoundAssignment>>(url, assigneePaths, queryParameters: parameters);
            }

            return null;
        }

        public async Task<IEnumerable<TrakHoundAssignment>> GetAssignments(string assigneePath, DateTime start, DateTime stop, int skip = _defaultSkip, int take = _defaultTake, SortOrder sortOrder = _defaultSortOrder, string routerId = null)
        {
            if (!string.IsNullOrEmpty(assigneePath) && start > DateTime.MinValue && stop > DateTime.MinValue)
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _assignmentsRoute);

                var parameters = new Dictionary<string, string>();
                parameters["assigneePath"] = assigneePath;
                parameters["start"] = start.ToUnixTime().ToString();
                parameters["stop"] = stop.ToUnixTime().ToString();
                parameters["skip"] = skip.ToString();
                parameters["take"] = take.ToString();
                parameters["sortOrder"] = ((int)sortOrder).ToString();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.QueryJson<IEnumerable<TrakHoundAssignment>>(url, parameters);
            }

            return null;
        }

        public async Task<IEnumerable<TrakHoundAssignment>> GetAssignments(IEnumerable<string> assigneePaths, DateTime start, DateTime stop, int skip = _defaultSkip, int take = _defaultTake, SortOrder sortOrder = _defaultSortOrder, string routerId = null)
        {
            if (!assigneePaths.IsNullOrEmpty() && start > DateTime.MinValue && stop > DateTime.MinValue)
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _assignmentsRoute);

                var parameters = new Dictionary<string, string>();
                parameters["start"] = start.ToUnixTime().ToString();
                parameters["stop"] = stop.ToUnixTime().ToString();
                parameters["skip"] = skip.ToString();
                parameters["take"] = take.ToString();
                parameters["sortOrder"] = ((int)sortOrder).ToString();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.QueryJson<IEnumerable<TrakHoundAssignment>>(url, assigneePaths, queryParameters: parameters);
            }

            return null;
        }

        public async Task<IEnumerable<TrakHoundAssignment>> GetAssignments(string assigneePath, string startExpression, string stopExpression, int skip = _defaultSkip, int take = _defaultTake, SortOrder sortOrder = _defaultSortOrder, string routerId = null)
        {
            if (!string.IsNullOrEmpty(assigneePath) && !string.IsNullOrEmpty(startExpression) && !string.IsNullOrEmpty(stopExpression))
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _assignmentsRoute);

                var parameters = new Dictionary<string, string>();
                parameters["assigneePath"] = assigneePath;
                parameters["start"] = startExpression;
                parameters["stop"] = stopExpression;
                parameters["skip"] = skip.ToString();
                parameters["take"] = take.ToString();
                parameters["sortOrder"] = ((int)sortOrder).ToString();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.QueryJson<IEnumerable<TrakHoundAssignment>>(url, parameters);
            }

            return null;
        }

        public async Task<IEnumerable<TrakHoundAssignment>> GetAssignments(IEnumerable<string> assigneePaths, string startExpression, string stopExpression, int skip = _defaultSkip, int take = _defaultTake, SortOrder sortOrder = _defaultSortOrder, string routerId = null)
        {
            if (!assigneePaths.IsNullOrEmpty() && !string.IsNullOrEmpty(startExpression) && !string.IsNullOrEmpty(stopExpression))
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _assignmentsRoute);

                var parameters = new Dictionary<string, string>();
                parameters["start"] = startExpression;
                parameters["stop"] = stopExpression;
                parameters["skip"] = skip.ToString();
                parameters["take"] = take.ToString();
                parameters["sortOrder"] = ((int)sortOrder).ToString();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.QueryJson<IEnumerable<TrakHoundAssignment>>(url, assigneePaths, queryParameters: parameters);
            }

            return null;
        }


        public async Task<IEnumerable<TrakHoundAssignment>> GetCurrentAssignments(string assigneePath, string routerId = null)
        {
            if (!string.IsNullOrEmpty(assigneePath))
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _assignmentsRoute);

                var parameters = new Dictionary<string, string>();
                parameters["assigneePath"] = assigneePath;
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.QueryJson<IEnumerable<TrakHoundAssignment>>(url, parameters);
            }

            return null;
        }

        public async Task<IEnumerable<TrakHoundAssignment>> GetCurrentAssignments(IEnumerable<string> assigneePaths, string routerId = null)
        {
            if (!assigneePaths.IsNullOrEmpty())
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _assignmentsRoute);

                var parameters = new Dictionary<string, string>();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.QueryJson<IEnumerable<TrakHoundAssignment>>(url, assigneePaths, queryParameters: parameters);
            }

            return null;
        }


        public async Task<ITrakHoundConsumer<IEnumerable<TrakHoundAssignment>>> SubscribeAssignments(string assigneePath, string routerId = null)
        {
            if (!string.IsNullOrEmpty(assigneePath))
            {
                var route = Url.Combine(ApiRoute, _objectsRoute, _assignmentsRoute);

                var parameters = new Dictionary<string, string>();
                parameters["assigneePath"] = assigneePath;
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.SubscribeJson<IEnumerable<TrakHoundAssignment>>(route, parameters);
            }

            return null;
        }

        public async Task<ITrakHoundConsumer<IEnumerable<TrakHoundAssignment>>> SubscribeAssignments(IEnumerable<string> assigneePaths, string routerId = null)
        {
            if (!assigneePaths.IsNullOrEmpty())
            {
                var route = Url.Combine(ApiRoute, _objectsRoute, _assignmentsRoute);

                var parameters = new Dictionary<string, string>();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.SubscribeJson<IEnumerable<TrakHoundAssignment>>(route, assigneePaths, queryParameters: parameters);
            }

            return null;
        }


        public async Task<bool> PublishAssignment(string assigneePath, string memberPath, DateTime? addTimestamp = null, DateTime? removeTimestamp = null, bool async = false, string routerId = null)
        {
            if (!string.IsNullOrEmpty(assigneePath) && !string.IsNullOrEmpty(memberPath))
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _assignmentsRoute);

                var parameters = new Dictionary<string, string>();
                parameters["assigneePath"] = assigneePath;
                parameters["memberPath"] = memberPath;
                if (addTimestamp != null) parameters["addTimestamp"] = addTimestamp.Value.ToUnixTime().ToString();
                if (removeTimestamp != null) parameters["removeTimestamp"] = removeTimestamp.Value.ToUnixTime().ToString();
                parameters["async"] = async.ToString();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                var response = await ApiClient.Publish(url, parameters);
                await PublishAfter();
                return response.Success;
            }

            return false;
        }

        public async Task<bool> PublishAssignments(IEnumerable<TrakHoundAssignmentEntry> entries, bool async = false, string routerId = null)
        {
            if (!entries.IsNullOrEmpty())
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, $"{_assignmentsRoute}/batch");

                var parameters = new Dictionary<string, string>();
                parameters["async"] = async.ToString();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                var response = await ApiClient.Publish(url, entries, queryParameters: parameters);
                await PublishAfter();
                return response.Success;
            }

            return false;
        }

        public async Task<bool> RemoveAssignment(string assigneePath, string memberPath, DateTime? removeTimestamp = null, bool async = false, string routerId = null)
        {
            if (!string.IsNullOrEmpty(assigneePath) && !string.IsNullOrEmpty(memberPath))
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, $"{_assignmentsRoute}/remove");

                var parameters = new Dictionary<string, string>();
                parameters["assigneePath"] = assigneePath;
                parameters["memberPath"] = memberPath;
                if (removeTimestamp != null) parameters["removeTimestamp"] = removeTimestamp.Value.ToUnixTime().ToString();
                parameters["async"] = async.ToString();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                var response = await ApiClient.Publish(url, parameters);
                await PublishAfter();
                return response.Success;
            }

            return false;
        }
    }
}
