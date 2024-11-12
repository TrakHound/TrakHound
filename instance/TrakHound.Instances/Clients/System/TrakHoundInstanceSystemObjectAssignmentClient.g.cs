// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrakHound.Entities;
using TrakHound.Routing;

namespace TrakHound.Clients
{
    internal partial class TrakHoundInstanceSystemObjectAssignmentClient : TrakHoundInstanceEntityClient<ITrakHoundObjectAssignmentEntity>, ITrakHoundSystemObjectAssignmentClient
    {


        public TrakHoundInstanceSystemObjectAssignmentClient(TrakHoundInstanceClient baseClient) : base(baseClient) 
        {
        }


        public async Task<IEnumerable<ITrakHoundObjectAssignmentEntity>> CurrentByAssignee(
            string assigneePath,
            string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router.Entities.Objects.Assignments.CurrentByAssignee(new string[] { assigneePath });
                if (response.IsSuccess)
                {
                    return response.Content.ToDistinct();
                }
            }

            return default;
        }

        public async Task<IEnumerable<ITrakHoundObjectAssignmentEntity>> CurrentByAssignee(
            IEnumerable<string> assigneePaths,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router.Entities.Objects.Assignments.CurrentByAssignee(assigneePaths,skip,take,sortOrder);
                if (response.IsSuccess)
                {
                    return response.Content.ToDistinct();
                }
            }

