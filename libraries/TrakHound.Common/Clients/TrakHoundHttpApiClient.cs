// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TrakHound.Api;
using TrakHound.Http;

namespace TrakHound.Clients
{
    internal class TrakHoundHttpApiClient : ITrakHoundApiClient
    {
        private readonly TrakHoundHttpClient _baseClient;


        public string HttpBaseUrl => _baseClient != null ? _baseClient.HttpBaseUrl : null;

        public string WebSocketsBaseUrl => _baseClient != null ? _baseClient.WebSocketsBaseUrl : null;

        public string RouterId => _baseClient != null ? _baseClient.RouterId : null;


        public TrakHoundHttpApiClient(TrakHoundHttpClient baseClient)
        {
            _baseClient = baseClient;
        }


        #region "Query"

        public async Task<TrakHoundApiResponse> Query(string route)
        {
            var url = Url.Combine(HttpBaseUrl, route);

            var httpResponse = await RestRequest.GetResponse(url);

            // Set the Path for the TrakHoundApiResponse
            string responsePath = null;
            if (httpResponse.Headers != null && httpResponse.Headers.Contains("Path"))
            {
                responsePath = httpResponse.Headers.GetValues("Path")?.FirstOrDefault();
            }

            return new TrakHoundApiResponse(httpResponse.StatusCode, httpResponse.Content, httpResponse.ContentType, responsePath);
        }

        public async Task<TrakHoundApiResponse> Query(string route, Dictionary<string, string> queryParameters)
        {
            var url = Url.Combine(HttpBaseUrl, route);

            // Add Parameters
            if (!queryParameters.IsNullOrEmpty())
            {
                foreach (var queryParameter in queryParameters)
                {
                    url = Url.AddQueryParameter(url, queryParameter.Key, queryParameter.Value);
                }
            }

            var httpResponse = await RestRequest.GetResponse(url);

            // Set the Path for the TrakHoundApiResponse
            string responsePath = null;
            if (httpResponse.Headers != null && httpResponse.Headers.Contains("Path"))
            {
                responsePath = httpResponse.Headers.GetValues("Path")?.FirstOrDefault();
            }

            return new TrakHoundApiResponse(httpResponse.StatusCode, httpResponse.Content, httpResponse.ContentType, responsePath);
        }

        public async Task<TrakHoundApiResponse> Query(string route, object requestBody, string contentType = "application/json", Dictionary<string, string> queryParameters = null)
        {
            var url = Url.Combine(HttpBaseUrl, route);

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

            // Set the Path for the TrakHoundApiResponse
            string responsePath = null;
            if (httpResponse.Headers != null && httpResponse.Headers.Contains("Path"))
            {
                responsePath = httpResponse.Headers.GetValues("Path")?.FirstOrDefault();
            }

            return new TrakHoundApiResponse(httpResponse.StatusCode, httpResponse.Content, httpResponse.ContentType, responsePath);
        }

        public async Task<TrakHoundApiResponse> Query(string route, byte[] requestBody, string contentType = "application/octet-stream", Dictionary<string, string> queryParameters = null)
        {
            var url = Url.Combine(HttpBaseUrl, route);

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

            // Set the Path for the TrakHoundApiResponse
            string responsePath = null;
            if (httpResponse.Headers != null && httpResponse.Headers.Contains("Path"))
            {
                responsePath = httpResponse.Headers.GetValues("Path")?.FirstOrDefault();
            }

            return new TrakHoundApiResponse(httpResponse.StatusCode, httpResponse.Content, httpResponse.ContentType, responsePath);
        }

        public async Task<TrakHoundApiResponse> Query(string route, Stream requestBody, string contentType = "application/octet-stream", Dictionary<string, string> queryParameters = null)
        {
            var url = Url.Combine(HttpBaseUrl, route);

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

            // Set the Path for the TrakHoundApiResponse
            string responsePath = null;
            if (httpResponse.Headers != null && httpResponse.Headers.Contains("Path"))
            {
                responsePath = httpResponse.Headers.GetValues("Path")?.FirstOrDefault();
            }

            return new TrakHoundApiResponse(httpResponse.StatusCode, httpResponse.Content, httpResponse.ContentType, responsePath);
        }


