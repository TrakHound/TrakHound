// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using TrakHound.Clients;
using TrakHound.Controllers.Http;
using TrakHound.Entities;
using TrakHound.Instances;
using TrakHound.Requests;
using TrakHound.Security;

namespace TrakHound.Http
{
    [ApiController]
    [Produces("application/json")]
    public abstract class TrakHoundHttpEntityControllerBase<TEntity> : TrakHoundHttpController, IDisposable where TEntity : ITrakHoundEntity
    {
        private readonly ITrakHoundInstance _instance;
        private readonly TrakHoundHttpWebSocketManager _webSocketManager;
        protected CancellationTokenSource _stop;


        public ITrakHoundInstance Instance => _instance;

        public TrakHoundHttpWebSocketManager WebSocketManager => _webSocketManager;


        public TrakHoundHttpEntityControllerBase(ITrakHoundInstance instance, TrakHoundHttpWebSocketManager webSocketManager)
        {
            _stop = new CancellationTokenSource();

            _instance = instance;
            _instance.Stopping += InstanceStopping;

            _webSocketManager = webSocketManager;
        }

        public void Dispose()
        {

        }


        private void InstanceStopping(object sender, EventArgs e)
        {
            if (_stop != null) _stop.Cancel();
        }


        protected ITrakHoundClient GetClient(string routerId)
        {
            var client =  _instance.ClientProvider.GetClient();
            if (client != null)
            {
                client.RouterId = routerId;
                client.AddMiddleware(new TrakHoundSourceMiddleware(CreateSource()));
                return client;
            }

            return null;
        }

        protected ITrakHoundEntityClient<TEntity> GetEntityClient(string routerId)
        {
            var client = GetClient(routerId);
            if (client != null)
            {
                return client.System.Entities.GetEntityClient<TEntity>();
            }

            return null;
        }


        protected bool IsNullOrEmpty(object obj)
        {
            if (obj == null) return true;

            return false;
        }

        protected bool IsNullOrEmpty<T>(IEnumerable<T> obj)
        {
            return obj.IsNullOrEmpty();
        }

        private TrakHoundSourceEntry CreateSource()
        {
            var author = new TrakHoundSourceEntry();

            var paths = new List<string>();

            var applicationAuthor = TrakHoundSourceEntry.CreateApplicationSource();
            if (applicationAuthor != null)
            {
                paths.Add(applicationAuthor.Path);
            }

            var entityCategory = TrakHoundEntity.GetEntityCategory<TEntity>();
            var entityClass = TrakHoundEntity.GetEntityClass<TEntity>();

            paths.Add("Entities API Controller");
            paths.Add($"{entityCategory}.{entityClass} API Controller");

            author.Path = string.Join('/', paths);
            return author;
        }


