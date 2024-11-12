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
    [TrakHoundApiController("objects/state")]
    public class States : EntitiesApiControllerBase
    {
        public States() { }

        public States(ITrakHoundApiConfiguration configuration, ITrakHoundClient client)
        {
            SetConfiguration(configuration);
            SetClient(client);
        }


        [TrakHoundApiQuery]
        public async Task<TrakHoundApiResponse> GetStates(
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

                var entities = new List<ITrakHoundObjectStateEntity>();

                // Include previous observation (last observation before range)
                if (includePrevious && startTimestamp > 0 && stopTimestamp > 0)
                {
                    var previousEntities = await Client.System.Entities.Objects.State.LastByObject(paths, startTimestamp, routerId);
                    if (!previousEntities.IsNullOrEmpty()) entities.AddRange(previousEntities);
                }

                // Get Range observations
                IEnumerable<ITrakHoundObjectStateEntity> rangeEntities;
                if (startTimestamp > 0 && stopTimestamp > 0)
                {
                    rangeEntities = await Client.System.Entities.Objects.State.QueryByObject(paths, startTimestamp, stopTimestamp, skip, take, (SortOrder)sortOrder, routerId);
                    if (!rangeEntities.IsNullOrEmpty()) entities.AddRange(rangeEntities);
                }
                else
                {
                    rangeEntities = await Client.System.Entities.Objects.State.LatestByObject(paths, routerId);
                    if (!rangeEntities.IsNullOrEmpty()) entities.AddRange(rangeEntities);
                }

                if (!entities.IsNullOrEmpty())
                {
                    var objectUuids = entities.Select(o => o.ObjectUuid).Distinct();
                    var objects = await Client.System.Entities.Objects.ReadByUuid(objectUuids, routerId);
                    var dObjects = objects?.ToDictionary(o => o.Uuid);

                    var definitionUuids = entities.Select(o => o.DefinitionUuid).Distinct();
                    var definitions = await Client.System.Entities.Definitions.ReadByUuid(definitionUuids, routerId);
                    var dDefinitions = definitions?.ToDictionary(o => o.Uuid);

                    var descriptions = await Client.System.Entities.Definitions.Description.QueryByDefinitionUuid(definitionUuids, routerId);
                    var dDescriptions = descriptions?.ToListDictionary(o => o.Uuid);

                    var results = new List<TrakHoundState>();
                    foreach (var entity in entities)
                    {
                        var obj = dObjects?.GetValueOrDefault(entity.ObjectUuid);
                        var definition = dDefinitions?.GetValueOrDefault(entity.DefinitionUuid);
                        var description = dDescriptions?.Get(entity.DefinitionUuid)?.FirstOrDefault(o => o.LanguageCode == "en");

                        if (obj != null && definition != null)
                        {
                            var result = new TrakHoundState();
                            result.Uuid = entity.Uuid;
                            result.Object = obj.GetAbsolutePath();
                            result.ObjectUuid = entity.ObjectUuid;
                            result.DefinitionUuid = entity.DefinitionUuid;
                            result.DefinitionId = definition.Id;
                            result.Type = definition.Type;
                            result.Description = description?.Text;
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
        public async Task<ITrakHoundConsumer<TrakHoundApiResponse>> SubscribeStates(
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

                var consumer = await Client.System.Entities.Objects.State.SubscribeByObject(paths, interval, take, routerId: routerId);
                if (consumer != null)
                {
                    var resultConsumer = new TrakHoundConsumer<IEnumerable<ITrakHoundObjectStateEntity>, TrakHoundApiResponse>(consumer);
                    resultConsumer.OnReceivedAsync = async (entities) =>
                    {
                        var objectUuids = entities.Select(o => o.ObjectUuid).Distinct();
                        var objects = await Client.System.Entities.Objects.ReadByUuid(objectUuids, routerId);
                        var dObjects = objects?.ToDictionary(o => o.Uuid);

                        var definitionUuids = entities.Select(o => o.DefinitionUuid).Distinct();
                        var definitions = await Client.System.Entities.Definitions.ReadByUuid(definitionUuids, routerId);
                        var dDefinitions = definitions?.ToDictionary(o => o.Uuid);

                        var descriptions = await Client.System.Entities.Definitions.Description.QueryByDefinitionUuid(definitionUuids, routerId);
                        var dDescriptions = descriptions?.ToListDictionary(o => o.Uuid);

                        var results = new List<TrakHoundState>();
                        foreach (var entity in entities)
                        {
                            var obj = dObjects?.GetValueOrDefault(entity.ObjectUuid);
                            var definition = dDefinitions?.GetValueOrDefault(entity.DefinitionUuid);
                            var description = dDescriptions?.Get(entity.DefinitionUuid)?.FirstOrDefault(o => o.LanguageCode == "en");

                            if (obj != null && definition != null)
                            {
                                var result = new TrakHoundState();
                                result.Uuid = entity.Uuid;
                                result.Object = obj.GetAbsolutePath();
                                result.ObjectUuid = entity.ObjectUuid;
                                result.DefinitionUuid = entity.DefinitionUuid;
                                result.DefinitionId = definition.Id;
                                result.Type = definition.Type;
                                result.Description = description?.Text;
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
        public async Task<TrakHoundApiResponse> PublishState(
            [FromQuery] string objectPath,
            [FromQuery] string definition = null,
            [FromQuery] string timestamp = null,
            [FromQuery] bool async = false,
            [FromQuery] string routerId = null
            )
        {
            if (!string.IsNullOrEmpty(objectPath) && !string.IsNullOrEmpty(definition))
            {
                DateTime? ts = !string.IsNullOrEmpty(timestamp) ? timestamp.ToDateTime() : null;

                var entry = new TrakHoundStateEntry();
                entry.ObjectPath = objectPath;
                entry.DefinitionId = definition;
                entry.Timestamp = ts;

                var entries = new TrakHoundStateEntry[] { entry };

                return await PublishStates(entries, async, routerId);
            }
            else
            {
                return BadRequest();
            }
        }

        [TrakHoundApiPublish("batch")]
        public async Task<TrakHoundApiResponse> PublishStates(
            [FromBody("application/json")] IEnumerable<TrakHoundStateEntry> entries,
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

        public void CreateEntities(TrakHoundEntityCollection collection, IEnumerable<TrakHoundStateEntry> entries, long created = 0)
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

                        // Add Definition
                        var definitionEntity = new TrakHoundDefinitionEntity(entry.DefinitionId, created: created);
                        collection.Add(definitionEntity);

                        // Add State
                        collection.Add(new TrakHoundObjectStateEntity(objectUuid, definitionEntity.Uuid, entry.TTL, timestamp: entityTimestamp, created: ts));
                    }
                }
            }
        }
    }
}
