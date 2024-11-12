// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TrakHound.Instances;

namespace TrakHound.Http
{
    [ApiController]
    [Route("_routers")]
    [Produces("application/json")]
    public class TrakHoundHttpRoutersController : ControllerBase
    {
        private readonly ITrakHoundInstance _server;


        public TrakHoundHttpRoutersController(ITrakHoundInstance server)
        {
            _server = server;
        }

        [HttpGet]
        public async Task<IActionResult> GetRouters()
        {
            var client = _server.ClientProvider.GetClient();
            if (client != null)
            {
                var informations = await client.System.Routers.GetRouters();
                if (informations != null)
                {
                    return Ok(informations);
                }
            }

            return NotFound();
        }

        [HttpGet("{routerId}")]
        public async Task<IActionResult> GetRouter([FromRoute] string routerId)
        {
            var client = _server.ClientProvider.GetClient();
            if (client != null)
            {
                var information = await client.System.Routers.GetRouter(routerId);
                if (information != null)
                {
                    return Ok(information);
                }
            }

            return NotFound();
        }
    }
}
