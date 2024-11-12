// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TrakHound
{
    public class TrakHoundConsumer<T> : ITrakHoundConsumer<T>
    {
        private readonly string _id;
        private readonly CancellationToken _cancellationToken;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly List<ITrakHoundConsumer<T>> _consumers = new List<ITrakHoundConsumer<T>>();
        private readonly bool _allowNull;
        private bool _disposed;


        public string Id => _id;

        public CancellationToken CancellationToken => _cancellationToken;

        public T InitialValue { get; set; }

        /// <summary>
        /// Event handler for when a new <typeparamref name="T"/> is received from the stream
        /// </summary>
        public event EventHandler<T> Received;

        public event EventHandler<string> Disposed;

        public Func<T, T> OnReceived { get; set; }

        public Func<T, Task<T>> OnReceivedAsync { get; set; }

        public Action OnDisposed { get; set; }


        public TrakHoundConsumer(bool allowNull = false)
        {
            _id = Guid.NewGuid().ToString();
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;
            _allowNull = allowNull;
        }

        public TrakHoundConsumer(CancellationToken cancellationToken, bool allowNull = false)
        {
            _id = Guid.NewGuid().ToString();
            _cancellationToken = cancellationToken;
            _allowNull = allowNull;
        }

        public TrakHoundConsumer(string consumerId, bool allowNull = false)
        {
            _id = !string.IsNullOrEmpty(consumerId) ? consumerId : Guid.NewGuid().ToString();
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;
            _allowNull = allowNull;
        }

        public TrakHoundConsumer(string consumerId, CancellationToken cancellationToken, bool allowNull = false)
        {
            _id = !string.IsNullOrEmpty(consumerId) ? consumerId : Guid.NewGuid().ToString();
            _cancellationToken = cancellationToken;
            _allowNull = allowNull;
        }

        public TrakHoundConsumer(ITrakHoundConsumer<T> consumer, string consumerId = null, bool allowNull = false)
        {
            _id = !string.IsNullOrEmpty(consumerId) ? consumerId : Guid.NewGuid().ToString();
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;
            _allowNull = allowNull;

            if (consumer != null)
            {
                _consumers.Add(consumer);
                InitialValue = consumer.InitialValue;
                consumer.Received += Receive;
            }
        }

        public TrakHoundConsumer(ITrakHoundConsumer<T> consumer, CancellationToken cancellationToken, string consumerId = null, bool allowNull = false)
        {
            _id = !string.IsNullOrEmpty(consumerId) ? consumerId : Guid.NewGuid().ToString();
            _cancellationToken = cancellationToken;
            _allowNull = allowNull;

            if (consumer != null)
            {
                _consumers.Add(consumer);
                InitialValue = consumer.InitialValue;
                consumer.Received += Receive;
            }
        }

        public TrakHoundConsumer(IEnumerable<ITrakHoundConsumer<T>> consumers, string consumerId = null, bool allowNull = false)
        {
            _id = !string.IsNullOrEmpty(consumerId) ? consumerId : Guid.NewGuid().ToString();
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;
            _allowNull = allowNull;

            if (!consumers.IsNullOrEmpty())
            {
                foreach (var consumer in consumers)
                {
                    _consumers.Add(consumer);
                    InitialValue = consumer.InitialValue;
                    consumer.Received += Receive;
                }
            }
        }

        public TrakHoundConsumer(IEnumerable<ITrakHoundConsumer<T>> consumers, CancellationToken cancellationToken, string consumerId = null, bool allowNull = false)
        {
            _id = !string.IsNullOrEmpty(consumerId) ? consumerId : Guid.NewGuid().ToString();
            _cancellationToken = cancellationToken;
            _allowNull = allowNull;

            if (!consumers.IsNullOrEmpty())
            {
                foreach (var consumer in consumers)
                {
                    _consumers.Add(consumer);
                    InitialValue = consumer.InitialValue;
                    consumer.Received += Receive;
                }
            }
        }


        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;

                if (_cancellationTokenSource != null) _cancellationTokenSource.Cancel();

                if (OnDisposed != null) OnDisposed();

                if (!_consumers.IsNullOrEmpty())
                {
                    foreach (var consumer in _consumers) consumer.Dispose();
                }

                if (Disposed != null) Disposed.Invoke(this, Id);
            }
        }

        public virtual bool Push(T item)
        {
            if (item != null && Received != null) Received.Invoke(this, item);

            return true;
        }


        private async void Receive(object sender, T item)
        {
            T x = item;

            if (OnReceived != null) x = OnReceived(item);
            if (OnReceivedAsync != null)
            {
                var task = OnReceivedAsync(item);
                if (task != null) x = await task;
            }

            if ((x != null || _allowNull) && Received != null) Received.Invoke(sender, item);
        }
    }


    public class TrakHoundConsumer<TInput, TOutput> : ITrakHoundConsumer<TInput, TOutput>
    {
        private readonly string _id;
        private readonly CancellationToken _cancellationToken;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly List<ITrakHoundConsumer<TInput>> _inputConsumers = new List<ITrakHoundConsumer<TInput>>();
        private readonly List<ITrakHoundConsumer<TInput, TOutput>> _inputOutputConsumers = new List<ITrakHoundConsumer<TInput, TOutput>>();
        private readonly bool _allowNull;
        private bool _disposed;


        public string Id => _id;

        public CancellationToken CancellationToken => _cancellationToken;

        public TOutput InitialValue { get; set; }

        /// <summary>
        /// Event handler for when a new <typeparamref name="T"/> is received from the stream
        /// </summary>
        public event EventHandler<TOutput> Received;

        public event EventHandler<string> Disposed;

        public Func<TInput, TOutput> OnReceived { get; set; }

        public Func<TInput, Task<TOutput>> OnReceivedAsync { get; set; }

        public Action OnDisposed { get; set; }


        public TrakHoundConsumer(bool allowNull = false)
        {
            _id = Guid.NewGuid().ToString();
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;
            _allowNull = allowNull;
        }

        public TrakHoundConsumer(CancellationToken cancellationToken, bool allowNull = false)
        {
            _id = Guid.NewGuid().ToString();
            _cancellationToken = cancellationToken;
            _allowNull = allowNull;
        }

        public TrakHoundConsumer(string consumerId, bool allowNull = false)
        {
            _id = !string.IsNullOrEmpty(consumerId) ? consumerId : Guid.NewGuid().ToString();
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;
            _allowNull = allowNull;
        }

        public TrakHoundConsumer(string consumerId, CancellationToken cancellationToken, bool allowNull = false)
        {
            _id = !string.IsNullOrEmpty(consumerId) ? consumerId : Guid.NewGuid().ToString();
            _cancellationToken = cancellationToken;
            _allowNull = allowNull;
        }

        public TrakHoundConsumer(ITrakHoundConsumer<TInput> consumer, string consumerId = null, bool allowNull = false, Func<TInput, TOutput> onReceived = null, Func<TInput, Task<TOutput>> onReceivedAsync = null)
        {
            _id = !string.IsNullOrEmpty(consumerId) ? consumerId : Guid.NewGuid().ToString();
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;
            _allowNull = allowNull;
            OnReceived = onReceived;
            OnReceivedAsync = onReceivedAsync;

            if (consumer != null)
            {
                _inputConsumers.Add(consumer);
                if (OnReceived != null) InitialValue = OnReceived(consumer.InitialValue);
                if (OnReceivedAsync != null) InitialValue = OnReceivedAsync(consumer.InitialValue).Result;
                consumer.Received += Receive;
            }
        }

        public TrakHoundConsumer(ITrakHoundConsumer<TInput> consumer, CancellationToken cancellationToken, string consumerId = null, bool allowNull = false, Func<TInput, TOutput> onReceived = null, Func<TInput, Task<TOutput>> onReceivedAsync = null)
        {
            _id = !string.IsNullOrEmpty(consumerId) ? consumerId : Guid.NewGuid().ToString();
            _cancellationToken = cancellationToken;
            _allowNull = allowNull;
            OnReceived = onReceived;
            OnReceivedAsync = onReceivedAsync;

            if (consumer != null)
            {
                _inputConsumers.Add(consumer);
                if (OnReceived != null) InitialValue = OnReceived(consumer.InitialValue);
                if (OnReceivedAsync != null) InitialValue = OnReceivedAsync(consumer.InitialValue).Result;
                consumer.Received += Receive;
            }
        }


        public TrakHoundConsumer(ITrakHoundConsumer<TInput, TOutput> consumer, string consumerId = null, bool allowNull = false)
        {
            _id = !string.IsNullOrEmpty(consumerId) ? consumerId : Guid.NewGuid().ToString();
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;
            _allowNull = allowNull;

            if (consumer != null)
            {
                _inputOutputConsumers.Add(consumer);
                InitialValue = consumer.InitialValue;
                consumer.Received += Receive;
            }
        }

        public TrakHoundConsumer(ITrakHoundConsumer<TInput, TOutput> consumer, CancellationToken cancellationToken, string consumerId = null, bool allowNull = false)
        {
            _id = !string.IsNullOrEmpty(consumerId) ? consumerId : Guid.NewGuid().ToString();
            _cancellationToken = cancellationToken;
            _allowNull = allowNull;

            if (consumer != null)
            {
                _inputOutputConsumers.Add(consumer);
                InitialValue = consumer.InitialValue;
                consumer.Received += Receive;
            }
        }

        public TrakHoundConsumer(IEnumerable<ITrakHoundConsumer<TInput>> consumers, string consumerId = null, bool allowNull = false, Func<TInput, TOutput> onReceived = null, Func<TInput, Task<TOutput>> onReceivedAsync = null)
        {
            _id = !string.IsNullOrEmpty(consumerId) ? consumerId : Guid.NewGuid().ToString();
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;
            _allowNull = allowNull;
            OnReceived = onReceived;
            OnReceivedAsync = onReceivedAsync;

            if (!consumers.IsNullOrEmpty())
            {
                foreach (var consumer in consumers)
                {
                    _inputConsumers.Add(consumer);
                    if (OnReceived != null) InitialValue = OnReceived(consumer.InitialValue);
                    if (OnReceivedAsync != null) InitialValue = OnReceivedAsync(consumer.InitialValue).Result;
                    consumer.Received += Receive;
                }
            }
        }

        public TrakHoundConsumer(IEnumerable<ITrakHoundConsumer<TInput>> consumers, CancellationToken cancellationToken, string consumerId = null, bool allowNull = false, Func<TInput, TOutput> onReceived = null, Func<TInput, Task<TOutput>> onReceivedAsync = null)
        {
            _id = !string.IsNullOrEmpty(consumerId) ? consumerId : Guid.NewGuid().ToString();
            _cancellationToken = cancellationToken;
            _allowNull = allowNull;
            OnReceived = onReceived;
            OnReceivedAsync = onReceivedAsync;

            if (!consumers.IsNullOrEmpty())
            {
                foreach (var consumer in consumers)
                {
                    _inputConsumers.Add(consumer);
                    if (OnReceived != null) InitialValue = OnReceived(consumer.InitialValue);
                    if (OnReceivedAsync != null) InitialValue = OnReceivedAsync(consumer.InitialValue).Result;
                    consumer.Received += Receive;
                }
            }
        }

        public TrakHoundConsumer(IEnumerable<ITrakHoundConsumer<TInput, TOutput>> consumers, string consumerId = null, bool allowNull = false)
        {
            _id = !string.IsNullOrEmpty(consumerId) ? consumerId : Guid.NewGuid().ToString();
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;
            _allowNull = allowNull;

            if (!consumers.IsNullOrEmpty())
            {
                foreach (var consumer in consumers)
                {
                    _inputOutputConsumers.Add(consumer);
                    InitialValue = consumer.InitialValue;
                    consumer.Received += Receive;
                }
            }
        }

        public TrakHoundConsumer(IEnumerable<ITrakHoundConsumer<TInput, TOutput>> consumers, CancellationToken cancellationToken, string consumerId = null, bool allowNull = false)
        {
            _id = !string.IsNullOrEmpty(consumerId) ? consumerId : Guid.NewGuid().ToString();
            _cancellationToken = cancellationToken;
            _allowNull = allowNull;

            if (!consumers.IsNullOrEmpty())
            {
                foreach (var consumer in consumers)
                {
                    _inputOutputConsumers.Add(consumer);
                    InitialValue = consumer.InitialValue;
                    consumer.Received += Receive;
                }
            }
        }


        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;

                if (_cancellationTokenSource != null) _cancellationTokenSource.Cancel();

                if (OnDisposed != null) OnDisposed();

                if (!_inputConsumers.IsNullOrEmpty())
                {
                    foreach (var consumer in _inputConsumers) consumer.Dispose();
                }

                if (!_inputOutputConsumers.IsNullOrEmpty())
                {
                    foreach (var consumer in _inputOutputConsumers) consumer.Dispose();
                }

                if (Disposed != null) Disposed.Invoke(this, Id);
            }
        }

        public virtual bool Push(TInput item)
        {
            Receive(item);

            return true;
        }

        public virtual bool Push(TOutput item)
        {
            if ((item != null || _allowNull) && Received != null) Received.Invoke(this, item);

            return true;
        }


        private void Receive(object sender, TInput item)
        {
            Receive(item);
        }

        private void Receive(object sender, TOutput item)
        {
            Receive(item);
        }

        private async void Receive(TInput item)
        {
            TOutput x = default;

            if (OnReceived != null) x = OnReceived(item);
            if (OnReceivedAsync != null) x = await OnReceivedAsync(item);

            if ((x != null || _allowNull) && Received != null) Received.Invoke(this, x);
        }

        private void Receive(TOutput item)
        {
            if (Received != null) Received.Invoke(this, item);
        }
    }
}
