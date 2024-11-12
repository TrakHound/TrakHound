// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Threading;
using System.Threading.Tasks;
using TrakHound.Configurations;
using TrakHound.Entities.Collections;
using TrakHound.Http;

namespace TrakHound.Clients
{
    internal class TrakHoundEntityCollectionClientConsumer : ITrakHoundConsumer<TrakHoundEntityCollection>
    {
        private readonly string _url;
        private readonly string _requestString;
        private readonly object _requestObject;
        private readonly TrakHoundHttpClientConfiguration _clientConfiguration;
        private readonly TrakHoundWebSocketsClient _websocketClient;
        private readonly CancellationToken _cancellationToken;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private string _consumerId;
        private bool _disposed;


        public string Id => _consumerId;
        public CancellationToken CancellationToken => _cancellationToken;

        public TrakHoundEntityCollection InitialValue { get; set; }

        public Func<TrakHoundEntityCollection, TrakHoundEntityCollection> OnReceived { get; set; }

        public Func<TrakHoundEntityCollection, Task<TrakHoundEntityCollection>> OnReceivedAsync { get; set; }

        public Action OnDisposed { get; set; }


        public event EventHandler<string> Subscribed; // ConsumerId

        public event EventHandler<TrakHoundEntityCollection> Received;

        public event EventHandler<string> Disposed;


        public TrakHoundEntityCollectionClientConsumer(TrakHoundHttpClientConfiguration clientConfiguration, string url)
        {
            _consumerId = Guid.NewGuid().ToString();
            _clientConfiguration = clientConfiguration;
            _url = url;
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;

            _websocketClient = new TrakHoundWebSocketsClient(Url.Combine(clientConfiguration?.GetWebSocketBaseUrl(), _url));
            _websocketClient.ResponseReceived += WebsocketClientResponseReceived;
        }

        public TrakHoundEntityCollectionClientConsumer(TrakHoundHttpClientConfiguration clientConfiguration, string url, CancellationToken cancellationToken)
        {
            _consumerId = Guid.NewGuid().ToString();
            _url = url;
            _cancellationToken = cancellationToken;
            _clientConfiguration = clientConfiguration;

            _websocketClient = new TrakHoundWebSocketsClient(Url.Combine(clientConfiguration?.GetWebSocketBaseUrl(), _url));
            _websocketClient.ResponseReceived += WebsocketClientResponseReceived;
        }

        public TrakHoundEntityCollectionClientConsumer(TrakHoundHttpClientConfiguration clientConfiguration, string url, string requestBody)
        {
            _consumerId = Guid.NewGuid().ToString();
            _url = url;
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;
            _requestString = requestBody;
            _clientConfiguration = clientConfiguration;

            _websocketClient = new TrakHoundWebSocketsClient(Url.Combine(clientConfiguration?.GetWebSocketBaseUrl(), _url));
            _websocketClient.ResponseReceived += WebsocketClientResponseReceived;
        }

        public TrakHoundEntityCollectionClientConsumer(TrakHoundHttpClientConfiguration clientConfiguration, string url, CancellationToken cancellationToken, string requestBody)
        {
            _consumerId = Guid.NewGuid().ToString();
            _url = url;
            _cancellationToken = cancellationToken;
            _requestString = requestBody;
            _clientConfiguration = clientConfiguration;

            _websocketClient = new TrakHoundWebSocketsClient(Url.Combine(clientConfiguration?.GetWebSocketBaseUrl(), _url));
            _websocketClient.ResponseReceived += WebsocketClientResponseReceived;
        }

        public TrakHoundEntityCollectionClientConsumer(TrakHoundHttpClientConfiguration clientConfiguration, string url, object requestBody)
        {
            _consumerId = Guid.NewGuid().ToString();
            _url = url;
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;
            _requestObject = requestBody;
            _clientConfiguration = clientConfiguration;

            _websocketClient = new TrakHoundWebSocketsClient(Url.Combine(clientConfiguration?.GetWebSocketBaseUrl(), _url));
            _websocketClient.ResponseReceived += WebsocketClientResponseReceived;
        }

        public TrakHoundEntityCollectionClientConsumer(TrakHoundHttpClientConfiguration clientConfiguration, string url, CancellationToken cancellationToken, object requestBody)
        {
            _consumerId = Guid.NewGuid().ToString();
            _url = url;
            _cancellationToken = cancellationToken;
            _requestObject = requestBody;
            _clientConfiguration = clientConfiguration;

            _websocketClient = new TrakHoundWebSocketsClient(Url.Combine(clientConfiguration?.GetWebSocketBaseUrl(), _url));
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
                var jsonResponse = Json.Convert<TrakHoundJsonEntityCollection>(json);
                if (jsonResponse != null) Push(jsonResponse.ToCollection());
            }
        }

        public virtual bool Push(TrakHoundEntityCollection item)
        {
            if (item != null && Received != null) Received.Invoke(this, item);

            return true;
        }
    }
}
