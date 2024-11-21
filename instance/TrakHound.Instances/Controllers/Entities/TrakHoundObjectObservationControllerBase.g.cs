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
    [Route($"{HttpConstants.EntitiesPrefix}/objects/observation")]
    public abstract class TrakHoundObjectObservationControllerBase : TrakHoundHttpEntityControllerBase<ITrakHoundObjectObservationEntity>
    {
        public TrakHoundObjectObservationControllerBase(ITrakHoundInstance instance, TrakHoundHttpWebSocketManager webSocketManager) : base(instance, webSocketManager) { }


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
        [HttpPost("object/path")]
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
        /// Last by Object
        /// </summary>
        [HttpGet("last")]
        public async Task<IActionResult> LastByObject(
            [FromQuery] string path,
            [FromQuery] long timestamp,
            [FromQuery] string routerId = null,
            [FromQuery] bool indentOutput = false
            )
        {
            var client = GetClient(routerId);
            if (client != null)
            {
                return await OnLastByObject(client, path, timestamp, indentOutput);
            }
            else
            {
                return StatusCode(500, "Entity Client Not Found");
            }
        }

        protected virtual Task<IActionResult> OnLastByObject(
            ITrakHoundClient client,
            string path,
            long timestamp,
            bool indentOutput = false
        )
        {
            return Task.FromResult<IActionResult>(NotFound());
        }


        /// <summary>
        /// Last by Object
        /// </summary>
        [HttpPost("last")]
        public async Task<IActionResult> LastByObject(
            [FromBody] IEnumerable<string> paths,
            [FromQuery] long timestamp,
            [FromQuery] string routerId = null,
            [FromQuery] bool indentOutput = false
            )
        {
            var client = GetClient(routerId);
            if (client != null)
            {
                return await OnLastByObject(client, paths, timestamp, indentOutput);
            }
            else
            {
                return StatusCode(500, "Entity Client Not Found");
            }
        }

        protected virtual Task<IActionResult> OnLastByObject(
            ITrakHoundClient client,
            IEnumerable<string> paths,
            long timestamp,
            bool indentOutput = false
        )
        {
            return Task.FromResult<IActionResult>(NotFound());
        }


        /// <summary>
        /// Last by Object UUID
        /// </summary>
        [HttpGet("last/object/{objectUuid}")]
        public async Task<IActionResult> LastByObjectUuid(
            [FromRoute] string objectUuid,
            [FromQuery] long timestamp,
            [FromQuery] string routerId = null,
            [FromQuery] bool indentOutput = false
            )
        {
            var client = GetClient(routerId);
            if (client != null)
            {
                return await OnLastByObjectUuid(client, objectUuid, timestamp, indentOutput);
            }
            else
            {
                return StatusCode(500, "Entity Client Not Found");
            }
        }

        protected virtual Task<IActionResult> OnLastByObjectUuid(
            ITrakHoundClient client,
            string objectUuid,
            long timestamp,
            bool indentOutput = false
        )
        {
            return Task.FromResult<IActionResult>(NotFound());
        }


        /// <summary>
        /// Last by Object UUID
        /// </summary>
        [HttpPost("last/object")]
        public async Task<IActionResult> LastByObjectUuid(
            [FromBody] IEnumerable<string> objectUuids,
            [FromQuery] long timestamp,
            [FromQuery] string routerId = null,
            [FromQuery] bool indentOutput = false
            )
        {
            var client = GetClient(routerId);
            if (client != null)
            {
                return await OnLastByObjectUuid(client, objectUuids, timestamp, indentOutput);
            }
            else
            {
                return StatusCode(500, "Entity Client Not Found");
            }
        }

        protected virtual Task<IActionResult> OnLastByObjectUuid(
            ITrakHoundClient client,
            IEnumerable<string> objectUuids,
            long timestamp,
            bool indentOutput = false
        )
        {
            return Task.FromResult<IActionResult>(NotFound());
        }


        /// <summary>
        /// Aggregate by Object
        /// </summary>
        [HttpGet("aggregate")]
        public async Task<IActionResult> AggregateByObject(
            [FromQuery] string path,
            [FromQuery] TrakHound.TrakHoundAggregateType aggregateType,
            [FromQuery] long start,
            [FromQuery] long stop,
            [FromQuery] string routerId = null,
            [FromQuery] bool indentOutput = false
            )
        {
            var client = GetClient(routerId);
            if (client != null)
            {
                return await OnAggregateByObject(client, path, aggregateType, start, stop, indentOutput);
            }
            else
            {
                return StatusCode(500, "Entity Client Not Found");
            }
        }

        protected virtual Task<IActionResult> OnAggregateByObject(
            ITrakHoundClient client,
            string path,
            TrakHound.TrakHoundAggregateType aggregateType,
            long start,
            long stop,
            bool indentOutput = false
        )
        {
            return Task.FromResult<IActionResult>(NotFound());
        }


        /// <summary>
        /// Aggregate by Object
        /// </summary>
        [HttpPost("aggregate")]
        public async Task<IActionResult> AggregateByObject(
            [FromBody] IEnumerable<string> paths,
            [FromQuery] TrakHound.TrakHoundAggregateType aggregateType,
            [FromQuery] long start,
            [FromQuery] long stop,
            [FromQuery] string routerId = null,
            [FromQuery] bool indentOutput = false
            )
        {
            var client = GetClient(routerId);
            if (client != null)
            {
                return await OnAggregateByObject(client, paths, aggregateType, start, stop, indentOutput);
            }
            else
            {
                return StatusCode(500, "Entity Client Not Found");
            }
        }

        protected virtual Task<IActionResult> OnAggregateByObject(
            ITrakHoundClient client,
            IEnumerable<string> paths,
            TrakHound.TrakHoundAggregateType aggregateType,
            long start,
            long stop,
            bool indentOutput = false
        )
        {
            return Task.FromResult<IActionResult>(NotFound());
        }


        /// <summary>
        /// Aggregate by Object UUID
        /// </summary>
        [HttpGet("aggregate/object/{objectUuid}")]
        public async Task<IActionResult> AggregateByObjectUuid(
            [FromRoute] string objectUuid,
            [FromQuery] TrakHound.TrakHoundAggregateType aggregateType,
            [FromQuery] long start,
            [FromQuery] long stop,
            [FromQuery] string routerId = null,
            [FromQuery] bool indentOutput = false
            )
        {
            var client = GetClient(routerId);
            if (client != null)
            {
                return await OnAggregateByObjectUuid(client, objectUuid, aggregateType, start, stop, indentOutput);
            }
            else
            {
                return StatusCode(500, "Entity Client Not Found");
            }
        }

        protected virtual Task<IActionResult> OnAggregateByObjectUuid(
            ITrakHoundClient client,
            string objectUuid,
            TrakHound.TrakHoundAggregateType aggregateType,
            long start,
            long stop,
            bool indentOutput = false
        )
        {
            return Task.FromResult<IActionResult>(NotFound());
        }


        /// <summary>
        /// Aggregate by Object UUID
        /// </summary>
        [HttpPost("aggregate/object")]
        public async Task<IActionResult> AggregateByObjectUuid(
            [FromBody] IEnumerable<string> objectUuids,
            [FromQuery] TrakHound.TrakHoundAggregateType aggregateType,
            [FromQuery] long start,
            [FromQuery] long stop,
            [FromQuery] string routerId = null,
            [FromQuery] bool indentOutput = false
            )
        {
            var client = GetClient(routerId);
            if (client != null)
            {
                return await OnAggregateByObjectUuid(client, objectUuids, aggregateType, start, stop, indentOutput);
            }
            else
            {
                return StatusCode(500, "Entity Client Not Found");
            }
        }

        protected virtual Task<IActionResult> OnAggregateByObjectUuid(
            ITrakHoundClient client,
            IEnumerable<string> objectUuids,
            TrakHound.TrakHoundAggregateType aggregateType,
            long start,
            long stop,
            bool indentOutput = false
        )
        {
            return Task.FromResult<IActionResult>(NotFound());
        }


        /// <summary>
        /// Aggregate Window by Object
        /// </summary>
        [HttpGet("aggregate/window")]
        public async Task<IActionResult> AggregateWindowByObject(
            [FromQuery] string path,
            [FromQuery] TrakHound.TrakHoundAggregateType aggregateType,
            [FromQuery] long window,
            [FromQuery] long start,
            [FromQuery] long stop,
            [FromQuery] string routerId = null,
            [FromQuery] bool indentOutput = false
            )
        {
            var client = GetClient(routerId);
            if (client != null)
            {
                return await OnAggregateWindowByObject(client, path, aggregateType, window, start, stop, indentOutput);
            }
            else
            {
                return StatusCode(500, "Entity Client Not Found");
            }
        }

        protected virtual Task<IActionResult> OnAggregateWindowByObject(
            ITrakHoundClient client,
            string path,
            TrakHound.TrakHoundAggregateType aggregateType,
            long window,
            long start,
            long stop,
            bool indentOutput = false
        )
        {
            return Task.FromResult<IActionResult>(NotFound());
        }


        /// <summary>
        /// Aggregate Window by Object
        /// </summary>
        [HttpPost("aggregate/window")]
        public async Task<IActionResult> AggregateWindowByObject(
            [FromBody] IEnumerable<string> paths,
            [FromQuery] TrakHound.TrakHoundAggregateType aggregateType,
            [FromQuery] long window,
            [FromQuery] long start,
            [FromQuery] long stop,
            [FromQuery] string routerId = null,
            [FromQuery] bool indentOutput = false
            )
        {
            var client = GetClient(routerId);
            if (client != null)
            {
                return await OnAggregateWindowByObject(client, paths, aggregateType, window, start, stop, indentOutput);
            }
            else
            {
                return StatusCode(500, "Entity Client Not Found");
            }
        }

        protected virtual Task<IActionResult> OnAggregateWindowByObject(
            ITrakHoundClient client,
            IEnumerable<string> paths,
            TrakHound.TrakHoundAggregateType aggregateType,
            long window,
            long start,
            long stop,
            bool indentOutput = false
        )
        {
            return Task.FromResult<IActionResult>(NotFound());
        }


        /// <summary>
        /// Aggregate Window by Object UUID
        /// </summary>
        [HttpGet("aggregate/window/object/{objectUuid}")]
        public async Task<IActionResult> AggregateWindowByObjectUuid(
            [FromRoute] string objectUuid,
            [FromQuery] TrakHound.TrakHoundAggregateType aggregateType,
            [FromQuery] long window,
            [FromQuery] long start,
            [FromQuery] long stop,
            [FromQuery] string routerId = null,
            [FromQuery] bool indentOutput = false
            )
        {
            var client = GetClient(routerId);
            if (client != null)
            {
                return await OnAggregateWindowByObjectUuid(client, objectUuid, aggregateType, window, start, stop, indentOutput);
            }
            else
            {
                return StatusCode(500, "Entity Client Not Found");
            }
        }

        protected virtual Task<IActionResult> OnAggregateWindowByObjectUuid(
            ITrakHoundClient client,
            string objectUuid,
            TrakHound.TrakHoundAggregateType aggregateType,
            long window,
            long start,
            long stop,
            bool indentOutput = false
        )
        {
            return Task.FromResult<IActionResult>(NotFound());
        }


        /// <summary>
        /// Aggregate Window by Object UUID
        /// </summary>
        [HttpPost("aggregate/window/object")]
        public async Task<IActionResult> AggregateWindowByObjectUuid(
            [FromBody] IEnumerable<string> objectUuids,
            [FromQuery] TrakHound.TrakHoundAggregateType aggregateType,
            [FromQuery] long window,
            [FromQuery] long start,
            [FromQuery] long stop,
            [FromQuery] string routerId = null,
            [FromQuery] bool indentOutput = false
            )
        {
            var client = GetClient(routerId);
            if (client != null)
            {
                return await OnAggregateWindowByObjectUuid(client, objectUuids, aggregateType, window, start, stop, indentOutput);
            }
            else
            {
                return StatusCode(500, "Entity Client Not Found");
            }
        }

        protected virtual Task<IActionResult> OnAggregateWindowByObjectUuid(
            ITrakHoundClient client,
            IEnumerable<string> objectUuids,
            TrakHound.TrakHoundAggregateType aggregateType,
            long window,
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
        [HttpGet]
        public async Task<IActionResult> CountByObject(
            [FromQuery] string path,
            [FromQuery] long start,
            [FromQuery] long stop,
            [FromQuery] string routerId = null,
            [FromQuery] bool indentOutput = false
            )
        {
            var client = GetClient(routerId);
            if (client != null)
            {
                return await OnCountByObject(client, path, start, stop, indentOutput);
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
            bool indentOutput = false
        )
        {
            return Task.FromResult<IActionResult>(NotFound());
        }


        /// <summary>
        /// Count by Object
        /// </summary>
        [HttpPost("object/path")]
        public async Task<IActionResult> CountByObject(
            [FromBody] IEnumerable<string> paths,
            [FromQuery] long start,
            [FromQuery] long stop,
            [FromQuery] string routerId = null,
            [FromQuery] bool indentOutput = false
            )
        {
            var client = GetClient(routerId);
            if (client != null)
            {
                return await OnCountByObject(client, paths, start, stop, indentOutput);
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
            bool indentOutput = false
        )
        {
            return Task.FromResult<IActionResult>(NotFound());
        }


        /// <summary>
        /// Count by Object UUID
        /// </summary>
        [HttpGet("object/{objectUuid}")]
        public async Task<IActionResult> CountByObjectUuid(
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
                return await OnCountByObjectUuid(client, objectUuid, start, stop, indentOutput);
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
            bool indentOutput = false
        )
        {
            return Task.FromResult<IActionResult>(NotFound());
        }


        /// <summary>
        /// Count by Object UUID
        /// </summary>
        [HttpPost("object")]
        public async Task<IActionResult> CountByObjectUuid(
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
                return await OnCountByObjectUuid(client, objectUuids, start, stop, indentOutput);
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
