// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrakHound.Entities;
using TrakHound.Entities.Collections;

namespace TrakHound.Clients.Collections
{
    internal partial class TrakHoundCollectionObjectGroupClient : TrakHoundCollectionEntityClient<ITrakHoundObjectGroupEntity>, ITrakHoundSystemObjectGroupClient
    {
        private readonly TrakHoundEntityCollection _collection;






        public TrakHoundCollectionObjectGroupClient(TrakHoundEntityCollection collection) : base(collection) 
        { 
            _collection = collection;


        }


        public async Task<IEnumerable<ITrakHoundObjectGroupEntity>> QueryByGroup(
            string groupPath,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null)
        {
            return default;
        }

        public async Task<IEnumerable<ITrakHoundObjectGroupEntity>> QueryByGroup(
            IEnumerable<string> groupPaths,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null)
        {
            return default;
        }

        public async Task<IEnumerable<ITrakHoundObjectGroupEntity>> QueryByGroupUuid(
            string groupUuid,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null)
        {
            return default;
        }

        public async Task<IEnumerable<ITrakHoundObjectGroupEntity>> QueryByGroupUuid(
            IEnumerable<string> groupUuids,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null)
        {
            return default;
        }

        public async Task<IEnumerable<ITrakHoundObjectGroupEntity>> QueryByMember(
            string memberPath,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null)
        {
            return default;
        }

        public async Task<IEnumerable<ITrakHoundObjectGroupEntity>> QueryByMember(
            IEnumerable<string> memberPaths,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null)
        {
            return default;
        }

        public async Task<IEnumerable<ITrakHoundObjectGroupEntity>> QueryByMemberUuid(
            string memberUuid,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null)
        {
            return default;
        }

        public async Task<IEnumerable<ITrakHoundObjectGroupEntity>> QueryByMemberUuid(
            IEnumerable<string> memberUuids,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null)
        {
            return default;
        }

        public async Task<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectGroupEntity>>> SubscribeByGroup(
            IEnumerable<string> paths,
            int interval = 0,
            int take = 1000,
            string consumerId = null,
            string routerId = null)
        {
            return default;
        }

        public async Task<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectGroupEntity>>> SubscribeByGroupUuid(
            IEnumerable<string> groupUuids,
            int interval = 0,
            int take = 1000,
            string consumerId = null,
            string routerId = null)
        {
            return default;
        }

        public async Task<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectGroupEntity>>> SubscribeByMember(
            IEnumerable<string> paths,
            int interval = 0,
            int take = 1000,
            string consumerId = null,
            string routerId = null)
        {
            return default;
        }

        public async Task<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectGroupEntity>>> SubscribeByMemberUuid(
            IEnumerable<string> memberUuids,
            int interval = 0,
            int take = 1000,
            string consumerId = null,
            string routerId = null)
        {
            return default;
        }

        public async Task<bool> DeleteByGroupUuid(
            string groupUuid,
            string routerId = null)
        {
            return default;
        }

        public async Task<bool> DeleteByGroupUuid(
            IEnumerable<string> groupUuids,
            string routerId = null)
        {
            return default;
        }

        public async Task<bool> DeleteByMemberUuid(
            string memberUuid,
            string routerId = null)
        {
            return default;
        }

        public async Task<bool> DeleteByMemberUuid(
            IEnumerable<string> memberUuids,
            string routerId = null)
        {
            return default;
        }
}
}
