// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrakHound.Api;
using TrakHound.Clients;
using TrakHound.Entities.Collections;
using TrakHound.Http;
using TrakHound.Requests;

namespace TrakHound.Entities.Api
{
    [TrakHoundApiController("objects/observation")]
    public class Observations : EntitiesApiControllerBase
    {
        public Observations() { }

        public Observations(ITrakHoundApiConfiguration configuration, ITrakHoundClient client)
        {
            SetConfiguration(configuration);
            SetClient(client);
        }


        [TrakHoundApiQuery]
        public async Task<TrakHoundApiResponse> GetObservations(
            [FromQuery] string objectPath,
            [FromBody(ContentType = "application/json")] IEnumerable<string> objectPaths,
            [FromQuery] string start,
            [FromQuery] string stop,
            [FromQuery] long skip = 0,
            [FromQuery] long take = 1000,
            [FromQuery] int sortOrder = (int)SortOrder.Ascending,
            [FromQuery] bool includePrevious = false,
            [FromQuery] string routerId = null
            )
        {
            if (!string.IsNullOrEmpty(objectPath) || !objectPaths.IsNullOrEmpty())
            {
                var paths = !string.IsNullOrEmpty(objectPath) ? new string[] { objectPath } : objectPaths;

                var startTimestamp = start.ToDateTime().ToUnixTime();
                var stopTimestamp = stop.ToDateTime().ToUnixTime();

                var entities = new List<ITrakHoundObjectObservationEntity>();

                // Include previous observation (last observation before range)
                if (includePrevious && startTimestamp > 0 && stopTimestamp > 0)
                {
                    var previousEntities = await Client.System.Entities.Objects.Observation.LastByObject(paths, startTimestamp, routerId);
                    if (!previousEntities.IsNullOrEmpty()) entities.AddRange(previousEntities);
                }

                // Get Range observations
                IEnumerable<ITrakHoundObjectObservationEntity> rangeEntities;
                if (startTimestamp > 0 && stopTimestamp > 0)
                {
                    rangeEntities = await Client.System.Entities.Objects.Observation.QueryByObject(paths, startTimestamp, stopTimestamp, skip, take, (SortOrder)sortOrder, routerId);
                    if (!rangeEntities.IsNullOrEmpty()) entities.AddRange(rangeEntities);
                }
                else
                {
                    rangeEntities = await Client.System.Entities.Objects.Observation.LatestByObject(paths, routerId);
                    if (!rangeEntities.IsNullOrEmpty()) entities.AddRange(rangeEntities);
                }

                if (!entities.IsNullOrEmpty())
                {
                    var objectUuids = entities.Select(o => o.ObjectUuid).Distinct();
                    var objects = await Client.System.Entities.Objects.ReadByUuid(objectUuids, routerId);
                    var dObjects = objects?.ToDictionary(o => o.Uuid);

                    var results = new List<TrakHoundObservation>();
                    foreach (var entity in entities)
                    {
                        var obj = dObjects?.GetValueOrDefault(entity.ObjectUuid);
                        if (obj != null)
                        {
                            var result = new TrakHoundObservation();
                            result.Uuid = entity.Uuid;
                            result.Object = obj.GetAbsolutePath();
                            result.ObjectUuid = entity.ObjectUuid;
                            result.DataType = (TrakHoundObservationDataType)entity.DataType;
                            result.Value = entity.Value;
                            result.BatchId = entity.BatchId;
                            result.Sequence = entity.Sequence;
                            result.Timestamp = entity.Timestamp.ToLocalDateTime();
                            result.SourceUuid = entity.SourceUuid;
                            result.Created = entity.Created.ToLocalDateTime();
                            results.Add(result);
                        }
                    }
                    return Ok(results);
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                return BadRequest();
            }
        }

        [TrakHoundApiQuery("aggregate")]
        public async Task<TrakHoundApiResponse> AggregateObservations(
            [FromQuery] string objectPath,
            [FromBody(ContentType = "application/json")] IEnumerable<string> objectPaths,
            [FromQuery] string aggregateType,
            [FromQuery] string start,
            [FromQuery] string stop,
            [FromQuery] string routerId = null
            )
        {
            if (!string.IsNullOrEmpty(objectPath) || !objectPaths.IsNullOrEmpty())
            {
                var paths = !string.IsNullOrEmpty(objectPath) ? new string[] { objectPath } : objectPaths;

                var startTimestamp = start.ToDateTime().ToUnixTime();
                var stopTimestamp = stop.ToDateTime().ToUnixTime();

                var type = aggregateType.ConvertEnum<TrakHoundAggregateType>();

                var aggregates = await Client.System.Entities.Objects.Observation.AggregateByObject(paths, type, startTimestamp, stopTimestamp, routerId);
                if (!aggregates.IsNullOrEmpty())
                {
                    return Ok(TrakHoundHttpAggregateResponse.Create(aggregates));
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                return BadRequest();
            }
        }

        [TrakHoundApiQuery("aggregate/window")]
        public async Task<TrakHoundApiResponse> AggregateWindowObservations(
            [FromQuery] string objectPath,
            [FromBody(ContentType = "application/json")] IEnumerable<string> objectPaths,
            [FromQuery] string aggregateType,
            [FromQuery] string aggregateWindow,
            [FromQuery] string start,
            [FromQuery] string stop,
            [FromQuery] string routerId = null
            )
        {
            if (!string.IsNullOrEmpty(objectPath) || !objectPaths.IsNullOrEmpty())
            {
                var paths = !string.IsNullOrEmpty(objectPath) ? new string[] { objectPath } : objectPaths;

                var startTimestamp = start.ToDateTime().ToUnixTime();
                var stopTimestamp = stop.ToDateTime().ToUnixTime();

                var type = aggregateType.ConvertEnum<TrakHoundAggregateType>();
                var window = (long)aggregateWindow.ToTimeSpan().TotalNanoseconds;

                var aggregateWindows = await Client.System.Entities.Objects.Observation.AggregateWindowByObject(paths, type, window, startTimestamp, stopTimestamp, routerId);
                if (!aggregateWindows.IsNullOrEmpty())
                {
                    return Ok(TrakHoundHttpAggregateWindowResponse.Create(aggregateWindows));
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                return BadRequest();
            }
        }

        [TrakHoundApiQuery("values")]
        public async Task<TrakHoundApiResponse> GetObservationValues(
            [FromQuery] string objectPath,
            [FromBody(ContentType = "application/json")] IEnumerable<string> objectPaths,
            [FromQuery] string start,
            [FromQuery] string stop,
            [FromQuery] long skip = 0,
            [FromQuery] long take = 1000,
            [FromQuery] int sortOrder = (int)SortOrder.Ascending,
            [FromQuery] bool includePrevious = false,
            [FromQuery] string routerId = null
            )
        {
            if (!string.IsNullOrEmpty(objectPath) || !objectPaths.IsNullOrEmpty())
            {
                var paths = !string.IsNullOrEmpty(objectPath) ? new string[] { objectPath } : objectPaths;

                var startTimestamp = start.ToDateTime().ToUnixTime();
                var stopTimestamp = stop.ToDateTime().ToUnixTime();

                var entities = new List<ITrakHoundObjectObservationEntity>();

                // Include previous observation (last observation before range)
                if (includePrevious && startTimestamp > 0 && stopTimestamp > 0)
                {
                    var previousEntities = await Client.System.Entities.Objects.Observation.LastByObject(paths, startTimestamp, routerId);
                    if (!previousEntities.IsNullOrEmpty()) entities.AddRange(previousEntities);
                }

                // Get Range observations
                IEnumerable<ITrakHoundObjectObservationEntity> rangeEntities;
                if (startTimestamp > 0 && stopTimestamp > 0)
                {
                    rangeEntities = await Client.System.Entities.Objects.Observation.QueryByObject(paths, startTimestamp, stopTimestamp, skip, take, (SortOrder)sortOrder, routerId);
                    if (!rangeEntities.IsNullOrEmpty()) entities.AddRange(rangeEntities);
                }
                else
                {
                    rangeEntities = await Client.System.Entities.Objects.Observation.LatestByObject(paths, routerId);
                    if (!rangeEntities.IsNullOrEmpty()) entities.AddRange(rangeEntities);
                }

                if (!entities.IsNullOrEmpty())
                {
                    var results = new Dictionary<string, IEnumerable<TrakHoundObservationValue>>();

                    var objectUuids = entities.Select(o => o.ObjectUuid).Distinct();
                    var objects = await Client.System.Entities.Objects.ReadByUuid(objectUuids, routerId);
                    var dObjects = objects?.ToDictionary(o => o.Uuid);

                    foreach (var objectUuid in objectUuids)
                    {
                        var matchedEntities = entities.Where(o => o.ObjectUuid == objectUuid);
                        if (!matchedEntities.IsNullOrEmpty())
                        {
                            var obj = dObjects?.GetValueOrDefault(objectUuid);
                            if (obj != null && obj.Path != null)
                            {
                                var values = new List<TrakHoundObservationValue>();
                                foreach (var entity in matchedEntities)
                                {
                                    var value = new TrakHoundObservationValue();
                                    value.Value = entity.Value;
                                    value.Timestamp = entity.Timestamp.ToLocalDateTime();
                                    values.Add(value);
                                }

                                results.Add(obj.GetAbsolutePath(), values);
                            }
                        }
                    }
                    return Ok(results);
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                return BadRequest();
            }
        }

        [TrakHoundApiSubscribe]
        public async Task<ITrakHoundConsumer<TrakHoundApiResponse>> SubscribeObservations(
            [FromQuery] string objectPath,
            [FromBody(ContentType = "application/json")] IEnumerable<string> objectPaths,
            [FromQuery] int interval = _defaultSubscribeInterval,
            [FromQuery] int take = _defaultSubscribeTake,
            [FromQuery] string routerId = null
            )
        {
            if (!string.IsNullOrEmpty(objectPath) || !objectPaths.IsNullOrEmpty())
            {
                var paths = !string.IsNullOrEmpty(objectPath) ? new string[] { objectPath } : objectPaths;

                var consumer = await Client.System.Entities.Objects.Observation.SubscribeByObject(paths, interval, take, routerId: routerId);
                if (consumer != null)
                {
                    var resultConsumer = new TrakHoundConsumer<IEnumerable<ITrakHoundObjectObservationEntity>, TrakHoundApiResponse>(consumer);
                    resultConsumer.OnReceivedAsync = async (entities) =>
                    {
                        var objectUuids = entities.Select(o => o.ObjectUuid).Distinct();
                        var objects = await Client.System.Entities.Objects.ReadByUuid(objectUuids, routerId);
                        var dObjects = objects?.ToDictionary(o => o.Uuid);

                        var results = new List<TrakHoundObservation>();
                        foreach (var entity in entities)
                        {
                            var obj = dObjects?.GetValueOrDefault(entity.ObjectUuid);
                            if (obj != null)
                            {
                                var result = new TrakHoundObservation();
                                result.Uuid = entity.Uuid;
                                result.Object = obj.GetAbsolutePath();
                                result.ObjectUuid = entity.ObjectUuid;
                                result.DataType = (TrakHoundObservationDataType)entity.DataType;
                                result.Value = entity.Value;
                                result.BatchId = entity.BatchId;
                                result.Sequence = entity.Sequence;
                                result.Timestamp = entity.Timestamp.ToLocalDateTime();
                                result.SourceUuid = entity.SourceUuid;
                                result.Created = entity.Created.ToLocalDateTime();
                                results.Add(result);
                            }
                        }
                        return TrakHoundApiJsonResponse.Ok(results);
                    };

                    return resultConsumer;
                }
            }

            return null;
        }

        [TrakHoundApiPublish]
        public async Task<TrakHoundApiResponse> PublishObservation(
            [FromQuery] string objectPath,
            [FromQuery] string value,
            [FromQuery] string dataType = null,
            [FromQuery] ulong batchId = 0,
            [FromQuery] ulong sequence = 0,
            [FromQuery] string timestamp = null,
            [FromQuery] bool async = false,
            [FromQuery] string routerId = null
            )
        {
            var response = new TrakHoundApiResponse();

            if (!string.IsNullOrEmpty(objectPath) && !string.IsNullOrEmpty(value))
            {
                var dt = !string.IsNullOrEmpty(dataType) ? dataType.ConvertEnum<TrakHoundObservationDataType>() : TrakHoundObservationDataType.String;
                DateTime? ts = !string.IsNullOrEmpty(timestamp) ? timestamp.ToDateTime() : null;

                var entry = new TrakHoundObservationEntry();
                entry.ObjectPath = objectPath;
                entry.Value = value;
                entry.DataType = dt;
                entry.BatchId = batchId;
                entry.Sequence = sequence;
                entry.Timestamp = ts;

                var entries = new TrakHoundObservationEntry[] { entry };

                return await PublishObservations(entries, async, routerId);
            }
            else
            {
                response.StatusCode = 400;
            }

            return response;
        }

        [TrakHoundApiPublish("batch")]
        public async Task<TrakHoundApiResponse> PublishObservations(
            [FromBody("application/json")] IEnumerable<TrakHoundObservationEntry> entries,
            [FromQuery] bool async = false,
            [FromQuery] string routerId = null
            )
        {
            var response = new TrakHoundApiResponse();

            if (!entries.IsNullOrEmpty())
            {
                var now = UnixDateTime.Now;
                var publishCollection = new TrakHoundEntityCollection();

                CreateEntities(publishCollection, entries, now);

                // Publish Entities
                if (await PublishEntities(publishCollection, now, async, routerId: routerId))
                {
                    return Created();
                }
                else
                {
                    return InternalError();
                }
            }
            else
            {
                response.StatusCode = 400;
            }

            return response;
        }

        public void CreateEntities(TrakHoundEntityCollection collection, IEnumerable<TrakHoundObservationEntry> entries, long created = 0)
        {
            if (!entries.IsNullOrEmpty())
            {
                var ts = created > 0 ? created : UnixDateTime.Now;

                // Create Objects that are referenced by Path in entries
                CreateContentObjects(collection, entries, ts);

                foreach (var entry in entries)
                {
                    string objectUuid = TrakHoundPath.GetUuid(entry.ObjectPath);
                    if (!string.IsNullOrEmpty(objectUuid))
                    {
                        var batchId = entry.BatchId != null ? entry.BatchId.Value : 0;
                        var sequence = entry.Sequence != null ? entry.Sequence.Value : 0;
                        var entityTimestamp = entry.Timestamp != null ? entry.Timestamp.Value.ToUnixTime() : ts;

                        collection.Add(new TrakHoundObjectObservationEntity(objectUuid, entry.Value, entityTimestamp, batchId, sequence, dataType: (int)entry.DataType, created: ts));
                    }
                }
            }
        }
    }
}
