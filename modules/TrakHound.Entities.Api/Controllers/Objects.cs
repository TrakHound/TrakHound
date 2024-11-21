// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrakHound.Api;
using TrakHound.Clients;
using TrakHound.Entities.Collections;
using TrakHound.Requests;
using TrakHound.Security;

namespace TrakHound.Entities.Api
{
    [TrakHoundApiController("objects")]
    public class Objects : EntitiesApiControllerBase
    {
        public Objects() { }

        public Objects(ITrakHoundApiConfiguration configuration, ITrakHoundClient client)
        {
            SetConfiguration(configuration);
            SetClient(client);
        }



        [TrakHoundApiQuery]
        public async Task<TrakHoundApiResponse> Query(
            [FromQuery] string path,
            [FromBody(ContentType = "application/json")] IEnumerable<string> paths,
            [FromQuery] int skip = _defaultSkip,
            [FromQuery] int take = _defaultTake,
            [FromQuery] string routerId = null
            )
        {
            if (!string.IsNullOrEmpty(path))
            {
                var entities = await Client.System.Entities.Objects.Query(path, skip, take, routerId: routerId);
                if (!entities.IsNullOrEmpty())
                {
                    return Ok(await GetObjects(entities, routerId));
                }
                else
                {
                    return NotFound();

                }
            }
            else if (!paths.IsNullOrEmpty())
            {
                var entities = await Client.System.Entities.Objects.Query(paths, skip, take, routerId: routerId);
                if (!entities.IsNullOrEmpty())
                {
                    return Ok(await GetObjects(entities, routerId));
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

        [TrakHoundApiQuery("ids")]
        public async Task<TrakHoundApiResponse> QueryIds(
            [FromQuery] string path,
            [FromBody(ContentType = "application/json")] IEnumerable<string> paths,
            [FromQuery] int skip = _defaultSkip,
            [FromQuery] int take = _defaultTake,
            [FromQuery] string routerId = null
            )
        {
            if (!string.IsNullOrEmpty(path))
            {
                var entities = await Client.System.Entities.Objects.Query(path, skip, take, routerId: routerId);
                if (!entities.IsNullOrEmpty())
                {
                    var results = new Dictionary<string, string>();
                    foreach (var entity in entities)
                    {
                        results.Remove(entity.GetAbsolutePath());
                        results.Add(entity.GetAbsolutePath(), entity.Uuid);
                    }
                    return Ok(results);
                }
                else
                {
                    return NotFound();

                }
            }
            else if (!paths.IsNullOrEmpty())
            {
                var entities = await Client.System.Entities.Objects.Query(paths, skip, take, routerId: routerId);
                if (!entities.IsNullOrEmpty())
                {
                    var results = new Dictionary<string, string>();
                    foreach (var entity in entities)
                    {
                        results.Remove(entity.GetAbsolutePath());
                        results.Add(entity.GetAbsolutePath(), entity.Uuid);
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

        [TrakHoundApiQuery("content-type")]
        [TrakHoundPermission("read")]
        public async Task<TrakHoundApiResponse> QueryContentType(
            [FromQuery] string path,
            [FromBody(ContentType = "application/json")] IEnumerable<string> paths,
            [FromQuery] int skip = _defaultSkip,
            [FromQuery] int take = _defaultTake,
            [FromQuery] string routerId = null
            )
        {
            if (!string.IsNullOrEmpty(path))
            {
                var entities = await Client.System.Entities.Objects.Query(path, skip, take, routerId: routerId);
                if (!entities.IsNullOrEmpty())
                {
                    var results = new Dictionary<string, string>();
                    foreach (var entity in entities)
                    {
                        results.Remove(entity.GetAbsolutePath());
                        results.Add(entity.GetAbsolutePath(), entity.ContentType);
                    }
                    return Ok(results);
                }
                else
                {
                    return NotFound();

                }
            }
            else if (!paths.IsNullOrEmpty())
            {
                var entities = await Client.System.Entities.Objects.Query(paths, skip, take, routerId: routerId);
                if (!entities.IsNullOrEmpty())
                {
                    var results = new Dictionary<string, string>();
                    foreach (var entity in entities)
                    {
                        results.Remove(entity.GetAbsolutePath());
                        results.Add(entity.GetAbsolutePath(), entity.ContentType);
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

        [TrakHoundApiQuery("{uuid}/children")]
        public async Task<TrakHoundApiResponse> GetObjectChildren(
            [FromRoute] string uuid,
            [FromQuery] string routerId = null
            )
        {
            if (!string.IsNullOrEmpty(uuid))
            {
                var entities = await Client.System.Entities.Objects.QueryByParentUuid(uuid, routerId: routerId);
                if (!entities.IsNullOrEmpty())
                {
                    return Ok(await GetObjects(entities, routerId));
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

        [TrakHoundApiQuery("children")]
        public async Task<TrakHoundApiResponse> GetObjectChildren(
            [FromBody(ContentType = "application/json")] IEnumerable<string> uuids,
            [FromQuery] string routerId = null
            )
        {
            if (!uuids.IsNullOrEmpty())
            {
                var entities = await Client.System.Entities.Objects.QueryByParentUuid(uuids, routerId: routerId);
                if (!entities.IsNullOrEmpty())
                {
                    return Ok(await GetObjects(entities, routerId));
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
        public async Task<ITrakHoundConsumer<TrakHoundApiResponse>> Subscribe(
            [FromQuery] string path,
            [FromQuery] int interval = _defaultSubscribeInterval,
            [FromQuery] int take = _defaultSubscribeTake,
            [FromQuery] string routerId = null
            )
        {
            if (!string.IsNullOrEmpty(path))
            {
                var objectPaths = new string[] { path };
                var consumer = await Client.System.Entities.Objects.Subscribe(objectPaths, interval, take, routerId: routerId);
                if (consumer != null)
                {
                    var resultConsumer = new TrakHoundConsumer<IEnumerable<ITrakHoundObjectEntity>, TrakHoundApiResponse>(consumer);
                    resultConsumer.OnReceivedAsync = async (entities) =>
                    {
                        return TrakHoundApiJsonResponse.Ok(await GetObjects(entities, routerId));
                    };

                    return resultConsumer;
                }
            }

            return null;
        }

        private async Task<IEnumerable<TrakHoundObject>> GetObjects(IEnumerable<ITrakHoundObjectEntity> objects, string routerId)
        {
            if (!objects.IsNullOrEmpty())
            {
                // Query Object Metadata
                var metadataEntities = await Client.System.Entities.Objects.Metadata.QueryByEntityUuid(objects.Select(o => o.Uuid), routerId);
                var dMetadata = metadataEntities?.ToDistinct().ToListDictionary(o => o.EntityUuid);

                // Get Definition IDs
                var definitionUuids = new List<string>();
                foreach (var entity in objects)
                {
                    if (!string.IsNullOrEmpty(entity.DefinitionUuid)) definitionUuids.Add(entity.DefinitionUuid);
                }
                if (!metadataEntities.IsNullOrEmpty())
                {
                    foreach (var entity in metadataEntities)
                    {
                        if (!string.IsNullOrEmpty(entity.DefinitionUuid)) definitionUuids.Add(entity.DefinitionUuid);
                        if (!string.IsNullOrEmpty(entity.ValueDefinitionUuid)) definitionUuids.Add(entity.ValueDefinitionUuid);
                    }
                }


                // Query Definitions
                var definitions = await Client.System.Entities.Definitions.ReadByUuid(definitionUuids, routerId);
                var dDefinitions = definitions?.ToDistinct().ToDictionary(o => o.Uuid);

                // Query Definition Descriptions
                var descriptions = await Client.System.Entities.Definitions.Description.QueryByDefinitionUuid(definitionUuids, routerId);
                var dDescriptions = descriptions?.ToDistinct().ToListDictionary(o => o.DefinitionUuid);


                var objs = new List<TrakHoundObject>();
                foreach (var entity in objects)
                {
                    var obj = new TrakHoundObject();
                    obj.Path = entity.GetAbsolutePath();
                    obj.Uuid = entity.Uuid;
                    obj.Name = TrakHoundPath.GetObject(entity.Path);
                    obj.Namespace = entity.Namespace;
                    obj.ContentType = entity.ContentType;
                    obj.DefinitionUuid = entity.DefinitionUuid;
                    obj.ParentUuid = entity.ParentUuid;
                    obj.SourceUuid = entity.SourceUuid;
                    obj.Created = entity.Created.ToDateTime();

                    // Get Definition
                    if (!string.IsNullOrEmpty(entity.DefinitionUuid))
                    {
                        var definition = dDefinitions?.GetValueOrDefault(entity.DefinitionUuid);
                        if (definition != null)
                        {
                            obj.DefinitionId = definition.Id;
                            obj.Type = definition.Type;

                            // Get Definition Description
                            var description = dDescriptions?.Get(definition.Uuid)?.FirstOrDefault();
                            if (description != null)
                            {
                                obj.Description = description.Text;
                            }
                        }
                    }

                    // Get Metadata
                    var objectMetadata = dMetadata?.Get(entity.Uuid);
                    if (!objectMetadata.IsNullOrEmpty())
                    {
                        var metadatas = new List<TrakHoundMetadata>();

                        foreach (var metadataEntity in objectMetadata)
                        {
                            // Get Metadata Value Definition
                            ITrakHoundDefinitionEntity metadataValueDefinition = null;
                            if (!string.IsNullOrEmpty(metadataEntity.ValueDefinitionUuid)) metadataValueDefinition = dDefinitions?.GetValueOrDefault(metadataEntity.ValueDefinitionUuid);

                            var metadata = new TrakHoundMetadata();
                            metadata.Uuid = metadataEntity.Uuid;
                            metadata.Name = metadataEntity.Name;
                            metadata.DefinitionUuid = metadataEntity.DefinitionUuid;
                            metadata.ValueDefinitionUuid = metadataEntity.ValueDefinitionUuid;
                            metadata.Value = metadataEntity.Value;
                            metadata.SourceUuid = metadataEntity.SourceUuid;
                            metadata.Created = metadataEntity.Created.ToDateTime();

                            // Get Metadata Definition
                            if (!string.IsNullOrEmpty(metadataEntity.DefinitionUuid))
                            {
                                var metadataDefinition = dDefinitions?.GetValueOrDefault(metadataEntity.DefinitionUuid);
                                if (metadataDefinition != null)
                                {
                                    metadata.Type = metadataDefinition.Type;

                                    // Get Definition Description
                                    var metadataDescription = dDescriptions?.Get(metadataDefinition.Uuid)?.FirstOrDefault();
                                    if (metadataDescription != null)
                                    {
                                        metadata.Description = metadataDescription.Text;
                                    }
                                }
                            }

                            // Get Metadata Value Definition
                            if (!string.IsNullOrEmpty(metadataEntity.ValueDefinitionUuid))
                            {
                                var metadataDefinition = dDefinitions?.GetValueOrDefault(metadataEntity.ValueDefinitionUuid);
                                if (metadataDefinition != null)
                                {
                                    metadata.ValueType = metadataDefinition.Type;

                                    // Get Definition Description
                                    var metadataDescription = dDescriptions?.Get(metadataDefinition.Uuid)?.FirstOrDefault();
                                    if (metadataDescription != null)
                                    {
                                        metadata.ValueDescription = metadataDescription.Text;
                                    }
                                }
                            }

                            metadatas.Add(metadata);
                        }

                        obj.Metadata = metadatas;
                    }

                    objs.Add(obj);
                }

                return objs;
            }

            return null;
        }

        [TrakHoundApiPublish]
        public async Task<TrakHoundApiResponse> PublishObject(
            [FromQuery] string path,
            [FromQuery] string contentType = null,
            [FromQuery] string definitionId = null,
            [FromQuery] bool async = false,
            [FromQuery] string routerId = null
            )
        {
            if (!string.IsNullOrEmpty(path))
            {
                IEnumerable<CreateObjectResponse> createResponses = null;
                var now = UnixDateTime.Now;
                var publishCollection = new TrakHoundEntityCollection();

                if (!string.IsNullOrEmpty(path))
                {
                    var ns = TrakHoundPath.GetNamespace(path);
                    var partialPath = TrakHoundPath.GetPartialPath(path);

                    var entry = new TrakHoundObjectEntry();
                    entry.Namespace = !string.IsNullOrEmpty(ns) ? ns : TrakHoundNamespace.DefaultNamespace;
                    entry.Path = !partialPath.StartsWith('/') ? $"/{partialPath}" : partialPath;
                    entry.ContentType = contentType.ConvertEnum<TrakHoundObjectContentType>().ToString();
                    entry.DefinitionId = definitionId;
                    var entries = new List<TrakHoundObjectEntry> { entry };

                    createResponses = CreateObjects(publishCollection, entries, now);

                }
                else
                {
                    return BadRequest();
                }


                if (!createResponses.IsNullOrEmpty())
                {
                    if (await PublishEntities(publishCollection, now, async, routerId: routerId))
                    {
                        var uuids = createResponses.Select(o => o.ObjectUuid).Distinct();

                        if (!async)
                        {
                            var publishedObjects = await Client.System.Entities.Objects.ReadByUuid(uuids, routerId);
                            if (!publishedObjects.IsNullOrEmpty())
                            {
                                var objectResponses = new List<TrakHoundObjectResponse>();

                                foreach (var publishedObject in publishedObjects)
                                {
                                    var objectResponse = new TrakHoundObjectResponse();
                                    objectResponse.Uuid = publishedObject.Uuid;
                                    objectResponse.Path = publishedObject.Path;
                                    objectResponse.Name = publishedObject.Name;
                                    objectResponse.Namespace = publishedObject.Namespace;
                                    objectResponse.ContentType = publishedObject.ContentType;
                                    objectResponse.DefinitionUuid = publishedObject.DefinitionUuid;
                                    objectResponse.Priority = publishedObject.Priority;
                                    objectResponse.Created = publishedObject.Created.ToDateTime();

                                    objectResponses.Add(objectResponse);
                                }

                                return Ok(objectResponses);
                            }
                            else
                            {
                                return NotFound();
                            }
                        }
                        else
                        {
                            return Accepted();
                        }
                    }
                    else
                    {
                        return InternalError();
                    }
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

        [TrakHoundApiPublish("batch")]
        public async Task<TrakHoundApiResponse> PublishObjects(
            [FromBody("application/json")] IEnumerable<TrakHoundObjectEntry> entries,
            [FromQuery] bool async = false,
            [FromQuery] string routerId = null
            )
        {
            if (!entries.IsNullOrEmpty())
            {
                var now = UnixDateTime.Now;
                var publishCollection = new TrakHoundEntityCollection();

                var createResponses = CreateObjects(publishCollection, entries, now);
                if (!createResponses.IsNullOrEmpty())
                {
                    // Publish Entities
                    if (await PublishEntities(publishCollection, now, async, routerId: routerId))
                    {
                        var uuids = createResponses.Select(o => o.ObjectUuid).Distinct();

                        if (!async)
                        {
                            var publishedObjects = await Client.System.Entities.Objects.ReadByUuid(uuids, routerId);
                            if (!publishedObjects.IsNullOrEmpty())
                            {
                                var objectResponses = new List<TrakHoundObjectResponse>();

                                foreach (var publishedObject in publishedObjects)
                                {
                                    var objectResponse = new TrakHoundObjectResponse();
                                    objectResponse.Uuid = publishedObject.Uuid;
                                    objectResponse.Path = publishedObject.Path;
                                    objectResponse.Name = publishedObject.Name;
                                    objectResponse.Namespace = publishedObject.Namespace;
                                    objectResponse.ContentType = publishedObject.ContentType;
                                    objectResponse.DefinitionUuid = publishedObject.DefinitionUuid;
                                    objectResponse.Priority = publishedObject.Priority;
                                    objectResponse.Created = publishedObject.Created.ToDateTime();

                                    objectResponses.Add(objectResponse);
                                }

                                return Ok(objectResponses);
                            }
                            else
                            {
                                return NotFound();
                            }
                        }
                        else
                        {
                            return Accepted();
                        }
                    }
                    else
                    {
                        return InternalError();
                    }
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


        [TrakHoundApiDelete]
        public async Task<TrakHoundApiResponse> DeleteObject(
            [FromQuery] string path,
            [FromQuery] int skip = _defaultSkip,
            [FromQuery] int take = _defaultTake,
            [FromQuery] bool async = false,
            [FromQuery] string routerId = null
            )
        {
            if (!string.IsNullOrEmpty(path))
            {
                var entities = await Client.System.Entities.Objects.Query(path, skip, take, routerId: routerId);
                if (!entities.IsNullOrEmpty())
                {
                    var targetUuids = entities.Select(o => o.Uuid).Distinct();

                    var targetObjects = new List<ITrakHoundObjectEntity>();
                    targetObjects.AddRange(entities);

                    var childObjects = await Client.System.Entities.Objects.QueryChildrenByRootUuid(targetUuids, routerId: routerId);
                    if (!childObjects.IsNullOrEmpty()) targetObjects.AddRange(childObjects);

                    await DeleteContent(targetObjects, routerId);

                    var deleteUuids = targetObjects.Select(o => o.Uuid).Distinct();

                    // Delete Metadata
                    await Client.System.Entities.Objects.Metadata.DeleteByObjectUuid(deleteUuids, routerId);

                    var success = await Client.System.Entities.Objects.Delete(deleteUuids, async ? TrakHoundOperationMode.Async : TrakHoundOperationMode.Sync, routerId: routerId);
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

        [TrakHoundApiDelete("batch")]
        public async Task<TrakHoundApiResponse> DeleteObjects(
            [FromBody("application/json")] IEnumerable<string> paths,
            [FromQuery] bool async = false,
            [FromQuery] string routerId = null
            )
        {
            if (!paths.IsNullOrEmpty())
            {
                var entities = await Client.System.Entities.Objects.Query(paths, routerId: routerId);
                if (!entities.IsNullOrEmpty())
                {
                    var targetUuids = entities.Select(o => o.Uuid);

                    var targetObjects = new List<ITrakHoundObjectEntity>();
                    targetObjects.AddRange(entities);

                    var childObjects = await Client.System.Entities.Objects.QueryChildrenByRootUuid(targetUuids, routerId: routerId);
                    if (!childObjects.IsNullOrEmpty()) targetObjects.AddRange(childObjects);

                    // Delete Content
                    await DeleteContent(targetObjects, routerId);

                    var deleteUuids = targetObjects.Select(o => o.Uuid).Distinct();

                    // Delete Metadata
                    await Client.System.Entities.Objects.Metadata.DeleteByObjectUuid(deleteUuids, routerId);

                    if (!childObjects.IsNullOrEmpty())
                    {
                        // Delete Objects
                        var success = await Client.System.Entities.Objects.DeleteByRootUuid(targetUuids, routerId);
                        if (success)
                        {
                            success = await Client.System.Entities.Objects.Delete(targetUuids, async ? TrakHoundOperationMode.Async : TrakHoundOperationMode.Sync, routerId: routerId);
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
                            return InternalError();
                        }
                    }
                    else
                    {
                        var success = await Client.System.Entities.Objects.Delete(targetUuids, async ? TrakHoundOperationMode.Async : TrakHoundOperationMode.Sync, routerId: routerId);
                        if (success)
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
                    return NotFound();
                }
            }
            else
            {
                return BadRequest();
            }
        }

        [TrakHoundApiDelete("metadata")]
        public async Task<TrakHoundApiResponse> DeleteObjectMetadata(
            [FromQuery] string path,
            [FromQuery] string routerId = null
            )
        {
            if (!string.IsNullOrEmpty(path))
            {
                var entities = await Client.System.Entities.Objects.Query(path, routerId: routerId);
                if (!entities.IsNullOrEmpty())
                {
                    var objectUuids = entities.Select(o => o.Uuid).Distinct();

                    var success = await Client.System.Entities.Objects.Metadata.DeleteByObjectUuid(objectUuids, routerId);
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


        private async Task<bool> DeleteContent(IEnumerable<ITrakHoundObjectEntity> objects, string routerId)
        {
            if (!objects.IsNullOrEmpty())
            {
                // Assignments
                var assignmentObjects = objects.Where(o => o.ContentType == TrakHoundObjectContentTypes.Assignment);
                if (!assignmentObjects.IsNullOrEmpty())
                {
                    var objectUuids = assignmentObjects.Select(o => o.Uuid);
                    await Client.System.Entities.Objects.Assignment.DeleteByAssigneeUuid(objectUuids, routerId);
                }

                // Blobs
                var blobObjects = objects.Where(o => o.ContentType == TrakHoundObjectContentTypes.Blob);
                if (!blobObjects.IsNullOrEmpty())
                {
                    var uuids = blobObjects.Select(o => TrakHoundObjectBlobEntity.GenerateUuid(o.Uuid));
                    var blobEntities = await Client.System.Entities.Objects.Blob.ReadByUuid(uuids, routerId);
                    if (!blobEntities.IsNullOrEmpty())
                    {
                        var blobIds = blobEntities.Select(o => o.BlobId).Distinct();
                        foreach (var blobId in blobIds)
                        {
                            await Client.System.Blobs.Delete(blobId);
                        }
                    }

                    await Client.System.Entities.Objects.Blob.Delete(uuids, TrakHoundOperationMode.Sync, routerId);
                }

                // Booleans
                var booleanObjects = objects.Where(o => o.ContentType == TrakHoundObjectContentTypes.Boolean);
                if (!booleanObjects.IsNullOrEmpty())
                {
                    var uuids = booleanObjects.Select(o => TrakHoundObjectBooleanEntity.GenerateUuid(o.Uuid));
                    await Client.System.Entities.Objects.Boolean.Delete(uuids, TrakHoundOperationMode.Sync, routerId);
                }

                // Durations
                var durationObjects = objects.Where(o => o.ContentType == TrakHoundObjectContentTypes.Duration);
                if (!durationObjects.IsNullOrEmpty())
                {
                    var uuids = durationObjects.Select(o => TrakHoundObjectDurationEntity.GenerateUuid(o.Uuid));
                    await Client.System.Entities.Objects.Duration.Delete(uuids, TrakHoundOperationMode.Sync, routerId);
                }

                // Events
                var eventObjects = objects.Where(o => o.ContentType == TrakHoundObjectContentTypes.Event);
                if (!eventObjects.IsNullOrEmpty())
                {
                    var objectUuids = eventObjects.Select(o => o.Uuid);
                    await Client.System.Entities.Objects.Event.DeleteByObjectUuid(objectUuids, routerId);
                }

                // Groups
                var groupObjects = objects.Where(o => o.ContentType == TrakHoundObjectContentTypes.Group);
                if (!groupObjects.IsNullOrEmpty())
                {
                    var objectUuids = groupObjects.Select(o => o.Uuid);
                    await Client.System.Entities.Objects.Group.DeleteByGroupUuid(objectUuids, routerId);
                }

                // Hashes
                var hashObjects = objects.Where(o => o.ContentType == TrakHoundObjectContentTypes.Hash);
                if (!hashObjects.IsNullOrEmpty())
                {
                    var objectUuids = hashObjects.Select(o => o.Uuid);
                    await Client.System.Entities.Objects.Hash.DeleteByObjectUuid(objectUuids, routerId);
                }

                // Logs
                var logObjects = objects.Where(o => o.ContentType == TrakHoundObjectContentTypes.Log);
                if (!logObjects.IsNullOrEmpty())
                {
                    var objectUuids = logObjects.Select(o => o.Uuid);
                    await Client.System.Entities.Objects.Log.DeleteByObjectUuid(objectUuids, routerId);
                }

                // Numbers
                var numberObjects = objects.Where(o => o.ContentType == TrakHoundObjectContentTypes.Number);
                if (!numberObjects.IsNullOrEmpty())
                {
                    var uuids = numberObjects.Select(o => TrakHoundObjectNumberEntity.GenerateUuid(o.Uuid));
                    await Client.System.Entities.Objects.Number.Delete(uuids, TrakHoundOperationMode.Sync, routerId);
                }

                // Observations
                var observationObjects = objects.Where(o => o.ContentType == TrakHoundObjectContentTypes.Observation);
                if (!observationObjects.IsNullOrEmpty())
                {
                    var objectUuids = observationObjects.Select(o => o.Uuid);
                    await Client.System.Entities.Objects.Observation.DeleteByObjectUuid(objectUuids, routerId);
                }

                // References
                var referenceObjects = objects.Where(o => o.ContentType == TrakHoundObjectContentTypes.Reference);
                if (!referenceObjects.IsNullOrEmpty())
                {
                    var uuids = referenceObjects.Select(o => TrakHoundObjectReferenceEntity.GenerateUuid(o.Uuid));
                    await Client.System.Entities.Objects.Reference.Delete(uuids, TrakHoundOperationMode.Sync, routerId);
                }

                // Sets
                var setObjects = objects.Where(o => o.ContentType == TrakHoundObjectContentTypes.Set);
                if (!setObjects.IsNullOrEmpty())
                {
                    var objectUuids = setObjects.Select(o => o.Uuid);
                    await Client.System.Entities.Objects.Set.DeleteByObjectUuid(objectUuids, routerId);
                }

                // Statistics
                var statisticObjects = objects.Where(o => o.ContentType == TrakHoundObjectContentTypes.Statistic);
                if (!statisticObjects.IsNullOrEmpty())
                {
                    var objectUuids = statisticObjects.Select(o => o.Uuid);
                    await Client.System.Entities.Objects.Statistic.DeleteByObjectUuid(objectUuids, routerId);
                }

                // Strings
                var stringObjects = objects.Where(o => o.ContentType == TrakHoundObjectContentTypes.String);
                if (!stringObjects.IsNullOrEmpty())
                {
                    var uuids = stringObjects.Select(o => TrakHoundObjectStringEntity.GenerateUuid(o.Uuid));
                    await Client.System.Entities.Objects.String.Delete(uuids, TrakHoundOperationMode.Sync, routerId);
                }

                // Timestamps
                var timestampObjects = objects.Where(o => o.ContentType == TrakHoundObjectContentTypes.Timestamp);
                if (!timestampObjects.IsNullOrEmpty())
                {
                    var uuids = timestampObjects.Select(o => TrakHoundObjectTimestampEntity.GenerateUuid(o.Uuid));
                    await Client.System.Entities.Objects.Timestamp.Delete(uuids, TrakHoundOperationMode.Sync, routerId);
                }

                // Vocabularies
                var vocabularyObjects = objects.Where(o => o.ContentType == TrakHoundObjectContentTypes.Vocabulary);
                if (!vocabularyObjects.IsNullOrEmpty())
                {
                    var uuids = vocabularyObjects.Select(o => TrakHoundObjectVocabularyEntity.GenerateUuid(o.Uuid));
                    await Client.System.Entities.Objects.Vocabulary.Delete(uuids, TrakHoundOperationMode.Sync, routerId);
                }

                // VocabularySets
                var vocabularySetObjects = objects.Where(o => o.ContentType == TrakHoundObjectContentTypes.VocabularySet);
                if (!vocabularySetObjects.IsNullOrEmpty())
                {
                    var objectUuids = vocabularySetObjects.Select(o => o.Uuid);
                    await Client.System.Entities.Objects.VocabularySet.DeleteByObjectUuid(objectUuids, routerId);
                }
            }

            return false;
        }
    }
}
