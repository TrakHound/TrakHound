// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Drivers;
using TrakHound.Drivers.Entities;
using TrakHound.Entities;

namespace TrakHound.Http.Entities
{
    public class ObjectReferenceDriver :
        HttpEntityDriver<ITrakHoundObjectReferenceEntity>,
        IObjectReferenceSubscribeDriver,
        IObjectReferenceQueryDriver
    {
        public ObjectReferenceDriver() { }

        public ObjectReferenceDriver(ITrakHoundDriverConfiguration configuration) : base(configuration) { }


        public async Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectReferenceEntity>>>> Subscribe(IEnumerable<string> objectUuids)
        {
            var consumer = await Client.System.Entities.Objects.Reference.SubscribeByObjectUuid(objectUuids);
            if (consumer != null)
            {
                consumer.Disposed += ConsumerDisposed;
                lock (_lock) _consumers.Add(consumer.Id, consumer);

                var results = new List<TrakHoundResult<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectReferenceEntity>>>>();
                results.Add(new TrakHoundResult<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectReferenceEntity>>>(Id, "All", TrakHoundResultType.Ok, consumer));
                return new TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectReferenceEntity>>>(results);
            }
            else
            {
                return TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectReferenceEntity>>>.NotFound(Id, "All");
            }
        }


        public async Task<TrakHoundResponse<ITrakHoundObjectReferenceEntity>> Query(IEnumerable<string> objectUuids)
        {
            var results = new List<TrakHoundResult<ITrakHoundObjectReferenceEntity>>();

            if (!objectUuids.IsNullOrEmpty())
            {
                var entities = await Client.System.Entities.Objects.Reference.QueryByObjectUuid(objectUuids);
                var dEntities = entities?.ToDictionary(o => o.ObjectUuid);

                foreach (var objectUuid in objectUuids)
                {
                    var targetEntity = dEntities?.GetValueOrDefault(objectUuid);
                    if (targetEntity != null)
                    {
                        results.Add(new TrakHoundResult<ITrakHoundObjectReferenceEntity>(Id, objectUuid, TrakHoundResultType.Ok, targetEntity));
                    }
                    else
                    {
                        results.Add(new TrakHoundResult<ITrakHoundObjectReferenceEntity>(Id, objectUuid, TrakHoundResultType.NotFound));
                    }
                }
            }
            else
            {
                results.Add(new TrakHoundResult<ITrakHoundObjectReferenceEntity>(Id, null, TrakHoundResultType.NotFound));
            }

            return new TrakHoundResponse<ITrakHoundObjectReferenceEntity>(results);
        }
    }
}
