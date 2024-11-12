// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TrakHound.Api;
using TrakHound.Logging;

namespace TrakHound.Clients
{
    public partial class TrakHoundInstanceSystemApiClient : ITrakHoundSystemApiClient
    {
        private readonly ITrakHoundApiProvider _apiProvider;


        public TrakHoundInstanceSystemApiClient(ITrakHoundApiProvider apiProvider)
        {
            _apiProvider = apiProvider;
        }


        public async Task<IEnumerable<TrakHoundApiInformation>> GetRouteInformation()
        {
            return _apiProvider.GetRouteInformation();
        }

        public async Task<IEnumerable<TrakHoundApiInformation>> GetPackageInformation()
        {
            return _apiProvider.GetPackageInformation();
        }

        public async Task<TrakHoundApiInformation> GetInformation(string apiId)
        {
            return _apiProvider.GetInformation(apiId);
        }


        #region "Query by Package"

        public async Task<TrakHoundApiResponse> Query(string packageId, string packageVersion, string path, string routerId = null)
        {
            return await _apiProvider.QueryByPackage(packageId, packageVersion, path, null, null, null, routerId);
        }

        public async Task<TrakHoundApiResponse> Query(string packageId, string packageVersion, string path, Dictionary<string, string> queryParameters, string routerId = null)
        {
            return await _apiProvider.QueryByPackage(packageId, packageVersion, path, null, null, queryParameters, routerId);
        }

        public async Task<TrakHoundApiResponse> Query(string packageId, string packageVersion, string path, object requestBody, string contentType = "application/json", Dictionary<string, string> queryParameters = null, string routerId = null)
        {
            byte[] requestBodyBytes = null;
            if (requestBody != null)
            {
                requestBodyBytes = System.Text.Encoding.UTF8.GetBytes(requestBody.ToJson());
            }

            return await _apiProvider.QueryByPackage(packageId, packageVersion, path, TrakHoundApiResponse.GetContentStream(requestBodyBytes), contentType, queryParameters, routerId);
        }

        public async Task<TrakHoundApiResponse> Query(string packageId, string packageVersion, string path, byte[] requestBody, string contentType = "application/octet-stream", Dictionary<string, string> queryParameters = null, string routerId = null)
        {
            return await _apiProvider.QueryByPackage(packageId, packageVersion, path, TrakHoundApiResponse.GetContentStream(requestBody), contentType, queryParameters, routerId);
        }

        #endregion

        #region "Subscribe by Package"

        public async Task<ITrakHoundConsumer<TrakHoundApiResponse>> Subscribe(string packageId, string packageVersion, string path, string routerId = null)
        {
            return await _apiProvider.SubscribeByPackage(packageId, packageVersion, path, null, null, null, routerId);
        }

        public async Task<ITrakHoundConsumer<TrakHoundApiResponse>> Subscribe(string packageId, string packageVersion, string path, Dictionary<string, string> queryParameters, string routerId = null)
        {
            return await _apiProvider.SubscribeByPackage(packageId, packageVersion, path, null, null, queryParameters, routerId);
        }

        public async Task<ITrakHoundConsumer<TrakHoundApiResponse>> Subscribe(string packageId, string packageVersion, string path, object requestBody, string contentType = "application/json", Dictionary<string, string> queryParameters = null, string routerId = null)
        {
            byte[] requestBodyBytes = null;
            if (requestBody != null)
            {
                requestBodyBytes = System.Text.Encoding.UTF8.GetBytes(requestBody.ToJson());
            }

            return await _apiProvider.SubscribeByPackage(packageId, packageVersion, path, TrakHoundApiResponse.GetContentStream(requestBodyBytes), contentType, queryParameters, routerId);
        }

        public async Task<ITrakHoundConsumer<TrakHoundApiResponse>> Subscribe(string packageId, string packageVersion, string path, byte[] requestBody, string contentType = "application/octet-stream", Dictionary<string, string> queryParameters = null, string routerId = null)
        {
            return await _apiProvider.SubscribeByPackage(packageId, packageVersion, path, TrakHoundApiResponse.GetContentStream(requestBody), contentType, queryParameters, routerId);
        }

        #endregion

        #region "Publish by Package"

        public async Task<TrakHoundApiResponse> Publish(string packageId, string packageVersion, string path, string routerId = null)
        {
            return await _apiProvider.PublishByPackage(packageId, packageVersion, path, (Stream)null, null, null);
        }

        public async Task<TrakHoundApiResponse> Publish(string packageId, string packageVersion, string path, Dictionary<string, string> queryParameters, string routerId = null)
        {
            return await _apiProvider.PublishByPackage(packageId, packageVersion, path, (Stream)null, null, queryParameters);
        }

        public async Task<TrakHoundApiResponse> Publish(string packageId, string packageVersion, string path, object requestBody, string requestContentType = "application/json", Dictionary<string, string> queryParameters = null, string routerId = null)
        {
            byte[] requestBodyBytes = null;
            if (requestBody != null)
            {
                requestBodyBytes = System.Text.Encoding.UTF8.GetBytes(requestBody.ToJson());
            }

            return await _apiProvider.PublishByPackage(packageId, packageVersion, path, TrakHoundApiResponse.GetContentStream(requestBodyBytes), requestContentType, queryParameters);
        }

        public async Task<TrakHoundApiResponse> Publish(string packageId, string packageVersion, string path, byte[] requestBody, string requestContentType = "application/octet-stream", Dictionary<string, string> queryParameters = null, string routerId = null)
        {
            return await _apiProvider.PublishByPackage(packageId, packageVersion, path, TrakHoundApiResponse.GetContentStream(requestBody), requestContentType, queryParameters);
        }

        public async Task<TrakHoundApiResponse> Publish(string packageId, string packageVersion, string path, Stream stream, string requestContentType = "application/octet-stream", Dictionary<string, string> queryParameters = null, string routerId = null)
        {
            return await _apiProvider.PublishByPackage(packageId, packageVersion, path, stream, requestContentType, queryParameters);
        }

        #endregion


        public async Task<ITrakHoundConsumer<IEnumerable<TrakHoundLogItem>>> SubscribeToLog(string apiId, TrakHoundLogLevel minimumLevel = TrakHoundLogLevel.Information)
        {
            if (!string.IsNullOrEmpty(apiId))
            {
                var consumer = new TrakHoundInstanceConsumer<IEnumerable<TrakHoundLogItem>>();

                _apiProvider.ApiLogUpdated += (id, item) =>
                {
                    if (id == apiId)
                    {
                        consumer.Push(new TrakHoundLogItem[] { item });
                    }
                };

                return consumer;
            }

            return null;
        }
    }
}
