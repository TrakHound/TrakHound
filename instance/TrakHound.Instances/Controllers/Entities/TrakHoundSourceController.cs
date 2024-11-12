// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrakHound.Entities;

namespace TrakHound.Http.Entities
{
    public partial class TrakHoundSourceController
    {
        [HttpGet("{uuid}/chain")]
        public async Task<IActionResult> QueryChain(
            [FromRoute] string uuid,
            [FromQuery] string routerId = null,
            [FromQuery] bool indentOutput = false
            )
        {
            var client = GetClient(routerId);
            if (client != null)
            {
                var entities = await client.System.Entities.Sources.QueryChain(uuid, routerId);
                if (!entities.IsNullOrEmpty())
                {
                    var i = 0;
                    var a = new object[entities.Count()];
                    foreach (var entity in entities)
                    {
                        a[i] = TrakHoundEntity.ToArray(entity);
                        i++;
                    }

                    return ProcessJsonContentResponse(a.ToJson(indentOutput));
                }
                else
                {
                    return NotFound();
                }
            }

            return BadRequest();
        }

        [HttpPost("chain")]
        public async Task<IActionResult> QueryChain(
            [FromBody] IEnumerable<string> uuids,
            [FromQuery] string routerId = null,
            [FromQuery] bool indentOutput = false
            )
        {
            var client = GetClient(routerId);
            if (client != null)
            {
                var entities = await client.System.Entities.Sources.QueryChain(uuids, routerId);
                if (!entities.IsNullOrEmpty())
                {
                    var i = 0;
                    var a = new object[entities.Count()];
                    foreach (var entity in entities)
                    {
                        a[i] = TrakHoundEntity.ToArray(entity);
                        i++;
                    }

                    return ProcessJsonContentResponse(a.ToJson(indentOutput));
                }
                else
                {
                    return NotFound();
                }
            }

            return BadRequest();
        }


        [HttpGet("{uuid}/chain/uuid")]
        public async Task<IActionResult> QueryUuidChain(
            [FromRoute] string uuid,
            [FromQuery] string routerId = null,
            [FromQuery] bool indentOutput = false
            )
        {
            var client = GetClient(routerId);
            if (client != null)
            {
                var queryResults = await client.System.Entities.Sources.QueryUuidChain(uuid, routerId);
                if (!queryResults.IsNullOrEmpty())
                {
                    var httpResults = new List<TrakHoundHttpObjectQueryResult>();
                    foreach (var queryResult in queryResults)
                    {
                        httpResults.Add(new TrakHoundHttpObjectQueryResult(queryResult.Query, queryResult.Uuid));
                    }

                    return ProcessJsonContentResponse(httpResults.ToJson());
                }
                else
                {
                    return NotFound();
                }
            }

            return BadRequest();
        }

        [HttpPost("chain/uuid")]
        public async Task<IActionResult> QueryUuidChain(
            [FromBody] IEnumerable<string> uuids,
            [FromQuery] string routerId = null,
            [FromQuery] bool indentOutput = false
            )
        {
            var client = GetClient(routerId);
            if (client != null)
            {
                var queryResults = await client.System.Entities.Sources.QueryUuidChain(uuids, routerId);
                if (!queryResults.IsNullOrEmpty())
                {
                    var httpResults = new List<TrakHoundHttpObjectQueryResult>();
                    foreach (var queryResult in queryResults)
                    {
                        httpResults.Add(new TrakHoundHttpObjectQueryResult(queryResult.Query, queryResult.Uuid));
                    }

                    return ProcessJsonContentResponse(httpResults.ToJson());
                }
                else
                {
                    return NotFound();
                }
            }

            return BadRequest();
        }
    }
}
