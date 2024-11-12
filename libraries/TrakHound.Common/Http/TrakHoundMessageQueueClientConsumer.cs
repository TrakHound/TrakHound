// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Threading;
using TrakHound.Clients;
using TrakHound.MessageQueues;

namespace TrakHound.Http
{
    public class TrakHoundMessageQueueClientConsumer : ITrakHoundConsumer<TrakHoundMessageQueueResponse>
    {
        private readonly string _url;
        private readonly object _requestBody;
        private readonly TrakHoundHttpClientConfiguration _clientConfiguration;
        private readonly TrakHoundWebSocketsClient _websocketClient;
        private readonly CancellationToken _cancellationToken;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private string _consumerId;
        private bool _disposed;


        public string Id => _consumerId;
        public CancellationToken CancellationToken => _cancellationToken;

        public TrakHoundMessageQueueResponse InitialValue { get; set; }

        public Action OnDisposed { get; set; }


        public event EventHandler<string> Subscribed; // ConsumerId

        public event EventHandler<TrakHoundMessageQueueResponse> Received;

        public event EventHandler<string> Disposed;


        public TrakHoundMessageQueueClientConsumer(TrakHoundHttpClientConfiguration clientConfiguration, string url)
        {
            _consumerId = Guid.NewGuid().ToString();
            _clientConfiguration = clientConfiguration;
            _url = url;
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;

            _websocketClient = new TrakHoundWebSocketsClient(Url.Combine(clientConfiguration?.GetWebSocketBaseUrl(), _url));
            _websocketClient.ResponseReceived += WebsocketClientResponseReceived;
        }

        public TrakHoundMessageQueueClientConsumer(TrakHoundHttpClientConfiguration clientConfiguration, string url, CancellationToken cancellationToken)
        {
            _consumerId = Guid.NewGuid().ToString();
            _clientConfiguration = clientConfiguration;
            _url = url;
            _cancellationToken = cancellationToken;

            _websocketClient = new TrakHoundWebSocketsClient(Url.Combine(clientConfiguration?.GetWebSocketBaseUrl(), _url));
            _websocketClient.ResponseReceived += WebsocketClientResponseReceived;
        }

        public TrakHoundMessageQueueClientConsumer(TrakHoundHttpClientConfiguration clientConfiguration, string url, object requestBody)
        {
            _consumerId = Guid.NewGuid().ToString();
            _clientConfiguration = clientConfiguration;
            _url = url;
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;
            _requestBody = requestBody;

            _websocketClient = new TrakHoundWebSocketsClient(Url.Combine(clientConfiguration?.GetWebSocketBaseUrl(), _url), _requestBody);
            _websocketClient.ResponseReceived += WebsocketClientResponseReceived;
        }

        public TrakHoundMessageQueueClientConsumer(TrakHoundHttpClientConfiguration clientConfiguration, string url, CancellationToken cancellationToken, object requestBody)
        {
            _consumerId = Guid.NewGuid().ToString();
            _clientConfiguration = clientConfiguration;
            _url = url;
            _cancellationToken = cancellationToken;
            _requestBody = requestBody;

            _websocketClient = new TrakHoundWebSocketsClient(Url.Combine(clientConfiguration?.GetWebSocketBaseUrl(), _url), _requestBody);
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
                try
                {
                    var deliveryIdIndex = 0;

                    var i = 0;
                    while (i < response.Content.Length)
                    {
                        if (response.Content[i] == 10 && response.Content[i + 1] == 13)
                        {
                            deliveryIdIndex = i;
                            break;
                        }

                        i++;
                    }

                    if (deliveryIdIndex > 0)
                    {
                        var deliveryIdBytes = new byte[deliveryIdIndex];
                        Array.Copy(response.Content, 0, deliveryIdBytes, 0, deliveryIdIndex);
                        var deliveryId = StringFunctions.GetUtf8String(deliveryIdBytes);
                        //Console.WriteLine(deliveryId);

                        var contentLength = response.Content.Length - deliveryIdIndex - 2; // 2 = 10 & 13 bytes for CR+LF
                        var contentBytes = new byte[contentLength];
                        Array.Copy(response.Content, deliveryIdIndex + 2, contentBytes, 0, contentLength);
                        var content = StringFunctions.GetUtf8String(contentBytes);
                        //Console.WriteLine(content);
                    }
                }
                catch { }
            }
        }

        public virtual bool Push(TrakHoundMessageQueueResponse response)
        {
            if (Received != null) Received.Invoke(this, response);

            return true;
        }
    }
}
