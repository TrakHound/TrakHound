// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrakHound.Api;
using TrakHound.Clients;
using TrakHound.Entities.Collections;
using TrakHound.Requests;

namespace TrakHound.Entities.Api
{
    [TrakHoundApiController("objects/hash")]
    public class Hashes : EntitiesApiControllerBase
    {
        public Hashes() { }

        public Hashes(ITrakHoundApiConfiguration configuration, ITrakHoundClient client)
        {
            SetConfiguration(configuration);
            SetClient(client);
        }


        [TrakHoundApiQuery]
        public async Task<TrakHoundApiResponse> GetHashes(
			[FromQuery] string objectPath,
			[FromBody(ContentType = "application/json")] IEnumerable<string> objectPaths,
			[FromQuery] long skip = 0,
            [FromQuery] long take = 1000,
            [FromQuery] string routerId = null
            )
        {
			if (!string.IsNullOrEmpty(objectPath) || !objectPaths.IsNullOrEmpty())
			{
                var paths = !string.IsNullOrEmpty(objectPath) ? new string[] { objectPath } : objectPaths;

                var entities = await Client.System.Entities.Objects.Hash.QueryByObject(paths, skip, take, routerId: routerId);
                if (!entities.IsNullOrEmpty())
                {
                    var objectUuids = entities.Select(o => o.ObjectUuid).Distinct();
                    var objects = await Client.System.Entities.Objects.ReadByUuid(objectUuids, routerId);
                    var dObjects = objects?.ToDictionary(o => o.Uuid);

                    var results = new List<TrakHoundHash>();
                    foreach (var entity in entities)
                    {
                        var obj = dObjects?.GetValueOrDefault(entity.ObjectUuid);
                        if (obj != null)
                        {
                            var result = new TrakHoundHash();
                            result.Uuid = entity.Uuid;
                            result.Object = obj.GetAbsolutePath();
                            result.ObjectUuid = entity.ObjectUuid;
                            result.Key = entity.Key;
                            result.Value = entity.Value;
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

        [TrakHoundApiQuery("values")]
        public async Task<TrakHoundApiResponse> GetHashValues(
			[FromQuery] string objectPath,
			[FromBody(ContentType = "application/json")] IEnumerable<string> objectPaths,
			[FromQuery] long skip = 0,
            [FromQuery] long take = 1000,
            [FromQuery] string routerId = null
            )
        {
			if (!string.IsNullOrEmpty(objectPath) || !objectPaths.IsNullOrEmpty())
			{
                var paths = !string.IsNullOrEmpty(objectPath) ? new string[] { objectPath } : objectPaths;

                var entities = await Client.System.Entities.Objects.Hash.QueryByObject(paths, skip, take, routerId: routerId);
                if (!entities.IsNullOrEmpty())
                {
                    var dEntities = entities.ToListDictionary(o => o.ObjectUuid);

                    var objectUuids = entities.Select(o => o.ObjectUuid).Distinct();
                    var objects = await Client.System.Entities.Objects.ReadByUuid(objectUuids, routerId);
                    var dObjects = objects?.ToDictionary(o => o.Uuid);

                    var results = new Dictionary<string, Dictionary<string, string>>();

                    foreach (var objectUuid in objectUuids)
                    {
                        var entitiesByObject = dEntities.Get(objectUuid);
                        if (!entitiesByObject.IsNullOrEmpty())
                        {
                            var obj = dObjects?.GetValueOrDefault(objectUuid);
                            if (obj != null)
                            {
                                results.Add(obj.GetAbsolutePath(), entitiesByObject.ToDictionary(o => o.Key, o => o.Value));
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
        public async Task<ITrakHoundConsumer<TrakHoundApiResponse>> SubscribeHashes(
            [FromQuery] string objectPath,
            [FromQuery] int interval = _defaultSubscribeInterval,
            [FromQuery] int take = _defaultSubscribeTake,
            [FromQuery] string routerId = null
            )
        {
            if (!string.IsNullOrEmpty(objectPath))
            {
                var consumer = await Client.System.Entities.Objects.Hash.SubscribeByObject(objectPath, interval, take, routerId: routerId);
                if (consumer != null)
                {
                    var resultConsumer = new TrakHoundConsumer<IEnumerable<ITrakHoundObjectHashEntity>, TrakHoundApiResponse>(consumer);
                    resultConsumer.OnReceivedAsync = async (entities) =>
                    {
                        var objectUuids = entities.Select(o => o.ObjectUuid).Distinct();
                        var objects = await Client.System.Entities.Objects.ReadByUuid(objectUuids, routerId);
                        var dObjects = objects?.ToDictionary(o => o.Uuid);

                        var results = new List<TrakHoundHash>();
                        foreach (var entity in entities)
                        {
                            var obj = dObjects?.GetValueOrDefault(entity.ObjectUuid);
                            if (obj != null)
                            {
                                var result = new TrakHoundHash();
                                result.Uuid = entity.Uuid;
                                result.Object = obj.GetAbsolutePath();
                                result.ObjectUuid = entity.ObjectUuid;
                                result.Key = entity.Key;
                                result.Value = entity.Value;
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
        public async Task<TrakHoundApiResponse> PublishHash(
            [FromQuery] string objectPath,
            [FromQuery] string key,
            [FromQuery] string value,
            [FromBody("application/json")] Dictionary<string, string> values,
            [FromQuery] bool async = false,
            [FromQuery] string routerId = null
            )
        {
            var response = new TrakHoundApiResponse();

            if (!string.IsNullOrEmpty(objectPath) && (!values.IsNullOrEmpty() || (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))))
            {
                var entry = new TrakHoundHashEntry();
                entry.ObjectPath = objectPath;

                if (!values.IsNullOrEmpty())
                {
                    entry.Values = values;
                }
                else
                {
                    var queryValues = new Dictionary<string, string>();
                    queryValues.Add(key, value);
                    entry.Values = queryValues;
                }

                var entries = new TrakHoundHashEntry[] { entry };

                return await PublishHashes(entries, async, routerId);
            }
            else
            {
                response.StatusCode = 400;
            }

            return response;
        }

        [TrakHoundApiPublish("batch")]
        public async Task<TrakHoundApiResponse> PublishHashes(
            [FromBody("application/json")] IEnumerable<TrakHoundHashEntry> entries,
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

        public void CreateEntities(TrakHoundEntityCollection collection, IEnumerable<TrakHoundHashEntry> entries, long created = 0)
        {
            if (!entries.IsNullOrEmpty())
            {
                var ts = created > 0 ? created : UnixDateTime.Now;

                // Create Objects that are referenced by Path in entries
                CreateContentObjects(collection, entries, ts);

                foreach (var entry in entries)
                {
                    string objectUuid = TrakHoundPath.GetUuid(entry.ObjectPath);
                    if (!string.IsNullOrEmpty(objectUuid) && !entry.Values.IsNullOrEmpty())
                    {
                        foreach (var value in entry.Values)
                        {
                            // Add Entity to the publish list
                            collection.Add(new TrakHoundObjectHashEntity(objectUuid, value.Key, value.Value, created: ts));
                        }
                    }
                }
            }
        }

        [TrakHoundApiDelete]
        public async Task<TrakHoundApiResponse> DeleteHash(
            [FromQuery] string objectPath,
            [FromQuery] string key,
            [FromQuery] bool async = false,
            [FromQuery] string routerId = null
            )
        {
            if (!string.IsNullOrEmpty(objectPath) && !string.IsNullOrEmpty(key))
            {
                string objectUuid = TrakHoundPath.GetUuid(objectPath);
                var uuid = TrakHoundObjectHashEntity.GenerateUuid(objectUuid, key);

                var success = await Client.System.Entities.Objects.Hash.Delete(uuid, async ? TrakHoundOperationMode.Async : TrakHoundOperationMode.Sync, routerId);
                if (success)
                {
                    return Ok();
                }
                else
                {
                    return InternalError();
                }
            }
            else if (!string.IsNullOrEmpty(objectPath))
            {
                string objectUuid = TrakHoundPath.GetUuid(objectPath);

                var success = await Client.System.Entities.Objects.Hash.DeleteByObjectUuid(objectUuid, routerId);
                if (success)
                {
                    return Ok();
                }
                else
                {
                    return InternalError();
                }
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
