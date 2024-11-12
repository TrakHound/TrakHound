// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;
using TrakHound.Controllers.Http;
using TrakHound.Entities;

namespace TrakHound.Http.Entities
{
    public partial class TrakHoundObjectController : TrakHoundObjectControllerBase
    {

        #region "Query"

        [HttpGet]
        [RouteDescription("Read a List of Object Entities based on the query parameters")]
        [RouteParameterDescription("path", "An Object Path/Expression to return the results of")]
        [RouteParameterDescription("skip", "The number of items to skip")]
        [RouteParameterDescription("take", "The number of items to return")]
        [RouteParameterDescription("order", "The order of the items to return (Asc = 1, Desc = -1")]
        [RouteParameterDescription("routerId", "The ID or Name of the Router to use to satisfy the request")]
        [RouteResponse(404)]
        [RouteResponse(400)]
        public async Task<IActionResult> Query(
            [FromQuery] string path,
            [FromQuery] long skip = 0,
            [FromQuery] long take = 1000,
            [FromQuery] int sortOrder = (int)SortOrder.Ascending,
            [FromQuery] string routerId = null,
            [FromQuery] bool indentOutput = false
            )
        {
            var client = GetClient(routerId);
            if (client != null)
            {
                if (!string.IsNullOrEmpty(path))
                {
                    // Read by Object Query
                    var entities = await client.System.Entities.Objects.Query(path, skip, take, (SortOrder)sortOrder, routerId);
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

        [HttpGet("ids")]
        public async Task<IActionResult> QueryIds(
            [FromQuery] string path,
            [FromQuery] long skip = 0,
            [FromQuery] long take = 1000,
            [FromQuery] int sortOrder = (int)SortOrder.Ascending,
            [FromQuery] string routerId = null,
            [FromQuery] bool indentOutput = false
            )
        {
            if (!string.IsNullOrEmpty(path))
            {
                var client = GetClient(routerId);
                if (client != null)
                {
                    var queryResults = await client.System.Entities.Objects.QueryUuids(path, skip, take, (SortOrder)sortOrder, routerId);
                    if (!queryResults.IsNullOrEmpty())
                    {
                        var httpResults = new List<TrakHoundHttpObjectQueryResult>();
                        foreach (var queryResult in queryResults)
                        {
                            httpResults.Add(new TrakHoundHttpObjectQueryResult(queryResult.Query, queryResult.Uuid));
                        }

                        return ProcessJsonContentResponse(httpResults.ToJson(indentOutput));
                    }
                    else
                    {
                        return NotFound();
                    }
                }
            }

            return BadRequest();
        }

        [HttpPost("path")]
        public async Task<IActionResult> QueryByPath(
            [FromBody] IEnumerable<string> paths,
            [FromQuery] string routerId = null,
            [FromQuery] bool indentOutput = false
            )
        {
            if (!paths.IsNullOrEmpty())
            {
                var client = GetClient(routerId);
                if (client != null)
                {
                    var entities = await client.System.Entities.Objects.Query(paths, routerId: routerId);
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

        [HttpPost("ids/path")]
        public async Task<IActionResult> QueryIdsByPath(
            [FromBody] IEnumerable<string> paths,
            [FromQuery] string routerId = null,
            [FromQuery] bool indentOutput = false
            )
        {
            if (!paths.IsNullOrEmpty())
            {
                var client = GetClient(routerId);
                if (client != null)
                {
                    var queryResults = await client.System.Entities.Objects.QueryUuids(paths, routerId: routerId);
                    if (!queryResults.IsNullOrEmpty())
                    {
                        var httpResults = new List<TrakHoundHttpObjectQueryResult>();
                        foreach (var queryResult in queryResults)
                        {
                            httpResults.Add(new TrakHoundHttpObjectQueryResult(queryResult.Query, queryResult.Uuid));
                        }

                        return ProcessJsonContentResponse(httpResults.ToJson(indentOutput));
                    }
                    else
                    {
                        return NotFound();
                    }
                }
            }

            return BadRequest();
        }

        [HttpGet("root")]
        public async Task<IActionResult> QueryRoot(
            [FromQuery] long skip = 0,
            [FromQuery] long take = 1000,
            [FromQuery] int sortOrder = (int)SortOrder.Ascending,
            [FromQuery] string routerId = null,
            [FromQuery] bool indentOutput = false
            )
        {
            var client = GetClient(routerId);
            if (client != null)
            {
                var entities = await client.System.Entities.Objects.QueryRoot(skip, take, (SortOrder)sortOrder, routerId: routerId);
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
                return NotFound();
            }
        }

        [HttpPost("root")]
        public async Task<IActionResult> QueryRoot(
            [FromBody] IEnumerable<string> namespaces,
            [FromQuery] long skip = 0,
            [FromQuery] long take = 1000,
            [FromQuery] int sortOrder = (int)SortOrder.Ascending,
            [FromQuery] string routerId = null,
            [FromQuery] bool indentOutput = false
            )
        {
            if (!namespaces.IsNullOrEmpty())
            {
                var client = GetClient(routerId);
                if (client != null)
                {
                    var entities = await client.System.Entities.Objects.QueryRoot(namespaces, skip, take, (SortOrder)sortOrder, routerId: routerId);
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

        [HttpGet("root/ids")]
        public async Task<IActionResult> QueryRootUuids(
            [FromQuery] long skip = 0,
            [FromQuery] long take = 1000,
            [FromQuery] int sortOrder = (int)SortOrder.Ascending,
            [FromQuery] string routerId = null,
            [FromQuery] bool indentOutput = false
            )
        {
            var client = GetClient(routerId);
            if (client != null)
            {
                var results = await client.System.Entities.Objects.QueryRootUuids(skip, take, (SortOrder)sortOrder, routerId: routerId);
                if (!results.IsNullOrEmpty())
                {
                    var httpResults = new List<TrakHoundHttpObjectQueryResult>();
                    foreach (var result in results)
                    {
                        httpResults.Add(new TrakHoundHttpObjectQueryResult(result.Query, result.Uuid));
                    }

                    return ProcessJsonContentResponse(httpResults.ToJson(indentOutput));
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost("root/ids")]
        public async Task<IActionResult> QueryRootUuids(
            [FromBody] IEnumerable<string> namespaces,
            [FromQuery] long skip = 0,
            [FromQuery] long take = 1000,
            [FromQuery] int sortOrder = (int)SortOrder.Ascending,
            [FromQuery] string routerId = null,
            [FromQuery] bool indentOutput = false
            )
        {
            if (!namespaces.IsNullOrEmpty())
            {
                var client = GetClient(routerId);
                if (client != null)
                {
                    var results = await client.System.Entities.Objects.QueryRootUuids(namespaces, skip, take, (SortOrder)sortOrder, routerId: routerId);
                    if (!results.IsNullOrEmpty())
                    {
                        var httpResults = new List<TrakHoundHttpObjectQueryResult>();
                        foreach (var result in results)
                        {
                            httpResults.Add(new TrakHoundHttpObjectQueryResult(result.Query, result.Uuid));
                        }

                        return ProcessJsonContentResponse(httpResults.ToJson(indentOutput));
                    }
                    else
                    {
                        return NotFound();
                    }
                }
            }

            return BadRequest();
        }

        [HttpPost("parent")]
        public async Task<IActionResult> QueryByParentUuid(
            [FromBody] IEnumerable<string> parentUuids,
            [FromQuery] string routerId = null,
            [FromQuery] bool indentOutput = false
            )
        {
            if (!parentUuids.IsNullOrEmpty())
            {
                var client = GetClient(routerId);
                if (client != null)
                {
                    var entities = await client.System.Entities.Objects.QueryByParentUuid(parentUuids, routerId: routerId);
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
                    var entities = await client.System.Entities.Objects.QueryByChildUuid(childUuids, routerId: routerId);
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

        [HttpPost("root/children")]
        public async Task<IActionResult> QueryChildrenByRootUuid(
            [FromBody] IEnumerable<string> rootUuids,
            [FromQuery] string routerId = null,
            [FromQuery] bool indentOutput = false
            )
        {
            if (!rootUuids.IsNullOrEmpty())
            {
                var client = GetClient(routerId);
                if (client != null)
                {
                    var entities = await client.System.Entities.Objects.QueryChildrenByRootUuid(rootUuids, routerId: routerId);
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

        [HttpPost("child/root")]
        public async Task<IActionResult> QueryRootByChildUuid(
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
                    var entities = await client.System.Entities.Objects.QueryRootByChildUuid(childUuids, routerId: routerId);
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


        [HttpPost("ids/parent")]
        public async Task<IActionResult> QueryIdsByParentUuid(
            [FromBody] IEnumerable<string> parentUuids,
            [FromQuery] string routerId = null,
            [FromQuery] bool indentOutput = false
            )
        {
            if (!parentUuids.IsNullOrEmpty())
            {
                var client = GetClient(routerId);
                if (client != null)
                {
                    var results = await client.System.Entities.Objects.QueryUuidsByParentUuid(parentUuids, routerId: routerId);
                    if (!results.IsNullOrEmpty())
                    {
                        var httpResults = new List<TrakHoundHttpObjectQueryResult>();
                        foreach (var result in results)
                        {
                            httpResults.Add(new TrakHoundHttpObjectQueryResult(result.Query, result.Uuid));
                        }

                        return ProcessJsonContentResponse(httpResults.ToJson(indentOutput));
                    }
                    else
                    {
                        return NotFound();
                    }
                }
            }

            return BadRequest();
        }

        [HttpPost("ids/child")]
        public async Task<IActionResult> QueryIdsByChildUuid(
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
                    var results = await client.System.Entities.Objects.QueryUuidsByChildUuid(childUuids, routerId: routerId);
                    if (!results.IsNullOrEmpty())
                    {
                        var httpResults = new List<TrakHoundHttpObjectQueryResult>();
                        foreach (var result in results)
                        {
                            httpResults.Add(new TrakHoundHttpObjectQueryResult(result.Query, result.Uuid));
                        }

                        return ProcessJsonContentResponse(httpResults.ToJson(indentOutput));
                    }
                    else
                    {
                        return NotFound();
                    }
                }
            }

            return BadRequest();
        }

        [HttpPost("ids/root/children")]
        public async Task<IActionResult> QueryChildIdsByRootUuid(
            [FromBody] IEnumerable<string> rootUuids,
            [FromQuery] string routerId = null,
            [FromQuery] bool indentOutput = false
            )
        {
            if (!rootUuids.IsNullOrEmpty())
            {
                var client = GetClient(routerId);
                if (client != null)
                {
                    var results = await client.System.Entities.Objects.QueryChildUuidsByRootUuid(rootUuids, routerId: routerId);
                    if (!results.IsNullOrEmpty())
                    {
                        var httpResults = new List<TrakHoundHttpObjectQueryResult>();
                        foreach (var result in results)
                        {
                            httpResults.Add(new TrakHoundHttpObjectQueryResult(result.Query, result.Uuid));
                        }

                        return ProcessJsonContentResponse(httpResults.ToJson(indentOutput));
                    }
                    else
                    {
                        return NotFound();
                    }
                }
            }

            return BadRequest();
        }

        [HttpPost("ids/child/root")]
        public async Task<IActionResult> QueryRootIdsByChildUuid(
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
                    var results = await client.System.Entities.Objects.QueryRootUuidsByChildUuid(childUuids, routerId: routerId);
                    if (!results.IsNullOrEmpty())
                    {
                        var httpResults = new List<TrakHoundHttpObjectQueryResult>();
                        foreach (var result in results)
                        {
                            httpResults.Add(new TrakHoundHttpObjectQueryResult(result.Query, result.Uuid));
                        }

                        return ProcessJsonContentResponse(httpResults.ToJson(indentOutput));
                    }
                    else
                    {
                        return NotFound();
                    }
                }
            }

            return BadRequest();
        }



        [HttpPost("query")]
        public async Task<IActionResult> QueryByRequest(
            [FromBody] TrakHoundHttpObjectQueryRequest queryRequest,
            [FromQuery] string routerId = null,
            [FromQuery] bool indentOutput = false
            )
        {
            if (queryRequest != null && !queryRequest.Queries.IsNullOrEmpty())
            {
                var client = GetClient(routerId);
                if (client != null)
                {
                    var entities = await client.System.Entities.Objects.Query(queryRequest.ToRequest(), routerId: routerId);
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

        [HttpPost("ids/query")]
        public async Task<IActionResult> QueryIds(
            [FromBody] TrakHoundHttpObjectQueryRequest queryRequest,
            [FromQuery] string routerId = null,
            [FromQuery] bool indentOutput = false
            )
        {
            if (queryRequest != null && !queryRequest.Queries.IsNullOrEmpty())
            {
                var client = GetClient(routerId);
                if (client != null)
                {
                    var results = await client.System.Entities.Objects.QueryUuids(queryRequest.ToRequest(), routerId: routerId);
                    if (!results.IsNullOrEmpty())
                    {
                        var httpResults = new List<TrakHoundHttpObjectQueryResult>();
                        foreach (var result in results)
                        {
                            httpResults.Add(new TrakHoundHttpObjectQueryResult(result.Query, result.Uuid));
                        }

                        return ProcessJsonContentResponse(httpResults.ToJson(indentOutput));
                    }
                    else
                    {
                        return NotFound();
                    }
                }
            }

            return BadRequest();
        }

        #endregion

        #region "Namespaces"

        [HttpGet("namespaces")]
        public async Task<IActionResult> ListNamespaces(
            [FromQuery] long skip = 0,
            [FromQuery] long take = 1000,
            [FromQuery] int sortOrder = (int)SortOrder.Ascending,
            [FromQuery] string routerId = null
            )
        {
            var client = GetClient(routerId);
            if (client != null)
            {
                var results = await client.System.Entities.Objects.ListNamespaces(skip, take, (SortOrder)sortOrder, routerId);
                if (!results.IsNullOrEmpty())
                {
                    return Ok(results);
                }
                else
                {
                    return NotFound();
                }
            }

            return BadRequest();
        }

        #endregion

        #region "Count"

        [HttpPost("count/child")]
        public async Task<IActionResult> QueryChildCount(
            [FromBody] IEnumerable<string> uuids,
            [FromQuery] string routerId = null,
            [FromQuery] bool indentOutput = false
            )
        {
            if (!uuids.IsNullOrEmpty())
            {
                var client = GetClient(routerId);
                if (client != null)
                {
                    var entities = await client.System.Entities.Objects.QueryChildCount(uuids, routerId: routerId);
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

        #endregion

        #region "Notify"

        [HttpGet("notify")]
        public async Task Notify(
            [FromQuery(Name = "path")] string path,
            [FromQuery] string type = null,
            [FromQuery] int interval = 0,
            [FromQuery] int take = 100,
            [FromQuery] string routerId = null,
            [FromQuery] string consumerId = null,
            [FromQuery] bool indentOutput = false
            )
        {
            if (!string.IsNullOrEmpty(path))
            {
                var getConsumer = async () =>
                {
                    var client = GetClient(routerId);
                    if (client != null)
                    {
                        var notificationType = type != null ? type.ConvertEnum<TrakHoundEntityNotificationType>() : TrakHoundEntityNotificationType.All;

                        return await client.System.Entities.Objects.Notify(path, notificationType, routerId);
                    }
                    return null;
                };

                var formatResponse = (IEnumerable<TrakHoundEntityNotification> notifications) =>
                {
                    return notifications.ToJson(indentOutput).ToUtf8Bytes();
                };

                await WebSocketManager.Create<IEnumerable<TrakHoundEntityNotification>>(HttpContext, getConsumer, formatResponse);
            }
        }

        #endregion

        #region "Index"

        [HttpPost("index/request")]
        public async Task<IActionResult> QueryIndex(
            [FromBody] IEnumerable<TrakHoundHttpEntityIndexRequest> httpRequests,
            [FromQuery] long skip = 0,
            [FromQuery] long take = 1000,
            [FromQuery] int sortOrder = (int)SortOrder.Ascending,
            [FromQuery] string routerId = null
            )
        {
            var client = GetClient(routerId);
            if (client != null)
            {
                var requests = httpRequests.ToRequests();
                if (!requests.IsNullOrEmpty())
                {
                    var entities = await client.System.Entities.Objects.QueryIndex(requests, skip, take, (SortOrder)sortOrder, routerId);
                    if (!entities.IsNullOrEmpty())
                    {
                        return Ok(entities);
                    }
                    else
                    {
                        return NotFound();
                    }
                }
            }

            return BadRequest();
        }

        [HttpPost("index")]
        public async Task<IActionResult> UpdateIndex(
            [FromBody] IEnumerable<TrakHoundHttpEntityIndexPublishRequest> requests,
            [FromQuery] bool async = true,
            [FromQuery] string routerId = null
            )
        {
            if (!requests.IsNullOrEmpty())
            {
                var client = GetClient(routerId);
                if (client != null)
                {
                    var operationMode = async ? TrakHoundOperationMode.Async : TrakHoundOperationMode.Sync;

                    if (await client.System.Entities.Objects.UpdateIndex(requests.ToRequests(), operationMode, routerId))
                    {
                        return Ok();
                    }
                    else
                    {
                        return NotFound();
                    }
                }
            }

            return BadRequest();
        }

        #endregion

        #region "Delete"

        [HttpDelete("root/children/{rootUuid}")]
        public async Task<IActionResult> DeleteByRootUuid(
            [FromRoute] string rootUuid,
            [FromQuery] string routerId = null,
            [FromQuery] bool indentOutput = false
            )
        {
            if (!string.IsNullOrEmpty(rootUuid))
            {
                var client = GetClient(routerId);
                if (client != null)
                {
                    if (await client.System.Entities.Objects.DeleteByRootUuid(rootUuid, routerId: routerId))
                    {
                        return Ok();
                    }
                    else
                    {
                        return NotFound();
                    }
                }
            }

            return BadRequest();
        }

        [HttpPost("root/children/delete")]
        public async Task<IActionResult> DeleteByRootUuid(
            [FromBody] IEnumerable<string> rootUuids,
            [FromQuery] string routerId = null,
            [FromQuery] bool indentOutput = false
            )
        {
            if (!rootUuids.IsNullOrEmpty())
            {
                var client = GetClient(routerId);
                if (client != null)
                {
                    if (await client.System.Entities.Objects.DeleteByRootUuid(rootUuids, routerId: routerId))
                    {
                        return Ok();
                    }
                    else
                    {
                        return NotFound();
                    }
                }
            }

            return BadRequest();
        }

        #endregion

        #region "Expire"

        [HttpPost("expire/pattern")]
        public async Task<IActionResult> ExpireByPattern(
            [FromBody] IEnumerable<string> patterns,
            [FromQuery] long created,
            [FromQuery] string routerId = null,
            [FromQuery] bool indentOutput = false
            )
        {
            if (!patterns.IsNullOrEmpty())
            {
                var client = GetClient(routerId);
                if (client != null)
                {
                    var results = await client.System.Entities.Objects.Expire(patterns, created, routerId: routerId);
                    if (!results.IsNullOrEmpty())
                    {
                        return Ok(TrakHoundHttpDeleteResult.Create(results));
                    }
                    else
                    {
                        return NotFound();
                    }
                }
            }

            return BadRequest();
        }

        [HttpPost("expire/update/pattern")]
        public async Task<IActionResult> ExpireByUpdateByPattern(
            [FromBody] IEnumerable<string> patterns,
            [FromQuery] long lastUpdated,
            [FromQuery] string routerId = null,
            [FromQuery] bool indentOutput = false
            )
        {
            if (!patterns.IsNullOrEmpty())
            {
                var client = GetClient(routerId);
                if (client != null)
                {
                    var results = await client.System.Entities.Objects.ExpireByUpdate(patterns, lastUpdated, routerId: routerId);
                    if (!results.IsNullOrEmpty())
                    {
                        return Ok(TrakHoundHttpDeleteResult.Create(results));
                    }
                    else
                    {
                        return NotFound();
                    }
                }
            }

            return BadRequest();
        }

        [HttpPost("expire/acccess/pattern")]
        public async Task<IActionResult> ExpireByAccessByPattern(
            [FromBody] IEnumerable<string> patterns,
            [FromQuery] long lastAccessed,
            [FromQuery] string routerId = null,
            [FromQuery] bool indentOutput = false
            )
        {
            if (!patterns.IsNullOrEmpty())
            {
                var client = GetClient(routerId);
                if (client != null)
                {
                    var results = await client.System.Entities.Objects.ExpireByAccess(patterns, lastAccessed, routerId: routerId);
                    if (!results.IsNullOrEmpty())
                    {
                        return Ok(TrakHoundHttpDeleteResult.Create(results));
                    }
                    else
                    {
                        return NotFound();
                    }
                }
            }

            return BadRequest();
        }

        #endregion

    }
}
