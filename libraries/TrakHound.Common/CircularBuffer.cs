// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;

namespace TrakHound
{
    public class CircularBuffer<TKey, TValue>
    {
        private readonly Dictionary<TKey, CircularBuffer<TValue>> _buffers = new Dictionary<TKey, CircularBuffer<TValue>>();
        private readonly int _bufferLength;
        private readonly object _lock = new object();

        public CircularBuffer(int length)
        {
            _bufferLength = length;
        }

        public void Add(TKey key, TValue item)
        {
            if (key != null && item != null)
            {
                lock (_lock)
                {
                    var buffer = _buffers.GetValueOrDefault(key);
                    if (buffer != null)
                    {
                        buffer.Add(item);
                    }
                    else
                    {
                        buffer = new CircularBuffer<TValue>(_bufferLength);
                        buffer.Add(item);
                        _buffers.Add(key, buffer);
                    }
                }
            }
        }

        public TValue[] Get(TKey key)
        {
            if (key != null)
            {
                var buffer = _buffers.GetValueOrDefault(key);
                if (buffer != null)
                {
                    return buffer.Get();
                }
            }

            return default;
        }
    }

    public class CircularBuffer<TValue>
    {
        private readonly object _lock = new object();
        private readonly int _itemLimit;
        private readonly TValue[] _items;
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


        public CircularBuffer(int limit = 5000000)
        {
            _itemLimit = limit;
            _items = new TValue[limit];
        }


        public TValue[] Get()
        {
            lock (_lock)
            {
                var result = new TValue[_itemLimit];

                Array.Copy(_items, 0, result, 0, _itemLimit);

                return result;
            }
        }

        public void Add(TValue item)
        {
            if (item != null)
            {
                lock (_lock)
                {
                    Array.Copy(_items, 0, _items, 1, _itemLimit - 1);
                    _items[0] = item;
                }
            }
        }

        public void Clear()
        {
            lock (_lock)
            {
                for (var i = 0; i < _items.Length; i++)
                {
                    _items[i] = default;
                }
            }
        }
    }
}
