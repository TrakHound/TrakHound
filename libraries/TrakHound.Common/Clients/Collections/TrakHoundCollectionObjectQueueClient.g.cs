// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrakHound.Entities;
using TrakHound.Entities.Collections;

namespace TrakHound.Clients.Collections
{
    internal partial class TrakHoundCollectionObjectQueueClient : TrakHoundCollectionEntityClient<ITrakHoundObjectQueueEntity>, ITrakHoundSystemObjectQueueClient
    {
        private readonly TrakHoundEntityCollection _collection;






        public TrakHoundCollectionObjectQueueClient(TrakHoundEntityCollection collection) : base(collection) 
        { 
            _collection = collection;


        }


        public async Task<IEnumerable<ITrakHoundObjectQueueEntity>> QueryByQueue(
            string queuePath,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null)
        {
            return default;
        }

        public async Task<IEnumerable<ITrakHoundObjectQueueEntity>> QueryByQueueUuid(
            string queueUuid,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null)
        {
            return default;
        }

        public async Task<IEnumerable<ITrakHoundObjectQueueEntity>> QueryByQueueUuid(
            IEnumerable<string> queueUuids,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null)
        {
            return default;
        }

        public async Task<IEnumerable<ITrakHoundObjectQueueEntity>> Pull(
            string queueUuid,
            int count = 1,
            string routerId = null)
        {
            return default;
        }

        public async Task<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectQueueEntity>>> SubscribeByQueue(
            IEnumerable<string> paths,
            int interval = 0,
            int take = 1000,
            string consumerId = null,
            string routerId = null)
        {
            return default;
        }

        public async Task<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectQueueEntity>>> SubscribeByQueueUuid(
            IEnumerable<string> queueUuids,
            int interval = 0,
            int take = 1000,
            string consumerId = null,
            string routerId = null)
        {
            return default;
        }
}
}
