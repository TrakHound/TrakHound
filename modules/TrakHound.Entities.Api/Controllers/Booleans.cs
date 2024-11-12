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
    [TrakHoundApiController("objects/boolean")]
    public class Booleans : EntitiesApiControllerBase
    {
        public Booleans() { }

        public Booleans(ITrakHoundApiConfiguration configuration, ITrakHoundClient client)
        {
            SetConfiguration(configuration);
            SetClient(client);
        }


        [TrakHoundApiQuery]
        public async Task<TrakHoundApiResponse> GetBooleans(
            [FromQuery] string objectPath,
            [FromBody(ContentType = "application/json")] IEnumerable<string> objectPaths,
            [FromQuery] string routerId = null
            )
        {
            if (!string.IsNullOrEmpty(objectPath) || !objectPaths.IsNullOrEmpty())
            {
                var paths = !string.IsNullOrEmpty(objectPath) ? new string[] { objectPath } : objectPaths;

                var entities = await Client.System.Entities.Objects.Boolean.QueryByObject(paths, routerId);
                if (!entities.IsNullOrEmpty())
                {
                    var objectUuids = entities.Select(o => o.ObjectUuid).Distinct();
                    var objects = await Client.System.Entities.Objects.ReadByUuid(objectUuids, routerId);
                    var dObjects = objects?.ToDictionary(o => o.Uuid);

                    var results = new List<TrakHoundBoolean>();
                    foreach (var entity in entities)
                    {
                        var obj = dObjects?.GetValueOrDefault(entity.ObjectUuid);
                        if (obj != null)
                        {
                            var result = new TrakHoundBoolean();
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
        public async Task<TrakHoundApiResponse> GetBooleanValues(
            [FromQuery] string objectPath,
            [FromBody(ContentType = "application/json")] IEnumerable<string> objectPaths,
            [FromQuery] string routerId = null
            )
        {
            if (!string.IsNullOrEmpty(objectPath) || !objectPaths.IsNullOrEmpty())
            {
                var paths = !string.IsNullOrEmpty(objectPath) ? new string[] { objectPath } : objectPaths;

                var entities = await Client.System.Entities.Objects.Boolean.QueryByObject(paths, routerId);
                if (!entities.IsNullOrEmpty())
                {
                    var objectUuids = entities.Select(o => o.ObjectUuid).Distinct();
                    var objects = await Client.System.Entities.Objects.ReadByUuid(objectUuids, routerId);
                    var dObjects = objects?.ToDictionary(o => o.Uuid);

                    var values = new Dictionary<string, bool>();
                    foreach (var entity in entities)
                    {
                        var obj = dObjects?.GetValueOrDefault(entity.ObjectUuid);
                        if (obj != null)
                        {
                            var key = obj.GetAbsolutePath();
                            if (!values.ContainsKey(key)) values[key] = entity.Value;
                        }
                    }
                    return Ok(values);
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


        [TrakHoundApiPublish]
        public async Task<TrakHoundApiResponse> PublishBoolean(
            [FromQuery] string objectPath,
            [FromQuery] string value,
            [FromQuery] bool async = false,
            [FromQuery] string routerId = null
            )
        {
            var response = new TrakHoundApiResponse();

            if (!string.IsNullOrEmpty(objectPath) && !string.IsNullOrEmpty(value))
            {
                var entry = new TrakHoundBooleanEntry();
                entry.ObjectPath = objectPath;
                entry.Value = value.ToBoolean();

                var entries = new TrakHoundBooleanEntry[] { entry };

                return await PublishBooleans(entries, async, routerId);
            }
            else
            {
                response.StatusCode = 400;
            }

            return response;
        }

        [TrakHoundApiPublish("batch")]
        public async Task<TrakHoundApiResponse> PublishBooleans(
            [FromBody("application/json")] IEnumerable<TrakHoundBooleanEntry> entries,
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

        public void CreateEntities(TrakHoundEntityCollection collection, IEnumerable<TrakHoundBooleanEntry> entries, long created = 0)
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
                        collection.Add(new TrakHoundObjectBooleanEntity(objectUuid, entry.Value, created: ts));
                    }
                }
            }
        }

        [TrakHoundApiDelete]
        public async Task<TrakHoundApiResponse> DeleteBoolean(
            [FromQuery] string objectPath,
            [FromQuery] bool async = false,
            [FromQuery] string routerId = null
            )
        {
            if (!string.IsNullOrEmpty(objectPath))
            {
                var objs = await Client.System.Entities.Objects.Query(objectPath, routerId: routerId);
                if (!objs.IsNullOrEmpty())
                {
                    var objectUuids = objs.Select(o => o.Uuid).Distinct();
                    var entities = await Client.System.Entities.Objects.Boolean.QueryByObjectUuid(objectUuids, routerId);
                    if (!entities.IsNullOrEmpty())
                    {
                        var uuids = entities.Select(o => o.Uuid).Distinct();
                        var success = await Client.System.Entities.Objects.Boolean.Delete(uuids, async ? TrakHoundOperationMode.Async : TrakHoundOperationMode.Sync, routerId);
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
    }
}
