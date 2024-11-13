// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TrakHound.Api;
using TrakHound.Http;
using TrakHound.Logging;

namespace TrakHound.Clients
{
    internal class TrakHoundHttpSystemApiClient : ITrakHoundSystemApiClient
    {
        private readonly TrakHoundHttpClient _baseClient;


        public string BaseUrl => _baseClient != null ? _baseClient.HttpBaseUrl : null;

        public string RouterId => _baseClient != null ? _baseClient.RouterId : null;


        public TrakHoundHttpSystemApiClient(TrakHoundHttpClient baseClient)
        {
            _baseClient = baseClient;
        }


        #region "Information"

        public async Task<IEnumerable<TrakHoundApiInformation>> GetRouteInformation()
        {
            var url = Url.Combine(BaseUrl, "_api/information/route");
            return await RestRequest.Get<IEnumerable<TrakHoundApiInformation>>(url);
        }

        public async Task<IEnumerable<TrakHoundApiInformation>> GetPackageInformation()
        {
            var url = Url.Combine(BaseUrl, "_api/information/package");
            return await RestRequest.Get<IEnumerable<TrakHoundApiInformation>>(url);
        }

        public async Task<TrakHoundApiInformation> GetInformation(string apiId)
        {
            var url = Url.Combine(BaseUrl, "_api/information");
            url = Url.Combine(url, apiId);

            return await RestRequest.Get<TrakHoundApiInformation>(url);
        }

        #endregion

        #region "Query by Package"

        public async Task<TrakHoundApiResponse> Query(string packageId, string packageVersion, string path, string routerId = null)
        {
            var url = Url.Combine(BaseUrl, "api");
            url = Url.Combine(url, packageId);
            url = Url.Combine(url, path);
            url = Url.AddQueryParameter(url, "packageVersion", packageVersion);

            var httpResponse = await RestRequest.GetResponse(url);
            var parameters = TrakHoundHttp.GetApiParameters(httpResponse);

            return new TrakHoundApiResponse(httpResponse.StatusCode, httpResponse.Content, httpResponse.ContentType, parameters);
        }

        public async Task<TrakHoundApiResponse> Query(string packageId, string packageVersion, string path, Dictionary<string, string> queryParameters, string routerId = null)
        {
            var url = Url.Combine(BaseUrl, "api");
            url = Url.Combine(url, packageId);
            url = Url.Combine(url, path);
            url = Url.AddQueryParameter(url, "packageVersion", packageVersion);

            // Add Parameters
            if (!queryParameters.IsNullOrEmpty())
            {
                foreach (var queryParameter in queryParameters)
                {
                    url = Url.AddQueryParameter(url, queryParameter.Key, queryParameter.Value);
                }
            }

            var httpResponse = await RestRequest.GetResponse(url);
            var parameters = TrakHoundHttp.GetApiParameters(httpResponse);

            return new TrakHoundApiResponse(httpResponse.StatusCode, httpResponse.Content, httpResponse.ContentType, parameters);
        }

        public async Task<TrakHoundApiResponse> Query(string packageId, string packageVersion, string path, object requestBody, string contentType = "application/json", Dictionary<string, string> queryParameters = null, string routerId = null)
        {
            var url = Url.Combine(BaseUrl, "api");
            url = Url.Combine(url, packageId);
            url = Url.Combine(url, path);
            url = Url.AddQueryParameter(url, "packageVersion", packageVersion);

            // Add Parameters
            if (!queryParameters.IsNullOrEmpty())
            {
                foreach (var queryParameter in queryParameters)
                {
                    url = Url.AddQueryParameter(url, queryParameter.Key, queryParameter.Value);
                }
            }

            HttpResponse httpResponse;
            if (requestBody != null) httpResponse = await RestRequest.PostResponse(url, requestBody, contentType);
            else httpResponse = await RestRequest.GetResponse(url);

            var parameters = TrakHoundHttp.GetApiParameters(httpResponse);

            return new TrakHoundApiResponse(httpResponse.StatusCode, httpResponse.Content, httpResponse.ContentType, parameters);
        }