        public async Task<byte[]> QueryBytes(string route)
        {
            var response = await Query(route);
            return response.GetContentBytes();
        }

        public async Task<byte[]> QueryBytes(string route, Dictionary<string, string> queryParameters)
        {
            var response = await Query(route, queryParameters);
            return response.GetContentBytes();
        }

        public async Task<byte[]> QueryBytes(string route, object requestBody, string contentType = "application/json", Dictionary<string, string> queryParameters = null)
        {
            var response = await Query(route, requestBody, contentType, queryParameters);
            return response.GetContentBytes();
        }

        public async Task<byte[]> QueryBytes(string route, byte[] requestBody, string contentType = "application/octet-stream", Dictionary<string, string> queryParameters = null)
        {
            var response = await Query(route, requestBody, contentType, queryParameters);
            return response.GetContentBytes();
        }

        public async Task<byte[]> QueryBytes(string route, Stream requestBody, string contentType = "application/octet-stream", Dictionary<string, string> queryParameters = null)
        {
            var response = await Query(route, requestBody, contentType, queryParameters);
            return response.GetContentBytes();
        }


        public async Task<string> QueryString(string route)
        {
            var response = await Query(route);
            return response.GetContentUtf8String();
        }

        public async Task<string> QueryString(string route, Dictionary<string, string> queryParameters)
        {
            var response = await Query(route, queryParameters);
            return response.GetContentUtf8String();
        }

        public async Task<string> QueryString(string route, object requestBody, string contentType = "application/json", Dictionary<string, string> queryParameters = null)
        {
            var response = await Query(route, requestBody, contentType, queryParameters);
            return response.GetContentUtf8String();
        }

        public async Task<string> QueryString(string route, byte[] requestBody, string contentType = "application/octet-stream", Dictionary<string, string> queryParameters = null)
        {
            var response = await Query(route, requestBody, contentType, queryParameters);
            return response.GetContentUtf8String();
        }

        public async Task<string> QueryString(string route, Stream requestBody, string contentType = "application/octet-stream", Dictionary<string, string> queryParameters = null)
        {
            var response = await Query(route, requestBody, contentType, queryParameters);
            return response.GetContentUtf8String();
        }


        public async Task<TOutput> QueryJson<TOutput>(string route)
        {
            var response = await Query(route);
            return Json.Convert<TOutput>(response.GetContentUtf8String());
        }

        public async Task<TOutput> QueryJson<TOutput>(string route, Dictionary<string, string> queryParameters)
        {
            var response = await Query(route, queryParameters);
            return Json.Convert<TOutput>(response.GetContentUtf8String());
        }

        public async Task<TOutput> QueryJson<TOutput>(string route, object requestBody, string contentType = "application/json", Dictionary<string, string> queryParameters = null)
        {
            var response = await Query(route, requestBody, contentType, queryParameters);
            return Json.Convert<TOutput>(response.GetContentUtf8String());
        }

        public async Task<TOutput> QueryJson<TOutput>(string route, byte[] requestBody, string contentType = "application/octet-stream", Dictionary<string, string> queryParameters = null)
        {
            var response = await Query(route, requestBody, contentType, queryParameters);
            return Json.Convert<TOutput>(response.GetContentUtf8String());
        }

        public async Task<TOutput> QueryJson<TOutput>(string route, Stream requestBody, string contentType = "application/octet-stream", Dictionary<string, string> queryParameters = null)
        {
            var response = await Query(route, requestBody, contentType, queryParameters);
            return Json.Convert<TOutput>(response.GetContentUtf8String());
        }

        #endregion

        #region "Subscribe"

