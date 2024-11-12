// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Mvc;
using TrakHound.Blazor.Services;

namespace TrakHound.Instance.Controllers
{
    [ApiController]
    [Route("_download")]
    public class DownloadController : ControllerBase
    {
        private readonly TrakHoundDownloadService _service;


        public DownloadController(TrakHoundDownloadService service)
        {
            _service = service;
        }


        [HttpGet("{key}")]
        public IActionResult Download([FromRoute] string key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                var item = _service.GetItem(key);
                if (item != null)
                {
                    var stream = new MemoryStream(item.Content);
                    var result = new FileStreamResult(stream, item.ContentType);

                    if (!string.IsNullOrEmpty(item.Filename))
                    {
                        result.FileDownloadName = item.Filename;
                    }

                    return result;
                }
                else
                {
                    return NotFound();
                }
            }

            return BadRequest();
        }
    }
}