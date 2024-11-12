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
    [Route($"{HttpConstants.EntitiesPrefix}/sources/metadata")]
    public abstract class TrakHoundSourceMetadataControllerBase : TrakHoundHttpEntityControllerBase<ITrakHoundSourceMetadataEntity>
    {
        public TrakHoundSourceMetadataControllerBase(ITrakHoundInstance instance, TrakHoundHttpWebSocketManager webSocketManager) : base(instance, webSocketManager) { }


        /// <summary>
        /// Query by Source UUID
        /// </summary>
        [HttpGet("source/{sourceUuid}")]
        public async Task<IActionResult> QueryBySourceUuid(
            [FromRoute] string sourceUuid,
            [FromQuery] string routerId = null,
            [FromQuery] bool indentOutput = false
            )
        {
            var client = GetClient(routerId);
            if (client != null)
            {
                return await OnQueryBySourceUuid(client, sourceUuid, indentOutput);
            }
            else
            {
                return StatusCode(500, "Entity Client Not Found");
            }
        }

        protected virtual Task<IActionResult> OnQueryBySourceUuid(
            ITrakHoundClient client,
            string sourceUuid,
            bool indentOutput = false
        )
        {
            return Task.FromResult<IActionResult>(NotFound());
        }


        /// <summary>
        /// Query by Source UUID
        /// </summary>
        [HttpPost("source")]
        public async Task<IActionResult> QueryBySourceUuid(
            [FromBody] IEnumerable<string> sourceUuids,
            [FromQuery] string routerId = null,
            [FromQuery] bool indentOutput = false
            )
        {
            var client = GetClient(routerId);
            if (client != null)
            {
                return await OnQueryBySourceUuid(client, sourceUuids, indentOutput);
            }
            else
            {
                return StatusCode(500, "Entity Client Not Found");
            }
        }

        protected virtual Task<IActionResult> OnQueryBySourceUuid(
            ITrakHoundClient client,
            IEnumerable<string> sourceUuids,
            bool indentOutput = false
        )
        {
            return Task.FromResult<IActionResult>(NotFound());
        }

    }
}
