// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrakHound.Entities;
using TrakHound.Entities.Collections;

namespace TrakHound.Clients.Collections
{
    internal partial class TrakHoundCollectionObjectLogClient : TrakHoundCollectionEntityClient<ITrakHoundObjectLogEntity>, ITrakHoundSystemObjectLogClient
    {
        private readonly TrakHoundEntityCollection _collection;






        public TrakHoundCollectionObjectLogClient(TrakHoundEntityCollection collection) : base(collection) 
        { 
            _collection = collection;


        }


        public async Task<IEnumerable<ITrakHoundObjectLogEntity>> QueryByObject(
            string path,
            TrakHound.TrakHoundLogLevel minLevel,
            long start,
            long stop,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null)
        {
            return default;
        }

        public async Task<IEnumerable<ITrakHoundObjectLogEntity>> QueryByObject(
            IEnumerable<string> paths,
            TrakHound.TrakHoundLogLevel minLevel,
            long start,
            long stop,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null)
        {
            return default;
        }

        public async Task<IEnumerable<ITrakHoundObjectLogEntity>> QueryByObjectUuid(
            string objectUuid,
            TrakHound.TrakHoundLogLevel minLevel,
            long start,
            long stop,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null)
        {
            return default;
        }

        public async Task<IEnumerable<ITrakHoundObjectLogEntity>> QueryByObjectUuid(
            IEnumerable<string> objectUuids,
            TrakHound.TrakHoundLogLevel minLevel,
            long start,
            long stop,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null)
        {
            return default;
        }

        public async Task<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectLogEntity>>> SubscribeByObject(
            IEnumerable<string> paths,
            TrakHound.TrakHoundLogLevel minLevel,
            int interval = 0,
            int take = 1000,
            string consumerId = null,
            string routerId = null)
        {
            return default;
        }

        public async Task<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectLogEntity>>> SubscribeByObjectUuid(
            IEnumerable<string> objectUuids,
            TrakHound.TrakHoundLogLevel minLevel,
            int interval = 0,
            int take = 1000,
            string consumerId = null,
            string routerId = null)
        {
            return default;
        }

        public async Task<bool> DeleteByObjectUuid(
            string objectUuid,
            string routerId = null)
        {
            return default;
        }

        public async Task<bool> DeleteByObjectUuid(
            IEnumerable<string> objectUuids,
            string routerId = null)
        {
            return default;
        }
}
}
