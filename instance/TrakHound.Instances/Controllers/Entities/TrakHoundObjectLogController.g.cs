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
    public partial class TrakHoundObjectLogController : TrakHoundObjectLogControllerBase
    {
        public TrakHoundObjectLogController(ITrakHoundInstance instance, TrakHoundHttpWebSocketManager webSocketManager) : base(instance, webSocketManager) { }



        protected async override Task<IActionResult> OnQueryByObject(
            ITrakHoundClient client,
            string path,
            TrakHound.TrakHoundLogLevel minLevel,
            long start,
            long stop,
            long skip,
            long take,
            SortOrder sortOrder,
            bool indentOutput = false
        )
        {
            var result = await client.System.Entities.Objects.Log.QueryByObject(
            path,
            minLevel,
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
            TrakHound.TrakHoundLogLevel minLevel,
            long start,
            long stop,
            long skip,
            long take,
            SortOrder sortOrder,
            bool indentOutput = false
        )
        {
            var result = await client.System.Entities.Objects.Log.QueryByObject(
            paths,
            minLevel,
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
            TrakHound.TrakHoundLogLevel minLevel,
            long start,
            long stop,
            long skip,
            long take,
            SortOrder sortOrder,
            bool indentOutput = false
        )
        {
            var result = await client.System.Entities.Objects.Log.QueryByObjectUuid(
            objectUuid,
            minLevel,
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
            TrakHound.TrakHoundLogLevel minLevel,
            long start,
            long stop,
            long skip,
            long take,
            SortOrder sortOrder,
            bool indentOutput = false
        )
        {
            var result = await client.System.Entities.Objects.Log.QueryByObjectUuid(
            objectUuids,
            minLevel,
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
            TrakHound.TrakHoundLogLevel minLevel,
            int interval,
            int take,
            string consumerId,
            bool indentOutput = false
        )
        {
            Func<Stream, Task<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectLogEntity>>>> getConsumer = async (Stream requestBody) =>

            {
                var paths = GetRequestBodyJson<IEnumerable<string>>(requestBody);


                var consumer = await client.System.Entities.Objects.Log.SubscribeByObject(
                paths,
                minLevel,
                interval,
                take,
                consumerId);

                var id = !string.IsNullOrEmpty(consumerId) ? consumerId : consumer.Id;

                // Convert to Queued Consumer (batch Entities)
                return new TrakHoundQueueConsumer<ITrakHoundObjectLogEntity>(consumer, id, interval, take);
            };

            var formatResponse = (IEnumerable<ITrakHoundObjectLogEntity> entities) =>
            {
                var content = new List<object[]>();
                foreach (var entity in entities) content.Add(TrakHoundEntity.ToArray(entity));
                return content.ToJson(indentOutput).ToUtf8Bytes();
            };

            await WebSocketManager.Create<IEnumerable<ITrakHoundObjectLogEntity>>(HttpContext, getConsumer, formatResponse);
        }



        protected async override Task OnSubscribeByObjectUuid(
            ITrakHoundClient client,
            TrakHound.TrakHoundLogLevel minLevel,
            int interval,
            int take,
            string consumerId,
            bool indentOutput = false
        )
        {
            Func<Stream, Task<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectLogEntity>>>> getConsumer = async (Stream requestBody) =>

            {
                var objectUuids = GetRequestBodyJson<IEnumerable<string>>(requestBody);


                var consumer = await client.System.Entities.Objects.Log.SubscribeByObjectUuid(
                objectUuids,
                minLevel,
                interval,
                take,
                consumerId);

                var id = !string.IsNullOrEmpty(consumerId) ? consumerId : consumer.Id;

                // Convert to Queued Consumer (batch Entities)
                return new TrakHoundQueueConsumer<ITrakHoundObjectLogEntity>(consumer, id, interval, take);
            };

            var formatResponse = (IEnumerable<ITrakHoundObjectLogEntity> entities) =>
            {
                var content = new List<object[]>();
                foreach (var entity in entities) content.Add(TrakHoundEntity.ToArray(entity));
                return content.ToJson(indentOutput).ToUtf8Bytes();
            };

            await WebSocketManager.Create<IEnumerable<ITrakHoundObjectLogEntity>>(HttpContext, getConsumer, formatResponse);
        }




        protected async override Task<IActionResult> OnDeleteByObjectUuid(
            ITrakHoundClient client,
            string objectUuid,
            bool indentOutput = false
        )
        {
            var result = await client.System.Entities.Objects.Log.DeleteByObjectUuid(
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
            var result = await client.System.Entities.Objects.Log.DeleteByObjectUuid(
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
