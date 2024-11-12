// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrakHound.Entities;
using TrakHound.Entities.Collections;

namespace TrakHound.Clients.Collections
{
    internal partial class TrakHoundCollectionObjectBlobClient : TrakHoundCollectionEntityClient<ITrakHoundObjectBlobEntity>, ITrakHoundSystemObjectBlobClient
    {
        private readonly TrakHoundEntityCollection _collection;






        public TrakHoundCollectionObjectBlobClient(TrakHoundEntityCollection collection) : base(collection) 
        { 
            _collection = collection;


        }


        public async Task<IEnumerable<ITrakHoundObjectBlobEntity>> QueryByObject(
            string path,
            string routerId = null)
        {
            return default;
        }

        public async Task<IEnumerable<ITrakHoundObjectBlobEntity>> QueryByObject(
            IEnumerable<string> paths,
            string routerId = null)
        {
            return default;
        }

        public async Task<ITrakHoundObjectBlobEntity> QueryByObjectUuid(
            string objectUuid,
            string routerId = null)
        {
            return default;
        }

        public async Task<IEnumerable<ITrakHoundObjectBlobEntity>> QueryByObjectUuid(
            IEnumerable<string> objectUuids,
            string routerId = null)
        {
            return default;
        }

        public async Task<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectBlobEntity>>> SubscribeByObject(
            IEnumerable<string> paths,
            int interval = 0,
            int take = 1000,
            string consumerId = null,
            string routerId = null)
        {
            return default;
        }

        public async Task<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectBlobEntity>>> SubscribeByObjectUuid(
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
