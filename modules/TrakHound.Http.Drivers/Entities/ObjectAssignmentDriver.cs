// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Drivers;
using TrakHound.Drivers.Entities;
using TrakHound.Entities;

namespace TrakHound.Http.Entities
{
    public class ObjectAssignmentDriver : 
        HttpEntityDriver<ITrakHoundObjectAssignmentEntity>,
        IObjectAssignmentCurrentDriver,
        IObjectAssignmentSubscribeDriver,
        IObjectAssignmentQueryDriver
    {
        public ObjectAssignmentDriver() { }

        public ObjectAssignmentDriver(ITrakHoundDriverConfiguration configuration) : base(configuration) { }


        public async Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectAssignmentEntity>>>> SubscribeByAssignee(IEnumerable<string> objectUuids)
        {
            var consumer = await Client.System.Entities.Objects.Assignment.SubscribeByAssigneeUuid(objectUuids);
            if (consumer != null)
            {
                consumer.Disposed += ConsumerDisposed;
                lock (_lock) _consumers.Add(consumer.Id, consumer);

                var results = new List<TrakHoundResult<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectAssignmentEntity>>>>();
                results.Add(new TrakHoundResult<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectAssignmentEntity>>>(Id, "All", TrakHoundResultType.Ok, consumer));
                return new TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectAssignmentEntity>>>(results);
            }
            else
            {
                return TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectAssignmentEntity>>>.NotFound(Id, "All");
            }
        }

        public async Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectAssignmentEntity>>>> SubscribeByMember(IEnumerable<string> objectUuids)
        {
            var consumer = await Client.System.Entities.Objects.Assignment.SubscribeByMemberUuid(objectUuids);
            if (consumer != null)
            {
                consumer.Disposed += ConsumerDisposed;
                lock (_lock) _consumers.Add(consumer.Id, consumer);

                var results = new List<TrakHoundResult<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectAssignmentEntity>>>>();
                results.Add(new TrakHoundResult<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectAssignmentEntity>>>(Id, "All", TrakHoundResultType.Ok, consumer));
                return new TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectAssignmentEntity>>>(results);
            }
            else
            {
                return TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectAssignmentEntity>>>.NotFound(Id, "All");
            }
        }


        public async Task<TrakHoundResponse<ITrakHoundObjectAssignmentEntity>> CurrentByAssignee(IEnumerable<string> objectUuids)
        {
            var results = new List<TrakHoundResult<ITrakHoundObjectAssignmentEntity>>();

            if (!objectUuids.IsNullOrEmpty())
            {
                var entities = await Client.System.Entities.Objects.Assignment.CurrentByAssigneeUuid(objectUuids);
                var dEntities = entities?.ToListDictionary(o => o.AssigneeUuid);

                foreach (var objectUuid in objectUuids)
                {
                    var targetEntities = dEntities?.Get(objectUuid);
                    if (!targetEntities.IsNullOrEmpty())
                    {
                        foreach (var targetEntity in targetEntities)
                        {
                            results.Add(new TrakHoundResult<ITrakHoundObjectAssignmentEntity>(Id, objectUuid, TrakHoundResultType.Ok, targetEntity));
                        }
                    }
                    else
                    {
                        results.Add(new TrakHoundResult<ITrakHoundObjectAssignmentEntity>(Id, objectUuid, TrakHoundResultType.NotFound));
                    }
                }
            }
            else
            {
                results.Add(new TrakHoundResult<ITrakHoundObjectAssignmentEntity>(Id, null, TrakHoundResultType.NotFound));
            }

            return new TrakHoundResponse<ITrakHoundObjectAssignmentEntity>(results);
        }

        public async Task<TrakHoundResponse<ITrakHoundObjectAssignmentEntity>> CurrentByMember(IEnumerable<string> objectUuids)
        {
            var results = new List<TrakHoundResult<ITrakHoundObjectAssignmentEntity>>();

            if (!objectUuids.IsNullOrEmpty())
            {
                var entities = await Client.System.Entities.Objects.Assignment.CurrentByMemberUuid(objectUuids);
                var dEntities = entities?.ToListDictionary(o => o.MemberUuid);

                foreach (var objectUuid in objectUuids)
                {
                    var targetEntities = dEntities?.Get(objectUuid);
                    if (!targetEntities.IsNullOrEmpty())
                    {
                        foreach (var targetEntity in targetEntities)
                        {
                            results.Add(new TrakHoundResult<ITrakHoundObjectAssignmentEntity>(Id, objectUuid, TrakHoundResultType.Ok, targetEntity));
                        }
                    }
                    else
                    {
                        results.Add(new TrakHoundResult<ITrakHoundObjectAssignmentEntity>(Id, objectUuid, TrakHoundResultType.NotFound));
                    }
                }
            }
            else
            {
                results.Add(new TrakHoundResult<ITrakHoundObjectAssignmentEntity>(Id, null, TrakHoundResultType.NotFound));
            }

            return new TrakHoundResponse<ITrakHoundObjectAssignmentEntity>(results);
        }


        public async Task<TrakHoundResponse<ITrakHoundObjectAssignmentEntity>> QueryByAssignee(IEnumerable<TrakHoundRangeQuery> queries, long skip, long take, SortOrder sortOrder)
        {
            var results = new List<TrakHoundResult<ITrakHoundObjectAssignmentEntity>>();

            if (!queries.IsNullOrEmpty())
            {
                var entities = await Client.System.Entities.Objects.Assignment.QueryByAssigneeRange(queries, skip, take, sortOrder);
                var dEntities = entities?.ToListDictionary(o => o.AssigneeUuid);

                foreach (var query in queries)
                {
                    var targetEntities = dEntities?.Get(query.Target);
                    if (!targetEntities.IsNullOrEmpty())
                    {
                        foreach (var targetEntity in targetEntities)
                        {
                            results.Add(new TrakHoundResult<ITrakHoundObjectAssignmentEntity>(Id, query.Target, TrakHoundResultType.Ok, targetEntity));
                        }
                    }
                    else
                    {
                        results.Add(new TrakHoundResult<ITrakHoundObjectAssignmentEntity>(Id, query.Target, TrakHoundResultType.NotFound));
                    }
                }
            }
            else
            {
                results.Add(new TrakHoundResult<ITrakHoundObjectAssignmentEntity>(Id, null, TrakHoundResultType.NotFound));
            }

            return new TrakHoundResponse<ITrakHoundObjectAssignmentEntity>(results);
        }

        public async Task<TrakHoundResponse<ITrakHoundObjectAssignmentEntity>> QueryByMember(IEnumerable<TrakHoundRangeQuery> queries, long skip, long take, SortOrder sortOrder)
        {
            var results = new List<TrakHoundResult<ITrakHoundObjectAssignmentEntity>>();

            if (!queries.IsNullOrEmpty())
            {
                var entities = await Client.System.Entities.Objects.Assignment.QueryByMemberRange(queries, skip, take, sortOrder);
                var dEntities = entities?.ToListDictionary(o => o.MemberUuid);

                foreach (var query in queries)
                {
                    var targetEntities = dEntities?.Get(query.Target);
                    if (!targetEntities.IsNullOrEmpty())
                    {
                        foreach (var targetEntity in targetEntities)
                        {
                            results.Add(new TrakHoundResult<ITrakHoundObjectAssignmentEntity>(Id, query.Target, TrakHoundResultType.Ok, targetEntity));
                        }
                    }
                    else
                    {
                        results.Add(new TrakHoundResult<ITrakHoundObjectAssignmentEntity>(Id, query.Target, TrakHoundResultType.NotFound));
                    }
                }
            }
            else
            {
                results.Add(new TrakHoundResult<ITrakHoundObjectAssignmentEntity>(Id, null, TrakHoundResultType.NotFound));
            }

            return new TrakHoundResponse<ITrakHoundObjectAssignmentEntity>(results);
        }


        public async Task<TrakHoundResponse<TrakHoundCount>> CountByAssignee(IEnumerable<TrakHoundRangeQuery> queries)
        {
            return TrakHoundResponse<TrakHoundCount>.RouteNotConfigured(Id, null);
        }

        public async Task<TrakHoundResponse<TrakHoundCount>> CountByMember(IEnumerable<TrakHoundRangeQuery> queries)
        {
            return TrakHoundResponse<TrakHoundCount>.RouteNotConfigured(Id, null);
        }
    }
}
