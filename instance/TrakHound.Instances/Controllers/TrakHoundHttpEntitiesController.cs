// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TrakHound.Entities.Collections;
using TrakHound.Entities.QueryEngines;
using TrakHound.Instances;

namespace TrakHound.Http
{
    [ApiController]
    [Route(HttpConstants.EntitiesPrefix)]
    [Produces("application/json")]
    public class TrakHoundHttpEntitiesController : ControllerBase
    {
        private readonly ITrakHoundInstance _server;
        private readonly TrakHoundHttpWebSocketManager _webSocketManager;


        public ITrakHoundInstance Server => _server;

        public TrakHoundHttpWebSocketManager WebSocketManager => _webSocketManager;


        public TrakHoundHttpEntitiesController(ITrakHoundInstance server, TrakHoundHttpWebSocketManager webSocketManager)
        {
            _server = server;
            _webSocketManager = webSocketManager;
        }


        [HttpGet]
        public async Task<IActionResult> Query([FromQuery(Name = "q")] string query, [FromQuery] string routerId = null)
        {
            if (!string.IsNullOrEmpty(query))
            {
                var client = _server.ClientProvider.GetClient();
                if (client != null)
                {
                    client.RouterId = routerId;

                    var queryEngine = new TrakHoundQueryEngine(client.System.Entities);
                    var results = await queryEngine.ExecuteQuery(query);
                    if (!results.IsNullOrEmpty())
                    {
                        var response = new TrakHoundQueryResponse(results, true);
                        return Ok(new TrakHoundQueryJsonResponse(response));
                    }
                    else
                    {
                        return NotFound();
                    }
                }
            }

            return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> Query([FromQuery] string routerId = null)
        {
            var query = await this.GetBodyString();
            if (!string.IsNullOrEmpty(query))
            {
                var client = _server.ClientProvider.GetClient();
                if (client != null)
                {
                    client.RouterId = routerId;

                    var queryEngine = new TrakHoundQueryEngine(client.System.Entities);
                    var results = await queryEngine.ExecuteQuery(query);
                    if (!results.IsNullOrEmpty())
                    {
                        var response = new TrakHoundQueryResponse(results, true);
                        return Ok(new TrakHoundQueryJsonResponse(response));
                    }
                    else
                    {
                        return NotFound();
                    }
                }
            }

            return BadRequest();
        }


        [Route("subscribe")]
        public async Task Subscribe(
            [FromQuery] int interval = 100,
            [FromQuery] int count = 1000,
            [FromQuery] string routerId = null,
            [FromQuery] bool indentOutput = false
            )
        {
            var client = _server.ClientProvider.GetClient();
            if (client != null)
            {
                client.RouterId = routerId;

                var getConsumer = async () =>
                {
                    return await client.System.Entities.Subscribe(interval, count);
                };

                var formatResponse = (TrakHoundEntityCollection collection) =>
                {
                    return new TrakHoundJsonEntityCollection(collection).ToJson(indentOutput).ToUtf8Bytes();
                };

                await _webSocketManager.Create<TrakHoundEntityCollection>(HttpContext, getConsumer, formatResponse);
            }
        }

        [Route("subscribe/query")]
        public async Task Subscribe(
            [FromQuery(Name = "q")] string query,
            [FromQuery] int interval = 100,
            [FromQuery] int count = 1000,
            [FromQuery] string routerId = null,
            [FromQuery] bool indentOutput = false
            )
        {
            if (!string.IsNullOrEmpty(query))
            {
                var client = _server.ClientProvider.GetClient();
                if (client != null)
                {
                    client.RouterId = routerId;

                    var getConsumer = async () =>
                    {
                        return await client.System.Entities.Subscribe(query, interval, count);
                    };

                    var formatResponse = (TrakHoundQueryResponse queryResponse) =>
                    {
                        return new TrakHoundQueryJsonResponse(queryResponse).ToJson(indentOutput).ToUtf8Bytes();
                    };

                    await _webSocketManager.Create<TrakHoundQueryResponse>(HttpContext, getConsumer, formatResponse);
                }
            }
            else
            {
                var client = _server.ClientProvider.GetClient();
                if (client != null)
                {
                    client.RouterId = routerId;

                    var getConsumer = async (Stream requestBody) =>
                    {
                        return await client.System.Entities.Subscribe(TrakHoundHttpController.GetRequestBodyString(requestBody), interval, count);
                    };

                    var formatResponse = (TrakHoundQueryResponse queryResponse) =>
                    {
                        return new TrakHoundQueryJsonResponse(queryResponse).ToJson(indentOutput).ToUtf8Bytes();
                    };

                    await _webSocketManager.Create<TrakHoundQueryResponse>(HttpContext, getConsumer, formatResponse);
                }
            }
        }

        [Route("subscribe/requests")]
        public async Task SubscribeRequests(
            [FromQuery] int interval = 100,
            [FromQuery] int count = 1000,
            [FromQuery] string routerId = null,
            [FromQuery] bool indentOutput = false
            )
        {
            var client = _server.ClientProvider.GetClient();
            if (client != null)
            {
                client.RouterId = routerId;

                // Get Consumer
                var getConsumer = async (Stream requestBody) =>
                {
                    var httpRequests = TrakHoundHttpController.GetRequestBodyJson<IEnumerable<TrakHoundHttpEntitySubscriptionRequest>>(requestBody);
                    var requests = TrakHoundHttpEntitySubscriptionRequest.ToSubscriptionRequests(httpRequests);
                    return await client.System.Entities.Subscribe(requests, interval, count);
                };

                var formatResponse = (TrakHoundEntityCollection collection) =>
                {
                    return new TrakHoundJsonEntityCollection(collection).ToJson(indentOutput).ToUtf8Bytes();
                };

                await _webSocketManager.Create<TrakHoundEntityCollection>(HttpContext, getConsumer, formatResponse);
            }
        }

        [HttpPost("publish")]
        public async Task<IActionResult> Publish(
            [FromBody] TrakHoundJsonEntityCollection entityCollection,
            [FromQuery] bool async = true,
            [FromQuery] string routerId = null
            )
        {
            if (entityCollection != null)
            {
                var client = _server.ClientProvider.GetClient();
                if (client != null)
                {
                    client.RouterId = routerId;

                    var entities = entityCollection.GetEntities();
                    if (!entities.IsNullOrEmpty())
                    {
                        var operationMode = async ? TrakHoundOperationMode.Async : TrakHoundOperationMode.Sync;

                        if (await client.System.Entities.Publish(entities, operationMode))
                        {
                            if (operationMode == TrakHoundOperationMode.Async)
                            {
                                return Accepted();
                            }
                            else
                            {
                                return Created();
                            }
                        }
                        else
                        {
                            return StatusCode(500, "Entity Failed to Publish");
                        }
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
                else
                {
                    return StatusCode(500, "Entity Client Not Found");
                }
            }

            return BadRequest();
        }

        [Route("publish")]
        public async Task PublishStream(
            [FromQuery] string routerId = null,
            [FromQuery] string consumerId = null
            )
        {
            if (Request.HttpContext.WebSockets.IsWebSocketRequest)
            {
                var client = _server.ClientProvider.GetClient();
                if (client != null)
                {
                    var consumer = new TrakHoundConsumer<TrakHoundEntityCollection>(consumerId);
                    consumer.Received += async (o, collection) =>
                    {
                        var entities = collection.GetEntities();
                        if (!entities.IsNullOrEmpty())
                        {
                            await client.System.Entities.Publish(entities);
                        }
                    };

                    var formatResponse = (byte[] responseBytes) =>
                    {
                        var json = StringFunctions.GetUtf8String(responseBytes);
                        if (!string.IsNullOrEmpty(json))
                        {
                            var jsonCollection = Json.Convert<TrakHoundJsonEntityCollection>(json);
                            if (jsonCollection != null)
                            {
                                return jsonCollection.ToCollection();
                            }
                        }
                        return null;
                    };

                    await _webSocketManager.CreateClient<TrakHoundEntityCollection>(HttpContext, consumer, formatResponse);
                }
            }
        }
    }
}
