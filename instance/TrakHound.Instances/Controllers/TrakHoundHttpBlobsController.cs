// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TrakHound.Instances;

namespace TrakHound.Http
{
    [ApiController]
    [Route(HttpConstants.BlobsPrefix)]
    public class TrakHoundHttpBlobsController : ControllerBase
    {
        private readonly ITrakHoundInstance _server;


        public TrakHoundHttpBlobsController(ITrakHoundInstance server)
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
                    var stream = await client.System.Blobs.Read(blobId);
                    if (stream != null)
                    {
                        return File(stream, "application/octet-stream");
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
        public async Task<IActionResult> Publish([FromQuery] string blobId, [FromQuery] string routerId = null)
        {
            var client = _server.ClientProvider.GetClient();
            if (client != null)
            {
                client.RouterId = routerId;

                if (!string.IsNullOrEmpty(blobId) && Request.Body != null)
                {
                    if (await client.System.Blobs.Publish(blobId, Request.Body))
                    {
                        return Ok($"Blob ID = {blobId} Published Successfully");
                    }
                    else
                    {
                        return StatusCode(500);
                    }
                }
            }

            return BadRequest();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] string blobId, [FromQuery] string routerId = null)
        {
            var client = _server.ClientProvider.GetClient();
            if (client != null)
            {
                client.RouterId = routerId;

                if (!string.IsNullOrEmpty(blobId))
                {
                    if (await client.System.Blobs.Delete(blobId))
                    {
                        return Ok($"Blob ID = {blobId} Deleted Successfully");
                    }
                    else
                    {
                        return StatusCode(500, $"Error Deleting Blob ID = {blobId}");
                    }
                }
            }

            return BadRequest();
        }
    }
}