        public async Task<ITrakHoundConsumer<TrakHoundApiResponse>> Subscribe(string route)
        {
            var url = Url.Combine(route, "subscribe");

            var consumer = new TrakHoundApiClientConsumer(_baseClient.ClientConfiguration, url);
            consumer.Subscribe();
            return consumer;
        }

        public async Task<ITrakHoundConsumer<TrakHoundApiResponse>> Subscribe(string route, Dictionary<string, string> queryParameters)
        {
            var url = Url.Combine(route, "subscribe");

            // Add Parameters
            if (!queryParameters.IsNullOrEmpty())
            {
                foreach (var queryParameter in queryParameters)
                {
                    url = Url.AddQueryParameter(url, queryParameter.Key, queryParameter.Value);
                }
            }

            var consumer = new TrakHoundApiClientConsumer(_baseClient.ClientConfiguration, url);
            consumer.Subscribe();
            return consumer;
        }

        public async Task<ITrakHoundConsumer<TrakHoundApiResponse>> Subscribe(string route, object requestBody, string contentType = "application/json", Dictionary<string, string> queryParameters = null)
        {
            var url = Url.Combine(route, "subscribe");

            // Add Parameters
            if (!queryParameters.IsNullOrEmpty())
            {
                foreach (var queryParameter in queryParameters)
                {
                    url = Url.AddQueryParameter(url, queryParameter.Key, queryParameter.Value);
                }
            }

            var consumer = new TrakHoundApiClientConsumer(_baseClient.ClientConfiguration, url, requestBody, contentType);
            consumer.Subscribe();
            return consumer;
        }

        public async Task<ITrakHoundConsumer<TrakHoundApiResponse>> Subscribe(string route, byte[] requestBody, string contentType = "application/octet-stream", Dictionary<string, string> queryParameters = null)
        {
            var url = Url.Combine(route, "subscribe");

            // Add Parameters
            if (!queryParameters.IsNullOrEmpty())
            {
                foreach (var queryParameter in queryParameters)
                {
                    url = Url.AddQueryParameter(url, queryParameter.Key, queryParameter.Value);
                }
            }

            var consumer = new TrakHoundApiClientConsumer(_baseClient.ClientConfiguration, url, requestBody, contentType);
            consumer.Subscribe();
            return consumer;
        }

        public async Task<ITrakHoundConsumer<TrakHoundApiResponse>> Subscribe(string route, Stream requestBody, string contentType = "application/octet-stream", Dictionary<string, string> queryParameters = null)
        {
            var url = Url.Combine(route, "subscribe");

            // Add Parameters
            if (!queryParameters.IsNullOrEmpty())
            {
                foreach (var queryParameter in queryParameters)
                {
                    url = Url.AddQueryParameter(url, queryParameter.Key, queryParameter.Value);
                }
            }

            var consumer = new TrakHoundApiClientConsumer(_baseClient.ClientConfiguration, url, requestBody, contentType);
            consumer.Subscribe();
            return consumer;
        }


        public async Task<ITrakHoundConsumer<byte[]>> SubscribeBytes(string route)
        {
            var apiConsumer = await Subscribe(route);
            if (apiConsumer != null)
            {
                var consumer = new TrakHoundConsumer<TrakHoundApiResponse, byte[]>(apiConsumer, apiConsumer.CancellationToken, apiConsumer.Id);
                consumer.OnReceived = (response) => response.GetContentBytes();
                return consumer;
            }

            return null;
        }

        public async Task<ITrakHoundConsumer<byte[]>> SubscribeBytes(string route, Dictionary<string, string> queryParameters)
        {
            var apiConsumer = await Subscribe(route, queryParameters);
            if (apiConsumer != null)
            {
                var consumer = new TrakHoundConsumer<TrakHoundApiResponse, byte[]>(apiConsumer, apiConsumer.CancellationToken, apiConsumer.Id);
                consumer.OnReceived = (response) => response.GetContentBytes();
                return consumer;
            }

            return null;
        }

