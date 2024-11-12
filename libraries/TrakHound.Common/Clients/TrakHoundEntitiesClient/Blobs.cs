// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TrakHound.Requests;

namespace TrakHound.Clients
{
    public abstract partial class TrakHoundEntitiesClientBase
    {
        private const string _blobsRoute = "blob";


        public async Task<TrakHoundBlob> GetBlob(string objectPath, string routerId = null)
        {
            if (!string.IsNullOrEmpty(objectPath))
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _blobsRoute);

                var parameters = new Dictionary<string, string>();
                parameters["objectPath"] = objectPath;
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return (await ApiClient.QueryJson<IEnumerable<TrakHoundBlob>>(url, parameters))?.FirstOrDefault();
            }

            return null;
        }

        public async Task<IEnumerable<TrakHoundBlob>> GetBlobs(string objectPath, string routerId = null)
        {
            if (!string.IsNullOrEmpty(objectPath))
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _blobsRoute);

                var parameters = new Dictionary<string, string>();
                parameters["objectPath"] = objectPath;
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.QueryJson<IEnumerable<TrakHoundBlob>>(url, parameters);
            }

            return null;
        }

        public async Task<string> GetBlobString(string objectPath, string routerId = null)
        {
            var bytes = await GetBlobBytes(objectPath, routerId);
            if (!bytes.IsNullOrEmpty())
            {
                try
                {
                    return System.Text.Encoding.UTF8.GetString(bytes);
                }
                catch { }
            }

            return null;
        }

        public async Task<byte[]> GetBlobBytes(string objectPath, string routerId = null)
        {
            if (!string.IsNullOrEmpty(objectPath))
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, $"{_blobsRoute}/download");

                var parameters = new Dictionary<string, string>();
                parameters["objectPath"] = objectPath;
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                return await ApiClient.QueryBytes(url, parameters);
            }

            return null;
        }

        public async Task<Stream> GetBlobStream(string objectPath, string routerId = null)
        {
            if (!string.IsNullOrEmpty(objectPath))
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, $"{_blobsRoute}/download");

                var parameters = new Dictionary<string, string>();
                parameters["objectPath"] = objectPath;
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                var response = await ApiClient.Query(url, parameters);
                if (response.Success && response.Content != null)
                {
                    return response.Content;
                }
            }

            return null;
        }


        public async Task<bool> PublishBlobString(string objectPath, string content, string filename = null, bool async = false, string routerId = null)
        {
            return await PublishBlob(objectPath, content, "text/plain", filename, async, routerId);
        }

        public async Task<bool> PublishBlobJson(string objectPath, string content, string filename = null, bool async = false, string routerId = null)
        {
            return await PublishBlob(objectPath, content, "application/json", filename, async, routerId);
        }

        public async Task<bool> PublishBlob(string objectPath, string content, string contentType = "text/plain", string filename = null, bool async = false, string routerId = null)
        {
            if (!string.IsNullOrEmpty(objectPath) && !string.IsNullOrEmpty(content) && !string.IsNullOrEmpty(contentType))
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _blobsRoute);
                var contentBytes = content.ToUtf8Bytes();

                var parameters = new Dictionary<string, string>();
                parameters["objectPath"] = objectPath;
                parameters["contentType"] = contentType;
                if (!string.IsNullOrEmpty(filename)) parameters["filename"] = filename;
                parameters["async"] = async.ToString();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                var response = await ApiClient.Publish(url, contentBytes, queryParameters: parameters);
                await PublishAfter();
                return response.Success;
            }

            return false;
        }

        public async Task<bool> PublishBlob(string objectPath, byte[] content, string contentType = "application/octet-stream", string filename = null, bool async = false, string routerId = null)
        {
            if (!string.IsNullOrEmpty(objectPath) && !content.IsNullOrEmpty() && !string.IsNullOrEmpty(contentType))
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _blobsRoute);

                var parameters = new Dictionary<string, string>();
                parameters["objectPath"] = objectPath;
                parameters["contentType"] = contentType;
                if (!string.IsNullOrEmpty(filename)) parameters["filename"] = filename;
                parameters["async"] = async.ToString();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                var response = await ApiClient.Publish(url, content, queryParameters: parameters);
                await PublishAfter();
                return response.Success;
            }

            return false;
        }

        public async Task<bool> PublishBlob(string objectPath, Stream stream, string contentType = "application/octet-stream", string filename = null, bool async = false, string routerId = null)
        {
            if (!string.IsNullOrEmpty(objectPath) && stream != null && !string.IsNullOrEmpty(contentType))
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _blobsRoute);

                var parameters = new Dictionary<string, string>();
                parameters["objectPath"] = objectPath;
                parameters["contentType"] = contentType;
                if (!string.IsNullOrEmpty(filename)) parameters["filename"] = filename;
                parameters["async"] = async.ToString();
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                var response = await ApiClient.Publish(url, stream, queryParameters: parameters);
                await PublishAfter();
                return response.Success;
            }

            return false;
        }


        public async Task<bool> DeleteBlob(string objectPath, string routerId = null)
        {
            if (!string.IsNullOrEmpty(objectPath))
            {
                var url = Url.Combine(ApiRoute, _objectsRoute, _blobsRoute);

                var parameters = new Dictionary<string, string>();
                parameters["objectPath"] = objectPath;
                if (!string.IsNullOrEmpty(routerId)) parameters["routerId"] = routerId;

                var response = await ApiClient.Delete(url, parameters);
                return response.Success;
            }

            return false;
        }
    }
}
