// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TrakHound.Entities;

namespace TrakHound.Clients
{
    internal partial class TrakHoundInstanceSystemObjectMessageQueueClient
    {

        //public async Task<ITrakHoundConsumer<IEnumerable<TrakHoundEntityNotification>>> Notify(string path, TrakHoundEntityNotificationType notificationType = TrakHoundEntityNotificationType.All, string routerId = null)
        //{
        //    if (!string.IsNullOrEmpty(path))
        //    {
        //        var router = BaseClient.GetRouter(routerId);
        //        if (router != null)
        //        {
        //            var response = await router.Entities.Objects.MessageQueues.Notify(path, notificationType);
        //            if (response.IsSuccess)
        //            {
        //                return new TrakHoundConsumer<IEnumerable<TrakHoundEntityNotification>>(response.Content);
        //            }
        //        }
        //    }

        //    return null;
        //}


        //public async Task<ITrakHoundConsumer<TrakHoundAcknowledgeResult<ITrakHoundObjectMessageQueueEntity>>> SubscribeByObjectUuid(string objectUuid, bool acknowledge = true, string routerId = null)
        //{
        //    if (!string.IsNullOrEmpty(objectUuid))
        //    {
        //        var router = BaseClient.GetRouter(routerId);
        //        if (router != null)
        //        {
        //            var response = await router.Entities.Objects.MessageQueues.SubscribeByObjectUuid(objectUuid, acknowledge);
        //            if (response.IsSuccess)
        //            {
        //                return new TrakHoundConsumer<TrakHoundAcknowledgeResult<ITrakHoundObjectMessageQueueEntity>>(response.Content);
        //            }
        //        }
        //    }

        //    return null;
        //}

        //public async Task<IEnumerable<TrakHoundAcknowledgeResult<ITrakHoundObjectMessageQueueEntity>>> PullByObjectUuid(string objectUuid, int count = 1, bool acknowledge = true, string routerId = null)
        //{
        //    if (!string.IsNullOrEmpty(objectUuid))
        //    {
        //        var router = BaseClient.GetRouter(routerId);
        //        if (router != null)
        //        {
        //            var response = await router.Entities.Objects.MessageQueues.Pull(objectUuid, count, acknowledge);
        //            if (response.IsSuccess)
        //            {
        //                return response.Content;
        //            }
        //        }
        //    }

        //    return null;
        //}

        //public async Task<bool> AcknowledgeByObjectUuid(string objectUuid, IEnumerable<string> deliveryIds, string routerId = null)
        //{
        //    if (!string.IsNullOrEmpty(objectUuid) && !deliveryIds.IsNullOrEmpty())
        //    {
        //        var router = BaseClient.GetRouter(routerId);
        //        if (router != null)
        //        {
        //            var response = await router.Entities.Objects.MessageQueues.Acknowledge(objectUuid, deliveryIds);
        //            if (response.IsSuccess)
        //            {
        //                return response.Content.FirstOrDefault();
        //            }
        //        }
        //    }

        //    return false;
        //}

        //public async Task<bool> RejectByObjectUuid(string objectUuid, IEnumerable<string> deliveryIds, string routerId = null)
        //{
        //    if (!string.IsNullOrEmpty(objectUuid) && !deliveryIds.IsNullOrEmpty())
        //    {
        //        var router = BaseClient.GetRouter(routerId);
        //        if (router != null)
        //        {
        //            var response = await router.Entities.Objects.MessageQueues.Reject(objectUuid, deliveryIds);
        //            if (response.IsSuccess)
        //            {
        //                return response.Content.FirstOrDefault();
        //            }
        //        }
        //    }

        //    return false;
        //}
    }
}
