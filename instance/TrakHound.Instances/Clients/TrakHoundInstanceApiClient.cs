// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TrakHound.Api;

namespace TrakHound.Clients
{
    public partial class TrakHoundInstanceApiClient : ITrakHoundApiClient
    {
        private readonly ITrakHoundApiProvider _apiProvider;


        public TrakHoundInstanceApiClient(ITrakHoundApiProvider apiProvider)
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


        #region "Query by Route"

        public async Task<TrakHoundApiResponse> Query(string route)
        {
            return await _apiProvider.QueryByRoute(route, null, null, null);
        }

        public async Task<TrakHoundApiResponse> Query(string route, Dictionary<string, string> queryParameters)
        {
            return await _apiProvider.QueryByRoute(route, null, null, queryParameters);
        }

        public async Task<TrakHoundApiResponse> Query(string route, object requestBody, string contentType = "application/json", Dictionary<string, string> queryParameters = null)
        {
            return await _apiProvider.QueryByRoute(route, TrakHoundApiResponse.GetJsonContentStream(requestBody), contentType, queryParameters);
        }

        public async Task<TrakHoundApiResponse> Query(string route, byte[] requestBody, string contentType = "application/octet-stream", Dictionary<string, string> queryParameters = null)
        {
            return await _apiProvider.QueryByRoute(route, TrakHoundApiResponse.GetContentStream(requestBody), contentType, queryParameters);
        }

        public async Task<TrakHoundApiResponse> Query(string route, Stream requestBody, string contentType = "application/octet-stream", Dictionary<string, string> queryParameters = null)
        {
            return await _apiProvider.QueryByRoute(route, requestBody, contentType, queryParameters);
        }



        public async Task<byte[]> QueryBytes(string route)
        {
            return (await Query(route)).GetContentBytes();
        }

        public async Task<byte[]> QueryBytes(string route, Dictionary<string, string> queryParameters)
        {
            return (await Query(route, queryParameters)).GetContentBytes();
        }

        public async Task<byte[]> QueryBytes(string route, object requestBody, string contentType = "application/json", Dictionary<string, string> queryParameters = null)
        {
            return (await Query(route, TrakHoundApiResponse.GetJsonContentStream(requestBody), contentType, queryParameters)).GetContentBytes();
        }

        public async Task<byte[]> QueryBytes(string route, byte[] requestBody, string contentType = "application/octet-stream", Dictionary<string, string> queryParameters = null)
        {
            return (await Query(route, requestBody, contentType, queryParameters)).GetContentBytes();
        }

        public async Task<byte[]> QueryBytes(string route, Stream requestBody, string contentType = "application/octet-stream", Dictionary<string, string> queryParameters = null)
        {
            return (await Query(route, requestBody, contentType, queryParameters)).GetContentBytes();
        }


        public async Task<string> QueryString(string route)
        {
            return (await Query(route)).GetContentUtf8String();
        }

        public async Task<string> QueryString(string route, Dictionary<string, string> queryParameters)
        {
            return (await Query(route, queryParameters)).GetContentUtf8String();
        }

        public async Task<string> QueryString(string route, object requestBody, string contentType = "application/json", Dictionary<string, string> queryParameters = null)
        {
            return (await Query(route, TrakHoundApiResponse.GetJsonContentStream(requestBody), contentType, queryParameters)).GetContentUtf8String();
        }

        public async Task<string> QueryString(string route, byte[] requestBody, string contentType = "application/octet-stream", Dictionary<string, string> queryParameters = null)
        {
            return (await Query(route, requestBody, contentType, queryParameters)).GetContentUtf8String();
        }

        public async Task<string> QueryString(string route, Stream requestBody, string contentType = "application/octet-stream", Dictionary<string, string> queryParameters = null)
        {
            return (await Query(route, requestBody, contentType, queryParameters)).GetContentUtf8String();
        }


        public async Task<TOutput> QueryJson<TOutput>(string route)
        {
            return (await Query(route)).GetJsonContentObject<TOutput>();
        }

        public async Task<TOutput> QueryJson<TOutput>(string route, Dictionary<string, string> queryParameters)
        {
            return (await Query(route, queryParameters)).GetJsonContentObject<TOutput>();
        }

        public async Task<TOutput> QueryJson<TOutput>(string route, object requestBody, string contentType = "application/json", Dictionary<string, string> queryParameters = null)
        {
            return (await Query(route, TrakHoundApiResponse.GetJsonContentStream(requestBody), contentType, queryParameters)).GetJsonContentObject<TOutput>();
        }

