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
    [TrakHoundApiController("objects/group")]
    public class Groups : EntitiesApiControllerBase
    {
        public Groups() { }

        public Groups(ITrakHoundApiConfiguration configuration, ITrakHoundClient client)
        {
            SetConfiguration(configuration);
            SetClient(client);
        }


        [TrakHoundApiQuery]
        public async Task<TrakHoundApiResponse> GetGroups(
            [FromQuery] string groupPath,
            [FromBody(ContentType = "application/json")] IEnumerable<string> groupPaths,
            [FromQuery] long skip = 0,
            [FromQuery] long take = 1000,
            [FromQuery] string routerId = null
            )
        {
            if (!string.IsNullOrEmpty(groupPath) || !groupPaths.IsNullOrEmpty())
            {
                var paths = !string.IsNullOrEmpty(groupPath) ? new string[] { groupPath } : groupPaths;

                var entities = await Client.System.Entities.Objects.Group.QueryByGroup(paths, skip, take, routerId: routerId);
                if (!entities.IsNullOrEmpty())
                {
                    var objectUuids = new List<string>();
                    objectUuids.AddRange(entities.Select(o => o.GroupUuid));
                    objectUuids.AddRange(entities.Select(o => o.MemberUuid));

                    var objects = await Client.System.Entities.Objects.ReadByUuid(objectUuids.Distinct(), routerId);
                    var dObjects = objects?.ToDictionary(o => o.Uuid);

                    var results = new List<TrakHoundGroup>();

                    foreach (var entity in entities)
                    {
                        var group = dObjects?.GetValueOrDefault(entity.GroupUuid);
                        var member = dObjects?.GetValueOrDefault(entity.MemberUuid);

                        if (group != null && member != null)
                        {
                            var result = new TrakHoundGroup();
                            result.Uuid = entity.Uuid;
                            result.Group = group.GetAbsolutePath();
                            result.GroupUuid = entity.GroupUuid;
                            result.Member = member.GetAbsolutePath();
                            result.MemberUuid = entity.MemberUuid;
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

        [TrakHoundApiSubscribe]
        public async Task<ITrakHoundConsumer<TrakHoundApiResponse>> SubscribeGroups(
            [FromQuery] string groupPath,
            [FromQuery] int interval = _defaultSubscribeInterval,
            [FromQuery] int take = _defaultSubscribeTake,
            [FromQuery] string routerId = null
            )
        {
            if (!string.IsNullOrEmpty(groupPath))
            {
                var groupPaths = new string[] { groupPath };
                var consumer = await Client.System.Entities.Objects.Group.SubscribeByGroup(groupPaths, interval, take, routerId: routerId);
                if (consumer != null)
                {
                    var resultConsumer = new TrakHoundConsumer<IEnumerable<ITrakHoundObjectGroupEntity>, TrakHoundApiResponse>(consumer);
                    resultConsumer.OnReceivedAsync = async (entities) =>
                    {
                        var objectUuids = new List<string>();
                        objectUuids.AddRange(entities.Select(o => o.GroupUuid));
                        objectUuids.AddRange(entities.Select(o => o.MemberUuid));

                        var objects = await Client.System.Entities.Objects.ReadByUuid(objectUuids.Distinct(), routerId);
                        var dObjects = objects?.ToDictionary(o => o.Uuid);

                        var results = new List<TrakHoundGroup>();
                        foreach (var entity in entities)
                        {
                            var group = dObjects?.GetValueOrDefault(entity.GroupUuid);
                            var member = dObjects?.GetValueOrDefault(entity.MemberUuid);

                            if (group != null && member != null)
                            {
                                var result = new TrakHoundGroup();
                                result.Uuid = entity.Uuid;
                                result.Group = group.GetAbsolutePath();
                                result.GroupUuid = entity.GroupUuid;
                                result.Member = member.GetAbsolutePath();
                                result.MemberUuid = entity.MemberUuid;
                                result.SourceUuid = entity.SourceUuid;
                                result.Created = entity.Created.ToLocalDateTime();
                                results.Add(result);
                            }
                        }
                        return TrakHoundApiJsonResponse.Ok(results);
                    };

                    return resultConsumer;
                }
            }

            return null;
        }

        [TrakHoundApiSubscribe("member")]
        public async Task<ITrakHoundConsumer<TrakHoundApiResponse>> SubscribeGroupsByMember(
            [FromQuery] string memberPath,
            [FromQuery] int interval = _defaultSubscribeInterval,
            [FromQuery] int take = _defaultSubscribeTake,
            [FromQuery] string routerId = null
            )
        {
            if (!string.IsNullOrEmpty(memberPath))
            {
                var memberPaths = new string[] { memberPath };
                var consumer = await Client.System.Entities.Objects.Group.SubscribeByMember(memberPaths, interval, take, routerId: routerId);
                if (consumer != null)
                {
                    var resultConsumer = new TrakHoundConsumer<IEnumerable<ITrakHoundObjectGroupEntity>, TrakHoundApiResponse>(consumer);
                    resultConsumer.OnReceivedAsync = async (entities) =>
                    {
                        var objectUuids = new List<string>();
                        objectUuids.AddRange(entities.Select(o => o.GroupUuid));
                        objectUuids.AddRange(entities.Select(o => o.MemberUuid));

                        var objects = await Client.System.Entities.Objects.ReadByUuid(objectUuids.Distinct(), routerId);
                        var dObjects = objects?.ToDictionary(o => o.Uuid);

                        var results = new List<TrakHoundGroup>();
                        foreach (var entity in entities)
                        {
                            var group = dObjects?.GetValueOrDefault(entity.GroupUuid);
                            var member = dObjects?.GetValueOrDefault(entity.MemberUuid);

                            if (group != null && member != null)
                            {
                                var result = new TrakHoundGroup();
                                result.Uuid = entity.Uuid;
                                result.Group = group.GetAbsolutePath();
                                result.GroupUuid = entity.GroupUuid;
                                result.Member = member.GetAbsolutePath();
                                result.MemberUuid = entity.MemberUuid;
                                result.SourceUuid = entity.SourceUuid;
                                result.Created = entity.Created.ToLocalDateTime();
                                results.Add(result);
                            }
                        }
                        return TrakHoundApiJsonResponse.Ok(results);
                    };

                    return resultConsumer;
                }
            }

            return null;
        }


        [TrakHoundApiPublish]
        public async Task<TrakHoundApiResponse> PublishGroup(
            [FromQuery] string groupPath,
            [FromQuery] string memberPath,
            [FromQuery] bool async = false,
            [FromQuery] string routerId = null
            )
        {
            var response = new TrakHoundApiResponse();

            if (!string.IsNullOrEmpty(groupPath) && !string.IsNullOrEmpty(memberPath))
            {
                var entry = new TrakHoundGroupEntry();
                entry.GroupPath = groupPath;
                entry.MemberPath = memberPath;

                var entries = new TrakHoundGroupEntry[] { entry };

                return await PublishGroups(entries, async, routerId);
            }
            else
            {
                response.StatusCode = 400;
            }

            return response;
        }

        [TrakHoundApiPublish("batch")]
        public async Task<TrakHoundApiResponse> PublishGroups(
            [FromBody("application/json")] IEnumerable<TrakHoundGroupEntry> entries,
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

        public void CreateEntities(TrakHoundEntityCollection collection, IEnumerable<TrakHoundGroupEntry> entries, long created = 0)
        {
            if (!entries.IsNullOrEmpty())
            {
                var ts = created > 0 ? created : UnixDateTime.Now;

                // Create Objects that are referenced by Path in entries
                CreateContentObjects(collection, entries, ts);

                foreach (var entry in entries)
                {
                    string groupUuid = TrakHoundPath.GetUuid(entry.GroupPath);
                    string memberUuid = TrakHoundPath.GetUuid(entry.MemberPath);

                    if (!string.IsNullOrEmpty(groupUuid) && !string.IsNullOrEmpty(memberUuid))
                    {
                        // Add Entity to the publish list
                        collection.Add(new TrakHoundObjectGroupEntity(groupUuid, memberUuid, created: ts));
                    }
                }
            }
        }

        [TrakHoundApiDelete]
        public async Task<TrakHoundApiResponse> DeleteGroup(
            [FromQuery] string groupPath,
            [FromQuery] string memberPath,
            [FromQuery] bool async = false,
            [FromQuery] string routerId = null
            )
        {
            if (!string.IsNullOrEmpty(groupPath) && !string.IsNullOrEmpty(memberPath))
            {
                string groupUuid = TrakHoundPath.GetUuid(groupPath);
                string memberUuid = TrakHoundPath.GetUuid(memberPath);
                var uuid = TrakHoundObjectGroupEntity.GenerateUuid(groupUuid, memberUuid);

                var success = await Client.System.Entities.Objects.Group.Delete(uuid, async ? TrakHoundOperationMode.Async : TrakHoundOperationMode.Sync, routerId);
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
                return BadRequest();
            }
        }
    }
}
