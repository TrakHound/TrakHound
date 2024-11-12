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
    public partial class TrakHoundObjectGroupController : TrakHoundObjectGroupControllerBase
    {
        public TrakHoundObjectGroupController(ITrakHoundInstance instance, TrakHoundHttpWebSocketManager webSocketManager) : base(instance, webSocketManager) { }



        protected async override Task<IActionResult> OnQueryByGroup(
            ITrakHoundClient client,
            string groupPath,
            long skip,
            long take,
            SortOrder sortOrder,
            bool indentOutput = false
        )
        {
            var result = await client.System.Entities.Objects.Group.QueryByGroup(
            groupPath,
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



        protected async override Task<IActionResult> OnQueryByGroup(
            ITrakHoundClient client,
            IEnumerable<string> groupPaths,
            long skip,
            long take,
            SortOrder sortOrder,
            bool indentOutput = false
        )
        {
            var result = await client.System.Entities.Objects.Group.QueryByGroup(
            groupPaths,
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



        protected async override Task<IActionResult> OnQueryByGroupUuid(
            ITrakHoundClient client,
            string groupUuid,
            long skip,
            long take,
            SortOrder sortOrder,
            bool indentOutput = false
        )
        {
            var result = await client.System.Entities.Objects.Group.QueryByGroupUuid(
            groupUuid,
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



        protected async override Task<IActionResult> OnQueryByGroupUuid(
            ITrakHoundClient client,
            IEnumerable<string> groupUuids,
            long skip,
            long take,
            SortOrder sortOrder,
            bool indentOutput = false
        )
        {
            var result = await client.System.Entities.Objects.Group.QueryByGroupUuid(
            groupUuids,
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



        protected async override Task<IActionResult> OnQueryByMember(
            ITrakHoundClient client,
            string memberPath,
            long skip,
            long take,
            SortOrder sortOrder,
            bool indentOutput = false
        )
        {
            var result = await client.System.Entities.Objects.Group.QueryByMember(
            memberPath,
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



        protected async override Task<IActionResult> OnQueryByMember(
            ITrakHoundClient client,
            IEnumerable<string> memberPaths,
            long skip,
            long take,
            SortOrder sortOrder,
            bool indentOutput = false
        )
        {
            var result = await client.System.Entities.Objects.Group.QueryByMember(
            memberPaths,
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



        protected async override Task<IActionResult> OnQueryByMemberUuid(
            ITrakHoundClient client,
            string memberUuid,
            long skip,
            long take,
            SortOrder sortOrder,
            bool indentOutput = false
        )
        {
            var result = await client.System.Entities.Objects.Group.QueryByMemberUuid(
            memberUuid,
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



        protected async override Task<IActionResult> OnQueryByMemberUuid(
            ITrakHoundClient client,
            IEnumerable<string> memberUuids,
            long skip,
            long take,
            SortOrder sortOrder,
            bool indentOutput = false
        )
        {
            var result = await client.System.Entities.Objects.Group.QueryByMemberUuid(
            memberUuids,
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


        protected async override Task OnSubscribeByGroup(
            ITrakHoundClient client,
            int interval,
            int take,
            string consumerId,
            bool indentOutput = false
        )
        {
            Func<Stream, Task<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectGroupEntity>>>> getConsumer = async (Stream requestBody) =>

            {
                var paths = GetRequestBodyJson<IEnumerable<string>>(requestBody);


                var consumer = await client.System.Entities.Objects.Group.SubscribeByGroup(
                paths,
                interval,
                take,
                consumerId);

                var id = !string.IsNullOrEmpty(consumerId) ? consumerId : consumer.Id;

                // Convert to Queued Consumer (batch Entities)
                return new TrakHoundQueueConsumer<ITrakHoundObjectGroupEntity>(consumer, id, interval, take);
            };

            var formatResponse = (IEnumerable<ITrakHoundObjectGroupEntity> entities) =>
            {
                var content = new List<object[]>();
                foreach (var entity in entities) content.Add(TrakHoundEntity.ToArray(entity));
                return content.ToJson(indentOutput).ToUtf8Bytes();
            };

            await WebSocketManager.Create<IEnumerable<ITrakHoundObjectGroupEntity>>(HttpContext, getConsumer, formatResponse);
        }



        protected async override Task OnSubscribeByGroupUuid(
            ITrakHoundClient client,
            int interval,
            int take,
            string consumerId,
            bool indentOutput = false
        )
        {
            Func<Stream, Task<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectGroupEntity>>>> getConsumer = async (Stream requestBody) =>

            {
                var groupUuids = GetRequestBodyJson<IEnumerable<string>>(requestBody);


                var consumer = await client.System.Entities.Objects.Group.SubscribeByGroupUuid(
                groupUuids,
                interval,
                take,
                consumerId);

                var id = !string.IsNullOrEmpty(consumerId) ? consumerId : consumer.Id;

                // Convert to Queued Consumer (batch Entities)
                return new TrakHoundQueueConsumer<ITrakHoundObjectGroupEntity>(consumer, id, interval, take);
            };

            var formatResponse = (IEnumerable<ITrakHoundObjectGroupEntity> entities) =>
            {
                var content = new List<object[]>();
                foreach (var entity in entities) content.Add(TrakHoundEntity.ToArray(entity));
                return content.ToJson(indentOutput).ToUtf8Bytes();
            };

            await WebSocketManager.Create<IEnumerable<ITrakHoundObjectGroupEntity>>(HttpContext, getConsumer, formatResponse);
        }



        protected async override Task OnSubscribeByMember(
            ITrakHoundClient client,
            int interval,
            int take,
            string consumerId,
            bool indentOutput = false
        )
        {
            Func<Stream, Task<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectGroupEntity>>>> getConsumer = async (Stream requestBody) =>

            {
                var paths = GetRequestBodyJson<IEnumerable<string>>(requestBody);


                var consumer = await client.System.Entities.Objects.Group.SubscribeByMember(
                paths,
                interval,
                take,
                consumerId);

                var id = !string.IsNullOrEmpty(consumerId) ? consumerId : consumer.Id;

                // Convert to Queued Consumer (batch Entities)
                return new TrakHoundQueueConsumer<ITrakHoundObjectGroupEntity>(consumer, id, interval, take);
            };

            var formatResponse = (IEnumerable<ITrakHoundObjectGroupEntity> entities) =>
            {
                var content = new List<object[]>();
                foreach (var entity in entities) content.Add(TrakHoundEntity.ToArray(entity));
                return content.ToJson(indentOutput).ToUtf8Bytes();
            };

            await WebSocketManager.Create<IEnumerable<ITrakHoundObjectGroupEntity>>(HttpContext, getConsumer, formatResponse);
        }



        protected async override Task OnSubscribeByMemberUuid(
            ITrakHoundClient client,
            int interval,
            int take,
            string consumerId,
            bool indentOutput = false
        )
        {
            Func<Stream, Task<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectGroupEntity>>>> getConsumer = async (Stream requestBody) =>

            {
                var memberUuids = GetRequestBodyJson<IEnumerable<string>>(requestBody);


                var consumer = await client.System.Entities.Objects.Group.SubscribeByMemberUuid(
                memberUuids,
                interval,
                take,
                consumerId);

                var id = !string.IsNullOrEmpty(consumerId) ? consumerId : consumer.Id;

                // Convert to Queued Consumer (batch Entities)
                return new TrakHoundQueueConsumer<ITrakHoundObjectGroupEntity>(consumer, id, interval, take);
            };

            var formatResponse = (IEnumerable<ITrakHoundObjectGroupEntity> entities) =>
            {
                var content = new List<object[]>();
                foreach (var entity in entities) content.Add(TrakHoundEntity.ToArray(entity));
                return content.ToJson(indentOutput).ToUtf8Bytes();
            };

            await WebSocketManager.Create<IEnumerable<ITrakHoundObjectGroupEntity>>(HttpContext, getConsumer, formatResponse);
        }




        protected async override Task<IActionResult> OnDeleteByGroupUuid(
            ITrakHoundClient client,
            string groupUuid,
            bool indentOutput = false
        )
        {
            var result = await client.System.Entities.Objects.Group.DeleteByGroupUuid(
            groupUuid);

            if (result)
            {
                return Ok();
            }
            else
            {
                return NotFound();
            }

        }



        protected async override Task<IActionResult> OnDeleteByGroupUuid(
            ITrakHoundClient client,
            IEnumerable<string> groupUuids,
            bool indentOutput = false
        )
        {
            var result = await client.System.Entities.Objects.Group.DeleteByGroupUuid(
            groupUuids);

            if (result)
            {
                return Ok();
            }
            else
            {
                return NotFound();
            }

        }



        protected async override Task<IActionResult> OnDeleteByMemberUuid(
            ITrakHoundClient client,
            string memberUuid,
            bool indentOutput = false
        )
        {
            var result = await client.System.Entities.Objects.Group.DeleteByMemberUuid(
            memberUuid);

            if (result)
            {
                return Ok();
            }
            else
            {
                return NotFound();
            }

        }



        protected async override Task<IActionResult> OnDeleteByMemberUuid(
            ITrakHoundClient client,
            IEnumerable<string> memberUuids,
            bool indentOutput = false
        )
        {
            var result = await client.System.Entities.Objects.Group.DeleteByMemberUuid(
            memberUuids);

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
