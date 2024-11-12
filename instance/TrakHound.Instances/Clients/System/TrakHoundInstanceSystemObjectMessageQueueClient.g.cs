// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrakHound.Entities;
using TrakHound.Routing;

namespace TrakHound.Clients
{
    internal partial class TrakHoundInstanceSystemObjectMessageQueueClient : TrakHoundInstanceEntityClient<ITrakHoundObjectMessageQueueEntity>, ITrakHoundSystemObjectMessageQueueClient
    {


        public TrakHoundInstanceSystemObjectMessageQueueClient(TrakHoundInstanceClient baseClient) : base(baseClient) 
        {
        }


        public async Task<IEnumerable<ITrakHoundObjectMessageQueueEntity>> QueryByObject(
            string path,
            string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router.Entities.Objects.MessageQueues.QueryByObject(new string[] { path });
                if (response.IsSuccess)
                {
                    return response.Content.ToDistinct();
                }
            }

            return default;
        }

        public async Task<IEnumerable<ITrakHoundObjectMessageQueueEntity>> QueryByObject(
            IEnumerable<string> paths,
            string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router.Entities.Objects.MessageQueues.QueryByObject(paths);
                if (response.IsSuccess)
                {
                    return response.Content.ToDistinct();
                }
            }

            return default;
        }

        public async Task<ITrakHoundObjectMessageQueueEntity> QueryByObjectUuid(
            string objectUuid,
            string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router.Entities.Objects.MessageQueues.QueryByObjectUuid(new string[] { objectUuid });
                if (response.IsSuccess)
                {
                    return response.Content.FirstOrDefault();
                }
            }

            return default;
        }

        public async Task<IEnumerable<ITrakHoundObjectMessageQueueEntity>> QueryByObjectUuid(
            IEnumerable<string> objectUuids,
            string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router.Entities.Objects.MessageQueues.QueryByObjectUuid(objectUuids);
                if (response.IsSuccess)
                {
                    return response.Content.ToDistinct();
                }
            }

            return default;
        }

        public async Task<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectMessageQueueEntity>>> SubscribeByObject(
            IEnumerable<string> paths,
            int interval = 0,
            int take = 1000,
            string consumerId = null,
            string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router.Entities.Objects.MessageQueues.SubscribeByObject(paths);
                if (response.IsSuccess)
                {
                    return new TrakHoundConsumer<IEnumerable<ITrakHoundObjectMessageQueueEntity>>(response.Content);
                }      
            }
            
            return null;

        }

        public async Task<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectMessageQueueEntity>>> SubscribeByObjectUuid(
            IEnumerable<string> objectUuids,
            int interval = 0,
            int take = 1000,
            string consumerId = null,
            string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router.Entities.Objects.MessageQueues.SubscribeByObjectUuid(objectUuids);
                if (response.IsSuccess)
                {
                    return new TrakHoundConsumer<IEnumerable<ITrakHoundObjectMessageQueueEntity>>(response.Content);
                }      
            }
            
            return null;

        }

    }
}
