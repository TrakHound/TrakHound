// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrakHound.Entities;
using TrakHound.Routing;

namespace TrakHound.Clients
{
    internal partial class TrakHoundInstanceSystemObjectStatisticClient : TrakHoundInstanceEntityClient<ITrakHoundObjectStatisticEntity>, ITrakHoundSystemObjectStatisticClient
    {


        public TrakHoundInstanceSystemObjectStatisticClient(TrakHoundInstanceClient baseClient) : base(baseClient) 
        {
        }


        public async Task<IEnumerable<ITrakHoundObjectStatisticEntity>> QueryByObject(
            string path,
            long start,
            long stop,
            long span,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router.Entities.Objects.Statistics.QueryByObject(new string[] { path },start,stop,span,skip,take,sortOrder);
                if (response.IsSuccess)
                {
                    return response.Content.ToDistinct();
                }
            }

            return default;
        }

        public async Task<IEnumerable<ITrakHoundObjectStatisticEntity>> QueryByObject(
            IEnumerable<string> paths,
            long start,
            long stop,
            long span,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router.Entities.Objects.Statistics.QueryByObject(paths,start,stop,span,skip,take,sortOrder);
                if (response.IsSuccess)
                {
                    return response.Content.ToDistinct();
                }
            }

            return default;
        }

        public async Task<IEnumerable<ITrakHoundObjectStatisticEntity>> QueryByObjectUuid(
            string objectUuid,
            long start,
            long stop,
            long span,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router.Entities.Objects.Statistics.QueryByObjectUuid(new string[] { objectUuid },start,stop,span,skip,take,sortOrder);
                if (response.IsSuccess)
                {
                    return response.Content.ToDistinct();
                }
            }

            return default;
        }

        public async Task<IEnumerable<ITrakHoundObjectStatisticEntity>> QueryByObjectUuid(
            IEnumerable<string> objectUuids,
            long start,
            long stop,
            long span,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router.Entities.Objects.Statistics.QueryByObjectUuid(objectUuids,start,stop,span,skip,take,sortOrder);
                if (response.IsSuccess)
                {
                    return response.Content.ToDistinct();
                }
            }

            return default;
        }

        public async Task<IEnumerable<TrakHoundTimeRangeSpan>> SpansByObject(
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
                var response = await router.Entities.Objects.Statistics.SpansByObject(new string[] { path },start,stop,skip,take,sortOrder);
                if (response.IsSuccess)
                {
                    return response.Content;
                }
            }

            return default;
        }

        public async Task<IEnumerable<TrakHoundTimeRangeSpan>> SpansByObject(
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
                var response = await router.Entities.Objects.Statistics.SpansByObject(paths,start,stop,skip,take,sortOrder);
                if (response.IsSuccess)
                {
                    return response.Content;
                }
            }

            return default;
        }

        public async Task<IEnumerable<TrakHoundTimeRangeSpan>> SpansByObjectUuid(
            string objectUuid,
            long start,
            long stop,
            string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router.Entities.Objects.Statistics.SpansByObjectUuid(new string[] { objectUuid },start,stop);
                if (response.IsSuccess)
                {
                    return response.Content;
                }
            }

            return default;
        }

        public async Task<IEnumerable<TrakHoundTimeRangeSpan>> SpansByObjectUuid(
            IEnumerable<string> objectUuids,
            long start,
            long stop,
            string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router.Entities.Objects.Statistics.SpansByObjectUuid(objectUuids,start,stop);
                if (response.IsSuccess)
                {
                    return response.Content;
                }
            }

            return default;
        }

        public async Task<IEnumerable<TrakHoundCount>> CountByObject(
            string path,
            long start,
            long stop,
            long span,
            string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router.Entities.Objects.Statistics.CountByObject(new string[] { path },start,stop,span);
                if (response.IsSuccess)
                {
                    return response.Content;
                }
            }

            return default;
        }

        public async Task<IEnumerable<TrakHoundCount>> CountByObject(
            IEnumerable<string> paths,
            long start,
            long stop,
            long span,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router.Entities.Objects.Statistics.CountByObject(paths,start,stop,span,skip,take,sortOrder);
                if (response.IsSuccess)
                {
                    return response.Content;
                }
            }

            return default;
        }

        public async Task<TrakHoundCount> CountByObjectUuid(
            string objectUuid,
            long start,
            long stop,
            long span,
            string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router.Entities.Objects.Statistics.CountByObjectUuid(new string[] { objectUuid },start,stop,span);
                if (response.IsSuccess)
                {
                    return response.Content.FirstOrDefault();
                }
            }

            return default;
        }

        public async Task<IEnumerable<TrakHoundCount>> CountByObjectUuid(
            IEnumerable<string> objectUuids,
            long start,
            long stop,
            long span,
            string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router.Entities.Objects.Statistics.CountByObjectUuid(objectUuids,start,stop,span);
                if (response.IsSuccess)
                {
                    return response.Content;
                }
            }

            return default;
        }

        public async Task<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectStatisticEntity>>> SubscribeByObject(
            IEnumerable<string> paths,
            int interval = 0,
            int take = 1000,
            string consumerId = null,
            string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router.Entities.Objects.Statistics.SubscribeByObject(paths);
                if (response.IsSuccess)
                {
                    return new TrakHoundConsumer<IEnumerable<ITrakHoundObjectStatisticEntity>>(response.Content);
                }      
            }
            
            return null;

        }

        public async Task<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectStatisticEntity>>> SubscribeByObjectUuid(
            IEnumerable<string> objectUuids,
            int interval = 0,
            int take = 1000,
            string consumerId = null,
            string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router.Entities.Objects.Statistics.SubscribeByObjectUuid(objectUuids);
                if (response.IsSuccess)
                {
                    return new TrakHoundConsumer<IEnumerable<ITrakHoundObjectStatisticEntity>>(response.Content);
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
                var response = await router.Entities.Objects.Statistics.DeleteByObjectUuid(new string[] { objectUuid });
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
                var response = await router.Entities.Objects.Statistics.DeleteByObjectUuid(objectUuids);
                if (response.IsSuccess)
                {
                    return response.Content.FirstOrDefault();
                }
            }

            return default;
        }

    }
}
