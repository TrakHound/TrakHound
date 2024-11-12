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
    [Route($"{HttpConstants.EntitiesPrefix}/objects/group")]
    public abstract class TrakHoundObjectGroupControllerBase : TrakHoundHttpEntityControllerBase<ITrakHoundObjectGroupEntity>
    {
        public TrakHoundObjectGroupControllerBase(ITrakHoundInstance instance, TrakHoundHttpWebSocketManager webSocketManager) : base(instance, webSocketManager) { }


        /// <summary>
        /// Query by Group
        /// </summary>
        [HttpGet("group")]
        public async Task<IActionResult> QueryByGroup(
            [FromQuery] string groupPath,
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
                return await OnQueryByGroup(client, groupPath, skip, take, (SortOrder)sortOrder, indentOutput);
            }
            else
            {
                return StatusCode(500, "Entity Client Not Found");
            }
        }

        protected virtual Task<IActionResult> OnQueryByGroup(
            ITrakHoundClient client,
            string groupPath,
            long skip,
            long take,
            SortOrder sortOrder,
            bool indentOutput = false
        )
        {
            return Task.FromResult<IActionResult>(NotFound());
        }


        /// <summary>
        /// Query by Group
        /// </summary>
        [HttpPost("group")]
        public async Task<IActionResult> QueryByGroup(
            [FromBody] IEnumerable<string> groupPaths,
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
                return await OnQueryByGroup(client, groupPaths, skip, take, (SortOrder)sortOrder, indentOutput);
            }
            else
            {
                return StatusCode(500, "Entity Client Not Found");
            }
        }

        protected virtual Task<IActionResult> OnQueryByGroup(
            ITrakHoundClient client,
            IEnumerable<string> groupPaths,
            long skip,
            long take,
            SortOrder sortOrder,
            bool indentOutput = false
        )
        {
            return Task.FromResult<IActionResult>(NotFound());
        }


        /// <summary>
        /// Query by Group UUID
        /// </summary>
        [HttpGet("group/{groupUuid}")]
        public async Task<IActionResult> QueryByGroupUuid(
            [FromRoute] string groupUuid,
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
                return await OnQueryByGroupUuid(client, groupUuid, skip, take, (SortOrder)sortOrder, indentOutput);
            }
            else
            {
                return StatusCode(500, "Entity Client Not Found");
            }
        }

        protected virtual Task<IActionResult> OnQueryByGroupUuid(
            ITrakHoundClient client,
            string groupUuid,
            long skip,
            long take,
            SortOrder sortOrder,
            bool indentOutput = false
        )
        {
            return Task.FromResult<IActionResult>(NotFound());
        }


        /// <summary>
        /// Query by Group UUID
        /// </summary>
        [HttpPost("group")]
        public async Task<IActionResult> QueryByGroupUuid(
            [FromBody] IEnumerable<string> groupUuids,
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
                return await OnQueryByGroupUuid(client, groupUuids, skip, take, (SortOrder)sortOrder, indentOutput);
            }
            else
            {
                return StatusCode(500, "Entity Client Not Found");
            }
        }

        protected virtual Task<IActionResult> OnQueryByGroupUuid(
            ITrakHoundClient client,
            IEnumerable<string> groupUuids,
            long skip,
            long take,
            SortOrder sortOrder,
            bool indentOutput = false
        )
        {
            return Task.FromResult<IActionResult>(NotFound());
        }


        /// <summary>
        /// Query by Member
        /// </summary>
        [HttpGet("member")]
        public async Task<IActionResult> QueryByMember(
            [FromQuery] string memberPath,
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
                return await OnQueryByMember(client, memberPath, skip, take, (SortOrder)sortOrder, indentOutput);
            }
            else
            {
                return StatusCode(500, "Entity Client Not Found");
            }
        }

        protected virtual Task<IActionResult> OnQueryByMember(
            ITrakHoundClient client,
            string memberPath,
            long skip,
            long take,
            SortOrder sortOrder,
            bool indentOutput = false
        )
        {
            return Task.FromResult<IActionResult>(NotFound());
        }


        /// <summary>
        /// Query by Member
        /// </summary>
        [HttpPost("member")]
        public async Task<IActionResult> QueryByMember(
            [FromBody] IEnumerable<string> memberPaths,
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
                return await OnQueryByMember(client, memberPaths, skip, take, (SortOrder)sortOrder, indentOutput);
            }
            else
            {
                return StatusCode(500, "Entity Client Not Found");
            }
        }

        protected virtual Task<IActionResult> OnQueryByMember(
            ITrakHoundClient client,
            IEnumerable<string> memberPaths,
            long skip,
            long take,
            SortOrder sortOrder,
            bool indentOutput = false
        )
        {
            return Task.FromResult<IActionResult>(NotFound());
        }


        /// <summary>
        /// Query by Member UUID
        /// </summary>
        [HttpGet("member/{memberUuid}")]
        public async Task<IActionResult> QueryByMemberUuid(
            [FromRoute] string memberUuid,
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
                return await OnQueryByMemberUuid(client, memberUuid, skip, take, (SortOrder)sortOrder, indentOutput);
            }
            else
            {
                return StatusCode(500, "Entity Client Not Found");
            }
        }

        protected virtual Task<IActionResult> OnQueryByMemberUuid(
            ITrakHoundClient client,
            string memberUuid,
            long skip,
            long take,
            SortOrder sortOrder,
            bool indentOutput = false
        )
        {
            return Task.FromResult<IActionResult>(NotFound());
        }


        /// <summary>
        /// Query by Member UUID
        /// </summary>
        [HttpPost("member")]
        public async Task<IActionResult> QueryByMemberUuid(
            [FromBody] IEnumerable<string> memberUuids,
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
                return await OnQueryByMemberUuid(client, memberUuids, skip, take, (SortOrder)sortOrder, indentOutput);
            }
            else
            {
                return StatusCode(500, "Entity Client Not Found");
            }
        }

        protected virtual Task<IActionResult> OnQueryByMemberUuid(
            ITrakHoundClient client,
            IEnumerable<string> memberUuids,
            long skip,
            long take,
            SortOrder sortOrder,
            bool indentOutput = false
        )
        {
            return Task.FromResult<IActionResult>(NotFound());
        }


        /// <summary>
        /// Subscribe by Group
        /// </summary>
        [Route("group/path/subscribe")]
        public async Task SubscribeByGroup(

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
                await OnSubscribeByGroup(client, interval, take, consumerId, indentOutput);
            }
            else
            {
                //return StatusCode(500, "Entity Client Not Found");
            }
        }

        protected virtual Task OnSubscribeByGroup(
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
        /// Subscribe by Group UUID
        /// </summary>
        [Route("group/subscribe")]
        public async Task SubscribeByGroupUuid(

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
                await OnSubscribeByGroupUuid(client, interval, take, consumerId, indentOutput);
            }
            else
            {
                //return StatusCode(500, "Entity Client Not Found");
            }
        }

        protected virtual Task OnSubscribeByGroupUuid(
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
        /// Subscribe by Member
        /// </summary>
        [Route("member/path/subscribe")]
        public async Task SubscribeByMember(

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
                await OnSubscribeByMember(client, interval, take, consumerId, indentOutput);
            }
            else
            {
                //return StatusCode(500, "Entity Client Not Found");
            }
        }

        protected virtual Task OnSubscribeByMember(
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
        /// Subscribe by Member UUID
        /// </summary>
        [Route("member/subscribe")]
        public async Task SubscribeByMemberUuid(

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
                await OnSubscribeByMemberUuid(client, interval, take, consumerId, indentOutput);
            }
            else
            {
                //return StatusCode(500, "Entity Client Not Found");
            }
        }

        protected virtual Task OnSubscribeByMemberUuid(
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
        /// Delete by Group UUID
        /// </summary>
        [HttpDelete("group/{groupUuid}")]
        public async Task<IActionResult> DeleteByGroupUuid(
            [FromRoute] string groupUuid,
            [FromQuery] string routerId = null,
            [FromQuery] bool indentOutput = false
            )
        {
            var client = GetClient(routerId);
            if (client != null)
            {
                return await OnDeleteByGroupUuid(client, groupUuid, indentOutput);
            }
            else
            {
                return StatusCode(500, "Entity Client Not Found");
            }
        }

        protected virtual Task<IActionResult> OnDeleteByGroupUuid(
            ITrakHoundClient client,
            string groupUuid,
            bool indentOutput = false
        )
        {
            return Task.FromResult<IActionResult>(NotFound());
        }


        /// <summary>
        /// Delete by Group UUID
        /// </summary>
        [HttpPost("delete/group")]
        public async Task<IActionResult> DeleteByGroupUuid(
            [FromBody] IEnumerable<string> groupUuids,
            [FromQuery] string routerId = null,
            [FromQuery] bool indentOutput = false
            )
        {
            var client = GetClient(routerId);
            if (client != null)
            {
                return await OnDeleteByGroupUuid(client, groupUuids, indentOutput);
            }
            else
            {
                return StatusCode(500, "Entity Client Not Found");
            }
        }

        protected virtual Task<IActionResult> OnDeleteByGroupUuid(
            ITrakHoundClient client,
            IEnumerable<string> groupUuids,
            bool indentOutput = false
        )
        {
            return Task.FromResult<IActionResult>(NotFound());
        }


        /// <summary>
        /// Delete by Member UUID
        /// </summary>
        [HttpDelete("member/{memberUuid}")]
        public async Task<IActionResult> DeleteByMemberUuid(
            [FromRoute] string memberUuid,
            [FromQuery] string routerId = null,
            [FromQuery] bool indentOutput = false
            )
        {
            var client = GetClient(routerId);
            if (client != null)
            {
                return await OnDeleteByMemberUuid(client, memberUuid, indentOutput);
            }
            else
            {
                return StatusCode(500, "Entity Client Not Found");
            }
        }

        protected virtual Task<IActionResult> OnDeleteByMemberUuid(
            ITrakHoundClient client,
            string memberUuid,
            bool indentOutput = false
        )
        {
            return Task.FromResult<IActionResult>(NotFound());
        }


        /// <summary>
        /// Delete by Member UUID
        /// </summary>
        [HttpPost("delete/member")]
        public async Task<IActionResult> DeleteByMemberUuid(
            [FromBody] IEnumerable<string> memberUuids,
            [FromQuery] string routerId = null,
            [FromQuery] bool indentOutput = false
            )
        {
            var client = GetClient(routerId);
            if (client != null)
            {
                return await OnDeleteByMemberUuid(client, memberUuids, indentOutput);
            }
            else
            {
                return StatusCode(500, "Entity Client Not Found");
            }
        }

        protected virtual Task<IActionResult> OnDeleteByMemberUuid(
            ITrakHoundClient client,
            IEnumerable<string> memberUuids,
            bool indentOutput = false
        )
        {
            return Task.FromResult<IActionResult>(NotFound());
        }

    }
}
