// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Entities;
using TrakHound.Http;

namespace TrakHound.Clients
{
    internal partial class TrakHoundHttpSystemObjectQueueClient
    {
        public async Task<ITrakHoundConsumer<IEnumerable<TrakHoundEntityNotification>>> Notify(string path, TrakHoundEntityNotificationType notificationType = TrakHoundEntityNotificationType.All, string routerId = null)
        {
            var url = TrakHoundHttp.GetEntityPath<ITrakHoundObjectQueueEntity>();
            url = Url.Combine(url, "notify");
            url = Url.AddQueryParameter(url, "path", path);
            url = Url.AddQueryParameter(url, "type", notificationType.ToString());
            url = Url.AddQueryParameter(url, "routerId", BaseClient.GetRouterId(routerId));

            var consumer = new TrakHoundNotificationClientConsumer(BaseClient.ClientConfiguration, url);
            consumer.Subscribe();
            return consumer;
        }
    }
}
