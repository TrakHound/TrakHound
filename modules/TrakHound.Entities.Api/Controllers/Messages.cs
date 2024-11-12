// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TrakHound.Api;
using TrakHound.Clients;
using TrakHound.Entities.Collections;
using TrakHound.Messages;
using TrakHound.Requests;

namespace TrakHound.Entities.Api
{
    [TrakHoundApiController("objects/message")]
    public class Messages : EntitiesApiControllerBase
    {
        private const int _defaultQos = 0;


        public Messages() { }

        public Messages(ITrakHoundApiConfiguration configuration, ITrakHoundClient client)
        {
            SetConfiguration(configuration);
            SetClient(client);
        }


        [TrakHoundApiQuery("mappings")]
        public async Task<TrakHoundApiResponse> GetMessageMappings(
            [FromQuery] string objectPath,
            [FromBody(ContentType = "application/json")] IEnumerable<string> objectPaths,
            [FromQuery] string routerId = null
            )
        {
            if (!string.IsNullOrEmpty(objectPath) || !objectPaths.IsNullOrEmpty())
            {
                var paths = !string.IsNullOrEmpty(objectPath) ? new string[] { objectPath } : objectPaths;

                var entities = await Client.System.Entities.Objects.Message.QueryByObject(paths, routerId);
                if (!entities.IsNullOrEmpty())
                {
                    var objectUuids = entities.Select(o => o.ObjectUuid).Distinct();
                    var objects = await Client.System.Entities.Objects.ReadByUuid(objectUuids, routerId);
                    var dObjects = objects?.ToDictionary(o => o.Uuid);

                    var results = new List<TrakHoundMessageMapping>();
                    foreach (var entity in entities)
                    {
                        var obj = dObjects?.GetValueOrDefault(entity.ObjectUuid);
                        if (obj != null)
                        {
                            var result = new TrakHoundMessageMapping();
                            result.Uuid = entity.Uuid;
                            result.Object = obj.GetAbsolutePath();
                            result.ObjectUuid = entity.ObjectUuid;
                            result.BrokerId = entity.BrokerId;
                            result.Topic = entity.Topic;
                            result.ContentType = entity.ContentType;
                            result.Retain = entity.Retain;
                            result.QoS = entity.Qos;
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
        public async Task<ITrakHoundConsumer<TrakHoundApiResponse>> SubscribeMessageContent(
           [FromQuery] string objectPath,
           [FromBody(ContentType = "application/json")] IEnumerable<string> objectPaths,
           [FromQuery] string clientId = null,
           [FromQuery] int qos = _defaultQos,
            [FromQuery] string routerId = null
           )
        {
            if (!string.IsNullOrEmpty(objectPath) || !objectPaths.IsNullOrEmpty())
            {
                var paths = !string.IsNullOrEmpty(objectPath) ? new string[] { objectPath } : objectPaths;

                var entities = await Client.System.Entities.Objects.Message.QueryByObject(paths, routerId);
                if (!entities.IsNullOrEmpty())
                {
                    var objectUuids = entities.Select(o => o.ObjectUuid).Distinct();
                    var objects = await Client.System.Entities.Objects.ReadByUuid(objectUuids, routerId);
                    var dObjects = objects?.ToDictionary(o => o.Uuid);

                    var brokerIds = entities.Select(o => o.BrokerId).Distinct();
                    foreach (var brokerId in brokerIds)
                    {
                        var topics = entities.Select(o => o.Topic).Distinct();

                        var messageConsumer = await Client.System.Messages.Subscribe(brokerId, clientId, topics, qos, routerId);
                        if (messageConsumer != null)
                        {
                            var outputConsumer = new TrakHoundConsumer<TrakHoundApiResponse>();

                            var resultConsumer = new TrakHoundConsumer<TrakHoundMessageResponse>(messageConsumer);
                            outputConsumer.Disposed += (s, args) => resultConsumer.Dispose();
                            resultConsumer.Received += (s, messageResponse) =>
                            {
                                var topicEntities = entities.Where(o => TrakHoundMessageTopics.IsMatch(o.Topic, messageResponse.Topic));
                                if (!topicEntities.IsNullOrEmpty())
                                {
                                    foreach (var entity in topicEntities)
                                    {
                                        var obj = dObjects?.GetValueOrDefault(entity.ObjectUuid);
                                        if (obj != null)
                                        {
                                            var parameters = new Dictionary<string, string>();
                                            parameters.Add("Object", obj.GetAbsolutePath());
                                            parameters.Add("BrokerId", messageResponse.BrokerId);
                                            parameters.Add("Topic", messageResponse.Topic);
                                            parameters.Add("Retain", messageResponse.Retain.ToString());
                                            parameters.Add("Qos", messageResponse.Qos.ToString());
                                            parameters.Add("Timestamp", messageResponse.Timestamp.ToDateTime().ToISO8601String());

                                            var contentType = !string.IsNullOrEmpty(entity.ContentType) ? entity.ContentType : "text/plain";

                                            var response = new TrakHoundApiResponse(200, messageResponse.Content, contentType);
                                            response.Parameters = parameters;

                                            outputConsumer.Push(response);
                                        }
                                    }
                                }
                            };

                            return outputConsumer;
                        }
                    }
                }
            }

            return null;
        }

        [TrakHoundApiPublish("mappings")]
        public async Task<TrakHoundApiResponse> PublishMessageMapping(
            [FromQuery] string objectPath,
            [FromQuery] string brokerId,
            [FromQuery] string topic,
            [FromQuery] string contentType = "application/octet-stream",
            [FromQuery] bool retain = false,
            [FromQuery] int qos = 0,
            [FromQuery] bool async = false,
            [FromQuery] string routerId = null
            )
        {
            if (!string.IsNullOrEmpty(objectPath) && !string.IsNullOrEmpty(brokerId))
            {
                var now = UnixDateTime.Now;
                var publishCollection = new TrakHoundEntityCollection();

                if (string.IsNullOrEmpty(topic)) topic = objectPath.Trim(TrakHoundPath.PathSeparator);

                var entry = new TrakHoundMessageMappingEntry();
                entry.ObjectPath = objectPath;
                entry.BrokerId = brokerId;
                entry.Topic = topic;
                entry.ContentType = contentType;
                entry.Retain = retain;
                entry.QoS = qos;

                var entries = new TrakHoundMessageMappingEntry[] { entry };

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
        public async Task<TrakHoundApiResponse> PublishMessageMappings(
            [FromBody("application/json")] IEnumerable<TrakHoundMessageMappingEntry> entries,
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

        [TrakHoundApiPublish("content")]
        public async Task<TrakHoundApiResponse> PublishMessage(
            [FromQuery] string objectPath,
            [FromBody("application/octet-stream")] Stream content,
            [FromQuery] string routerId = null
            )
        {
            if (!string.IsNullOrEmpty(objectPath) && TrakHoundPath.IsAbsolute(objectPath) && content != null)
            {
                var now = UnixDateTime.Now;

                var entity = (await Client.System.Entities.Objects.Message.QueryByObject(objectPath, routerId))?.FirstOrDefault();
                if (entity != null)
                {
                    // Publish Message Content
                    if (await Client.System.Messages.Publish(entity.BrokerId, entity.Topic, content, entity.Retain, entity.Qos, routerId))
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

        [TrakHoundApiQuery("brokers")]
        public async Task<TrakHoundApiResponse> GetMessageBrokers(
            [FromQuery] string routerId = null
            )
        {
            var brokers = await Client.System.Messages.QueryBrokers();
            if (!brokers.IsNullOrEmpty())
            {
                return Ok(brokers);
            }
            else
            {
                return NotFound();
            }
        }

        [TrakHoundApiQuery("brokers/{brokerId}")]
        public async Task<TrakHoundApiResponse> GetMessageBroker(
            [FromRoute] string brokerId,
            [FromQuery] string routerId = null
            )
        {
            if (!string.IsNullOrEmpty(brokerId))
            {
                var brokerIds = new string[] { brokerId };
                var broker = (await Client.System.Messages.QueryBrokersById(brokerIds))?.FirstOrDefault();
                if (broker != null)
                {
                    return Ok(broker);
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

        [TrakHoundApiQuery("senders")]
        public async Task<TrakHoundApiResponse> GetMessageSenders(
            [FromQuery] string routerId = null
            )
        {
            var senders = await Client.System.Messages.QuerySenders();
            if (!senders.IsNullOrEmpty())
            {
                return Ok(senders);
            }
            else
            {
                return NotFound();
            }
        }

        [TrakHoundApiQuery("senders/{senderId}")]
        public async Task<TrakHoundApiResponse> GetMessageSender(
            [FromRoute] string senderId,
            [FromQuery] string routerId = null
            )
        {
            if (!string.IsNullOrEmpty(senderId))
            {
                var senderIds = new string[] { senderId };
                var sender = (await Client.System.Messages.QuerySendersById(senderIds))?.FirstOrDefault();
                if (sender != null)
                {
                    return Ok(sender);
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

        public void CreateEntities(TrakHoundEntityCollection collection, IEnumerable<TrakHoundMessageMappingEntry> entries, long created = 0)
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
                        var topic = !string.IsNullOrEmpty(entry.Topic) ? entry.Topic : entry.ObjectPath.Trim(TrakHoundPath.PathSeparator);
                        var contentType = !string.IsNullOrEmpty(entry.ContentType) ? entry.ContentType : "text/plain";

                        collection.Add(new TrakHoundObjectMessageEntity(objectUuid, entry.BrokerId, topic, contentType, entry.Retain, entry.QoS, created: ts));
                    }
                }
            }
        }
    }
}
