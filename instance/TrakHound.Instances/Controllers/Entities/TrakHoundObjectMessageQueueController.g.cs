// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TrakHound.Clients;
using TrakHound.Entities;
using TrakHound.Entities.Collections;
using TrakHound.Instances;

namespace TrakHound.Http.Entities
{
    public partial class TrakHoundObjectMessageQueueController : TrakHoundObjectMessageQueueControllerBase
    {
        public TrakHoundObjectMessageQueueController(ITrakHoundInstance instance, TrakHoundHttpWebSocketManager webSocketManager) : base(instance, webSocketManager) { }



        protected async override Task<IActionResult> OnQueryByObject(
            ITrakHoundClient client,
            string path,
            bool indentOutput = false
        )
        {
            var result = await client.System.Entities.Objects.MessageQueue.QueryByObject(
            path);

            if (!IsNullOrEmpty(result))
            {
                return ProcessJsonContentResponse(result, indentOutput);
            }
            else
            {
                return NotFound();
            }
        }



        protected async override Task<IActionResult> OnQueryByObject(
            ITrakHoundClient client,
            IEnumerable<string> paths,
            bool indentOutput = false
        )
        {
            var result = await client.System.Entities.Objects.MessageQueue.QueryByObject(
            paths);

            if (!IsNullOrEmpty(result))
            {
                return ProcessJsonContentResponse(result, indentOutput);
            }
            else
            {
                return NotFound();
            }
        }



        protected async override Task<IActionResult> OnQueryByObjectUuid(
            ITrakHoundClient client,
            string objectUuid,
            bool indentOutput = false
        )
        {
            var result = await client.System.Entities.Objects.MessageQueue.QueryByObjectUuid(
            objectUuid);

            if (!IsNullOrEmpty(result))
            {
                return ProcessJsonContentResponse(result, indentOutput);
            }
            else
            {
                return NotFound();
            }
        }



        protected async override Task<IActionResult> OnQueryByObjectUuid(
            ITrakHoundClient client,
            IEnumerable<string> objectUuids,
            bool indentOutput = false
        )
        {
            var result = await client.System.Entities.Objects.MessageQueue.QueryByObjectUuid(
            objectUuids);

            if (!IsNullOrEmpty(result))
            {
                return ProcessJsonContentResponse(result, indentOutput);
            }
            else
            {
                return NotFound();
            }
        }


        protected async override Task OnSubscribeByObject(
            ITrakHoundClient client,
            int interval,
            int take,
            string consumerId,
            bool indentOutput = false
        )
        {
            Func<Stream, Task<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectMessageQueueEntity>>>> getConsumer = async (Stream requestBody) =>

            {
                var paths = GetRequestBodyJson<IEnumerable<string>>(requestBody);


                var consumer = await client.System.Entities.Objects.MessageQueue.SubscribeByObject(
                paths,
                interval,
                take,
                consumerId);

                var id = !string.IsNullOrEmpty(consumerId) ? consumerId : consumer.Id;

                // Convert to Queued Consumer (batch Entities)
                return new TrakHoundQueueConsumer<ITrakHoundObjectMessageQueueEntity>(consumer, id, interval, take);
            };

            var formatResponse = (IEnumerable<ITrakHoundObjectMessageQueueEntity> entities) =>
            {
                var content = new List<object[]>();
                foreach (var entity in entities) content.Add(TrakHoundEntity.ToArray(entity));
                return content.ToJson(indentOutput).ToUtf8Bytes();
            };

            await WebSocketManager.Create<IEnumerable<ITrakHoundObjectMessageQueueEntity>>(HttpContext, getConsumer, formatResponse);
        }



        protected async override Task OnSubscribeByObjectUuid(
            ITrakHoundClient client,
            int interval,
            int take,
            string consumerId,
            bool indentOutput = false
        )
        {
            Func<Stream, Task<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectMessageQueueEntity>>>> getConsumer = async (Stream requestBody) =>

            {
                var objectUuids = GetRequestBodyJson<IEnumerable<string>>(requestBody);


                var consumer = await client.System.Entities.Objects.MessageQueue.SubscribeByObjectUuid(
                objectUuids,
                interval,
                take,
                consumerId);

                var id = !string.IsNullOrEmpty(consumerId) ? consumerId : consumer.Id;

                // Convert to Queued Consumer (batch Entities)
                return new TrakHoundQueueConsumer<ITrakHoundObjectMessageQueueEntity>(consumer, id, interval, take);
            };

            var formatResponse = (IEnumerable<ITrakHoundObjectMessageQueueEntity> entities) =>
            {
                var content = new List<object[]>();
                foreach (var entity in entities) content.Add(TrakHoundEntity.ToArray(entity));
                return content.ToJson(indentOutput).ToUtf8Bytes();
            };

            await WebSocketManager.Create<IEnumerable<ITrakHoundObjectMessageQueueEntity>>(HttpContext, getConsumer, formatResponse);
        }

    }
}
