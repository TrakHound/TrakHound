// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Functions;
using TrakHound.Instances;

namespace TrakHound.Http
{
    [ApiController]
    [Route(HttpConstants.FunctionsPrefix)]
    public class TrakHoundHttpFunctionsController : ControllerBase
    {
        private readonly ITrakHoundInstance _server;


        public TrakHoundHttpFunctionsController(ITrakHoundInstance server)
        {
            _server = server;
        }


        [HttpGet("information")]
        public IActionResult GetInformation()
        {
            var informations = _server.FunctionManager.GetInformation();
            if (informations != null)
            {
                return Ok(informations);
            }

            return NotFound();
        }

        [HttpGet("information/{functionId}")]
        public IActionResult GetInformation(string functionId)
        {
            var information = _server.FunctionManager.GetInformation(functionId);
            if (information != null)
            {
                return Ok(information);
            }

            return NotFound();
        }

        [HttpPost("information")]
        public IActionResult GetInformation([FromBody] IEnumerable<string> functionIds)
        {
            var informations = _server.FunctionManager.GetInformation(functionIds);
            if (informations != null)
            {
                return Ok(informations);
            }

            return NotFound();
        }



        [HttpGet("run/{functionId}")]
        public async Task<IActionResult> Run([FromRoute] string functionId, [FromQuery] string runId = null, [FromQuery] long timestamp = 0, [FromQuery] string routerId = null)
        {
            if (!string.IsNullOrEmpty(functionId))
            {
                var client = _server.ClientProvider.GetClient();
                if (client != null)
                {
                    client.RouterId = routerId;

                    var response = await client.System.Functions.Run(functionId, runId: runId, timestamp: timestamp);
                    return ProcessResponse(response);
                }
            }

            return BadRequest();
        }

        [HttpPost("run/{functionId}")]
        public async Task<IActionResult> Run([FromRoute] string functionId, [FromBody] Dictionary<string, string> inputParameters, [FromQuery] string runId = null, [FromQuery] long timestamp = 0, [FromQuery] string routerId = null)
        {
            if (!string.IsNullOrEmpty(functionId))
            {
                var client = _server.ClientProvider.GetClient();
                if (client != null)
                {
                    client.RouterId = routerId;

                    var response = await client.System.Functions.Run(functionId, inputParameters, runId, timestamp);
                    return ProcessResponse(response);
                }
            }

            return BadRequest();
        }


        private IActionResult ProcessResponse(TrakHoundFunctionResponse response)
        {
            return StatusCode(response.StatusCode, response.Parameters);
        }
    }
}
