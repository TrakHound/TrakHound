// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;

namespace TrakHound.Buffers
{
    class OperationItemQueue<TItem> : IDisposable
    {
        private const int _accessInterval = 30000;

        private readonly object _lock = new object();
        private readonly int _itemLimit;
        private readonly System.Timers.Timer _accessTimer;
        private TItem[] _items;
        private int _itemIndex;
        private int _itemCount = 0;
        private long _lastAccessed;


        public int Limit => _itemLimit;

        /// <summary>
        /// Gets the current number of Items in the Queue
        /// </summary>
        public int Count
        {
            get
            {
                lock (_lock) return _itemCount;
            }
        }


        public OperationItemQueue(int limit = 5000000)
        {
            _itemLimit = limit;
            _itemIndex = 0;

            //_accessTimer = new System.Timers.Timer();
            //_accessTimer.Interval = _accessInterval;
            //_accessTimer.Elapsed += AccessTimerElapsed;
            //_accessTimer.Start();
        }

        public void Dispose()
        {
            if (_accessTimer != null) _accessTimer.Dispose();
        }

        private void AccessTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            lock (_lock)
            {
                if (_itemCount == 0 && _items != null)
                {
                    _items = null;
                    //GC.Collect();
                }
            }
        }


        /// <summary>
        /// Take (n) number of TItems and remove them from queue
        /// </summary>
        public int Take(ref TItem[] buffer, int bufferIndex, int count)
        {
            lock (_lock)
            {
                if (_itemCount > 0 && _items != null && bufferIndex >= 0 && bufferIndex < buffer.Length && count > 0)
                {
                    var take = count < _itemCount ? count : _itemCount;

                    Array.Copy(_items, 0, buffer, bufferIndex, take);

                    var remaining = _itemCount - take;

                    if (remaining > 0)
                    {
                        Array.Copy(_items, take, _items, 0, remaining);
                    }

                    _itemIndex -= take;
                    _itemCount -= take;

                    _lastAccessed = UnixDateTime.Now;

                    return take;
                }
            }

            return 0;
        }

        /// <summary>
        /// Get (n) Items from Queue without removing them
        /// </summary>
        public int Get(ref TItem[] buffer, int bufferIndex, int count)
        {
            lock (_lock)
            {
                if (_itemCount > 0 && _items != null && bufferIndex >= 0 && bufferIndex < buffer.Length && count > 0)
                {
                    var take = count < _itemCount ? count : _itemCount;

                    Array.Copy(_items, 0, buffer, bufferIndex, take);

                    return take;
                }
            }

            return 0;
        }

        /// <summary>
        /// Removes (n) Items from top of Queue
        /// </summary>
        public int Remove(int count)
        {
            lock (_lock)
            {
                if (_itemCount > 0 && _items != null && count > 0)
                {
                    var take = count < _itemCount ? count : _itemCount;
                    var remaining = _itemCount - take;

                    if (remaining > 0)
                    {
                        Array.Copy(_items, take, _items, 0, remaining);
                    }

                    _itemIndex -= take;
                    _itemCount -= take;

                    return take;
                }
            }

            return 0;
        }

        public bool Add(TItem item)
        {
            if (item != null)
            {
                lock (_lock)
                {
                    if (_itemIndex < _itemLimit)
                    {
                        if (_items == null) _items = new TItem[_itemLimit];

                        _items[_itemIndex] = item;
                        _itemIndex++;
                        _itemCount++;

                        return true;
                    }
                }
            }

            return false;
        }

        public bool Add(IEnumerable<TItem> items)
        {
            if (!items.IsNullOrEmpty())
            {
                lock (_lock)
                {
                    var remainingSize = _itemLimit - _itemCount;
                    if (items.Count() <= remainingSize)
                    {
                        if (_items == null) _items = new TItem[_itemLimit];

                        foreach (var item in items)
                        {
                            _items[_itemIndex] = item;
                            _itemIndex++;
                        }

                        return true;
                    }
                }
            }

            return false;
        }
    }
}
