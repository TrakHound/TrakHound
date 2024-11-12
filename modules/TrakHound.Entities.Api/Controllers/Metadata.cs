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
    [TrakHoundApiController("objects/metadata")]
    public class Metadata : EntitiesApiControllerBase
    {
        public Metadata() { }

        public Metadata(ITrakHoundApiConfiguration configuration, ITrakHoundClient client)
        {
            SetConfiguration(configuration);
            SetClient(client);
        }


        [TrakHoundApiQuery]
        public async Task<TrakHoundApiResponse> GetMetadata(
            [FromQuery] string entityUuid,
            [FromBody(ContentType = "application/json")] IEnumerable<string> entityUuids,
            [FromQuery] string routerId = null
            )
        {
            if (!string.IsNullOrEmpty(entityUuid) || !entityUuids.IsNullOrEmpty())
            {
                var uuids = !string.IsNullOrEmpty(entityUuid) ? new string[] { entityUuid } : entityUuids;

                var entities = await Client.System.Entities.Objects.Metadata.QueryByEntityUuid(uuids, routerId);
                if (!entities.IsNullOrEmpty())
                {
                    var results = new List<TrakHoundMetadata>();
                    foreach (var entity in entities)
                    {
                        var result = new TrakHoundMetadata();
                        result.Uuid = entity.Uuid;
                        result.EntityUuid = entity.EntityUuid;
                        result.Name = entity.Name;
                        result.Value = entity.Value;
                        result.SourceUuid = entity.SourceUuid;
                        result.Created = entity.Created.ToLocalDateTime();
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
        public async Task<TrakHoundApiResponse> PublishMetadata(
            [FromQuery] string entityUuid,
            [FromQuery] string name,
            [FromQuery] string value,
            [FromQuery] bool async = false,
            [FromQuery] string routerId = null
            )
        {
            var response = new TrakHoundApiResponse();

            if (!string.IsNullOrEmpty(entityUuid) && !string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(value))
            {
                var entry = new TrakHoundMetadataEntry();
                entry.EntityUuid = entityUuid;
                entry.Name = name;
                entry.Value = value;

                var entries = new TrakHoundMetadataEntry[] { entry };

                return await PublishMetadata(entries, async, routerId);
            }
            else
            {
                response.StatusCode = 400;
            }

            return response;
        }

        [TrakHoundApiPublish("batch")]
        public async Task<TrakHoundApiResponse> PublishMetadata(
            [FromBody("application/json")] IEnumerable<TrakHoundMetadataEntry> entries,
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

        public void CreateEntities(TrakHoundEntityCollection collection, IEnumerable<TrakHoundMetadataEntry> entries, long created = 0)
        {
            if (!entries.IsNullOrEmpty())
            {
                var ts = created > 0 ? created : UnixDateTime.Now;

                foreach (var entry in entries)
                {
                    if (!string.IsNullOrEmpty(entry.EntityUuid))
                    {
                        var definitionUuid = !string.IsNullOrEmpty(entry.DefinitionId) ? TrakHoundDefinitionEntity.GenerateUuid(entry.DefinitionId) : null;
                        var valueDefinitionUuid = !string.IsNullOrEmpty(entry.ValueDefinitionId) ? TrakHoundDefinitionEntity.GenerateUuid(entry.ValueDefinitionId) : null;

                        collection.Add(new TrakHoundObjectMetadataEntity(entry.EntityUuid, entry.Name, entry.Value, definitionUuid, valueDefinitionUuid, created: ts));
                    }
                }
            }
        }

        [TrakHoundApiDelete]
        public async Task<TrakHoundApiResponse> DeleteMetadata(
            [FromQuery] string entityUuid,
            [FromQuery] string name,
            [FromQuery] bool async = false,
            [FromQuery] string routerId = null
            )
        {
            if (!string.IsNullOrEmpty(entityUuid))
            {
                var operationMode = async ? TrakHoundOperationMode.Async : TrakHoundOperationMode.Sync;

                if (!string.IsNullOrEmpty(name))
                {
                    var uuid = TrakHoundObjectMetadataEntity.GenerateUuid(entityUuid, name);
                    if (await Client.System.Entities.Objects.Metadata.Delete(uuid, operationMode, routerId))
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
                    if (await Client.System.Entities.Objects.Metadata.DeleteByObjectUuid(entityUuid, routerId))
                    {
                        return Ok();
                    }
                    else
                    {
                        return InternalError();
                    }
                }
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
