// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using TrakHound.Logging;

namespace TrakHound.Http
{
    public class TrakHoundHttpWebSocketManager : IAsyncDisposable
    {
        private readonly ITrakHoundLogger _logger = new TrakHoundLogger<TrakHoundHttpWebSocketManager>();
        private readonly Dictionary<string, WebSocket> _webSockets = new Dictionary<string, WebSocket>();
        private readonly object _lock = new object();


        public struct FormatResponse
        {
            public string Header { get; set; }

            public byte[] Content { get; set; }


            public FormatResponse(string header, byte[] content)
            {
                Header = header;
                Content = content;
            }
        }


        public async ValueTask DisposeAsync()
        {
            IEnumerable<WebSocket> webSockets;
            lock (_lock) webSockets = _webSockets.Values;

            if (!webSockets.IsNullOrEmpty())
            {
                foreach (var webSocket in webSockets)
                {
                    try
                    {
                        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "TrakHound WebSocketManager Closed", CancellationToken.None);
                        webSocket.Dispose();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning($"Error Closing WebSocket Connection : {ex.Message}");
                    }
                }
            }

            lock (_lock) _webSockets.Clear();
        }


        public async Task Create<T>(HttpContext httpContext, Func<Task<ITrakHoundConsumer<T>>> getConsumer, Func<T, byte[]> formatResponse)
        {
            if (httpContext.WebSockets.IsWebSocketRequest)
            {
                _logger.LogDebug($"WebSocket Request Received : {httpContext.Connection.RemoteIpAddress} : {httpContext.Request.Path}{httpContext.Request.QueryString}");


                // Set Status Code to Successful
                httpContext.Response.StatusCode = StatusCodes.Status200OK;

                // Get Accept-Encodings Header (for response compression)
                var acceptEncodings = httpContext.Request.Headers.AcceptEncoding.Select(o => o);

                // Open WebSocket Client
                using (var webSocket = await httpContext.WebSockets.AcceptWebSocketAsync())
                {
                    _logger.LogDebug($"WebSocket Connected : {httpContext.Connection.RemoteIpAddress} : {httpContext.Request.Path}{httpContext.Request.QueryString}");

                    var consumer = await getConsumer();
                    if (consumer != null && !string.IsNullOrEmpty(consumer.Id))
                    {
                        // Add WebSocket
                        lock (_lock) _webSockets.Add(consumer.Id, webSocket);


                        try
                        {
                            // Write Consumer's Initial Value
                            if (consumer.InitialValue != null) await HandleResponse(webSocket, consumer.InitialValue, formatResponse, acceptEncodings);

                            // Subscribe to Consumer's Received Handler
                            consumer.Received += async (s, o) => await HandleResponse(webSocket, o, formatResponse, acceptEncodings);

                            // Listen for Close Response
                            var buffer = new byte[1]; // Nothing is written to the Buffer on a CloseStatus but can't be Null
                            var receiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                            while (!receiveResult.CloseStatus.HasValue)
                            {
                                receiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                            }

                            // Close WebSocket Client
                            await webSocket.CloseAsync(receiveResult.CloseStatus.Value, receiveResult.CloseStatusDescription, CancellationToken.None);
                        }
                        catch { }
                        finally
                        {
                            lock (_lock) _webSockets.Remove(consumer.Id);
                            consumer.Dispose();
                        }
                    }
                    else
                    {
                        httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                    }

                    _logger.LogDebug($"WebSocket Disconnected : {httpContext.Connection.RemoteIpAddress} : {httpContext.Request.Path}{httpContext.Request.QueryString}");
                }
            }
            else
            {
                httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }

        public async Task Create<T>(HttpContext httpContext, Func<Stream, Task<ITrakHoundConsumer<T>>> getConsumer, Func<T, byte[]> formatResponse)
        {
            if (httpContext.WebSockets.IsWebSocketRequest)
            {
                _logger.LogDebug($"WebSocket Request Received : {httpContext.Connection.RemoteIpAddress} : {httpContext.Request.Path}{httpContext.Request.QueryString}");


                // Set Status Code to Successful
                httpContext.Response.StatusCode = StatusCodes.Status200OK;

                // Get Accept-Encodings Header (for response compression)
                var acceptEncodings = httpContext.Request.Headers.AcceptEncoding.Select(o => o);

                // Open WebSocket Client
                using (var webSocket = await httpContext.WebSockets.AcceptWebSocketAsync())
                {
                    _logger.LogDebug($"WebSocket Connected : {httpContext.Connection.RemoteIpAddress} : {httpContext.Request.Path}{httpContext.Request.QueryString}");


                    // Read the Request Body
                    Stream requestBody = null;
                    var hasRequestBody = httpContext.Request.Query["requestBody"] == "true";
                    if (hasRequestBody)
                    {
                        requestBody = new MemoryStream();
                        var requestBodyBuffer = new byte[256];
                        WebSocketReceiveResult result = null;

                        while (webSocket.State == WebSocketState.Open)
                        {
                            var bytesReceived = new ArraySegment<byte>(requestBodyBuffer);
                            result = await webSocket.ReceiveAsync(bytesReceived, CancellationToken.None);

                            requestBody.Write(requestBodyBuffer, 0, result.Count);

                            if (result.EndOfMessage) break;
                        }

                        requestBody.Seek(0, SeekOrigin.Begin);
                    }


                    // Get the TrakHound Consumer
                    var consumer = await getConsumer(requestBody);

                    // Dispose of Request Body Stream
                    if (requestBody != null) requestBody.Dispose();

                    if (consumer != null && !string.IsNullOrEmpty(consumer.Id))
                    {
                        // Add WebSocket
                        lock (_lock) _webSockets.Add(consumer.Id, webSocket);

                        try
                        {
                            // Write Consumer's Initial Value
                            if (consumer.InitialValue != null) await HandleResponse(webSocket, consumer.InitialValue, formatResponse, acceptEncodings);

                            // Subscribe to Consumer's Received Handler
                            consumer.Received += async (s, o) => await HandleResponse(webSocket, o, formatResponse, acceptEncodings);

                            // Listen for Close Response
                            var buffer = new byte[1]; // Nothing is written to the Buffer on a CloseStatus but can't be Null
                            var receiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                            while (!receiveResult.CloseStatus.HasValue)
                            {
                                receiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                            }

                            // Close WebSocket Client
                            await webSocket.CloseAsync(receiveResult.CloseStatus.Value, receiveResult.CloseStatusDescription, CancellationToken.None);
                        }
                        catch { }
                        finally
                        {
                            lock (_lock) _webSockets.Remove(consumer.Id);
                            consumer.Dispose();
                        }
                    }
                    else
                    {
                        //httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                    }

                    _logger.LogDebug($"WebSocket Disconnected : {httpContext.Connection.RemoteIpAddress} : {httpContext.Request.Path}{httpContext.Request.QueryString}");
                }
            }
            else
            {
                httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }


        public async Task CreateClient<T>(HttpContext httpContext, TrakHoundConsumer<T> consumer, Func<byte[], T> formatResponse)
        {
            if (httpContext.WebSockets.IsWebSocketRequest)
            {
                _logger.LogDebug($"WebSocket Client Request Received : {httpContext.Connection.RemoteIpAddress} : {httpContext.Request.Path}{httpContext.Request.QueryString}");


                // Set Status Code to Successful
                httpContext.Response.StatusCode = StatusCodes.Status200OK;

                // Get Accept-Encodings Header (for response compression)
                var acceptEncodings = httpContext.Request.Headers.AcceptEncoding.Select(o => o);

                // Open WebSocket Client
                using (var webSocket = await httpContext.WebSockets.AcceptWebSocketAsync())
                {
                    _logger.LogDebug($"WebSocket Client Connected : {httpContext.Connection.RemoteIpAddress} : {httpContext.Request.Path}{httpContext.Request.QueryString}");

                    if (consumer != null && !string.IsNullOrEmpty(consumer.Id))
                    {
                        // Add WebSocket
                        lock (_lock) _webSockets.Add(consumer.Id, webSocket);


                        try
                        {
                            var initialBufferSize = 1024;
                            byte[] responseBuffer = new byte[initialBufferSize];
                            var bufferOffset = 0;
                            var dataPerPacket = 256;

                            WebSocketReceiveResult result = null;

                            do
                            {
                                bufferOffset = 0;
                                Array.Resize(ref responseBuffer, initialBufferSize);

                                do
                                {
                                    var bytesReceived = new ArraySegment<byte>(responseBuffer, bufferOffset, dataPerPacket);
                                    result = await webSocket.ReceiveAsync(bytesReceived, CancellationToken.None);
                                    bufferOffset += result.Count;

                                    if (result.EndOfMessage)
                                    {
                                        Array.Resize(ref responseBuffer, bufferOffset);
                                        break;
                                    }
                                    else
                                    {
                                        // Resize the response array if the next message won't fit in the buffer
                                        if (bufferOffset >= responseBuffer.Length - dataPerPacket)
                                        {
                                            Array.Resize(ref responseBuffer, responseBuffer.Length + dataPerPacket);
                                        }
                                    }
                                } while (!result.CloseStatus.HasValue) ;

                                    var response = HandleResponse(responseBuffer, formatResponse, acceptEncodings);
                                if (response != null)
                                {
                                    consumer.Push(response);
                                }

                            } while (!result.CloseStatus.HasValue);
                        }
                        catch { }
                        finally
                        {
                            lock (_lock) _webSockets.Remove(consumer.Id);
                            consumer.Dispose();
                        }
                    }
                    else
                    {
                        httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                    }

                    _logger.LogDebug($"WebSocket Client Disconnected : {httpContext.Connection.RemoteIpAddress} : {httpContext.Request.Path}{httpContext.Request.QueryString}");
                }
            }
            else
            {
                httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }


        private static async Task HandleResponse<T>(WebSocket webSocket, T message, Func<T, byte[]> formatResponse, IEnumerable<string> acceptEncodings)
        {
            try
            {
                var responseBytes = formatResponse(message);
                if (responseBytes != null)
                {
                    if (acceptEncodings.Contains("gzip"))
                    {
                        // Compressed
                        using (var outputStream = new MemoryStream())
                        {
                            using (var zip = new GZipStream(outputStream, CompressionMode.Compress, true))
                            {
                                zip.Write(responseBytes, 0, responseBytes.Length);
                            }
                            var compressedBytes = outputStream.ToArray();
                            await webSocket.SendAsync(compressedBytes, System.Net.WebSockets.WebSocketMessageType.Binary, true, CancellationToken.None);
                        }
                    }
                    else
                    {
                        // Uncompressed
                        await webSocket.SendAsync(responseBytes, System.Net.WebSockets.WebSocketMessageType.Binary, true, CancellationToken.None);
                    }
                }
            }
            catch { }
        }

        private static T HandleResponse<T>(byte[] responseBytes, Func<byte[], T> formatResponse, IEnumerable<string> acceptEncodings)
        {
            if (!responseBytes.IsNullOrEmpty())
            {
                try
                {
                    if (acceptEncodings != null && acceptEncodings.Contains("gzip"))
                    {
                        using (var inputStream = new MemoryStream(responseBytes))
                        using (var outputStream = new MemoryStream())
                        using (var gzipStream = new GZipStream(inputStream, CompressionMode.Decompress))
                        {
                            gzipStream.CopyTo(outputStream);

                            var bytes = outputStream.ToArray();
                            if (!bytes.IsNullOrEmpty())
                            {
                                return formatResponse(bytes);
                            }
                        }
                    }
                    else
                    {
                        if (!responseBytes.IsNullOrEmpty())
                        {
                            return formatResponse(responseBytes);
                        }
                    }
                }
                catch { }
            }

            return default;
        }
    }
}
