// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using TrakHound.Instances;
using TrakHound.MessageQueues;

namespace TrakHound.Http
{
    [ApiController]
    [Route(HttpConstants.MessageQueuesPrefix)]
    public class TrakHoundHttpMessageQueuesController : TrakHoundHttpController
    {
        private readonly ITrakHoundInstance _instance;
        private readonly TrakHoundHttpWebSocketManager _webSocketManager;


        public TrakHoundHttpMessageQueuesController(ITrakHoundInstance instance, TrakHoundHttpWebSocketManager webSocketManager)
        {
            _instance = instance;
            _webSocketManager = webSocketManager;
        }


        [HttpGet("pull")]
        public async Task Pull([FromQuery] string queue, [FromQuery] bool acknowledge = true, [FromQuery] string routerId = null)
        {
            var client = _instance.ClientProvider.GetClient();
            if (client != null)
            {
                client.RouterId = routerId;

                if (!string.IsNullOrEmpty(queue))
                {
                    var response = await client.System.MessageQueues.Pull(queue, acknowledge);
                    if (response.IsValid)
                    {
                        var responseBytes = TrakHoundHttpMessageQueueResponse.Create(response);
                        if (responseBytes != null)
                        {
                            Response.StatusCode = 200;
                            await Response.Body.WriteAsync(responseBytes);
                        }
                        else
                        {
                            Response.StatusCode = 500;
                        }
                    }
                    else
                    {
                        Response.StatusCode = 404;
                    }
                }
            }
            else
            {
                Response.StatusCode = 400;
            }
        }

        [Route("subscribe")]
        public async Task Subscribe([FromQuery] string queue, [FromQuery] bool acknowledge = true, [FromQuery] string routerId = null)
        {
            var client = _instance.ClientProvider.GetClient();
            if (client != null)
            {
                client.RouterId = routerId;

                Func<Task<ITrakHoundConsumer<TrakHoundMessageQueueResponse>>> getConsumer = async () =>
                {
                    return await client.System.MessageQueues.Subscribe(queue, acknowledge);
                };

                var formatResponse = (TrakHoundMessageQueueResponse messageResponse) =>
                {
                    return TrakHoundHttpMessageQueueResponse.Create(messageResponse);
                };

                await _webSocketManager.Create<TrakHoundMessageQueueResponse>(HttpContext, getConsumer, formatResponse);
            }
        }

        [HttpPost("publish")]
        public async Task<IActionResult> Publish([FromQuery] string queue, [FromQuery] string routerId = null)
        {
            var client = _instance.ClientProvider.GetClient();
            if (client != null)
            {
                client.RouterId = routerId;

                if (!string.IsNullOrEmpty(queue) && Request.Body != null)
                {
                    if (await client.System.MessageQueues.Publish(queue, Request.Body))
                    {
                        return Ok($"Queue = {queue} Published Successfully");
                    }
                    else
                    {
                        return StatusCode(500);
                    }
                }
            }

            return BadRequest();
        }

        [HttpPut("acknowledge")]
        public async Task<IActionResult> Acknowledge([FromQuery] string queue, [FromQuery] string deliveryId, [FromQuery] string routerId = null)
        {
            var client = _instance.ClientProvider.GetClient();
            if (client != null)
            {
                client.RouterId = routerId;

                if (!string.IsNullOrEmpty(queue) && !string.IsNullOrEmpty(deliveryId))
                {
                    if (await client.System.MessageQueues.Acknowledge(queue, deliveryId))
                    {
                        return Ok($"Queue = {queue} Acknowledged Successfully");
                    }
                    else
                    {
                        return StatusCode(500);
                    }
                }
            }

            return BadRequest();
        }

        [HttpPut("reject")]
        public async Task<IActionResult> Reject([FromQuery] string queue, [FromQuery] string deliveryId, [FromQuery] string routerId = null)
        {
            var client = _instance.ClientProvider.GetClient();
            if (client != null)
            {
                client.RouterId = routerId;

                if (!string.IsNullOrEmpty(queue) && !string.IsNullOrEmpty(deliveryId))
                {
                    if (await client.System.MessageQueues.Reject(queue, deliveryId))
                    {
                        return Ok($"Queue = {queue} Rejected Successfully");
                    }
                    else
                    {
                        return StatusCode(500);
                    }
                }
            }

            return BadRequest();
        }
    }
}
