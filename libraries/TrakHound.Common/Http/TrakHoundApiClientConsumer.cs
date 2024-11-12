// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using TrakHound.Api;
using TrakHound.Http;

namespace TrakHound.Clients
{
    public class TrakHoundApiClientConsumer : ITrakHoundConsumer<TrakHoundApiResponse>
    {
        private readonly string _url;
        private readonly string _requestContentType;
        private readonly Stream _requestBody;
        private readonly TrakHoundHttpClientConfiguration _clientConfiguration;
        private readonly TrakHoundWebSocketsClient _websocketClient;
        private readonly CancellationToken _cancellationToken;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private string _consumerId;
        private bool _disposed;


        public string Id => _consumerId;

        public CancellationToken CancellationToken => _cancellationToken;

        public TrakHoundApiResponse InitialValue { get; set; }

        public Func<TrakHoundApiResponse, TrakHoundApiResponse> OnReceived { get; set; }

        public Func<TrakHoundApiResponse, Task<TrakHoundApiResponse>> OnReceivedAsync { get; set; }

        public Action OnDisposed { get; set; }


        public event EventHandler<TrakHoundApiResponse> Received;

        public event EventHandler<string> Disposed;


        public TrakHoundApiClientConsumer(TrakHoundHttpClientConfiguration clientConfiguration, string url)
        {
            _consumerId = Guid.NewGuid().ToString();
            _clientConfiguration = clientConfiguration;
            _url = url;
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;

            _websocketClient = new TrakHoundWebSocketsClient(Url.Combine(clientConfiguration?.GetWebSocketBaseUrl(), _url));
            _websocketClient.ResponseReceived += WebsocketClientResponseReceived;
        }

        public TrakHoundApiClientConsumer(TrakHoundHttpClientConfiguration clientConfiguration, string url, CancellationToken cancellationToken)
        {
            _consumerId = Guid.NewGuid().ToString();
            _clientConfiguration = clientConfiguration;
            _url = url;
            _cancellationToken = cancellationToken;

            _websocketClient = new TrakHoundWebSocketsClient(Url.Combine(clientConfiguration?.GetWebSocketBaseUrl(), _url));
            _websocketClient.ResponseReceived += WebsocketClientResponseReceived;
        }

        public TrakHoundApiClientConsumer(TrakHoundHttpClientConfiguration clientConfiguration, string url, Dictionary<string, string> queryParameters)
        {
            _consumerId = Guid.NewGuid().ToString();
            _clientConfiguration = clientConfiguration;
            _url = url;
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;

            _websocketClient = new TrakHoundWebSocketsClient(Url.Combine(clientConfiguration?.GetWebSocketBaseUrl(), _url));
            _websocketClient.ResponseReceived += WebsocketClientResponseReceived;
        }

        public TrakHoundApiClientConsumer(TrakHoundHttpClientConfiguration clientConfiguration, string url, CancellationToken cancellationToken, Dictionary<string, string> queryParameters)
        {
            _consumerId = Guid.NewGuid().ToString();
            _clientConfiguration = clientConfiguration;
            _url = url;
            _cancellationToken = cancellationToken;

            _websocketClient = new TrakHoundWebSocketsClient(Url.Combine(clientConfiguration?.GetWebSocketBaseUrl(), _url));
            _websocketClient.ResponseReceived += WebsocketClientResponseReceived;
        }

        public TrakHoundApiClientConsumer(TrakHoundHttpClientConfiguration clientConfiguration, string url, object requestBody, string contentType = "application/json", Dictionary<string, string> queryParameters = null)
        {
            _consumerId = Guid.NewGuid().ToString();
            _clientConfiguration = clientConfiguration;
            _url = url;
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;
            _requestBody = TrakHoundApiResponse.GetJsonContentStream(requestBody);
            _requestContentType = contentType;

            var wsUrl = Url.Combine(clientConfiguration?.GetWebSocketBaseUrl(), _url);
            wsUrl = Url.AddQueryParameter(wsUrl, "requestBody", "true");

            _websocketClient = new TrakHoundWebSocketsClient(wsUrl, _requestBody);
            _websocketClient.ResponseReceived += WebsocketClientResponseReceived;
        }

        public TrakHoundApiClientConsumer(TrakHoundHttpClientConfiguration clientConfiguration, string url, CancellationToken cancellationToken, object requestBody, string contentType = "application/json", Dictionary<string, string> queryParameters = null)
        {
            _consumerId = Guid.NewGuid().ToString();
            _clientConfiguration = clientConfiguration;
            _url = url;
            _cancellationToken = cancellationToken;
            _requestBody = TrakHoundApiResponse.GetJsonContentStream(requestBody);
            _requestContentType = contentType;

            var wsUrl = Url.Combine(clientConfiguration?.GetWebSocketBaseUrl(), _url);
            wsUrl = Url.AddQueryParameter(wsUrl, "requestBody", "true");

            _websocketClient = new TrakHoundWebSocketsClient(wsUrl, _requestBody);
            _websocketClient.ResponseReceived += WebsocketClientResponseReceived;
        }

