// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Api;
using TrakHound.Clients;
using TrakHound.Entities.Collections;
using TrakHound.Requests;

namespace TrakHound.Entities.Api
{
    [TrakHoundApiController("sources")]
    public class Sources : EntitiesApiControllerBase
    {
        public Sources() { }

        public Sources(ITrakHoundApiConfiguration configuration, ITrakHoundClient client)
        {
            SetConfiguration(configuration);
            SetClient(client);
        }


        [TrakHoundApiQuery("{uuid}")]
        public async Task<TrakHoundApiResponse> GetSource(
            [FromRoute] string uuid,
            [FromQuery] string routerId = null
            )
        {
            if (!string.IsNullOrEmpty(uuid))
            {
                var entity = await Client.System.Entities.Sources.ReadByUuid(uuid, routerId);
                if (entity != null)
                {
                    var result = new TrakHoundSource();
                    result.Uuid = entity.Uuid;
                    result.Type = entity.Type;
                    result.Sender = entity.Sender;
                    result.Created = entity.Created.ToDateTime();

                    return Ok(result);
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

        [TrakHoundApiQuery("{uuid}/chain")]
        public async Task<TrakHoundApiResponse> GetSourceChain(
            [FromRoute] string uuid,
            [FromQuery] string routerId = null
            )
        {
            if (!string.IsNullOrEmpty(uuid))
            {
                var entities = await Client.System.Entities.Sources.QueryChain(uuid, routerId);
                if (!entities.IsNullOrEmpty())
                {
                    var results = new List<TrakHoundSource>();
                    foreach (var entity in entities)
                    {
                        var result = new TrakHoundSource();
                        result.Uuid = entity.Uuid;
                        result.Type = entity.Type;
                        result.Sender = entity.Sender;
                        result.Created = entity.Created.ToDateTime();

                        results.Add(result);
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
        public async Task<TrakHoundApiResponse> PublishSource(
            [FromQuery] string type,
            [FromQuery] string sender,
            [FromQuery] string description = null,
            [FromQuery] bool async = false,
            [FromQuery] string routerId = null
            )
        {
            var response = new TrakHoundApiResponse();

            if (!string.IsNullOrEmpty(type) && !string.IsNullOrEmpty(sender))
            {
                var entry = new TrakHoundSourceEntry();
                entry.Type = type;
                entry.Sender = sender;

                var entries = new TrakHoundSourceEntry[] { entry };

                return await PublishSources(entries, async, routerId);
            }
            else
            {
                response.StatusCode = 400;
            }

            return response;
        }


        private IEnumerable<ITrakHoundSourceEntity> GetSourceEntities(TrakHoundSourceEntry entry, long created = 0)
        {
            var entities = new List<ITrakHoundSourceEntity>();

            if (entry != null && !string.IsNullOrEmpty(entry.Path))
            {
                var pathParts = entry.Path.Split('/');
                var rootParts = new List<string>();

                foreach (var pathPart in pathParts)
                {
                    var partPaths = new List<string>();
                    partPaths.AddRange(rootParts);
                    partPaths.Add(pathPart);

                    var partPath = string.Join('/', partPaths);

                    var name = TrakHoundPath.GetObject(partPath);
                    var description = partPath == entry.Path ? entry.Type : null;
                    //var parentUuid = TrakHoundSourcePath.GetUuid(TrakHoundPath.GetParentPath(partPath));

                    //var entity = new TrakHoundSourceEntity(name, description, parentUuid);
                    //if (entity.IsValid)
                    //{
                    //    entities.Add(entity);
                    //}

                    rootParts.Add(pathPart);
                }
            }

            return entities;
        }


        [TrakHoundApiPublish("batch")]
        public async Task<TrakHoundApiResponse> PublishSources(
            [FromBody("application/json")] IEnumerable<TrakHoundSourceEntry> entries,
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

        public void CreateEntities(TrakHoundEntityCollection collection, IEnumerable<TrakHoundSourceEntry> entries, long created = 0)
        {
            if (!entries.IsNullOrEmpty())
            {
                var ts = created > 0 ? created : UnixDateTime.Now;

                foreach (var entry in entries)
                {
                    collection.Add(GetSourceEntities(entry, ts));
                }
            }
        }


        [TrakHoundApiDelete]
        public async Task<TrakHoundApiResponse> DeleteSource(
            [FromQuery] string path,
            [FromQuery] bool async = false,
            [FromQuery] string routerId = null
            )
        {
            if (!string.IsNullOrEmpty(path))
            {
                //var uuid = TrakHoundSourcePath.GetUuid(path);
                //if (!string.IsNullOrEmpty(uuid))
                //{
                //    var success = await Client.System.Entities.Sources.Delete(uuid, async ? TrakHoundOperationMode.Async : TrakHoundOperationMode.Sync);
                //    if (success)
                //    {
                //        return Ok();
                //    }
                //    else
                //    {
                //        return InternalError();
                //    }
                //}
                //else
                //{
                //    return NotFound();
                //}

                return InternalError();
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
