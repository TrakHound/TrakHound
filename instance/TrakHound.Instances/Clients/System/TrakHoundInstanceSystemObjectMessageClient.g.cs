// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrakHound.Entities;
using TrakHound.Routing;

namespace TrakHound.Clients
{
    internal partial class TrakHoundInstanceSystemObjectMessageClient : TrakHoundInstanceEntityClient<ITrakHoundObjectMessageEntity>, ITrakHoundSystemObjectMessageClient
    {


        public TrakHoundInstanceSystemObjectMessageClient(TrakHoundInstanceClient baseClient) : base(baseClient) 
        {
        }


        public async Task<IEnumerable<ITrakHoundObjectMessageEntity>> QueryByObject(
            string path,
            string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router.Entities.Objects.Messages.QueryByObject(new string[] { path });
                if (response.IsSuccess)
                {
                    return response.Content.ToDistinct();
                }
            }

            return default;
        }

        public async Task<IEnumerable<ITrakHoundObjectMessageEntity>> QueryByObject(
            IEnumerable<string> paths,
            string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router.Entities.Objects.Messages.QueryByObject(paths);
                if (response.IsSuccess)
                {
                    return response.Content.ToDistinct();
                }
            }

            return default;
        }

        public async Task<ITrakHoundObjectMessageEntity> QueryByObjectUuid(
            string objectUuid,
            string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router.Entities.Objects.Messages.QueryByObjectUuid(new string[] { objectUuid });
                if (response.IsSuccess)
                {
                    return response.Content.FirstOrDefault();
                }
            }

            return default;
        }

        public async Task<IEnumerable<ITrakHoundObjectMessageEntity>> QueryByObjectUuid(
            IEnumerable<string> objectUuids,
            string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router.Entities.Objects.Messages.QueryByObjectUuid(objectUuids);
                if (response.IsSuccess)
                {
                    return response.Content.ToDistinct();
                }
            }

            return default;
        }

        public async Task<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectMessageEntity>>> SubscribeByObject(
            IEnumerable<string> paths,
            int interval = 0,
            int take = 1000,
            string consumerId = null,
            string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router.Entities.Objects.Messages.SubscribeByObject(paths);
                if (response.IsSuccess)
                {
                    return new TrakHoundConsumer<IEnumerable<ITrakHoundObjectMessageEntity>>(response.Content);
                }      
            }
            
            return null;

        }

        public async Task<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectMessageEntity>>> SubscribeByObjectUuid(
            IEnumerable<string> objectUuids,
            int interval = 0,
            int take = 1000,
            string consumerId = null,
            string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router.Entities.Objects.Messages.SubscribeByObjectUuid(objectUuids);
                if (response.IsSuccess)
                {
                    return new TrakHoundConsumer<IEnumerable<ITrakHoundObjectMessageEntity>>(response.Content);
                }      
            }
            
            return null;

        }

    }
}
