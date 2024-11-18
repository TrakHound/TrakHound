// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrakHound.Api;
using TrakHound.Clients;
using TrakHound.Entities.Collections;
using TrakHound.Requests;

namespace TrakHound.Entities.Api
{
    [TrakHoundApiController("objects/statistic")]
    public class Statistics : EntitiesApiControllerBase
    {
        public Statistics() { }

        public Statistics(ITrakHoundApiConfiguration configuration, ITrakHoundClient client)
        {
            SetConfiguration(configuration);
            SetClient(client);
        }


        [TrakHoundApiQuery]
        public async Task<TrakHoundApiResponse> GetStatistics(
            [FromQuery] string objectPath,
            [FromBody(ContentType = "application/json")] IEnumerable<string> objectPaths,
            [FromQuery] string start,
            [FromQuery] string stop,
            [FromQuery] string span,
            [FromQuery] long skip = 0,
            [FromQuery] long take = 1000,
            [FromQuery] int sortOrder = (int)SortOrder.Ascending,
            [FromQuery] string routerId = null
            )
        {
            if (!string.IsNullOrEmpty(objectPath) || !objectPaths.IsNullOrEmpty())
            {
                var paths = !string.IsNullOrEmpty(objectPath) ? new string[] { objectPath } : objectPaths;

                var startTimestamp = start.ToDateTime().ToUnixTime();
                var stopTimestamp = stop.ToDateTime().ToUnixTime();
                var timeRangeSpan = (long)span.ToTimeSpan().TotalNanoseconds;

                IEnumerable<ITrakHoundObjectStatisticEntity> entities = null;
                if (startTimestamp > 0 && stopTimestamp > 0)
                {
                    entities = await Client.System.Entities.Objects.Statistic.QueryByObject(paths, startTimestamp, stopTimestamp, timeRangeSpan, skip, take, (SortOrder)sortOrder, routerId);
                }

                if (!entities.IsNullOrEmpty())
                {
                    var objectUuids = entities.Select(o => o.ObjectUuid).Distinct();
                    var objects = await Client.System.Entities.Objects.ReadByUuid(objectUuids, routerId);
                    var dObjects = objects?.ToDictionary(o => o.Uuid);

                    var results = new List<TrakHoundStatistic>();
                    foreach (var entity in entities)
                    {
                        var obj = dObjects?.GetValueOrDefault(entity.ObjectUuid);
                        if (obj != null)
                        {
                            var result = new TrakHoundStatistic();
                            result.Uuid = entity.Uuid;
                            result.Object = obj.GetAbsolutePath();
                            result.ObjectUuid = entity.ObjectUuid;

                            result.Start = entity.TimeRangeStart.ToDateTime();
                            result.End = entity.TimeRangeEnd.ToDateTime();

                            result.DataType = (TrakHoundStatisticDataType)entity.DataType;
                            result.Value = entity.Value;
                            result.Timestamp = entity.Timestamp.ToLocalDateTime();
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

        [TrakHoundApiSubscribe]
        public async Task<ITrakHoundConsumer<TrakHoundApiResponse>> SubscribeStatistics(
            [FromQuery] string objectPath,
            [FromQuery] int interval = _defaultSubscribeInterval,
            [FromQuery] int take = _defaultSubscribeTake,
            [FromQuery] string routerId = null
            )
        {
            if (!string.IsNullOrEmpty(objectPath))
            {
                var objectPaths = new string[] { objectPath };
                var consumer = await Client.System.Entities.Objects.Statistic.SubscribeByObject(objectPaths, interval, take, routerId: routerId);
                if (consumer != null)
                {
                    var resultConsumer = new TrakHoundConsumer<IEnumerable<ITrakHoundObjectStatisticEntity>, TrakHoundApiResponse>(consumer);
                    resultConsumer.OnReceivedAsync = async (entities) =>
                    {
                        var objectUuids = entities.Select(o => o.ObjectUuid).Distinct();
                        var objects = await Client.System.Entities.Objects.ReadByUuid(objectUuids, routerId);
                        var dObjects = objects?.ToDictionary(o => o.Uuid);

                        var results = new List<TrakHoundStatistic>();
                        foreach (var entity in entities)
                        {
                            var obj = dObjects?.GetValueOrDefault(entity.ObjectUuid);
                            if (obj != null)
                            {
                                var result = new TrakHoundStatistic();
                                result.Uuid = entity.Uuid;
                                result.Object = obj.GetAbsolutePath();
                                result.ObjectUuid = entity.ObjectUuid;

                                result.Start = entity.TimeRangeStart.ToDateTime();
                                result.End = entity.TimeRangeEnd.ToDateTime();

                                result.DataType = (TrakHoundStatisticDataType)entity.DataType;
                                result.Value = entity.Value;
                                result.Timestamp = entity.Timestamp.ToLocalDateTime();
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
        public async Task<TrakHoundApiResponse> PublishStatistic(
            [FromQuery] string objectPath,
            [FromQuery] string value,
            [FromQuery] string dataType = null,
            [FromQuery] string rangeStart = null,
            [FromQuery] string rangeEnd = null,
            [FromQuery] string timestamp = null,
            [FromQuery] string aggregateType = null,
            [FromQuery] bool async = false,
            [FromQuery] string routerId = null
            )
        {
            var response = new TrakHoundApiResponse();

            if (!string.IsNullOrEmpty(objectPath) && (!string.IsNullOrEmpty(rangeStart) && !string.IsNullOrEmpty(rangeEnd)))
            {
                var dt = !string.IsNullOrEmpty(dataType) ? dataType.ConvertEnum<TrakHoundStatisticDataType>() : TrakHoundStatisticDataType.Float;
                DateTime? ts = !string.IsNullOrEmpty(timestamp) ? timestamp.ToDateTime() : null;

                var aggregate = aggregateType.ConvertEnum<TrakHoundUpdateType>();

                var entry = new TrakHoundStatisticEntry();
                entry.ObjectPath = objectPath;
                entry.Value = value;
                entry.DataType = dt;
                entry.AggregateType = aggregate;
                entry.RangeStart = rangeStart;
                entry.RangeEnd = rangeEnd;
                entry.Timestamp = ts;

                var entries = new TrakHoundStatisticEntry[] { entry };

                return await PublishStatistics(entries, async, routerId);
            }
            else
            {
                response.StatusCode = 400;
            }

            return response;
        }

        [TrakHoundApiPublish("batch")]
        public async Task<TrakHoundApiResponse> PublishStatistics(
            [FromBody("application/json")] IEnumerable<TrakHoundStatisticEntry> entries,
            [FromQuery] bool async = false,
            [FromQuery] string routerId = null
            )
        {
            var response = new TrakHoundApiResponse();

            if (!entries.IsNullOrEmpty())
            {
                var now = UnixDateTime.Now;
                var publishCollection = new TrakHoundEntityCollection();

                await CreateEntities(publishCollection, entries, now);

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

        public async Task CreateEntities(TrakHoundEntityCollection collection, IEnumerable<TrakHoundStatisticEntry> entries, long created = 0, string routerId = null)
        {
            if (!entries.IsNullOrEmpty())
            {
                var ts = created > 0 ? created : UnixDateTime.Now;

                // Create Objects that are referenced by Path in entries
                CreateContentObjects(collection, entries, ts);

                var incrementEntries = entries.Where(o => o.AggregateType == TrakHoundUpdateType.Increment);
                if (!incrementEntries.IsNullOrEmpty())
                {
                    var objectUuids = incrementEntries.Select(o => TrakHoundPath.GetUuid(o.ObjectPath)).Distinct();
                    foreach (var objectUuid in objectUuids)
                    {
                        var objectIncrementEntries = incrementEntries.Where(o => TrakHoundPath.GetUuid(o.ObjectPath) == objectUuid);
                        var objectTimeRangeIds = objectIncrementEntries.Select(o => GetTimeRangeId(o)).Distinct();
                        foreach (var timeRangeId in objectTimeRangeIds)
                        {
                            var timeRange = TimeRange.Parse(timeRangeId);
                            var rangeStart = timeRange.From.ToUnixTime();
                            var rangeEnd = timeRange.To.ToUnixTime();
                            var rangeSpan = (long)timeRange.Duration.TotalNanoseconds;

                            double value = 0;
                            long entityTimestamp = 0;
                            TrakHoundStatisticDataType? dataType = null;

                            var latest = (await Client.System.Entities.Objects.Statistic.QueryByObjectUuid(objectUuid, rangeStart, rangeEnd, rangeSpan, 0, 1, SortOrder.Descending, routerId))?.FirstOrDefault();
                            if (latest != null)
                            {
                                value = latest.Value.ToDouble();
                                entityTimestamp = latest.Timestamp;
                                created = latest.Created;
                                dataType = (TrakHoundStatisticDataType)latest.DataType;
                            }

                            foreach (var entry in objectIncrementEntries.Where(o => GetTimeRangeId(o) == timeRangeId).OrderBy(o => o.Timestamp))
                            {
                                value += entry.Value.ToDouble();
                                entityTimestamp = entry.Timestamp != null ? entry.Timestamp.Value.ToUnixTime() : ts;
                                dataType = entry.DataType;
                            }

                            collection.Add(new TrakHoundObjectStatisticEntity(objectUuid, rangeStart, rangeEnd, value, dataType: dataType, timestamp: entityTimestamp, created: ts));
                        }
                    }
                }

                var absoluteEntries = entries.Where(o => o.AggregateType == TrakHoundUpdateType.Absolute);
                if (!absoluteEntries.IsNullOrEmpty())
                {
                    foreach (var entry in absoluteEntries)
                    {
                        // Get the ObjectUuid for the given Path from the Transaction
                        string objectUuid = TrakHoundPath.GetUuid(entry.ObjectPath);
                        if (!string.IsNullOrEmpty(objectUuid))
                        {
                            var rangeStart = entry.RangeStart.ToDateTime();
                            var rangeEnd = entry.RangeEnd.ToDateTime();

                            var entityTimestamp = entry.Timestamp != null ? entry.Timestamp.Value.ToUnixTime() : ts;
                            TrakHoundStatisticDataType? dataType = entry.DataType;

                            // Add Entity to the publish list
                            collection.Add(new TrakHoundObjectStatisticEntity(objectUuid, rangeStart, rangeEnd, entry.Value, dataType: dataType, timestamp: entityTimestamp, created: ts));
                        }
                    }
                }
            }
        }

        private static string GetTimeRangeId(TrakHoundStatisticEntry entry)
        {
            return new TimeRange(entry.RangeStart.ToDateTime(), entry.RangeEnd.ToDateTime()).ToString();
        }
    }
}
