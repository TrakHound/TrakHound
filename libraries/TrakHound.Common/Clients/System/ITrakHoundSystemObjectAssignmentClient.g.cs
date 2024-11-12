// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Entities;

namespace TrakHound.Clients
{
    public partial interface ITrakHoundSystemObjectAssignmentClient : ITrakHoundEntityClient<ITrakHoundObjectAssignmentEntity>
    {
        Task<IEnumerable<ITrakHoundObjectAssignmentEntity>> CurrentByAssignee(
            string assigneePath,
            string routerId = null);

        Task<IEnumerable<ITrakHoundObjectAssignmentEntity>> CurrentByAssignee(
            IEnumerable<string> assigneePaths,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null);

        Task<IEnumerable<ITrakHoundObjectAssignmentEntity>> CurrentByAssigneeUuid(
            string assigneeUuid,
            string routerId = null);

        Task<IEnumerable<ITrakHoundObjectAssignmentEntity>> CurrentByAssigneeUuid(
            IEnumerable<string> assigneeUuids,
            string routerId = null);

        Task<IEnumerable<ITrakHoundObjectAssignmentEntity>> CurrentByMember(
            string memberPath,
            string routerId = null);

        Task<IEnumerable<ITrakHoundObjectAssignmentEntity>> CurrentByMember(
            IEnumerable<string> memberPaths,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null);

        Task<IEnumerable<ITrakHoundObjectAssignmentEntity>> CurrentByMemberUuid(
            string memberUuid,
            string routerId = null);

        Task<IEnumerable<ITrakHoundObjectAssignmentEntity>> CurrentByMemberUuid(
            IEnumerable<string> memberUuids,
            string routerId = null);

        Task<IEnumerable<ITrakHoundObjectAssignmentEntity>> QueryByAssignee(
            string assigneePath,
            long start,
            long stop,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null);

        Task<IEnumerable<ITrakHoundObjectAssignmentEntity>> QueryByAssignee(
            IEnumerable<string> assigneePaths,
            long start,
            long stop,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null);

        Task<IEnumerable<ITrakHoundObjectAssignmentEntity>> QueryByAssigneeUuid(
            string assigneeUuid,
            long start,
            long stop,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null);

        Task<IEnumerable<ITrakHoundObjectAssignmentEntity>> QueryByAssigneeUuid(
            IEnumerable<string> assigneeUuids,
            long start,
            long stop,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null);

        Task<IEnumerable<ITrakHoundObjectAssignmentEntity>> QueryByMember(
            string memberPath,
            long start,
            long stop,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null);

        Task<IEnumerable<ITrakHoundObjectAssignmentEntity>> QueryByMember(
            IEnumerable<string> memberPaths,
            long start,
            long stop,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null);

        Task<IEnumerable<ITrakHoundObjectAssignmentEntity>> QueryByMemberUuid(
            string memberUuid,
            long start,
            long stop,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null);

        Task<IEnumerable<ITrakHoundObjectAssignmentEntity>> QueryByMemberUuid(
            IEnumerable<string> memberUuids,
            long start,
            long stop,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null);

        Task<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectAssignmentEntity>>> SubscribeByAssignee(
            IEnumerable<string> paths,
            int interval = 0,
            int take = 1000,
            string consumerId = null,
            string routerId = null);

        Task<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectAssignmentEntity>>> SubscribeByAssigneeUuid(
            IEnumerable<string> assigneeUuids,
            int interval = 0,
            int take = 1000,
            string consumerId = null,
            string routerId = null);

        Task<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectAssignmentEntity>>> SubscribeByMember(
            IEnumerable<string> paths,
            int interval = 0,
            int take = 1000,
            string consumerId = null,
            string routerId = null);

        Task<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectAssignmentEntity>>> SubscribeByMemberUuid(
            IEnumerable<string> memberUuids,
            int interval = 0,
            int take = 1000,
            string consumerId = null,
            string routerId = null);

        Task<bool> DeleteByAssigneeUuid(
            string assigneeUuid,
            string routerId = null);

        Task<bool> DeleteByAssigneeUuid(
            IEnumerable<string> assigneeUuids,
            string routerId = null);

        Task<bool> DeleteByMemberUuid(
            string memberUuid,
            string routerId = null);

        Task<bool> DeleteByMemberUuid(
            IEnumerable<string> memberUuids,
            string routerId = null);

    }
}