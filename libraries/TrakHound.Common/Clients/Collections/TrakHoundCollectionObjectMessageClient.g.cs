// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrakHound.Entities;
using TrakHound.Entities.Collections;

namespace TrakHound.Clients.Collections
{
    internal partial class TrakHoundCollectionObjectMessageClient : TrakHoundCollectionEntityClient<ITrakHoundObjectMessageEntity>, ITrakHoundSystemObjectMessageClient
    {
        private readonly TrakHoundEntityCollection _collection;






        public TrakHoundCollectionObjectMessageClient(TrakHoundEntityCollection collection) : base(collection) 
        { 
            _collection = collection;


        }


        public async Task<IEnumerable<ITrakHoundObjectMessageEntity>> QueryByObject(
            string path,
            string routerId = null)
        {
            return default;
        }

        public async Task<IEnumerable<ITrakHoundObjectMessageEntity>> QueryByObject(
            IEnumerable<string> paths,
            string routerId = null)
        {
            return default;
        }

        public async Task<ITrakHoundObjectMessageEntity> QueryByObjectUuid(
            string objectUuid,
            string routerId = null)
        {
            return default;
        }

        public async Task<IEnumerable<ITrakHoundObjectMessageEntity>> QueryByObjectUuid(
            IEnumerable<string> objectUuids,
            string routerId = null)
        {
            return default;
        }

        public async Task<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectMessageEntity>>> SubscribeByObject(
            IEnumerable<string> paths,
            int interval = 0,
            int take = 1000,
            string consumerId = null,
            string routerId = null)
        {
            return default;
        }

        public async Task<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectMessageEntity>>> SubscribeByObjectUuid(
            IEnumerable<string> objectUuids,
            int interval = 0,
            int take = 1000,
            string consumerId = null,
            string routerId = null)
        {
            return default;
        }
}
}