        public async Task<ITrakHoundConsumer<byte[]>> SubscribeBytes(string route, object requestBody, string contentType = "application/json", Dictionary<string, string> queryParameters = null)
        {
            var apiConsumer = await Subscribe(route, requestBody, contentType, queryParameters);
            if (apiConsumer != null)
            {
                var consumer = new TrakHoundConsumer<TrakHoundApiResponse, byte[]>(apiConsumer, apiConsumer.CancellationToken, apiConsumer.Id);
                consumer.OnReceived = (response) => response.GetContentBytes();
                return consumer;
            }

            return null;
        }

        public async Task<ITrakHoundConsumer<byte[]>> SubscribeBytes(string route, byte[] requestBody, string contentType = "application/octet-stream", Dictionary<string, string> queryParameters = null)
        {
            var apiConsumer = await Subscribe(route, requestBody, contentType, queryParameters);
            if (apiConsumer != null)
            {
                var consumer = new TrakHoundConsumer<TrakHoundApiResponse, byte[]>(apiConsumer, apiConsumer.CancellationToken, apiConsumer.Id);
                consumer.OnReceived = (response) => response.GetContentBytes();
                return consumer;
            }

            return null;
        }

        public async Task<ITrakHoundConsumer<byte[]>> SubscribeBytes(string route, Stream requestBody, string contentType = "application/octet-stream", Dictionary<string, string> queryParameters = null)
        {
            var apiConsumer = await Subscribe(route, requestBody, contentType, queryParameters);
            if (apiConsumer != null)
            {
                var consumer = new TrakHoundConsumer<TrakHoundApiResponse, byte[]>(apiConsumer, apiConsumer.CancellationToken, apiConsumer.Id);
                consumer.OnReceived = (response) => response.GetContentBytes();
                return consumer;
            }

            return null;
        }


        public async Task<ITrakHoundConsumer<string>> SubscribeString(string route)
        {
            var apiConsumer = await Subscribe(route);
            if (apiConsumer != null)
            {
                var consumer = new TrakHoundConsumer<TrakHoundApiResponse, string>(apiConsumer, apiConsumer.CancellationToken, apiConsumer.Id);
                consumer.OnReceived = (response) => response.GetContentUtf8String();
                return consumer;
            }

            return null;
        }

        public async Task<ITrakHoundConsumer<string>> SubscribeString(string route, Dictionary<string, string> queryParameters)
        {
            var apiConsumer = await Subscribe(route, queryParameters);
            if (apiConsumer != null)
            {
                var consumer = new TrakHoundConsumer<TrakHoundApiResponse, string>(apiConsumer, apiConsumer.CancellationToken, apiConsumer.Id);
                consumer.OnReceived = (response) => response.GetContentUtf8String();
                return consumer;
            }

            return null;
        }

        public async Task<ITrakHoundConsumer<string>> SubscribeString(string route, object requestBody, string contentType = "application/json", Dictionary<string, string> queryParameters = null)
        {
            var apiConsumer = await Subscribe(route, requestBody, contentType, queryParameters);
            if (apiConsumer != null)
            {
                var consumer = new TrakHoundConsumer<TrakHoundApiResponse, string>(apiConsumer, apiConsumer.CancellationToken, apiConsumer.Id);
                consumer.OnReceived = (response) => response.GetContentUtf8String();
                return consumer;
            }

            return null;
        }

        public async Task<ITrakHoundConsumer<string>> SubscribeString(string route, byte[] requestBody, string contentType = "application/octet-stream", Dictionary<string, string> queryParameters = null)
        {
            var apiConsumer = await Subscribe(route, requestBody, contentType, queryParameters);
            if (apiConsumer != null)
            {
                var consumer = new TrakHoundConsumer<TrakHoundApiResponse, string>(apiConsumer, apiConsumer.CancellationToken, apiConsumer.Id);
                consumer.OnReceived = (response) => response.GetContentUtf8String();
                return consumer;
            }

            return null;
        }

