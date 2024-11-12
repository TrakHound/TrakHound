// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;

namespace TrakHound
{
    public class TrakHoundQueueConsumer<T> : TrakHoundConsumer<IEnumerable<T>>
    {
        private readonly ITrakHoundConsumer<T> _internalConsumer;
        private readonly ITrakHoundConsumer<IEnumerable<T>> _internalListConsumer;
        private readonly ItemIntervalQueue<T> _queue;


        public TrakHoundQueueConsumer(string consumerId, int interval = 5000, int limit = 1000) : base(consumerId)
        {
            OnReceived = ProcessMessage;
            OnDisposed = DisposeInternal;

            _queue = new ItemIntervalQueue<T>(interval, limit);
            _queue.ItemsReceived += QueueItemsReceived;
        }

        public TrakHoundQueueConsumer(ITrakHoundConsumer<T> consumer, string consumerId = null, int interval = 5000, int limit = 1000) : base(consumerId)
        {
            if (consumer != null)
            {
                _internalConsumer = consumer;
                _internalConsumer.Received += InternalConsumerReceived;
                _internalConsumer.Disposed += InternalConsumerDisposed;
            }

            OnReceived = ProcessMessage;
            OnDisposed = DisposeInternal;

            _queue = new ItemIntervalQueue<T>(interval, limit);
            _queue.ItemsReceived += QueueItemsReceived;
        }

        public TrakHoundQueueConsumer(ITrakHoundConsumer<IEnumerable<T>> consumer, string consumerId = null, int interval = 5000, int limit = 1000) : base(consumerId)
        {
            if (consumer != null)
            {
                _internalListConsumer = consumer;
                _internalListConsumer.Received += InternalConsumerReceived;
                _internalListConsumer.Disposed += InternalConsumerDisposed;
            }

            OnReceived = ProcessMessage;
            OnDisposed = DisposeInternal;

            _queue = new ItemIntervalQueue<T>(interval, limit);
            _queue.ItemsReceived += QueueItemsReceived;
        }

        public virtual bool Push(T item)
        {
            if (item != null) _queue.Add(item);

            return true;
        }

        private void InternalConsumerReceived(object sender, T item)
        {
            if (item != null) _queue.Add(item);
        }

        private void InternalConsumerReceived(object sender, IEnumerable<T> items)
        {
            if (items != null) _queue.Add(items);
        }

        private void InternalConsumerDisposed(object sender, string e)
        {
            _queue.Dispose();
            Dispose();
        }

        private void DisposeInternal()
        {
            _queue.Dispose();
            if (_internalConsumer != null) _internalConsumer.Dispose();
            if (_internalListConsumer != null) _internalListConsumer.Dispose();
        }

        private IEnumerable<T> ProcessMessage(IEnumerable<T> items)
        {
            if (items != null)
            {
                _queue.Add(items);
            }

            return default;
        }

        private void QueueItemsReceived(object sender, IEnumerable<T> items)
        {
            if (!items.IsNullOrEmpty())
            {
                Push(items);
            }
        }
    }

    public class TrakHoundSingleQueueConsumer<TInput, TOutput> : TrakHoundConsumer<TInput, TOutput>
    {
        private readonly ItemIntervalQueue<TInput> _queue;


        public TrakHoundSingleQueueConsumer(string consumerId, int interval = 5000, int limit = 1000) : base(consumerId)
        {
            OnReceived = ProcessMessage;
            _queue = new ItemIntervalQueue<TInput>(interval, limit);
            _queue.ItemsReceived += QueueItemsReceived;
        }

        private TOutput ProcessMessage(TInput item)
        {
            if (item != null)
            {
                _queue.Add(item);
            }

            return default;
        }

        private void QueueItemsReceived(object sender, IEnumerable<TInput> items)
        {
            if (!items.IsNullOrEmpty())
            {
                foreach (var item in items)
                {
                    // Push to base Consumer
                    Push(item);
                }
            }
        }
    }

    public class TrakHoundQueueConsumer<TInput, TOutput> : TrakHoundConsumer<IEnumerable<TInput>, TOutput>
    {
        private readonly ItemIntervalQueue<TInput> _queue;

        public new Func<IEnumerable<TInput>, TOutput> OnReceived { get; set; }


        public TrakHoundQueueConsumer(IEnumerable<ITrakHoundConsumer<IEnumerable<TInput>>> consumers, int interval = 5000, int limit = 1000) : base(consumers)
        {
            base.OnReceived = ProcessMessage;
            _queue = new ItemIntervalQueue<TInput>(interval, limit);
            _queue.ItemsReceived += QueueItemsReceived;
        }

        public TrakHoundQueueConsumer(string consumerId, int interval = 5000, int limit = 1000) : base(consumerId)
        {
            base.OnReceived = ProcessMessage;
            _queue = new ItemIntervalQueue<TInput>(interval, limit);
            _queue.ItemsReceived += QueueItemsReceived;
        }

        private TOutput ProcessMessage(IEnumerable<TInput> items)
        {
            if (!items.IsNullOrEmpty())
            {
                _queue.Add(items);
            }

            return default;
        }

        private void QueueItemsReceived(object sender, IEnumerable<TInput> items)
        {
            if (!items.IsNullOrEmpty())
            {
                var outputItem = OnReceived(items);

                // Push to base Consumer
                Push(outputItem);
            }
        }
    }
}
