// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Commands;
using TrakHound.Instances;

namespace TrakHound.Http
{
    [ApiController]
    [Route(HttpConstants.CommandsPrefix)]
    public class TrakHoundHttpCommandsController : ControllerBase
    {
        private readonly ITrakHoundInstance _server;


        public TrakHoundHttpCommandsController(ITrakHoundInstance server)
        {
            _server = server;
        }


        [HttpGet("run")]
        public async Task<IActionResult> Run([FromQuery] string commandId, [FromQuery] string routerId = null)
        {
            var client = _server.ClientProvider.GetClient();
            if (client != null)
            {
                client.RouterId = routerId;

                if (!string.IsNullOrEmpty(commandId))
                {
                    return ProcessResponse(await client.System.Commands.Run(commandId));
                }
            }

            return BadRequest();
        }

        [HttpPost("run")]
        public async Task<IActionResult> Run([FromQuery] string commandId, [FromBody] Dictionary<string, string> parameters = null, [FromQuery] string routerId = null)
        {
            var client = _server.ClientProvider.GetClient();
            if (client != null)
            {
                client.RouterId = routerId;

                if (!string.IsNullOrEmpty(commandId))
                {
                    return ProcessResponse(await client.System.Commands.Run(commandId, parameters));
                }
            }

            return BadRequest();
        }


        private IActionResult ProcessResponse(IEnumerable<TrakHoundCommandResponse> responses)
        {
            if (!responses.IsNullOrEmpty())
            {
                var httpResponses = new List<TrakHoundCommandJsonResponse>();
                foreach (var response in responses)
                {
                    httpResponses.Add(new TrakHoundCommandJsonResponse(response));
                }

                return Ok(httpResponses);
            }
            else
            {
                return NotFound();
            }
        }
    }
}
