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
    [Route($"{HttpConstants.EntitiesPrefix}/objects/metadata")]
    public abstract class TrakHoundObjectMetadataControllerBase : TrakHoundHttpEntityControllerBase<ITrakHoundObjectMetadataEntity>
    {
        public TrakHoundObjectMetadataControllerBase(ITrakHoundInstance instance, TrakHoundHttpWebSocketManager webSocketManager) : base(instance, webSocketManager) { }


        /// <summary>
        /// Query by Entity UUID
        /// </summary>
        [HttpGet("entity/{entityUuid}")]
        public async Task<IActionResult> QueryByEntityUuid(
            [FromRoute] string entityUuid,
            [FromQuery] string routerId = null,
            [FromQuery] bool indentOutput = false
            )
        {
            var client = GetClient(routerId);
            if (client != null)
            {
                return await OnQueryByEntityUuid(client, entityUuid, indentOutput);
            }
            else
            {
                return StatusCode(500, "Entity Client Not Found");
            }
        }

        protected virtual Task<IActionResult> OnQueryByEntityUuid(
            ITrakHoundClient client,
            string entityUuid,
            bool indentOutput = false
        )
        {
            return Task.FromResult<IActionResult>(NotFound());
        }


        /// <summary>
        /// Query by Entity UUID
        /// </summary>
        [HttpPost("entity")]
        public async Task<IActionResult> QueryByEntityUuid(
            [FromBody] IEnumerable<string> entityUuids,
            [FromQuery] string routerId = null,
            [FromQuery] bool indentOutput = false
            )
        {
            var client = GetClient(routerId);
            if (client != null)
            {
                return await OnQueryByEntityUuid(client, entityUuids, indentOutput);
            }
            else
            {
                return StatusCode(500, "Entity Client Not Found");
            }
        }

        protected virtual Task<IActionResult> OnQueryByEntityUuid(
            ITrakHoundClient client,
            IEnumerable<string> entityUuids,
            bool indentOutput = false
        )
        {
            return Task.FromResult<IActionResult>(NotFound());
        }


        /// <summary>
        /// Query by Entity UUID
        /// </summary>
        [HttpGet("entity/name/{entityUuid}")]
        public async Task<IActionResult> QueryByEntityUuid(
            [FromRoute] string entityUuid,
            [FromQuery] string name,
            [FromQuery] TrakHound.Entities.TrakHoundMetadataQueryType queryType,
            [FromQuery] string query,
            [FromQuery] string routerId = null,
            [FromQuery] bool indentOutput = false
            )
        {
            var client = GetClient(routerId);
            if (client != null)
            {
                return await OnQueryByEntityUuid(client, entityUuid, name, queryType, query, indentOutput);
            }
            else
            {
                return StatusCode(500, "Entity Client Not Found");
            }
        }

        protected virtual Task<IActionResult> OnQueryByEntityUuid(
            ITrakHoundClient client,
            string entityUuid,
            string name,
            TrakHound.Entities.TrakHoundMetadataQueryType queryType,
            string query,
            bool indentOutput = false
        )
        {
            return Task.FromResult<IActionResult>(NotFound());
        }


        /// <summary>
        /// Query by Entity UUID
        /// </summary>
        [HttpPost("entity/name")]
        public async Task<IActionResult> QueryByEntityUuid(
            [FromBody] IEnumerable<string> entityUuids,
            [FromQuery] string name,
            [FromQuery] TrakHound.Entities.TrakHoundMetadataQueryType queryType,
            [FromQuery] string query,
            [FromQuery] string routerId = null,
            [FromQuery] bool indentOutput = false
            )
        {
            var client = GetClient(routerId);
            if (client != null)
            {
                return await OnQueryByEntityUuid(client, entityUuids, name, queryType, query, indentOutput);
            }
            else
            {
                return StatusCode(500, "Entity Client Not Found");
            }
        }

        protected virtual Task<IActionResult> OnQueryByEntityUuid(
            ITrakHoundClient client,
            IEnumerable<string> entityUuids,
            string name,
            TrakHound.Entities.TrakHoundMetadataQueryType queryType,
            string query,
            bool indentOutput = false
        )
        {
            return Task.FromResult<IActionResult>(NotFound());
        }


        /// <summary>
        /// Query by Name
        /// </summary>
        [HttpGet("name")]
        public async Task<IActionResult> QueryByName(
            [FromQuery] string name,
            [FromQuery] TrakHound.Entities.TrakHoundMetadataQueryType queryType,
            [FromQuery] string query,
            [FromQuery] string routerId = null,
            [FromQuery] bool indentOutput = false
            )
        {
            var client = GetClient(routerId);
            if (client != null)
            {
                return await OnQueryByName(client, name, queryType, query, indentOutput);
            }
            else
            {
                return StatusCode(500, "Entity Client Not Found");
            }
        }

        protected virtual Task<IActionResult> OnQueryByName(
            ITrakHoundClient client,
            string name,
            TrakHound.Entities.TrakHoundMetadataQueryType queryType,
            string query,
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
