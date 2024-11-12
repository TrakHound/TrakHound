// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TrakHound.Entities;
using TrakHound.Http;

namespace TrakHound.Clients
{
    internal partial class TrakHoundHttpSystemObjectMessageQueueClient
    {

        public async Task<ITrakHoundConsumer<IEnumerable<TrakHoundEntityNotification>>> Notify(string path, TrakHoundEntityNotificationType notificationType = TrakHoundEntityNotificationType.All, string routerId = null)
        {
            var url = TrakHoundHttp.GetEntityPath<ITrakHoundObjectMessageQueueEntity>();
            url = Url.Combine(url, "notify");
            url = Url.AddQueryParameter(url, "path", path);
            url = Url.AddQueryParameter(url, "type", notificationType.ToString());
            url = Url.AddQueryParameter(url, "routerId", BaseClient.GetRouterId(routerId));

            var consumer = new TrakHoundNotificationClientConsumer(BaseClient.ClientConfiguration, url);
            consumer.Subscribe();
            return consumer;
        }

        public async Task<ITrakHoundConsumer<TrakHoundAcknowledgeResult<ITrakHoundObjectMessageQueueEntity>>> SubscribeByObjectUuid(string objectUuid, bool acknowledge = true, string routerId = null)
        {
            //var url = TrakHoundHttp.GetEntityPath<ITrakHoundObjectMessageQueueEntity>();
            //url = Url.Combine(url, "subscribe");
            //url = Url.AddQueryParameter(url, "path", path);
            //url = Url.AddQueryParameter(url, "routerId", BaseClient.GetRouterId(routerId));

            //var consumer = new TrakHoundNotificationClientConsumer(BaseClient.ClientConfiguration, url);
            //consumer.Subscribe();
            //return consumer;

            return null;
        }

        public async Task<IEnumerable<TrakHoundAcknowledgeResult<ITrakHoundObjectMessageQueueEntity>>> PullByObjectUuid(string objectUuid, int count = 1, bool acknowledge = true, string routerId = null)
        {
            //var url = TrakHoundHttp.GetEntityPath<ITrakHoundObjectMessageQueueEntity>();
            //url = Url.Combine(url, "pull");
            //url = Url.AddQueryParameter(url, "path", path);
            //url = Url.AddQueryParameter(url, "routerId", BaseClient.GetRouterId(routerId));

            //var consumer = new TrakHoundNotificationClientConsumer(BaseClient.ClientConfiguration, url);
            //consumer.Subscribe();
            //return consumer;

            return null;
        }


        public async Task<bool> AcknowledgeByObjectUuid(string objectUuid, IEnumerable<string> deliveryIds, string routerId = null)
        {
            return false;
        }

        public async Task<bool> RejectByObjectUuid(string objectUuid, IEnumerable<string> deliveryIds, string routerId = null)
        {
            return false;
        }


        class TrakHoundHttpAcknowledgeResult<T>
        {
            [JsonPropertyName("deliveryId")]
            public string DeliveryId { get; set; }

            [JsonPropertyName("content")]
            public object[] Content { get; set; }
        }
    }
}
