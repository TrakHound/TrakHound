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
    [Route($"{HttpConstants.EntitiesPrefix}/objects/assignment")]
    public abstract class TrakHoundObjectAssignmentControllerBase : TrakHoundHttpEntityControllerBase<ITrakHoundObjectAssignmentEntity>
    {
        public TrakHoundObjectAssignmentControllerBase(ITrakHoundInstance instance, TrakHoundHttpWebSocketManager webSocketManager) : base(instance, webSocketManager) { }


        /// <summary>
        /// Current by Assignee
        /// </summary>
        [HttpGet("current/assignee")]
        public async Task<IActionResult> CurrentByAssignee(
            [FromQuery] string assigneePath,
            [FromQuery] string routerId = null,
            [FromQuery] bool indentOutput = false
            )
        {
            var client = GetClient(routerId);
            if (client != null)
            {
                return await OnCurrentByAssignee(client, assigneePath, indentOutput);
            }
            else
            {
                return StatusCode(500, "Entity Client Not Found");
            }
        }

        protected virtual Task<IActionResult> OnCurrentByAssignee(
            ITrakHoundClient client,
            string assigneePath,
            bool indentOutput = false
        )
        {
            return Task.FromResult<IActionResult>(NotFound());
        }


        /// <summary>
        /// Current by Assignee
        /// </summary>
        [HttpPost("current/assignee/path")]
        public async Task<IActionResult> CurrentByAssignee(
            [FromBody] IEnumerable<string> assigneePaths,
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
                return await OnCurrentByAssignee(client, assigneePaths, skip, take, (SortOrder)sortOrder, indentOutput);
            }
            else
            {
                return StatusCode(500, "Entity Client Not Found");
            }
        }

        protected virtual Task<IActionResult> OnCurrentByAssignee(
            ITrakHoundClient client,
            IEnumerable<string> assigneePaths,
            long skip,
            long take,
            SortOrder sortOrder,
            bool indentOutput = false
        )
        {
            return Task.FromResult<IActionResult>(NotFound());
        }


        /// <summary>
        /// Current by Assignee UUID
        /// </summary>
        [HttpGet("current/assignee/{assigneeUuid}")]
        public async Task<IActionResult> CurrentByAssigneeUuid(
            [FromRoute] string assigneeUuid,
            [FromQuery] string routerId = null,
            [FromQuery] bool indentOutput = false
            )
        {
            var client = GetClient(routerId);
            if (client != null)
            {
                return await OnCurrentByAssigneeUuid(client, assigneeUuid, indentOutput);
            }
            else
            {
                return StatusCode(500, "Entity Client Not Found");
            }
        }

        protected virtual Task<IActionResult> OnCurrentByAssigneeUuid(
            ITrakHoundClient client,
            string assigneeUuid,
            bool indentOutput = false
        )
        {
            return Task.FromResult<IActionResult>(NotFound());
        }


        /// <summary>
        /// Current by Assignee UUID
        /// </summary>
        [HttpPost("current/assignee")]
        public async Task<IActionResult> CurrentByAssigneeUuid(
            [FromBody] IEnumerable<string> assigneeUuids,
            [FromQuery] string routerId = null,
            [FromQuery] bool indentOutput = false
            )
        {
            var client = GetClient(routerId);
            if (client != null)
            {
                return await OnCurrentByAssigneeUuid(client, assigneeUuids, indentOutput);
            }
            else
            {
                return StatusCode(500, "Entity Client Not Found");
            }
        }

        protected virtual Task<IActionResult> OnCurrentByAssigneeUuid(
            ITrakHoundClient client,
            IEnumerable<string> assigneeUuids,
            bool indentOutput = false
        )
        {
            return Task.FromResult<IActionResult>(NotFound());
        }


        /// <summary>
        /// Current by Member
        /// </summary>
        [HttpGet("current/member")]
        public async Task<IActionResult> CurrentByMember(
            [FromQuery] string memberPath,
            [FromQuery] string routerId = null,
            [FromQuery] bool indentOutput = false
            )
        {
            var client = GetClient(routerId);
            if (client != null)
            {
                return await OnCurrentByMember(client, memberPath, indentOutput);
            }
            else
            {
                return StatusCode(500, "Entity Client Not Found");
            }
        }

        protected virtual Task<IActionResult> OnCurrentByMember(
            ITrakHoundClient client,
            string memberPath,
            bool indentOutput = false
        )
        {
            return Task.FromResult<IActionResult>(NotFound());
        }


        /// <summary>
        /// Current by Member
        /// </summary>
        [HttpPost("current/member/path")]
        public async Task<IActionResult> CurrentByMember(
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
                return await OnCurrentByMember(client, memberPaths, skip, take, (SortOrder)sortOrder, indentOutput);
            }
            else
            {
                return StatusCode(500, "Entity Client Not Found");
            }
        }

        protected virtual Task<IActionResult> OnCurrentByMember(
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
        /// Current by Member UUID
        /// </summary>
        [HttpGet("current/member/{memberUuid}")]
        public async Task<IActionResult> CurrentByMemberUuid(
            [FromRoute] string memberUuid,
            [FromQuery] string routerId = null,
            [FromQuery] bool indentOutput = false
            )
        {
            var client = GetClient(routerId);
            if (client != null)
            {
                return await OnCurrentByMemberUuid(client, memberUuid, indentOutput);
            }
            else
            {
                return StatusCode(500, "Entity Client Not Found");
            }
        }

        protected virtual Task<IActionResult> OnCurrentByMemberUuid(
            ITrakHoundClient client,
            string memberUuid,
            bool indentOutput = false
        )
        {
            return Task.FromResult<IActionResult>(NotFound());
        }


        /// <summary>
        /// Current by Member UUID
        /// </summary>
        [HttpPost("current/member")]
        public async Task<IActionResult> CurrentByMemberUuid(
            [FromBody] IEnumerable<string> memberUuids,
            [FromQuery] string routerId = null,
            [FromQuery] bool indentOutput = false
            )
        {
            var client = GetClient(routerId);
            if (client != null)
            {
                return await OnCurrentByMemberUuid(client, memberUuids, indentOutput);
            }
            else
            {
                return StatusCode(500, "Entity Client Not Found");
            }
        }

        protected virtual Task<IActionResult> OnCurrentByMemberUuid(
            ITrakHoundClient client,
            IEnumerable<string> memberUuids,
            bool indentOutput = false
        )
        {
            return Task.FromResult<IActionResult>(NotFound());
        }


        /// <summary>
        /// Query by Assignee
        /// </summary>
        [HttpGet("assignee")]
        public async Task<IActionResult> QueryByAssignee(
            [FromQuery] string assigneePath,
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
                return await OnQueryByAssignee(client, assigneePath, start, stop, skip, take, (SortOrder)sortOrder, indentOutput);
            }
            else
            {
                return StatusCode(500, "Entity Client Not Found");
            }
        }

        protected virtual Task<IActionResult> OnQueryByAssignee(
            ITrakHoundClient client,
            string assigneePath,
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
        /// Query by Assignee
        /// </summary>
        [HttpPost("assignee/path")]
        public async Task<IActionResult> QueryByAssignee(
            [FromBody] IEnumerable<string> assigneePaths,
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
                return await OnQueryByAssignee(client, assigneePaths, start, stop, skip, take, (SortOrder)sortOrder, indentOutput);
            }
            else
            {
                return StatusCode(500, "Entity Client Not Found");
            }
        }

        protected virtual Task<IActionResult> OnQueryByAssignee(
            ITrakHoundClient client,
            IEnumerable<string> assigneePaths,
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
        /// Query by Assignee UUID
        /// </summary>
        [HttpGet("assignee/{assigneeUuid}")]
        public async Task<IActionResult> QueryByAssigneeUuid(
            [FromRoute] string assigneeUuid,
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
                return await OnQueryByAssigneeUuid(client, assigneeUuid, start, stop, skip, take, (SortOrder)sortOrder, indentOutput);
            }
            else
            {
                return StatusCode(500, "Entity Client Not Found");
            }
        }

        protected virtual Task<IActionResult> OnQueryByAssigneeUuid(
            ITrakHoundClient client,
            string assigneeUuid,
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
        /// Query by Assignee UUID
        /// </summary>
        [HttpPost("assignee")]
        public async Task<IActionResult> QueryByAssigneeUuid(
            [FromBody] IEnumerable<string> assigneeUuids,
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
                return await OnQueryByAssigneeUuid(client, assigneeUuids, start, stop, skip, take, (SortOrder)sortOrder, indentOutput);
            }
            else
            {
                return StatusCode(500, "Entity Client Not Found");
            }
        }

        protected virtual Task<IActionResult> OnQueryByAssigneeUuid(
            ITrakHoundClient client,
            IEnumerable<string> assigneeUuids,
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
        /// Query by Member
        /// </summary>
        [HttpGet("member")]
        public async Task<IActionResult> QueryByMember(
            [FromQuery] string memberPath,
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
                return await OnQueryByMember(client, memberPath, start, stop, skip, take, (SortOrder)sortOrder, indentOutput);
            }
            else
            {
                return StatusCode(500, "Entity Client Not Found");
            }
        }

        protected virtual Task<IActionResult> OnQueryByMember(
            ITrakHoundClient client,
            string memberPath,
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
        /// Query by Member
        /// </summary>
        [HttpPost("member/path")]
        public async Task<IActionResult> QueryByMember(
            [FromBody] IEnumerable<string> memberPaths,
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
                return await OnQueryByMember(client, memberPaths, start, stop, skip, take, (SortOrder)sortOrder, indentOutput);
            }
            else
            {
                return StatusCode(500, "Entity Client Not Found");
            }
        }

        protected virtual Task<IActionResult> OnQueryByMember(
            ITrakHoundClient client,
            IEnumerable<string> memberPaths,
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
        /// Query by Member UUID
        /// </summary>
        [HttpGet("member/{memberUuid}")]
        public async Task<IActionResult> QueryByMemberUuid(
            [FromRoute] string memberUuid,
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
                return await OnQueryByMemberUuid(client, memberUuid, start, stop, skip, take, (SortOrder)sortOrder, indentOutput);
            }
            else
            {
                return StatusCode(500, "Entity Client Not Found");
            }
        }

        protected virtual Task<IActionResult> OnQueryByMemberUuid(
            ITrakHoundClient client,
            string memberUuid,
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
        /// Query by Member UUID
        /// </summary>
        [HttpPost("member")]
        public async Task<IActionResult> QueryByMemberUuid(
            [FromBody] IEnumerable<string> memberUuids,
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
                return await OnQueryByMemberUuid(client, memberUuids, start, stop, skip, take, (SortOrder)sortOrder, indentOutput);
            }
            else
            {
                return StatusCode(500, "Entity Client Not Found");
            }
        }

        protected virtual Task<IActionResult> OnQueryByMemberUuid(
            ITrakHoundClient client,
            IEnumerable<string> memberUuids,
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
        /// Subscribe by Assignee
        /// </summary>
        [Route("assignee/path/subscribe")]
        public async Task SubscribeByAssignee(

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
                await OnSubscribeByAssignee(client, interval, take, consumerId, indentOutput);
            }
            else
            {
                //return StatusCode(500, "Entity Client Not Found");
            }
        }

        protected virtual Task OnSubscribeByAssignee(
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
        /// Subscribe by Assignee UUID
        /// </summary>
        [Route("assignee/subscribe")]
        public async Task SubscribeByAssigneeUuid(

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
                await OnSubscribeByAssigneeUuid(client, interval, take, consumerId, indentOutput);
            }
            else
            {
                //return StatusCode(500, "Entity Client Not Found");
            }
        }

        protected virtual Task OnSubscribeByAssigneeUuid(
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
        /// Delete by Assignee UUID
        /// </summary>
        [HttpDelete("assignee/{assigneeUuid}")]
        public async Task<IActionResult> DeleteByAssigneeUuid(
            [FromRoute] string assigneeUuid,
            [FromQuery] string routerId = null,
            [FromQuery] bool indentOutput = false
            )
        {
            var client = GetClient(routerId);
            if (client != null)
            {
                return await OnDeleteByAssigneeUuid(client, assigneeUuid, indentOutput);
            }
            else
            {
                return StatusCode(500, "Entity Client Not Found");
            }
        }

        protected virtual Task<IActionResult> OnDeleteByAssigneeUuid(
            ITrakHoundClient client,
            string assigneeUuid,
            bool indentOutput = false
        )
        {
            return Task.FromResult<IActionResult>(NotFound());
        }


        /// <summary>
        /// Delete by Assignee UUID
        /// </summary>
        [HttpPost("delete/assignee")]
        public async Task<IActionResult> DeleteByAssigneeUuid(
            [FromBody] IEnumerable<string> assigneeUuids,
            [FromQuery] string routerId = null,
            [FromQuery] bool indentOutput = false
            )
        {
            var client = GetClient(routerId);
            if (client != null)
            {
                return await OnDeleteByAssigneeUuid(client, assigneeUuids, indentOutput);
            }
            else
            {
                return StatusCode(500, "Entity Client Not Found");
            }
        }

        protected virtual Task<IActionResult> OnDeleteByAssigneeUuid(
            ITrakHoundClient client,
            IEnumerable<string> assigneeUuids,
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
