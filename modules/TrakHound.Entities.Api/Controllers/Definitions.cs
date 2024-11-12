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
    [TrakHoundApiController("definitions")]
    public class Definitions : EntitiesApiControllerBase
    {
        public Definitions() { }

        public Definitions(ITrakHoundApiConfiguration configuration, ITrakHoundClient client)
        {
            SetConfiguration(configuration);
            SetClient(client);
        }


        [TrakHoundApiQuery("query")]
        public async Task<TrakHoundApiResponse> GetDefinitions(
            [FromQuery] string pattern,
            [FromQuery] string routerId = null
            )
        {
            if (!string.IsNullOrEmpty(pattern))
            {
                var definitionEntities = await Client.System.Entities.Definitions.Query(pattern, routerId: routerId);
                if (!definitionEntities.IsNullOrEmpty())
                {
                    var foundUuids = definitionEntities.Select(o => o.Uuid);

                    var metadataEntities = await Client.System.Entities.Definitions.Metadata.QueryByDefinitionUuid(foundUuids, routerId);
                    var dMetadataEntities = metadataEntities?.ToListDictionary(o => o.DefinitionUuid);

                    var descriptionEntities = await Client.System.Entities.Definitions.Description.QueryByDefinitionUuid(foundUuids, routerId);
                    var dDescriptionEntities = descriptionEntities?.ToListDictionary(o => o.DefinitionUuid);

                    var results = new List<TrakHoundDefinition>();
                    foreach (var definitionEntity in definitionEntities)
                    {
                        var result = new TrakHoundDefinition();
                        result.Uuid = definitionEntity.Uuid;
                        result.Id = definitionEntity.Id;
                        result.Type = definitionEntity.Type;
                        result.ParentUuid = definitionEntity.ParentUuid;
                        result.SourceUuid = definitionEntity.SourceUuid;
                        result.Created = definitionEntity.Created.ToDateTime();

                        // Metadata
                        var matchMetadataEntities = dMetadataEntities?.Get(definitionEntity.Uuid);
                        if (!matchMetadataEntities.IsNullOrEmpty())
                        {
                            var metadataModels = new List<TrakHoundDefinitionMetadata>();
                            foreach (var metadata in matchMetadataEntities)
                            {
                                var metadataModel = new TrakHoundDefinitionMetadata();
                                metadataModel.Uuid = metadata.Uuid;
                                metadataModel.Name = metadata.Name;
                                metadataModel.Value = metadata.Value;
                                metadataModel.SourceUuid = metadata.SourceUuid;
                                metadataModel.Created = metadata.Created.ToDateTime();
                                metadataModels.Add(metadataModel);
                            }
                            result.Metadata = metadataModels;
                        }

                        // Descriptions
                        var matchDescriptionEntities = dDescriptionEntities?.Get(definitionEntity.Uuid);
                        if (!matchDescriptionEntities.IsNullOrEmpty())
                        {
                            var descriptionModels = new List<TrakHoundDefinitionDescription>();
                            foreach (var description in matchDescriptionEntities)
                            {
                                var descriptionModel = new TrakHoundDefinitionDescription();
                                descriptionModel.Uuid = description.Uuid;
                                descriptionModel.LanguageCode = description.LanguageCode;
                                descriptionModel.Text = description.Text;
                                descriptionModel.SourceUuid = description.SourceUuid;
                                descriptionModel.Created = description.Created.ToDateTime();
                                descriptionModels.Add(descriptionModel);
                            }
                            result.Descriptions = descriptionModels;
                        }

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

        [TrakHoundApiQuery]
        public async Task<TrakHoundApiResponse> GetDefinitions(
            [FromBody(ContentType = "application/json")] IEnumerable<string> definitionIds,
            [FromQuery] string routerId = null
            )
        {
            if (!definitionIds.IsNullOrEmpty())
            {
                var uuids = definitionIds.Select(o => TrakHoundDefinitionEntity.GenerateUuid(o));

                var definitionEntities = await Client.System.Entities.Definitions.ReadByUuid(uuids, routerId);
                if (!definitionEntities.IsNullOrEmpty())
                {
                    var foundUuids = definitionEntities.Select(o => o.Uuid);

                    var metadataEntities = await Client.System.Entities.Definitions.Metadata.QueryByDefinitionUuid(foundUuids, routerId);
                    var dMetadataEntities = metadataEntities?.ToListDictionary(o => o.DefinitionUuid);

                    var descriptionEntities = await Client.System.Entities.Definitions.Description.QueryByDefinitionUuid(foundUuids, routerId);
                    var dDescriptionEntities = descriptionEntities?.ToListDictionary(o => o.DefinitionUuid);

                    var results = new List<TrakHoundDefinition>();
                    foreach (var definitionEntity in definitionEntities)
                    {
                        var result = new TrakHoundDefinition();
                        result.Uuid = definitionEntity.Uuid;
                        result.Id = definitionEntity.Id;
                        result.Type = definitionEntity.Type;
                        result.ParentUuid = definitionEntity.ParentUuid;
                        result.SourceUuid = definitionEntity.SourceUuid;
                        result.Created = definitionEntity.Created.ToDateTime();

                        // Metadata
                        var matchMetadataEntities = dMetadataEntities?.Get(definitionEntity.Uuid);
                        if (!matchMetadataEntities.IsNullOrEmpty())
                        {
                            var metadataModels = new List<TrakHoundDefinitionMetadata>();
                            foreach (var metadata in matchMetadataEntities)
                            {
                                var metadataModel = new TrakHoundDefinitionMetadata();
                                metadataModel.Uuid = metadata.Uuid;
                                metadataModel.Name = metadata.Name;
                                metadataModel.Value = metadata.Value;
                                metadataModel.SourceUuid = metadata.SourceUuid;
                                metadataModel.Created = metadata.Created.ToDateTime();
                                metadataModels.Add(metadataModel);
                            }
                            result.Metadata = metadataModels;
                        }

                        // Descriptions
                        var matchDescriptionEntities = dDescriptionEntities?.Get(definitionEntity.Uuid);
                        if (!matchDescriptionEntities.IsNullOrEmpty())
                        {
                            var descriptionModels = new List<TrakHoundDefinitionDescription>();
                            foreach (var description in matchDescriptionEntities)
                            {
                                var descriptionModel = new TrakHoundDefinitionDescription();
                                descriptionModel.Uuid = description.Uuid;
                                descriptionModel.LanguageCode = description.LanguageCode;
                                descriptionModel.Text = description.Text;
                                descriptionModel.SourceUuid = description.SourceUuid;
                                descriptionModel.Created = description.Created.ToDateTime();
                                descriptionModels.Add(descriptionModel);
                            }
                            result.Descriptions = descriptionModels;
                        }

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

        [TrakHoundApiQuery("{definitionId}")]
        public async Task<TrakHoundApiResponse> GetDefinitionById(
            [FromRoute] string definitionId,
            [FromQuery] string routerId = null
            )
        {
            if (!string.IsNullOrEmpty(definitionId))
            {
                var uuid = TrakHoundDefinitionEntity.GenerateUuid(definitionId);

                var definitionEntity = await Client.System.Entities.Definitions.ReadByUuid(uuid, routerId);
                if (definitionEntity != null)
                {
                    var definitionModel = new TrakHoundDefinition();
                    definitionModel.Uuid = definitionEntity.Uuid;
                    definitionModel.Id = definitionEntity.Id;
                    definitionModel.Type = definitionEntity.Type;
                    definitionModel.ParentUuid = definitionEntity.ParentUuid;
                    definitionModel.SourceUuid = definitionEntity.SourceUuid;
                    definitionModel.Created = definitionEntity.Created.ToDateTime();

                    // Metadata
                    var metadataEntities = await Client.System.Entities.Definitions.Metadata.QueryByDefinitionUuid(uuid, routerId);
                    if (!metadataEntities.IsNullOrEmpty())
                    {
                        var metadataModels = new List<TrakHoundDefinitionMetadata>();
                        foreach (var metadataEntity in metadataEntities)
                        {
                            var metadataModel = new TrakHoundDefinitionMetadata();
                            metadataModel.Uuid = metadataEntity.Uuid;
                            metadataModel.Name = metadataEntity.Name;
                            metadataModel.Value = metadataEntity.Value;
                            metadataModel.SourceUuid = metadataEntity.SourceUuid;
                            metadataModel.Created = metadataEntity.Created.ToDateTime();
                            metadataModels.Add(metadataModel);
                        }
                        definitionModel.Metadata = metadataModels;
                    }

                    // Descriptions
                    var descriptionEntities = await Client.System.Entities.Definitions.Description.QueryByDefinitionUuid(uuid, routerId);
                    if (!descriptionEntities.IsNullOrEmpty())
                    {
                        var descriptionModels = new List<TrakHoundDefinitionDescription>();
                        foreach (var description in descriptionEntities)
                        {
                            var descriptionModel = new TrakHoundDefinitionDescription();
                            descriptionModel.Uuid = description.Uuid;
                            descriptionModel.LanguageCode = description.LanguageCode;
                            descriptionModel.Text = description.Text;
                            descriptionModel.SourceUuid = description.SourceUuid;
                            descriptionModel.Created = description.Created.ToDateTime();
                            descriptionModels.Add(descriptionModel);
                        }
                        definitionModel.Descriptions = descriptionModels;
                    }

                    return Ok(definitionModel);
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

        [TrakHoundApiQuery("uuid/{uuid}")]
        public async Task<TrakHoundApiResponse> GetDefinitionByUuid(
            [FromRoute] string uuid,
            [FromQuery] string routerId = null
            )
        {
            if (!string.IsNullOrEmpty(uuid))
            {
                var definitionEntity = await Client.System.Entities.Definitions.ReadByUuid(uuid, routerId);
                if (definitionEntity != null)
                {
                    var definitionModel = new TrakHoundDefinition();
                    definitionModel.Uuid = definitionEntity.Uuid;
                    definitionModel.Id = definitionEntity.Id;
                    definitionModel.Type = definitionEntity.Type;
                    definitionModel.ParentUuid = definitionEntity.ParentUuid;
                    definitionModel.SourceUuid = definitionEntity.SourceUuid;
                    definitionModel.Created = definitionEntity.Created.ToDateTime();

                    // Metadata
                    var metadataEntities = await Client.System.Entities.Definitions.Metadata.QueryByDefinitionUuid(uuid, routerId);
                    if (!metadataEntities.IsNullOrEmpty())
                    {
                        var metadataModels = new List<TrakHoundDefinitionMetadata>();
                        foreach (var metadataEntity in metadataEntities)
                        {
                            var metadataModel = new TrakHoundDefinitionMetadata();
                            metadataModel.Uuid = metadataEntity.Uuid;
                            metadataModel.Name = metadataEntity.Name;
                            metadataModel.Value = metadataEntity.Value;
                            metadataModel.SourceUuid = metadataEntity.SourceUuid;
                            metadataModel.Created = metadataEntity.Created.ToDateTime();
                            metadataModels.Add(metadataModel);
                        }
                        definitionModel.Metadata = metadataModels;
                    }

                    // Descriptions
                    var descriptionEntities = await Client.System.Entities.Definitions.Description.QueryByDefinitionUuid(uuid, routerId);
                    if (!descriptionEntities.IsNullOrEmpty())
                    {
                        var descriptionModels = new List<TrakHoundDefinitionDescription>();
                        foreach (var description in descriptionEntities)
                        {
                            var descriptionModel = new TrakHoundDefinitionDescription();
                            descriptionModel.Uuid = description.Uuid;
                            descriptionModel.LanguageCode = description.LanguageCode;
                            descriptionModel.Text = description.Text;
                            descriptionModel.SourceUuid = description.SourceUuid;
                            descriptionModel.Created = description.Created.ToDateTime();
                            descriptionModels.Add(descriptionModel);
                        }
                        definitionModel.Descriptions = descriptionModels;
                    }

                    return Ok(definitionModel);
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

        [TrakHoundApiQuery("{definitionId}/parents")]
        public async Task<TrakHoundApiResponse> GetDefinitionParentsById(
            [FromRoute] string definitionId,
            [FromQuery] string routerId = null
            )
        {
            if (!string.IsNullOrEmpty(definitionId))
            {
                var uuid = TrakHoundDefinitionEntity.GenerateUuid(definitionId);

                var definitionEntities = await Client.System.Entities.Definitions.QueryByChildUuid(uuid, routerId: routerId);
                if (!definitionEntities.IsNullOrEmpty())
                {
                    var foundUuids = definitionEntities.Select(o => o.Uuid);

                    var metadataEntities = await Client.System.Entities.Definitions.Metadata.QueryByDefinitionUuid(foundUuids, routerId);
                    var dMetadataEntities = metadataEntities?.ToListDictionary(o => o.DefinitionUuid);

                    var descriptionEntities = await Client.System.Entities.Definitions.Description.QueryByDefinitionUuid(foundUuids, routerId);
                    var dDescriptionEntities = descriptionEntities?.ToListDictionary(o => o.DefinitionUuid);

                    var results = new List<TrakHoundDefinition>();
                    foreach (var definitionEntity in definitionEntities)
                    {
                        var result = new TrakHoundDefinition();
                        result.Uuid = definitionEntity.Uuid;
                        result.Id = definitionEntity.Id;
                        result.Type = definitionEntity.Type;
                        result.ParentUuid = definitionEntity.ParentUuid;
                        result.SourceUuid = definitionEntity.SourceUuid;
                        result.Created = definitionEntity.Created.ToDateTime();

                        // Metadata
                        var matchMetadataEntities = dMetadataEntities?.Get(definitionEntity.Uuid);
                        if (!matchMetadataEntities.IsNullOrEmpty())
                        {
                            var metadataModels = new List<TrakHoundDefinitionMetadata>();
                            foreach (var metadata in matchMetadataEntities)
                            {
                                var metadataModel = new TrakHoundDefinitionMetadata();
                                metadataModel.Uuid = metadata.Uuid;
                                metadataModel.Name = metadata.Name;
                                metadataModel.Value = metadata.Value;
                                metadataModel.SourceUuid = metadata.SourceUuid;
                                metadataModel.Created = metadata.Created.ToDateTime();
                                metadataModels.Add(metadataModel);
                            }
                            result.Metadata = metadataModels;
                        }

                        // Descriptions
                        var matchDescriptionEntities = dDescriptionEntities?.Get(definitionEntity.Uuid);
                        if (!matchDescriptionEntities.IsNullOrEmpty())
                        {
                            var descriptionModels = new List<TrakHoundDefinitionDescription>();
                            foreach (var description in matchDescriptionEntities)
                            {
                                var descriptionModel = new TrakHoundDefinitionDescription();
                                descriptionModel.Uuid = description.Uuid;
                                descriptionModel.LanguageCode = description.LanguageCode;
                                descriptionModel.Text = description.Text;
                                descriptionModel.SourceUuid = description.SourceUuid;
                                descriptionModel.Created = description.Created.ToDateTime();
                                descriptionModels.Add(descriptionModel);
                            }
                            result.Descriptions = descriptionModels;
                        }

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

        [TrakHoundApiQuery("uuid/{uuid}/parents")]
        public async Task<TrakHoundApiResponse> GetDefinitionParentsByUuid(
            [FromRoute] string uuid,
            [FromQuery] string routerId = null
            )
        {
            if (!string.IsNullOrEmpty(uuid))
            {
                var definitionEntities = await Client.System.Entities.Definitions.QueryByChildUuid(uuid, routerId: routerId);
                if (!definitionEntities.IsNullOrEmpty())
                {
                    var foundUuids = definitionEntities.Select(o => o.Uuid);

                    var metadataEntities = await Client.System.Entities.Definitions.Metadata.QueryByDefinitionUuid(foundUuids, routerId);
                    var dMetadataEntities = metadataEntities?.ToListDictionary(o => o.DefinitionUuid);

                    var descriptionEntities = await Client.System.Entities.Definitions.Description.QueryByDefinitionUuid(foundUuids, routerId);
                    var dDescriptionEntities = descriptionEntities?.ToListDictionary(o => o.DefinitionUuid);

                    var results = new List<TrakHoundDefinition>();
                    foreach (var definitionEntity in definitionEntities)
                    {
                        var result = new TrakHoundDefinition();
                        result.Uuid = definitionEntity.Uuid;
                        result.Id = definitionEntity.Id;
                        result.Type = definitionEntity.Type;
                        result.ParentUuid = definitionEntity.ParentUuid;
                        result.SourceUuid = definitionEntity.SourceUuid;
                        result.Created = definitionEntity.Created.ToDateTime();

                        // Metadata
                        var matchMetadataEntities = dMetadataEntities?.Get(definitionEntity.Uuid);
                        if (!matchMetadataEntities.IsNullOrEmpty())
                        {
                            var metadataModels = new List<TrakHoundDefinitionMetadata>();
                            foreach (var metadata in matchMetadataEntities)
                            {
                                var metadataModel = new TrakHoundDefinitionMetadata();
                                metadataModel.Uuid = metadata.Uuid;
                                metadataModel.Name = metadata.Name;
                                metadataModel.Value = metadata.Value;
                                metadataModel.SourceUuid = metadata.SourceUuid;
                                metadataModel.Created = metadata.Created.ToDateTime();
                                metadataModels.Add(metadataModel);
                            }
                            result.Metadata = metadataModels;
                        }

                        // Descriptions
                        var matchDescriptionEntities = dDescriptionEntities?.Get(definitionEntity.Uuid);
                        if (!matchDescriptionEntities.IsNullOrEmpty())
                        {
                            var descriptionModels = new List<TrakHoundDefinitionDescription>();
                            foreach (var description in matchDescriptionEntities)
                            {
                                var descriptionModel = new TrakHoundDefinitionDescription();
                                descriptionModel.Uuid = description.Uuid;
                                descriptionModel.LanguageCode = description.LanguageCode;
                                descriptionModel.Text = description.Text;
                                descriptionModel.SourceUuid = description.SourceUuid;
                                descriptionModel.Created = description.Created.ToDateTime();
                                descriptionModels.Add(descriptionModel);
                            }
                            result.Descriptions = descriptionModels;
                        }

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
        public async Task<TrakHoundApiResponse> PublishDefinition(
            [FromQuery] string id,
            [FromQuery] string description = null,
            [FromQuery] string languageCode = TrakHoundDefinitionDescriptionEntity.DefaultLanguageCode,
            [FromQuery] string parentId = null,
            [FromQuery] bool async = false,
            [FromQuery] string routerId = null
            )
        {
            var response = new TrakHoundApiResponse();

            if (!string.IsNullOrEmpty(id))
            {
                var entry = new TrakHoundDefinitionEntry();
                entry.Id = id;
                entry.ParentId = parentId;

                // Description
                if (!string.IsNullOrEmpty(languageCode))
                {
                    entry.Descriptions.Add(languageCode, description);
                }

                var entries = new TrakHoundDefinitionEntry[] { entry };

                return await PublishDefinitions(entries, async, routerId);
            }
            else
            {
                response.StatusCode = 400;
            }

            return response;
        }

        [TrakHoundApiPublish("batch")]
        public async Task<TrakHoundApiResponse> PublishDefinitions(
            [FromBody("application/json")] IEnumerable<TrakHoundDefinitionEntry> entries,
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

        public void CreateEntities(TrakHoundEntityCollection collection, IEnumerable<TrakHoundDefinitionEntry> entries, long created = 0)
        {
            if (!entries.IsNullOrEmpty())
            {
                var ts = created > 0 ? created : UnixDateTime.Now;

                foreach (var entry in entries)
                {
                    if (!string.IsNullOrEmpty(entry.Id))
                    {
                        var parentUuid = !string.IsNullOrEmpty(entry.ParentId) ? TrakHoundDefinitionEntity.GenerateUuid(entry.ParentId) : null;

                        var definitionEntity = new TrakHoundDefinitionEntity(entry.Id, parentUuid, created: ts);
                        collection.Add(definitionEntity);

                        // Metadata
                        if (!entry.Metadata.IsNullOrEmpty())
                        {
                            foreach (var metadata in entry.Metadata)
                            {
                                var metadataName = metadata.Key;
                                var metadataValue = metadata.Value;
                                collection.Add(new TrakHoundDefinitionMetadataEntity(definitionEntity.Uuid, metadataName, metadataValue, created: ts));
                            }
                        }

                        // Descriptions
                        if (!entry.Descriptions.IsNullOrEmpty())
                        {
                            foreach (var description in entry.Descriptions)
                            {
                                var descriptionLanguageCode = description.Key;
                                var descriptionText = description.Value;
                                collection.Add(new TrakHoundDefinitionDescriptionEntity(definitionEntity.Uuid, descriptionText, descriptionLanguageCode, created: ts));
                            }
                        }
                    }
                }
            }
        }

        [TrakHoundApiDelete]
        public async Task<TrakHoundApiResponse> DeleteDefinition(
            [FromQuery] string id,
            [FromQuery] bool async = false,
            [FromQuery] string routerId = null
            )
        {
            if (!string.IsNullOrEmpty(id))
            {
                var uuid = TrakHoundDefinitionEntity.GenerateUuid(id);

                var entity = await Client.System.Entities.Definitions.ReadByUuid(uuid, routerId);
                if (entity != null)
                {
                    var success = await Client.System.Entities.Definitions.Delete(uuid, async ? TrakHoundOperationMode.Async : TrakHoundOperationMode.Sync, routerId);
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
                return BadRequest();
            }
        }
    }
}
