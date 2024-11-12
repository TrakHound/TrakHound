// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Drivers;
using TrakHound.Drivers.Entities;
using TrakHound.Entities;

namespace TrakHound.Http.Entities
{
    public class ObjectStateDriver : 
        HttpEntityDriver<ITrakHoundObjectStateEntity>,
        IObjectStateSubscribeDriver,
        IObjectStateLatestDriver,
        IObjectStateQueryDriver
    {
        public ObjectStateDriver() { }

        public ObjectStateDriver(ITrakHoundDriverConfiguration configuration) : base(configuration) { }


        public async Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectStateEntity>>>> Subscribe(IEnumerable<string> objectUuids)
        {
            var consumer = await Client.System.Entities.Objects.State.SubscribeByObjectUuid(objectUuids);
            if (consumer != null)
            {
                consumer.Disposed += ConsumerDisposed;
                lock (_lock) _consumers.Add(consumer.Id, consumer);

                var results = new List<TrakHoundResult<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectStateEntity>>>>();
                results.Add(new TrakHoundResult<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectStateEntity>>>(Id, "All", TrakHoundResultType.Ok, consumer));
                return new TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectStateEntity>>>(results);
            }
            else
            {
                return TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectStateEntity>>>.NotFound(Id, "All");
            }
        }


        public async Task<TrakHoundResponse<ITrakHoundObjectStateEntity>> Latest(IEnumerable<string> objectUuids)
        {
            var results = new List<TrakHoundResult<ITrakHoundObjectStateEntity>>();

            if (!objectUuids.IsNullOrEmpty())
            {
                var entities = await Client.System.Entities.Objects.State.LatestByObjectUuid(objectUuids);
                var dEntities = entities?.ToDictionary(o => o.ObjectUuid);

                foreach (var objectUuid in objectUuids)
                {
                    var targetEntity = dEntities?.GetValueOrDefault(objectUuid);
                    if (targetEntity != null)
                    {
                        results.Add(new TrakHoundResult<ITrakHoundObjectStateEntity>(Id, objectUuid, TrakHoundResultType.Ok, targetEntity));
                    }
                    else
                    {
                        results.Add(new TrakHoundResult<ITrakHoundObjectStateEntity>(Id, objectUuid, TrakHoundResultType.NotFound));
                    }
                }
            }
            else
            {
                results.Add(new TrakHoundResult<ITrakHoundObjectStateEntity>(Id, null, TrakHoundResultType.NotFound));
            }

            return new TrakHoundResponse<ITrakHoundObjectStateEntity>(results);
        }


        public async Task<TrakHoundResponse<ITrakHoundObjectStateEntity>> Query(IEnumerable<TrakHoundRangeQuery> queries, long skip, long take, SortOrder sortOrder)
        {
            var results = new List<TrakHoundResult<ITrakHoundObjectStateEntity>>();

            if (!queries.IsNullOrEmpty())
            {
                var entities = await Client.System.Entities.Objects.State.QueryByRange(queries, skip, take, sortOrder);
                var dEntities = entities?.ToListDictionary(o => o.ObjectUuid);

                foreach (var query in queries)
                {
                    var targetEntities = dEntities?.Get(query.Target);
                    if (!targetEntities.IsNullOrEmpty())
                    {
                        foreach (var targetEntity in targetEntities)
                        {
                            results.Add(new TrakHoundResult<ITrakHoundObjectStateEntity>(Id, query.Target, TrakHoundResultType.Ok, targetEntity));
                        }
                    }
                    else
                    {
                        results.Add(new TrakHoundResult<ITrakHoundObjectStateEntity>(Id, query.Target, TrakHoundResultType.NotFound));
                    }
                }
            }
            else
            {
                results.Add(new TrakHoundResult<ITrakHoundObjectStateEntity>(Id, null, TrakHoundResultType.NotFound));
            }

            return new TrakHoundResponse<ITrakHoundObjectStateEntity>(results);
        }

        public async Task<TrakHoundResponse<ITrakHoundObjectStateEntity>> Query(IEnumerable<TrakHoundRangeQuery> queries, string conditionQuery, long skip, long take, SortOrder sortOrder)
        {
            var results = new List<TrakHoundResult<ITrakHoundObjectStateEntity>>();

            if (!queries.IsNullOrEmpty())
            {
                var entities = await Client.System.Entities.Objects.State.QueryByRange(queries, skip, take, sortOrder);
                var dEntities = entities?.ToListDictionary(o => o.ObjectUuid);

                foreach (var query in queries)
                {
                    var targetEntities = dEntities?.Get(query.Target);
                    if (!targetEntities.IsNullOrEmpty())
                    {
                        foreach (var targetEntity in targetEntities)
                        {
                            results.Add(new TrakHoundResult<ITrakHoundObjectStateEntity>(Id, query.Target, TrakHoundResultType.Ok, targetEntity));
                        }
                    }
                    else
                    {
                        results.Add(new TrakHoundResult<ITrakHoundObjectStateEntity>(Id, query.Target, TrakHoundResultType.NotFound));
                    }
                }
            }
            else
            {
                results.Add(new TrakHoundResult<ITrakHoundObjectStateEntity>(Id, null, TrakHoundResultType.NotFound));
            }

            return new TrakHoundResponse<ITrakHoundObjectStateEntity>(results);
        }

        public async Task<TrakHoundResponse<ITrakHoundObjectStateEntity>> Last(IEnumerable<TrakHoundTimeQuery> queries)
        {
            return TrakHoundResponse<ITrakHoundObjectStateEntity>.RouteNotConfigured(Id, null);
        }

        public async Task<TrakHoundResponse<TrakHoundCount>> Count(IEnumerable<TrakHoundRangeQuery> queries)
        {
            return TrakHoundResponse<TrakHoundCount>.RouteNotConfigured(Id, null);
        }
    }
}
