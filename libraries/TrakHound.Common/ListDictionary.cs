// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;

namespace TrakHound
{
    public class ListDictionary<TKey, TValue>
    {
        private readonly Dictionary<TKey, HashSet<TValue>> _dictionary = new Dictionary<TKey, HashSet<TValue>>();
        private readonly object _lock = new object();
        private int _count = 0;


        public int Count
        {
            get
            {
                lock (_lock) return _count;
            }
        }

        public IEnumerable<TKey> Keys => _dictionary.Keys;

        public IEnumerable<TValue> Values
        {
            get
            {
                var values = new List<TValue>();

                lock (_lock)
                {
                    foreach (var pair in _dictionary)
                    {
                        if (!pair.Value.IsNullOrEmpty())
                        {
                            foreach (var value in pair.Value) values.Add(value);
                        }
                    }
                }

                return values;
            }
        }


        public bool Add(TKey key, TValue value)
        {
            if (key != null && value != null)
            {
                lock (_lock)
                {
                    if (_dictionary.ContainsKey(key))
                    {
                        if (!_dictionary[key].Contains(value))
                        {
                            _dictionary[key].Add(value);
                            _count++;
                        }
                    }
                    else
                    {
                        HashSet<TValue> list = new HashSet<TValue>();
                        list.Add(value);
                        _count++;
                        _dictionary.Add(key, list);
                    }
                }

                return true;
            }

            return false;
        }

        public bool Add(TKey key, IEnumerable<TValue> values)
        {
            if (key != null && !values.IsNullOrEmpty())
            {
                lock (_lock)
                {
                    if (_dictionary.ContainsKey(key))
                    {
                        foreach (var value in values)
                        {
                            if (!_dictionary[key].Contains(value))
                            {
                                _dictionary[key].Add(value);
                                _count++;
                            }
                        }
                    }
                    else
                    {
                        HashSet<TValue> list = new HashSet<TValue>();
                        foreach (var value in values) list.Add(value);
                        _count += values.Count();
                        _dictionary.Add(key, list);
                    }
                }

                return true;
            }

            return false;
        }

        public IEnumerable<TValue> Get(TKey key)
        {
            if (key != null)
            {
                lock (_lock)
                {
                    if (_dictionary.ContainsKey(key))
                    {
                        return _dictionary[key];
                    }
                }
            }

            return null;
        }

        public bool ContainsKey(TKey key)
        {
            if (key != null)
            {
                lock (_lock) return _dictionary.ContainsKey(key);
            }

            return false;
        }

        public bool Contains(TKey key, TValue value)
        {
            if (key != null && value != null)
            { 
                lock (_lock)
                {
                    if (_dictionary.ContainsKey(key))
                    {
                        var x = _dictionary[key];
                        if (x != null)
                        {
                            return x.Contains(value);
                        }
                    }
                }
            }

            return false;
        }

        public bool AllKeysContains(TValue value)
        {
            if (value != null)
            {
                var contains = true;

                lock (_lock)
                {
                    foreach (var key in _dictionary.Keys)
                    {
                        var x = _dictionary[key];
                        if (x != null)
                        {
                            contains = x.Contains(value);
                            if (!contains) break;
                        }
                    }
                }

                return contains;
            }

            return false;
        }

        public void Remove(TKey key)
        {
            if (key != null)
            {
                lock (_lock) _dictionary.Remove(key);
            }
        }

        public void Remove(TKey key, TValue value)
        {
            if (key != null)
            {
                lock (_lock)
                {
                    var x = _dictionary[key];
                    if (x != null)
                    {
                        x.Remove(value);
                    }
                }
            }
        }

        public void Clear()
        {
            lock (_lock) _dictionary.Clear();
        }
    }


    public static class ListDictionaryExtensions
    {
        public static ListDictionary<TKey, TSource> ToListDictionary<TKey, TSource>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            if (!source.IsNullOrEmpty() && keySelector != null)
            {
                var results = new ListDictionary<TKey, TSource>();
                foreach (var item in source)
                {
                    var key = keySelector(item);
                    if (key != null)
                    {
                        results.Add(key, item);
                    }
                }
                return results;
            }

            return null;
        }

        public static ListDictionary<TKey, TValue> ToListDictionary<TKey, TValue, TSource>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TValue> valueSelector)
        {
            if (!source.IsNullOrEmpty() && keySelector != null && valueSelector != null)
            {
                var results = new ListDictionary<TKey, TValue>();
                foreach (var item in source)
                {
                    var key = keySelector(item);
                    var value = valueSelector(item);
                    if (key != null)
                    {
                        results.Add(key, value);
                    }
                }
                return results;
            }

            return null;
        }
    }
}
