// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Mvc;
using TrakHound.Instances;

namespace TrakHound.Http
{
    public class TrakHoundHttpAppController : ControllerBase
    {
        private readonly ITrakHoundInstance _server;


        public TrakHoundHttpAppController(ITrakHoundInstance server)
        {
            _server = server;
        }


        public IActionResult GetInformations()
        {
            var information = _server.AppProvider.GetInformation();
            if (information != null)
            {
                return Ok(information);
            }

            return NotFound();
        }

        public IActionResult GetInformation(string appId)
        {
            var information = _server.AppProvider.GetInformation(appId);
            if (information != null)
            {
                return Ok(information);
            }

            return NotFound();
        }
    }
}
