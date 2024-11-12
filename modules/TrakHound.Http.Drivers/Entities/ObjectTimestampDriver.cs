// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Drivers;
using TrakHound.Drivers.Entities;
using TrakHound.Entities;

namespace TrakHound.Http.Entities
{
    public class ObjectTimestampDriver :
        HttpEntityDriver<ITrakHoundObjectTimestampEntity>,
        IObjectTimestampSubscribeDriver,
        IObjectTimestampQueryDriver
    {
        public ObjectTimestampDriver() { }

        public ObjectTimestampDriver(ITrakHoundDriverConfiguration configuration) : base(configuration) { }


        public async Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectTimestampEntity>>>> Subscribe(IEnumerable<string> objectUuids)
        {
            var consumer = await Client.System.Entities.Objects.Timestamp.SubscribeByObjectUuid(objectUuids);
            if (consumer != null)
            {
                consumer.Disposed += ConsumerDisposed;
                lock (_lock) _consumers.Add(consumer.Id, consumer);

                var results = new List<TrakHoundResult<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectTimestampEntity>>>>();
                results.Add(new TrakHoundResult<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectTimestampEntity>>>(Id, "All", TrakHoundResultType.Ok, consumer));
                return new TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectTimestampEntity>>>(results);
            }
            else
            {
                return TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectTimestampEntity>>>.NotFound(Id, "All");
            }
        }


        public async Task<TrakHoundResponse<ITrakHoundObjectTimestampEntity>> Query(IEnumerable<string> objectUuids)
        {
            var results = new List<TrakHoundResult<ITrakHoundObjectTimestampEntity>>();

            if (!objectUuids.IsNullOrEmpty())
            {
                var entities = await Client.System.Entities.Objects.Timestamp.QueryByObjectUuid(objectUuids);
                var dEntities = entities?.ToDictionary(o => o.ObjectUuid);

                foreach (var objectUuid in objectUuids)
                {
                    var targetEntity = dEntities?.GetValueOrDefault(objectUuid);
                    if (targetEntity != null)
                    {
                        results.Add(new TrakHoundResult<ITrakHoundObjectTimestampEntity>(Id, objectUuid, TrakHoundResultType.Ok, targetEntity));
                    }
                    else
                    {
                        results.Add(new TrakHoundResult<ITrakHoundObjectTimestampEntity>(Id, objectUuid, TrakHoundResultType.NotFound));
                    }
                }
            }
            else
            {
                results.Add(new TrakHoundResult<ITrakHoundObjectTimestampEntity>(Id, null, TrakHoundResultType.NotFound));
            }

            return new TrakHoundResponse<ITrakHoundObjectTimestampEntity>(results);
        }

        public async Task<TrakHoundResponse<ITrakHoundObjectTimestampEntity>> Query(IEnumerable<string> objectUuids, long from, long to)
        {
            var results = new List<TrakHoundResult<ITrakHoundObjectTimestampEntity>>();

            if (!objectUuids.IsNullOrEmpty())
            {
                var entities = await Client.System.Entities.Objects.Timestamp.QueryByObjectUuid(objectUuids, from, to);
                var dEntities = entities?.ToDictionary(o => o.ObjectUuid);

                foreach (var objectUuid in objectUuids)
                {
                    var targetEntity = dEntities?.GetValueOrDefault(objectUuid);
                    if (targetEntity != null)
                    {
                        results.Add(new TrakHoundResult<ITrakHoundObjectTimestampEntity>(Id, objectUuid, TrakHoundResultType.Ok, targetEntity));
                    }
                    else
                    {
                        results.Add(new TrakHoundResult<ITrakHoundObjectTimestampEntity>(Id, objectUuid, TrakHoundResultType.NotFound));
                    }
                }
            }
            else
            {
                results.Add(new TrakHoundResult<ITrakHoundObjectTimestampEntity>(Id, null, TrakHoundResultType.NotFound));
            }

            return new TrakHoundResponse<ITrakHoundObjectTimestampEntity>(results);
        }
    }
}
