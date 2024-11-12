// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TrakHound.Clients
{
    internal class TrakHoundInstanceConsumer<TResult> : TrakHoundInstanceConsumer<TResult, TResult> { }

    internal class TrakHoundInstanceConsumer<TInput, TOutput> : ITrakHoundConsumer<TOutput>
    {
        private readonly string _id;
        private readonly CancellationToken _cancellationToken;
        private readonly List<ITrakHoundConsumer<TInput>> _consumers = new List<ITrakHoundConsumer<TInput>>();


        public string Id => _id;

        public CancellationToken CancellationToken => _cancellationToken;

        public TOutput InitialValue { get; set; }

        /// <summary>
        /// Event handler for when a new <typeparamref name="TOutput"/> is received from the stream
        /// </summary>
        public event EventHandler<TOutput> Received;

        public event EventHandler<string> Disposed;

        public Func<TInput, TOutput> OnProcess { get; set; }

        public Func<TOutput, TOutput> OnReceived { get; set; }

        public Func<TOutput, Task<TOutput>> OnReceivedAsync { get; set; }

        public Action OnDisposed { get; set; }


        public TrakHoundInstanceConsumer()
        {
            _id = Guid.NewGuid().ToString();
        }

        public TrakHoundInstanceConsumer(ITrakHoundConsumer<TInput> consumer)
        {
            _id = Guid.NewGuid().ToString();

            if (consumer != null)
            {
                _consumers.Add(consumer);
                consumer.Received += Receive;
            }
        }

        public TrakHoundInstanceConsumer(ITrakHoundConsumer<TInput> consumer, CancellationToken cancellationToken)
        {
            _id = Guid.NewGuid().ToString();
            _cancellationToken = cancellationToken;

            if (consumer != null)
            {
                _consumers.Add(consumer);
                consumer.Received += Receive;
            }
        }

        public TrakHoundInstanceConsumer(IEnumerable<ITrakHoundConsumer<TInput>> consumers)
        {
            _id = Guid.NewGuid().ToString();

            if (!consumers.IsNullOrEmpty())
            {
                foreach (var consumer in consumers)
                {
                    _consumers.Add(consumer);
                    consumer.Received += Receive;
                }
            }
        }

        public TrakHoundInstanceConsumer(IEnumerable<ITrakHoundConsumer<TInput>> consumers, CancellationToken cancellationToken)
        {
            _id = Guid.NewGuid().ToString();
            _cancellationToken = cancellationToken;

            if (!consumers.IsNullOrEmpty())
            {
                foreach (var consumer in consumers)
                {
                    _consumers.Add(consumer);
                    consumer.Received += Receive;
                }
            }
        }


        public void Dispose()
        {
            if (OnDisposed != null) OnDisposed();

            if (!_consumers.IsNullOrEmpty())
            {
                foreach (var consumer in _consumers) consumer.Dispose();
            }

            if (Disposed != null) Disposed.Invoke(this, Id);
        }

        public virtual bool Push(TOutput item)
        {
            if (Received != null) Received.Invoke(this, item);
            //if (item != null && Received != null) Received.Invoke(this, item);

            return true;
        }


        private void Receive(object sender, TInput item)
        {
            if (item != null && OnProcess != null)
            {
                var x = OnProcess(item);
                if (x != null && Received != null) Received.Invoke(sender, x);
            }
            else
            {
                if (Received != null) Received.Invoke(sender, default(TOutput));
            }
        }
    }
}