        public async Task<ITrakHoundConsumer<string>> SubscribeString(string route, Stream requestBody, string contentType = "application/octet-stream", Dictionary<string, string> queryParameters = null)
        {
            var apiConsumer = await Subscribe(route, requestBody, contentType, queryParameters);
            if (apiConsumer != null)
            {
                var consumer = new TrakHoundConsumer<TrakHoundApiResponse, string>(apiConsumer, apiConsumer.CancellationToken, apiConsumer.Id);
                consumer.OnReceived = (response) => response.GetContentUtf8String();
                return consumer;
            }

            return null;
        }


        public async Task<ITrakHoundConsumer<TOutput>> SubscribeJson<TOutput>(string route)
        {
            var apiConsumer = await Subscribe(route);
            if (apiConsumer != null)
            {
                var consumer = new TrakHoundConsumer<TrakHoundApiResponse, TOutput>(apiConsumer, apiConsumer.CancellationToken, apiConsumer.Id);
                consumer.OnReceived = (response) => response.GetJsonContentObject<TOutput>();
                return consumer;
            }

            return null;
        }

        public async Task<ITrakHoundConsumer<TOutput>> SubscribeJson<TOutput>(string route, Dictionary<string, string> queryParameters)
        {
            var apiConsumer = await Subscribe(route, queryParameters);
            if (apiConsumer != null)
            {
                var consumer = new TrakHoundConsumer<TrakHoundApiResponse, TOutput>(apiConsumer, apiConsumer.CancellationToken, apiConsumer.Id);
                consumer.OnReceived = (response) => response.GetJsonContentObject<TOutput>();
                return consumer;
            }

            return null;
        }

        public async Task<ITrakHoundConsumer<TOutput>> SubscribeJson<TOutput>(string route, object requestBody, string contentType = "application/json", Dictionary<string, string> queryParameters = null)
        {
            var apiConsumer = await Subscribe(route, requestBody, contentType, queryParameters);
            if (apiConsumer != null)
            {
                var consumer = new TrakHoundConsumer<TrakHoundApiResponse, TOutput>(apiConsumer, apiConsumer.CancellationToken, apiConsumer.Id);
                consumer.OnReceived = (response) => response.GetJsonContentObject<TOutput>();
                return consumer;
            }

            return null;
        }

        public async Task<ITrakHoundConsumer<TOutput>> SubscribeJson<TOutput>(string route, byte[] requestBody, string contentType = "application/octet-stream", Dictionary<string, string> queryParameters = null)
        {
            var apiConsumer = await Subscribe(route, requestBody, contentType, queryParameters);
            if (apiConsumer != null)
            {
                var consumer = new TrakHoundConsumer<TrakHoundApiResponse, TOutput>(apiConsumer, apiConsumer.CancellationToken, apiConsumer.Id);
                consumer.OnReceived = (response) => response.GetJsonContentObject<TOutput>();
                return consumer;
            }

            return null;
        }

        public async Task<ITrakHoundConsumer<TOutput>> SubscribeJson<TOutput>(string route, Stream requestBody, string contentType = "application/octet-stream", Dictionary<string, string> queryParameters = null)
        {
            var apiConsumer = await Subscribe(route, requestBody, contentType, queryParameters);
            if (apiConsumer != null)
            {
                var consumer = new TrakHoundConsumer<TrakHoundApiResponse, TOutput>(apiConsumer, apiConsumer.CancellationToken, apiConsumer.Id);
                consumer.OnReceived = (response) => response.GetJsonContentObject<TOutput>();
                return consumer;
            }

            return null;
        }

        #endregion

        #region "Publish"

