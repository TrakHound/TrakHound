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
    [TrakHoundApiController("objects/reference")]
    public class References : EntitiesApiControllerBase
    {
        public References() { }

        public References(ITrakHoundApiConfiguration configuration, ITrakHoundClient client)
        {
            SetConfiguration(configuration);
            SetClient(client);
        }


        [TrakHoundApiQuery]
        public async Task<TrakHoundApiResponse> GetReferences(
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

                var entities = await Client.System.Entities.Objects.Reference.QueryByObject(paths, routerId);
                if (!entities.IsNullOrEmpty())
                {
                    var objectUuids = new List<string>();
                    objectUuids.AddRange(entities.Select(o => o.ObjectUuid));
                    objectUuids.AddRange(entities.Select(o => o.TargetUuid));

                    var objects = await Client.System.Entities.Objects.ReadByUuid(objectUuids.Distinct(), routerId);
                    var dObjects = objects?.ToDictionary(o => o.Uuid);

                    var results = new List<TrakHoundReference>();
                    foreach (var entity in entities)
                    {
                        var obj = dObjects?.GetValueOrDefault(entity.ObjectUuid);
                        var target = dObjects?.GetValueOrDefault(entity.TargetUuid);

                        if (obj != null && target != null)
                        {
                            var result = new TrakHoundReference();
                            result.Uuid = entity.Uuid;
                            result.Object = obj.GetAbsolutePath();
                            result.ObjectUuid = entity.ObjectUuid;
                            result.Target = target.GetAbsolutePath();
                            result.TargetUuid = entity.TargetUuid;
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

        [TrakHoundApiPublish]
        public async Task<TrakHoundApiResponse> PublishReference(
            [FromQuery] string objectPath,
            [FromQuery] string targetPath,
            [FromQuery] bool async = false,
            [FromQuery] string routerId = null
            )
        {
            var response = new TrakHoundApiResponse();

            if (!string.IsNullOrEmpty(objectPath) && !string.IsNullOrEmpty(targetPath))
            {
                var entry = new TrakHoundReferenceEntry();
                entry.ObjectPath = objectPath;
                entry.TargetPath = targetPath;

                var entries = new TrakHoundReferenceEntry[] { entry };

                return await PublishReferences(entries, async, routerId);
            }
            else
            {
                response.StatusCode = 400;
            }

            return response;
        }

        [TrakHoundApiPublish("batch")]
        public async Task<TrakHoundApiResponse> PublishReferences(
            [FromBody("application/json")] IEnumerable<TrakHoundReferenceEntry> entries,
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

        public void CreateEntities(TrakHoundEntityCollection collection, IEnumerable<TrakHoundReferenceEntry> entries, long created = 0)
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
                        // Add Entity to the publish list
                        collection.Add(new TrakHoundObjectReferenceEntity(objectUuid, targetUuid, created: ts));
                    }
                }
            }
        }
    }
}
