// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TrakHound.Api;
using TrakHound.Clients;
using TrakHound.Entities.Collections;
using TrakHound.Requests;

namespace TrakHound.Entities.Api
{
    [TrakHoundApiController("objects/blob")]
    public class Blobs : EntitiesApiControllerBase
    {
        public Blobs() { }

        public Blobs(ITrakHoundApiConfiguration configuration, ITrakHoundClient client)
        {
            SetConfiguration(configuration);
            SetClient(client);
        }


        [TrakHoundApiQuery]
        public async Task<TrakHoundApiResponse> GetBlobs(
            [FromQuery] string objectPath,
            [FromBody(ContentType = "application/json")] IEnumerable<string> objectPaths,
            [FromQuery] string routerId = null
            )
        {
            if (!string.IsNullOrEmpty(objectPath) || !objectPaths.IsNullOrEmpty())
            {
                var paths = !string.IsNullOrEmpty(objectPath) ? new string[] { objectPath } : objectPaths;

                var entities = (await Client.System.Entities.Objects.Blob.QueryByObject(paths, routerId));
                if (!entities.IsNullOrEmpty())
                {
                    var objectUuids = entities.Select(o => o.ObjectUuid).Distinct();
                    var objects = await Client.System.Entities.Objects.ReadByUuid(objectUuids, routerId);
                    var dObjects = objects?.ToDictionary(o => o.Uuid);

                    var results = new List<TrakHoundBlob>();
                    foreach (var entity in entities)
                    {
                        var obj = dObjects?.GetValueOrDefault(entity.ObjectUuid);
                        if (obj != null)
                        {
                            var result = new TrakHoundBlob();
                            result.Uuid = entity.Uuid;
                            result.Object = obj.GetAbsolutePath();
                            result.ObjectUuid = entity.ObjectUuid;
                            result.BlobId = entity.BlobId;
                            result.ContentType = entity.ContentType;
                            result.Size = entity.Size;
                            result.Filename = entity.Filename;
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

        [TrakHoundApiQuery("download")]
        public async Task<TrakHoundApiResponse> DownloadBlob(
            [FromQuery] string objectPath,
            [FromQuery] string routerId = null
            )
        {
            if (!string.IsNullOrEmpty(objectPath))
            {
                if (TrakHoundPath.IsAbsolute(objectPath))
                {
                    var objectUuid = TrakHoundPath.GetUuid(objectPath);
                    var entity = await Client.System.Entities.Objects.Blob.QueryByObjectUuid(objectUuid, routerId);
                    if (entity != null)
                    {
                        var stream = await Client.System.Blobs.Read(objectUuid, routerId);
                        if (stream != null)
                        {
                            var response = new TrakHoundApiResponse();
                            response.Success = true;
                            response.StatusCode = 200;
                            response.ContentType = entity.ContentType;
                            response.Content = stream;
                            return response;
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
                    return BadRequest("'objectPath' Invalid : Path is not Absolute");
                }
            }
            else
            {
                return BadRequest();
            }
        }

        [TrakHoundApiPublish]
        public async Task<TrakHoundApiResponse> PublishBlob(
            [FromQuery] string objectPath,
            [FromQuery] string contentType,
            [FromQuery] string blobId = null,
            [FromQuery] long size = 0,
            [FromQuery] string filename = null,
            [FromBody("application/octet-stream")] Stream content = null,
            [FromQuery] bool async = false,
            [FromQuery] string routerId = null
            )
        {
            var response = new TrakHoundApiResponse();

            if (!string.IsNullOrEmpty(objectPath) && !string.IsNullOrEmpty(contentType))
            {
                var now = UnixDateTime.Now;
                var publishCollection = new TrakHoundEntityCollection();

                if (string.IsNullOrEmpty(blobId)) blobId = TrakHoundPath.GetUuid(objectPath);

                if (content != null)
                {
                    var entry = new TrakHoundBlobEntry();
                    entry.ObjectPath = objectPath;
                    entry.BlobId = blobId;
                    entry.ContentType = contentType;
                    entry.Size = content.Length;
                    entry.Filename = filename;

                    var entries = new TrakHoundBlobEntry[] { entry };

                    // Create Objects that are referenced by Path in entries
                    CreateEntities(publishCollection, entries, now);

                    // Publish Entities
                    if (await PublishEntities(publishCollection, now, async, routerId: routerId))
                    {
                        // Publish Blob Content
                        if (await Client.System.Blobs.Publish(blobId, content, routerId))
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
                        return InternalError();
                    }
                }
                else if (size > 0)
                {
                    var entry = new TrakHoundBlobEntry();
                    entry.ObjectPath = objectPath;
                    entry.BlobId = blobId;
                    entry.ContentType = contentType;
                    entry.Size = size;
                    entry.Filename = filename;

                    var entries = new TrakHoundBlobEntry[] { entry };

                    // Create Objects that are referenced by Path in entries
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
            }
            else
            {
                response.StatusCode = 400;
            }

            return response;
        }

        [TrakHoundApiPublish("batch")]
        public async Task<TrakHoundApiResponse> PublishBlobs(
            [FromBody("application/json")] IEnumerable<TrakHoundBlobEntry> entries,
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

        public void CreateEntities(TrakHoundEntityCollection collection, IEnumerable<TrakHoundBlobEntry> entries, long created = 0)
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
                        // Add Entity to the publish list
                        collection.Add(new TrakHoundObjectBlobEntity(objectUuid, entry.BlobId, entry.ContentType, entry.Size, entry.Filename, created: ts));
                    }
                }
            }
        }

        [TrakHoundApiDelete]
        public async Task<TrakHoundApiResponse> DeleteBlobs(
            [FromQuery] string objectPath,
            [FromBody(ContentType = "application/json")] IEnumerable<string> objectPaths,
            [FromQuery] string routerId = null
            )
        {
            if (!string.IsNullOrEmpty(objectPath) || !objectPaths.IsNullOrEmpty())
            {
                var paths = !string.IsNullOrEmpty(objectPath) ? new string[] { objectPath } : objectPaths;

                var entities = (await Client.System.Entities.Objects.Blob.QueryByObject(paths, routerId));
                if (!entities.IsNullOrEmpty())
                {
                    var blobIds = entities.Select(o => o.BlobId).Distinct();
                    foreach (var blobId in blobIds)
                    {
                        await Client.System.Blobs.Delete(blobId, routerId);
                    }

                    var uuids = entities.Select(o => o.Uuid).Distinct();
                    if (await Client.System.Entities.Objects.Blob.Delete(uuids, TrakHoundOperationMode.Sync, routerId))
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