        public async Task<TrakHoundApiResponse> Publish(string route)
        {
            var url = Url.Combine(HttpBaseUrl, route);
            url = Url.Combine(url, "publish");

            var httpResponse = await RestRequest.PutResponse(url);

            // Set the Path for the TrakHoundApiResponse
            string responsePath = null;
            if (httpResponse.Headers != null && httpResponse.Headers.Contains("Path"))
            {
                responsePath = httpResponse.Headers.GetValues("Path")?.FirstOrDefault();
            }

            return new TrakHoundApiResponse(httpResponse.StatusCode, httpResponse.Content, httpResponse.ContentType, responsePath);
        }

        public async Task<TrakHoundApiResponse> Publish(string route, Dictionary<string, string> queryParameters)
        {
            var url = Url.Combine(HttpBaseUrl, route);
            url = Url.Combine(url, "publish");

            // Add Parameters
            if (!queryParameters.IsNullOrEmpty())
            {
                foreach (var queryParameter in queryParameters)
                {
                    url = Url.AddQueryParameter(url, queryParameter.Key, queryParameter.Value);
                }
            }

            var httpResponse = await RestRequest.PutResponse(url);

            // Set the Path for the TrakHoundApiResponse
            string responsePath = null;
            if (httpResponse.Headers != null && httpResponse.Headers.Contains("Path"))
            {
                responsePath = httpResponse.Headers.GetValues("Path")?.FirstOrDefault();
            }

            return new TrakHoundApiResponse(httpResponse.StatusCode, httpResponse.Content, httpResponse.ContentType, responsePath);
        }

        public async Task<TrakHoundApiResponse> Publish(string route, object requestBody, string requestContentType = "application/json", Dictionary<string, string> queryParameters = null)
        {
            var url = Url.Combine(HttpBaseUrl, route);
            url = Url.Combine(url, "publish");

            // Add Parameters
            if (!queryParameters.IsNullOrEmpty())
            {
                foreach (var queryParameter in queryParameters)
                {
                    url = Url.AddQueryParameter(url, queryParameter.Key, queryParameter.Value);
                }
            }

            var httpResponse = await RestRequest.PostResponse(url, requestBody, requestContentType);

            // Set the Path for the TrakHoundApiResponse
            string responsePath = null;
            if (httpResponse.Headers != null && httpResponse.Headers.Contains("Path"))
            {
                responsePath = httpResponse.Headers.GetValues("Path")?.FirstOrDefault();
            }

            return new TrakHoundApiResponse(httpResponse.StatusCode, httpResponse.Content, httpResponse.ContentType, responsePath);
        }

        public async Task<TrakHoundApiResponse> Publish(string route, byte[] requestBody, string requestContentType = "application/octet-stream", Dictionary<string, string> queryParameters = null)
        {
            var url = Url.Combine(HttpBaseUrl, route);
            url = Url.Combine(url, "publish");

            // Add Parameters
            if (!queryParameters.IsNullOrEmpty())
            {
                foreach (var queryParameter in queryParameters)
                {
                    url = Url.AddQueryParameter(url, queryParameter.Key, queryParameter.Value);
                }
            }

            var httpResponse = await RestRequest.PostResponse(url, requestBody, requestContentType);

            // Set the Path for the TrakHoundApiResponse
            string responsePath = null;
            if (httpResponse.Headers != null && httpResponse.Headers.Contains("Path"))
            {
                responsePath = httpResponse.Headers.GetValues("Path")?.FirstOrDefault();
            }

            return new TrakHoundApiResponse(httpResponse.StatusCode, httpResponse.Content, httpResponse.ContentType, responsePath);
        }

        public async Task<TrakHoundApiResponse> Publish(string route, Stream stream, string requestContentType = "application/octet-stream", Dictionary<string, string> queryParameters = null)
        {
            var url = Url.Combine(HttpBaseUrl, route);
            url = Url.Combine(url, "publish");

            // Add Parameters
            if (!queryParameters.IsNullOrEmpty())
            {
                foreach (var queryParameter in queryParameters)
                {
                    url = Url.AddQueryParameter(url, queryParameter.Key, queryParameter.Value);
                }
            }

            var httpResponse = await RestRequest.PostResponse(url, stream, requestContentType);

            // Set the Path for the TrakHoundApiResponse
            string responsePath = null;
            if (httpResponse.Headers != null && httpResponse.Headers.Contains("Path"))
            {
                responsePath = httpResponse.Headers.GetValues("Path")?.FirstOrDefault();
            }

            return new TrakHoundApiResponse(httpResponse.StatusCode, httpResponse.Content, httpResponse.ContentType, responsePath);
        }


