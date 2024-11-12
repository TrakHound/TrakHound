// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Entities;

namespace TrakHound.Clients
{
    public partial interface ITrakHoundSystemObjectQueueClient : ITrakHoundEntityClient<ITrakHoundObjectQueueEntity>
    {
        Task<IEnumerable<ITrakHoundObjectQueueEntity>> QueryByQueue(
            string queuePath,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null);

        Task<IEnumerable<ITrakHoundObjectQueueEntity>> QueryByQueueUuid(
            string queueUuid,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null);

        Task<IEnumerable<ITrakHoundObjectQueueEntity>> QueryByQueueUuid(
            IEnumerable<string> queueUuids,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null);

        Task<IEnumerable<ITrakHoundObjectQueueEntity>> Pull(
            string queueUuid,
            int count = 1,
            string routerId = null);

        Task<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectQueueEntity>>> SubscribeByQueue(
            IEnumerable<string> paths,
            int interval = 0,
            int take = 1000,
            string consumerId = null,
            string routerId = null);

        Task<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectQueueEntity>>> SubscribeByQueueUuid(
            IEnumerable<string> queueUuids,
            int interval = 0,
            int take = 1000,
            string consumerId = null,
            string routerId = null);

    }
}