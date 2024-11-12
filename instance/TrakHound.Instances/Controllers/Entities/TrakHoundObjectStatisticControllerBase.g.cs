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
    [Route($"{HttpConstants.EntitiesPrefix}/objects/statistic")]
    public abstract class TrakHoundObjectStatisticControllerBase : TrakHoundHttpEntityControllerBase<ITrakHoundObjectStatisticEntity>
    {
        public TrakHoundObjectStatisticControllerBase(ITrakHoundInstance instance, TrakHoundHttpWebSocketManager webSocketManager) : base(instance, webSocketManager) { }


        /// <summary>
        /// Query by Object
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> QueryByObject(
            [FromQuery] string path,
            [FromQuery] long start,
            [FromQuery] long stop,
            [FromQuery] long span,
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
                return await OnQueryByObject(client, path, start, stop, span, skip, take, (SortOrder)sortOrder, indentOutput);
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
            long span,
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
        [HttpPost("object/path")]
        public async Task<IActionResult> QueryByObject(
            [FromBody] IEnumerable<string> paths,
            [FromQuery] long start,
            [FromQuery] long stop,
            [FromQuery] long span,
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
                return await OnQueryByObject(client, paths, start, stop, span, skip, take, (SortOrder)sortOrder, indentOutput);
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
            long span,
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
            [FromQuery] long span,
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
                return await OnQueryByObjectUuid(client, objectUuid, start, stop, span, skip, take, (SortOrder)sortOrder, indentOutput);
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
            long span,
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
            [FromQuery] long span,
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
                return await OnQueryByObjectUuid(client, objectUuids, start, stop, span, skip, take, (SortOrder)sortOrder, indentOutput);
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
            long span,
            long skip,
            long take,
            SortOrder sortOrder,
            bool indentOutput = false
        )
        {
            return Task.FromResult<IActionResult>(NotFound());
        }


        /// <summary>
        /// Spans by Object
        /// </summary>
        [HttpGet("spans")]
        public async Task<IActionResult> SpansByObject(
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
                return await OnSpansByObject(client, path, start, stop, skip, take, (SortOrder)sortOrder, indentOutput);
            }
            else
            {
                return StatusCode(500, "Entity Client Not Found");
            }
        }

        protected virtual Task<IActionResult> OnSpansByObject(
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
        /// Spans by Object
        /// </summary>
        [HttpPost("spans/object/path")]
        public async Task<IActionResult> SpansByObject(
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
                return await OnSpansByObject(client, paths, start, stop, skip, take, (SortOrder)sortOrder, indentOutput);
            }
            else
            {
                return StatusCode(500, "Entity Client Not Found");
            }
        }

        protected virtual Task<IActionResult> OnSpansByObject(
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
        /// Spans by Object UUID
        /// </summary>
        [HttpGet("object/{objectUuid}/spans")]
        public async Task<IActionResult> SpansByObjectUuid(
            [FromRoute] string objectUuid,
            [FromQuery] long start,
            [FromQuery] long stop,
            [FromQuery] string routerId = null,
            [FromQuery] bool indentOutput = false
            )
        {
            var client = GetClient(routerId);
            if (client != null)
            {
                return await OnSpansByObjectUuid(client, objectUuid, start, stop, indentOutput);
            }
            else
            {
                return StatusCode(500, "Entity Client Not Found");
            }
        }

        protected virtual Task<IActionResult> OnSpansByObjectUuid(
            ITrakHoundClient client,
            string objectUuid,
            long start,
            long stop,
            bool indentOutput = false
        )
        {
            return Task.FromResult<IActionResult>(NotFound());
        }


        /// <summary>
        /// Spans by Object UUID
        /// </summary>
        [HttpPost("object/spans")]
        public async Task<IActionResult> SpansByObjectUuid(
            [FromBody] IEnumerable<string> objectUuids,
            [FromQuery] long start,
            [FromQuery] long stop,
            [FromQuery] string routerId = null,
            [FromQuery] bool indentOutput = false
            )
        {
            var client = GetClient(routerId);
            if (client != null)
            {
                return await OnSpansByObjectUuid(client, objectUuids, start, stop, indentOutput);
            }
            else
            {
                return StatusCode(500, "Entity Client Not Found");
            }
        }

        protected virtual Task<IActionResult> OnSpansByObjectUuid(
            ITrakHoundClient client,
            IEnumerable<string> objectUuids,
            long start,
            long stop,
            bool indentOutput = false
        )
        {
            return Task.FromResult<IActionResult>(NotFound());
        }


        /// <summary>
        /// Count by Object
        /// </summary>
        [HttpGet("count")]
        public async Task<IActionResult> CountByObject(
            [FromQuery] string path,
            [FromQuery] long start,
            [FromQuery] long stop,
            [FromQuery] long span,
            [FromQuery] string routerId = null,
            [FromQuery] bool indentOutput = false
            )
        {
            var client = GetClient(routerId);
            if (client != null)
            {
                return await OnCountByObject(client, path, start, stop, span, indentOutput);
            }
            else
            {
                return StatusCode(500, "Entity Client Not Found");
            }
        }

        protected virtual Task<IActionResult> OnCountByObject(
            ITrakHoundClient client,
            string path,
            long start,
            long stop,
            long span,
            bool indentOutput = false
        )
        {
            return Task.FromResult<IActionResult>(NotFound());
        }


        /// <summary>
        /// Count by Object
        /// </summary>
        [HttpPost("count/object/path")]
        public async Task<IActionResult> CountByObject(
            [FromBody] IEnumerable<string> paths,
            [FromQuery] long start,
            [FromQuery] long stop,
            [FromQuery] long span,
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
                return await OnCountByObject(client, paths, start, stop, span, skip, take, (SortOrder)sortOrder, indentOutput);
            }
            else
            {
                return StatusCode(500, "Entity Client Not Found");
            }
        }

        protected virtual Task<IActionResult> OnCountByObject(
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
            return Task.FromResult<IActionResult>(NotFound());
        }


        /// <summary>
        /// Count by Object UUID
        /// </summary>
        [HttpGet("object/{objectUuid}/count")]
        public async Task<IActionResult> CountByObjectUuid(
            [FromRoute] string objectUuid,
            [FromQuery] long start,
            [FromQuery] long stop,
            [FromQuery] long span,
            [FromQuery] string routerId = null,
            [FromQuery] bool indentOutput = false
            )
        {
            var client = GetClient(routerId);
            if (client != null)
            {
                return await OnCountByObjectUuid(client, objectUuid, start, stop, span, indentOutput);
            }
            else
            {
                return StatusCode(500, "Entity Client Not Found");
            }
        }

        protected virtual Task<IActionResult> OnCountByObjectUuid(
            ITrakHoundClient client,
            string objectUuid,
            long start,
            long stop,
            long span,
            bool indentOutput = false
        )
        {
            return Task.FromResult<IActionResult>(NotFound());
        }


        /// <summary>
        /// Count by Object UUID
        /// </summary>
        [HttpPost("object/count")]
        public async Task<IActionResult> CountByObjectUuid(
            [FromBody] IEnumerable<string> objectUuids,
            [FromQuery] long start,
            [FromQuery] long stop,
            [FromQuery] long span,
            [FromQuery] string routerId = null,
            [FromQuery] bool indentOutput = false
            )
        {
            var client = GetClient(routerId);
            if (client != null)
            {
                return await OnCountByObjectUuid(client, objectUuids, start, stop, span, indentOutput);
            }
            else
            {
                return StatusCode(500, "Entity Client Not Found");
            }
        }

        protected virtual Task<IActionResult> OnCountByObjectUuid(
            ITrakHoundClient client,
            IEnumerable<string> objectUuids,
            long start,
            long stop,
            long span,
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
