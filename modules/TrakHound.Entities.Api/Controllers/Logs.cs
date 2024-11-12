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
    [TrakHoundApiController("objects/log")]
    public class Logs : EntitiesApiControllerBase
    {
        public Logs() { }

        public Logs(ITrakHoundApiConfiguration configuration, ITrakHoundClient client)
        {
            SetConfiguration(configuration);
            SetClient(client);
        }


        [TrakHoundApiQuery]
        public async Task<TrakHoundApiResponse> GetLogs(
            [FromQuery] string objectPath,
            [FromBody(ContentType = "application/json")] IEnumerable<string> objectPaths,
            [FromQuery] string start = null,
            [FromQuery] string stop = null,
            [FromQuery] string level = null,
            [FromQuery] long skip = 0,
            [FromQuery] long take = 1000,
            [FromQuery] int sortOrder = (int)SortOrder.Ascending,
            [FromQuery] string routerId = null
            )
        {
            if (!string.IsNullOrEmpty(objectPath) || !objectPaths.IsNullOrEmpty())
            {
                var paths = !string.IsNullOrEmpty(objectPath) ? new string[] { objectPath } : objectPaths;
                var minimumLevel = !string.IsNullOrEmpty(level) ? level.ConvertEnum<TrakHoundLogLevel>() : TrakHoundLogLevel.Trace;

                if (!string.IsNullOrEmpty(start) || !string.IsNullOrEmpty(stop))
                {
                    var startTimestamp = Math.Max(0, start.ToDateTime().ToUnixTime());
                    var stopTimestamp = stop.ToDateTime().ToUnixTime();

                    if (startTimestamp >= 0 && stopTimestamp > 0)
                    {
                        var entities = await Client.System.Entities.Objects.Log.QueryByObject(paths, minimumLevel, startTimestamp, stopTimestamp, skip, take, (SortOrder)sortOrder, routerId);
                        if (!entities.IsNullOrEmpty())
                        {
                            var objectUuids = entities.Select(o => o.ObjectUuid).Distinct();
                            var objects = await Client.System.Entities.Objects.ReadByUuid(objectUuids, routerId);
                            var dObjects = objects?.ToDictionary(o => o.Uuid);

                            var results = new List<TrakHoundLog>();
                            foreach (var entity in entities)
                            {
                                var obj = dObjects?.GetValueOrDefault(entity.ObjectUuid);
                                if (obj != null)
                                {
                                    var result = new TrakHoundLog();
                                    result.Uuid = entity.Uuid;
                                    result.Object = obj.GetAbsolutePath();
                                    result.ObjectUuid = entity.ObjectUuid;
                                    result.Level = (TrakHoundLogLevel)entity.LogLevel;
                                    result.Message = entity.Message;
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
                else
                {
                    var entities = await Client.System.Entities.Objects.Log.QueryByObject(paths, minimumLevel, 0, long.MaxValue, skip, take, (SortOrder)sortOrder, routerId);
                    if (!entities.IsNullOrEmpty())
                    {
                        var objectUuids = entities.Select(o => o.ObjectUuid).Distinct();
                        var objects = await Client.System.Entities.Objects.ReadByUuid(objectUuids, routerId);
                        var dObjects = objects?.ToDictionary(o => o.Uuid);

                        var results = new List<TrakHoundLog>();
                        foreach (var entity in entities)
                        {
                            var obj = dObjects?.GetValueOrDefault(entity.ObjectUuid);
                            if (obj != null)
                            {
                                var result = new TrakHoundLog();
                                result.Uuid = entity.Uuid;
                                result.Object = obj.GetAbsolutePath();
                                result.ObjectUuid = entity.ObjectUuid;
                                result.Level = (TrakHoundLogLevel)entity.LogLevel;
                                result.Message = entity.Message;
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
            }
            else
            {
                return BadRequest();
            }
        }

        [TrakHoundApiQuery("values")]
        public async Task<TrakHoundApiResponse> GetLogValues(
            [FromQuery] string objectPath,
            [FromBody(ContentType = "application/json")] IEnumerable<string> objectPaths,
            [FromQuery] string start,
            [FromQuery] string stop,
            [FromQuery] string level = null,
            [FromQuery] long skip = 0,
            [FromQuery] long take = 1000,
            [FromQuery] int sortOrder = (int)SortOrder.Ascending,
            [FromQuery] string routerId = null
            )
        {
            if (!string.IsNullOrEmpty(objectPath) || !objectPaths.IsNullOrEmpty())
            {
                var paths = !string.IsNullOrEmpty(objectPath) ? new string[] { objectPath } : objectPaths;
                var minimumLevel = !string.IsNullOrEmpty(level) ? level.ConvertEnum<TrakHoundLogLevel>() : TrakHoundLogLevel.Trace;

                var startTimestamp = start.ToDateTime().ToUnixTime();
                var stopTimestamp = stop.ToDateTime().ToUnixTime();

                var entities = await Client.System.Entities.Objects.Log.QueryByObject(paths, minimumLevel, startTimestamp, stopTimestamp, skip, take, (SortOrder)sortOrder, routerId);
                if (!entities.IsNullOrEmpty())
                {
                    var objectUuids = entities.Select(o => o.ObjectUuid).Distinct();
                    var objects = await Client.System.Entities.Objects.ReadByUuid(objectUuids, routerId);
                    var dObjects = objects?.ToDictionary(o => o.Uuid);

                    if (!objects.IsNullOrEmpty())
                    {
                        var results = new Dictionary<string, IEnumerable<TrakHoundLogValue>>();
                        foreach (var objectEntity in objects)
                        {
                            var matchedEntities = entities.Where(o => o.ObjectUuid == objectEntity.Uuid);
                            if (!matchedEntities.IsNullOrEmpty())
                            {
                                var values = new List<TrakHoundLogValue>();
                                foreach (var entity in matchedEntities)
                                {
                                    var value = new TrakHoundLogValue();
                                    value.Level = (TrakHoundLogLevel)entity.LogLevel;
                                    value.Message = entity.Message;
                                    value.Timestamp = entity.Timestamp.ToLocalDateTime();
                                    values.Add(value);
                                }

                                results.Add(objectEntity.GetAbsolutePath(), values);
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
                    return NotFound();
                }
            }
            else
            {
                return BadRequest();
            }
        }

        [TrakHoundApiSubscribe]
        public async Task<ITrakHoundConsumer<TrakHoundApiResponse>> SubscribeLogs(
            [FromQuery] string objectPath,
            [FromBody(ContentType = "application/json")] IEnumerable<string> objectPaths,
            [FromQuery] string level = null,
            [FromQuery] int interval = _defaultSubscribeInterval,
            [FromQuery] int take = _defaultSubscribeTake,
            [FromQuery] string routerId = null
            )
        {
            if (!string.IsNullOrEmpty(objectPath) || !objectPaths.IsNullOrEmpty())
            {
                var paths = !string.IsNullOrEmpty(objectPath) ? new string[] { objectPath } : objectPaths;
                var minimumLevel = !string.IsNullOrEmpty(level) ? level.ConvertEnum<TrakHoundLogLevel>() : TrakHoundLogLevel.Trace;

                var consumer = await Client.System.Entities.Objects.Log.SubscribeByObject(paths, minimumLevel, interval, take, routerId: routerId);
                if (consumer != null)
                {
                    var resultConsumer = new TrakHoundConsumer<IEnumerable<ITrakHoundObjectLogEntity>, TrakHoundApiResponse>(consumer);
                    resultConsumer.OnReceivedAsync = async (entities) =>
                    {
                        var objectUuids = entities.Select(o => o.ObjectUuid).Distinct();
                        var objects = await Client.System.Entities.Objects.ReadByUuid(objectUuids, routerId);
                        var dObjects = objects?.ToDictionary(o => o.Uuid);

                        var results = new List<TrakHoundLog>();
                        foreach (var entity in entities)
                        {
                            var obj = dObjects?.GetValueOrDefault(entity.ObjectUuid);
                            if (obj != null)
                            {
                                var result = new TrakHoundLog();
                                result.Uuid = entity.Uuid;
                                result.Object = obj.GetAbsolutePath();
                                result.ObjectUuid = entity.ObjectUuid;
                                result.Level = (TrakHoundLogLevel)entity.LogLevel;
                                result.Message = entity.Message;
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
        public async Task<TrakHoundApiResponse> PublishLog(
            [FromQuery] string objectPath,
            [FromQuery] string message,
            [FromQuery] string level = null,
            [FromQuery] string code = null,
            [FromQuery] string timestamp = null,
            [FromQuery] bool async = false,
            [FromQuery] string routerId = null
            )
        {
            var response = new TrakHoundApiResponse();

            if (!string.IsNullOrEmpty(objectPath) && !string.IsNullOrEmpty(message))
            {
                var levelValue = !string.IsNullOrEmpty(level) ? level.ConvertEnum<TrakHoundLogLevel>() : TrakHoundLogLevel.Trace;

                DateTime? ts = !string.IsNullOrEmpty(timestamp) ? timestamp.ToDateTime() : null;

                var entry = new TrakHoundLogEntry();
                entry.ObjectPath = objectPath;
                entry.LogLevel = levelValue;
                entry.Message = message;
                entry.Code = code;
                entry.Timestamp = ts;

                var entries = new TrakHoundLogEntry[] { entry };

                return await PublishLogs(entries, async, routerId);
            }
            else
            {
                response.StatusCode = 400;
            }

            return response;
        }

        [TrakHoundApiPublish("batch")]
        public async Task<TrakHoundApiResponse> PublishLogs(
            [FromBody("application/json")] IEnumerable<TrakHoundLogEntry> entries,
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

        public void CreateEntities(TrakHoundEntityCollection collection, IEnumerable<TrakHoundLogEntry> entries, long created = 0)
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
                        var entityTimestamp = entry.Timestamp != null ? entry.Timestamp.Value.ToUnixTime() : ts;

                        // Add Entity to the publish list
                        collection.Add(new TrakHoundObjectLogEntity(objectUuid, entry.LogLevel, entry.Message, entry.Code, timestamp: entityTimestamp, created: ts));
                    }
                }
            }
        }
    }
}
