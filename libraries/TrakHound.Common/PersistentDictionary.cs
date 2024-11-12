// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace TrakHound
{
    public interface IPersistentDictionary
    {
        string Id { get; }

        long KeyCount { get; }

        Task Write();   
    }


    public class PersistentDictionary<TKey, TValue> : IPersistentDictionary
    {
        private const string _defaultBaseDirectoryName = "cache";
        private static readonly string _defaultBaseDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _defaultBaseDirectoryName);

        private readonly string _id;
        private readonly string _baseDirectory;
        private readonly Func<TValue, TValue, bool> _comparer;
        private readonly Func<string, TKey> _keyConverter;
        private readonly Func<string, TValue> _valueConverter;
        private readonly Dictionary<TKey, TValue> _entries = new Dictionary<TKey, TValue>();
        private readonly object _lock = new object();
        private long _keyCount;
        private bool _isChanged;


        public string Id => _id;

        public long KeyCount => _keyCount;


        public PersistentDictionary(
            string id,
            string baseDirectory = null,
            Func<TValue, TValue, bool> comparer = null,
            Func<string, TKey> keyConverter = null,
            Func<string, TValue> valueConverter = null
            )
        {
            _id = id;
            _comparer = comparer != null ? comparer : DefaultComparer;
            _keyConverter = keyConverter != null ? keyConverter : DefaultKeyConverter;
            _valueConverter = valueConverter != null ? valueConverter : DefaultValueConverter;

            if (!string.IsNullOrEmpty(baseDirectory))
            {
                _baseDirectory = baseDirectory;
                if (!Path.IsPathRooted(_baseDirectory)) _baseDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _baseDirectory);
            }
            else
            {
                _baseDirectory = _defaultBaseDirectory;
            }

            PersistentDictionaryManager.AddDictionary(_id, this);
        }

        private static bool DefaultComparer(TValue existingValue, TValue newValue)
        {
            return EqualityComparer<TValue>.Default.Equals(existingValue, newValue);
        }

        private static TKey DefaultKeyConverter(string inputKey)
        {
            try
            {
                return (TKey)Convert.ChangeType(inputKey, typeof(TKey));
            }
            catch { }

            return default;
        }

        private static TValue DefaultValueConverter(string inputValue)
        {
            try
            {
                return (TValue)Convert.ChangeType(inputValue, typeof(TValue));
            }
            catch { }

            return default;
        }


        public bool ContainsKey(TKey key)
        { 
            if (key != null)
            {
                lock (_lock)
                {
                    return _entries.ContainsKey(key);
                }
            }

            return false;
        }

        public void Add(TKey key, TValue value)
        {
            if (key != null)
            {
                lock (_lock)
                {
                    if (_entries.ContainsKey(key))
                    {
                        var existingValue = _entries[key];
                        if (_comparer == null || !_comparer(existingValue, value))
                        {
                            _entries[key] = value;
                            _isChanged = true;
                        }
                    }
                    else
                    {
                        _entries.Add(key, value);
                        _keyCount++;
                        _isChanged = true;
                    }
                }
            }
        }

        public TValue Get(TKey key)
        {
            if (key != null)
            {
                lock (_lock)
                {
                    if (_entries.ContainsKey(key))
                    {
                        return _entries[key];
                    }
                }
            }

            return default;
        }

        public bool Remove(TKey key)
        {
            if (key != null)
            {
                lock (_lock)
                {
                    return _entries.Remove(key);
                }
            }

            return false;
        }


        public void Recover()
        {
            if (_id != null)
            {
                var pagePath = Path.Combine(_baseDirectory, _id);
                if (File.Exists(pagePath))
                {
                    var contentLines = ReadFile(pagePath);
                    if (contentLines != null)
                    {
                        foreach (var line in contentLines)
                        {
                            var delimeter = "|-|";
                            var i = line.IndexOf(delimeter);
                            if (i > 0 && i + 1 < line.Length)
                            {
                                var key = _keyConverter(line.Substring(0, i));
                                var value = _valueConverter(line.Substring(i + delimeter.Length));

                                //Console.WriteLine($"PersistentDictionary : {_id} : Recover() : Key = {key} : Value = {value}"); // DEBUG

                                Add(key, value);
                            }
                        }
                    }
                }
            }
        }

        public async Task Write()
        {
            Dictionary<TKey, TValue> inputEntries = null;
            var write = false;

            lock (_lock)
            {
                if (_isChanged && _id != null)
                {
                    _isChanged = false;
                    inputEntries = new Dictionary<TKey, TValue>(_entries);
                    write = true;
                }
            }

            if (write)
            {
                //Console.WriteLine($"PersistentDictionary : {_id} : Write() : write = {write}"); // DEBUG

                if (!inputEntries.IsNullOrEmpty())
                {
                    //Console.WriteLine($"PersistentDictionary : {_id} : Write() : inputEntries.Count() = {inputEntries.Count}"); // DEBUG
                }

                if (!Directory.Exists(_baseDirectory)) Directory.CreateDirectory(_baseDirectory);

                var pagePath = Path.Combine(_baseDirectory, _id);

                var writeEntries = new Dictionary<TKey, TValue>();

                //if (File.Exists(pagePath))
                //{
                //    var contentLines = ReadFile(pagePath);
                //    if (contentLines != null)
                //    {
                //        foreach (var line in contentLines)
                //        {
                //            var i = line.IndexOf(':');
                //            if (i > 0 && i + 1 < line.Length)
                //            {
                //                var key = _keyConverter(line.Substring(0, i));
                //                var value = _valueConverter(line.Substring(i + 1));

                //                writeEntries.Add(key, value);
                //            }
                //        }
                //    }
                //}

                // Add New Entries
                foreach (var entry in inputEntries)
                {
                    writeEntries.Remove(entry.Key);
                    writeEntries.Add(entry.Key, entry.Value);

                    //Console.WriteLine($"PersistentDictionary : {_id} : Write() : Key = {entry.Key} : Value = {entry.Value}"); // DEBUG
                }

                // Create Write Data
                var writeLines = new List<string>();
                foreach (var entry in writeEntries)
                {
                    writeLines.Add($"{entry.Key}|-|{entry.Value}");
                }
                var writeContent = string.Join("\r\n", writeLines);



                //Console.WriteLine($"PersistentDictionary : {_id} : WriteFile()"); // DEBUG



                await WriteFile(pagePath, writeContent);
            }
        }

        private string[] ReadFile(string path)
        {
            try
            {
                using (var fileStream = File.Open(path, FileMode.Open, FileAccess.Read))
                {
                    using (var readStream = new MemoryStream())
                    using (var gzipStream = new GZipStream(fileStream, CompressionMode.Decompress))
                    {
                        gzipStream.CopyTo(readStream);

                        var bytes = readStream.ToArray();

                        var s = System.Text.Encoding.UTF8.GetString(bytes);
                        if (!string.IsNullOrEmpty(s))
                        {
                            return s.Split(Environment.NewLine);
                        }
                    }
                }
            }
            catch { }

            return null;
        }

        private async Task<bool> WriteFile(string path, string content)
        {
            try
            {
                var inputBytes = System.Text.Encoding.UTF8.GetBytes(content);

                using (var stream = new MemoryStream())
                {
                    using (var gzipStream = new GZipStream(stream, CompressionMode.Compress))
                    {
                        await gzipStream.WriteAsync(inputBytes, 0, inputBytes.Length);
                        await gzipStream.FlushAsync();
                    }

                    var compressedBytes = stream.ToArray();
                    await File.WriteAllBytesAsync(path, compressedBytes);
                    return true;
                }
            }
            catch { }

            return false;
        }
    }

    public static class PersistentDictionaryManager
    {
        private const int _defaultWriteInterval = 30000; // 30 Seconds

        private static readonly ListDictionary<string, IPersistentDictionary> _dictionaries = new ListDictionary<string, IPersistentDictionary>();
        private static readonly List<string> _changed = new List<string>();
        private static readonly System.Timers.Timer _writeTimer;
        private static readonly object _lock = new object();


        static PersistentDictionaryManager()
        {
            // Write to Disk at interval
            _writeTimer = new System.Timers.Timer();
            _writeTimer.Interval = _defaultWriteInterval;
            _writeTimer.Elapsed += (s, args) => WriteAll();
            _writeTimer.Start();

            AppDomain.CurrentDomain.ProcessExit += Dispose;
        }

        public static void SetWriteInterval(int writeInterval)
        {
            if (writeInterval > 0)
            {
                _writeTimer.Stop();
                _writeTimer.Interval = writeInterval;
                _writeTimer.Start();
            }
        }

        public static async void Dispose(object sender, EventArgs args)
        {
            if (_writeTimer != null) _writeTimer.Dispose();

            await WriteAll();
        }


        public static void AddDictionary(string id, IPersistentDictionary dictionary)
        {
            if (id != null && dictionary != null)
            {
                lock (_lock)
                {
                    if (!_dictionaries.ContainsKey(id)) _dictionaries.Add(id, dictionary);
                }
            }
        }

        public static IEnumerable<IPersistentDictionary> Get()
        {
            var dictionaries = new List<IPersistentDictionary>();

            lock (_lock)
            {
                foreach (var dictionary in _dictionaries.Values)
                {
                    dictionaries.Add(dictionary);
                }
            }

            return dictionaries;
        }


        public static async Task WriteAll()
        {
            var dictionaries = Get();
            foreach (var dictionary in dictionaries)
            {
                //Console.WriteLine($"PersistentDictionary : WriteAll() : {dictionary.Id}");


                await dictionary.Write();
            }
        }
    }
}
