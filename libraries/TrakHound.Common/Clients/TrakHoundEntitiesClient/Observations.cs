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
        private const string _observationsRoute = "observation";


        public async Task<IEnumerable<TrakHoundObservation>> GetObservations(string objectPath, DateTime start, DateTime stop, int skip = _defaultSkip, int take = _defaultTake, SortOrder sortOrder = _defaultSortOrder, bool includePrevious = false, string routerId = null)
        {
            if (!string.IsNullOrEmpty(objectPath) && start > DateTime.MinValue && stop > DateTime.MinValue)
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _observationsRoute);

                var parameters = new Dictionary<string, string>();
                parameters["objectPath"] = objectPath;
                parameters["start"] = start.ToUnixTime().ToString();
                parameters["stop"] = stop.ToUnixTime().ToString();
                parameters["skip"] = skip.ToString();
                parameters["take"] = take.ToString();
                parameters["sortOrder"] = ((int)sortOrder).ToString();
                parameters["includePrevious"] = includePrevious.ToString();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.QueryJson<IEnumerable<TrakHoundObservation>>(url, parameters);
            }

            return null;
        }

        public async Task<IEnumerable<TrakHoundObservation>> GetObservations(string objectPath, string startExpression, string stopExpression, int skip = _defaultSkip, int take = _defaultTake, SortOrder sortOrder = _defaultSortOrder, bool includePrevious = false, string routerId = null)
        {
            if (!string.IsNullOrEmpty(objectPath) && !string.IsNullOrEmpty(startExpression) && !string.IsNullOrEmpty(stopExpression))
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _observationsRoute);

                var parameters = new Dictionary<string, string>();
                parameters["objectPath"] = objectPath;
                parameters["start"] = startExpression;
                parameters["stop"] = stopExpression;
                parameters["skip"] = skip.ToString();
                parameters["take"] = take.ToString();
                parameters["sortOrder"] = ((int)sortOrder).ToString();
                parameters["includePrevious"] = includePrevious.ToString();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.QueryJson<IEnumerable<TrakHoundObservation>>(url, parameters);
            }

            return null;
        }

        public async Task<IEnumerable<TrakHoundObservation>> GetObservations(IEnumerable<string> objectPaths, DateTime start, DateTime stop, int skip = _defaultSkip, int take = _defaultTake, SortOrder sortOrder = _defaultSortOrder, bool includePrevious = false, string routerId = null)
        {
            if (!objectPaths.IsNullOrEmpty() && start > DateTime.MinValue && stop > DateTime.MinValue)
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _observationsRoute);

                var parameters = new Dictionary<string, string>();
                parameters["start"] = start.ToUnixTime().ToString();
                parameters["stop"] = stop.ToUnixTime().ToString();
                parameters["skip"] = skip.ToString();
                parameters["take"] = take.ToString();
                parameters["sortOrder"] = ((int)sortOrder).ToString();
                parameters["includePrevious"] = includePrevious.ToString();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.QueryJson<IEnumerable<TrakHoundObservation>>(url, objectPaths, queryParameters: parameters);
            }

            return null;
        }

        public async Task<IEnumerable<TrakHoundObservation>> GetObservations(IEnumerable<string> objectPaths, string startExpression, string stopExpression, int skip = _defaultSkip, int take = _defaultTake, SortOrder sortOrder = _defaultSortOrder, bool includePrevious = false, string routerId = null)
        {
            if (!objectPaths.IsNullOrEmpty() && !string.IsNullOrEmpty(startExpression) && !string.IsNullOrEmpty(stopExpression))
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _observationsRoute);

                var parameters = new Dictionary<string, string>();
                parameters["start"] = startExpression;
                parameters["stop"] = stopExpression;
                parameters["skip"] = skip.ToString();
                parameters["take"] = take.ToString();
                parameters["sortOrder"] = ((int)sortOrder).ToString();
                parameters["includePrevious"] = includePrevious.ToString();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.QueryJson<IEnumerable<TrakHoundObservation>>(url, objectPaths, queryParameters: parameters);
            }

            return null;
        }


        public async Task<Dictionary<string, IEnumerable<TrakHoundObservationValue>>> GetObservationValues(string objectPath, DateTime start, DateTime stop, int skip = _defaultSkip, int take = _defaultTake, SortOrder sortOrder = _defaultSortOrder, bool includePrevious = false, string routerId = null)
        {
            if (!string.IsNullOrEmpty(objectPath) && start > DateTime.MinValue && stop > DateTime.MinValue)
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _observationsRoute);
                url = Url.Combine(url, "values");

                var parameters = new Dictionary<string, string>();
                parameters["objectPath"] = objectPath;
                parameters["start"] = start.ToUnixTime().ToString();
                parameters["stop"] = stop.ToUnixTime().ToString();
                parameters["skip"] = skip.ToString();
                parameters["take"] = take.ToString();
                parameters["sortOrder"] = ((int)sortOrder).ToString();
                parameters["includePrevious"] = includePrevious.ToString();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.QueryJson<Dictionary<string, IEnumerable<TrakHoundObservationValue>>>(url, parameters);
            }

            return null;
        }

        public async Task<Dictionary<string, IEnumerable<TrakHoundObservationValue>>> GetObservationValues(string objectPath, string startExpression, string stopExpression, int skip = _defaultSkip, int take = _defaultTake, SortOrder sortOrder = _defaultSortOrder, bool includePrevious = false, string routerId = null)
        {
            if (!string.IsNullOrEmpty(objectPath) && !string.IsNullOrEmpty(startExpression) && !string.IsNullOrEmpty(stopExpression))
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _observationsRoute);
                url = Url.Combine(url, "values");

                var parameters = new Dictionary<string, string>();
                parameters["objectPath"] = objectPath;
                parameters["start"] = startExpression;
                parameters["stop"] = stopExpression;
                parameters["skip"] = skip.ToString();
                parameters["take"] = take.ToString();
                parameters["sortOrder"] = ((int)sortOrder).ToString();
                parameters["includePrevious"] = includePrevious.ToString();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.QueryJson<Dictionary<string, IEnumerable<TrakHoundObservationValue>>>(url, parameters);
            }

            return null;
        }

        public async Task<Dictionary<string, IEnumerable<TrakHoundObservationValue>>> GetObservationValues(IEnumerable<string> objectPaths, DateTime start, DateTime stop, int skip = _defaultSkip, int take = _defaultTake, SortOrder sortOrder = _defaultSortOrder, bool includePrevious = false, string routerId = null)
        {
            if (!objectPaths.IsNullOrEmpty() && start > DateTime.MinValue && stop > DateTime.MinValue)
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _observationsRoute);
                url = Url.Combine(url, "values");

                var parameters = new Dictionary<string, string>();
                parameters["start"] = start.ToUnixTime().ToString();
                parameters["stop"] = stop.ToUnixTime().ToString();
                parameters["skip"] = skip.ToString();
                parameters["take"] = take.ToString();
                parameters["sortOrder"] = ((int)sortOrder).ToString();
                parameters["includePrevious"] = includePrevious.ToString();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.QueryJson<Dictionary<string, IEnumerable<TrakHoundObservationValue>>>(url, objectPaths, queryParameters: parameters);
            }

            return null;
        }

        public async Task<Dictionary<string, IEnumerable<TrakHoundObservationValue>>> GetObservationValues(IEnumerable<string> objectPaths, string startExpression, string stopExpression, int skip = _defaultSkip, int take = _defaultTake, SortOrder sortOrder = _defaultSortOrder, bool includePrevious = false, string routerId = null)
        {
            if (!objectPaths.IsNullOrEmpty() && !string.IsNullOrEmpty(startExpression) && !string.IsNullOrEmpty(stopExpression))
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _observationsRoute);
                url = Url.Combine(url, "values");

                var parameters = new Dictionary<string, string>();
                parameters["start"] = startExpression;
                parameters["stop"] = stopExpression;
                parameters["skip"] = skip.ToString();
                parameters["take"] = take.ToString();
                parameters["sortOrder"] = ((int)sortOrder).ToString();
                parameters["includePrevious"] = includePrevious.ToString();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.QueryJson<Dictionary<string, IEnumerable<TrakHoundObservationValue>>>(url, objectPaths, queryParameters: parameters);
            }

            return null;
        }


        public async Task<TValue> GetLatestObservationValue<TValue>(string objectPath, string routerId = null)
        {
            var observation = await GetLatestObservation(objectPath, routerId);
            if (observation != null)
            {
                try
                {
                    return (TValue)Convert.ChangeType(observation.Value, typeof(TValue));
                }
                catch { }
            }

            return default;
        }

        public async Task<TrakHoundObservation> GetLatestObservation(string objectPath, string routerId = null)
        {
            var observations = await GetLatestObservations(objectPath, routerId);
            if (!observations.IsNullOrEmpty()) return observations.FirstOrDefault();

            return null;
        }

        public async Task<IEnumerable<TrakHoundObservation>> GetLatestObservations(string objectPath, string routerId = null)
        {
            if (!string.IsNullOrEmpty(objectPath))
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _observationsRoute);

                var parameters = new Dictionary<string, string>();
                parameters["objectPath"] = objectPath;
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.QueryJson<IEnumerable<TrakHoundObservation>>(url, parameters);
            }

            return null;
        }

        public async Task<IEnumerable<TrakHoundObservation>> GetLatestObservations(IEnumerable<string> objectPaths, string routerId = null)
        {
            if (!objectPaths.IsNullOrEmpty())
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _observationsRoute);

                var parameters = new Dictionary<string, string>();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.QueryJson<IEnumerable<TrakHoundObservation>>(url, objectPaths, queryParameters: parameters);
            }

            return null;
        }


        public async Task<ITrakHoundConsumer<IEnumerable<TrakHoundObservation>>> SubscribeObservations(string objectPath, string routerId = null)
        {
            if (!string.IsNullOrEmpty(objectPath))
            {
                var route = Url.Combine(ApiRoute, _objectsRoute, _observationsRoute);

                var parameters = new Dictionary<string, string>();
                parameters["objectPath"] = objectPath;
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.SubscribeJson<IEnumerable<TrakHoundObservation>>(route, parameters);
            }

            return null;
        }

        public async Task<ITrakHoundConsumer<IEnumerable<TrakHoundObservation>>> SubscribeObservations(IEnumerable<string> objectPaths, string routerId = null)
        {
            if (!objectPaths.IsNullOrEmpty())
            {
                var route = Url.Combine(ApiRoute, _objectsRoute, _observationsRoute);

                var parameters = new Dictionary<string, string>();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.SubscribeJson<IEnumerable<TrakHoundObservation>>(route, objectPaths, queryParameters: parameters);
            }

            return null;
        }


        public async Task<bool> PublishObservation(
            string objectPath,
            object value,
            TrakHoundObservationDataType? dataType = null,
            DateTime? timestamp = null,
            ulong batchId = 0,
            ulong sequence = 0,
            bool async = false,
            string routerId = null
            )
        {
            if (!string.IsNullOrEmpty(objectPath) && value != null)
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _observationsRoute);

                var parameters = new Dictionary<string, string>();
                parameters["objectPath"] = objectPath;
                parameters["value"] = value.ToString();
                if (dataType != null) parameters["dataType"] = dataType.Value.ToString();
                if (timestamp != null) parameters["timestamp"] = timestamp.Value.ToUnixTime().ToString();
                parameters["batchId"] = batchId.ToString();
                parameters["sequence"] = sequence.ToString();
                parameters["async"] = async.ToString();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                var response = await ApiClient.Publish(url, parameters);
                await PublishAfter();
                return response.Success;
            }

            return false;
        }

        public async Task<bool> PublishObservations(IEnumerable<TrakHoundObservationEntry> entries, bool async = false, string routerId = null)
        {
            if (!entries.IsNullOrEmpty())
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, $"{_observationsRoute}/batch");

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
