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
    public partial class TrakHoundObjectEventController : TrakHoundObjectEventControllerBase
    {
        public TrakHoundObjectEventController(ITrakHoundInstance instance, TrakHoundHttpWebSocketManager webSocketManager) : base(instance, webSocketManager) { }



        protected async override Task<IActionResult> OnLatestByObject(
            ITrakHoundClient client,
            string path,
            bool indentOutput = false
        )
        {
            var result = await client.System.Entities.Objects.Event.LatestByObject(
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



        protected async override Task<IActionResult> OnLatestByObject(
            ITrakHoundClient client,
            IEnumerable<string> paths,
            bool indentOutput = false
        )
        {
            var result = await client.System.Entities.Objects.Event.LatestByObject(
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



        protected async override Task<IActionResult> OnLatestByObjectUuid(
            ITrakHoundClient client,
            string objectUuid,
            bool indentOutput = false
        )
        {
            var result = await client.System.Entities.Objects.Event.LatestByObjectUuid(
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



        protected async override Task<IActionResult> OnLatestByObjectUuid(
            ITrakHoundClient client,
            IEnumerable<string> objectUuids,
            bool indentOutput = false
        )
        {
            var result = await client.System.Entities.Objects.Event.LatestByObjectUuid(
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



        protected async override Task<IActionResult> OnQueryByObject(
            ITrakHoundClient client,
            string path,
            long start,
            long stop,
            long skip,
            long take,
            SortOrder sortOrder,
            bool indentOutput = false
        )
        {
            var result = await client.System.Entities.Objects.Event.QueryByObject(
            path,
            start,
            stop,
            skip,
            take,
            sortOrder);

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
            long start,
            long stop,
            long skip,
            long take,
            SortOrder sortOrder,
            bool indentOutput = false
        )
        {
            var result = await client.System.Entities.Objects.Event.QueryByObject(
            paths,
            start,
            stop,
            skip,
            take,
            sortOrder);

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
            long start,
            long stop,
            long skip,
            long take,
            SortOrder sortOrder,
            bool indentOutput = false
        )
        {
            var result = await client.System.Entities.Objects.Event.QueryByObjectUuid(
            objectUuid,
            start,
            stop,
            skip,
            take,
            sortOrder);

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
            long start,
            long stop,
            long skip,
            long take,
            SortOrder sortOrder,
            bool indentOutput = false
        )
        {
            var result = await client.System.Entities.Objects.Event.QueryByObjectUuid(
            objectUuids,
            start,
            stop,
            skip,
            take,
            sortOrder);

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
            Func<Stream, Task<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectEventEntity>>>> getConsumer = async (Stream requestBody) =>

            {
                var paths = GetRequestBodyJson<IEnumerable<string>>(requestBody);


                var consumer = await client.System.Entities.Objects.Event.SubscribeByObject(
                paths,
                interval,
                take,
                consumerId);

                var id = !string.IsNullOrEmpty(consumerId) ? consumerId : consumer.Id;

                // Convert to Queued Consumer (batch Entities)
                return new TrakHoundQueueConsumer<ITrakHoundObjectEventEntity>(consumer, id, interval, take);
            };

            var formatResponse = (IEnumerable<ITrakHoundObjectEventEntity> entities) =>
            {
                var content = new List<object[]>();
                foreach (var entity in entities) content.Add(TrakHoundEntity.ToArray(entity));
                return content.ToJson(indentOutput).ToUtf8Bytes();
            };

            await WebSocketManager.Create<IEnumerable<ITrakHoundObjectEventEntity>>(HttpContext, getConsumer, formatResponse);
        }



        protected async override Task OnSubscribeByObjectUuid(
            ITrakHoundClient client,
            int interval,
            int take,
            string consumerId,
            bool indentOutput = false
        )
        {
            Func<Stream, Task<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectEventEntity>>>> getConsumer = async (Stream requestBody) =>

            {
                var objectUuids = GetRequestBodyJson<IEnumerable<string>>(requestBody);


                var consumer = await client.System.Entities.Objects.Event.SubscribeByObjectUuid(
                objectUuids,
                interval,
                take,
                consumerId);

                var id = !string.IsNullOrEmpty(consumerId) ? consumerId : consumer.Id;

                // Convert to Queued Consumer (batch Entities)
                return new TrakHoundQueueConsumer<ITrakHoundObjectEventEntity>(consumer, id, interval, take);
            };

            var formatResponse = (IEnumerable<ITrakHoundObjectEventEntity> entities) =>
            {
                var content = new List<object[]>();
                foreach (var entity in entities) content.Add(TrakHoundEntity.ToArray(entity));
                return content.ToJson(indentOutput).ToUtf8Bytes();
            };

            await WebSocketManager.Create<IEnumerable<ITrakHoundObjectEventEntity>>(HttpContext, getConsumer, formatResponse);
        }




        protected async override Task<IActionResult> OnDeleteByObjectUuid(
            ITrakHoundClient client,
            string objectUuid,
            bool indentOutput = false
        )
        {
            var result = await client.System.Entities.Objects.Event.DeleteByObjectUuid(
            objectUuid);

            if (result)
            {
                return Ok();
            }
            else
            {
                return NotFound();
            }

        }



        protected async override Task<IActionResult> OnDeleteByObjectUuid(
            ITrakHoundClient client,
            IEnumerable<string> objectUuids,
            bool indentOutput = false
        )
        {
            var result = await client.System.Entities.Objects.Event.DeleteByObjectUuid(
            objectUuids);

            if (result)
            {
                return Ok();
            }
            else
            {
                return NotFound();
            }

        }
    }
}
