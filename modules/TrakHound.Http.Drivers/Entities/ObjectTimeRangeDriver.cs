// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Drivers;
using TrakHound.Drivers.Entities;
using TrakHound.Entities;

namespace TrakHound.Http.Entities
{
    public class ObjectTimeRangeDriver : 
        HttpEntityDriver<ITrakHoundObjectTimeRangeEntity>, 
        IObjectTimeRangeSubscribeDriver,
        IObjectTimeRangeQueryDriver
    {
        public ObjectTimeRangeDriver() { }

        public ObjectTimeRangeDriver(ITrakHoundDriverConfiguration configuration) : base(configuration) { }


        public async Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectTimeRangeEntity>>>> Subscribe(IEnumerable<string> objectUuids)
        {
            var consumer = await Client.System.Entities.Objects.TimeRange.SubscribeByObjectUuid(objectUuids);
            if (consumer != null)
            {
                consumer.Disposed += ConsumerDisposed;
                lock (_lock) _consumers.Add(consumer.Id, consumer);

                var results = new List<TrakHoundResult<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectTimeRangeEntity>>>>();
                results.Add(new TrakHoundResult<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectTimeRangeEntity>>>(Id, "All", TrakHoundResultType.Ok, consumer));
                return new TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectTimeRangeEntity>>>(results);
            }
            else
            {
                return TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectTimeRangeEntity>>>.NotFound(Id, "All");
            }
        }


        public async Task<TrakHoundResponse<ITrakHoundObjectTimeRangeEntity>> Query(IEnumerable<string> objectUuids)
        {
            var results = new List<TrakHoundResult<ITrakHoundObjectTimeRangeEntity>>();

            if (!objectUuids.IsNullOrEmpty())
            {
                var entities = await Client.System.Entities.Objects.TimeRange.QueryByObjectUuid(objectUuids);
                var dEntities = entities?.ToDictionary(o => o.ObjectUuid);

                foreach (var objectUuid in objectUuids)
                {
                    var targetEntity = dEntities?.GetValueOrDefault(objectUuid);
                    if (targetEntity != null)
                    {
                        results.Add(new TrakHoundResult<ITrakHoundObjectTimeRangeEntity>(Id, objectUuid, TrakHoundResultType.Ok, targetEntity));
                    }
                    else
                    {
                        results.Add(new TrakHoundResult<ITrakHoundObjectTimeRangeEntity>(Id, objectUuid, TrakHoundResultType.NotFound));
                    }
                }
            }
            else
            {
                results.Add(new TrakHoundResult<ITrakHoundObjectTimeRangeEntity>(Id, null, TrakHoundResultType.NotFound));
            }

            return new TrakHoundResponse<ITrakHoundObjectTimeRangeEntity>(results);
        }
    }
}
