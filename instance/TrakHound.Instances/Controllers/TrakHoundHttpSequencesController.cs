// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Instances;

namespace TrakHound.Http
{
    [ApiController]
    [Route(HttpConstants.SequencesPrefix)]
    public class TrakHoundHttpSequencesController : ControllerBase
    {
        private readonly ITrakHoundInstance _server;


        public TrakHoundHttpSequencesController(ITrakHoundInstance server)
        {
            _server = server;
        }


        [HttpGet]
        public async Task<IActionResult> Read([FromQuery] string blobId, [FromQuery] string routerId = null)
        {
            var client = _server.ClientProvider.GetClient();
            if (client != null)
            {
                client.RouterId = routerId;

                if (!string.IsNullOrEmpty(blobId))
                {
                    var responses = await client.System.Sequences.Read(new string[] { blobId });
                    if (responses != null)
                    {
                        return Ok(TrakHoundHttpSequenceResponse.Create(responses));
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
        public async Task<IActionResult> Read([FromBody] IEnumerable<string> blobIds, [FromQuery] string routerId = null)
        {
            var client = _server.ClientProvider.GetClient();
            if (client != null)
            {
                client.RouterId = routerId;

                if (!blobIds.IsNullOrEmpty())
                {
                    var responses = await client.System.Sequences.Read(blobIds);
                    if (responses != null)
                    {
                        return Ok(TrakHoundHttpSequenceResponse.Create(responses));
                    }
                    else
                    {
                        return NotFound();
                    }
                }
            }

            return BadRequest();
        }

        [HttpPost("publish")]
        public async Task<IActionResult> Publish([FromQuery] string sequenceId, [FromQuery] double value, [FromQuery] long created = 0, [FromQuery] string routerId = null)
        {
            var client = _server.ClientProvider.GetClient();
            if (client != null)
            {
                client.RouterId = routerId;

                if (!string.IsNullOrEmpty(sequenceId) && value != 0)
                {
                    var createdTimestamp = created > 0 ? created : UnixDateTime.Now;

                    var response = await client.System.Sequences.Publish(sequenceId, value, createdTimestamp);
                    if (response.IsValid)
                    {
                        return Ok(new TrakHoundHttpSequenceResponse(response));
                    }
                    else
                    {
                        return StatusCode(500);
                    }
                }
            }

            return BadRequest();
        }

        [HttpPost("delete")]
        public async Task<IActionResult> Delete([FromBody] IEnumerable<string> sequenceIds, [FromQuery] string routerId = null)
        {
            var client = _server.ClientProvider.GetClient();
            if (client != null)
            {
                client.RouterId = routerId;

                if (!sequenceIds.IsNullOrEmpty())
                {
                    if (await client.System.Sequences.Delete(sequenceIds))
                    {
                        return Ok();
                    }
                    else
                    {
                        return StatusCode(500, "Error Deleting Sequences");
                    }
                }
            }

            return BadRequest();
        }
    }
}
