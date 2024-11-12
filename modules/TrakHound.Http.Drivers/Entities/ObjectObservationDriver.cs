// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Drivers;
using TrakHound.Drivers.Entities;
using TrakHound.Entities;

namespace TrakHound.Http.Entities
{
    public class ObjectObservationDriver :
        HttpEntityDriver<ITrakHoundObjectObservationEntity>,
        IObjectObservationLatestDriver,
        IObjectObservationSubscribeDriver,
        IObjectObservationQueryDriver
    {
        public ObjectObservationDriver() { }

        public ObjectObservationDriver(ITrakHoundDriverConfiguration configuration) : base(configuration) { }


        public async Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectObservationEntity>>>> Subscribe(IEnumerable<string> objectUuids)
        {
            var consumer = await Client.System.Entities.Objects.Observation.SubscribeByObjectUuid(objectUuids);
            if (consumer != null)
            {
                consumer.Disposed += ConsumerDisposed;
                lock (_lock) _consumers.Add(consumer.Id, consumer);

                var results = new List<TrakHoundResult<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectObservationEntity>>>>();
                results.Add(new TrakHoundResult<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectObservationEntity>>>(Id, "All", TrakHoundResultType.Ok, consumer));
                return new TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectObservationEntity>>>(results);
            }
            else
            {
                return TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectObservationEntity>>>.NotFound(Id, "All");
            }
        }


        public async Task<TrakHoundResponse<ITrakHoundObjectObservationEntity>> Latest(IEnumerable<string> objectUuids)
        {
            var results = new List<TrakHoundResult<ITrakHoundObjectObservationEntity>>();

            if (!objectUuids.IsNullOrEmpty())
            {
                var entities = await Client.System.Entities.Objects.Observation.LatestByObjectUuid(objectUuids);
                var dEntities = entities?.ToDictionary(o => o.ObjectUuid);

                foreach (var objectUuid in objectUuids)
                {
                    var targetEntity = dEntities?.GetValueOrDefault(objectUuid);
                    if (targetEntity != null)
                    {
                        results.Add(new TrakHoundResult<ITrakHoundObjectObservationEntity>(Id, objectUuid, TrakHoundResultType.Ok, targetEntity));
                    }
                    else
                    {
                        results.Add(new TrakHoundResult<ITrakHoundObjectObservationEntity>(Id, objectUuid, TrakHoundResultType.NotFound));
                    }
                }
            }
            else
            {
                results.Add(new TrakHoundResult<ITrakHoundObjectObservationEntity>(Id, null, TrakHoundResultType.NotFound));
            }

            return new TrakHoundResponse<ITrakHoundObjectObservationEntity>(results);
        }


        public async Task<TrakHoundResponse<ITrakHoundObjectObservationEntity>> Query(IEnumerable<TrakHoundRangeQuery> queries, long skip, long take, SortOrder sortOrder)
        {
            var results = new List<TrakHoundResult<ITrakHoundObjectObservationEntity>>();

            if (!queries.IsNullOrEmpty())
            {
                var entities = await Client.System.Entities.Objects.Observation.QueryByRange(queries, skip, take, sortOrder);
                var dEntities = entities?.ToListDictionary(o => o.ObjectUuid);

                foreach (var query in queries)
                {
                    var targetEntities = dEntities?.Get(query.Target);
                    if (!targetEntities.IsNullOrEmpty())
                    {
                        foreach (var targetEntity in targetEntities)
                        {
                            results.Add(new TrakHoundResult<ITrakHoundObjectObservationEntity>(Id, query.Target, TrakHoundResultType.Ok, targetEntity));
                        }
                    }
                    else
                    {
                        results.Add(new TrakHoundResult<ITrakHoundObjectObservationEntity>(Id, query.Target, TrakHoundResultType.NotFound));
                    }
                }
            }
            else
            {
                results.Add(new TrakHoundResult<ITrakHoundObjectObservationEntity>(Id, null, TrakHoundResultType.NotFound));
            }

            return new TrakHoundResponse<ITrakHoundObjectObservationEntity>(results);
        }

        public async Task<TrakHoundResponse<ITrakHoundObjectObservationEntity>> Query(IEnumerable<TrakHoundRangeQuery> queries, IEnumerable<TrakHoundConditionGroupQuery> conditions, long skip, long take, SortOrder sortOrder)
        {
            return TrakHoundResponse<ITrakHoundObjectObservationEntity>.RouteNotConfigured(Id, null);
        }

        public async Task<TrakHoundResponse<ITrakHoundObjectObservationEntity>> Last(IEnumerable<TrakHoundTimeQuery> queries)
        {
            return TrakHoundResponse<ITrakHoundObjectObservationEntity>.RouteNotConfigured(Id, null);
        }

        public async Task<TrakHoundResponse<ITrakHoundObjectObservationEntity>> Last(IEnumerable<TrakHoundTimeQuery> queries, IEnumerable<TrakHoundConditionGroupQuery> conditions)
        {
            return TrakHoundResponse<ITrakHoundObjectObservationEntity>.RouteNotConfigured(Id, null);
        }

        public async Task<TrakHoundResponse<TrakHoundCount>> Count(IEnumerable<TrakHoundRangeQuery> queries)
        {
            return TrakHoundResponse<TrakHoundCount>.RouteNotConfigured(Id, null);
        }
    }
}
