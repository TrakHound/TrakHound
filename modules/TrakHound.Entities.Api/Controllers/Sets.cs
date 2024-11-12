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
    [TrakHoundApiController("objects/set")]
    public class Sets : EntitiesApiControllerBase
    {
        public Sets() { }

        public Sets(ITrakHoundApiConfiguration configuration, ITrakHoundClient client)
        {
            SetConfiguration(configuration);
            SetClient(client);
        }


        [TrakHoundApiQuery]
        public async Task<TrakHoundApiResponse> GetSets(
            [FromQuery] string objectPath,
            [FromBody(ContentType = "application/json")] IEnumerable<string> objectPaths,
            [FromQuery] long skip = 0,
            [FromQuery] long take = 1000,
            [FromQuery] int sortOrder = (int)SortOrder.Ascending,
            [FromQuery] string routerId = null
            )
        {
            if (!string.IsNullOrEmpty(objectPath) || !objectPaths.IsNullOrEmpty())
            {
                var paths = !string.IsNullOrEmpty(objectPath) ? new string[] { objectPath } : objectPaths;

                var entities = await Client.System.Entities.Objects.Set.QueryByObject(paths, skip, take, (SortOrder)sortOrder, routerId);
                if (!entities.IsNullOrEmpty())
                {
                    var objectUuids = entities.Select(o => o.ObjectUuid).Distinct();
                    var objects = await Client.System.Entities.Objects.ReadByUuid(objectUuids, routerId);
                    var dObjects = objects?.ToDictionary(o => o.Uuid);

                    var results = new List<TrakHoundSet>();
                    foreach (var entity in entities)
                    {
                        var obj = dObjects?.GetValueOrDefault(entity.ObjectUuid);
                        if (obj != null)
                        {
                            var result = new TrakHoundSet();
                            result.Uuid = entity.Uuid;
                            result.Object = obj.GetAbsolutePath();
                            result.ObjectUuid = entity.ObjectUuid;
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
        public async Task<TrakHoundApiResponse> GetSetValues(
            [FromQuery] string objectPath,
            [FromBody(ContentType = "application/json")] IEnumerable<string> objectPaths,
            [FromQuery] long skip = 0,
            [FromQuery] long take = 1000,
            [FromQuery] int sortOrder = (int)SortOrder.Ascending,
            [FromQuery] string routerId = null
            )
        {
            if (!string.IsNullOrEmpty(objectPath) || !objectPaths.IsNullOrEmpty())
            {
                var paths = !string.IsNullOrEmpty(objectPath) ? new string[] { objectPath } : objectPaths;

                var entities = await Client.System.Entities.Objects.Set.QueryByObject(paths, skip, take, (SortOrder)sortOrder, routerId);
                if (!entities.IsNullOrEmpty())
                {
                    var dEntities = entities.ToListDictionary(o => o.ObjectUuid);

                    var objectUuids = entities.Select(o => o.ObjectUuid).Distinct();
                    var objects = await Client.System.Entities.Objects.ReadByUuid(objectUuids, routerId);
                    var dObjects = objects?.ToDictionary(o => o.Uuid);

                    var results = new Dictionary<string, IEnumerable<string>>();

                    foreach (var objectUuid in objectUuids)
                    {
                        var entitiesByObject = dEntities.Get(objectUuid);
                        if (!entitiesByObject.IsNullOrEmpty())
                        {
                            var obj = dObjects?.GetValueOrDefault(objectUuid);
                            if (obj != null)
                            {
                                results.Add(obj.GetAbsolutePath(), entitiesByObject.Select(o => o.Value));
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
        public async Task<ITrakHoundConsumer<TrakHoundApiResponse>> SubscribeSets(
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

                var consumer = await Client.System.Entities.Objects.Set.SubscribeByObject(paths, interval, take, routerId: routerId);
                if (consumer != null)
                {
                    var resultConsumer = new TrakHoundConsumer<IEnumerable<ITrakHoundObjectSetEntity>, TrakHoundApiResponse>(consumer);
                    resultConsumer.OnReceivedAsync = async (entities) =>
                    {
                        var objectUuids = entities.Select(o => o.ObjectUuid).Distinct();
                        var objects = await Client.System.Entities.Objects.ReadByUuid(objectUuids, routerId);
                        var dObjects = objects?.ToDictionary(o => o.Uuid);

                        var results = new List<TrakHoundSet>();
                        foreach (var entity in entities)
                        {
                            var obj = dObjects?.GetValueOrDefault(entity.ObjectUuid);
                            if (obj != null)
                            {
                                var result = new TrakHoundSet();
                                result.Uuid = entity.Uuid;
                                result.Object = obj.GetAbsolutePath();
                                result.ObjectUuid = entity.ObjectUuid;
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
        public async Task<TrakHoundApiResponse> PublishSet(
            [FromQuery] string objectPath,
            [FromQuery] string value,
            [FromBody("application/json")] IEnumerable<string> values,
            [FromQuery] bool async = false,
            [FromQuery] string routerId = null
            )
        {
            var response = new TrakHoundApiResponse();

            if (!string.IsNullOrEmpty(objectPath) && (!string.IsNullOrEmpty(value) || !values.IsNullOrEmpty()))
            {
                var entry = new TrakHoundSetEntry();
                entry.ObjectPath = objectPath;
                entry.Values = !values.IsNullOrEmpty() ? values : new string[] { value };

                var entries = new TrakHoundSetEntry[] { entry };

                return await PublishSets(entries, async, routerId);
            }
            else
            {
                response.StatusCode = 400;
            }

            return response;
        }

        [TrakHoundApiPublish("batch")]
        public async Task<TrakHoundApiResponse> PublishSets(
            [FromBody("application/json")] IEnumerable<TrakHoundSetEntry> entries,
            [FromQuery] bool async = false,
            [FromQuery] string routerId = null
            )
        {
            var response = new TrakHoundApiResponse();

            if (!entries.IsNullOrEmpty())
            {
                var now = UnixDateTime.Now;
                var publishCollection = new TrakHoundEntityCollection();

                await CreateEntities(publishCollection, entries, now, routerId);

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

        public async Task CreateEntities(TrakHoundEntityCollection collection, IEnumerable<TrakHoundSetEntry> entries, long created = 0, string routerId = null)
        {
            if (!entries.IsNullOrEmpty())
            {
                var ts = created > 0 ? created : UnixDateTime.Now;
                var deleteObjectUuids = new List<string>();

                // Create Objects that are referenced by Path in entries
                CreateContentObjects(collection, entries, ts);

                foreach (var entry in entries)
                {
                    string objectUuid = TrakHoundPath.GetUuid(entry.ObjectPath);
                    if (!string.IsNullOrEmpty(objectUuid))
                    {
                        // If Absolute, delete previous entities first
                        if (entry.EntryType == Serialization.TrakHoundEntryType.Absolute)
                        {
                            deleteObjectUuids.Add(objectUuid);
                        }

                        // Create List of New Entities to Publish
                        if (!entry.Values.IsNullOrEmpty())
                        {
                            foreach (var value in entry.Values)
                            {
                                collection.Add(new TrakHoundObjectSetEntity(objectUuid, value, created: ts));
                            }
                        }
                    }
                }

                if (!deleteObjectUuids.IsNullOrEmpty())
                {
                    // Get list of existing entities
                    var existingEntities = await Client.System.Entities.Objects.Set.QueryByObjectUuid(deleteObjectUuids, routerId: routerId);
                    if (!existingEntities.IsNullOrEmpty())
                    {
                        var deleteUuids = existingEntities.Select(o => o.Uuid).Distinct();

                        // Delete the existing entities
                        await Client.System.Entities.Objects.Set.Delete(deleteUuids, TrakHoundOperationMode.Sync, routerId);
                    }
                }
            }
        }

        [TrakHoundApiDelete]
        public async Task<TrakHoundApiResponse> DeleteSet(
            [FromQuery] string objectPath,
            [FromQuery] string value,
            [FromQuery] bool async = false,
            [FromQuery] string routerId = null
            )
        {
            if (!string.IsNullOrEmpty(objectPath) && !string.IsNullOrEmpty(value))
            {
                var objects = await Client.System.Entities.Objects.Query(objectPath, routerId: routerId);
                if (!objects.IsNullOrEmpty())
                {
                    var uuids = new List<string>();
                    foreach (var obj in objects)
                    {
                        uuids.Add(TrakHoundObjectSetEntity.GenerateUuid(obj.Uuid, value));
                    }

                    var success = await Client.System.Entities.Objects.Set.Delete(uuids, async ? TrakHoundOperationMode.Async : TrakHoundOperationMode.Sync, routerId);
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
                    return NotFound();
                }
            }
            else if (!string.IsNullOrEmpty(objectPath))
            {
                string objectUuid = TrakHoundPath.GetUuid(objectPath);

                var success = await Client.System.Entities.Objects.Set.DeleteByObjectUuid(objectUuid, routerId);
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
