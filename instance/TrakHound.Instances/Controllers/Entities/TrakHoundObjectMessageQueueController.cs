// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Entities;

namespace TrakHound.Http.Entities
{
    public partial class TrakHoundObjectMessageQueueController
    {
        //[HttpGet("notify")]
        //public async Task Notify(
        //    [FromQuery(Name = "path")] string path,
        //    [FromQuery] string type = null,
        //    [FromQuery] int interval = 0,
        //    [FromQuery] int take = 100,
        //    [FromQuery] string routerId = null,
        //    [FromQuery] string consumerId = null,
        //    [FromQuery] bool indentOutput = false
        //    )
        //{
        //    if (!string.IsNullOrEmpty(path))
        //    {
        //        var getConsumer = async () =>
        //        {
        //            var client = GetClient(routerId);
        //            if (client != null)
        //            {
        //                var notificationType = type != null ? type.ConvertEnum<TrakHoundEntityNotificationType>() : TrakHoundEntityNotificationType.All;

        //                return await client.System.Entities.Objects.MessageQueue.Notify(path, notificationType, routerId);
        //            }
        //            return null;
        //        };

        //        var formatResponse = (IEnumerable<TrakHoundEntityNotification> notifications) =>
        //        {
        //            return notifications.ToJson(indentOutput).ToUtf8Bytes();
        //        };

        //        await WebSocketManager.Create<IEnumerable<TrakHoundEntityNotification>>(HttpContext, getConsumer, formatResponse);
        //    }
        //}
    }
}
