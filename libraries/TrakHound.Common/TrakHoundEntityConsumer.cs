// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TrakHound.Entities;

namespace TrakHound
{
    public class TrakHoundEntityConsumer<TEntity> : ITrakHoundConsumer<ITrakHoundEntity> where TEntity : ITrakHoundEntity
    {
        private readonly string _id;
        private readonly CancellationToken _cancellationToken;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly List<ITrakHoundConsumer<TEntity>> _consumers = new List<ITrakHoundConsumer<TEntity>>();
        private readonly bool _allowNull;


        public string Id => _id;

        public CancellationToken CancellationToken => _cancellationToken;

        public ITrakHoundEntity InitialValue { get; set; }

        public event EventHandler<ITrakHoundEntity> Received;

        public event EventHandler<string> Disposed;

        public Func<ITrakHoundEntity, ITrakHoundEntity> OnReceived { get; set; }

        public Func<ITrakHoundEntity, Task<ITrakHoundEntity>> OnReceivedAsync { get; set; }

        public Action OnDisposed { get; set; }


        public TrakHoundEntityConsumer()
        {
            _id = Guid.NewGuid().ToString();
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;
        }

        public TrakHoundEntityConsumer(CancellationToken cancellationToken)
        {
            _id = Guid.NewGuid().ToString();
            _cancellationToken = cancellationToken;
        }

        public TrakHoundEntityConsumer(string consumerId)
        {
            _id = !string.IsNullOrEmpty(consumerId) ? consumerId : Guid.NewGuid().ToString();
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;
        }

        public TrakHoundEntityConsumer(string consumerId, CancellationToken cancellationToken)
        {
            _id = !string.IsNullOrEmpty(consumerId) ? consumerId : Guid.NewGuid().ToString();
            _cancellationToken = cancellationToken;
        }

        public TrakHoundEntityConsumer(ITrakHoundConsumer<TEntity> consumer, string consumerId = null)
        {
            _id = !string.IsNullOrEmpty(consumerId) ? consumerId : Guid.NewGuid().ToString();
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;

            if (consumer != null)
            {
                _consumers.Add(consumer);
                InitialValue = consumer.InitialValue;
                consumer.Received += Receive;
            }
        }

        public TrakHoundEntityConsumer(ITrakHoundConsumer<TEntity> consumer, CancellationToken cancellationToken, string consumerId = null)
        {
            _id = !string.IsNullOrEmpty(consumerId) ? consumerId : Guid.NewGuid().ToString();
            _cancellationToken = cancellationToken;

            if (consumer != null)
            {
                _consumers.Add(consumer);
                InitialValue = consumer.InitialValue;
                consumer.Received += Receive;
            }
        }

        public TrakHoundEntityConsumer(IEnumerable<ITrakHoundConsumer<TEntity>> consumers, string consumerId = null)
        {
            _id = !string.IsNullOrEmpty(consumerId) ? consumerId : Guid.NewGuid().ToString();
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;

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

        public TrakHoundEntityConsumer(IEnumerable<ITrakHoundConsumer<TEntity>> consumers, CancellationToken cancellationToken, string consumerId = null)
        {
            _id = !string.IsNullOrEmpty(consumerId) ? consumerId : Guid.NewGuid().ToString();
            _cancellationToken = cancellationToken;

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
            if (_cancellationTokenSource != null) _cancellationTokenSource.Cancel();

            if (OnDisposed != null) OnDisposed();

            if (!_consumers.IsNullOrEmpty())
            {
                foreach (var consumer in _consumers) consumer.Dispose();
            }

            if (Disposed != null) Disposed.Invoke(this, Id);
        }

        public virtual bool Push(ITrakHoundEntity item)
        {
            if (item != null && Received != null) Received.Invoke(this, item);

            return true;
        }


        private void Receive(object sender, TEntity item)
        {
            if (Received != null) Received.Invoke(sender, item);
        }
    }
}
