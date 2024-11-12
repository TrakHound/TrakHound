// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TrakHound.Entities;
using TrakHound.Entities.Collections;

namespace TrakHound
{
    public class TrakHoundEntityQueueConsumer<TEntity> : ITrakHoundConsumer<TrakHoundEntityCollection> where TEntity : ITrakHoundEntity
    {
        private readonly string _id;
        private readonly CancellationToken _cancellationToken;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly List<ITrakHoundConsumer<TEntity>> _consumers = new List<ITrakHoundConsumer<TEntity>>();
        private readonly List<ITrakHoundConsumer<IEnumerable<TEntity>>> _arrayConsumers = new List<ITrakHoundConsumer<IEnumerable<TEntity>>>();
        private readonly ItemIntervalQueue<TEntity> _queue;
        private readonly bool _allowNull;


        public string Id => _id;

        public CancellationToken CancellationToken => _cancellationToken;

        public TrakHoundEntityCollection InitialValue { get; set; }

        public event EventHandler<TrakHoundEntityCollection> Received;

        public event EventHandler<string> Disposed;

        public Func<TrakHoundEntityCollection, TrakHoundEntityCollection> OnReceived { get; set; }

        public Func<TrakHoundEntityCollection, Task<TrakHoundEntityCollection>> OnReceivedAsync { get; set; }

        public Action OnDisposed { get; set; }


        public TrakHoundEntityQueueConsumer(int interval = 1000, int take = 1000, bool allowNull = false)
        {
            _id = Guid.NewGuid().ToString();
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;
            _allowNull = allowNull;
            _queue = new ItemIntervalQueue<TEntity>(interval, take);
            _queue.ItemsReceived += QueueItemsReceived;
        }

        public TrakHoundEntityQueueConsumer(CancellationToken cancellationToken, int interval = 1000, int take = 1000, bool allowNull = false)
        {
            _id = Guid.NewGuid().ToString();
            _cancellationToken = cancellationToken;
            _allowNull = allowNull;
            _queue = new ItemIntervalQueue<TEntity>(interval, take);
            _queue.ItemsReceived += QueueItemsReceived;
        }

        public TrakHoundEntityQueueConsumer(string consumerId, int interval = 1000, int take = 1000, bool allowNull = false)
        {
            _id = !string.IsNullOrEmpty(consumerId) ? consumerId : Guid.NewGuid().ToString();
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;
            _allowNull = allowNull;
            _queue = new ItemIntervalQueue<TEntity>(interval, take);
            _queue.ItemsReceived += QueueItemsReceived;
        }

        public TrakHoundEntityQueueConsumer(string consumerId, CancellationToken cancellationToken, int interval = 1000, int take = 1000, bool allowNull = false)
        {
            _id = !string.IsNullOrEmpty(consumerId) ? consumerId : Guid.NewGuid().ToString();
            _cancellationToken = cancellationToken;
            _allowNull = allowNull;
            _queue = new ItemIntervalQueue<TEntity>(interval, take);
            _queue.ItemsReceived += QueueItemsReceived;
        }

        public TrakHoundEntityQueueConsumer(ITrakHoundConsumer<TEntity> consumer, string consumerId = null, int interval = 1000, int take = 1000, bool allowNull = false)
        {
            _id = !string.IsNullOrEmpty(consumerId) ? consumerId : Guid.NewGuid().ToString();
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;
            _allowNull = allowNull;
            _queue = new ItemIntervalQueue<TEntity>(interval, take);
            _queue.ItemsReceived += QueueItemsReceived;

            if (consumer != null)
            {
                _consumers.Add(consumer);
                consumer.Received += Receive;
            }
        }

        public TrakHoundEntityQueueConsumer(ITrakHoundConsumer<TEntity> consumer, CancellationToken cancellationToken, string consumerId = null, int interval = 1000, int take = 1000, bool allowNull = false)
        {
            _id = !string.IsNullOrEmpty(consumerId) ? consumerId : Guid.NewGuid().ToString();
            _cancellationToken = cancellationToken;
            _allowNull = allowNull;
            _queue = new ItemIntervalQueue<TEntity>(interval, take);
            _queue.ItemsReceived += QueueItemsReceived;

            if (consumer != null)
            {
                _consumers.Add(consumer);
                consumer.Received += Receive;
            }
        }

        public TrakHoundEntityQueueConsumer(IEnumerable<ITrakHoundConsumer<TEntity>> consumers, string consumerId = null, int interval = 1000, int take = 1000, bool allowNull = false)
        {
            _id = !string.IsNullOrEmpty(consumerId) ? consumerId : Guid.NewGuid().ToString();
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;
            _allowNull = allowNull;
            _queue = new ItemIntervalQueue<TEntity>(interval, take);
            _queue.ItemsReceived += QueueItemsReceived;

            if (!consumers.IsNullOrEmpty())
            {
                foreach (var consumer in consumers)
                {
                    _consumers.Add(consumer);
                    consumer.Received += Receive;
                }
            }
        }

