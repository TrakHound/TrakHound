// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrakHound.Entities;
using TrakHound.Entities.Collections;

namespace TrakHound.Clients.Collections
{
    internal partial class TrakHoundCollectionObjectNumberClient : TrakHoundCollectionEntityClient<ITrakHoundObjectNumberEntity>, ITrakHoundSystemObjectNumberClient
    {
        private readonly TrakHoundEntityCollection _collection;






        public TrakHoundCollectionObjectNumberClient(TrakHoundEntityCollection collection) : base(collection) 
        { 
            _collection = collection;


        }


        public async Task<IEnumerable<ITrakHoundObjectNumberEntity>> QueryByObject(
            string path,
            string routerId = null)
        {
            return default;
        }

        public async Task<IEnumerable<ITrakHoundObjectNumberEntity>> QueryByObject(
            IEnumerable<string> paths,
            string routerId = null)
        {
            return default;
        }

        public async Task<ITrakHoundObjectNumberEntity> QueryByObjectUuid(
            string objectUuid,
            string routerId = null)
        {
            return default;
        }

        public async Task<IEnumerable<ITrakHoundObjectNumberEntity>> QueryByObjectUuid(
            IEnumerable<string> objectUuids,
            string routerId = null)
        {
            return default;
        }

        public async Task<IEnumerable<ITrakHoundObjectNumberEntity>> QueryByObject(
            string path,
            double minimum,
            double maximum,
            long objectSkip,
            long objectTake,
            string routerId = null)
        {
            return default;
        }

        public async Task<IEnumerable<ITrakHoundObjectNumberEntity>> QueryByObject(
            IEnumerable<string> paths,
            double minimum,
            double maximum,
            long objectSkip,
            long objectTake,
            string routerId = null)
        {
            return default;
        }

        public async Task<ITrakHoundObjectNumberEntity> QueryByObjectUuid(
            string objectUuid,
            double minimum,
            double maximum,
            string routerId = null)
        {
            return default;
        }

        public async Task<IEnumerable<ITrakHoundObjectNumberEntity>> QueryByObjectUuid(
            IEnumerable<string> objectUuids,
            double minimum,
            double maximum,
            string routerId = null)
        {
            return default;
        }

        public async Task<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectNumberEntity>>> SubscribeByObject(
            IEnumerable<string> paths,
            int interval = 0,
            int take = 1000,
            string consumerId = null,
            string routerId = null)
        {
            return default;
        }

        public async Task<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectNumberEntity>>> SubscribeByObjectUuid(
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
