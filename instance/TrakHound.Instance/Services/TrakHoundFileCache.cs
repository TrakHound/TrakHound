// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Instance.Services
{
    public class TrakHoundFileCache
    {
        private const int _ttl = 3600000;
        private const int _interval = 60000;
        private readonly CacheDictionary<string, byte[]> _files = new CacheDictionary<string, byte[]>(_ttl, _interval);
        private readonly object _lock = new object();


        public void Add(string path, byte[] contents)
        {
            if (!string.IsNullOrEmpty(path) && !contents.IsNullOrEmpty())
            {
                lock (_lock)
                {
                    _files.Remove(path);
                    _files.Add(path, contents);
                }
            }
        }

        public byte[] Get(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                lock (_lock)
                {
                    return _files.Get(path); ;
                }
            }

            return null;
        }
    }
}