        public TrakHoundEntityQueueConsumer(IEnumerable<ITrakHoundConsumer<TEntity>> consumers, CancellationToken cancellationToken, string consumerId = null, int interval = 1000, int take = 1000, bool allowNull = false)
        {
            _id = !string.IsNullOrEmpty(consumerId) ? consumerId : Guid.NewGuid().ToString();
            _cancellationToken = cancellationToken;
            _allowNull = allowNull;
            _queue = new ItemIntervalQueue<TEntity>(interval, take);
            _queue.ItemsReceived += QueueItemsReceived;

            if (!consumers.IsNullOrEmpty())
            {
                foreach (var consumer in consumers)
                {
                    _consumers.Add(consumer);
                    consumer.Received += Receive;
                }
            }
        }

        public TrakHoundEntityQueueConsumer(ITrakHoundConsumer<IEnumerable<TEntity>> consumer, string consumerId = null, int interval = 1000, int take = 1000, bool allowNull = false)
        {
            _id = !string.IsNullOrEmpty(consumerId) ? consumerId : Guid.NewGuid().ToString();
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;
            _allowNull = allowNull;
            _queue = new ItemIntervalQueue<TEntity>(interval, take);
            _queue.ItemsReceived += QueueItemsReceived;

            if (consumer != null)
            {
                _arrayConsumers.Add(consumer);
                consumer.Received += Receive;
            }
        }

        public TrakHoundEntityQueueConsumer(ITrakHoundConsumer<IEnumerable<TEntity>> consumer, CancellationToken cancellationToken, string consumerId = null, int interval = 1000, int take = 1000, bool allowNull = false)
        {
            _id = !string.IsNullOrEmpty(consumerId) ? consumerId : Guid.NewGuid().ToString();
            _cancellationToken = cancellationToken;
            _allowNull = allowNull;
            _queue = new ItemIntervalQueue<TEntity>(interval, take);
            _queue.ItemsReceived += QueueItemsReceived;

            if (consumer != null)
            {
                _arrayConsumers.Add(consumer);
                consumer.Received += Receive;
            }
        }

        public TrakHoundEntityQueueConsumer(IEnumerable<ITrakHoundConsumer<IEnumerable<TEntity>>> consumers, string consumerId = null, int interval = 1000, int take = 1000, bool allowNull = false)
        {
            _id = !string.IsNullOrEmpty(consumerId) ? consumerId : Guid.NewGuid().ToString();
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;
            _allowNull = allowNull;
            _queue = new ItemIntervalQueue<TEntity>(interval, take);
            _queue.ItemsReceived += QueueItemsReceived;

            if (!consumers.IsNullOrEmpty())
            {
                foreach (var consumer in consumers)
                {
                    _arrayConsumers.Add(consumer);
                    consumer.Received += Receive;
                }
            }
        }

        public TrakHoundEntityQueueConsumer(IEnumerable<ITrakHoundConsumer<IEnumerable<TEntity>>> consumers, CancellationToken cancellationToken, string consumerId = null, int interval = 1000, int take = 1000, bool allowNull = false)
        {
            _id = !string.IsNullOrEmpty(consumerId) ? consumerId : Guid.NewGuid().ToString();
            _cancellationToken = cancellationToken;
            _allowNull = allowNull;
            _queue = new ItemIntervalQueue<TEntity>(interval, take);
            _queue.ItemsReceived += QueueItemsReceived;

            if (!consumers.IsNullOrEmpty())
            {
                foreach (var consumer in consumers)
                {
                    _arrayConsumers.Add(consumer);
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

            if (!_arrayConsumers.IsNullOrEmpty())
            {
                foreach (var consumer in _arrayConsumers) consumer.Dispose();
            }

            if (Disposed != null) Disposed.Invoke(this, Id);
        }

        public virtual bool Push(TrakHoundEntityCollection item)
        {
            if (item != null && Received != null) Received.Invoke(this, item);

            return true;
        }


        private void Receive(object sender, TEntity item)
        {
            _queue.Add(item);
        }

        private void Receive(object sender, IEnumerable<TEntity> items)
        {
            _queue.Add(items);
        }

        private void QueueItemsReceived(object sender, IEnumerable<TEntity> items)
        {
            if (!items.IsNullOrEmpty())
            {
                var collection = new TrakHoundEntityCollection();
                foreach (var item in items) collection.Add<TEntity>(item);
                Push(collection);
            }
        }
    }
}
