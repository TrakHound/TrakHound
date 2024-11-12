// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TrakHound.Http.Entities
{
    public partial class TrakHoundObjectAssignmentController
    {
        /// <summary>
        /// 
        /// </summary>
        [HttpPost("assignee/range")]
        public async Task<IActionResult> QueryByAssigneeRange(
            [FromBody] IEnumerable<TrakHoundHttpRangeQueryRequest> httpQueries,
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
                var queries = httpQueries.ToRangeQueries();

                var entities = await client.System.Entities.Objects.Assignment.QueryByAssigneeRange(queries, skip, take, (SortOrder)sortOrder);
                if (!entities.IsNullOrEmpty())
                {
                    return ProcessJsonContentResponse(entities, indentOutput);
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                return StatusCode(500, "Entity Client Not Found");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [HttpPost("member/range")]
        public async Task<IActionResult> QueryByMemberRange(
            [FromBody] IEnumerable<TrakHoundHttpRangeQueryRequest> httpQueries,
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
                var queries = httpQueries.ToRangeQueries();

                var entities = await client.System.Entities.Objects.Assignment.QueryByMemberRange(queries, skip, take, (SortOrder)sortOrder);
                if (!entities.IsNullOrEmpty())
                {
                    return ProcessJsonContentResponse(entities, indentOutput);
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                return StatusCode(500, "Entity Client Not Found");
            }
        }
    }
}
