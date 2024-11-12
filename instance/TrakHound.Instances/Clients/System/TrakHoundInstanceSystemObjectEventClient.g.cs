// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrakHound.Entities;
using TrakHound.Routing;

namespace TrakHound.Clients
{
    internal partial class TrakHoundInstanceSystemObjectEventClient : TrakHoundInstanceEntityClient<ITrakHoundObjectEventEntity>, ITrakHoundSystemObjectEventClient
    {


        public TrakHoundInstanceSystemObjectEventClient(TrakHoundInstanceClient baseClient) : base(baseClient) 
        {
        }


        public async Task<IEnumerable<ITrakHoundObjectEventEntity>> LatestByObject(
            string path,
            string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router.Entities.Objects.Events.LatestByObject(new string[] { path });
                if (response.IsSuccess)
                {
                    return response.Content.ToDistinct();
                }
            }

            return default;
        }

        public async Task<IEnumerable<ITrakHoundObjectEventEntity>> LatestByObject(
            IEnumerable<string> paths,
            string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router.Entities.Objects.Events.LatestByObject(paths);
                if (response.IsSuccess)
                {
                    return response.Content.ToDistinct();
                }
            }

            return default;
        }

        public async Task<IEnumerable<ITrakHoundObjectEventEntity>> LatestByObjectUuid(
            string objectUuid,
            string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router.Entities.Objects.Events.LatestByObjectUuid(new string[] { objectUuid });
                if (response.IsSuccess)
                {
                    return response.Content.ToDistinct();
                }
            }

            return default;
        }

        public async Task<IEnumerable<ITrakHoundObjectEventEntity>> LatestByObjectUuid(
            IEnumerable<string> objectUuids,
            string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router.Entities.Objects.Events.LatestByObjectUuid(objectUuids);
                if (response.IsSuccess)
                {
                    return response.Content.ToDistinct();
                }
            }

            return default;
        }

        public async Task<IEnumerable<ITrakHoundObjectEventEntity>> QueryByObject(
            string path,
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
                var response = await router.Entities.Objects.Events.QueryByObject(new string[] { path },start,stop,skip,take,sortOrder);
                if (response.IsSuccess)
                {
                    return response.Content.ToDistinct();
                }
            }

            return default;
        }

        public async Task<IEnumerable<ITrakHoundObjectEventEntity>> QueryByObject(
            IEnumerable<string> paths,
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
                var response = await router.Entities.Objects.Events.QueryByObject(paths,start,stop,skip,take,sortOrder);
                if (response.IsSuccess)
                {
                    return response.Content.ToDistinct();
                }
            }

            return default;
        }

        public async Task<IEnumerable<ITrakHoundObjectEventEntity>> QueryByObjectUuid(
            string objectUuid,
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
                var response = await router.Entities.Objects.Events.QueryByObjectUuid(new string[] { objectUuid },start,stop,skip,take,sortOrder);
                if (response.IsSuccess)
                {
                    return response.Content.ToDistinct();
                }
            }

            return default;
        }

        public async Task<IEnumerable<ITrakHoundObjectEventEntity>> QueryByObjectUuid(
            IEnumerable<string> objectUuids,
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
                var response = await router.Entities.Objects.Events.QueryByObjectUuid(objectUuids,start,stop,skip,take,sortOrder);
                if (response.IsSuccess)
                {
                    return response.Content.ToDistinct();
                }
            }

            return default;
        }

        public async Task<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectEventEntity>>> SubscribeByObject(
            IEnumerable<string> paths,
            int interval = 0,
            int take = 1000,
            string consumerId = null,
            string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router.Entities.Objects.Events.SubscribeByObject(paths);
                if (response.IsSuccess)
                {
                    return new TrakHoundConsumer<IEnumerable<ITrakHoundObjectEventEntity>>(response.Content);
                }      
            }
            
            return null;

        }

        public async Task<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectEventEntity>>> SubscribeByObjectUuid(
            IEnumerable<string> objectUuids,
            int interval = 0,
            int take = 1000,
            string consumerId = null,
            string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router.Entities.Objects.Events.SubscribeByObjectUuid(objectUuids);
                if (response.IsSuccess)
                {
                    return new TrakHoundConsumer<IEnumerable<ITrakHoundObjectEventEntity>>(response.Content);
                }      
            }
            
            return null;

        }

        public async Task<bool> DeleteByObjectUuid(
            string objectUuid,
            string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router.Entities.Objects.Events.DeleteByObjectUuid(new string[] { objectUuid });
                if (response.IsSuccess)
                {
                    return response.Content.FirstOrDefault();
                }
            }

            return default;
        }

        public async Task<bool> DeleteByObjectUuid(
            IEnumerable<string> objectUuids,
            string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router.Entities.Objects.Events.DeleteByObjectUuid(objectUuids);
                if (response.IsSuccess)
                {
                    return response.Content.FirstOrDefault();
                }
            }

            return default;
        }

    }
}