        public async Task<TrakHoundApiResponse> Query(string packageId, string packageVersion, string path, byte[] requestBody, string contentType = "application/octet-stream", Dictionary<string, string> queryParameters = null, string routerId = null)
        {
            var url = Url.Combine(BaseUrl, "api");
            url = Url.Combine(url, packageId);
            url = Url.Combine(url, path);
            url = Url.AddQueryParameter(url, "packageVersion", packageVersion);

            // Add Parameters
            if (!queryParameters.IsNullOrEmpty())
            {
                foreach (var queryParameter in queryParameters)
                {
                    url = Url.AddQueryParameter(url, queryParameter.Key, queryParameter.Value);
                }
            }

            HttpResponse httpResponse;
            if (requestBody != null) httpResponse = await RestRequest.PostResponse(url, requestBody, contentType);
            else httpResponse = await RestRequest.GetResponse(url);

            var parameters = TrakHoundHttp.GetApiParameters(httpResponse);

            return new TrakHoundApiResponse(httpResponse.StatusCode, httpResponse.Content, httpResponse.ContentType, parameters);
        }

        #endregion

        #region "Subscribe by Package"

        public async Task<ITrakHoundConsumer<TrakHoundApiResponse>> Subscribe(string packageId, string packageVersion, string path, string routerId = null)
        {
            var url = Url.Combine(BaseUrl, HttpConstants.ApiPrefix);
            url = Url.Combine(url, packageId);
            url = Url.Combine(url, path);
            url = Url.Combine(url, "subscribe");
            url = Url.AddQueryParameter(url, "packageVersion", packageVersion);

            var consumer = new TrakHoundApiClientConsumer(_baseClient.ClientConfiguration, url, null);
            consumer.Subscribe();
            return consumer;
        }

        public async Task<ITrakHoundConsumer<TrakHoundApiResponse>> Subscribe(string packageId, string packageVersion, string path, Dictionary<string, string> queryParameters, string routerId = null)
        {
            var url = Url.Combine(BaseUrl, HttpConstants.ApiPrefix);
            url = Url.Combine(url, packageId);
            url = Url.Combine(url, path);
            url = Url.Combine(url, "subscribe");
            url = Url.AddQueryParameter(url, "packageVersion", packageVersion);

            // Add Parameters
            if (!queryParameters.IsNullOrEmpty())
            {
                foreach (var queryParameter in queryParameters)
                {
                    url = Url.AddQueryParameter(url, queryParameter.Key, queryParameter.Value);
                }
            }

            var consumer = new TrakHoundApiClientConsumer(_baseClient.ClientConfiguration, url, null);
            consumer.Subscribe();
            return consumer;
        }

        public async Task<ITrakHoundConsumer<TrakHoundApiResponse>> Subscribe(string packageId, string packageVersion, string path, object requestBody, string contentType = "application/json", Dictionary<string, string> queryParameters = null, string routerId = null)
        {
            var url = Url.Combine(BaseUrl, HttpConstants.ApiPrefix);
            url = Url.Combine(url, packageId);
            url = Url.Combine(url, path);
            url = Url.Combine(url, "subscribe");
            url = Url.AddQueryParameter(url, "packageVersion", packageVersion);

            // Add Parameters
            if (!queryParameters.IsNullOrEmpty())
            {
                foreach (var queryParameter in queryParameters)
                {
                    url = Url.AddQueryParameter(url, queryParameter.Key, queryParameter.Value);
                }
            }

            var consumer = new TrakHoundApiClientConsumer(_baseClient.ClientConfiguration, url, requestBody);
            consumer.Subscribe();
            return consumer;
        }

        public async Task<ITrakHoundConsumer<TrakHoundApiResponse>> Subscribe(string packageId, string packageVersion, string path, byte[] requestBody, string contentType = "application/octet-stream", Dictionary<string, string> queryParameters = null, string routerId = null)
        {
            var url = Url.Combine(BaseUrl, HttpConstants.ApiPrefix);
            url = Url.Combine(url, packageId);
            url = Url.Combine(url, path);
            url = Url.Combine(url, "subscribe");
            url = Url.AddQueryParameter(url, "packageVersion", packageVersion);

            // Add Parameters
            if (!queryParameters.IsNullOrEmpty())
            {
                foreach (var queryParameter in queryParameters)
                {
                    url = Url.AddQueryParameter(url, queryParameter.Key, queryParameter.Value);
                }
            }

            var consumer = new TrakHoundApiClientConsumer(_baseClient.ClientConfiguration, url, requestBody);
            consumer.Subscribe();
            return consumer;
        }

        #endregion

        #region "Publish by Package"

        public async Task<TrakHoundApiResponse> Publish(string packageId, string packageVersion, string path, string routerId = null)
        {
            var url = Url.Combine(BaseUrl, "api");
            url = Url.Combine(url, packageId);
            url = Url.Combine(url, path);
            url = Url.Combine(url, "publish");
            url = Url.AddQueryParameter(url, "packageVersion", packageVersion);

            var httpResponse = await RestRequest.PutResponse(url);
            var parameters = TrakHoundHttp.GetApiParameters(httpResponse);

            return new TrakHoundApiResponse(httpResponse.StatusCode, httpResponse.Content, httpResponse.ContentType, parameters);
        }

