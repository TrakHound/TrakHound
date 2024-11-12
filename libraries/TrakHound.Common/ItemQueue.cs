// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;

namespace TrakHound
{
    public class ItemQueue<T>
    {
        private readonly int _limit;
        private readonly Dictionary<string, T> _items = new Dictionary<string, T>();
        private readonly object _lock = new object();


        /// <summary>
        /// Gets the current number of Items in the Queue
        /// </summary>
        public long Count
        {
            get
            {
                lock (_lock) return _items.Count;
            }
        }


        public ItemQueue(int limit = int.MaxValue)
        {
            _limit = limit;
        }


        /// <summary>
        /// Take (n) number of Items and remove from the Queue
        /// </summary>
        public IEnumerable<T> Take(int count = 1)
        {
            lock (_lock)
            {
                var items = _items.Take(count);
                if (!items.IsNullOrEmpty())
                {
                    var x = new List<T>();

                    foreach (var item in items)
                    {
                        x.Add(item.Value);
                    }

                    // Remove Items from Queue
                    foreach (var item in items) _items.Remove(item.Key);

                    return x;
                }
            }

            return null;
        }

        public bool Add(T item)
        {
            if (item != null)
            {
                var key = Guid.NewGuid().ToString();

                lock (_lock)
                {
                    if (_items.Count > _limit) return false;

                    if (_items.TryGetValue(key, out var _))
                    {
                        _items.Remove(key);
                        _items.Add(key, item);
                        return true;
                    }
                    else
                    {
                        _items.Add(key, item);
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
