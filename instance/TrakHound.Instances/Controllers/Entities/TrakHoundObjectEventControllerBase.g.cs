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
    [Route($"{HttpConstants.EntitiesPrefix}/objects/event")]
    public abstract class TrakHoundObjectEventControllerBase : TrakHoundHttpEntityControllerBase<ITrakHoundObjectEventEntity>
    {
        public TrakHoundObjectEventControllerBase(ITrakHoundInstance instance, TrakHoundHttpWebSocketManager webSocketManager) : base(instance, webSocketManager) { }


        /// <summary>
        /// Latest by Object
        /// </summary>
        [HttpGet("latest")]
        public async Task<IActionResult> LatestByObject(
            [FromQuery] string path,
            [FromQuery] string routerId = null,
            [FromQuery] bool indentOutput = false
            )
        {
            var client = GetClient(routerId);
            if (client != null)
            {
                return await OnLatestByObject(client, path, indentOutput);
            }
            else
            {
                return StatusCode(500, "Entity Client Not Found");
            }
        }

        protected virtual Task<IActionResult> OnLatestByObject(
            ITrakHoundClient client,
            string path,
            bool indentOutput = false
        )
        {
            return Task.FromResult<IActionResult>(NotFound());
        }


        /// <summary>
        /// Latest by Object
        /// </summary>
        [HttpPost("latest")]
        public async Task<IActionResult> LatestByObject(
            [FromBody] IEnumerable<string> paths,
            [FromQuery] string routerId = null,
            [FromQuery] bool indentOutput = false
            )
        {
            var client = GetClient(routerId);
            if (client != null)
            {
                return await OnLatestByObject(client, paths, indentOutput);
            }
            else
            {
                return StatusCode(500, "Entity Client Not Found");
            }
        }

        protected virtual Task<IActionResult> OnLatestByObject(
            ITrakHoundClient client,
            IEnumerable<string> paths,
            bool indentOutput = false
        )
        {
            return Task.FromResult<IActionResult>(NotFound());
        }


        /// <summary>
        /// Latest by Object UUID
        /// </summary>
        [HttpGet("latest/object/{objectUuid}")]
        public async Task<IActionResult> LatestByObjectUuid(
            [FromRoute] string objectUuid,
            [FromQuery] string routerId = null,
            [FromQuery] bool indentOutput = false
            )
        {
            var client = GetClient(routerId);
            if (client != null)
            {
                return await OnLatestByObjectUuid(client, objectUuid, indentOutput);
            }
            else
            {
                return StatusCode(500, "Entity Client Not Found");
            }
        }

        protected virtual Task<IActionResult> OnLatestByObjectUuid(
            ITrakHoundClient client,
            string objectUuid,
            bool indentOutput = false
        )
        {
            return Task.FromResult<IActionResult>(NotFound());
        }


        /// <summary>
        /// Latest by Object UUID
        /// </summary>
        [HttpPost("latest/object")]
        public async Task<IActionResult> LatestByObjectUuid(
            [FromBody] IEnumerable<string> objectUuids,
            [FromQuery] string routerId = null,
            [FromQuery] bool indentOutput = false
            )
        {
            var client = GetClient(routerId);
            if (client != null)
            {
                return await OnLatestByObjectUuid(client, objectUuids, indentOutput);
            }
            else
            {
                return StatusCode(500, "Entity Client Not Found");
            }
        }

        protected virtual Task<IActionResult> OnLatestByObjectUuid(
            ITrakHoundClient client,
            IEnumerable<string> objectUuids,
            bool indentOutput = false
        )
        {
            return Task.FromResult<IActionResult>(NotFound());
        }


        /// <summary>
        /// Query by Object
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> QueryByObject(
            [FromQuery] string path,
            [FromQuery] long start,
            [FromQuery] long stop,
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
                return await OnQueryByObject(client, path, start, stop, skip, take, (SortOrder)sortOrder, indentOutput);
            }
            else
            {
                return StatusCode(500, "Entity Client Not Found");
            }
        }

        protected virtual Task<IActionResult> OnQueryByObject(
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
            return Task.FromResult<IActionResult>(NotFound());
        }


        /// <summary>
        /// Query by Object
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> QueryByObject(
            [FromBody] IEnumerable<string> paths,
            [FromQuery] long start,
            [FromQuery] long stop,
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
                return await OnQueryByObject(client, paths, start, stop, skip, take, (SortOrder)sortOrder, indentOutput);
            }
            else
            {
                return StatusCode(500, "Entity Client Not Found");
            }
        }

        protected virtual Task<IActionResult> OnQueryByObject(
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
            return Task.FromResult<IActionResult>(NotFound());
        }


        /// <summary>
        /// Query by Object UUID
        /// </summary>
        [HttpGet("object/{objectUuid}")]
        public async Task<IActionResult> QueryByObjectUuid(
            [FromRoute] string objectUuid,
            [FromQuery] long start,
            [FromQuery] long stop,
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
                return await OnQueryByObjectUuid(client, objectUuid, start, stop, skip, take, (SortOrder)sortOrder, indentOutput);
            }
            else
            {
                return StatusCode(500, "Entity Client Not Found");
            }
        }

        protected virtual Task<IActionResult> OnQueryByObjectUuid(
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
            return Task.FromResult<IActionResult>(NotFound());
        }


        /// <summary>
        /// Query by Object UUID
        /// </summary>
        [HttpPost("object")]
        public async Task<IActionResult> QueryByObjectUuid(
            [FromBody] IEnumerable<string> objectUuids,
            [FromQuery] long start,
            [FromQuery] long stop,
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
                return await OnQueryByObjectUuid(client, objectUuids, start, stop, skip, take, (SortOrder)sortOrder, indentOutput);
            }
            else
            {
                return StatusCode(500, "Entity Client Not Found");
            }
        }

        protected virtual Task<IActionResult> OnQueryByObjectUuid(
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
            return Task.FromResult<IActionResult>(NotFound());
        }


        /// <summary>
        /// Subscribe by Object
        /// </summary>
        [Route("object/path/subscribe")]
        public async Task SubscribeByObject(

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
                await OnSubscribeByObject(client, interval, take, consumerId, indentOutput);
            }
            else
            {
                //return StatusCode(500, "Entity Client Not Found");
            }
        }

        protected virtual Task OnSubscribeByObject(
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
        /// Subscribe by Object UUID
        /// </summary>
        [Route("object/subscribe")]
        public async Task SubscribeByObjectUuid(

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
                await OnSubscribeByObjectUuid(client, interval, take, consumerId, indentOutput);
            }
            else
            {
                //return StatusCode(500, "Entity Client Not Found");
            }
        }

        protected virtual Task OnSubscribeByObjectUuid(
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
        /// Delete by Object UUID
        /// </summary>
        [HttpDelete("object/{objectUuid}")]
        public async Task<IActionResult> DeleteByObjectUuid(
            [FromRoute] string objectUuid,
            [FromQuery] string routerId = null,
            [FromQuery] bool indentOutput = false
            )
        {
            var client = GetClient(routerId);
            if (client != null)
            {
                return await OnDeleteByObjectUuid(client, objectUuid, indentOutput);
            }
            else
            {
                return StatusCode(500, "Entity Client Not Found");
            }
        }

        protected virtual Task<IActionResult> OnDeleteByObjectUuid(
            ITrakHoundClient client,
            string objectUuid,
            bool indentOutput = false
        )
        {
            return Task.FromResult<IActionResult>(NotFound());
        }


        /// <summary>
        /// Delete by Object UUID
        /// </summary>
        [HttpPost("delete/object")]
        public async Task<IActionResult> DeleteByObjectUuid(
            [FromBody] IEnumerable<string> objectUuids,
            [FromQuery] string routerId = null,
            [FromQuery] bool indentOutput = false
            )
        {
            var client = GetClient(routerId);
            if (client != null)
            {
                return await OnDeleteByObjectUuid(client, objectUuids, indentOutput);
            }
            else
            {
                return StatusCode(500, "Entity Client Not Found");
            }
        }

        protected virtual Task<IActionResult> OnDeleteByObjectUuid(
            ITrakHoundClient client,
            IEnumerable<string> objectUuids,
            bool indentOutput = false
        )
        {
            return Task.FromResult<IActionResult>(NotFound());
        }

    }
}
