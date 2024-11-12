// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrakHound.Entities;
using TrakHound.Entities.Collections;

namespace TrakHound.Clients.Collections
{
    internal partial class TrakHoundCollectionObjectAssignmentClient : TrakHoundCollectionEntityClient<ITrakHoundObjectAssignmentEntity>, ITrakHoundSystemObjectAssignmentClient
    {
        private readonly TrakHoundEntityCollection _collection;






        public TrakHoundCollectionObjectAssignmentClient(TrakHoundEntityCollection collection) : base(collection) 
        { 
            _collection = collection;


        }


        public async Task<IEnumerable<ITrakHoundObjectAssignmentEntity>> CurrentByAssignee(
            string assigneePath,
            string routerId = null)
        {
            return default;
        }

        public async Task<IEnumerable<ITrakHoundObjectAssignmentEntity>> CurrentByAssignee(
            IEnumerable<string> assigneePaths,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null)
        {
            return default;
        }

        public async Task<IEnumerable<ITrakHoundObjectAssignmentEntity>> CurrentByAssigneeUuid(
            string assigneeUuid,
            string routerId = null)
        {
            return default;
        }

        public async Task<IEnumerable<ITrakHoundObjectAssignmentEntity>> CurrentByAssigneeUuid(
            IEnumerable<string> assigneeUuids,
            string routerId = null)
        {
            return default;
        }

        public async Task<IEnumerable<ITrakHoundObjectAssignmentEntity>> CurrentByMember(
            string memberPath,
            string routerId = null)
        {
            return default;
        }

        public async Task<IEnumerable<ITrakHoundObjectAssignmentEntity>> CurrentByMember(
            IEnumerable<string> memberPaths,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null)
        {
            return default;
        }

        public async Task<IEnumerable<ITrakHoundObjectAssignmentEntity>> CurrentByMemberUuid(
            string memberUuid,
            string routerId = null)
        {
            return default;
        }

        public async Task<IEnumerable<ITrakHoundObjectAssignmentEntity>> CurrentByMemberUuid(
            IEnumerable<string> memberUuids,
            string routerId = null)
        {
            return default;
        }

        public async Task<IEnumerable<ITrakHoundObjectAssignmentEntity>> QueryByAssignee(
            string assigneePath,
            long start,
            long stop,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null)
        {
            return default;
        }

        public async Task<IEnumerable<ITrakHoundObjectAssignmentEntity>> QueryByAssignee(
            IEnumerable<string> assigneePaths,
            long start,
            long stop,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null)
        {
            return default;
        }

        public async Task<IEnumerable<ITrakHoundObjectAssignmentEntity>> QueryByAssigneeUuid(
            string assigneeUuid,
            long start,
            long stop,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null)
        {
            return default;
        }

        public async Task<IEnumerable<ITrakHoundObjectAssignmentEntity>> QueryByAssigneeUuid(
            IEnumerable<string> assigneeUuids,
            long start,
            long stop,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null)
        {
            return default;
        }

        public async Task<IEnumerable<ITrakHoundObjectAssignmentEntity>> QueryByMember(
            string memberPath,
            long start,
            long stop,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null)
        {
            return default;
        }

        public async Task<IEnumerable<ITrakHoundObjectAssignmentEntity>> QueryByMember(
            IEnumerable<string> memberPaths,
            long start,
            long stop,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null)
        {
            return default;
        }

        public async Task<IEnumerable<ITrakHoundObjectAssignmentEntity>> QueryByMemberUuid(
            string memberUuid,
            long start,
            long stop,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null)
        {
            return default;
        }

        public async Task<IEnumerable<ITrakHoundObjectAssignmentEntity>> QueryByMemberUuid(
            IEnumerable<string> memberUuids,
            long start,
            long stop,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null)
        {
            return default;
        }

        public async Task<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectAssignmentEntity>>> SubscribeByAssignee(
            IEnumerable<string> paths,
            int interval = 0,
            int take = 1000,
            string consumerId = null,
            string routerId = null)
        {
            return default;
        }

        public async Task<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectAssignmentEntity>>> SubscribeByAssigneeUuid(
            IEnumerable<string> assigneeUuids,
            int interval = 0,
            int take = 1000,
            string consumerId = null,
            string routerId = null)
        {
            return default;
        }

        public async Task<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectAssignmentEntity>>> SubscribeByMember(
            IEnumerable<string> paths,
            int interval = 0,
            int take = 1000,
            string consumerId = null,
            string routerId = null)
        {
            return default;
        }

        public async Task<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectAssignmentEntity>>> SubscribeByMemberUuid(
            IEnumerable<string> memberUuids,
            int interval = 0,
            int take = 1000,
            string consumerId = null,
            string routerId = null)
        {
            return default;
        }

        public async Task<bool> DeleteByAssigneeUuid(
            string assigneeUuid,
            string routerId = null)
        {
            return default;
        }

        public async Task<bool> DeleteByAssigneeUuid(
            IEnumerable<string> assigneeUuids,
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
