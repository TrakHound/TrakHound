// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Entities;
using TrakHound.Requests;

namespace TrakHound.Clients
{
    public abstract partial class TrakHoundEntitiesClientBase
    {
        private const string _statisticsRoute = "statistic";


        public async Task<IEnumerable<TrakHoundStatistic>> GetStatistics(string objectPath, DateTime start, DateTime stop, TimeSpan span, int skip = _defaultSkip, int take = _defaultTake, SortOrder sortOrder = _defaultSortOrder, string routerId = null)
        {
            if (!string.IsNullOrEmpty(objectPath) && start > DateTime.MinValue && stop > DateTime.MinValue)
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _statisticsRoute);

                var parameters = new Dictionary<string, string>();
                parameters["objectPath"] = objectPath;
                parameters["start"] = start.ToUnixTime().ToString();
                parameters["stop"] = stop.ToUnixTime().ToString();
                parameters["span"] = ((long)span.TotalNanoseconds).ToString();
                parameters["skip"] = skip.ToString();
                parameters["take"] = take.ToString();
                parameters["sortOrder"] = ((int)sortOrder).ToString();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.QueryJson<IEnumerable<TrakHoundStatistic>>(url, parameters);
            }

            return null;
        }

        public async Task<IEnumerable<TrakHoundStatistic>> GetStatistics(string objectPath, string startExpression, string stopExpression, string spanExpression, int skip = _defaultSkip, int take = _defaultTake, SortOrder sortOrder = _defaultSortOrder, string routerId = null)
        {
            if (!string.IsNullOrEmpty(objectPath) && !string.IsNullOrEmpty(startExpression) && !string.IsNullOrEmpty(stopExpression))
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _statisticsRoute);

                var parameters = new Dictionary<string, string>();
                parameters["objectPath"] = objectPath;
                parameters["start"] = startExpression;
                parameters["stop"] = stopExpression;
                parameters["span"] = spanExpression;
                parameters["skip"] = skip.ToString();
                parameters["take"] = take.ToString();
                parameters["sortOrder"] = ((int)sortOrder).ToString();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.QueryJson<IEnumerable<TrakHoundStatistic>>(url, parameters);
            }

            return null;
        }

        public async Task<IEnumerable<TrakHoundStatistic>> GetStatistics(IEnumerable<string> objectPaths, DateTime start, DateTime stop, TimeSpan span, int skip = _defaultSkip, int take = _defaultTake, SortOrder sortOrder = _defaultSortOrder, string routerId = null)
        {
            if (!objectPaths.IsNullOrEmpty() && start > DateTime.MinValue && stop > DateTime.MinValue)
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _statisticsRoute);

                var parameters = new Dictionary<string, string>();
                parameters["start"] = start.ToUnixTime().ToString();
                parameters["stop"] = stop.ToUnixTime().ToString();
                parameters["span"] = ((long)span.TotalNanoseconds).ToString();
                parameters["skip"] = skip.ToString();
                parameters["take"] = take.ToString();
                parameters["sortOrder"] = ((int)sortOrder).ToString();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.QueryJson<IEnumerable<TrakHoundStatistic>>(url, objectPaths, queryParameters: parameters);
            }

            return null;
        }

        public async Task<IEnumerable<TrakHoundStatistic>> GetStatistics(IEnumerable<string> objectPaths, string startExpression, string stopExpression, string spanExpression, int skip = _defaultSkip, int take = _defaultTake, SortOrder sortOrder = _defaultSortOrder, string routerId = null)
        {
            if (!objectPaths.IsNullOrEmpty() && !string.IsNullOrEmpty(startExpression) && !string.IsNullOrEmpty(stopExpression))
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _statisticsRoute);

                var parameters = new Dictionary<string, string>();
                parameters["start"] = startExpression;
                parameters["stop"] = stopExpression;
                parameters["span"] = spanExpression;
                parameters["skip"] = skip.ToString();
                parameters["take"] = take.ToString();
                parameters["sortOrder"] = ((int)sortOrder).ToString();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.QueryJson<IEnumerable<TrakHoundStatistic>>(url, objectPaths, queryParameters: parameters);
            }

            return null;
        }


        public async Task<ITrakHoundConsumer<IEnumerable<TrakHoundStatistic>>> SubscribeStatistics(string objectPath, string routerId = null)
        {
            if (!string.IsNullOrEmpty(objectPath))
            {
                var route = Url.Combine(ApiRoute, _objectsRoute, _statisticsRoute);

                var parameters = new Dictionary<string, string>();
                parameters["objectPath"] = objectPath;
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.SubscribeJson<IEnumerable<TrakHoundStatistic>>(route, parameters);
            }

            return null;
        }

        public async Task<ITrakHoundConsumer<IEnumerable<TrakHoundStatistic>>> SubscribeStatistics(IEnumerable<string> objectPaths, string routerId = null)
        {
            if (!objectPaths.IsNullOrEmpty())
            {
                var route = Url.Combine(ApiRoute, _objectsRoute, _statisticsRoute);

                var parameters = new Dictionary<string, string>();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.SubscribeJson<IEnumerable<TrakHoundStatistic>>(route, objectPaths, queryParameters: parameters);
            }

            return null;
        }


        public async Task<bool> PublishStatistic(string objectPath, DateTime rangeStart, DateTime rangeEnd, object value, TrakHoundStatisticDataType? dataType = null, DateTime? timestamp = null, TrakHoundUpdateType aggregateType = TrakHoundUpdateType.Absolute, bool async = false, string routerId = null)
        {
            if (!string.IsNullOrEmpty(objectPath) && value != null)
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _statisticsRoute);

                var parameters = new Dictionary<string, string>();
                parameters["objectPath"] = objectPath;
                parameters["rangeStart"] = rangeStart.ToUnixTime().ToString();
                parameters["rangeEnd"] = rangeEnd.ToUnixTime().ToString();
                parameters["value"] = value.ToString();
                if (dataType != null) parameters["dataType"] = dataType.Value.ToString();
                if (timestamp != null) parameters["timestamp"] = timestamp.Value.ToUnixTime().ToString();
                parameters["async"] = async.ToString();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                var response = await ApiClient.Publish(url, parameters);
                await PublishAfter();
                return response.Success;
            }

            return false;
        }

        public async Task<bool> PublishStatistic(string objectPath, string rangeStart, string rangeEnd, object value, TrakHoundStatisticDataType? dataType = null, DateTime? timestamp = null, TrakHoundUpdateType aggregateType = TrakHoundUpdateType.Absolute, bool async = false, string routerId = null)
        {
            if (!string.IsNullOrEmpty(objectPath) && value != null)
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _statisticsRoute);

                var parameters = new Dictionary<string, string>();
                parameters["objectPath"] = objectPath;
                parameters["rangeStart"] = rangeStart;
                parameters["rangeEnd"] = rangeEnd;
                parameters["value"] = value.ToString();
                if (dataType != null) parameters["dataType"] = dataType.Value.ToString();
                if (timestamp != null) parameters["timestamp"] = timestamp.Value.ToUnixTime().ToString();
                parameters["async"] = async.ToString();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                var response = await ApiClient.Publish(url, parameters);
                await PublishAfter();
                return response.Success;
            }

            return false;
        }

        public async Task<bool> PublishStatistics(IEnumerable<TrakHoundStatisticEntry> entries, bool async = false, string routerId = null)
        {
            if (!entries.IsNullOrEmpty())
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, $"{_statisticsRoute}/batch");

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
