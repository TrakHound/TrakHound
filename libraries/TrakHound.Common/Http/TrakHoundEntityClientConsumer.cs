// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Threading;
using System.Threading.Tasks;
using TrakHound.Entities;
using TrakHound.Http;

namespace TrakHound.Clients
{
    internal class TrakHoundEntityClientConsumer<TEntity> : ITrakHoundConsumer<TEntity> where TEntity : ITrakHoundEntity
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

        public TEntity InitialValue { get; set; }

        public Func<TEntity, TEntity> OnReceived { get; set; }

        public Func<TEntity, Task<TEntity>> OnReceivedAsync { get; set; }

        public Action OnDisposed { get; set; }


        public event EventHandler<string> Subscribed; // ConsumerId

        public event EventHandler<TEntity> Received;

        public event EventHandler<string> Disposed;


        public TrakHoundEntityClientConsumer(TrakHoundHttpClientConfiguration clientConfiguration, string url)
        {
            _consumerId = Guid.NewGuid().ToString();
            _clientConfiguration = clientConfiguration;
            _url = url;
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;

            _websocketClient = new TrakHoundWebSocketsClient(Url.Combine(clientConfiguration?.GetWebSocketBaseUrl(), _url));
            _websocketClient.ResponseReceived += WebsocketClientResponseReceived;
        }

        public TrakHoundEntityClientConsumer(TrakHoundHttpClientConfiguration clientConfiguration, string url, CancellationToken cancellationToken)
        {
            _consumerId = Guid.NewGuid().ToString();
            _clientConfiguration = clientConfiguration;
            _url = url;
            _cancellationToken = cancellationToken;

            _websocketClient = new TrakHoundWebSocketsClient(Url.Combine(clientConfiguration?.GetWebSocketBaseUrl(), _url));
            _websocketClient.ResponseReceived += WebsocketClientResponseReceived;
        }

        public TrakHoundEntityClientConsumer(TrakHoundHttpClientConfiguration clientConfiguration, string url, object requestBody)
        {
            _consumerId = Guid.NewGuid().ToString();
            _clientConfiguration = clientConfiguration;
            _url = url;
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;
            _requestBody = requestBody;

            _websocketClient = new TrakHoundWebSocketsClient(Url.Combine(clientConfiguration?.GetWebSocketBaseUrl(), _url));
            _websocketClient.ResponseReceived += WebsocketClientResponseReceived;
        }

        public TrakHoundEntityClientConsumer(TrakHoundHttpClientConfiguration clientConfiguration, string url, CancellationToken cancellationToken, object requestBody)
        {
            _consumerId = Guid.NewGuid().ToString();
            _clientConfiguration = clientConfiguration;
            _url = url;
            _cancellationToken = cancellationToken;
            _requestBody = requestBody;

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
                var jsonResponse = Json.Convert<object[]>(json);
                if (jsonResponse != null) Push(TrakHoundEntity.FromArray<TEntity>(jsonResponse));
            }
        }

        public virtual bool Push(TEntity item)
        {
            if (item != null && Received != null) Received.Invoke(this, item);

            return true;
        }
    }
}
