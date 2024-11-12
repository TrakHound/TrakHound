// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Clients;
using TrakHound.Entities;
using TrakHound.Instances;

namespace TrakHound.Http.Entities
{
    [ApiController]
    [Route($"{HttpConstants.EntitiesPrefix}/objects/queue")]
    public abstract class TrakHoundObjectQueueControllerBase : TrakHoundHttpEntityControllerBase<ITrakHoundObjectQueueEntity>
    {
        public TrakHoundObjectQueueControllerBase(ITrakHoundInstance instance, TrakHoundHttpWebSocketManager webSocketManager) : base(instance, webSocketManager) { }


        /// <summary>
        /// Query by Queue
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> QueryByQueue(
            [FromQuery] string queuePath,
            [FromQuery] long skip = 0,
            [FromQuery] long take = 1000,
            [FromQuery] int sortOrder = (int)TrakHound.SortOrder.Ascending,
            [FromQuery] string routerId = null,
            [FromQuery] bool indentOutput = false
            )
        {
            var client = GetClient(routerId);
            if (client != null)
            {
                return await OnQueryByQueue(client, queuePath, skip, take, (SortOrder)sortOrder, indentOutput);
            }
            else
            {
                return StatusCode(500, "Entity Client Not Found");
            }
        }

        protected virtual Task<IActionResult> OnQueryByQueue(
            ITrakHoundClient client,
            string queuePath,
            long skip,
            long take,
            SortOrder sortOrder,
            bool indentOutput = false
        )
        {
            return Task.FromResult<IActionResult>(NotFound());
        }


        /// <summary>
        /// Query by Queue UUID
        /// </summary>
        [HttpGet("object/{queueUuid}")]
        public async Task<IActionResult> QueryByQueueUuid(
            [FromRoute] string queueUuid,
            [FromQuery] long skip = 0,
            [FromQuery] long take = 1000,
            [FromQuery] int sortOrder = (int)TrakHound.SortOrder.Ascending,
            [FromQuery] string routerId = null,
            [FromQuery] bool indentOutput = false
            )
        {
            var client = GetClient(routerId);
            if (client != null)
            {
                return await OnQueryByQueueUuid(client, queueUuid, skip, take, (SortOrder)sortOrder, indentOutput);
            }
            else
            {
                return StatusCode(500, "Entity Client Not Found");
            }
        }

        protected virtual Task<IActionResult> OnQueryByQueueUuid(
            ITrakHoundClient client,
            string queueUuid,
            long skip,
            long take,
            SortOrder sortOrder,
            bool indentOutput = false
        )
        {
            return Task.FromResult<IActionResult>(NotFound());
        }


        /// <summary>
        /// Query by Queue UUID
        /// </summary>
        [HttpPost("object")]
        public async Task<IActionResult> QueryByQueueUuid(
            [FromBody] IEnumerable<string> queueUuids,
            [FromQuery] long skip = 0,
            [FromQuery] long take = 1000,
            [FromQuery] int sortOrder = (int)TrakHound.SortOrder.Ascending,
            [FromQuery] string routerId = null,
            [FromQuery] bool indentOutput = false
            )
        {
            var client = GetClient(routerId);
            if (client != null)
            {
                return await OnQueryByQueueUuid(client, queueUuids, skip, take, (SortOrder)sortOrder, indentOutput);
            }
            else
            {
                return StatusCode(500, "Entity Client Not Found");
            }
        }

        protected virtual Task<IActionResult> OnQueryByQueueUuid(
            ITrakHoundClient client,
            IEnumerable<string> queueUuids,
            long skip,
            long take,
            SortOrder sortOrder,
            bool indentOutput = false
        )
        {
            return Task.FromResult<IActionResult>(NotFound());
        }


        /// <summary>
        /// Pull 
        /// </summary>
        [HttpGet("pull")]
        public async Task<IActionResult> Pull(
            [FromRoute] string queueUuid,
            [FromQuery] int count = 1,
            [FromQuery] string routerId = null,
            [FromQuery] bool indentOutput = false
            )
        {
            var client = GetClient(routerId);
            if (client != null)
            {
                return await OnPull(client, queueUuid, count, indentOutput);
            }
            else
            {
                return StatusCode(500, "Entity Client Not Found");
            }
        }

        protected virtual Task<IActionResult> OnPull(
            ITrakHoundClient client,
            string queueUuid,
            int count,
            bool indentOutput = false
        )
        {
            return Task.FromResult<IActionResult>(NotFound());
        }


        /// <summary>
        /// Subscribe by Queue
        /// </summary>
        [Route("queue/path/subscribe")]
        public async Task SubscribeByQueue(

            [FromQuery] int interval = 0,

            [FromQuery] int take = 1000,

            [FromQuery] string consumerId = null,
            [FromQuery] string routerId = null,
            [FromQuery] bool indentOutput = false
            )
        {
            var client = GetClient(routerId);
            if (client != null)
            {
                await OnSubscribeByQueue(client, interval, take, consumerId, indentOutput);
            }
            else
            {
                //return StatusCode(500, "Entity Client Not Found");
            }
        }

        protected virtual Task OnSubscribeByQueue(
            ITrakHoundClient client,
            int interval,
            int take,
            string consumerId,
            bool indentOutput = false
        )
        {
            return Task.FromResult<IActionResult>(NotFound());
        }


        /// <summary>
        /// Subscribe by Queue UUID
        /// </summary>
        [Route("queue/subscribe")]
        public async Task SubscribeByQueueUuid(

            [FromQuery] int interval = 0,

            [FromQuery] int take = 1000,

            [FromQuery] string consumerId = null,
            [FromQuery] string routerId = null,
            [FromQuery] bool indentOutput = false
            )
        {
            var client = GetClient(routerId);
            if (client != null)
            {
                await OnSubscribeByQueueUuid(client, interval, take, consumerId, indentOutput);
            }
            else
            {
                //return StatusCode(500, "Entity Client Not Found");
            }
        }

        protected virtual Task OnSubscribeByQueueUuid(
            ITrakHoundClient client,
            int interval,
            int take,
            string consumerId,
            bool indentOutput = false
        )
        {
            return Task.FromResult<IActionResult>(NotFound());
        }

    }
}
