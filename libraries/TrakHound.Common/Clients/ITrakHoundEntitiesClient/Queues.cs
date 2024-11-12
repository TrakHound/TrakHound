// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Requests;

namespace TrakHound.Clients
{
    public partial interface ITrakHoundEntitiesClient
    {
        Task<IEnumerable<TrakHoundQueue>> GetQueues(string queuePath, string routerId = null);


        Task<IEnumerable<TrakHoundQueue>> PullFromQueue(string queuePath, int count = 1, string routerId = null);


        Task<ITrakHoundConsumer<IEnumerable<TrakHoundQueue>>> SubscribeQueues(string queuePath, string routerId = null);


        Task<bool> PublishQueue(string queuePath, string memberPath, int index = 0, string routerId = null);

        Task<bool> PublishQueues(IEnumerable<TrakHoundQueueEntry> entries, string routerId = null);


        Task<bool> DeleteQueue(string queuePath, string memberPath, string routerId = null);
    }
}
