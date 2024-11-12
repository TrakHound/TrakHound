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
    [TrakHoundApiController("objects/vocabulary-set")]
    public class VocabularySets : EntitiesApiControllerBase
    {
        public VocabularySets() { }

        public VocabularySets(ITrakHoundApiConfiguration configuration, ITrakHoundClient client)
        {
            SetConfiguration(configuration);
            SetClient(client);
        }


        [TrakHoundApiQuery]
        public async Task<TrakHoundApiResponse> GetVocabularySets(
            [FromQuery] string objectPath,
            [FromQuery] long skip = 0,
            [FromQuery] long take = 1000,
            [FromQuery] string routerId = null
            )
        {
            var response = new TrakHoundApiResponse();

            if (!string.IsNullOrEmpty(objectPath))
            {
                var entities = await Client.System.Entities.Objects.VocabularySet.QueryByObject(objectPath, skip, take, routerId: routerId);
                if (!entities.IsNullOrEmpty())
                {
                    var objectUuids = entities.Select(o => o.ObjectUuid).Distinct();
                    var objects = await Client.System.Entities.Objects.ReadByUuid(objectUuids, routerId);
                    var dObjects = objects?.ToDictionary(o => o.Uuid);

                    var results = new List<TrakHoundVocabularySet>();
                    foreach (var entity in entities)
                    {
                        var obj = dObjects?.GetValueOrDefault(entity.ObjectUuid);
                        if (obj != null)
                        {
                            var result = new TrakHoundVocabularySet();
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
                    response.StatusCode = 404;
                }
            }
            else
            {
                response.StatusCode = 400;
            }

            return response;
        }

        [TrakHoundApiPublish]
        public async Task<TrakHoundApiResponse> PublishVocabularySet(
            [FromQuery] string objectPath,
            [FromQuery] string definitionId,
            [FromBody("application/json")] IEnumerable<string> definitionIds,
            [FromQuery] bool async = false,
            [FromQuery] string routerId = null
            )
        {
            var response = new TrakHoundApiResponse();

            if (!string.IsNullOrEmpty(objectPath) && (!string.IsNullOrEmpty(definitionId) || !definitionIds.IsNullOrEmpty()))
            {
                var entry = new TrakHoundVocabularySetEntry();
                entry.ObjectPath = objectPath;

                if (!string.IsNullOrEmpty(definitionId))
                {
                    entry.DefinitionIds = new string[] { definitionId };
                }
                else
                {
                    entry.DefinitionIds = definitionIds;
                }

                var entries = new TrakHoundVocabularySetEntry[] { entry };

                return await PublishVocabularySets(entries, async, routerId);
            }
            else
            {
                response.StatusCode = 400;
            }

            return response;
        }

        [TrakHoundApiPublish("batch")]
        public async Task<TrakHoundApiResponse> PublishVocabularySets(
            [FromBody("application/json")] IEnumerable<TrakHoundVocabularySetEntry> entries,
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

        public void CreateEntities(TrakHoundEntityCollection collection, IEnumerable<TrakHoundVocabularySetEntry> entries, long created = 0)
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
                        foreach (var definitionId in entry.DefinitionIds)
                        {
                            // Add Definition
                            var definitionEntity = new TrakHoundDefinitionEntity(definitionId, created: ts);
                            collection.Add(definitionEntity);

                            // Add VocabularySet
                            collection.Add(new TrakHoundObjectVocabularySetEntity(objectUuid, definitionEntity.Uuid, created: ts));
                        }
                    }
                }
            }
        }
    }
}
