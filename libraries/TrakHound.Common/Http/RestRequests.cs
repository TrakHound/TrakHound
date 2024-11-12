// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace TrakHound.Http
{
    public static class RestRequest
    {
        private const int DefaultTimeout = 300000;

        private static readonly HttpClient httpClient = new HttpClient(
            new HttpClientHandler
            {
                AutomaticDecompression = System.Net.DecompressionMethods.GZip
            }
            )
        {
            Timeout = TimeSpan.FromMilliseconds(DefaultTimeout),
            
        };

        public static int Timeout { get; set; } = DefaultTimeout;


        public static async Task<string> Get(string url, IDictionary<string, string> headers = null)
        {
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Add("Accept-Encoding", "gzip");

                if (!headers.IsNullOrEmpty())
                {
                    foreach (var header in headers)
                    {
                        request.Headers.Add(header.Key, header.Value);
                    }
                }

                using var response = await httpClient.SendAsync(request);
                return await ProcessStreamString(response);
            }
            catch (Exception ex)
            {
                //client.LogError(ex.Message);
            }

            return null;
        }

        public static async Task<T> Get<T>(string url, IDictionary<string, string> headers = null)
        {
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Add("Accept-Encoding", "gzip");

                if (!headers.IsNullOrEmpty())
                {
                    foreach (var header in headers)
                    {
                        request.Headers.Add(header.Key, header.Value);
                    }
                }

                using var response = await httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                return await ProcessStream<T>(response);
            }
            catch (Exception ex)
            {
                //client.LogError(ex.Message);
            }

            return default(T);
        }

        public static async Task<HttpResponse> GetResponse(string url, IDictionary<string, string> headers = null)
        {
            var httpResponse = new HttpResponse();

            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Add("Accept-Encoding", "gzip");

                if (!headers.IsNullOrEmpty())
                {
                    foreach (var header in headers)
                    {
                        request.Headers.Add(header.Key, header.Value);
                    }
                }

                using var response = await httpClient.SendAsync(request);
                var content = await ProcessStream(response);

                httpResponse.StatusCode = (int)response.StatusCode;
                httpResponse.ContentType = response.Content.Headers.ContentType?.MediaType;
                httpResponse.Content = content;
                httpResponse.Headers = response.Headers;
            }
            catch (Exception ex)
            {
                //client.LogError(ex.Message);
            }

            return httpResponse;
        }

        public static async Task<byte[]> GetBytes(string url, IDictionary<string, string> headers = null)
        {
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Add("Accept-Encoding", "gzip");

                if (!headers.IsNullOrEmpty())
                {
                    foreach (var header in headers)
                    {
                        request.Headers.Add(header.Key, header.Value);
                    }
                }

                using var response = await httpClient.SendAsync(request);
                return await ProcessStream(response);
                //response.EnsureSuccessStatusCode();
                //return await response.Content.ReadAsByteArrayAsync();
            }
            catch (Exception ex)
            {
                //client.LogError(ex.Message);
            }

            return null;
        }

        public static async Task<Stream> GetStream(string url, IDictionary<string, string> headers = null)
        {
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Add("Accept-Encoding", "gzip");

                if (!headers.IsNullOrEmpty())
                {
                    foreach (var header in headers)
                    {
                        request.Headers.Add(header.Key, header.Value);
                    }
                }

                var response = await httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStreamAsync();
            }
            catch (Exception ex)
            {
                //client.LogError(ex.Message);
            }

            return null;
        }

        public static async Task<string> Put(string url, IDictionary<string, string> headers = null)
        {
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Put, url);
                request.Headers.Add("Accept-Encoding", "gzip");

                if (!headers.IsNullOrEmpty())
                {
                    foreach (var header in headers)
                    {
                        request.Headers.Add(header.Key, header.Value);
                    }
                }

                using var response = await httpClient.SendAsync(request);
                return await ProcessStreamString(response);
            }
            catch (Exception ex)
            {
                //client.LogError(ex.Message);
            }

            return null;
        }

        public static async Task<T> Put<T>(string url, IDictionary<string, string> headers = null)
        {
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Put, url);
                request.Headers.Add("Accept-Encoding", "gzip");

                if (!headers.IsNullOrEmpty())
                {
                    foreach (var header in headers)
                    {
                        request.Headers.Add(header.Key, header.Value);
                    }
                }

                using var response = await httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                return await ProcessStream<T>(response);
            }
            catch (Exception ex)
            {
                //client.LogError(ex.Message);
            }

            return default(T);
        }

        public static async Task<HttpResponse> PutResponse(string url, IDictionary<string, string> headers = null)
        {
            var httpResponse = new HttpResponse();

            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Put, url);
                request.Headers.Add("Accept-Encoding", "gzip");

                if (!headers.IsNullOrEmpty())
                {
                    foreach (var header in headers)
                    {
                        request.Headers.Add(header.Key, header.Value);
                    }
                }

                using var response = await httpClient.SendAsync(request);
                var content = await ProcessStream(response);

                httpResponse.StatusCode = (int)response.StatusCode;
                httpResponse.ContentType = response.Content.Headers.ContentType?.MediaType;
                httpResponse.Content = content;
                httpResponse.Headers = response.Headers;
            }
            catch (Exception ex)
            {
                //client.LogError(ex.Message);
            }

            return httpResponse;
        }


        public static async Task<string> Post(string url, string body, string contentType = "text/plain", IDictionary<string, string> headers = null)
        {
            try
            {
				using var request = new HttpRequestMessage(HttpMethod.Post, url);
				request.Headers.Add("Accept-Encoding", "gzip");

				if (!headers.IsNullOrEmpty())
				{
					foreach (var header in headers)
					{
						request.Headers.Add(header.Key, header.Value);
					}
				}

				if (!string.IsNullOrEmpty(body))
				{
					using var content = new StringContent(body, Encoding.UTF8, contentType);
					request.Content = content;
				}

				using var response = await httpClient.SendAsync(request);
				return await ProcessStreamString(response);
			}
            catch (Exception ex)
            {
                //client.LogError(ex.Message);
            }

            return null;
        }

        public static async Task<string> Post(string url, object body, string contentType = "application/json", IDictionary<string, string> headers = null)
        {
            try
            {
				using var request = new HttpRequestMessage(HttpMethod.Post, url);
				request.Headers.Add("Accept-Encoding", "gzip");

				HttpContent content = null;

				if (!headers.IsNullOrEmpty())
				{
					foreach (var header in headers)
					{
						request.Headers.Add(header.Key, header.Value);
					}
				}

				if (body != null)
				{
					var jsonRequest = Json.Convert(body);
					if (!string.IsNullOrEmpty(jsonRequest))
					{
						content = new StringContent(jsonRequest, Encoding.UTF8, contentType);
					}
				}

				request.Content = content;

				using var response = await httpClient.SendAsync(request);
				return await ProcessStreamString(response);
			}
            catch (Exception ex)
            {
                //client.LogError(ex.Message);
            }

            return null;
        }

        public static async Task<bool> Post(string url, IDictionary<string, string> formFields, IDictionary<string, string> headers = null)
        {
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Post, url);
                request.Headers.Add("Accept-Encoding", "gzip");

                if (!headers.IsNullOrEmpty())
                {
                    foreach (var header in headers)
                    {
                        request.Headers.Add(header.Key, header.Value);
                    }
                }

                if (formFields != null)
                {
                    request.Content = new FormUrlEncodedContent(formFields);
                }

                using var response = await httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                return true;
            }
            catch (Exception ex)
            {
                //client.LogError(ex.Message);
            }

            return false;
        }

        public static async Task<bool> Post(string url, byte[] bytes, string contentType = "application/octet-stream", IDictionary<string, string> headers = null)
        {
            try
            {
				using var request = new HttpRequestMessage(HttpMethod.Post, url);
				request.Headers.Add("Accept-Encoding", "gzip");

				if (!headers.IsNullOrEmpty())
				{
					foreach (var header in headers)
					{
						request.Headers.Add(header.Key, header.Value);
					}
				}

				if (bytes != null)
				{
					var stream = new MemoryStream(bytes);
					stream.Seek(0, SeekOrigin.Begin);

					var content = new StreamContent(stream);
					request.Content = content;
					request.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
				}

				using var response = await httpClient.SendAsync(request);
				response.EnsureSuccessStatusCode();

				return true;
			}
            catch (Exception ex)
            {
                //client.LogError(ex.Message);
            }

            return false;
        }

        public static async Task<bool> Post(string url, Stream stream, string contentType = "application/octet-stream", IDictionary<string, string> headers = null)
        {
            try
            {
                if (stream != null)
                {
                    using var request = new HttpRequestMessage(HttpMethod.Post, url);
                    request.Headers.Add("Accept-Encoding", "gzip");

                    if (!headers.IsNullOrEmpty())
                    {
                        foreach (var header in headers)
                        {
                            request.Headers.Add(header.Key, header.Value);
                        }
                    }

                    using var content = new StreamContent(stream);
                    request.Content = content;
                    request.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);

                    request.Content = content;

                    using var response = await httpClient.SendAsync(request);
                    response.EnsureSuccessStatusCode();

                    return true;
                }
            }
            catch (Exception ex)
            {
                //client.LogError(ex.Message);
            }

            return false;
        }

        public static async Task<T> Post<T>(string url, string body, string contentType = "text/plain", IDictionary<string, string> headers = null)
        {
            try
            {
                if (body != null)
                {
                    using var request = new HttpRequestMessage(HttpMethod.Post, url);
                    request.Headers.Add("Accept-Encoding", "gzip");

                    if (!headers.IsNullOrEmpty())
                    {
                        foreach (var header in headers)
                        {
                            request.Headers.Add(header.Key, header.Value);
                        }
                    }

                    using var content = new StringContent(body, Encoding.UTF8, contentType);
                    request.Content = content;

                    using var response = await httpClient.SendAsync(request);
                    response.EnsureSuccessStatusCode();
                    return await ProcessStream<T>(response);
                }
            }
            catch (Exception ex)
            {
                //client.LogError(ex.Message);
            }

            return default(T);
        }

        public static async Task<T> Post<T>(string url, IDictionary<string, string> formFields, IDictionary<string, string> headers = null)
        {
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Post, url);
                request.Headers.Add("Accept-Encoding", "gzip");

                if (!headers.IsNullOrEmpty())
                {
                    foreach (var header in headers)
                    {
                        request.Headers.Add(header.Key, header.Value);
                    }
                }

                if (formFields != null)
                {
                    request.Content = new FormUrlEncodedContent(formFields);
                }

                using var response = await httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                return await ProcessStream<T>(response);
            }
            catch (Exception ex)
            {
                //client.LogError(ex.Message);
            }

            return default(T);
        }

        public static async Task<T> Post<T>(string url, byte[] bytes, string contentType = "application/json", IDictionary<string, string> headers = null)
		{
			try
			{
				using var request = new HttpRequestMessage(HttpMethod.Post, url);
				request.Headers.Add("Accept-Encoding", "gzip");

				if (!headers.IsNullOrEmpty())
				{
					foreach (var header in headers)
					{
						request.Headers.Add(header.Key, header.Value);
					}
				}

				if (bytes != null)
				{
					var stream = new MemoryStream(bytes);
					stream.Seek(0, SeekOrigin.Begin);

					var content = new StreamContent(stream);
					request.Content = content;
					request.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
				}

				using var response = await httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                return await ProcessStream<T>(response);
			}
			catch (Exception ex)
			{
				//client.LogError(ex.Message);
			}

			return default(T);
		}

		public static async Task<T> Post<T>(string url, object body, string contentType = "application/json", IDictionary<string, string> headers = null)
        {
            try
            {
                if (body != null)
                {
                    var jsonRequest = Json.Convert(body);
                    if (!string.IsNullOrEmpty(jsonRequest))
                    {
                        using var request = new HttpRequestMessage(HttpMethod.Post, url);
                        request.Headers.Add("Accept-Encoding", "gzip");

                        if (!headers.IsNullOrEmpty())
                        {
                            foreach (var header in headers)
                            {
                                request.Headers.Add(header.Key, header.Value);
                            }
                        }

                        using var content = new StringContent(jsonRequest, Encoding.UTF8, contentType);
                        request.Content = content;

                        using var response = await httpClient.SendAsync(request);
                        response.EnsureSuccessStatusCode();
                        return await ProcessStream<T>(response);
                    }
                }
            }
            catch (Exception ex)
            {
                //client.LogError(ex.Message);
            }

            return default(T);
        }

        public static async Task<T> Post<T>(string url, Stream stream, string contentType = "application/octet-stream", IDictionary<string, string> headers = null)
        {
            try
            {
                if (stream != null)
                {
                    using var request = new HttpRequestMessage(HttpMethod.Post, url);
                    request.Headers.Add("Accept-Encoding", "gzip");

                    if (!headers.IsNullOrEmpty())
                    {
                        foreach (var header in headers)
                        {
                            request.Headers.Add(header.Key, header.Value);
                        }
                    }

                    using var content = new StreamContent(stream);
                    request.Content = content;
                    request.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);

                    using var response = await httpClient.SendAsync(request);
                    response.EnsureSuccessStatusCode();
                    return await ProcessStream<T>(response);
                }
            }
            catch (Exception ex)
            {
                //client.LogError(ex.Message);
            }

            return default(T);
        }

        public static async Task<HttpResponse> PostResponse(string url, object body, string contentType = "application/json", IDictionary<string, string> headers = null)
        {
            var httpResponse = new HttpResponse();

            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Post, url);
                request.Headers.Add("Accept-Encoding", "gzip");

                if (!headers.IsNullOrEmpty())
                {
                    foreach (var header in headers)
                    {
                        request.Headers.Add(header.Key, header.Value);
                    }
                }

                if (body != null)
                {
                    var jsonRequest = Json.Convert(body);
                    {
                        request.Content = new StringContent(jsonRequest, Encoding.UTF8, contentType);
                    }
                }

                using var response = await httpClient.SendAsync(request);
                var content = await ProcessStream(response);

                httpResponse.StatusCode = (int)response.StatusCode;
                httpResponse.ContentType = response.Content.Headers.ContentType?.MediaType;
                httpResponse.Content = content;
                httpResponse.Headers = response.Headers;
            }
            catch (Exception ex)
            {
                //client.LogError(ex.Message);
            }

            return httpResponse;
        }

        public static async Task<HttpResponse> PostResponse(string url, byte[] bytes, string contentType = "application/octet-stream", IDictionary<string, string> headers = null)
        {
            var httpResponse = new HttpResponse();

            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Post, url);
                request.Headers.Add("Accept-Encoding", "gzip");

                if (!headers.IsNullOrEmpty())
                {
                    foreach (var header in headers)
                    {
                        request.Headers.Add(header.Key, header.Value);
                    }
                }

                if (!bytes.IsNullOrEmpty())
                {
                    request.Content = new ByteArrayContent(bytes);
                    request.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
                }

                using var response = await httpClient.SendAsync(request);
                var content = await ProcessStream(response);

                httpResponse.StatusCode = (int)response.StatusCode;
                httpResponse.ContentType = response.Content.Headers.ContentType?.MediaType;
                httpResponse.Content = content;
                httpResponse.Headers = response.Headers;
            }
            catch (Exception ex)
            {
                //client.LogError(ex.Message);
            }

            return httpResponse;
        }

        public static async Task<HttpResponse> PostResponse(string url, Stream stream, string contentType = "application/octet-stream", IDictionary<string, string> headers = null)
        {
            var httpResponse = new HttpResponse();

            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Post, url);
                request.Headers.Add("Accept-Encoding", "gzip");

                if (!headers.IsNullOrEmpty())
                {
                    foreach (var header in headers)
                    {
                        request.Headers.Add(header.Key, header.Value);
                    }
                }

                if (stream != null)
                {
                    request.Content = new StreamContent(stream);
                    request.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
                }

                using var response = await httpClient.SendAsync(request);
                var content = await ProcessStream(response);

                httpResponse.StatusCode = (int)response.StatusCode;
                httpResponse.ContentType = response.Content.Headers.ContentType?.MediaType;
                httpResponse.Content = content;
                httpResponse.Headers = response.Headers;
            }
            catch (Exception ex)
            {
                //client.LogError(ex.Message);
            }

            return httpResponse;
        }


        public static async Task<bool> Delete(string url, IDictionary<string, string> headers = null)
        {
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Delete, url);
                request.Headers.Add("Accept-Encoding", "gzip");

                if (!headers.IsNullOrEmpty())
                {
                    foreach (var header in headers)
                    {
                        request.Headers.Add(header.Key, header.Value);
                    }
                }

                using var response = await httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                return true;
            }
            catch (Exception ex)
            {
                //client.LogError(ex.Message);
            }

            return false;
        }

        public static async Task<T> Delete<T>(string url, IDictionary<string, string> headers = null)
        {
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Delete, url);
                request.Headers.Add("Accept-Encoding", "gzip");

                if (!headers.IsNullOrEmpty())
                {
                    foreach (var header in headers)
                    {
                        request.Headers.Add(header.Key, header.Value);
                    }
                }

                using var response = await httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                return await ProcessStream<T>(response);
            }
            catch (Exception ex)
            {
                //client.LogError(ex.Message);
            }

            return default(T);
        }

        public static async Task<HttpResponse> DeleteResponse(string url, IDictionary<string, string> headers = null)
        {
            var httpResponse = new HttpResponse();

            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Delete, url);
                request.Headers.Add("Accept-Encoding", "gzip");

                if (!headers.IsNullOrEmpty())
                {
                    foreach (var header in headers)
                    {
                        request.Headers.Add(header.Key, header.Value);
                    }
                }

                using var response = await httpClient.SendAsync(request);
                var content = await ProcessStream(response);

                httpResponse.StatusCode = (int)response.StatusCode;
                httpResponse.ContentType = response.Content.Headers.ContentType?.MediaType;
                httpResponse.Content = content;
                httpResponse.Headers = response.Headers;
            }
            catch (Exception ex)
            {
                //client.LogError(ex.Message);
            }

            return httpResponse;
        }


        private static async Task<byte[]> ProcessStream(HttpResponseMessage response)
        {
            if (response != null)
            {
                //response.EnsureSuccessStatusCode();

                byte[] bytes = null;
                using var stream = await response.Content.ReadAsStreamAsync();
                if (stream.Length > 0)
                {
                    // Gzip
                    if (response.Content.Headers.ContentEncoding.Contains("gzip"))
                    {
                        using (var readStream = new MemoryStream())
                        using (var gzipStream = new GZipStream(stream, CompressionMode.Decompress))
                        {
                            gzipStream.CopyTo(readStream);

                            bytes = readStream.ToArray();
                        }
                    }
                    else
                    {
                        using var memoryStream = new MemoryStream();
                        stream.CopyTo(memoryStream);
                        bytes = memoryStream.ToArray();
                    }
                }

                return bytes;
            }

            return default;
        }

        private static async Task<string> ProcessStreamString(HttpResponseMessage response)
        {
            if (response != null)
            {
                //response.EnsureSuccessStatusCode();

                string json = null;
                using var stream = await response.Content.ReadAsStreamAsync();

                // Gzip
                if (response.Content.Headers.ContentEncoding.Contains("gzip"))
                {
                    using (var readStream = new MemoryStream())
                    using (var gzipStream = new GZipStream(stream, CompressionMode.Decompress))
                    {
                        gzipStream.CopyTo(readStream);

                        var bytes = readStream.ToArray();

                        json = Encoding.ASCII.GetString(bytes);
                    }
                }
                else
                {
                    using (var streamReader = new StreamReader(stream))
                    {
                        json = streamReader.ReadToEnd();
                    }
                }

                return json;
            }

            return default;
        }

        private static async Task<T> ProcessStream<T>(HttpResponseMessage response)
        {
            if (response != null)
            {
                //response.EnsureSuccessStatusCode();

                string json = null;
                using var stream = await response.Content.ReadAsStreamAsync();

                // Gzip
                if (response.Content.Headers.ContentEncoding.Contains("gzip"))
                {
                    using (var readStream = new MemoryStream())
                    using (var gzipStream = new GZipStream(stream, CompressionMode.Decompress))
                    {
                        gzipStream.CopyTo(readStream);

                        var bytes = readStream.ToArray();

                        json = Encoding.ASCII.GetString(bytes);
                    }
                }
                else
                {
                    using (var streamReader = new StreamReader(stream))
                    {
                        json = streamReader.ReadToEnd();
                    }
                }

                if (!string.IsNullOrEmpty(json))
                {
                    return Json.Convert<T>(json);
                }
            }

            return default;
        }
    }
}
