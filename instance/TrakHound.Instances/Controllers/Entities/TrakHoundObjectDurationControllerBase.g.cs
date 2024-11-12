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
    [Route($"{HttpConstants.EntitiesPrefix}/objects/duration")]
    public abstract class TrakHoundObjectDurationControllerBase : TrakHoundHttpEntityControllerBase<ITrakHoundObjectDurationEntity>
    {
        public TrakHoundObjectDurationControllerBase(ITrakHoundInstance instance, TrakHoundHttpWebSocketManager webSocketManager) : base(instance, webSocketManager) { }


        /// <summary>
        /// Query by Object
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> QueryByObject(
            [FromQuery] string path,
            [FromQuery] string routerId = null,
            [FromQuery] bool indentOutput = false
            )
        {
            var client = GetClient(routerId);
            if (client != null)
            {
                return await OnQueryByObject(client, path, indentOutput);
            }
            else
            {
                return StatusCode(500, "Entity Client Not Found");
            }
        }

        protected virtual Task<IActionResult> OnQueryByObject(
            ITrakHoundClient client,
            string path,
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
            [FromQuery] string routerId = null,
            [FromQuery] bool indentOutput = false
            )
        {
            var client = GetClient(routerId);
            if (client != null)
            {
                return await OnQueryByObject(client, paths, indentOutput);
            }
            else
            {
                return StatusCode(500, "Entity Client Not Found");
            }
        }

        protected virtual Task<IActionResult> OnQueryByObject(
            ITrakHoundClient client,
            IEnumerable<string> paths,
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
            [FromQuery] string routerId = null,
            [FromQuery] bool indentOutput = false
            )
        {
            var client = GetClient(routerId);
            if (client != null)
            {
                return await OnQueryByObjectUuid(client, objectUuid, indentOutput);
            }
            else
            {
                return StatusCode(500, "Entity Client Not Found");
            }
        }

        protected virtual Task<IActionResult> OnQueryByObjectUuid(
            ITrakHoundClient client,
            string objectUuid,
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
            [FromQuery] string routerId = null,
            [FromQuery] bool indentOutput = false
            )
        {
            var client = GetClient(routerId);
            if (client != null)
            {
                return await OnQueryByObjectUuid(client, objectUuids, indentOutput);
            }
            else
            {
                return StatusCode(500, "Entity Client Not Found");
            }
        }

        protected virtual Task<IActionResult> OnQueryByObjectUuid(
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
        [HttpGet("object/filter")]
        public async Task<IActionResult> QueryByObject(
            [FromQuery] string path,
            [FromQuery] long minimum,
            [FromQuery] long maximum,
            [FromQuery] long objectSkip,
            [FromQuery] long objectTake,
            [FromQuery] string routerId = null,
            [FromQuery] bool indentOutput = false
            )
        {
            var client = GetClient(routerId);
            if (client != null)
            {
                return await OnQueryByObject(client, path, minimum, maximum, objectSkip, objectTake, indentOutput);
            }
            else
            {
                return StatusCode(500, "Entity Client Not Found");
            }
        }

        protected virtual Task<IActionResult> OnQueryByObject(
            ITrakHoundClient client,
            string path,
            long minimum,
            long maximum,
            long objectSkip,
            long objectTake,
            bool indentOutput = false
        )
        {
            return Task.FromResult<IActionResult>(NotFound());
        }


        /// <summary>
        /// Query by Object
        /// </summary>
        [HttpPost("object/path/filter")]
        public async Task<IActionResult> QueryByObject(
            [FromBody] IEnumerable<string> paths,
            [FromQuery] long minimum,
            [FromQuery] long maximum,
            [FromQuery] long objectSkip,
            [FromQuery] long objectTake,
            [FromQuery] string routerId = null,
            [FromQuery] bool indentOutput = false
            )
        {
            var client = GetClient(routerId);
            if (client != null)
            {
                return await OnQueryByObject(client, paths, minimum, maximum, objectSkip, objectTake, indentOutput);
            }
            else
            {
                return StatusCode(500, "Entity Client Not Found");
            }
        }

        protected virtual Task<IActionResult> OnQueryByObject(
            ITrakHoundClient client,
            IEnumerable<string> paths,
            long minimum,
            long maximum,
            long objectSkip,
            long objectTake,
            bool indentOutput = false
        )
        {
            return Task.FromResult<IActionResult>(NotFound());
        }


        /// <summary>
        /// Query by Object UUID
        /// </summary>
        [HttpGet("object/{objectUuid}/filter")]
        public async Task<IActionResult> QueryByObjectUuid(
            [FromRoute] string objectUuid,
            [FromQuery] long minimum,
            [FromQuery] long maximum,
            [FromQuery] string routerId = null,
            [FromQuery] bool indentOutput = false
            )
        {
            var client = GetClient(routerId);
            if (client != null)
            {
                return await OnQueryByObjectUuid(client, objectUuid, minimum, maximum, indentOutput);
            }
            else
            {
                return StatusCode(500, "Entity Client Not Found");
            }
        }

        protected virtual Task<IActionResult> OnQueryByObjectUuid(
            ITrakHoundClient client,
            string objectUuid,
            long minimum,
            long maximum,
            bool indentOutput = false
        )
        {
            return Task.FromResult<IActionResult>(NotFound());
        }


        /// <summary>
        /// Query by Object UUID
        /// </summary>
        [HttpPost("object/filter")]
        public async Task<IActionResult> QueryByObjectUuid(
            [FromBody] IEnumerable<string> objectUuids,
            [FromQuery] long minimum,
            [FromQuery] long maximum,
            [FromQuery] string routerId = null,
            [FromQuery] bool indentOutput = false
            )
        {
            var client = GetClient(routerId);
            if (client != null)
            {
                return await OnQueryByObjectUuid(client, objectUuids, minimum, maximum, indentOutput);
            }
            else
            {
                return StatusCode(500, "Entity Client Not Found");
            }
        }

        protected virtual Task<IActionResult> OnQueryByObjectUuid(
            ITrakHoundClient client,
            IEnumerable<string> objectUuids,
            long minimum,
            long maximum,
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

    }
}
