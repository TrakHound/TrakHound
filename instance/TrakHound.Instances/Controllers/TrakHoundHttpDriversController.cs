// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TrakHound.Instances;

namespace TrakHound.Http
{
    [ApiController]
    [Route("_drivers")]
    [Produces("application/json")]
    public class TrakHoundHttpDriversController : ControllerBase
    {
        private readonly ITrakHoundInstance _server;


        public TrakHoundHttpDriversController(ITrakHoundInstance server)
        {
            _server = server;
        }

        [HttpGet]
        public async Task<IActionResult> GetDrivers()
        {
            var client = _server.ClientProvider.GetClient();
            if (client != null)
            {
                var informations = await client.System.Drivers.GetDrivers();
                if (informations != null)
                {
                    return Ok(informations);
                }
            }

            return NotFound();
        }

        [HttpGet("{configurationId}")]
        public async Task<IActionResult> GetDriver([FromRoute] string configurationId)
        {
            var client = _server.ClientProvider.GetClient();
            if (client != null)
            {
                var information = await client.System.Drivers.GetDriver(configurationId);
                if (information != null)
                {
                    return Ok(information);
                }
            }

            return NotFound();
        }
    }
}