        public async Task<TrakHoundApiResponse> Publish(string packageId, string packageVersion, string path, Dictionary<string, string> queryParameters, string routerId = null)
        {
            var url = Url.Combine(BaseUrl, "api");
            url = Url.Combine(url, packageId);
            url = Url.Combine(url, path);
            url = Url.Combine(url, "publish");
            url = Url.AddQueryParameter(url, "packageVersion", packageVersion);

            // Add Parameters
            if (!queryParameters.IsNullOrEmpty())
            {
                foreach (var queryParameter in queryParameters)
                {
                    url = Url.AddQueryParameter(url, queryParameter.Key, queryParameter.Value);
                }
            }

            var httpResponse = await RestRequest.PutResponse(url);
            var parameters = TrakHoundHttp.GetApiParameters(httpResponse);

            return new TrakHoundApiResponse(httpResponse.StatusCode, httpResponse.Content, httpResponse.ContentType, parameters);
        }

        public async Task<TrakHoundApiResponse> Publish(string packageId, string packageVersion, string path, object requestBody, string requestContentType = "application/json", Dictionary<string, string> queryParameters = null, string routerId = null)
        {
            var url = Url.Combine(BaseUrl, "api");
            url = Url.Combine(url, packageId);
            url = Url.Combine(url, path);
            url = Url.Combine(url, "publish");
            url = Url.AddQueryParameter(url, "packageVersion", packageVersion);

            // Add Parameters
            if (!queryParameters.IsNullOrEmpty())
            {
                foreach (var queryParameter in queryParameters)
                {
                    url = Url.AddQueryParameter(url, queryParameter.Key, queryParameter.Value);
                }
            }

            var httpResponse = await RestRequest.PostResponse(url, requestBody, requestContentType);
            var parameters = TrakHoundHttp.GetApiParameters(httpResponse);

            return new TrakHoundApiResponse(httpResponse.StatusCode, httpResponse.Content, httpResponse.ContentType, parameters);
        }

        public async Task<TrakHoundApiResponse> Publish(string packageId, string packageVersion, string path, byte[] requestBody, string requestContentType = "application/octet-stream", Dictionary<string, string> queryParameters = null, string routerId = null)
        {
            var url = Url.Combine(BaseUrl, "api");
            url = Url.Combine(url, packageId);
            url = Url.Combine(url, path);
            url = Url.Combine(url, "publish");
            url = Url.AddQueryParameter(url, "packageVersion", packageVersion);

            // Add Parameters
            if (!queryParameters.IsNullOrEmpty())
            {
                foreach (var queryParameter in queryParameters)
                {
                    url = Url.AddQueryParameter(url, queryParameter.Key, queryParameter.Value);
                }
            }

            var httpResponse = await RestRequest.PostResponse(url, requestBody, requestContentType);
            var parameters = TrakHoundHttp.GetApiParameters(httpResponse);

            return new TrakHoundApiResponse(httpResponse.StatusCode, httpResponse.Content, httpResponse.ContentType, parameters);
        }

        public async Task<TrakHoundApiResponse> Publish(string packageId, string packageVersion, string path, Stream stream, string requestContentType = "application/octet-stream", Dictionary<string, string> queryParameters = null, string routerId = null)
        {
            var url = Url.Combine(BaseUrl, "api");
            url = Url.Combine(url, packageId);
            url = Url.Combine(url, path);
            url = Url.Combine(url, "publish");
            url = Url.AddQueryParameter(url, "packageVersion", packageVersion);

            // Add Parameters
            if (!queryParameters.IsNullOrEmpty())
            {
                foreach (var queryParameter in queryParameters)
                {
                    url = Url.AddQueryParameter(url, queryParameter.Key, queryParameter.Value);
                }
            }

            var httpResponse = await RestRequest.PostResponse(url, stream, requestContentType);
            var parameters = TrakHoundHttp.GetApiParameters(httpResponse);       

            return new TrakHoundApiResponse(httpResponse.StatusCode, httpResponse.Content, httpResponse.ContentType, parameters);
        }

        #endregion


        public async Task<ITrakHoundConsumer<IEnumerable<TrakHoundLogItem>>> SubscribeToLog(string apiId, TrakHoundLogLevel minimumLevel = TrakHoundLogLevel.Information)
        {
            return null;
        }

    }
}
