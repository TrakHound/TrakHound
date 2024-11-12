// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrakHound.Api;
using TrakHound.Clients;
using TrakHound.Entities.Collections;
using TrakHound.Requests;

namespace TrakHound.Entities.Api
{
    [TrakHoundApiController("objects/assignment")]
    public class Assignments : EntitiesApiControllerBase
    {
        public Assignments() { }

        public Assignments(ITrakHoundApiConfiguration configuration, ITrakHoundClient client)
        {
            SetConfiguration(configuration);
            SetClient(client);
        }


        [TrakHoundApiQuery]
        public async Task<TrakHoundApiResponse> GetAssignments(
            [FromQuery] string assigneePath,
            [FromBody(ContentType = "application/json")] IEnumerable<string> assigneePaths,
            [FromQuery] string start = null,
            [FromQuery] string stop = null,
            [FromQuery] long skip = 0,
            [FromQuery] long take = 1000,
            [FromQuery] string routerId = null
            )
        {
            if (!string.IsNullOrEmpty(assigneePath) || !assigneePaths.IsNullOrEmpty())
            {
                var paths = !string.IsNullOrEmpty(assigneePath) ? new string[] { assigneePath } : assigneePaths;

                var startTimestamp = start.ToDateTime().ToUnixTime();
                var stopTimestamp = stop.ToDateTime().ToUnixTime();

                IEnumerable<ITrakHoundObjectAssignmentEntity> entities;
                if (startTimestamp > 0 && stopTimestamp > 0)
                {
                    entities = await Client.System.Entities.Objects.Assignment.QueryByAssignee(paths, startTimestamp, stopTimestamp, skip, take, routerId: routerId);
                }
                else
                {
                    entities = await Client.System.Entities.Objects.Assignment.CurrentByAssignee(paths, routerId: routerId);
                }

                if (!entities.IsNullOrEmpty())
                {
                    var objectUuids = new List<string>();
                    objectUuids.AddRange(entities.Select(o => o.AssigneeUuid));
                    objectUuids.AddRange(entities.Select(o => o.MemberUuid));

                    var objects = await Client.System.Entities.Objects.ReadByUuid(objectUuids.Distinct(), routerId);
                    var dObjects = objects?.ToDictionary(o => o.Uuid);

                    var results = new List<TrakHoundAssignment>();

                    foreach (var entity in entities)
                    {
                        var assignee = dObjects?.GetValueOrDefault(entity.AssigneeUuid);
                        var member = dObjects?.GetValueOrDefault(entity.MemberUuid);

                        if (assignee != null && member != null)
                        {
                            var result = new TrakHoundAssignment();
                            result.Uuid = entity.Uuid;
                            result.Assignee = assignee.GetAbsolutePath();
                            result.AssigneeUuid = entity.AssigneeUuid;
                            result.Member = member.GetAbsolutePath();
                            result.MemberUuid = entity.MemberUuid;
                            result.AddTimestamp = entity.AddTimestamp.ToLocalDateTime();
                            result.RemoveTimestamp = entity.RemoveTimestamp > 0 ? entity.RemoveTimestamp.ToLocalDateTime() : null;
                            result.AddSourceUuid = entity.AddSourceUuid;
                            result.RemoveSourceUuid = entity.RemoveSourceUuid;
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
        public async Task<ITrakHoundConsumer<TrakHoundApiResponse>> SubscribeAssignments(
            [FromQuery] string assigneePath,
            [FromBody(ContentType = "application/json")] IEnumerable<string> assigneePaths,
            [FromQuery] int interval = _defaultSubscribeInterval,
            [FromQuery] int take = _defaultSubscribeTake,
            [FromQuery] string routerId = null
            )
        {
            if (!string.IsNullOrEmpty(assigneePath) || !assigneePaths.IsNullOrEmpty())
            {
                var paths = !string.IsNullOrEmpty(assigneePath) ? new string[] { assigneePath } : assigneePaths;

                var consumer = await Client.System.Entities.Objects.Assignment.SubscribeByAssignee(paths, interval, take, routerId);
                if (consumer != null)
                {
                    var resultConsumer = new TrakHoundConsumer<IEnumerable<ITrakHoundObjectAssignmentEntity>, TrakHoundApiResponse>(consumer);
                    resultConsumer.OnReceivedAsync = async (entities) =>
                    {
                        var objectUuids = new List<string>();
                        objectUuids.AddRange(entities.Select(o => o.AssigneeUuid));
                        objectUuids.AddRange(entities.Select(o => o.MemberUuid));

                        var objects = await Client.System.Entities.Objects.ReadByUuid(objectUuids.Distinct(), routerId);
                        var dObjects = objects?.ToDictionary(o => o.Uuid);

                        var results = new List<TrakHoundAssignment>();
                        foreach (var entity in entities)
                        {
                            var assignee = dObjects?.GetValueOrDefault(entity.AssigneeUuid);
                            var member = dObjects?.GetValueOrDefault(entity.MemberUuid);

                            if (assignee != null && member != null)
                            {
                                var result = new TrakHoundAssignment();
                                result.Uuid = entity.Uuid;
                                result.Assignee = assignee.GetAbsolutePath();
                                result.AssigneeUuid = entity.AssigneeUuid;
                                result.Member = member.GetAbsolutePath();
                                result.MemberUuid = entity.MemberUuid;
                                result.AddTimestamp = entity.AddTimestamp.ToLocalDateTime();
                                result.RemoveTimestamp = entity.RemoveTimestamp.ToLocalDateTime();
                                result.AddSourceUuid = entity.AddSourceUuid;
                                result.RemoveSourceUuid = entity.RemoveSourceUuid;
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
        public async Task<TrakHoundApiResponse> PublishAssignment(
            [FromQuery] string assigneePath,
            [FromQuery] string memberPath,
            [FromQuery] string addTimestamp = null,
            [FromQuery] string removeTimestamp = null,
            [FromQuery] bool async = false,
            [FromQuery] string routerId = null
            )
        {
            var response = new TrakHoundApiResponse();

            if (!string.IsNullOrEmpty(assigneePath) && !string.IsNullOrEmpty(memberPath))
            {
                var addTs = !string.IsNullOrEmpty(addTimestamp) ? addTimestamp.ToDateTime() : DateTime.Now;
                DateTime? removeTs = !string.IsNullOrEmpty(removeTimestamp) ? removeTimestamp.ToDateTime() : null;

                var entry = new TrakHoundAssignmentEntry();
                entry.AssigneePath = assigneePath;
                entry.MemberPath = memberPath;
                entry.AddTimestamp = addTs;
                entry.RemoveTimestamp = removeTs;

                var entries = new TrakHoundAssignmentEntry[] { entry };

                return await PublishAssignments(entries, async, routerId);
            }
            else
            {
                response.StatusCode = 400;
            }

            return response;
        }

        [TrakHoundApiPublish("remove")]
        public async Task<TrakHoundApiResponse> RemoveAssignment(
            [FromQuery] string assigneePath,
            [FromQuery] string memberPath,
            [FromQuery] string removeTimestamp = null,
            [FromQuery] bool async = false,
            [FromQuery] string routerId = null
            )
        {
            var response = new TrakHoundApiResponse();

            if (!string.IsNullOrEmpty(assigneePath) && !string.IsNullOrEmpty(memberPath))
            {
                var now = UnixDateTime.Now;
                var removeTs = !string.IsNullOrEmpty(removeTimestamp) ? removeTimestamp.ToDateTime().ToUnixTime() : now;
                var created = now;

                string assigneeUuid = TrakHoundPath.GetUuid(assigneePath);
                string memberUuid = TrakHoundPath.GetUuid(memberPath);

                if (!string.IsNullOrEmpty(assigneeUuid) && !string.IsNullOrEmpty(memberUuid))
                {
                    var assignments = await Client.System.Entities.Objects.Assignment.CurrentByAssigneeUuid(assigneeUuid, routerId);
                    if (!assignments.IsNullOrEmpty())
                    {
                        var memberAssignments = assignments.Where(o => o.MemberUuid == memberUuid);
                        if (!memberAssignments.IsNullOrEmpty())
                        {
                            var publishAssignments = new List<ITrakHoundObjectAssignmentEntity>();

                            foreach (var memberAssignment in memberAssignments)
                            {
                                var newMemberAssignment = new TrakHoundObjectAssignmentEntity(memberAssignment);
                                newMemberAssignment.RemoveTimestamp = removeTs;
                                newMemberAssignment.Created = created;
                                publishAssignments.Add(newMemberAssignment);
                            }

                            if (await Client.System.Entities.Objects.Assignment.Publish(publishAssignments, async ? TrakHoundOperationMode.Async : TrakHoundOperationMode.Sync, routerId))
                            {
                                response.StatusCode = 200;
                                response.Success = true;
                            }
                            else
                            {
                                response.StatusCode = 500;
                            }
                        }
                    }
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

        [TrakHoundApiPublish("batch")]
        public async Task<TrakHoundApiResponse> PublishAssignments(
            [FromBody("application/json")] IEnumerable<TrakHoundAssignmentEntry> entries,
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

        public void CreateEntities(TrakHoundEntityCollection collection, IEnumerable<TrakHoundAssignmentEntry> entries, long created = 0)
        {
            if (!entries.IsNullOrEmpty())
            {
                var ts = created > 0 ? created : UnixDateTime.Now;

                // Create Objects that are referenced by Path in entries
                CreateContentObjects(collection, entries, ts);

                foreach (var entry in entries)
                {
                    string assigneeUuid = TrakHoundPath.GetUuid(entry.AssigneePath);
                    string memberUuid = TrakHoundPath.GetUuid(entry.MemberPath);

                    if (!string.IsNullOrEmpty(assigneeUuid) && !string.IsNullOrEmpty(memberUuid))
                    {
                        var addTs = entry.AddTimestamp > DateTime.MinValue ? entry.AddTimestamp.ToUnixTime() : ts;
                        var removeTs = entry.RemoveTimestamp != null ? entry.RemoveTimestamp.Value.ToUnixTime() : 0;

                        collection.Add(new TrakHoundObjectAssignmentEntity(assigneeUuid, memberUuid, addTs, removeTimestamp: removeTs, created: ts));
                    }
                }
            }
        }
    }
}
