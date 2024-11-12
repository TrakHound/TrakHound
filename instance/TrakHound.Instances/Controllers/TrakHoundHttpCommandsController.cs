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
                    var response = await client.System.Commands.Run(commandId);
                    return ProcessResponse(response);
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
                    var response = await client.System.Commands.Run(commandId, parameters);
                    return ProcessResponse(response);
                }
            }

            return BadRequest();
        }


        private IActionResult ProcessResponse(TrakHoundCommandResponse response)
        {
            if (response.ContentType == null || MimeTypes.IsText(response.ContentType))
            {
                if (response.Content != null)
                {
                    var contentBytes = System.Text.Encoding.ASCII.GetString(response.Content);
                    var content = Content(contentBytes, response.ContentType);
                    content.StatusCode = response.StatusCode;
                    return content;
                }
                else if (response.StatusCode >= 200 && response.StatusCode < 300)
                {
                    return Ok();
                }
                else
                {
                    return StatusCode(response.StatusCode);
                }
            }
            else
            {
                return File(response.Content, response.ContentType, response.GetParameter("filename"));
            }
        }
    }
}
