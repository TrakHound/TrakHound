// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;

namespace TrakHound.Buffers
{
    class OperationItemQueue<TItem>
    {
        private readonly object _lock = new object();
        private readonly int _itemLimit;
        private readonly TItem[] _items;
        private int _itemIndex;
        private int _itemCount = 0;


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
            _items = new TItem[limit];
            _itemIndex = 0;
        }


        /// <summary>
        /// Take (n) number of TItems and remove them from queue
        /// </summary>
        public int Take(ref TItem[] buffer, int bufferIndex, int count)
        {
            lock (_lock)
            {
                if (_itemCount > 0 && bufferIndex >= 0 && bufferIndex < buffer.Length && count > 0)
                {
                    var take = count < _itemCount ? count : _itemCount;

                    Array.Copy(_items, 0, buffer, bufferIndex, take);

                    var remaining = _itemCount - take;

                    if (remaining > 0)
                    {
                        Array.Copy(_items, take, _items, 0, remaining);
                        //Array.Fill()
                    }

                    _itemIndex -= take;
                    _itemCount -= take;

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
                if (_itemCount > 0 && bufferIndex >= 0 && bufferIndex < buffer.Length && count > 0)
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
                if (_itemCount > 0 && count > 0)
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
