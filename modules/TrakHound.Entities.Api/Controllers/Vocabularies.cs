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
    [TrakHoundApiController("objects/vocabulary")]
    public class Vocabularies : EntitiesApiControllerBase
    {
        public Vocabularies() { }

        public Vocabularies(ITrakHoundApiConfiguration configuration, ITrakHoundClient client)
        {
            SetConfiguration(configuration);
            SetClient(client);
        }


        [TrakHoundApiQuery]
        public async Task<TrakHoundApiResponse> GetVocabularies(
            [FromQuery] string objectPath,
            [FromBody(ContentType = "application/json")] IEnumerable<string> objectPaths,
            [FromQuery] string routerId = null
            )
        {
            if (!string.IsNullOrEmpty(objectPath) || !objectPaths.IsNullOrEmpty())
            {
                var paths = !string.IsNullOrEmpty(objectPath) ? new string[] { objectPath } : objectPaths;

                var entities = await Client.System.Entities.Objects.Vocabulary.QueryByObject(paths, routerId);
                if (!entities.IsNullOrEmpty())
                {
                    var objectUuids = entities.Select(o => o.ObjectUuid).Distinct();
                    var objects = await Client.System.Entities.Objects.ReadByUuid(objectUuids, routerId);
                    var dObjects = objects?.ToDictionary(o => o.Uuid);

                    var results = new List<TrakHoundVocabulary>();
                    foreach (var entity in entities)
                    {
                        var obj = dObjects?.GetValueOrDefault(entity.ObjectUuid);
                        if (obj != null)
                        {
                            var result = new TrakHoundVocabulary();
                            result.Uuid = entity.Uuid;
                            result.Object = obj.GetAbsolutePath();
                            result.ObjectUuid = entity.ObjectUuid;
                            result.Value = entity.DefinitionUuid;
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
        public async Task<TrakHoundApiResponse> GetVocabularyValues(
            [FromQuery] string objectPath,
            [FromBody(ContentType = "application/json")] IEnumerable<string> objectPaths,
            [FromQuery] string routerId = null
            )
        {
            if (!string.IsNullOrEmpty(objectPath) || !objectPaths.IsNullOrEmpty())
            {
                var paths = !string.IsNullOrEmpty(objectPath) ? new string[] { objectPath } : objectPaths;

                var entities = await Client.System.Entities.Objects.Vocabulary.QueryByObject(paths, routerId);
                if (!entities.IsNullOrEmpty())
                {
                    var objectUuids = entities.Select(o => o.ObjectUuid).Distinct();
                    var objects = await Client.System.Entities.Objects.ReadByUuid(objectUuids, routerId);
                    var dObjects = objects?.ToDictionary(o => o.Uuid);

                    var dictionary = new Dictionary<string, string>();

                    foreach (var entity in entities)
                    {
                        var obj = dObjects?.GetValueOrDefault(entity.ObjectUuid);
                        if (obj != null)
                        {
                            var key = obj.GetAbsolutePath();

                            if (!dictionary.ContainsKey(key))
                            {
                                dictionary[key] = entity.DefinitionUuid;
                            }
                        }
                    }

                    return Ok(dictionary);
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
        public async Task<TrakHoundApiResponse> PublishVocabulary(
            [FromQuery] string objectPath,
            [FromQuery] string definitionId,
            [FromQuery] bool async = false,
            [FromQuery] string routerId = null
            )
        {
            var response = new TrakHoundApiResponse();

            if (!string.IsNullOrEmpty(objectPath) && !string.IsNullOrEmpty(definitionId))
            {
                var entry = new TrakHoundVocabularyEntry();
                entry.ObjectPath = objectPath;
                entry.DefinitionId = definitionId;

                var entries = new TrakHoundVocabularyEntry[] { entry };

                return await PublishVocabularies(entries, async, routerId);
            }
            else
            {
                response.StatusCode = 400;
            }

            return response;
        }

        [TrakHoundApiPublish("batch")]
        public async Task<TrakHoundApiResponse> PublishVocabularies(
            [FromBody("application/json")] IEnumerable<TrakHoundVocabularyEntry> entries,
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

        public void CreateEntities(TrakHoundEntityCollection collection, IEnumerable<TrakHoundVocabularyEntry> entries, long created = 0)
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
                        // Add Definition
                        var definitionEntity = new TrakHoundDefinitionEntity(entry.DefinitionId, created: ts);
                        collection.Add(definitionEntity);

                        // Add Vocabulary
                        collection.Add(new TrakHoundObjectVocabularyEntity(objectUuid, definitionEntity.Uuid, created: ts));
                    }
                }
            }
        }

        [TrakHoundApiDelete]
        public async Task<TrakHoundApiResponse> DeleteVocabulary(
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
                    var entities = await Client.System.Entities.Objects.Vocabulary.QueryByObjectUuid(objectUuids, routerId);
                    if (!entities.IsNullOrEmpty())
                    {
                        var uuids = entities.Select(o => o.Uuid).Distinct();
                        var success = await Client.System.Entities.Objects.Vocabulary.Delete(uuids, async ? TrakHoundOperationMode.Async : TrakHoundOperationMode.Sync, routerId);
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