        public async Task<TOutput> Publish<TOutput>(string route)
        {
            var response = await Publish(route);
            return Json.Convert<TOutput>(response.GetContentUtf8String());
        }

        public async Task<TOutput> Publish<TOutput>(string route, Dictionary<string, string> queryParameters)
        {
            var response = await Publish(route, queryParameters);
            return Json.Convert<TOutput>(response.GetContentUtf8String());
        }

        public async Task<TOutput> Publish<TOutput>(string route, byte[] requestBody, string requestContentType = "application/json", Dictionary<string, string> queryParameters = null)
        {
            var response = await Publish(route, requestBody, requestContentType, queryParameters);
            return Json.Convert<TOutput>(response.GetContentUtf8String());
        }

        public async Task<TOutput> Publish<TOutput>(string route, object requestBody, string requestContentType = "application/json", Dictionary<string, string> queryParameters = null)
        {
            var response = await Publish(route, requestBody, requestContentType, queryParameters);
            return Json.Convert<TOutput>(response.GetContentUtf8String());
        }

        public async Task<TOutput> Publish<TOutput>(string route, Stream stream, string requestContentType = "application/octet-stream", Dictionary<string, string> queryParameters = null)
        {
            var response = await Publish(route, stream, requestContentType, queryParameters);
            return Json.Convert<TOutput>(response.GetContentUtf8String());
        }

        #endregion

        #region "Delete"

        public async Task<TrakHoundApiResponse> Delete(string route)
        {
            var url = Url.Combine(HttpBaseUrl, route);

            var httpResponse = await RestRequest.DeleteResponse(url);

            // Set the Path for the TrakHoundApiResponse
            string responsePath = null;
            if (httpResponse.Headers != null && httpResponse.Headers.Contains("Path"))
            {
                responsePath = httpResponse.Headers.GetValues("Path")?.FirstOrDefault();
            }

            return new TrakHoundApiResponse(httpResponse.StatusCode, httpResponse.Content, httpResponse.ContentType, responsePath);
        }

        public async Task<TrakHoundApiResponse> Delete(string route, Dictionary<string, string> queryParameters)
        {
            var url = Url.Combine(HttpBaseUrl, route);

            // Add Parameters
            if (!queryParameters.IsNullOrEmpty())
            {
                foreach (var queryParameter in queryParameters)
                {
                    url = Url.AddQueryParameter(url, queryParameter.Key, queryParameter.Value);
                }
            }

            var httpResponse = await RestRequest.DeleteResponse(url);

            // Set the Path for the TrakHoundApiResponse
            string responsePath = null;
            if (httpResponse.Headers != null && httpResponse.Headers.Contains("Path"))
            {
                responsePath = httpResponse.Headers.GetValues("Path")?.FirstOrDefault();
            }

            return new TrakHoundApiResponse(httpResponse.StatusCode, httpResponse.Content, httpResponse.ContentType, responsePath);
        }

        public async Task<TrakHoundApiResponse> Delete(string route, object requestBody, string requestContentType = "application/json", Dictionary<string, string> queryParameters = null)
        {
            var url = Url.Combine(HttpBaseUrl, route);
            url = Url.Combine(url, "delete");

            // Add Parameters
            if (!queryParameters.IsNullOrEmpty())
            {
                foreach (var queryParameter in queryParameters)
                {
                    url = Url.AddQueryParameter(url, queryParameter.Key, queryParameter.Value);
                }
            }

            var httpResponse = await RestRequest.PostResponse(url, requestBody, requestContentType);

            // Set the Path for the TrakHoundApiResponse
            string responsePath = null;
            if (httpResponse.Headers != null && httpResponse.Headers.Contains("Path"))
            {
                responsePath = httpResponse.Headers.GetValues("Path")?.FirstOrDefault();
            }

            return new TrakHoundApiResponse(httpResponse.StatusCode, httpResponse.Content, httpResponse.ContentType, responsePath);
        }