        public async Task<TOutput> QueryJson<TOutput>(string route, byte[] requestBody, string contentType = "application/octet-stream", Dictionary<string, string> queryParameters = null)
        {
            return (await Query(route, requestBody, contentType, queryParameters)).GetJsonContentObject<TOutput>();
        }

        public async Task<TOutput> QueryJson<TOutput>(string route, Stream requestBody, string contentType = "application/octet-stream", Dictionary<string, string> queryParameters = null)
        {
            return (await Query(route, requestBody, contentType, queryParameters)).GetJsonContentObject<TOutput>();
        }

        #endregion

        #region "Subscribe by Route"

        public async Task<ITrakHoundConsumer<TrakHoundApiResponse>> Subscribe(string route)
        {
            return await _apiProvider.SubscribeByRoute(route, null, null, null);
        }

        public async Task<ITrakHoundConsumer<TrakHoundApiResponse>> Subscribe(string route, Dictionary<string, string> queryParameters)
        {
            return await _apiProvider.SubscribeByRoute(route, null, null, queryParameters);
        }

        public async Task<ITrakHoundConsumer<TrakHoundApiResponse>> Subscribe(string route, object requestBody, string contentType = "application/json", Dictionary<string, string> queryParameters = null)
        {
            return await _apiProvider.SubscribeByRoute(route, TrakHoundApiResponse.GetJsonContentStream(requestBody), contentType, queryParameters);
        }

        public async Task<ITrakHoundConsumer<TrakHoundApiResponse>> Subscribe(string route, byte[] requestBody, string contentType = "application/octet-stream", Dictionary<string, string> queryParameters = null)
        {
            return await _apiProvider.SubscribeByRoute(route, TrakHoundApiResponse.GetContentStream(requestBody), contentType, queryParameters);
        }

        public async Task<ITrakHoundConsumer<TrakHoundApiResponse>> Subscribe(string route, Stream requestBody, string contentType = "application/octet-stream", Dictionary<string, string> queryParameters = null)
        {
            return await _apiProvider.SubscribeByRoute(route, requestBody, contentType, queryParameters);
        }


        public async Task<ITrakHoundConsumer<byte[]>> SubscribeBytes(string route)
        {
            var apiConsumer = await Subscribe(route);
            if (apiConsumer != null)
            {
                var onReceived = (TrakHoundApiResponse response) => response.GetContentBytes();

                return new TrakHoundConsumer<TrakHoundApiResponse, byte[]>(apiConsumer, apiConsumer.CancellationToken, apiConsumer.Id, onReceived: onReceived);
            }

            return null;
        }

        public async Task<ITrakHoundConsumer<byte[]>> SubscribeBytes(string route, Dictionary<string, string> queryParameters)
        {
            var apiConsumer = await Subscribe(route, queryParameters);
            if (apiConsumer != null)
            {
                var onReceived = (TrakHoundApiResponse response) => response.GetContentBytes();

                return new TrakHoundConsumer<TrakHoundApiResponse, byte[]>(apiConsumer, apiConsumer.CancellationToken, apiConsumer.Id, onReceived: onReceived);
            }

            return null;
        }

        public async Task<ITrakHoundConsumer<byte[]>> SubscribeBytes(string route, object requestBody, string contentType = "application/json", Dictionary<string, string> queryParameters = null)
        {
            var apiConsumer = await Subscribe(route, requestBody, contentType, queryParameters);
            if (apiConsumer != null)
            {
                var onReceived = (TrakHoundApiResponse response) => response.GetContentBytes();

                return new TrakHoundConsumer<TrakHoundApiResponse, byte[]>(apiConsumer, apiConsumer.CancellationToken, apiConsumer.Id, onReceived: onReceived);
            }

            return null;
        }

        public async Task<ITrakHoundConsumer<byte[]>> SubscribeBytes(string route, byte[] requestBody, string contentType = "application/octet-stream", Dictionary<string, string> queryParameters = null)
        {
            var apiConsumer = await Subscribe(route, requestBody, contentType, queryParameters);
            if (apiConsumer != null)
            {
                var onReceived = (TrakHoundApiResponse response) => response.GetContentBytes();

                return new TrakHoundConsumer<TrakHoundApiResponse, byte[]>(apiConsumer, apiConsumer.CancellationToken, apiConsumer.Id, onReceived: onReceived);
            }

            return null;
        }

