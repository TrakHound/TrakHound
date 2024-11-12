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
    public partial class TrakHoundObjectStatisticController : TrakHoundObjectStatisticControllerBase
    {
        public TrakHoundObjectStatisticController(ITrakHoundInstance instance, TrakHoundHttpWebSocketManager webSocketManager) : base(instance, webSocketManager) { }



        protected async override Task<IActionResult> OnQueryByObject(
            ITrakHoundClient client,
            string path,
            long start,
            long stop,
            long span,
            long skip,
            long take,
            SortOrder sortOrder,
            bool indentOutput = false
        )
        {
            var result = await client.System.Entities.Objects.Statistic.QueryByObject(
            path,
            start,
            stop,
            span,
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
            long span,
            long skip,
            long take,
            SortOrder sortOrder,
            bool indentOutput = false
        )
        {
            var result = await client.System.Entities.Objects.Statistic.QueryByObject(
            paths,
            start,
            stop,
            span,
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
            long span,
            long skip,
            long take,
            SortOrder sortOrder,
            bool indentOutput = false
        )
        {
            var result = await client.System.Entities.Objects.Statistic.QueryByObjectUuid(
            objectUuid,
            start,
            stop,
            span,
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
            long span,
            long skip,
            long take,
            SortOrder sortOrder,
            bool indentOutput = false
        )
        {
            var result = await client.System.Entities.Objects.Statistic.QueryByObjectUuid(
            objectUuids,
            start,
            stop,
            span,
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



        protected async override Task<IActionResult> OnSpansByObject(
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
            var result = await client.System.Entities.Objects.Statistic.SpansByObject(
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



        protected async override Task<IActionResult> OnSpansByObject(
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
            var result = await client.System.Entities.Objects.Statistic.SpansByObject(
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



        protected async override Task<IActionResult> OnSpansByObjectUuid(
            ITrakHoundClient client,
            string objectUuid,
            long start,
            long stop,
            bool indentOutput = false
        )
        {
            var result = await client.System.Entities.Objects.Statistic.SpansByObjectUuid(
            objectUuid,
            start,
            stop);

            if (!IsNullOrEmpty(result))
            {
                return ProcessJsonContentResponse(result, indentOutput);
            }
            else
            {
                return NotFound();
            }
        }



        protected async override Task<IActionResult> OnSpansByObjectUuid(
            ITrakHoundClient client,
            IEnumerable<string> objectUuids,
            long start,
            long stop,
            bool indentOutput = false
        )
        {
            var result = await client.System.Entities.Objects.Statistic.SpansByObjectUuid(
            objectUuids,
            start,
            stop);

            if (!IsNullOrEmpty(result))
            {
                return ProcessJsonContentResponse(result, indentOutput);
            }
            else
            {
                return NotFound();
            }
        }



        protected async override Task<IActionResult> OnCountByObject(
            ITrakHoundClient client,
            string path,
            long start,
            long stop,
            long span,
            bool indentOutput = false
        )
        {
            var result = await client.System.Entities.Objects.Statistic.CountByObject(
            path,
            start,
            stop,
            span);

            if (!IsNullOrEmpty(result))
            {
                return ProcessJsonContentResponse(result, indentOutput);
            }
            else
            {
                return NotFound();
            }
        }



        protected async override Task<IActionResult> OnCountByObject(
            ITrakHoundClient client,
            IEnumerable<string> paths,
            long start,
            long stop,
            long span,
            long skip,
            long take,
            SortOrder sortOrder,
            bool indentOutput = false
        )
        {
            var result = await client.System.Entities.Objects.Statistic.CountByObject(
            paths,
            start,
            stop,
            span,
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



        protected async override Task<IActionResult> OnCountByObjectUuid(
            ITrakHoundClient client,
            string objectUuid,
            long start,
            long stop,
            long span,
            bool indentOutput = false
        )
        {
            var result = await client.System.Entities.Objects.Statistic.CountByObjectUuid(
            objectUuid,
            start,
            stop,
            span);

            if (!IsNullOrEmpty(result))
            {
                return ProcessJsonContentResponse(result, indentOutput);
            }
            else
            {
                return NotFound();
            }
        }



        protected async override Task<IActionResult> OnCountByObjectUuid(
            ITrakHoundClient client,
            IEnumerable<string> objectUuids,
            long start,
            long stop,
            long span,
            bool indentOutput = false
        )
        {
            var result = await client.System.Entities.Objects.Statistic.CountByObjectUuid(
            objectUuids,
            start,
            stop,
            span);

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
            Func<Stream, Task<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectStatisticEntity>>>> getConsumer = async (Stream requestBody) =>

            {
                var paths = GetRequestBodyJson<IEnumerable<string>>(requestBody);


                var consumer = await client.System.Entities.Objects.Statistic.SubscribeByObject(
                paths,
                interval,
                take,
                consumerId);

                var id = !string.IsNullOrEmpty(consumerId) ? consumerId : consumer.Id;

                // Convert to Queued Consumer (batch Entities)
                return new TrakHoundQueueConsumer<ITrakHoundObjectStatisticEntity>(consumer, id, interval, take);
            };

            var formatResponse = (IEnumerable<ITrakHoundObjectStatisticEntity> entities) =>
            {
                var content = new List<object[]>();
                foreach (var entity in entities) content.Add(TrakHoundEntity.ToArray(entity));
                return content.ToJson(indentOutput).ToUtf8Bytes();
            };

            await WebSocketManager.Create<IEnumerable<ITrakHoundObjectStatisticEntity>>(HttpContext, getConsumer, formatResponse);
        }



        protected async override Task OnSubscribeByObjectUuid(
            ITrakHoundClient client,
            int interval,
            int take,
            string consumerId,
            bool indentOutput = false
        )
        {
            Func<Stream, Task<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectStatisticEntity>>>> getConsumer = async (Stream requestBody) =>

            {
                var objectUuids = GetRequestBodyJson<IEnumerable<string>>(requestBody);


                var consumer = await client.System.Entities.Objects.Statistic.SubscribeByObjectUuid(
                objectUuids,
                interval,
                take,
                consumerId);

                var id = !string.IsNullOrEmpty(consumerId) ? consumerId : consumer.Id;

                // Convert to Queued Consumer (batch Entities)
                return new TrakHoundQueueConsumer<ITrakHoundObjectStatisticEntity>(consumer, id, interval, take);
            };

            var formatResponse = (IEnumerable<ITrakHoundObjectStatisticEntity> entities) =>
            {
                var content = new List<object[]>();
                foreach (var entity in entities) content.Add(TrakHoundEntity.ToArray(entity));
                return content.ToJson(indentOutput).ToUtf8Bytes();
            };

            await WebSocketManager.Create<IEnumerable<ITrakHoundObjectStatisticEntity>>(HttpContext, getConsumer, formatResponse);
        }




        protected async override Task<IActionResult> OnDeleteByObjectUuid(
            ITrakHoundClient client,
            string objectUuid,
            bool indentOutput = false
        )
        {
            var result = await client.System.Entities.Objects.Statistic.DeleteByObjectUuid(
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
            var result = await client.System.Entities.Objects.Statistic.DeleteByObjectUuid(
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
