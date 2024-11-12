// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.IO;
using System.Threading;
using TrakHound.Clients;
using TrakHound.Messages;

namespace TrakHound.Http
{
    public class TrakHoundMessageClientConsumer : ITrakHoundConsumer<TrakHoundMessageResponse>
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

        public TrakHoundMessageResponse InitialValue { get; set; }

        public Action OnDisposed { get; set; }


        public event EventHandler<string> Subscribed; // ConsumerId

        public event EventHandler<TrakHoundMessageResponse> Received;

        public event EventHandler<string> Disposed;


        public TrakHoundMessageClientConsumer(TrakHoundHttpClientConfiguration clientConfiguration, string url)
        {
            _consumerId = Guid.NewGuid().ToString();
            _clientConfiguration = clientConfiguration;
            _url = url;
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;

            _websocketClient = new TrakHoundWebSocketsClient(Url.Combine(clientConfiguration?.GetWebSocketBaseUrl(), _url));
            _websocketClient.ResponseReceived += WebsocketClientResponseReceived;
        }

        public TrakHoundMessageClientConsumer(TrakHoundHttpClientConfiguration clientConfiguration, string url, CancellationToken cancellationToken)
        {
            _consumerId = Guid.NewGuid().ToString();
            _clientConfiguration = clientConfiguration;
            _url = url;
            _cancellationToken = cancellationToken;

            _websocketClient = new TrakHoundWebSocketsClient(Url.Combine(clientConfiguration?.GetWebSocketBaseUrl(), _url));
            _websocketClient.ResponseReceived += WebsocketClientResponseReceived;
        }

        public TrakHoundMessageClientConsumer(TrakHoundHttpClientConfiguration clientConfiguration, string url, object requestBody)
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

        public TrakHoundMessageClientConsumer(TrakHoundHttpClientConfiguration clientConfiguration, string url, CancellationToken cancellationToken, object requestBody)
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
                    var topicIndex = 0;

                    var i = 0;
                    while (i < response.Content.Length)
                    {
                        if (response.Content[i] == 10 && response.Content[i + 1] == 13)
                        {
                            topicIndex = i;
                            break;
                        }

                        i++;
                    }

                    if (topicIndex > 0)
                    {
                        var topicBytes = new byte[topicIndex];
                        Array.Copy(response.Content, 0, topicBytes, 0, topicIndex);
                        var topic = StringFunctions.GetUtf8String(topicBytes);
                        //Console.WriteLine(topic);

                        var contentLength = response.Content.Length - topicIndex - 2; // 2 = 10 & 13 bytes for CR+LF
                        var contentBytes = new byte[contentLength];
                        Array.Copy(response.Content, topicIndex + 2, contentBytes, 0, contentLength);
                        var content = StringFunctions.GetUtf8String(contentBytes);
                        //Console.WriteLine(content);

                        var messageResponse = new TrakHoundMessageResponse();

                        messageResponse.BrokerId = "DEBUG"; // DEBUG ONLY

                        messageResponse.Topic = topic;
                        messageResponse.Content = new MemoryStream(contentBytes);
                        messageResponse.Timestamp = UnixDateTime.Now;

                        Push(messageResponse);
                    }
                }
                catch { }
            }
        }

        public virtual bool Push(TrakHoundMessageResponse item)
        {
            if (Received != null) Received.Invoke(this, item);

            return true;
        }
    }
}
