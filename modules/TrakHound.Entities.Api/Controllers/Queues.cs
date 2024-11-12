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
    [TrakHoundApiController("objects/queue")]
    public class Queues : EntitiesApiControllerBase
    {
        public Queues() { }

        public Queues(ITrakHoundApiConfiguration configuration, ITrakHoundClient client)
        {
            SetConfiguration(configuration);
            SetClient(client);
        }


        [TrakHoundApiQuery]
        public async Task<TrakHoundApiResponse> GetQueues(
            [FromQuery] string queuePath,
            [FromQuery] long skip = 0,
            [FromQuery] long take = 1000,
            [FromQuery] string routerId = null
            )
        {
            if (!string.IsNullOrEmpty(queuePath))
            {
                var queueObjects = await Client.System.Entities.Objects.Query(queuePath, routerId: routerId);
                if (!queueObjects.IsNullOrEmpty())
                {
                    var queueUuids = queueObjects.Select(o => o.Uuid);
                    var dQueueObjects = queueObjects?.ToDictionary(o => o.Uuid);

                    var entities = await Client.System.Entities.Objects.Queue.QueryByQueueUuid(queueUuids, skip, take, routerId: routerId);
                    if (!entities.IsNullOrEmpty())
                    {
                        var memberUuids = entities.Select(o => o.MemberUuid).Distinct();
                        var memberObjects = await Client.System.Entities.Objects.ReadByUuid(memberUuids, routerId);
                        var dMemberObjects = memberObjects?.ToDictionary(o => o.Uuid);

                        var results = new List<TrakHoundQueue>();
                        foreach (var entity in entities)
                        {
                            var queueObject = dQueueObjects?.GetValueOrDefault(entity.QueueUuid);
                            var memberObject = dMemberObjects?.GetValueOrDefault(entity.MemberUuid);

                            if (queueObject != null && memberObject != null)
                            {
                                var result = new TrakHoundQueue();
                                result.Uuid = entity.Uuid;
                                result.Queue = queueObject.GetAbsolutePath();
                                result.QueueUuid = entity.QueueUuid;
                                result.Index = entity.Index;
                                result.Member = memberObject.GetAbsolutePath();
                                result.MemberUuid = entity.MemberUuid;
                                result.Timestamp = entity.Timestamp.ToLocalDateTime();
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
                    return NotFound();
                }
            }
            else
            {
                return BadRequest();
            }
        }

        [TrakHoundApiSubscribe]
        public async Task<ITrakHoundConsumer<TrakHoundApiResponse>> SubscribeQueues(
            [FromQuery] string queuePath,
            [FromQuery] int interval = _defaultSubscribeInterval,
            [FromQuery] int take = _defaultSubscribeTake,
            [FromQuery] string routerId = null
            )
        {
            if (!string.IsNullOrEmpty(queuePath))
            {
                var consumer = await Client.System.Entities.Objects.Queue.Notify(queuePath, TrakHoundEntityNotificationType.ComponentChanged, routerId);
                if (consumer != null)
                {
                    var resultConsumer = new TrakHoundConsumer<IEnumerable<TrakHoundEntityNotification>, TrakHoundApiResponse>(consumer, allowNull: true);
                    resultConsumer.OnReceivedAsync = async (entities) =>
                    {
                        return await GetQueues(queuePath);
                    };

                    return resultConsumer;
                }
            }

            return null;
        }

        [TrakHoundApiQuery("pull")]
        public async Task<TrakHoundApiResponse> PullFromQueue(
            [FromQuery] string queuePath,
            [FromQuery] int count = 1,
            [FromQuery] string routerId = null
            )
        {
            if (!string.IsNullOrEmpty(queuePath))
            {
                if (TrakHoundPath.IsAbsolute(queuePath))
                {
                    var queueUuid = TrakHoundPath.GetUuid(queuePath);
                    var queueObject = await Client.System.Entities.Objects.ReadByUuid(queueUuid, routerId);
                    if (queueObject != null)
                    {
                        var entities = await Client.System.Entities.Objects.Queue.Pull(queueObject.Uuid, count, routerId);
                        if (!entities.IsNullOrEmpty())
                        {
                            var memberUuids = entities.Select(o => o.MemberUuid).Distinct();
                            var memberObjects = await Client.System.Entities.Objects.ReadByUuid(memberUuids, routerId);
                            var dMemberObjects = memberObjects?.ToDictionary(o => o.Uuid);

                            var results = new List<TrakHoundQueue>();
                            foreach (var entity in entities)
                            {
                                var memberObject = dMemberObjects?.GetValueOrDefault(entity.MemberUuid);
                                if (memberObject != null)
                                {
                                    var result = new TrakHoundQueue();
                                    result.Uuid = entity.Uuid;
                                    result.Queue = queueObject.GetAbsolutePath();
                                    result.QueueUuid = entity.QueueUuid;
                                    result.Index = entity.Index;
                                    result.Member = memberObject.GetAbsolutePath();
                                    result.MemberUuid = entity.MemberUuid;
                                    result.Timestamp = entity.Timestamp.ToLocalDateTime();
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
                        return NotFound();
                    }
                }
                else
                {
                    return BadRequest("'queuePath' Invalid : Path is not Absolute");
                }
            }
            else
            {
                return BadRequest();
            }
        }

        [TrakHoundApiPublish]
        public async Task<TrakHoundApiResponse> PublishQueue(
            [FromQuery] string queuePath,
            [FromQuery] string memberPath,
            [FromQuery] int index = 0,
            [FromQuery] bool async = false,
            [FromQuery] string routerId = null
            )
        {
            var response = new TrakHoundApiResponse();

            if (!string.IsNullOrEmpty(queuePath) && !string.IsNullOrEmpty(memberPath))
            {
                var entry = new TrakHoundQueueEntry();
                entry.QueuePath = queuePath;
                entry.MemberPath = memberPath;
                entry.Index = index;

                var entries = new TrakHoundQueueEntry[] { entry };

                return await PublishQueues(entries, async, routerId);
            }
            else
            {
                response.StatusCode = 400;
            }

            return response;
        }

        [TrakHoundApiPublish("batch")]
        public async Task<TrakHoundApiResponse> PublishQueues(
            [FromBody("application/json")] IEnumerable<TrakHoundQueueEntry> entries,
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

        public void CreateEntities(TrakHoundEntityCollection collection, IEnumerable<TrakHoundQueueEntry> entries, long created = 0)
        {
            if (!entries.IsNullOrEmpty())
            {
                var ts = created > 0 ? created : UnixDateTime.Now;

                // Create Objects that are referenced by Path in entries
                CreateContentObjects(collection, entries, ts);

                foreach (var entry in entries)
                {
                    if (TrakHoundPath.IsAbsolute(entry.MemberPath))
                    {
                        // Get the ObjectUuid
                        string queueUuid = TrakHoundPath.GetUuid(entry.QueuePath);
                        string memberUuid = TrakHoundPath.GetUuid(entry.MemberPath);

                        if (!string.IsNullOrEmpty(queueUuid))
                        {
                            collection.Add(new TrakHoundObjectQueueEntity(queueUuid, memberUuid, entry.Index, timestamp: ts, created: ts));
                        }
                    }
                }
            }
        }

        [TrakHoundApiDelete]
        public async Task<TrakHoundApiResponse> DeleteQueue(
            [FromQuery] string queuePath,
            [FromQuery] string memberPath,
            [FromQuery] bool async = false,
            [FromQuery] string routerId = null
            )
        {
            if (!string.IsNullOrEmpty(queuePath) && !string.IsNullOrEmpty(memberPath))
            {
                var groupObjects = await Client.System.Entities.Objects.Query(queuePath, routerId: routerId);
                var memberObjects = await Client.System.Entities.Objects.Query(memberPath, routerId: routerId);
                if (!groupObjects.IsNullOrEmpty() && !memberObjects.IsNullOrEmpty())
                {
                    var uuids = new List<string>();
                    foreach (var groupObject in groupObjects)
                    {
                        foreach (var memberObject in memberObjects)
                        {
                            uuids.Add(TrakHoundObjectQueueEntity.GenerateUuid(groupObject.Uuid, memberObject.Uuid));
                        }
                    }

                    var success = await Client.System.Entities.Objects.Queue.Delete(uuids, async ? TrakHoundOperationMode.Async : TrakHoundOperationMode.Sync, routerId);
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
