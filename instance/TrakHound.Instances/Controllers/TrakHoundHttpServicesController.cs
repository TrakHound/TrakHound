// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Instances;

namespace TrakHound.Http
{
    [ApiController]
    [Route(HttpConstants.ServicesPrefix)]
    public class TrakHoundHttpServicesController : ControllerBase
    {
        private readonly ITrakHoundInstance _server;


        public TrakHoundHttpServicesController(ITrakHoundInstance server)
        {
            _server = server;
        }


        [HttpGet("information")]
        public IActionResult GetInformation()
        {
            var informations = _server.ServiceManager.GetInformation();
            if (informations != null)
            {
                return Ok(informations);
            }

            return NotFound();
        }

        [HttpGet("information/{serviceId}")]
        public IActionResult GetInformation(string serviceId)
        {
            var information = _server.ServiceManager.GetInformation(serviceId);
            if (information != null)
            {
                return Ok(information);
            }

            return NotFound();
        }

        [HttpPost("information")]
        public IActionResult GetInformation([FromBody] IEnumerable<string> serviceIds)
        {
            var informations = _server.ServiceManager.GetInformation(serviceIds);
            if (informations != null)
            {
                return Ok(informations);
            }

            return NotFound();
        }


        [HttpGet]
        public async Task<IActionResult> Retrieve(
            [FromQuery] int skip = 0,
            [FromQuery] int take = 100,
            [FromQuery] int order = (int)SortOrder.Ascending,
            [FromQuery] string routerId = null
            )
        {
            var client = _server.ClientProvider.GetClient();
            if (client != null)
            {
                client.RouterId = routerId;

                var services = await client.System.Services.Retrieve(skip, take, (SortOrder)order);
                if (!services.IsNullOrEmpty())
                {
                    return Ok(services);
                }
                else
                {
                    return StatusCode(404);
                }
            }

            return BadRequest();
        }

        [HttpGet("{serviceId}/status")]
        public async Task<IActionResult> GetStatus([FromRoute] string serviceId, [FromQuery] string routerId = null)
        {
            var client = _server.ClientProvider.GetClient();
            if (client != null)
            {
                client.RouterId = routerId;

                if (!string.IsNullOrEmpty(serviceId))
                {
                    var statuses = await client.System.Services.GetStatus(new string[] { serviceId });
                    if (!statuses.IsNullOrEmpty())
                    {
                        return Ok(TrakHoundHttpServiceStatus.Create(statuses));
                    }
                    else
                    {
                        return StatusCode(404);
                    }
                }
            }

            return BadRequest();
        }

        [HttpPost("status")]
        public async Task<IActionResult> GetStatus([FromBody] IEnumerable<string> serviceIds, [FromQuery] string routerId = null)
        {
            var client = _server.ClientProvider.GetClient();
            if (client != null)
            {
                client.RouterId = routerId;

                if (!serviceIds.IsNullOrEmpty())
                {
                    var statuses = await client.System.Services.GetStatus(serviceIds);
                    if (!statuses.IsNullOrEmpty())
                    {
                        return Ok(TrakHoundHttpServiceStatus.Create(statuses));
                    }
                    else
                    {
                        return StatusCode(404);
                    }
                }
            }

            return BadRequest();
        }

        [HttpPut("{serviceId}/start")]
        public async Task<IActionResult> StartService([FromRoute] string serviceId, [FromQuery] string routerId = null)
        {
            var client = _server.ClientProvider.GetClient();
            if (client != null)
            {
                client.RouterId = routerId;

                if (!string.IsNullOrEmpty(serviceId))
                {
                    var status = await client.System.Services.StartService(serviceId);

                    return Ok(new TrakHoundHttpServiceStatus(status));
                }
            }

            return BadRequest();
        }

        [HttpPut("{serviceId}/stop")]
        public async Task<IActionResult> StopService([FromRoute] string serviceId, [FromQuery] string routerId = null)
        {
            var client = _server.ClientProvider.GetClient();
            if (client != null)
            {
                client.RouterId = routerId;

                if (!string.IsNullOrEmpty(serviceId))
                {
                    var status = await client.System.Services.StopService(serviceId);

                    return Ok(new TrakHoundHttpServiceStatus(status));
                }
            }

            return BadRequest();
        }
    }
}
