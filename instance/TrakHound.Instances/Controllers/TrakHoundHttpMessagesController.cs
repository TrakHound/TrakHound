// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TrakHound.Instances;
using TrakHound.Messages;

namespace TrakHound.Http
{
    [ApiController]
    [Route(HttpConstants.MessagesPrefix)]
    public class TrakHoundHttpMessagesController : TrakHoundHttpController
    {
        private readonly ITrakHoundInstance _instance;
        private readonly TrakHoundHttpWebSocketManager _webSocketManager;


        public TrakHoundHttpMessagesController(ITrakHoundInstance instance, TrakHoundHttpWebSocketManager webSocketManager)
        {
            _instance = instance;
            _webSocketManager = webSocketManager;
        }


        [HttpGet("brokers")]
        public async Task<IActionResult> QueryBrokers([FromQuery] string routerId = null)
        {
            var client = _instance.ClientProvider.GetClient();
            if (client != null)
            {
                client.RouterId = routerId;

                var brokers = await client.System.Messages.QueryBrokers();
                if (!brokers.IsNullOrEmpty())
                {
                    return Ok(brokers);
                }
                else
                {
                    return NotFound();
                }
            }

            return BadRequest();
        }

        [HttpPost("brokers")]
        public async Task<IActionResult> QueryBrokers([FromBody] IEnumerable<string> brokerIds, [FromQuery] string routerId = null)
        {
            var client = _instance.ClientProvider.GetClient();
            if (client != null)
            {
                client.RouterId = routerId;

                if (!brokerIds.IsNullOrEmpty())
                {
                    var brokers = await client.System.Messages.QueryBrokersById(brokerIds);
                    if (!brokers.IsNullOrEmpty())
                    {
                        return Ok(brokers);
                    }
                    else
                    {
                        return NotFound();
                    }
                }
            }

            return BadRequest();
        }

        [HttpGet("senders")]
        public async Task<IActionResult> QuerySenders([FromQuery] string routerId = null)
        {
            var client = _instance.ClientProvider.GetClient();
            if (client != null)
            {
                client.RouterId = routerId;

                var senders = await client.System.Messages.QuerySenders();
                if (!senders.IsNullOrEmpty())
                {
                    return Ok(senders);
                }
                else
                {
                    return NotFound();
                }
            }

            return BadRequest();
        }

        [HttpPost("senders")]
        public async Task<IActionResult> QuerySenders([FromBody] IEnumerable<string> senderIds, [FromQuery] string routerId = null)
        {
            var client = _instance.ClientProvider.GetClient();
            if (client != null)
            {
                client.RouterId = routerId;

                if (!senderIds.IsNullOrEmpty())
                {
                    var brokers = await client.System.Messages.QuerySendersById(senderIds);
                    if (!brokers.IsNullOrEmpty())
                    {
                        return Ok(brokers);
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
            [FromQuery] string brokerId,
            [FromQuery] string clientId = null,
            [FromQuery] int qos = 0,
            [FromQuery] string routerId = null
            )
        {
            var client = _instance.ClientProvider.GetClient();
            if (client != null)
            {
                client.RouterId = routerId;

                Func<Stream, Task<ITrakHoundConsumer<TrakHoundMessageResponse>>> getConsumer = async (Stream requestBody) =>
                {
                    var topics = GetRequestBodyJson<IEnumerable<string>>(requestBody);

                    return await client.System.Messages.Subscribe(brokerId, clientId, topics, qos);
                };

                var formatResponse = (TrakHoundMessageResponse messageResponse) =>
                {
                    if (!string.IsNullOrEmpty(messageResponse.Topic) && messageResponse.Content != null)
                    {
                        var headerBytes = messageResponse.Topic.ToUtf8Bytes();
                        var dividerBytes = new byte[] { 10, 13 };
                        var contentBytes = messageResponse.GetContentBytes();

                        var responseBytes = new byte[headerBytes.Length + dividerBytes.Length + contentBytes.Length];
                        Array.Copy(headerBytes, 0, responseBytes, 0, headerBytes.Length);
                        Array.Copy(dividerBytes, 0, responseBytes, headerBytes.Length, dividerBytes.Length);
                        Array.Copy(contentBytes, 0, responseBytes, headerBytes.Length + dividerBytes.Length, contentBytes.Length);
                        return responseBytes;
                    }

                    return null;
                };

                await _webSocketManager.Create<TrakHoundMessageResponse>(HttpContext, getConsumer, formatResponse);
            }
        }

        [HttpPost("publish")]
        public async Task<IActionResult> Publish(
            [FromQuery] string brokerId,
            [FromQuery] string topic,
            [FromQuery] bool retain = false,
            [FromQuery] int qos = 0,
            [FromQuery] string routerId = null
            )
        {
            var client = _instance.ClientProvider.GetClient();
            if (client != null)
            {
                client.RouterId = routerId;

                if (!string.IsNullOrEmpty(topic) && Request.Body != null)
                {
                    if (await client.System.Messages.Publish(brokerId, topic, Request.Body, retain, qos))
                    {
                        return Ok($"Topic = {topic} Published Successfully");
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
