// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Entities;

namespace TrakHound.Drivers.Entities
{
    /// <summary>
    /// Driver used to Query TrakHound Object MessageQueues
    /// </summary>
    public interface IObjectMessageQueueQueryDriver : ITrakHoundDriver
    {
        /// <summary>
        /// Query the MessageQueue Entities with the specified Object UUID's
        /// </summary>
        Task<TrakHoundResponse<ITrakHoundObjectMessageQueueEntity>> Query(IEnumerable<string> objectUuids);
    }

    //public interface IObjectMessageQueuePullDriver : ITrakHoundDriver
    //{
    //    public Task<TrakHoundResponse<TrakHoundAcknowledgeResult<ITrakHoundObjectMessageQueueEntity>>> Pull(string objectUuid, int count, bool acknowledge);

    //    public Task<TrakHoundResponse<bool>> Acknowledge(string objectUuid, IEnumerable<string> deliveryIds);

    //    public Task<TrakHoundResponse<bool>> Reject(string objectUuid, IEnumerable<string> deliveryIds);
    //}

    //public interface IObjectMessageQueueSubscribeDriver : ITrakHoundDriver
    //{
    //    Task<TrakHoundResponse<ITrakHoundConsumer<TrakHoundAcknowledgeResult<ITrakHoundObjectMessageQueueEntity>>>> Subscribe(string objectUuid, bool acknowledge);
    //}
}
