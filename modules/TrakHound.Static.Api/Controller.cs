// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.IO;
using System.Threading.Tasks;
using TrakHound.Api;

namespace TrakHound.Static.Api
{
    public class Controller : TrakHoundApiController
    {
        private const string _basePath = "static:/.files";


        [TrakHoundApiQuery("{*path}")]
        public async Task<TrakHoundApiResponse> Download([FromRoute] string path)
        {
            Logger.LogDebug($"Request : Download : path = {path}");

            var blobPath = TrakHoundPath.Combine(_basePath, path);
            var blob = await Client.Entities.GetBlob(blobPath);
            if (blob != null)
            {
                Logger.LogDebug($"Request : Download : path = {path} : Blob Read Successfully");

                var stream = await Client.Entities.GetBlobStream(blobPath);
                if (stream != null)
                {
                    Logger.LogDebug($"Request : Download : path = {path} : Blob Stream Read Successfully");

                    return File(stream, blob.ContentType, blob.Filename);
                }
                else
                {
                    return InternalError();
                }
            }
            else
            {
                return NotFound($"({path}) Not Found");
            }
        }

        [TrakHoundApiPublish("{*path}")]
        public async Task<TrakHoundApiResponse> Upload(
            [FromRoute] string path,
            [FromBody] Stream content,
            [FromQuery] string contentType,
            [FromQuery] string filename = null
            )
        {
            if (!string.IsNullOrEmpty(path) && content != null && !string.IsNullOrEmpty(contentType))
            {
                var blobPath = TrakHoundPath.Combine(_basePath, path);

                if (await Client.Entities.PublishBlob(blobPath, content, contentType, filename))
                {
                    return Created($"({path}) Uploaded Successfully");
                }
                else
                {
                    return InternalError();
                }
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