        public async Task<ITrakHoundConsumer<byte[]>> SubscribeBytes(string route, Stream requestBody, string contentType = "application/octet-stream", Dictionary<string, string> queryParameters = null)
        {
            var apiConsumer = await Subscribe(route, requestBody, contentType, queryParameters);
            if (apiConsumer != null)
            {
                var onReceived = (TrakHoundApiResponse response) => response.GetContentBytes();

                return new TrakHoundConsumer<TrakHoundApiResponse, byte[]>(apiConsumer, apiConsumer.CancellationToken, apiConsumer.Id, onReceived: onReceived);
            }

            return null;
        }


        public async Task<ITrakHoundConsumer<string>> SubscribeString(string route)
        {
            var apiConsumer = await Subscribe(route);
            if (apiConsumer != null)
            {
                var onReceived = (TrakHoundApiResponse response) => response.GetContentUtf8String();

                return new TrakHoundConsumer<TrakHoundApiResponse, string>(apiConsumer, apiConsumer.CancellationToken, apiConsumer.Id, onReceived: onReceived);
            }

            return null;
        }

        public async Task<ITrakHoundConsumer<string>> SubscribeString(string route, Dictionary<string, string> queryParameters)
        {
            var apiConsumer = await Subscribe(route, queryParameters);
            if (apiConsumer != null)
            {
                var onReceived = (TrakHoundApiResponse response) => response.GetContentUtf8String();

                return new TrakHoundConsumer<TrakHoundApiResponse, string>(apiConsumer, apiConsumer.CancellationToken, apiConsumer.Id, onReceived: onReceived);
            }

            return null;
        }

        public async Task<ITrakHoundConsumer<string>> SubscribeString(string route, object requestBody, string contentType = "application/json", Dictionary<string, string> queryParameters = null)
        {
            var apiConsumer = await Subscribe(route, requestBody, contentType, queryParameters);
            if (apiConsumer != null)
            {
                var onReceived = (TrakHoundApiResponse response) => response.GetContentUtf8String();

                return new TrakHoundConsumer<TrakHoundApiResponse, string>(apiConsumer, apiConsumer.CancellationToken, apiConsumer.Id, onReceived: onReceived);
            }

            return null;
        }

        public async Task<ITrakHoundConsumer<string>> SubscribeString(string route, byte[] requestBody, string contentType = "application/octet-stream", Dictionary<string, string> queryParameters = null)
        {
            var apiConsumer = await Subscribe(route, requestBody, contentType, queryParameters);
            if (apiConsumer != null)
            {
                var onReceived = (TrakHoundApiResponse response) => response.GetContentUtf8String();

                return new TrakHoundConsumer<TrakHoundApiResponse, string>(apiConsumer, apiConsumer.CancellationToken, apiConsumer.Id, onReceived: onReceived);
            }

            return null;
        }

        public async Task<ITrakHoundConsumer<string>> SubscribeString(string route, Stream requestBody, string contentType = "application/octet-stream", Dictionary<string, string> queryParameters = null)
        {
            var apiConsumer = await Subscribe(route, requestBody, contentType, queryParameters);
            if (apiConsumer != null)
            {
                var onReceived = (TrakHoundApiResponse response) => response.GetContentUtf8String();

                return new TrakHoundConsumer<TrakHoundApiResponse, string>(apiConsumer, apiConsumer.CancellationToken, apiConsumer.Id, onReceived: onReceived);
            }

            return null;
        }


        public async Task<ITrakHoundConsumer<TOutput>> SubscribeJson<TOutput>(string route)
        {
            var apiConsumer = await Subscribe(route);
            if (apiConsumer != null)
            {
                var onReceived = (TrakHoundApiResponse response) => response.GetJsonContentObject<TOutput>();

                return new TrakHoundConsumer<TrakHoundApiResponse, TOutput>(apiConsumer, apiConsumer.CancellationToken, apiConsumer.Id, onReceived: onReceived);
            }

            return null;
        }

        public async Task<ITrakHoundConsumer<TOutput>> SubscribeJson<TOutput>(string route, Dictionary<string, string> queryParameters)
        {
            var apiConsumer = await Subscribe(route, queryParameters);
            if (apiConsumer != null)
            {
                var onReceived = (TrakHoundApiResponse response) => response.GetJsonContentObject<TOutput>();

                return new TrakHoundConsumer<TrakHoundApiResponse, TOutput>(apiConsumer, apiConsumer.CancellationToken, apiConsumer.Id, onReceived: onReceived);
            }

            return null;
        }