        /// <summary>
        /// Read By Uuid
        /// </summary>
        [HttpGet("{uuid}")]
        [TrakHoundPermission("read")]
        public async Task<IActionResult> ReadByUuid(
            [FromRoute] string uuid,
            [FromQuery] string routerId = null,
            [FromQuery] bool indentOutput = false
            )
        {
            if (!string.IsNullOrEmpty(uuid))
            {
                var entityClient = GetEntityClient(routerId);
                if (entityClient != null)
                {
                    var decodedUuid = HttpUtility.UrlDecode(uuid);

                    var entity = await entityClient.ReadByUuid(decodedUuid);
                    if (entity != null)
                    {
                        return ProcessJsonContentResponse(entity, indentOutput);
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

            return BadRequest();
        }

        /// <summary>
        /// Read By Uuid
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> ReadByUuid(
            [FromBody] IEnumerable<string> uuids,
            [FromQuery] string routerId = null,
            [FromQuery] bool indentOutput = false
            )
        {
            if (!uuids.IsNullOrEmpty())
            {
                var entityClient = GetEntityClient(routerId);
                if (entityClient != null)
                {
                    var entities = await entityClient.ReadByUuid(uuids);
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

            return BadRequest();
        }


        [Route("subscribe")]
        public async Task BaseSubscribe(
            [FromQuery] int interval = 0,
            [FromQuery] int take = 0,
            [FromQuery] string routerId = null,
            [FromQuery] string consumerId = null,
            [FromQuery] bool indentOutput = false
            )
        {
            var getConsumer = async () =>
            {
                var client = GetClient(routerId);
                if (client != null)
                {
                    return await client.System.Entities.GetEntityClient<TEntity>().Subscribe();
                }
                return null;
            };

            var formatResponse = (IEnumerable<TEntity> entities) =>
            {
                var content = new List<object[]>();
                foreach (var entity in entities.ToDistinct()) content.Add(TrakHoundEntity.ToArray(entity));
                return content.ToJson(indentOutput).ToUtf8Bytes();
            };

            await _webSocketManager.Create<IEnumerable<TEntity>>(HttpContext, getConsumer, formatResponse);
        }


        [HttpPost("publish")]
        [RouteDescription("Publish a list of Entities")]
        [RouteParameterDescription("async", "The Operation Mode to use for the request (sync = false, async = true)")]
        [RouteParameterDescription("routerId", "The ID or Name of the Router to use to satisfy the request")]
        public async Task<IActionResult> PublishEntities(
            [FromBody] IEnumerable<object[]> entityArrays,
            [FromQuery] bool async = true,
            [FromQuery] string routerId = null,
            [FromQuery] string ledgerId = null
            )
        {
            if (!entityArrays.IsNullOrEmpty())
            {
                var entityClient = GetEntityClient(routerId);
                if (entityClient != null)
                {
                    var entities = new List<TEntity>();
                    foreach (var entityArray in entityArrays)
                    {
                        var entity = TrakHoundEntity.FromArray<TEntity>(entityArray);
                        if (entity != null)
                        {
                            entities.Add(entity);
                        }
                        else
                        {
                            return BadRequest();
                        }
                    }

                    var operationMode = async ? TrakHoundOperationMode.Async : TrakHoundOperationMode.Sync;

                    if (await entityClient.Publish(entities, operationMode, ledgerId))
                    {
                        if (operationMode == TrakHoundOperationMode.Async)
                        {
                            return Accepted();
                        }
                        else
                        {
                            return Created("", entities);
                        }
                    }
                    else
                    {
                        return StatusCode(500, "Entity Failed to Publish");
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
                var client = GetClient(routerId);
                if (client != null)
                {
                    var consumer = new TrakHoundConsumer<IEnumerable<TEntity>>(consumerId);
                    consumer.Received += async (o, e) =>
                    {
                        await client.System.Entities.GetEntityClient<TEntity>().Publish(e);
                    };

                    var formatResponse = (byte[] responseBytes) =>
                    {
                        var json = StringFunctions.GetUtf8String(responseBytes);
                        if (!string.IsNullOrEmpty(json))
                        {
                            var jsonLines = json.Split("\r\n");
                            if (!jsonLines.IsNullOrEmpty())
                            {
                                var entities = new List<TEntity>();
                                foreach (var line in jsonLines)
                                {
                                    var entity = TrakHoundEntity.FromJson<TEntity>(line);
                                    if (entity != null)
                                    {
                                        entities.Add(entity);
                                    }
                                }
                                return entities;
                            }
                        }

                        return null;
                    };

                    await _webSocketManager.CreateClient<IEnumerable<TEntity>>(HttpContext, consumer, formatResponse);
                }
            }
        }


        [HttpDelete("{uuid}")]
        public async Task<IActionResult> DeleteEntity(
            [FromRoute] string uuid,
            [FromQuery] bool async = true,
            [FromQuery] string routerId = null,
            [FromQuery] string ledgerId = null
            )
        {
            if (!string.IsNullOrEmpty(uuid))
            {
                var entityClient = GetEntityClient(routerId);
                if (entityClient != null)
                {
                    var decodedUuid = HttpUtility.UrlDecode(uuid);
                    var operationMode = async ? TrakHoundOperationMode.Async : TrakHoundOperationMode.Sync;

                    if (await entityClient.Delete(decodedUuid, operationMode, ledgerId))
                    {
                        if (operationMode == TrakHoundOperationMode.Async)
                        {
                            return Accepted();
                        }
                        else
                        {
                            return Ok();
                        }
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

            return BadRequest();
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteEntities(
            [FromBody] IEnumerable<string> uuids,
            [FromQuery] bool async = true,
            [FromQuery] string routerId = null,
            [FromQuery] string ledgerId = null
            )
        {
            if (!uuids.IsNullOrEmpty())
            {
                var entityClient = GetEntityClient(routerId);
                if (entityClient != null)
                {
                    var operationMode = async ? TrakHoundOperationMode.Async : TrakHoundOperationMode.Sync;

                    if (await entityClient.Delete(uuids, operationMode, ledgerId))
                    {
                        if (operationMode == TrakHoundOperationMode.Async)
                        {
                            return Accepted();
                        }
                        else
                        {
                            return Ok();
                        }
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

            return BadRequest();
        }


        [HttpPost("expire")]
        public async Task<IActionResult> Expire(
            [FromBody] IEnumerable<TrakHoundHttpDeleteRequest> requests,
            [FromQuery] string routerId = null
            )
        {
            if (!requests.IsNullOrEmpty())
            {
                var entityClient = GetEntityClient(routerId);
                if (entityClient != null)
                {
                    var results = await entityClient.Expire(requests.ToRequests());
                    if (!results.IsNullOrEmpty())
                    {
                        return Ok(TrakHoundHttpDeleteResult.Create(results));
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

            return BadRequest();
        }

        [HttpPost("expire/access")]
        public async Task<IActionResult> ExpireByAccess(
            [FromBody] IEnumerable<TrakHoundHttpDeleteRequest> requests,
            [FromQuery] string routerId = null
            )
        {
            if (!requests.IsNullOrEmpty())
            {
                var entityClient = GetEntityClient(routerId);
                if (entityClient != null)
                {
                    var results = await entityClient.ExpireByAccess(requests.ToRequests());
                    if (!results.IsNullOrEmpty())
                    {
                        return Ok(TrakHoundHttpDeleteResult.Create(results));
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

            return BadRequest();
        }

        [HttpPost("expire/update")]
        public async Task<IActionResult> ExpireByUpdate(
            [FromBody] IEnumerable<TrakHoundHttpDeleteRequest> requests,
            [FromQuery] string routerId = null
            )
        {
            if (!requests.IsNullOrEmpty())
            {
                var entityClient = GetEntityClient(routerId);
                if (entityClient != null)
                {
                    var results = await entityClient.ExpireByUpdate(requests.ToRequests());
                    if (!results.IsNullOrEmpty())
                    {
                        return Ok(TrakHoundHttpDeleteResult.Create(results));
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

            return BadRequest();
        }
    }
}
