// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrakHound.Entities;

namespace TrakHound.Http.Entities
{
    public partial class TrakHoundDefinitionController
    {
        [HttpGet]
        public async Task<IActionResult> Read(
            [FromQuery(Name = "uuids")] string entityUuidsJson,
            [FromQuery(Name = "pattern")] string pattern,
            [FromQuery(Name = "types")] string typesJson,
            [FromQuery] long skip = 0,
            [FromQuery] long take = 1000,
            [FromQuery(Name = "order")] SortOrder sortOrder = SortOrder.Ascending,
            [FromQuery] string routerId = null,
            [FromQuery] bool indentOutput = false
            )
        {
            IEnumerable<ITrakHoundDefinitionEntity> entities = null;
            bool isValid = false;

            var client = GetClient(routerId);
            if (client != null)
            {
                if (!string.IsNullOrEmpty(pattern))
                {
                    // Read by Definition Pattern
                    entities = await client.System.Entities.Definitions.Query(pattern, skip, take, sortOrder, routerId);
                    isValid = true;
                }
                else if (!string.IsNullOrEmpty(typesJson))
                {
                    // Read by Definition Type(s)
                    var types = Json.Convert<IEnumerable<string>>(typesJson);
                    if (!types.IsNullOrEmpty())
                    {
                        entities = await client.System.Entities.Definitions.QueryByType(types, skip, take, sortOrder, routerId);
                        isValid = true;
                    }
                }
                else if (!string.IsNullOrEmpty(entityUuidsJson))
                {
                    // Read by Definition UUID
                    var uuids = Json.Convert<IEnumerable<string>>(entityUuidsJson);
                    if (!uuids.IsNullOrEmpty())
                    {
                        entities = await client.System.Entities.Definitions.ReadByUuid(uuids, routerId);
                        isValid = true;
                    }
                }
            }

            if (isValid)
            {
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

        [HttpPost("child")]
        public async Task<IActionResult> QueryByChildUuid(
            [FromBody] IEnumerable<string> childUuids,
            [FromQuery] string routerId = null,
            [FromQuery] bool indentOutput = false
            )
        {
            if (!childUuids.IsNullOrEmpty())
            {
                var client = GetClient(routerId);
                if (client != null)
                {
                    var entities = await client.System.Entities.Definitions.QueryByChildUuid(childUuids, routerId: routerId);
                    if (!entities.IsNullOrEmpty())
                    {
                        return ProcessJsonContentResponse(entities, indentOutput);
                    }
                    else
                    {
                        return NotFound();
                    }
                }
            }

            return BadRequest();
        }

        [HttpPost("type")]
        public async Task<IActionResult> QueryByType(
            [FromBody] IEnumerable<string> types,
            [FromQuery] long skip = 0,
            [FromQuery] long take = 1000,
            [FromQuery(Name = "order")] SortOrder sortOrder = SortOrder.Ascending,
            [FromQuery] string routerId = null,
            [FromQuery] bool indentOutput = false
            )
        {
            var client = GetClient(routerId);
            if (client != null)
            {
                var entities = await client.System.Entities.Definitions.QueryByType(types, skip, take, sortOrder, routerId);
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

        [HttpPost("ids/type")]
        public async Task<IActionResult> QueryIdsByType(
            [FromBody] IEnumerable<string> types,
            [FromQuery] long skip = 0,
            [FromQuery] long take = 1000,
            [FromQuery(Name = "order")] SortOrder sortOrder = SortOrder.Ascending,
            [FromQuery] string routerId = null
            )
        {
            var client = GetClient(routerId);
            if (client != null)
            {
                var queryResults = await client.System.Entities.Definitions.QueryIdsByType(types, skip, take, sortOrder, routerId);
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