        public async Task<ITrakHoundConsumer<TOutput>> SubscribeJson<TOutput>(string route, object requestBody, string contentType = "application/json", Dictionary<string, string> queryParameters = null)
        {
            var apiConsumer = await Subscribe(route, requestBody, contentType, queryParameters);
            if (apiConsumer != null)
            {
                var onReceived = (TrakHoundApiResponse response) => response.GetJsonContentObject<TOutput>();

                return new TrakHoundConsumer<TrakHoundApiResponse, TOutput>(apiConsumer, apiConsumer.CancellationToken, apiConsumer.Id, onReceived: onReceived);
            }

            return null;
        }

        public async Task<ITrakHoundConsumer<TOutput>> SubscribeJson<TOutput>(string route, byte[] requestBody, string contentType = "application/octet-stream", Dictionary<string, string> queryParameters = null)
        {
            var apiConsumer = await Subscribe(route, requestBody, contentType, queryParameters);
            if (apiConsumer != null)
            {
                var onReceived = (TrakHoundApiResponse response) => response.GetJsonContentObject<TOutput>();

                return new TrakHoundConsumer<TrakHoundApiResponse, TOutput>(apiConsumer, apiConsumer.CancellationToken, apiConsumer.Id, onReceived: onReceived);
            }

            return null;
        }

        public async Task<ITrakHoundConsumer<TOutput>> SubscribeJson<TOutput>(string route, Stream requestBody, string contentType = "application/octet-stream", Dictionary<string, string> queryParameters = null)
        {
            var apiConsumer = await Subscribe(route, requestBody, contentType, queryParameters);
            if (apiConsumer != null)
            {
                var onReceived = (TrakHoundApiResponse response) => response.GetJsonContentObject<TOutput>();

                return new TrakHoundConsumer<TrakHoundApiResponse, TOutput>(apiConsumer, apiConsumer.CancellationToken, apiConsumer.Id, onReceived: onReceived);
            }

            return null;
        }

        #endregion

        #region "Publish by Route"

        public async Task<TrakHoundApiResponse> Publish(string route)
        {
            return await _apiProvider.PublishByRoute(route, (Stream)null, null, null);
        }

        public async Task<TrakHoundApiResponse> Publish(string route, Dictionary<string, string> queryParameters)
        {
            return await _apiProvider.PublishByRoute(route, (Stream)null, null, queryParameters);
        }

        public async Task<TrakHoundApiResponse> Publish(string route, object requestBody, string requestContentType = "application/json", Dictionary<string, string> queryParameters = null)
        {
            byte[] requestBodyBytes = null;
            if (requestBody != null)
            {
                requestBodyBytes = System.Text.Encoding.UTF8.GetBytes(requestBody.ToJson());
            }

            return await _apiProvider.PublishByRoute(route, TrakHoundApiResponse.GetContentStream(requestBodyBytes), requestContentType, queryParameters);
        }

        public async Task<TrakHoundApiResponse> Publish(string route, byte[] requestBody, string requestContentType = "application/octet-stream", Dictionary<string, string> queryParameters = null)
        {
            return await _apiProvider.PublishByRoute(route, TrakHoundApiResponse.GetContentStream(requestBody), requestContentType, queryParameters);
        }

        public async Task<TrakHoundApiResponse> Publish(string route, Stream stream, string requestContentType = "application/octet-stream", Dictionary<string, string> queryParameters = null)
        {
            Stream requestBody = null;
            if (stream != null)
            {
                requestBody = new MemoryStream();
                await stream.CopyToAsync(requestBody);
                requestBody.Seek(0, SeekOrigin.Begin);
            }

            return await _apiProvider.PublishByRoute(route, requestBody, requestContentType, queryParameters);
        }

        #endregion


        public async Task<TOutput> Publish<TOutput>(string route)
        {
            return Json.Convert<TOutput>((await _apiProvider.PublishByRoute(route, (Stream)null, null, null)).GetContentUtf8String());
        }

        public async Task<TOutput> Publish<TOutput>(string route, Dictionary<string, string> queryParameters)
        {
            return Json.Convert<TOutput>((await _apiProvider.PublishByRoute(route, (Stream)null, null, queryParameters)).GetContentUtf8String());
        }

        public async Task<TOutput> Publish<TOutput>(string route, object requestBody, string requestContentType = "application/json", Dictionary<string, string> queryParameters = null)
        {
            byte[] requestBodyBytes = null;
            if (requestBody != null)
            {
                requestBodyBytes = System.Text.Encoding.UTF8.GetBytes(requestBody.ToJson());
            }

            return Json.Convert<TOutput>((await _apiProvider.PublishByRoute(route, TrakHoundApiResponse.GetContentStream(requestBodyBytes), requestContentType, queryParameters)).GetContentUtf8String());
        }

