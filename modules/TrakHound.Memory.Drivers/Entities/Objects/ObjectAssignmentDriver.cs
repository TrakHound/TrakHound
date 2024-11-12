// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Drivers.Entities;
using TrakHound.Entities;
using TrakHound.Memory;

namespace TrakHound.Drivers.Memory
{
    public class ObjectAssignmentDriver : MemoryEntityDriver<ITrakHoundObjectAssignmentEntity>,
        //IObjectAssignmentCurrentDriver,
        IObjectAssignmentSubscribeDriver
        //IObjectAssignmentEmptyDriver
    {
        private readonly Dictionary<string, IEnumerable<ITrakHoundObjectAssignmentEntity>> _assignmentsByAssignee = new Dictionary<string, IEnumerable<ITrakHoundObjectAssignmentEntity>>();
        private readonly Dictionary<string, IEnumerable<ITrakHoundObjectAssignmentEntity>> _assignmentsByMember = new Dictionary<string, IEnumerable<ITrakHoundObjectAssignmentEntity>>();
        private readonly HashSet<string> _emptyByAssignee = new HashSet<string>();
        private readonly HashSet<string> _emptyByMember = new HashSet<string>();


        public ObjectAssignmentDriver(ITrakHoundDriverConfiguration configuration) : base(configuration) { }


        //protected override bool PublishCompare(ITrakHoundObjectAssignmentEntity newEntity, ITrakHoundObjectAssignmentEntity existingEntity)
        //{
        //    return newEntity.AddTimestamp > _initializedTimestamp || newEntity.RemoveTimestamp > _initializedTimestamp;
        //}

        //protected override bool PublishCompare(ITrakHoundObjectAssignmentEntity newEntity, ITrakHoundObjectAssignmentEntity existingEntity)
        //{
        //    return existingEntity != null ? newEntity.AddTimestamp != existingEntity.AddTimestamp || newEntity.RemoveTimestamp != existingEntity.RemoveTimestamp : true;
        //}


        protected override TrakHoundPublishResult<ITrakHoundObjectAssignmentEntity> OnPublish(ITrakHoundObjectAssignmentEntity assignment)
        {
            if (assignment != null)
            {
                lock (_lock)
                {
                    var newObjectAssignments = new List<ITrakHoundObjectAssignmentEntity>();

                    // Add Existing
                    _assignmentsByAssignee.TryGetValue(assignment.AssigneeUuid, out var existingAssignments);
                    if (!existingAssignments.IsNullOrEmpty())
                    {
                        foreach (var existingAssignment in existingAssignments)
                        {
                            //if (existingAssignment.AssigneeUuid != assignment.AssigneeUuid && existingAssignment.ObjectUuid != assignment.ObjectUuid)
                            if (existingAssignment.AssigneeUuid != assignment.AssigneeUuid || existingAssignment.MemberUuid != assignment.MemberUuid)
                            {
                                newObjectAssignments.Add(existingAssignment);
                            }
                        }
                    }

                    if (assignment.RemoveTimestamp < 1) newObjectAssignments.Add(assignment);

                    _emptyByAssignee.Remove(assignment.AssigneeUuid);
                    _assignmentsByAssignee.Remove(assignment.AssigneeUuid);
                    _assignmentsByAssignee.Add(assignment.AssigneeUuid, newObjectAssignments);


                    var newMemberAssignments = new List<ITrakHoundObjectAssignmentEntity>();

                    // Add Existing
                    _assignmentsByMember.TryGetValue(assignment.MemberUuid, out existingAssignments);
                    if (!existingAssignments.IsNullOrEmpty())
                    {
                        foreach (var existingAssignment in existingAssignments)
                        {
                            //if (existingAssignment.AssigneeUuid != assignment.AssigneeUuid && existingAssignment.ObjectUuid != assignment.ObjectUuid)
                            if (existingAssignment.AssigneeUuid != assignment.AssigneeUuid || existingAssignment.MemberUuid != assignment.MemberUuid)
                            {
                                newMemberAssignments.Add(existingAssignment);
                            }
                        }
                    }

                    if (assignment.RemoveTimestamp < 1) newMemberAssignments.Add(assignment);

                    _emptyByMember.Remove(assignment.MemberUuid);
                    _assignmentsByMember.Remove(assignment.MemberUuid);
                    _assignmentsByMember.Add(assignment.MemberUuid, newMemberAssignments);

                    return base.OnPublish(assignment);
                }
            }

            return new TrakHoundPublishResult<ITrakHoundObjectAssignmentEntity>();
        }

        protected override bool OnDelete(EntityDeleteRequest request)
        {
            lock (_lock)
            {
                var entity = _entities.GetValueOrDefault(request.Target);
                if (entity != null)
                {
                    _assignmentsByAssignee.Remove(entity.AssigneeUuid);
                }
            }

            return base.OnDelete(request);
        }


        #region "Current"

        public async Task<TrakHoundResponse<ITrakHoundObjectAssignmentEntity>> CurrentByAssignee(IEnumerable<string> objectIds)
        {
            var stpw = System.Diagnostics.Stopwatch.StartNew();
            var results = new List<TrakHoundResult<ITrakHoundObjectAssignmentEntity>>();

            if (!objectIds.IsNullOrEmpty())
            {
                foreach (var objectId in objectIds)
                {
                    if (!string.IsNullOrEmpty(objectId))
                    {
                        lock (_lock)
                        {
                            if (_assignmentsByAssignee.TryGetValue(objectId, out var assignments))
                            {
                                if (!assignments.IsNullOrEmpty())
                                {
                                    // Update Last Accessed
                                    _accessed.Remove(objectId);
                                    _accessed.Add(objectId, UnixDateTime.Now);

                                    foreach (var assignment in assignments)
                                    {
                                        results.Add(new TrakHoundResult<ITrakHoundObjectAssignmentEntity>(Id, objectId, TrakHoundResultType.Ok, assignment));
                                    }
                                }
                            }
                            else if (_emptyByAssignee.Contains(objectId))
                            {
                                // Update Last Accessed
                                _accessed.Remove(objectId);
                                _accessed.Add(objectId, UnixDateTime.Now);

                                results.Add(new TrakHoundResult<ITrakHoundObjectAssignmentEntity>(Id, objectId, TrakHoundResultType.Empty));
                            }
                            else
                            {
                                results.Add(new TrakHoundResult<ITrakHoundObjectAssignmentEntity>(Id, objectId, TrakHoundResultType.NotFound));
                            }
                        }
                    }
                    else
                    {
                        results.Add(new TrakHoundResult<ITrakHoundObjectAssignmentEntity>(Id, null, TrakHoundResultType.BadRequest));
                    }
                }
            }
            else
            {
                results.Add(new TrakHoundResult<ITrakHoundObjectAssignmentEntity>(Id, null, TrakHoundResultType.BadRequest));
            }

            stpw.Stop();
            return new TrakHoundResponse<ITrakHoundObjectAssignmentEntity>(results, stpw.ElapsedTicks);
        }


        public async Task<TrakHoundResponse<ITrakHoundObjectAssignmentEntity>> CurrentByMember(IEnumerable<string> memberIds)
        {
            var stpw = System.Diagnostics.Stopwatch.StartNew();
            var results = new List<TrakHoundResult<ITrakHoundObjectAssignmentEntity>>();

            if (!memberIds.IsNullOrEmpty())
            {
                foreach (var memberId in memberIds)
                {
                    if (!string.IsNullOrEmpty(memberId))
                    {
                        lock (_lock)
                        {
                            if (_assignmentsByMember.TryGetValue(memberId, out var assignments))
                            {
                                if (!assignments.IsNullOrEmpty())
                                {
                                    // Update Last Accessed
                                    _accessed.Remove(memberId);
                                    _accessed.Add(memberId, UnixDateTime.Now);

                                    foreach (var assignment in assignments)
                                    {
                                        results.Add(new TrakHoundResult<ITrakHoundObjectAssignmentEntity>(Id, memberId, TrakHoundResultType.Ok, assignment));
                                    }
                                }
                            }
                            else if (_emptyByMember.Contains(memberId))
                            {
                                // Update Last Accessed
                                _accessed.Remove(memberId);
                                _accessed.Add(memberId, UnixDateTime.Now);

                                results.Add(new TrakHoundResult<ITrakHoundObjectAssignmentEntity>(Id, memberId, TrakHoundResultType.Empty));
                            }
                            else
                            {
                                results.Add(new TrakHoundResult<ITrakHoundObjectAssignmentEntity>(Id, memberId, TrakHoundResultType.NotFound));
                            }
                        }
                    }
                    else
                    {
                        results.Add(new TrakHoundResult<ITrakHoundObjectAssignmentEntity>(Id, null, TrakHoundResultType.BadRequest));
                    }
                }
            }
            else
            {
                results.Add(new TrakHoundResult<ITrakHoundObjectAssignmentEntity>(Id, null, TrakHoundResultType.BadRequest));
            }

            stpw.Stop();
            return new TrakHoundResponse<ITrakHoundObjectAssignmentEntity>(results, stpw.ElapsedTicks);
        }

        #endregion

        #region "Subscribe"

        public async Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectAssignmentEntity>>>> SubscribeByAssignee(IEnumerable<string> objectUuids)
        {
            var consumer = new MemoryConsumer<IEnumerable<ITrakHoundObjectAssignmentEntity>>(objectUuids, ProcessAssigneeMessage);
            _publishConsumers.Add(consumer.Id, consumer);

            var results = new List<TrakHoundResult<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectAssignmentEntity>>>>();
            results.Add(new TrakHoundResult<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectAssignmentEntity>>>(Id, "All", TrakHoundResultType.Ok, consumer));

            return new TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectAssignmentEntity>>>(results);
        }

        private static IEnumerable<ITrakHoundObjectAssignmentEntity> ProcessAssigneeMessage(IEnumerable<string> keys, IEnumerable<ITrakHoundObjectAssignmentEntity> entities)
        {
            var resultEntities = new List<ITrakHoundObjectAssignmentEntity>();
            foreach (var entity in entities)
            {
                if (keys.Contains(entity.AssigneeUuid))
                {
                    resultEntities.Add(entity);
                }
            }
            return resultEntities;
        }


        public async Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectAssignmentEntity>>>> SubscribeByMember(IEnumerable<string> objectUuids)
        {
            var consumer = new MemoryConsumer<IEnumerable<ITrakHoundObjectAssignmentEntity>>(objectUuids, ProcessMemberMessage);
            _publishConsumers.Add(consumer.Id, consumer);

            var results = new List<TrakHoundResult<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectAssignmentEntity>>>>();
            results.Add(new TrakHoundResult<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectAssignmentEntity>>>(Id, "All", TrakHoundResultType.Ok, consumer));

            return new TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectAssignmentEntity>>>(results);
        }

        private static IEnumerable<ITrakHoundObjectAssignmentEntity> ProcessMemberMessage(IEnumerable<string> keys, IEnumerable<ITrakHoundObjectAssignmentEntity> entities)
        {
            var resultEntities = new List<ITrakHoundObjectAssignmentEntity>();
            foreach (var entity in entities)
            {
                if (keys.Contains(entity.MemberUuid))
                {
                    resultEntities.Add(entity);
                }
            }
            return resultEntities;
        }

        #endregion

        #region "Empty"

        public async Task<TrakHoundResponse<bool>> EmptyAssignee(IEnumerable<EntityEmptyRequest> requests)
        {
            if (!requests.IsNullOrEmpty())
            {
                var stpw = System.Diagnostics.Stopwatch.StartNew();
                var results = new List<TrakHoundResult<bool>>();

                foreach (var request in requests)
                {
                    if (!string.IsNullOrEmpty(request.EntityUuid))
                    {
                        lock (_lock)
                        {
                            long latestTimestamp = 0;

                            var existingEntities = _assignmentsByAssignee.GetValueOrDefault(request.EntityUuid);
                            if (!existingEntities.IsNullOrEmpty())
                            {
                                latestTimestamp = existingEntities.Select(o => o.AddTimestamp).Max();
                            }

                            if (request.Timestamp > latestTimestamp && !_emptyByAssignee.Contains(request.EntityUuid))
                            {
                                _emptyByAssignee.Add(request.EntityUuid);

                                _updated.Remove(request.EntityUuid);
                                _updated.Add(request.EntityUuid, UnixDateTime.Now);
                            }
                        }

                        results.Add(new TrakHoundResult<bool>(Id, request.EntityUuid, TrakHoundResultType.Ok, true));
                    }
                }

                stpw.Stop();
                return new TrakHoundResponse<bool>(results, stpw.ElapsedTicks);
            }

            return TrakHoundResponse<bool>.InternalError(Id, null);
        }

        public async Task<TrakHoundResponse<bool>> EmptyMember(IEnumerable<EntityEmptyRequest> requests)
        {
            if (!requests.IsNullOrEmpty())
            {
                var stpw = System.Diagnostics.Stopwatch.StartNew();
                var results = new List<TrakHoundResult<bool>>();

                foreach (var request in requests)
                {
                    if (!string.IsNullOrEmpty(request.EntityUuid))
                    {
                        lock (_lock)
                        {
                            long latestTimestamp = 0;

                            var existingEntities = _assignmentsByMember.GetValueOrDefault(request.EntityUuid);
                            if (!existingEntities.IsNullOrEmpty())
                            {
                                latestTimestamp = existingEntities.Select(o => o.AddTimestamp).Max();
                            }

                            if (request.Timestamp > latestTimestamp && !_emptyByMember.Contains(request.EntityUuid))
                            {
                                _emptyByMember.Add(request.EntityUuid);

                                _updated.Remove(request.EntityUuid);
                                _updated.Add(request.EntityUuid, UnixDateTime.Now);
                            }
                        }

                        results.Add(new TrakHoundResult<bool>(Id, request.EntityUuid, TrakHoundResultType.Ok, true));
                    }
                }

                stpw.Stop();
                return new TrakHoundResponse<bool>(results, stpw.ElapsedTicks);
            }

            return TrakHoundResponse<bool>.InternalError(Id, null);
        }

        #endregion

    }
}
