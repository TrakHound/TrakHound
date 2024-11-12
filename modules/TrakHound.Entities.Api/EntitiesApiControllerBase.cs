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
    public abstract class EntitiesApiControllerBase : TrakHoundApiController
    {
        protected const int _defaultSkip = 0;
        protected const int _defaultTake = 1000;
        protected const int _defaultSubscribeInterval = 100;
        protected const int _defaultSubscribeTake = 1000;



        protected struct CreateObjectResponse
        {
            public string Path { get; set; }
            public string ObjectUuid { get; set; }
        }


        protected override void AfterClientSet()
        {
            //Client.AddMiddleware(new TrakHoundOnChangeMiddleware(Configuration.Id));
        }


        protected async Task<IEnumerable<CreateObjectResponse>> CreateObjects(IEnumerable<TrakHoundObjectEntry> entries, bool async = false, string routerId = null)
        {
            var responses = new List<CreateObjectResponse>();

            if (!entries.IsNullOrEmpty())
            {
                var objectEntities = new Dictionary<string, ITrakHoundObjectEntity>();
                var metadataEntities = new Dictionary<string, ITrakHoundObjectMetadataEntity>();
                var now = UnixDateTime.Now;

                foreach (var entry in entries)
                {
                    var entryNamespace = !string.IsNullOrEmpty(entry.Namespace) ? entry.Namespace : TrakHoundPath.GetNamespace(entry.Path);
                    if (string.IsNullOrEmpty(entryNamespace)) entryNamespace = TrakHoundNamespace.DefaultNamespace;

                    var entryPath = TrakHoundPath.ToRoot(TrakHoundPath.GetPartialPath(entry.Path));

                    // Get Definition
                    var definitionUuid = !string.IsNullOrEmpty(entry.DefinitionId) ? TrakHoundDefinitionEntity.GenerateUuid(entry.DefinitionId) : null;

                    // Get Parent
                    string parentUuid = null;
                    var parentPath = TrakHoundPath.GetParentPath(entryPath);
                    if (!string.IsNullOrEmpty(parentPath))
                    {
                        // Get all of the Parent Paths
                        var parentPaths = TrakHoundPath.GetPaths(parentPath);
                        if (!parentPaths.IsNullOrEmpty())
                        {
                            foreach (var path in parentPaths)
                            {
                                var parentEntity = new TrakHoundObjectEntity(path, TrakHoundObjectContentType.Directory, ns: entryNamespace, priority: 0);
                                parentUuid = parentEntity.Uuid;

                                var existingParentEntity = objectEntities.GetValueOrDefault(parentEntity.Uuid);
                                if (existingParentEntity == null || parentEntity.Priority >= existingParentEntity.Priority)
                                {
                                    objectEntities.Remove(parentEntity.Uuid);
                                    objectEntities.Add(parentEntity.Uuid, parentEntity);
                                }
                            }
                        }
                    }

                    // Add Object to list of Entities to create
                    var createObjectEntity = new TrakHoundObjectEntity(entryPath, entry.ContentType.ConvertEnum<TrakHoundObjectContentType>(), definitionUuid, entryNamespace, entry.Priority, created: now);

                    var existingObjectEntity = objectEntities.GetValueOrDefault(createObjectEntity.Uuid);
                    if (existingObjectEntity == null || createObjectEntity.Priority >= existingObjectEntity.Priority)
                    {
                        objectEntities.Remove(createObjectEntity.Uuid);
                        objectEntities.Add(createObjectEntity.Uuid, createObjectEntity);
                    }

                    // Add Object Metadata to list of Entities to create
                    if (!entry.Metadata.IsNullOrEmpty())
                    {
                        foreach (var metadata in entry.Metadata)
                        {
                            var metadataEntity = new TrakHoundObjectMetadataEntity(createObjectEntity.Uuid, metadata.Key, metadata.Value, created: now);
                            metadataEntities.Add(metadataEntity.Uuid, metadataEntity);
                        }
                    }

                    var response = new CreateObjectResponse();
                    response.Path = createObjectEntity.GetAbsolutePath();
                    response.ObjectUuid = createObjectEntity.Uuid;
                    responses.Add(response);
                }

                var operationMode = async ? TrakHoundOperationMode.Async : TrakHoundOperationMode.Sync;

                if ((objectEntities.IsNullOrEmpty() || await Client.System.Entities.Objects.Publish(objectEntities.Values, operationMode, routerId)) &&
                    (metadataEntities.IsNullOrEmpty() || await Client.System.Entities.Objects.Metadata.Publish(metadataEntities.Values, operationMode, routerId)))
                {
                    return responses;
                }
            }

            return null;
        }

        protected async Task CreateContentObjects(IEnumerable<ITrakHoundEntityEntryOperation> entries, bool async = false, string routerId = null)
        {
            if (!entries.IsNullOrEmpty())
            {
                // Get list of Object Entries to Create
                var objectEntries = new List<TrakHoundObjectEntry>();
                foreach (var entry in entries)
                {
                    if (entry.Category == TrakHoundEntityCategoryId.Objects)
                    {
                        if (entry.Class == TrakHoundObjectsEntityClassId.Object)
                        {
                            objectEntries.Add((TrakHoundObjectEntry)entry);
                        }
                        else
                        {
                            var objectEntry = new TrakHoundObjectEntry();
                            objectEntry.Path = entry.AssemblyId;

                            if (typeof(ITrakHoundObjectsEntryOperation).IsAssignableFrom(entry.GetType()))
                            {
                                objectEntry.DefinitionId = ((ITrakHoundObjectsEntryOperation)entry).AssemblyDefinitionId;
                            }

                            objectEntry.ContentType = TrakHoundObjectsEntityClassName.Get(entry.Class);
                            objectEntry.Priority = 0;
                            objectEntries.Add(objectEntry);
                        }
                    }
                }

                if (!objectEntries.IsNullOrEmpty()) await CreateObjects(objectEntries, async, routerId);
            }
        }


        protected IEnumerable<CreateObjectResponse> CreateObjects(TrakHoundEntityCollection publishCollection, IEnumerable<TrakHoundObjectEntry> entries, long created = 0)
        {
            var responses = new List<CreateObjectResponse>();

            if (!entries.IsNullOrEmpty())
            {
                var objectEntities = new Dictionary<string, ITrakHoundObjectEntity>();
                var metadataEntities = new Dictionary<string, ITrakHoundObjectMetadataEntity>();
                var ts = created > 0 ? created : UnixDateTime.Now;

                foreach (var entry in entries)
                {
                    var entryNamespace = !string.IsNullOrEmpty(entry.Namespace) ? entry.Namespace : TrakHoundPath.GetNamespace(entry.Path);
                    if (string.IsNullOrEmpty(entryNamespace)) entryNamespace = TrakHoundNamespace.DefaultNamespace;

                    var entryPath = TrakHoundPath.ToRoot(TrakHoundPath.GetPartialPath(entry.Path));

                    // Get Definition
                    var definitionUuid = !string.IsNullOrEmpty(entry.DefinitionId) ? TrakHoundDefinitionEntity.GenerateUuid(entry.DefinitionId) : null;

                    // Get Parent
                    string parentUuid = null;
                    var parentPath = TrakHoundPath.GetParentPath(entryPath);
                    if (!string.IsNullOrEmpty(parentPath))
                    {
                        // Get all of the Parent Paths
                        var parentPaths = TrakHoundPath.GetPaths(parentPath);
                        if (!parentPaths.IsNullOrEmpty())
                        {
                            foreach (var path in parentPaths)
                            {
                                var parentEntity = new TrakHoundObjectEntity(path, TrakHoundObjectContentType.Directory, ns: entryNamespace, priority: 0, created: ts);
                                parentUuid = parentEntity.Uuid;

                                var existingParentEntity = objectEntities.GetValueOrDefault(parentEntity.Uuid);
                                if (existingParentEntity == null || parentEntity.Priority >= existingParentEntity.Priority)
                                {
                                    objectEntities.Remove(parentEntity.Uuid);
                                    objectEntities.Add(parentEntity.Uuid, parentEntity);
                                }
                            }
                        }
                    }

                    // Add Object to list of Entities to create
                    var createObjectEntity = new TrakHoundObjectEntity(entryPath, entry.ContentType.ConvertEnum<TrakHoundObjectContentType>(), definitionUuid, entryNamespace, entry.Priority, created: ts);

                    var existingObjectEntity = objectEntities.GetValueOrDefault(createObjectEntity.Uuid);
                    if (existingObjectEntity == null || createObjectEntity.Priority >= existingObjectEntity.Priority)
                    {
                        objectEntities.Remove(createObjectEntity.Uuid);
                        objectEntities.Add(createObjectEntity.Uuid, createObjectEntity);
                    }
                    
                    // Add Object Metadata to list of Entities to create
                    if (!entry.Metadata.IsNullOrEmpty())
                    {
                        foreach (var metadata in entry.Metadata)
                        {
                            var metadataEntity = new TrakHoundObjectMetadataEntity(createObjectEntity.Uuid, metadata.Key, metadata.Value, created: ts);
                            metadataEntities.Add(metadataEntity.Uuid, metadataEntity);
                        }
                    }

                    var response = new CreateObjectResponse();
                    response.Path = createObjectEntity.GetAbsolutePath();
                    response.ObjectUuid = createObjectEntity.Uuid;
                    responses.Add(response);
                }

                publishCollection.Add(objectEntities.Values);
                publishCollection.Add(metadataEntities.Values);

                return responses;
            }

            return null;
        }

        protected void CreateContentObjects(TrakHoundEntityCollection publishCollection, IEnumerable<ITrakHoundEntityEntryOperation> entries, long created = 0)
        {
            if (!entries.IsNullOrEmpty())
            {
                // Get list of Object Entries to Create
                var objectEntries = new List<TrakHoundObjectEntry>();
                foreach (var entry in entries)
                {
                    if (entry.Category == TrakHoundEntityCategoryId.Objects)
                    {
                        if (entry.Class == TrakHoundObjectsEntityClassId.Object)
                        {
                            objectEntries.Add((TrakHoundObjectEntry)entry);
                        }
                        else
                        {
                            var ns = TrakHoundPath.GetNamespace(entry.AssemblyId);
                            var partialPath = TrakHoundPath.GetPartialPath(entry.AssemblyId);

                            var objectEntry = new TrakHoundObjectEntry();
                            objectEntry.Namespace = ns;
                            objectEntry.Path = partialPath;

                            if (typeof(ITrakHoundObjectsEntryOperation).IsAssignableFrom(entry.GetType()))
                            {
                                objectEntry.DefinitionId = ((ITrakHoundObjectsEntryOperation)entry).AssemblyDefinitionId;
                            }

                            objectEntry.ContentType = TrakHoundObjectsEntityClassName.Get(entry.Class);
                            objectEntry.Priority = 0;
                            objectEntries.Add(objectEntry);
                        }
                    }
                }

                if (!objectEntries.IsNullOrEmpty()) CreateObjects(publishCollection, objectEntries, created);
            }
        }


        protected async Task<bool> PublishEntities(
            TrakHoundEntityCollection publishCollection,
            long timestamp,
            bool async,
            TrakHoundSourceEntry source = null,
            IEnumerable<EntityIndexPublishRequest> indexes = null,
            string routerId = null
            )
        {
            // Get the Entities to Publish
            var entities = publishCollection?.GetEntities()?.ToList();
            if (!entities.IsNullOrEmpty())
            {
                // Set Source
                string sourceUuid;
                if (source != null)
                {
                    var chain = new TrakHoundSourceChain();
                    chain.Add(source);
                    chain.Add(SourceChain);
                    sourceUuid = chain.GetUuid();

                    entities.AddRange(TrakHoundSourceEntry.GetEntities(chain.GetEntry()));
                }
                else
                {
                    sourceUuid = SourceChain.GetUuid();
                }

                foreach (var entity in entities)
                {
                    if (entity != null && typeof(ITrakHoundSourcedEntity).IsAssignableFrom(entity.GetType()))
                    {
                        ((ITrakHoundSourcedEntity)entity).SetSource(sourceUuid, true);
                    }
                }

                var validEntities = entities.Where(o => o.IsValid);
                if (!validEntities.IsNullOrEmpty())
                {
                    if (!async)
                    {
                        // Publish to the Client Synchronously

                        if (await Client.System.Entities.Publish(validEntities, TrakHoundOperationMode.Sync, routerId) &&
                            (indexes.IsNullOrEmpty() || await Client.System.Entities.Objects.UpdateIndex(indexes, TrakHoundOperationMode.Sync, routerId)))
                        {
                            return true;
                        }
                    }
                    else
                    {
                        // Use Batches to batch publish Asynchronously

                        var batchClient = new TrakHoundBatchClient(Client, routerId: routerId);
                        await batchClient.Add(validEntities);
                        await batchClient.Add(indexes);
                        return await batchClient.Flush();
                    }
                }
            }

            return false;
        }
    }
}