            return default;
        }

        public async Task<IEnumerable<ITrakHoundObjectAssignmentEntity>> CurrentByAssigneeUuid(
            string assigneeUuid,
            string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router.Entities.Objects.Assignments.CurrentByAssigneeUuid(new string[] { assigneeUuid });
                if (response.IsSuccess)
                {
                    return response.Content.ToDistinct();
                }
            }

            return default;
        }

        public async Task<IEnumerable<ITrakHoundObjectAssignmentEntity>> CurrentByAssigneeUuid(
            IEnumerable<string> assigneeUuids,
            string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router.Entities.Objects.Assignments.CurrentByAssigneeUuid(assigneeUuids);
                if (response.IsSuccess)
                {
                    return response.Content.ToDistinct();
                }
            }

            return default;
        }

        public async Task<IEnumerable<ITrakHoundObjectAssignmentEntity>> CurrentByMember(
            string memberPath,
            string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router.Entities.Objects.Assignments.CurrentByMember(new string[] { memberPath });
                if (response.IsSuccess)
                {
                    return response.Content.ToDistinct();
                }
            }

            return default;
        }

        public async Task<IEnumerable<ITrakHoundObjectAssignmentEntity>> CurrentByMember(
            IEnumerable<string> memberPaths,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router.Entities.Objects.Assignments.CurrentByMember(memberPaths,skip,take,sortOrder);
                if (response.IsSuccess)
                {
                    return response.Content.ToDistinct();
                }
            }

            return default;
        }

        public async Task<IEnumerable<ITrakHoundObjectAssignmentEntity>> CurrentByMemberUuid(
            string memberUuid,
            string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router.Entities.Objects.Assignments.CurrentByMemberUuid(new string[] { memberUuid });
                if (response.IsSuccess)
                {
                    return response.Content.ToDistinct();
                }
            }

            return default;
        }

        public async Task<IEnumerable<ITrakHoundObjectAssignmentEntity>> CurrentByMemberUuid(
            IEnumerable<string> memberUuids,
            string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router.Entities.Objects.Assignments.CurrentByMemberUuid(memberUuids);
                if (response.IsSuccess)
                {
                    return response.Content.ToDistinct();
                }
            }

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
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router.Entities.Objects.Assignments.QueryByAssignee(new string[] { assigneePath },start,stop,skip,take,sortOrder);
                if (response.IsSuccess)
                {
                    return response.Content.ToDistinct();
                }
            }

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
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router.Entities.Objects.Assignments.QueryByAssignee(assigneePaths,start,stop,skip,take,sortOrder);
                if (response.IsSuccess)
                {
                    return response.Content.ToDistinct();
                }
            }

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
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router.Entities.Objects.Assignments.QueryByAssigneeUuid(new string[] { assigneeUuid },start,stop,skip,take,sortOrder);
                if (response.IsSuccess)
                {
                    return response.Content.ToDistinct();
                }
            }

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
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router.Entities.Objects.Assignments.QueryByAssigneeUuid(assigneeUuids,start,stop,skip,take,sortOrder);
                if (response.IsSuccess)
                {
                    return response.Content.ToDistinct();
                }
            }

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
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router.Entities.Objects.Assignments.QueryByMember(new string[] { memberPath },start,stop,skip,take,sortOrder);
                if (response.IsSuccess)
                {
                    return response.Content.ToDistinct();
                }
            }

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
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router.Entities.Objects.Assignments.QueryByMember(memberPaths,start,stop,skip,take,sortOrder);
                if (response.IsSuccess)
                {
                    return response.Content.ToDistinct();
                }
            }

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
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router.Entities.Objects.Assignments.QueryByMemberUuid(new string[] { memberUuid },start,stop,skip,take,sortOrder);
                if (response.IsSuccess)
                {
                    return response.Content.ToDistinct();
                }
            }

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
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router.Entities.Objects.Assignments.QueryByMemberUuid(memberUuids,start,stop,skip,take,sortOrder);
                if (response.IsSuccess)
                {
                    return response.Content.ToDistinct();
                }
            }

            return default;
        }

        public async Task<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectAssignmentEntity>>> SubscribeByAssignee(
            IEnumerable<string> paths,
            int interval = 0,
            int take = 1000,
            string consumerId = null,
            string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router.Entities.Objects.Assignments.SubscribeByAssignee(paths);
                if (response.IsSuccess)
                {
                    return new TrakHoundConsumer<IEnumerable<ITrakHoundObjectAssignmentEntity>>(response.Content);
                }      
            }
            
            return null;

        }

        public async Task<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectAssignmentEntity>>> SubscribeByAssigneeUuid(
            IEnumerable<string> assigneeUuids,
            int interval = 0,
            int take = 1000,
            string consumerId = null,
            string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router.Entities.Objects.Assignments.SubscribeByAssigneeUuid(assigneeUuids);
                if (response.IsSuccess)
                {
                    return new TrakHoundConsumer<IEnumerable<ITrakHoundObjectAssignmentEntity>>(response.Content);
                }      
            }
            
            return null;

        }

        public async Task<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectAssignmentEntity>>> SubscribeByMember(
            IEnumerable<string> paths,
            int interval = 0,
            int take = 1000,
            string consumerId = null,
            string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router.Entities.Objects.Assignments.SubscribeByMember(paths);
                if (response.IsSuccess)
                {
                    return new TrakHoundConsumer<IEnumerable<ITrakHoundObjectAssignmentEntity>>(response.Content);
                }      
            }
            
            return null;

        }

        public async Task<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectAssignmentEntity>>> SubscribeByMemberUuid(
            IEnumerable<string> memberUuids,
            int interval = 0,
            int take = 1000,
            string consumerId = null,
            string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router.Entities.Objects.Assignments.SubscribeByMemberUuid(memberUuids);
                if (response.IsSuccess)
                {
                    return new TrakHoundConsumer<IEnumerable<ITrakHoundObjectAssignmentEntity>>(response.Content);
                }      
            }
            
            return null;

        }

        public async Task<bool> DeleteByAssigneeUuid(
            string assigneeUuid,
            string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router.Entities.Objects.Assignments.DeleteByAssigneeUuid(new string[] { assigneeUuid });
                if (response.IsSuccess)
                {
                    return response.Content.FirstOrDefault();
                }
            }

            return default;
        }

        public async Task<bool> DeleteByAssigneeUuid(
            IEnumerable<string> assigneeUuids,
            string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router.Entities.Objects.Assignments.DeleteByAssigneeUuid(assigneeUuids);
                if (response.IsSuccess)
                {
                    return response.Content.FirstOrDefault();
                }
            }

            return default;
        }

        public async Task<bool> DeleteByMemberUuid(
            string memberUuid,
            string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router.Entities.Objects.Assignments.DeleteByMemberUuid(new string[] { memberUuid });
                if (response.IsSuccess)
                {
                    return response.Content.FirstOrDefault();
                }
            }

            return default;
        }

        public async Task<bool> DeleteByMemberUuid(
            IEnumerable<string> memberUuids,
            string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router.Entities.Objects.Assignments.DeleteByMemberUuid(memberUuids);
                if (response.IsSuccess)
                {
                    return response.Content.FirstOrDefault();
                }
            }

            return default;
        }

    }
}
