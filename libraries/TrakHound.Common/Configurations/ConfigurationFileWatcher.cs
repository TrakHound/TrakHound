// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace TrakHound.Configurations
{
    class ConfigurationFileWatcher<T> : IDisposable
    {
        protected const int DefaultInterval = 2000;

        private readonly string _category;
        private readonly string _path;
        private readonly Func<string, T> _readFunction;
        private readonly int _interval;
        private readonly List<string> _changed = new List<string>();
        private readonly object _lock = new object();
        private FileSystemWatcher _watcher;
        private System.Timers.Timer _timer;
        private bool _update = false;


        public EventHandler<T> ConfigurationUpdated { get; set; }

        public EventHandler<ConfigurationFileRemovedArgs> ConfigurationRemoved { get; set; }

        public EventHandler<string> ErrorReceived { get; set; }


        public ConfigurationFileWatcher(string category, string path, Func<string, T> readFunction, int interval = DefaultInterval)
        {
            _category = category;
            _path = path;
            _readFunction = readFunction;
            _interval = interval;

            Start();
        }

        private void Start()
        {
            try
            {
                if (!Directory.Exists(_path)) Directory.CreateDirectory(_path);

                _watcher = new FileSystemWatcher();
                _watcher.Path = _path;
                _watcher.Filter = "*";
                _watcher.Created += FileChanged;
                _watcher.Changed += FileChanged;
                _watcher.Deleted += FileDeleted;
                _watcher.Error += FileWatcherError;
                _watcher.EnableRaisingEvents = true;

                _timer = new System.Timers.Timer();
                _timer.Interval = _interval;
                _timer.Elapsed += UpdateTimerElapsed;
                _timer.Enabled = true;
            }
            catch (Exception ex)
            {
                if (ErrorReceived != null) ErrorReceived.Invoke(this, ex.Message);
            }
        }

        private void Update()
        {
            if (_update)
            {
                IEnumerable<string> changed;
                lock (_lock)
                {
                    changed = _changed.ToList().Distinct();
                    _changed.Clear();
                }

                if (!changed.IsNullOrEmpty())
                {
                    foreach (var path in changed)
                    {
                        var configuration = ReadFile(path);
                        if (configuration != null)
                        {
                            if (ConfigurationUpdated != null) ConfigurationUpdated.Invoke(this, configuration);
                        }
                    }
                }

                _update = false;
            }
        }

        private void UpdateTimerElapsed(object sender, EventArgs args)
        {
            Update();
        }


        private void FileChanged(object sender, FileSystemEventArgs args)
        {
            if (args.ChangeType == WatcherChangeTypes.Changed || args.ChangeType == WatcherChangeTypes.Created)
            {
                lock (_lock) _changed.Add(args.FullPath);

                _update = true;
            }
        }

        private void FileDeleted(object sender, FileSystemEventArgs args)
        {
            var filename = args.Name;
            if (!string.IsNullOrEmpty(filename))
            {
                var configurationId = Path.GetFileNameWithoutExtension(filename);

                if (ConfigurationRemoved != null) ConfigurationRemoved.Invoke(this, new ConfigurationFileRemovedArgs(_category, configurationId));
            }
        }

        private void FileWatcherError(object sender, ErrorEventArgs e)
        {
            Console.WriteLine(e.GetException().Message);
        }

        private T ReadFile(string path)
        {
            try
            {
                if (_readFunction != null)
                {
                    return _readFunction(path);
                }
            }
            catch (Exception ex)
            {
                if (ErrorReceived != null) ErrorReceived.Invoke(this, ex.Message);
            }

            return default;
        }


        public void Dispose()
        {
            if (_watcher != null) _watcher.Dispose();
            if (_timer != null) _timer.Dispose();
        }
    }
}
