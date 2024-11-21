// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrakHound.Entities;
using TrakHound.Routing;

namespace TrakHound.Clients
{
    internal partial class TrakHoundInstanceSystemObjectObservationClient : TrakHoundInstanceEntityClient<ITrakHoundObjectObservationEntity>, ITrakHoundSystemObjectObservationClient
    {


        public TrakHoundInstanceSystemObjectObservationClient(TrakHoundInstanceClient baseClient) : base(baseClient) 
        {
        }


        public async Task<IEnumerable<ITrakHoundObjectObservationEntity>> LatestByObject(
            string path,
            string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router.Entities.Objects.Observations.LatestByObject(new string[] { path });
                if (response.IsSuccess)
                {
                    return response.Content.ToDistinct();
                }
            }

            return default;
        }

        public async Task<IEnumerable<ITrakHoundObjectObservationEntity>> LatestByObject(
            IEnumerable<string> paths,
            string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router.Entities.Objects.Observations.LatestByObject(paths);
                if (response.IsSuccess)
                {
                    return response.Content.ToDistinct();
                }
            }

            return default;
        }

        public async Task<IEnumerable<ITrakHoundObjectObservationEntity>> LatestByObjectUuid(
            string objectUuid,
            string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router.Entities.Objects.Observations.LatestByObjectUuid(new string[] { objectUuid });
                if (response.IsSuccess)
                {
                    return response.Content.ToDistinct();
                }
            }

            return default;
        }

        public async Task<IEnumerable<ITrakHoundObjectObservationEntity>> LatestByObjectUuid(
            IEnumerable<string> objectUuids,
            string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router.Entities.Objects.Observations.LatestByObjectUuid(objectUuids);
                if (response.IsSuccess)
                {
                    return response.Content.ToDistinct();
                }
            }

            return default;
        }

        public async Task<IEnumerable<ITrakHoundObjectObservationEntity>> QueryByObject(
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
                var response = await router.Entities.Objects.Observations.QueryByObject(new string[] { path },start,stop,skip,take,sortOrder);
                if (response.IsSuccess)
                {
                    return response.Content.ToDistinct();
                }
            }

            return default;
        }

        public async Task<IEnumerable<ITrakHoundObjectObservationEntity>> QueryByObject(
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
                var response = await router.Entities.Objects.Observations.QueryByObject(paths,start,stop,skip,take,sortOrder);
                if (response.IsSuccess)
                {
                    return response.Content.ToDistinct();
                }
            }

            return default;
        }

        public async Task<IEnumerable<ITrakHoundObjectObservationEntity>> QueryByObjectUuid(
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
                var response = await router.Entities.Objects.Observations.QueryByObjectUuid(new string[] { objectUuid },start,stop,skip,take,sortOrder);
                if (response.IsSuccess)
                {
                    return response.Content.ToDistinct();
                }
            }

            return default;
        }

        public async Task<IEnumerable<ITrakHoundObjectObservationEntity>> QueryByObjectUuid(
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
                var response = await router.Entities.Objects.Observations.QueryByObjectUuid(objectUuids,start,stop,skip,take,sortOrder);
                if (response.IsSuccess)
                {
                    return response.Content.ToDistinct();
                }
            }

            return default;
        }

        public async Task<IEnumerable<ITrakHoundObjectObservationEntity>> LastByObject(
            string path,
            long timestamp,
            string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router.Entities.Objects.Observations.LastByObject(new string[] { path },timestamp);
                if (response.IsSuccess)
                {
                    return response.Content.ToDistinct();
                }
            }

            return default;
        }

        public async Task<IEnumerable<ITrakHoundObjectObservationEntity>> LastByObject(
            IEnumerable<string> paths,
            long timestamp,
            string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router.Entities.Objects.Observations.LastByObject(paths,timestamp);
                if (response.IsSuccess)
                {
                    return response.Content.ToDistinct();
                }
            }

            return default;
        }

        public async Task<IEnumerable<ITrakHoundObjectObservationEntity>> LastByObjectUuid(
            string objectUuid,
            long timestamp,
            string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router.Entities.Objects.Observations.LastByObjectUuid(new string[] { objectUuid },timestamp);
                if (response.IsSuccess)
                {
                    return response.Content.ToDistinct();
                }
            }

            return default;
        }

        public async Task<IEnumerable<ITrakHoundObjectObservationEntity>> LastByObjectUuid(
            IEnumerable<string> objectUuids,
            long timestamp,
            string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router.Entities.Objects.Observations.LastByObjectUuid(objectUuids,timestamp);
                if (response.IsSuccess)
                {
                    return response.Content.ToDistinct();
                }
            }

            return default;
        }

        public async Task<IEnumerable<TrakHoundAggregate>> AggregateByObject(
            string path,
            TrakHound.TrakHoundAggregateType aggregateType,
            long start,
            long stop,
            string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router.Entities.Objects.Observations.AggregateByObject(new string[] { path },aggregateType,start,stop);
                if (response.IsSuccess)
                {
                    return response.Content;
                }
            }

            return default;
        }

        public async Task<IEnumerable<TrakHoundAggregate>> AggregateByObject(
            IEnumerable<string> paths,
            TrakHound.TrakHoundAggregateType aggregateType,
            long start,
            long stop,
            string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router.Entities.Objects.Observations.AggregateByObject(paths,aggregateType,start,stop);
                if (response.IsSuccess)
                {
                    return response.Content;
                }
            }

            return default;
        }

        public async Task<IEnumerable<TrakHoundAggregate>> AggregateByObjectUuid(
            string objectUuid,
            TrakHound.TrakHoundAggregateType aggregateType,
            long start,
            long stop,
            string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router.Entities.Objects.Observations.AggregateByObjectUuid(new string[] { objectUuid },aggregateType,start,stop);
                if (response.IsSuccess)
                {
                    return response.Content;
                }
            }

            return default;
        }

        public async Task<IEnumerable<TrakHoundAggregate>> AggregateByObjectUuid(
            IEnumerable<string> objectUuids,
            TrakHound.TrakHoundAggregateType aggregateType,
            long start,
            long stop,
            string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router.Entities.Objects.Observations.AggregateByObjectUuid(objectUuids,aggregateType,start,stop);
                if (response.IsSuccess)
                {
                    return response.Content;
                }
            }

            return default;
        }

        public async Task<IEnumerable<TrakHoundAggregateWindow>> AggregateWindowByObject(
            string path,
            TrakHound.TrakHoundAggregateType aggregateType,
            long window,
            long start,
            long stop,
            string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router.Entities.Objects.Observations.AggregateWindowByObject(new string[] { path },aggregateType,window,start,stop);
                if (response.IsSuccess)
                {
                    return response.Content;
                }
            }

            return default;
        }

        public async Task<IEnumerable<TrakHoundAggregateWindow>> AggregateWindowByObject(
            IEnumerable<string> paths,
            TrakHound.TrakHoundAggregateType aggregateType,
            long window,
            long start,
            long stop,
            string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router.Entities.Objects.Observations.AggregateWindowByObject(paths,aggregateType,window,start,stop);
                if (response.IsSuccess)
                {
                    return response.Content;
                }
            }

            return default;
        }

        public async Task<IEnumerable<TrakHoundAggregateWindow>> AggregateWindowByObjectUuid(
            string objectUuid,
            TrakHound.TrakHoundAggregateType aggregateType,
            long window,
            long start,
            long stop,
            string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router.Entities.Objects.Observations.AggregateWindowByObjectUuid(new string[] { objectUuid },aggregateType,window,start,stop);
                if (response.IsSuccess)
                {
                    return response.Content;
                }
            }

            return default;
        }

        public async Task<IEnumerable<TrakHoundAggregateWindow>> AggregateWindowByObjectUuid(
            IEnumerable<string> objectUuids,
            TrakHound.TrakHoundAggregateType aggregateType,
            long window,
            long start,
            long stop,
            string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router.Entities.Objects.Observations.AggregateWindowByObjectUuid(objectUuids,aggregateType,window,start,stop);
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
            string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router.Entities.Objects.Observations.CountByObject(new string[] { path },start,stop);
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
            string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router.Entities.Objects.Observations.CountByObject(paths,start,stop);
                if (response.IsSuccess)
                {
                    return response.Content;
                }
            }

            return default;
        }

        public async Task<IEnumerable<TrakHoundCount>> CountByObjectUuid(
            string objectUuid,
            long start,
            long stop,
            string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router.Entities.Objects.Observations.CountByObjectUuid(new string[] { objectUuid },start,stop);
                if (response.IsSuccess)
                {
                    return response.Content;
                }
            }

            return default;
        }

        public async Task<IEnumerable<TrakHoundCount>> CountByObjectUuid(
            IEnumerable<string> objectUuids,
            long start,
            long stop,
            string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router.Entities.Objects.Observations.CountByObjectUuid(objectUuids,start,stop);
                if (response.IsSuccess)
                {
                    return response.Content;
                }
            }

            return default;
        }

        public async Task<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectObservationEntity>>> SubscribeByObject(
            IEnumerable<string> paths,
            int interval = 0,
            int take = 1000,
            string consumerId = null,
            string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router.Entities.Objects.Observations.SubscribeByObject(paths);
                if (response.IsSuccess)
                {
                    return new TrakHoundConsumer<IEnumerable<ITrakHoundObjectObservationEntity>>(response.Content);
                }      
            }
            
            return null;

        }

        public async Task<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectObservationEntity>>> SubscribeByObjectUuid(
            IEnumerable<string> objectUuids,
            int interval = 0,
            int take = 1000,
            string consumerId = null,
            string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router.Entities.Objects.Observations.SubscribeByObjectUuid(objectUuids);
                if (response.IsSuccess)
                {
                    return new TrakHoundConsumer<IEnumerable<ITrakHoundObjectObservationEntity>>(response.Content);
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
                var response = await router.Entities.Objects.Observations.DeleteByObjectUuid(new string[] { objectUuid });
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
                var response = await router.Entities.Objects.Observations.DeleteByObjectUuid(objectUuids);
                if (response.IsSuccess)
                {
                    return response.Content.FirstOrDefault();
                }
            }

            return default;
        }

    }
}
