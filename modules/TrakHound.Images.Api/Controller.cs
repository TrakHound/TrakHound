// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.IO;
using System.Threading.Tasks;
using TrakHound.Api;

namespace TrakHound.Images.Api
{
    public class Controller : TrakHoundApiController
    {
        private const string _basePath = "/.images/files";
        private const string _original = "original";
        private const string _messageQueue = "/.images/work-queue";


        [TrakHoundApiQuery("{id}")]
        public async Task<TrakHoundApiResponse> Get([FromRoute] string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var blobPath = TrakHoundPath.Combine(_basePath, id, _original);
                var blob = await Client.Entities.GetBlob(blobPath);
                if (blob != null)
                {
                    var blobStream = await Client.Entities.GetBlobStream(blobPath);
                    if (blobStream != null)
                    {
                        return File(blobStream, blob.ContentType, null);
                    }
                    else
                    {
                        return NotFound();
                    }
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                return BadRequest();
            }
        }

        [TrakHoundApiQuery("{id}/{quality}")]
        public async Task<TrakHoundApiResponse> GetQuality([FromRoute] string id, [FromRoute] string quality)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var blobPath = TrakHoundPath.Combine(_basePath, id, quality);
                var blob = await Client.Entities.GetBlob(blobPath);
                if (blob != null)
                {
                    var blobStream = await Client.Entities.GetBlobStream(blobPath);
                    if (blobStream != null)
                    {
                        return File(blobStream, blob.ContentType, null);
                    }
                    else
                    {
                        return NotFound();
                    }
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                return BadRequest();
            }
        }


        [TrakHoundApiPublish("{id}")]
        public async Task<TrakHoundApiResponse> Publish([FromBody] Stream content, [FromRoute] string id, [FromQuery] string contentType)
        {
            if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(contentType) && content != null)
            {
                var blobPath = TrakHoundPath.Combine(_basePath, id, _original);
                if (await Client.Entities.PublishBlob(blobPath, content, contentType))
                {
                    await Client.Entities.PublishMessageQueueContent(_messageQueue, id);

                    return Created();
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
