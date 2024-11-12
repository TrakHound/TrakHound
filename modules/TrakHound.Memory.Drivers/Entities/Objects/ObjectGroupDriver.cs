// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Drivers.Entities;
using TrakHound.Entities;
using TrakHound.Memory;

namespace TrakHound.Drivers.Memory
{
    public class ObjectGroupDriver : MemoryEntityDriver<ITrakHoundObjectGroupEntity>,
        IObjectGroupQueryDriver,
        IObjectGroupSubscribeDriver,
        IObjectGroupEmptyDriver
    {
        private readonly Dictionary<string, IEnumerable<ITrakHoundObjectGroupEntity>> _groupsByGroup = new Dictionary<string, IEnumerable<ITrakHoundObjectGroupEntity>>();
        private readonly Dictionary<string, IEnumerable<ITrakHoundObjectGroupEntity>> _groupsByMember = new Dictionary<string, IEnumerable<ITrakHoundObjectGroupEntity>>();
        private readonly HashSet<string> _emptyByGroup = new HashSet<string>();
        private readonly HashSet<string> _emptyByMember = new HashSet<string>();


        public ObjectGroupDriver(ITrakHoundDriverConfiguration configuration) : base(configuration) { }


        protected override TrakHoundPublishResult<ITrakHoundObjectGroupEntity> OnPublish(ITrakHoundObjectGroupEntity group)
        {
            if (group != null)
            {
                lock (_lock)
                {
                    var newObjectGroups = new List<ITrakHoundObjectGroupEntity>();

                    // Add Existing
                    _groupsByGroup.TryGetValue(group.GroupUuid, out var existingGroups);
                    if (!existingGroups.IsNullOrEmpty())
                    {
                        foreach (var existingGroup in existingGroups)
                        {
                            if (existingGroup.GroupUuid != group.GroupUuid || existingGroup.MemberUuid != group.MemberUuid)
                            {
                                newObjectGroups.Add(existingGroup);
                            }
                        }
                    }

                    newObjectGroups.Add(group);

                    _emptyByGroup.Remove(group.GroupUuid);
                    _groupsByGroup.Remove(group.GroupUuid);
                    _groupsByGroup.Add(group.GroupUuid, newObjectGroups.ToList());


                    var newMemberGroups = new List<ITrakHoundObjectGroupEntity>();

                    // Add Existing
                    _groupsByMember.TryGetValue(group.MemberUuid, out existingGroups);
                    if (!existingGroups.IsNullOrEmpty())
                    {
                        foreach (var existingGroup in existingGroups)
                        {
                            if (existingGroup.GroupUuid != group.GroupUuid || existingGroup.MemberUuid != group.MemberUuid)
                            {
                                newMemberGroups.Add(existingGroup);
                            }
                        }
                    }

                    newMemberGroups.Add(group);

                    _emptyByMember.Remove(group.MemberUuid);
                    _groupsByMember.Remove(group.MemberUuid);
                    _groupsByMember.Add(group.MemberUuid, newMemberGroups.ToList());

                    return base.OnPublish(group);
                }
            }

            return new TrakHoundPublishResult<ITrakHoundObjectGroupEntity>();
        }

        protected override bool OnDelete(EntityDeleteRequest request)
        {
            lock (_lock)
            {
                var entity = _entities.GetValueOrDefault(request.Target);
                if (entity != null)
                {
                    _groupsByGroup.Remove(entity.GroupUuid);
                }
            }

            return base.OnDelete(request);
        }

        protected override void OnDeleteAfter(IEnumerable<EntityDeleteRequest> requests)
        {
            if (!requests.IsNullOrEmpty())
            {
                lock (_lock)
                {
                    foreach (var request in requests)
                    {
                        _groupsByGroup.Remove(request.Target);
                    }
                }
            }
        }


        #region "Query"

        public async Task<TrakHoundResponse<ITrakHoundObjectGroupEntity>> QueryByGroup(IEnumerable<string> objectIds)
        {
            var stpw = System.Diagnostics.Stopwatch.StartNew();
            var results = new List<TrakHoundResult<ITrakHoundObjectGroupEntity>>();

            if (!objectIds.IsNullOrEmpty())
            {
                foreach (var objectId in objectIds)
                {
                    if (!string.IsNullOrEmpty(objectId))
                    {
                        lock (_lock)
                        {
                            if (_groupsByGroup.TryGetValue(objectId, out var groups))
                            {
                                if (!groups.IsNullOrEmpty())
                                {
                                    // Update Last Accessed
                                    _accessed.Remove(objectId);
                                    _accessed.Add(objectId, UnixDateTime.Now);

                                    foreach (var group in groups)
                                    {
                                        results.Add(new TrakHoundResult<ITrakHoundObjectGroupEntity>(Id, objectId, TrakHoundResultType.Ok, group, group.Uuid));
                                    }
                                }
                            }
                            else if (_emptyByGroup.Contains(objectId))
                            {
                                // Update Last Accessed
                                _accessed.Remove(objectId);
                                _accessed.Add(objectId, UnixDateTime.Now);

                                results.Add(new TrakHoundResult<ITrakHoundObjectGroupEntity>(Id, objectId, TrakHoundResultType.Empty));
                            }
                            else
                            {
                                results.Add(new TrakHoundResult<ITrakHoundObjectGroupEntity>(Id, objectId, TrakHoundResultType.NotFound));
                            }
                        }
                    }
                    else
                    {
                        results.Add(new TrakHoundResult<ITrakHoundObjectGroupEntity>(Id, null, TrakHoundResultType.BadRequest));
                    }
                }
            }
            else
            {
                results.Add(new TrakHoundResult<ITrakHoundObjectGroupEntity>(Id, null, TrakHoundResultType.BadRequest));
            }

            stpw.Stop();
            return new TrakHoundResponse<ITrakHoundObjectGroupEntity>(results, stpw.ElapsedTicks);
        }


        public async Task<TrakHoundResponse<ITrakHoundObjectGroupEntity>> QueryByMember(IEnumerable<string> memberIds)
        {
            var stpw = System.Diagnostics.Stopwatch.StartNew();
            var results = new List<TrakHoundResult<ITrakHoundObjectGroupEntity>>();

            if (!memberIds.IsNullOrEmpty())
            {
                foreach (var memberId in memberIds)
                {
                    if (!string.IsNullOrEmpty(memberId))
                    {
                        lock (_lock)
                        {
                            if (_groupsByMember.TryGetValue(memberId, out var groups))
                            {
                                if (!groups.IsNullOrEmpty())
                                {
                                    // Update Last Accessed
                                    _accessed.Remove(memberId);
                                    _accessed.Add(memberId, UnixDateTime.Now);

                                    foreach (var group in groups)
                                    {
                                        results.Add(new TrakHoundResult<ITrakHoundObjectGroupEntity>(Id, memberId, TrakHoundResultType.Ok, group, group.Uuid));
                                    }
                                }
                            }
                            else if (_emptyByMember.Contains(memberId))
                            {
                                // Update Last Accessed
                                _accessed.Remove(memberId);
                                _accessed.Add(memberId, UnixDateTime.Now);

                                results.Add(new TrakHoundResult<ITrakHoundObjectGroupEntity>(Id, memberId, TrakHoundResultType.Empty));
                            }
                            else
                            {
                                results.Add(new TrakHoundResult<ITrakHoundObjectGroupEntity>(Id, memberId, TrakHoundResultType.NotFound));
                            }
                        }
                    }
                    else
                    {
                        results.Add(new TrakHoundResult<ITrakHoundObjectGroupEntity>(Id, null, TrakHoundResultType.BadRequest));
                    }
                }
            }
            else
            {
                results.Add(new TrakHoundResult<ITrakHoundObjectGroupEntity>(Id, null, TrakHoundResultType.BadRequest));
            }

            stpw.Stop();
            return new TrakHoundResponse<ITrakHoundObjectGroupEntity>(results, stpw.ElapsedTicks);
        }

        #endregion

        #region "Subscribe"

        public async Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectGroupEntity>>>> SubscribeByGroup(IEnumerable<string> objectUuids)
        {
            var consumer = new MemoryConsumer<IEnumerable<ITrakHoundObjectGroupEntity>>(objectUuids, ProcessGroupMessage);
            _publishConsumers.Add(consumer.Id, consumer);

            var results = new List<TrakHoundResult<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectGroupEntity>>>>();
            results.Add(new TrakHoundResult<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectGroupEntity>>>(Id, "All", TrakHoundResultType.Ok, consumer));

            return new TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectGroupEntity>>>(results);
        }

        private static IEnumerable<ITrakHoundObjectGroupEntity> ProcessGroupMessage(IEnumerable<string> keys, IEnumerable<ITrakHoundObjectGroupEntity> entities)
        {
            var resultEntities = new List<ITrakHoundObjectGroupEntity>();
            foreach (var entity in entities)
            {
                if (keys.Contains(entity.GroupUuid))
                {
                    resultEntities.Add(entity);
                }
            }
            return resultEntities;
        }


        public async Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectGroupEntity>>>> SubscribeByMember(IEnumerable<string> objectUuids)
        {
            var consumer = new MemoryConsumer<IEnumerable<ITrakHoundObjectGroupEntity>>(objectUuids, ProcessMemberMessage);
            _publishConsumers.Add(consumer.Id, consumer);

            var results = new List<TrakHoundResult<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectGroupEntity>>>>();
            results.Add(new TrakHoundResult<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectGroupEntity>>>(Id, "All", TrakHoundResultType.Ok, consumer));

            return new TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectGroupEntity>>>(results);
        }

        private static IEnumerable<ITrakHoundObjectGroupEntity> ProcessMemberMessage(IEnumerable<string> keys, IEnumerable<ITrakHoundObjectGroupEntity> entities)
        {
            var resultEntities = new List<ITrakHoundObjectGroupEntity>();
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

        public async Task<TrakHoundResponse<bool>> EmptyByGroup(IEnumerable<EntityEmptyRequest> requests)
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

                            var existingEntities = _groupsByGroup.GetValueOrDefault(request.EntityUuid);
                            if (!existingEntities.IsNullOrEmpty())
                            {
                                latestTimestamp = existingEntities.Select(o => o.Created).Max();
                            }

                            if (request.Timestamp > latestTimestamp && !_emptyByGroup.Contains(request.EntityUuid))
                            {
                                _emptyByGroup.Add(request.EntityUuid);

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

        public async Task<TrakHoundResponse<bool>> EmptyByMember(IEnumerable<EntityEmptyRequest> requests)
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

                            var existingEntities = _groupsByMember.GetValueOrDefault(request.EntityUuid);
                            if (!existingEntities.IsNullOrEmpty())
                            {
                                latestTimestamp = existingEntities.Select(o => o.Created).Max();
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
