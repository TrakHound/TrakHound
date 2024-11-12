// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TrakHound
{
    public class CacheDictionary<TKey, TValue> : IDisposable
    {
        private readonly int _ttl;
        private readonly int _expirationInterval;
        private readonly Dictionary<TKey, TValue> _cache = new Dictionary<TKey, TValue>();
        private readonly Dictionary<TKey, long> _created = new Dictionary<TKey, long>();
        private readonly object _lock = new object();
        private CancellationTokenSource _stop;


        public CacheDictionary(int ttl = 60000, int expirationInterval = 5000)
        {
            _ttl = ttl;
            _expirationInterval = expirationInterval;

            _stop = new CancellationTokenSource();
            _ = Task.Run(() => Worker(_stop.Token));
        }

        public void Dispose()
        {
            _stop.Cancel();
        }


        public void Add(TKey key, TValue value)
        {
            if (key != null)
            {
                lock (_lock)
                {
                    _cache[key] = value;
                    _created[key] = UnixDateTime.Now;
                }
            }
        }

        public TValue Get(TKey key)
        {
            if (key != null)
            {
                lock (_lock)
                {
                    if (_cache.ContainsKey(key))
                    {
                        return _cache[key];
                    }
                }
            }

            return default;
        }

        public void Remove(TKey key)
        {
            if (key != null)
            {
                lock (_lock)
                {
                    _cache.Remove(key);
                    _created.Remove(key);
                }
            }
        }

        public bool ContainsKey(TKey key)
        {
            if (key != null)
            {
                lock (_lock)
                {
                    return _cache.ContainsKey(key);
                }
            }

            return false;
        }


        private async Task Worker(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var expiredKeys = new HashSet<TKey>();
                    var now = UnixDateTime.Now;

                    lock (_lock)
                    {
                        foreach (var pair in _created)
                        {
                            var diff = now - pair.Value;
                            if (diff > (_ttl * 1000000))
                            {
                                expiredKeys.Add(pair.Key);
                            }
                        }

                        foreach (var key in expiredKeys)
                        {
                            _cache.Remove(key);
                            _created.Remove(key);
                        }
                    }

                    await Task.Delay(_expirationInterval);
                }
            }
            catch { }
        }
    }
}