        public async Task<TOutput> Publish<TOutput>(string route, byte[] requestBody, string requestContentType = "application/octet-stream", Dictionary<string, string> queryParameters = null)
        {
            return Json.Convert<TOutput>((await _apiProvider.PublishByRoute(route, TrakHoundApiResponse.GetContentStream(requestBody), requestContentType, queryParameters)).GetContentUtf8String());
        }

        public async Task<TOutput> Publish<TOutput>(string route, Stream stream, string requestContentType = "application/octet-stream", Dictionary<string, string> queryParameters = null)
        {
            Stream requestBody = null;
            if (stream != null)
            {
                requestBody = new MemoryStream();
                await stream.CopyToAsync(requestBody);
                requestBody.Seek(0, SeekOrigin.Begin);
            }

            return Json.Convert<TOutput>((await _apiProvider.PublishByRoute(route, requestBody, requestContentType, queryParameters)).GetContentUtf8String());
        }


        public async Task<TrakHoundApiResponse> Delete(string route)
        {
            return await _apiProvider.Delete(route, (Stream)null, null, null);
        }

        public async Task<TrakHoundApiResponse> Delete(string route, Dictionary<string, string> queryParameters)
        {
            return await _apiProvider.Delete(route, (Stream)null, null, queryParameters);
        }

        public async Task<TrakHoundApiResponse> Delete(string route, object requestBody, string requestContentType = "application/json", Dictionary<string, string> queryParameters = null)
        {
            byte[] requestBodyBytes = null;
            if (requestBody != null)
            {
                requestBodyBytes = System.Text.Encoding.UTF8.GetBytes(requestBody.ToJson());
            }

            return await _apiProvider.Delete(route, TrakHoundApiResponse.GetContentStream(requestBodyBytes), requestContentType, queryParameters);
        }

        public async Task<TrakHoundApiResponse> Delete(string route, byte[] requestBody, string requestContentType = "application/octet-stream", Dictionary<string, string> queryParameters = null)
        {
            return await _apiProvider.Delete(route, TrakHoundApiResponse.GetContentStream(requestBody), requestContentType, queryParameters);
        }

        public async Task<TrakHoundApiResponse> Delete(string route, Stream stream, string requestContentType = "application/octet-stream", Dictionary<string, string> queryParameters = null)
        {
            return await _apiProvider.Delete(route, stream, requestContentType, queryParameters);
        }

        public async Task<TOutput> Delete<TOutput>(string route)
        {
            return Json.Convert<TOutput>((await _apiProvider.Delete(route, (Stream)null, null, null)).GetContentUtf8String());
        }

        public async Task<TOutput> Delete<TOutput>(string route, Dictionary<string, string> queryParameters)
        {
            return Json.Convert<TOutput>((await _apiProvider.Delete(route, (Stream)null, null, queryParameters)).GetContentUtf8String());
        }

        public async Task<TOutput> Delete<TOutput>(string route, object requestBody, string requestContentType = "application/json", Dictionary<string, string> queryParameters = null)
        {
            byte[] requestBodyBytes = null;
            if (requestBody != null)
            {
                requestBodyBytes = System.Text.Encoding.UTF8.GetBytes(requestBody.ToJson());
            }

            return Json.Convert<TOutput>((await _apiProvider.Delete(route, TrakHoundApiResponse.GetContentStream(requestBodyBytes), requestContentType, queryParameters)).GetContentUtf8String());
        }

        public async Task<TOutput> Delete<TOutput>(string route, byte[] requestBody, string requestContentType = "application/octet-stream", Dictionary<string, string> queryParameters = null)
        {
            return Json.Convert<TOutput>((await _apiProvider.Delete(route, TrakHoundApiResponse.GetContentStream(requestBody), requestContentType, queryParameters)).GetContentUtf8String());
        }

        public async Task<TOutput> Delete<TOutput>(string route, Stream stream, string requestContentType = "application/octet-stream", Dictionary<string, string> queryParameters = null)
        {
            return Json.Convert<TOutput>((await _apiProvider.Delete(route, stream, requestContentType, queryParameters)).GetContentUtf8String());
        }


        //public async Task<TrakHoundApiResponse> Delete(string route, Dictionary<string, string> queryParameters = null, byte[] requestBody = null)
        //{
        //    return await _apiProvider.Delete(route, queryParameters, requestBody);
        //}


        private static string GetString(byte[] bytes)
        {
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
    }
}
