// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Entities;

namespace TrakHound.Clients
{
    internal partial class TrakHoundInstanceSystemObjectQueueClient
    {

        public async Task<ITrakHoundConsumer<IEnumerable<TrakHoundEntityNotification>>> Notify(string path, TrakHoundEntityNotificationType notificationType = TrakHoundEntityNotificationType.All, string routerId = null)
        {
            if (!string.IsNullOrEmpty(path))
            {
                var router = BaseClient.GetRouter(routerId);
                if (router != null)
                {
                    var response = await router.Entities.Objects.Queues.Notify(path, notificationType);
                    if (response.IsSuccess)
                    {
                        return new TrakHoundConsumer<IEnumerable<TrakHoundEntityNotification>>(response.Content);
                    }
                }
            }

            return null;
        }

    }
}
