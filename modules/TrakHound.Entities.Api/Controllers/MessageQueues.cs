// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TrakHound.Api;
using TrakHound.Clients;
using TrakHound.Entities.Collections;
using TrakHound.MessageQueues;
using TrakHound.Requests;

namespace TrakHound.Entities.Api
{
    [TrakHoundApiController("objects/message-queue")]
    public class MessageQueues : EntitiesApiControllerBase
    {
        public MessageQueues() { }

        public MessageQueues(ITrakHoundApiConfiguration configuration, ITrakHoundClient client)
        {
            SetConfiguration(configuration);
            SetClient(client);
        }


        [TrakHoundApiQuery("mappings")]
        public async Task<TrakHoundApiResponse> GetMessageQueueMappings(
            [FromQuery] string objectPath,
            [FromBody(ContentType = "application/json")] IEnumerable<string> objectPaths,
            [FromQuery] string routerId = null
            )
        {
            if (!string.IsNullOrEmpty(objectPath) || !objectPaths.IsNullOrEmpty())
            {
                var paths = !string.IsNullOrEmpty(objectPath) ? new string[] { objectPath } : objectPaths;

                var entities = await Client.System.Entities.Objects.MessageQueue.QueryByObject(paths, routerId);
                if (!entities.IsNullOrEmpty())
                {
                    var objectUuids = entities.Select(o => o.ObjectUuid).Distinct();
                    var objects = await Client.System.Entities.Objects.ReadByUuid(objectUuids, routerId);
                    var dObjects = objects?.ToDictionary(o => o.Uuid);

                    var results = new List<TrakHoundMessageQueueMapping>();
                    foreach (var entity in entities)
                    {
                        var obj = dObjects?.GetValueOrDefault(entity.ObjectUuid);
                        if (obj != null)
                        {
                            var result = new TrakHoundMessageQueueMapping();
                            result.Uuid = entity.Uuid;
                            result.Object = obj.GetAbsolutePath();
                            result.ObjectUuid = entity.ObjectUuid;
                            result.QueueId = entity.QueueId;
                            result.ContentType = entity.ContentType;
                            result.SourceUuid = entity.SourceUuid;
                            result.Created = entity.Created.ToDateTime();
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

        [TrakHoundApiSubscribe("content")]
        public async Task<ITrakHoundConsumer<TrakHoundApiResponse>> SubscribeMessageQueueContent(
            [FromQuery] string objectPath,
            [FromQuery] bool acknowledge = true,
            [FromQuery] string routerId = null
            )
        {
            if (!string.IsNullOrEmpty(objectPath))
            {
                if (TrakHoundPath.IsAbsolute(objectPath))
                {
                    var objectUuid = TrakHoundPath.GetUuid(objectPath);
                    var obj = await Client.System.Entities.Objects.ReadByUuid(objectUuid, routerId);
                    if (obj != null)
                    {
                        var entity = await Client.System.Entities.Objects.MessageQueue.QueryByObjectUuid(objectUuid, routerId);
                        if (entity != null)
                        {
                            var messageQueueConsumer = await Client.System.MessageQueues.Subscribe(entity.QueueId, acknowledge, routerId);
                            if (messageQueueConsumer != null)
                            {
                                var resultConsumer = new TrakHoundConsumer<TrakHoundMessageQueueResponse, TrakHoundApiResponse>(messageQueueConsumer);
                                resultConsumer.OnReceived = (messageQueueResponse) =>
                                {
                                    var parameters = new Dictionary<string, string>();
                                    parameters.Add("Object", obj.GetAbsolutePath());
                                    parameters.Add("DeliveryId", messageQueueResponse.DeliveryId);

                                    var response = new TrakHoundApiResponse(200, messageQueueResponse.Content, entity.ContentType);
                                    response.Parameters = parameters;

                                    return response;
                                };

                                return resultConsumer;
                            }
                        }
                    }
                }
            }

            return null;
        }

        [TrakHoundApiQuery("content/pull")]
        public async Task<TrakHoundApiResponse> PullContentFromMessageQueue(
            [FromQuery] string objectPath,
            [FromQuery] bool acknowledge = true,
            [FromQuery] string routerId = null
            )
        {
            if (!string.IsNullOrEmpty(objectPath))
            {
                if (TrakHoundPath.IsAbsolute(objectPath))
                {
                    var objectUuid = TrakHoundPath.GetUuid(objectPath);
                    var obj = await Client.System.Entities.Objects.ReadByUuid(objectUuid, routerId);
                    if (obj != null)
                    {
                        var entity = await Client.System.Entities.Objects.MessageQueue.QueryByObjectUuid(objectUuid, routerId);
                        if (entity != null)
                        {
                            var messageQueueResponse = await Client.System.MessageQueues.Pull(entity.QueueId, acknowledge, routerId);
                            if (messageQueueResponse.IsValid)
                            {
                                var parameters = new Dictionary<string, string>();
                                parameters.Add("Object", obj.GetAbsolutePath());
                                parameters.Add("QueueId", entity.QueueId);
                                parameters.Add("DeliveryId", messageQueueResponse.DeliveryId);

                                var response = new TrakHoundApiResponse(200, messageQueueResponse.Content, entity.ContentType);
                                response.Parameters = parameters;

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

        [TrakHoundApiPublish("content")]
        public async Task<TrakHoundApiResponse> PublishMessageQueue(
            [FromQuery] string objectPath,
            [FromBody("application/octet-stream")] Stream content,
            [FromQuery] string routerId = null
            )
        {
            if (!string.IsNullOrEmpty(objectPath) && TrakHoundPath.IsAbsolute(objectPath) && content != null)
            {
                var now = UnixDateTime.Now;

                var entity = (await Client.System.Entities.Objects.MessageQueue.QueryByObject(objectPath, routerId))?.FirstOrDefault();
                if (entity != null)
                {
                    // Publish Message Content
                    if (await Client.System.MessageQueues.Publish(entity.QueueId, content, routerId))
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
                    return NotFound();
                }
            }
            else
            {
                return BadRequest();
            }
        }


        [TrakHoundApiQuery("acknowledge")]
        public async Task<TrakHoundApiResponse> AcknowledgeMessageQueue(
            [FromQuery] string objectPath,
            [FromBody("application/json")] IEnumerable<string> deliveryIds,
            [FromQuery] string routerId = null
            )
        {
            if (!string.IsNullOrEmpty(objectPath) && !deliveryIds.IsNullOrEmpty())
            {
                if (TrakHoundPath.IsAbsolute(objectPath))
                {
                    var objectUuid = TrakHoundPath.GetUuid(objectPath);
                    if (!string.IsNullOrEmpty(objectUuid))
                    {
                        var entity = await Client.System.Entities.Objects.MessageQueue.QueryByObjectUuid(objectUuid, routerId);
                        if (entity != null)
                        {
                            var success = false;
                            foreach (var deliveryId in deliveryIds)
                            {
                                success = await Client.System.MessageQueues.Acknowledge(entity.QueueId, deliveryId, routerId);
                                if (!success) break;
                            }

                            if (success)
                            {
                                return Ok();
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
                        return BadRequest("Invalid Object Path");
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

        [TrakHoundApiQuery("reject")]
        public async Task<TrakHoundApiResponse> RejectMessageQueue(
            [FromQuery] string objectPath,
            [FromBody("application/json")] IEnumerable<string> deliveryIds,
            [FromQuery] string routerId = null
            )
        {
            if (!string.IsNullOrEmpty(objectPath) && !deliveryIds.IsNullOrEmpty())
            {
                if (TrakHoundPath.IsAbsolute(objectPath))
                {
                    var objectUuid = TrakHoundPath.GetUuid(objectPath);
                    if (!string.IsNullOrEmpty(objectUuid))
                    {
                        var entity = await Client.System.Entities.Objects.MessageQueue.QueryByObjectUuid(objectUuid, routerId);
                        if (entity != null)
                        {
                            var success = false;
                            foreach (var deliveryId in deliveryIds)
                            {
                                success = await Client.System.MessageQueues.Reject(entity.QueueId, deliveryId, routerId);
                                if (!success) break;
                            }

                            if (success)
                            {
                                return Ok();
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
                        return BadRequest("Invalid Object Path");
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

        [TrakHoundApiPublish("mappings")]
        public async Task<TrakHoundApiResponse> PublishMessageQueueMapping(
            [FromQuery] string objectPath,
            [FromQuery] string queueId,
            [FromQuery] string contentType = "application/octet-stream",
            [FromQuery] bool async = false,
            [FromQuery] string routerId = null
            )
        {
            if (!string.IsNullOrEmpty(objectPath) && !string.IsNullOrEmpty(queueId))
            {
                var now = UnixDateTime.Now;
                var publishCollection = new TrakHoundEntityCollection();

                var entry = new TrakHoundMessageQueueMappingEntry();
                entry.ObjectPath = objectPath;
                entry.QueueId = queueId;
                entry.ContentType = contentType;

                var entries = new TrakHoundMessageQueueMappingEntry[] { entry };

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
                return BadRequest();
            }
        }

        [TrakHoundApiPublish("mappings/batch")]
        public async Task<TrakHoundApiResponse> PublishMessageQueueMappings(
            [FromBody("application/json")] IEnumerable<TrakHoundMessageQueueMappingEntry> entries,
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

        public void CreateEntities(TrakHoundEntityCollection collection, IEnumerable<TrakHoundMessageQueueMappingEntry> entries, long created = 0)
        {
            if (!entries.IsNullOrEmpty())
            {
                var ts = created > 0 ? created : UnixDateTime.Now;

                // Create Objects that are referenced by Path in entries
                CreateContentObjects(collection, entries, ts);

                foreach (var entry in entries)
                {
                    string objectUuid = TrakHoundPath.GetUuid(entry.ObjectPath);
                    if (!string.IsNullOrEmpty(objectUuid) && !string.IsNullOrEmpty(entry.QueueId))
                    {
                        var contentType = !string.IsNullOrEmpty(entry.ContentType) ? entry.ContentType : "application/octet-stream";

                        collection.Add(new TrakHoundObjectMessageQueueEntity(objectUuid, entry.QueueId, contentType, created: ts));
                    }
                }
            }
        }
    }
}
