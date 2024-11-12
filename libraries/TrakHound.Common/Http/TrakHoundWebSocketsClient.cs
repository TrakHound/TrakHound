// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace TrakHound.Http
{
    public struct TrakHoundWebSocketResponse
    {
        public byte[] Content { get; set; }
    }


    public class TrakHoundWebSocketsClient : IDisposable
    {
        private readonly string _url;
        private readonly Stream _requestBody;
        private CancellationTokenSource _stop;
        private bool _started;
        private ClientWebSocket _client;


        public event EventHandler<TrakHoundWebSocketResponse> ResponseReceived;
        public event EventHandler Connected;
        public event EventHandler Disconnected;
        public event EventHandler<Exception> ConnectionError;


        public int ReconnectInterval { get; set; } = 10000;

        public IEnumerable<string> AcceptEncoding { get; set; }


        public TrakHoundWebSocketsClient(string url)
        {
            _url = url;
        }

        public TrakHoundWebSocketsClient(string url, object requestBody)
        {
            _url = url;
            _requestBody = GetRequestBodyStream(requestBody);
        }

        public TrakHoundWebSocketsClient(string url, byte[] requestBody)
        {
            _url = url;
            _requestBody = GetRequestBodyStream(requestBody);
        }

        public TrakHoundWebSocketsClient(string url, Stream requestBody)
        {
            _url = url;
            _requestBody = requestBody;
        }

        public void Dispose()
        {
            Stop();
            if (_client != null) _client.Dispose();
        }


        public void Start()
        {
            if (!_started)
            {
                _started = true;
                _stop = new CancellationTokenSource();
                _ = Task.Run(Worker, _stop.Token);
            }
        }

        public void Stop()
        {
            if (_started)
            {
                if (_stop != null) _stop.Cancel();

                try
                {
                    if (_client != null)
                    {
                        _client.CloseAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None);
                    }
                }
                catch { }

                _started = false;
            }
        }



        private async Task Worker()
        {
            do
            {
                try
                {
                    try
                    {
                        _client = new ClientWebSocket();

                        await _client.ConnectAsync(new Uri(_url), _stop.Token);

                        if (Connected != null) Connected.Invoke(this, new EventArgs());


                        // Send Request Body
                        if (_requestBody != null)
                        {
                            byte[] requestBodyBytes;
                            using (var requestBodyStream = new MemoryStream())
                            {
                                await _requestBody.CopyToAsync(requestBodyStream);

                                requestBodyBytes = requestBodyStream.ToArray();
                            }

                            await _client.SendAsync(requestBodyBytes, WebSocketMessageType.Binary, true, _stop.Token);
                        }



                        var initialBufferSize = 1024;
                        byte[] responseBuffer = new byte[initialBufferSize];
                        var bufferOffset = 0;
                        var dataPerPacket = 256;

                        while (_client.State == WebSocketState.Open)
                        {
                            WebSocketReceiveResult result = null;
                            bufferOffset = 0;
                            Array.Resize(ref responseBuffer, initialBufferSize);

                            while (_client.State == WebSocketState.Open)
                            {
                                var bytesReceived = new ArraySegment<byte>(responseBuffer, bufferOffset, dataPerPacket);
                                result = await _client.ReceiveAsync(bytesReceived, _stop.Token);
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
                            }

                            HandleResponse(result, responseBuffer);
                        }

                        _client.Dispose();
                    }
                    catch (TaskCanceledException) { }
                    catch (Exception ex)
                    {
                        if (ConnectionError != null) ConnectionError.Invoke(this, ex);
                    }

                    if (Disconnected != null) Disconnected.Invoke(this, new EventArgs());

                    await Task.Delay(ReconnectInterval, _stop.Token);
                }
                catch (TaskCanceledException) { }
                catch (Exception) { }

            } while (!_stop.Token.IsCancellationRequested);
        }

        private void HandleResponse(WebSocketReceiveResult result, byte[] responseBytes)
        {
            if (result != null && !responseBytes.IsNullOrEmpty())
            {
                try
                {
                    if (AcceptEncoding != null && AcceptEncoding.Contains("gzip"))
                    {
                        using (var inputStream = new MemoryStream(responseBytes))
                        using (var outputStream = new MemoryStream())
                        using (var gzipStream = new GZipStream(inputStream, CompressionMode.Decompress))
                        {
                            gzipStream.CopyTo(outputStream);

                            var bytes = outputStream.ToArray();
                            if (!bytes.IsNullOrEmpty())
                            {
                                var response = new TrakHoundWebSocketResponse();
                                response.Content = bytes;

                                if (ResponseReceived != null) ResponseReceived.Invoke(this, response);
                            }
                        }
                    }
                    else
                    {
                        if (!responseBytes.IsNullOrEmpty())
                        {
                            var response = new TrakHoundWebSocketResponse();
                            response.Content = responseBytes;

                            if (ResponseReceived != null) ResponseReceived.Invoke(this, response);
                        }
                    }
                }
                catch { }
            }
        }


        private static Stream GetRequestBodyStream(object requestBody)
        {
            if (requestBody != null)
            {
                var jsonRequest = Json.Convert(requestBody);
                if (!string.IsNullOrEmpty(jsonRequest))
                {
                    return GetRequestBodyStream(jsonRequest.ToUtf8Bytes());
                }
            }

            return null;
        }

        private static Stream GetRequestBodyStream(byte[] requestBody)
        {
            if (!requestBody.IsNullOrEmpty())
            {
                try
                {
                    var stream = new MemoryStream(requestBody);
                    stream.Seek(0, SeekOrigin.Begin);
                    return stream;
                }
                catch { }
            }

            return null;
        }
    }
}
