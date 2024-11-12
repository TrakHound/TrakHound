// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TrakHound
{
    public class ItemIntervalQueue<T> : IDisposable
    {
        private static readonly object _lock = new object();
        private readonly ItemQueue<T> _queue;
        private readonly int _interval;
        private readonly int _take;
        private CancellationTokenSource stop;
        private bool _isStarted;


        public EventHandler<IEnumerable<T>> ItemsReceived { get; set; }


        public ItemIntervalQueue(int interval, int take = 100, int limit = int.MaxValue)
        {
            _queue = new ItemQueue<T>(limit);
            _interval = Math.Max(1, interval);
            _take = take > 0 ? take : int.MaxValue;
            Start();
        }

        public void Add(T item)
        {
            _queue.Add(item);
        }

        public void Add(IEnumerable<T> items)
        {
            if (!items.IsNullOrEmpty())
            {
                foreach (var item in items) _queue.Add(item);
            }
        }

        public void Dispose()
        {
            Stop();
        }


        private void Start()
        {
            if (!_isStarted)
            {
                _isStarted = true;

                stop = new CancellationTokenSource();

                _ = Task.Run(() => Worker(stop.Token));
            }
        }

        private void Stop()
        {
            if (_isStarted)
            {
                if (stop != null) stop.Cancel();
                _isStarted = false;
            }
        }


        private async void Worker(CancellationToken cancellationToken)
        {
            try
            {
                do
                {
                    var items = _queue.Take(_take);
                    if (!items.IsNullOrEmpty())
                    {
                        if (ItemsReceived != null) ItemsReceived.Invoke(this, items);
                    }

                    await Task.Delay(_interval, cancellationToken);

                } while (!cancellationToken.IsCancellationRequested);
            }
            catch (TaskCanceledException) { }
            catch (Exception) { }
        }
    }
}
