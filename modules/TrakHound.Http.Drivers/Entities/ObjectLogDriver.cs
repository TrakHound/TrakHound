// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Drivers;
using TrakHound.Drivers.Entities;
using TrakHound.Entities;

namespace TrakHound.Http.Entities
{
    public class ObjectLogDriver : 
        HttpEntityDriver<ITrakHoundObjectLogEntity>,
        IObjectLogSubscribeDriver,
        IObjectLogQueryDriver
    {
        public ObjectLogDriver() { }

        public ObjectLogDriver(ITrakHoundDriverConfiguration configuration) : base(configuration) { }


        public async Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectLogEntity>>>> Subscribe(TrakHoundLogLevel minimumLevel)
        {
            return TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectLogEntity>>>.RouteNotConfigured(Id, "All");
        }

        public async Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectLogEntity>>>> Subscribe(IEnumerable<string> objectUuids, TrakHoundLogLevel minimumLevel)
        {
            var consumer = await Client.System.Entities.Objects.Log.SubscribeByObjectUuid(objectUuids, minimumLevel);
            if (consumer != null)
            {
                consumer.Disposed += ConsumerDisposed;
                lock (_lock) _consumers.Add(consumer.Id, consumer);

                var results = new List<TrakHoundResult<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectLogEntity>>>>();
                results.Add(new TrakHoundResult<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectLogEntity>>>(Id, "All", TrakHoundResultType.Ok, consumer));
                return new TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectLogEntity>>>(results);
            }
            else
            {
                return TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectLogEntity>>>.NotFound(Id, "All");
            }
        }


        public async Task<TrakHoundResponse<ITrakHoundObjectLogEntity>> Query(IEnumerable<string> objectUuids, TrakHoundLogLevel minimumLevel, long skip, long take, SortOrder sortOrder)
        {
            var results = new List<TrakHoundResult<ITrakHoundObjectLogEntity>>();

            if (!objectUuids.IsNullOrEmpty())
            {
                var entities = await Client.System.Entities.Objects.Log.QueryByObjectUuid(objectUuids, minimumLevel, skip, take, sortOrder);
                var dEntities = entities?.ToListDictionary(o => o.ObjectUuid);

                foreach (var objectUuid in objectUuids)
                {
                    var targetEntities = dEntities?.Get(objectUuid);
                    if (!targetEntities.IsNullOrEmpty())
                    {
                        foreach (var targetEntity in targetEntities)
                        {
                            results.Add(new TrakHoundResult<ITrakHoundObjectLogEntity>(Id, objectUuid, TrakHoundResultType.Ok, targetEntity));
                        }
                    }
                    else
                    {
                        results.Add(new TrakHoundResult<ITrakHoundObjectLogEntity>(Id, objectUuid, TrakHoundResultType.NotFound));
                    }
                }
            }
            else
            {
                results.Add(new TrakHoundResult<ITrakHoundObjectLogEntity>(Id, null, TrakHoundResultType.NotFound));
            }

            return new TrakHoundResponse<ITrakHoundObjectLogEntity>(results);
        }

        public async Task<TrakHoundResponse<ITrakHoundObjectLogEntity>> Query(IEnumerable<TrakHoundRangeQuery> queries, TrakHoundLogLevel minimumLevel, long skip, long take, SortOrder sortOrder)
        {
            var results = new List<TrakHoundResult<ITrakHoundObjectLogEntity>>();

            if (!queries.IsNullOrEmpty())
            {
                var entities = await Client.System.Entities.Objects.Log.QueryByRange(queries, minimumLevel, skip, take, sortOrder);
                var dEntities = entities?.ToListDictionary(o => o.ObjectUuid);

                foreach (var query in queries)
                {
                    var targetEntities = dEntities?.Get(query.Target);
                    if (!targetEntities.IsNullOrEmpty())
                    {
                        foreach (var targetEntity in targetEntities)
                        {
                            results.Add(new TrakHoundResult<ITrakHoundObjectLogEntity>(Id, query.Target, TrakHoundResultType.Ok, targetEntity));
                        }
                    }
                    else
                    {
                        results.Add(new TrakHoundResult<ITrakHoundObjectLogEntity>(Id, query.Target, TrakHoundResultType.NotFound));
                    }
                }
            }
            else
            {
                results.Add(new TrakHoundResult<ITrakHoundObjectLogEntity>(Id, null, TrakHoundResultType.NotFound));
            }

            return new TrakHoundResponse<ITrakHoundObjectLogEntity>(results);
        }

        //public async Task<TrakHoundResponse<TrakHoundCount>> Count(IEnumerable<TrakHoundRangeQuery> queries)
        //{
        //    return TrakHoundResponse<TrakHoundCount>.RouteNotConfigured(Id, null);
        //}
    }
}
