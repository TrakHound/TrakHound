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
    [TrakHoundApiController("objects/event")]
    public class Events : EntitiesApiControllerBase
    {
        public Events() { }

        public Events(ITrakHoundApiConfiguration configuration, ITrakHoundClient client)
        {
            SetConfiguration(configuration);
            SetClient(client);
        }


        [TrakHoundApiQuery]
        public async Task<TrakHoundApiResponse> GetEvents(
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

                IEnumerable<ITrakHoundObjectEventEntity> entities;
                if (startTimestamp > 0 && stopTimestamp > 0)
                {
                    entities = await Client.System.Entities.Objects.Event.QueryByObject(paths, startTimestamp, stopTimestamp, skip, take, (SortOrder)sortOrder, routerId);
                }
                else
                {
                    entities = await Client.System.Entities.Objects.Event.LatestByObject(paths, routerId);
                }

                if (!entities.IsNullOrEmpty())
                {
                    var objectUuids = new List<string>();
                    objectUuids.AddRange(entities.Select(o => o.ObjectUuid));
                    objectUuids.AddRange(entities.Select(o => o.TargetUuid));

                    var objects = await Client.System.Entities.Objects.ReadByUuid(objectUuids.Distinct(), routerId);
                    var dObjects = objects?.ToDictionary(o => o.Uuid);

                    var results = new List<TrakHoundEvent>();
                    foreach (var entity in entities)
                    {
                        var obj = dObjects?.GetValueOrDefault(entity.ObjectUuid);
                        var target = dObjects?.GetValueOrDefault(entity.TargetUuid);

                        if (obj != null && target != null)
                        {
                            var result = new TrakHoundEvent();
                            result.Uuid = entity.Uuid;
                            result.Object = obj.GetAbsolutePath();
                            result.ObjectUuid = entity.ObjectUuid;
                            result.Target = target.GetAbsolutePath();
                            result.TargetUuid = entity.TargetUuid;
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

        [TrakHoundApiSubscribe]
        public async Task<ITrakHoundConsumer<TrakHoundApiResponse>> SubscribeEvents(
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

                var consumer = await Client.System.Entities.Objects.Event.SubscribeByObject(paths, interval, take, routerId: routerId);
                if (consumer != null)
                {
                    var resultConsumer = new TrakHoundConsumer<IEnumerable<ITrakHoundObjectEventEntity>, TrakHoundApiResponse>(consumer);
                    resultConsumer.OnReceivedAsync = async (entities) =>
                    {
                        var objectUuids = new List<string>();
                        objectUuids.AddRange(entities.Select(o => o.ObjectUuid));
                        objectUuids.AddRange(entities.Select(o => o.TargetUuid));

                        var objects = await Client.System.Entities.Objects.ReadByUuid(objectUuids.Distinct(), routerId);
                        var dObjects = objects?.ToDictionary(o => o.Uuid);

                        var results = new List<TrakHoundEvent>();
                        foreach (var entity in entities)
                        {
                            var obj = dObjects?.GetValueOrDefault(entity.ObjectUuid);
                            var target = dObjects?.GetValueOrDefault(entity.TargetUuid);

                            if (obj != null && target != null)
                            {
                                var result = new TrakHoundEvent();
                                result.Uuid = entity.Uuid;
                                result.Object = obj.GetAbsolutePath();
                                result.ObjectUuid = entity.ObjectUuid;
                                result.Target = target.GetAbsolutePath();
                                result.TargetUuid = entity.TargetUuid;
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
        public async Task<TrakHoundApiResponse> PublishEvent(
            [FromQuery] string objectPath,
            [FromQuery] string targetPath,
            [FromQuery] string timestamp = null,
            [FromQuery] bool async = false,
            [FromQuery] string routerId = null
            )
        {
            if (!string.IsNullOrEmpty(objectPath) && !string.IsNullOrEmpty(targetPath))
            {
                DateTime? ts = !string.IsNullOrEmpty(timestamp) ? timestamp.ToDateTime() : null;

                var entry = new TrakHoundEventEntry();
                entry.ObjectPath = objectPath;
                entry.TargetPath = targetPath;
                entry.Timestamp = ts;

                var entries = new TrakHoundEventEntry[] { entry };

                return await PublishEvents(entries, async, routerId);
            }
            else
            {
                return BadRequest();
            }
        }

        [TrakHoundApiPublish("batch")]
        public async Task<TrakHoundApiResponse> PublishEvents(
            [FromBody("application/json")] IEnumerable<TrakHoundEventEntry> entries,
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

        public void CreateEntities(TrakHoundEntityCollection collection, IEnumerable<TrakHoundEventEntry> entries, long created = 0)
        {
            if (!entries.IsNullOrEmpty())
            {
                var ts = created > 0 ? created : UnixDateTime.Now;

                // Create Objects that are referenced by Path in entries
                CreateContentObjects(collection, entries, ts);

                foreach (var entry in entries)
                {
                    string objectUuid = TrakHoundPath.GetUuid(entry.ObjectPath);
                    string targetUuid = TrakHoundPath.GetUuid(entry.TargetPath);
                    if (!string.IsNullOrEmpty(objectUuid) && !string.IsNullOrEmpty(targetUuid))
                    {
                        var entityTimestamp = entry.Timestamp != null ? entry.Timestamp.Value.ToUnixTime() : ts;

                        // Add Entity to the publish list
                        collection.Add(new TrakHoundObjectEventEntity(objectUuid, targetUuid, timestamp: entityTimestamp, created: ts));
                    }
                }
            }
        }
    }
}
