// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Mvc;
using TrakHound.Instances;

namespace TrakHound.Http
{
    public class TrakHoundHttpInstanceController : ControllerBase
    {
        private readonly ITrakHoundInstance _instance;


        public TrakHoundHttpInstanceController(ITrakHoundInstance instance)
        {
            _instance = instance;
        }


        public IActionResult GetHostInformation()
        {
            return Ok(_instance.Information);
        }
    }
}