        public TrakHoundApiClientConsumer(TrakHoundHttpClientConfiguration clientConfiguration, string url, byte[] requestBody, string contentType = "application/octet-stream", Dictionary<string, string> queryParameters = null)
        {
            _consumerId = Guid.NewGuid().ToString();
            _clientConfiguration = clientConfiguration;
            _url = url;
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;
            _requestBody = TrakHoundApiResponse.GetContentStream(requestBody);
            _requestContentType = contentType;

            var wsUrl = Url.Combine(clientConfiguration?.GetWebSocketBaseUrl(), _url);
            wsUrl = Url.AddQueryParameter(wsUrl, "requestBody", "true");

            _websocketClient = new TrakHoundWebSocketsClient(wsUrl, _requestBody);
            _websocketClient.ResponseReceived += WebsocketClientResponseReceived;
        }

        public TrakHoundApiClientConsumer(TrakHoundHttpClientConfiguration clientConfiguration, string url, CancellationToken cancellationToken, byte[] requestBody, string contentType = "application/octet-stream", Dictionary<string, string> queryParameters = null)
        {
            _consumerId = Guid.NewGuid().ToString();
            _clientConfiguration = clientConfiguration;
            _url = url;
            _cancellationToken = cancellationToken;
            _requestBody = TrakHoundApiResponse.GetContentStream(requestBody);
            _requestContentType = contentType;

            var wsUrl = Url.Combine(clientConfiguration?.GetWebSocketBaseUrl(), _url);
            wsUrl = Url.AddQueryParameter(wsUrl, "requestBody", "true");

            _websocketClient = new TrakHoundWebSocketsClient(wsUrl, _requestBody);
            _websocketClient.ResponseReceived += WebsocketClientResponseReceived;
        }

        public TrakHoundApiClientConsumer(TrakHoundHttpClientConfiguration clientConfiguration, string url, Stream requestBody, string contentType = "application/octet-stream", Dictionary<string, string> queryParameters = null)
        {
            _consumerId = Guid.NewGuid().ToString();
            _clientConfiguration = clientConfiguration;
            _url = url;
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;
            _requestBody = requestBody;
            _requestContentType = contentType;

            var wsUrl = Url.Combine(clientConfiguration?.GetWebSocketBaseUrl(), _url);
            wsUrl = Url.AddQueryParameter(wsUrl, "requestBody", "true");

            _websocketClient = new TrakHoundWebSocketsClient(wsUrl, _requestBody);
            _websocketClient.ResponseReceived += WebsocketClientResponseReceived;
        }

        public TrakHoundApiClientConsumer(TrakHoundHttpClientConfiguration clientConfiguration, string url, CancellationToken cancellationToken, Stream requestBody, string contentType = "application/octet-stream", Dictionary<string, string> queryParameters = null)
        {
            _consumerId = Guid.NewGuid().ToString();
            _url = url;
            _cancellationToken = cancellationToken;
            _clientConfiguration = clientConfiguration;
            _requestBody = requestBody;
            _requestContentType = contentType;

            var wsUrl = Url.Combine(clientConfiguration?.GetWebSocketBaseUrl(), _url);
            wsUrl = Url.AddQueryParameter(wsUrl, "requestBody", "true");

            _websocketClient = new TrakHoundWebSocketsClient(wsUrl, _requestBody);
            _websocketClient.ResponseReceived += WebsocketClientResponseReceived;
        }

        public void Dispose()
        {
            _disposed = true;

            if (_cancellationTokenSource != null) _cancellationTokenSource.Cancel();

            if (_websocketClient != null) _websocketClient.Dispose();

            if (OnDisposed != null) OnDisposed();

            if (Disposed != null) Disposed.Invoke(this, Id);
        }


        public void Subscribe()
        {
            _websocketClient.Start();
        }

        private void WebsocketClientResponseReceived(object sender, TrakHoundWebSocketResponse response)
        {
            if (response.Content != null)
            {
                var json = System.Text.Encoding.UTF8.GetString(response.Content);
                var jsonResponse = Json.Convert<TrakHoundApiJsonResponse>(json);

                Push(jsonResponse.ToResponse());
            }
        }

        public virtual bool Push(TrakHoundApiResponse item)
        {
            if (item.IsValid())
            {
                if (Received != null)
                {
                    Received.Invoke(this, item);
                }
                else
                {
                    InitialValue = item;
                }
            }
            
            return true;
        }
    }
}
