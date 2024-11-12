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
    public partial class TrakHoundObjectDurationController : TrakHoundObjectDurationControllerBase
    {
        public TrakHoundObjectDurationController(ITrakHoundInstance instance, TrakHoundHttpWebSocketManager webSocketManager) : base(instance, webSocketManager) { }



        protected async override Task<IActionResult> OnQueryByObject(
            ITrakHoundClient client,
            string path,
            bool indentOutput = false
        )
        {
            var result = await client.System.Entities.Objects.Duration.QueryByObject(
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
            var result = await client.System.Entities.Objects.Duration.QueryByObject(
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
            var result = await client.System.Entities.Objects.Duration.QueryByObjectUuid(
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
            var result = await client.System.Entities.Objects.Duration.QueryByObjectUuid(
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
            long minimum,
            long maximum,
            long objectSkip,
            long objectTake,
            bool indentOutput = false
        )
        {
            var result = await client.System.Entities.Objects.Duration.QueryByObject(
            path,
            minimum,
            maximum,
            objectSkip,
            objectTake);

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
            long minimum,
            long maximum,
            long objectSkip,
            long objectTake,
            bool indentOutput = false
        )
        {
            var result = await client.System.Entities.Objects.Duration.QueryByObject(
            paths,
            minimum,
            maximum,
            objectSkip,
            objectTake);

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
            long minimum,
            long maximum,
            bool indentOutput = false
        )
        {
            var result = await client.System.Entities.Objects.Duration.QueryByObjectUuid(
            objectUuid,
            minimum,
            maximum);

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
            long minimum,
            long maximum,
            bool indentOutput = false
        )
        {
            var result = await client.System.Entities.Objects.Duration.QueryByObjectUuid(
            objectUuids,
            minimum,
            maximum);

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
            Func<Stream, Task<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectDurationEntity>>>> getConsumer = async (Stream requestBody) =>

            {
                var paths = GetRequestBodyJson<IEnumerable<string>>(requestBody);


                var consumer = await client.System.Entities.Objects.Duration.SubscribeByObject(
                paths,
                interval,
                take,
                consumerId);

                var id = !string.IsNullOrEmpty(consumerId) ? consumerId : consumer.Id;

                // Convert to Queued Consumer (batch Entities)
                return new TrakHoundQueueConsumer<ITrakHoundObjectDurationEntity>(consumer, id, interval, take);
            };

            var formatResponse = (IEnumerable<ITrakHoundObjectDurationEntity> entities) =>
            {
                var content = new List<object[]>();
                foreach (var entity in entities) content.Add(TrakHoundEntity.ToArray(entity));
                return content.ToJson(indentOutput).ToUtf8Bytes();
            };

            await WebSocketManager.Create<IEnumerable<ITrakHoundObjectDurationEntity>>(HttpContext, getConsumer, formatResponse);
        }



        protected async override Task OnSubscribeByObjectUuid(
            ITrakHoundClient client,
            int interval,
            int take,
            string consumerId,
            bool indentOutput = false
        )
        {
            Func<Stream, Task<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectDurationEntity>>>> getConsumer = async (Stream requestBody) =>

            {
                var objectUuids = GetRequestBodyJson<IEnumerable<string>>(requestBody);


                var consumer = await client.System.Entities.Objects.Duration.SubscribeByObjectUuid(
                objectUuids,
                interval,
                take,
                consumerId);

                var id = !string.IsNullOrEmpty(consumerId) ? consumerId : consumer.Id;

                // Convert to Queued Consumer (batch Entities)
                return new TrakHoundQueueConsumer<ITrakHoundObjectDurationEntity>(consumer, id, interval, take);
            };

            var formatResponse = (IEnumerable<ITrakHoundObjectDurationEntity> entities) =>
            {
                var content = new List<object[]>();
                foreach (var entity in entities) content.Add(TrakHoundEntity.ToArray(entity));
                return content.ToJson(indentOutput).ToUtf8Bytes();
            };

            await WebSocketManager.Create<IEnumerable<ITrakHoundObjectDurationEntity>>(HttpContext, getConsumer, formatResponse);
        }

    }
}
