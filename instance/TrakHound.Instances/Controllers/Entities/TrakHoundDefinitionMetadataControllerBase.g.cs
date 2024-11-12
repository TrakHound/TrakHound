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
    [Route($"{HttpConstants.EntitiesPrefix}/definitions/metadata")]
    public abstract class TrakHoundDefinitionMetadataControllerBase : TrakHoundHttpEntityControllerBase<ITrakHoundDefinitionMetadataEntity>
    {
        public TrakHoundDefinitionMetadataControllerBase(ITrakHoundInstance instance, TrakHoundHttpWebSocketManager webSocketManager) : base(instance, webSocketManager) { }


        /// <summary>
        /// Query by Definition UUID
        /// </summary>
        [HttpGet("definition/{definitionUuid}")]
        public async Task<IActionResult> QueryByDefinitionUuid(
            [FromRoute] string definitionUuid,
            [FromQuery] string routerId = null,
            [FromQuery] bool indentOutput = false
            )
        {
            var client = GetClient(routerId);
            if (client != null)
            {
                return await OnQueryByDefinitionUuid(client, definitionUuid, indentOutput);
            }
            else
            {
                return StatusCode(500, "Entity Client Not Found");
            }
        }

        protected virtual Task<IActionResult> OnQueryByDefinitionUuid(
            ITrakHoundClient client,
            string definitionUuid,
            bool indentOutput = false
        )
        {
            return Task.FromResult<IActionResult>(NotFound());
        }


        /// <summary>
        /// Query by Definition UUID
        /// </summary>
        [HttpPost("definition")]
        public async Task<IActionResult> QueryByDefinitionUuid(
            [FromBody] IEnumerable<string> definitionUuids,
            [FromQuery] string routerId = null,
            [FromQuery] bool indentOutput = false
            )
        {
            var client = GetClient(routerId);
            if (client != null)
            {
                return await OnQueryByDefinitionUuid(client, definitionUuids, indentOutput);
            }
            else
            {
                return StatusCode(500, "Entity Client Not Found");
            }
        }

        protected virtual Task<IActionResult> OnQueryByDefinitionUuid(
            ITrakHoundClient client,
            IEnumerable<string> definitionUuids,
            bool indentOutput = false
        )
        {
            return Task.FromResult<IActionResult>(NotFound());
        }

    }
}
