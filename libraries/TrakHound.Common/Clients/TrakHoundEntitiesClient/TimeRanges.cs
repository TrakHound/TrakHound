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
        private const string _timeRangesRoute = "time-range";


        public async Task<TrakHoundTimeRange> GetTimeRange(string objectPath, string routerId = null)
        {
            if (!string.IsNullOrEmpty(objectPath))
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _timeRangesRoute);

                var parameters = new Dictionary<string, string>();
                parameters["objectPath"] = objectPath;
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return (await ApiClient.QueryJson<IEnumerable<TrakHoundTimeRange>>(url, parameters))?.FirstOrDefault();
            }

            return null;
        }

        public async Task<IEnumerable<TrakHoundTimeRange>> GetTimeRanges(string objectPath, string routerId = null)
        {
            if (!string.IsNullOrEmpty(objectPath))
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _timeRangesRoute);

                var parameters = new Dictionary<string, string>();
                parameters["objectPath"] = objectPath;
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.QueryJson<IEnumerable<TrakHoundTimeRange>>(url, parameters);
            }

            return null;
        }

        public async Task<IEnumerable<TrakHoundTimeRange>> GetTimeRanges(IEnumerable<string> objectPaths, string routerId = null)
        {
            if (!objectPaths.IsNullOrEmpty())
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _timeRangesRoute);

                var parameters = new Dictionary<string, string>();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.QueryJson<IEnumerable<TrakHoundTimeRange>>(url, objectPaths, queryParameters: parameters);
            }

            return null;
        }

        public async Task<DateTime?> GetTimeRangeValue(string objectPath, string routerId = null)
        {
            if (!string.IsNullOrEmpty(objectPath))
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, $"{_timeRangesRoute}/values");

                var parameters = new Dictionary<string, string>();
                parameters["objectPath"] = objectPath;
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return (await ApiClient.QueryJson<Dictionary<string, string>>(url, parameters))?.FirstOrDefault().Value?.ToDateTime();
            }

            return null;
        }


        public async Task<ITrakHoundConsumer<IEnumerable<TrakHoundTimeRange>>> SubscribeTimeRanges(string objectPath, string routerId = null)
        {
            if (!string.IsNullOrEmpty(objectPath))
            {
                var route = Url.Combine(ApiRoute, _objectsRoute, _timeRangesRoute);

                var parameters = new Dictionary<string, string>();
                parameters["objectPath"] = objectPath;
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.SubscribeJson<IEnumerable<TrakHoundTimeRange>>(route, parameters);
            }

            return null;
        }


        public async Task<bool> PublishTimeRange(string objectPath, long start, long end, bool async = false, string routerId = null) => await PublishTimeRange(objectPath, start.ToDateTime(), end.ToDateTime(), async, routerId);
        
        public async Task<bool> PublishTimeRange(string objectPath, string timeRangeExpression, bool async = false, string routerId = null)
        {
            var timeRange = TimeRange.Parse(timeRangeExpression);

            return await PublishTimeRange(objectPath, timeRange.From, timeRange.To, async, routerId);
        }

        public async Task<bool> PublishTimeRange(string objectPath, DateTime start, DateTime end, bool async = false, string routerId = null)
        {
            if (!string.IsNullOrEmpty(objectPath) && start > DateTime.MinValue && end > DateTime.MinValue)
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _timeRangesRoute);

                var parameters = new Dictionary<string, string>();
                parameters["objectPath"] = objectPath;
                parameters["start"] = start.ToUnixTime().ToString();
                parameters["end"] = end.ToUnixTime().ToString();
                parameters["async"] = async.ToString();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                var response = await ApiClient.Publish(url, parameters);
                await PublishAfter();
                return response.Success;
            }

            return false;
        }


        public async Task<bool> DeleteTimeRange(string objectPath, string routerId = null)
        {
            if (!string.IsNullOrEmpty(objectPath))
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _timeRangesRoute);

                var parameters = new Dictionary<string, string>();
                parameters["objectPath"] = objectPath;
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                var response = await ApiClient.Delete(url, parameters);
                return response.Success;
            }

            return false;
        }
    }
}
