// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.IO;
using System.Threading.Tasks;
using TrakHound.Http;

namespace TrakHound.Clients
{
    internal class TrakHoundHttpSystemBlobsClient : ITrakHoundSystemBlobsClient
    {
        private readonly TrakHoundHttpClient _baseClient;


        public string BaseUrl => _baseClient != null ? _baseClient.HttpBaseUrl : null;

        public string RouterId => _baseClient != null ? _baseClient.RouterId : null;


        public TrakHoundHttpSystemBlobsClient(TrakHoundHttpClient baseClient)
        {
            _baseClient = baseClient;
        }


        public async Task<Stream> Read(string blobId, string routerId = null)
        {
            var url = Url.Combine(BaseUrl, HttpConstants.BlobsPrefix);
            url = Url.AddQueryParameter(url, "blobId", blobId);
            url = Url.AddQueryParameter(url, "routerId", _baseClient.GetRouterId(routerId));

            return await RestRequest.GetStream(url);
        }

        public async Task<bool> Publish(string blobId, Stream content, string routerId = null)
        {
            var url = Url.Combine(BaseUrl, HttpConstants.BlobsPrefix);
            url = Url.Combine(url, "publish");
            url = Url.AddQueryParameter(url, "blobId", blobId);
            url = Url.AddQueryParameter(url, "routerId", _baseClient.GetRouterId(routerId));

            var httpResponse = await RestRequest.PostResponse(url, content);
            return httpResponse.StatusCode >= 200 && httpResponse.StatusCode < 300;
        }

        public async Task<bool> Delete(string blobId, string routerId = null)
        {
            var url = Url.Combine(BaseUrl, HttpConstants.BlobsPrefix);
            url = Url.AddQueryParameter(url, "blobId", blobId);
            url = Url.AddQueryParameter(url, "routerId", _baseClient.GetRouterId(routerId));

            var httpResponse = await RestRequest.DeleteResponse(url);
            return httpResponse.StatusCode >= 200 && httpResponse.StatusCode < 300;
        }
    }
}