        public async Task<TrakHoundApiResponse> Delete(string route, byte[] requestBody, string requestContentType = "application/octet-stream", Dictionary<string, string> queryParameters = null)
        {
            var url = Url.Combine(HttpBaseUrl, route);
            url = Url.Combine(url, "delete");

            // Add Parameters
            if (!queryParameters.IsNullOrEmpty())
            {
                foreach (var queryParameter in queryParameters)
                {
                    url = Url.AddQueryParameter(url, queryParameter.Key, queryParameter.Value);
                }
            }

            var httpResponse = await RestRequest.PostResponse(url, requestBody, requestContentType);

            // Set the Path for the TrakHoundApiResponse
            string responsePath = null;
            if (httpResponse.Headers != null && httpResponse.Headers.Contains("Path"))
            {
                responsePath = httpResponse.Headers.GetValues("Path")?.FirstOrDefault();
            }

            return new TrakHoundApiResponse(httpResponse.StatusCode, httpResponse.Content, httpResponse.ContentType, responsePath);
        }

        public async Task<TrakHoundApiResponse> Delete(string route, Stream stream, string requestContentType = "application/octet-stream", Dictionary<string, string> queryParameters = null)
        {
            var url = Url.Combine(HttpBaseUrl, route);
            url = Url.Combine(url, "delete");

            // Add Parameters
            if (!queryParameters.IsNullOrEmpty())
            {
                foreach (var queryParameter in queryParameters)
                {
                    url = Url.AddQueryParameter(url, queryParameter.Key, queryParameter.Value);
                }
            }

            var httpResponse = await RestRequest.PostResponse(url, stream, requestContentType);

            // Set the Path for the TrakHoundApiResponse
            string responsePath = null;
            if (httpResponse.Headers != null && httpResponse.Headers.Contains("Path"))
            {
                responsePath = httpResponse.Headers.GetValues("Path")?.FirstOrDefault();
            }

            return new TrakHoundApiResponse(httpResponse.StatusCode, httpResponse.Content, httpResponse.ContentType, responsePath);
        }

        public async Task<TOutput> Delete<TOutput>(string route)
        {
            var response = await Delete(route);
            return Json.Convert<TOutput>(response.GetContentUtf8String());
        }

        public async Task<TOutput> Delete<TOutput>(string route, Dictionary<string, string> queryParameters)
        {
            var response = await Delete(route, queryParameters);
            return Json.Convert<TOutput>(response.GetContentUtf8String());
        }

        public async Task<TOutput> Delete<TOutput>(string route, byte[] requestBody, string requestContentType = "application/json", Dictionary<string, string> queryParameters = null)
        {
            var response = await Delete(route, requestBody, requestContentType, queryParameters);
            return Json.Convert<TOutput>(response.GetContentUtf8String());
        }

        public async Task<TOutput> Delete<TOutput>(string route, object requestBody, string requestContentType = "application/json", Dictionary<string, string> queryParameters = null)
        {
            var response = await Delete(route, requestBody, requestContentType, queryParameters);
            return Json.Convert<TOutput>(response.GetContentUtf8String());
        }

        public async Task<TOutput> Delete<TOutput>(string route, Stream stream, string requestContentType = "application/octet-stream", Dictionary<string, string> queryParameters = null)
        {
            var response = await Delete(route, stream, requestContentType, queryParameters);
            return Json.Convert<TOutput>(response.GetContentUtf8String());
        }

        #endregion

    }
}
