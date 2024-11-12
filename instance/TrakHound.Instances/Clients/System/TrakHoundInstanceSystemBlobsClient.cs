// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace TrakHound.Clients
{
    public class TrakHoundInstanceSystemBlobsClient : ITrakHoundSystemBlobsClient
    {
        private readonly TrakHoundInstanceClient _baseClient;


        public TrakHoundInstanceSystemBlobsClient(TrakHoundInstanceClient baseClient)
        {
            _baseClient = baseClient;
        }


        public async Task<Stream> Read(string blobId, string routerId = null)
        {
            if (!string.IsNullOrEmpty(blobId))
            {
                var router = _baseClient.GetRouter(routerId);
                if (router != null)
                {
                    var response = await router.Blobs.Read(blobId);
                    if (response.IsSuccess)
                    {
                        return response.Content.FirstOrDefault();
                    }
                }
            }

            return default;
        }

        public async Task<bool> Publish(string blobId, Stream content, string routerId = null)
        {
            if (!string.IsNullOrEmpty(blobId))
            {
                var router = _baseClient.GetRouter(routerId);
                if (router != null)
                {
                    var response = await router.Blobs.Publish(blobId, content);
                    if (response.IsSuccess)
                    {
                        return response.Content.FirstOrDefault();
                    }
                }
            }

            return default;
        }

        public async Task<bool> Delete(string blobId, string routerId = null)
        {
            if (!string.IsNullOrEmpty(blobId))
            {
                var router = _baseClient.GetRouter(routerId);
                if (router != null)
                {
                    var response = await router.Blobs.Delete(blobId);
                    if (response.IsSuccess)
                    {
                        return response.Content.FirstOrDefault();
                    }
                }
            }

            return default;
        }
    }
}
