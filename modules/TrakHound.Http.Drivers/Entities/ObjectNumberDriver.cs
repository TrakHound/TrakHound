// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Drivers;
using TrakHound.Drivers.Entities;
using TrakHound.Entities;

namespace TrakHound.Http.Entities
{
    public class ObjectNumberDriver :
        HttpEntityDriver<ITrakHoundObjectNumberEntity>,
        IObjectNumberSubscribeDriver,
        IObjectNumberQueryDriver
    {
        public ObjectNumberDriver() { }

        public ObjectNumberDriver(ITrakHoundDriverConfiguration configuration) : base(configuration) { }


        public async Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectNumberEntity>>>> Subscribe(IEnumerable<string> objectUuids)
        {
            var consumer = await Client.System.Entities.Objects.Number.SubscribeByObjectUuid(objectUuids);
            if (consumer != null)
            {
                consumer.Disposed += ConsumerDisposed;
                lock (_lock) _consumers.Add(consumer.Id, consumer);

                var results = new List<TrakHoundResult<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectNumberEntity>>>>();
                results.Add(new TrakHoundResult<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectNumberEntity>>>(Id, "All", TrakHoundResultType.Ok, consumer));
                return new TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectNumberEntity>>>(results);
            }
            else
            {
                return TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectNumberEntity>>>.NotFound(Id, "All");
            }
        }


        public async Task<TrakHoundResponse<ITrakHoundObjectNumberEntity>> Query(IEnumerable<string> objectUuids)
        {
            var results = new List<TrakHoundResult<ITrakHoundObjectNumberEntity>>();

            if (!objectUuids.IsNullOrEmpty())
            {
                var entities = await Client.System.Entities.Objects.Number.QueryByObjectUuid(objectUuids);
                var dEntities = entities?.ToDictionary(o => o.ObjectUuid);

                foreach (var objectUuid in objectUuids)
                {
                    var targetEntity = dEntities?.GetValueOrDefault(objectUuid);
                    if (targetEntity != null)
                    {
                        results.Add(new TrakHoundResult<ITrakHoundObjectNumberEntity>(Id, objectUuid, TrakHoundResultType.Ok, targetEntity));
                    }
                    else
                    {
                        results.Add(new TrakHoundResult<ITrakHoundObjectNumberEntity>(Id, objectUuid, TrakHoundResultType.NotFound));
                    }
                }
            }
            else
            {
                results.Add(new TrakHoundResult<ITrakHoundObjectNumberEntity>(Id, null, TrakHoundResultType.NotFound));
            }

            return new TrakHoundResponse<ITrakHoundObjectNumberEntity>(results);
        }

        public async Task<TrakHoundResponse<ITrakHoundObjectNumberEntity>> Query(IEnumerable<string> objectUuids, double from, double to)
        {
            var results = new List<TrakHoundResult<ITrakHoundObjectNumberEntity>>();

            if (!objectUuids.IsNullOrEmpty())
            {
                var entities = await Client.System.Entities.Objects.Number.QueryByObjectUuid(objectUuids, from, to);
                var dEntities = entities?.ToDictionary(o => o.ObjectUuid);

                foreach (var objectUuid in objectUuids)
                {
                    var targetEntity = dEntities?.GetValueOrDefault(objectUuid);
                    if (targetEntity != null)
                    {
                        results.Add(new TrakHoundResult<ITrakHoundObjectNumberEntity>(Id, objectUuid, TrakHoundResultType.Ok, targetEntity));
                    }
                    else
                    {
                        results.Add(new TrakHoundResult<ITrakHoundObjectNumberEntity>(Id, objectUuid, TrakHoundResultType.NotFound));
                    }
                }
            }
            else
            {
                results.Add(new TrakHoundResult<ITrakHoundObjectNumberEntity>(Id, null, TrakHoundResultType.NotFound));
            }

            return new TrakHoundResponse<ITrakHoundObjectNumberEntity>(results);
        }
    }
}
