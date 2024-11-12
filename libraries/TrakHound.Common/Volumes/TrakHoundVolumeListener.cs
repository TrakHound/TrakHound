// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TrakHound.Volumes
{
    public class TrakHoundVolumeListener : ITrakHoundVolumeListener
    {
        private readonly string _basePath;
        private readonly string _path;
        private readonly string _filter;
        private readonly FileSystemWatcher _watcher;
        private readonly Dictionary<string, ChangeItem> _changedItems = new Dictionary<string, ChangeItem>();
        private readonly DelayEvent _delayEvent = new DelayEvent(100);
        private readonly object _lock = new object();

        struct ChangeItem
        {
            public string Path { get; set; }
            public TrakHoundVolumeOnChangeType Type { get; set; }

            public ChangeItem(string path, TrakHoundVolumeOnChangeType type)
            {
                Path = path;
                Type = type;
            }
        }


        public event TrakHoundVolumeOnChangeHandler Changed;


        public TrakHoundVolumeListener(string basePath, string path, string filter = null)
        {
            _basePath = basePath;
            _path = path;
            
            var fullPath = string.IsNullOrEmpty(_path) && _path != "/" ? Path.Combine(basePath, _path) : basePath;
            var filterExpression = !string.IsNullOrEmpty(filter) ? filter : "*.*";

            if (!Directory.Exists(fullPath)) Directory.CreateDirectory(fullPath);

            _watcher = new FileSystemWatcher(fullPath, filterExpression);
            _watcher.IncludeSubdirectories = true;
            _watcher.Created += FileSystemWatcherChanged;
            _watcher.Changed += FileSystemWatcherChanged;
            _watcher.Deleted += FileSystemWatcherChanged;
            _watcher.Renamed += FileSystemWatcherChanged;

            _delayEvent.Elapsed += DelayEventElapsed;
        }

        public void Start()
        {
            _watcher.EnableRaisingEvents = true;
        }

        public void Stop()
        {
            _watcher.EnableRaisingEvents = false;
        }

        public void Dispose()
        {
            if (_watcher != null)
            {
                _watcher.Created -= FileSystemWatcherChanged;
                _watcher.Changed -= FileSystemWatcherChanged;
                _watcher.Deleted -= FileSystemWatcherChanged;
                _watcher.Renamed -= FileSystemWatcherChanged;

                _watcher.Dispose();
            }
        }

        private void FileSystemWatcherChanged(object sender, FileSystemEventArgs args)
        {
            var relativePath = Path.GetRelativePath(_basePath, args.FullPath);
            if (relativePath != null) relativePath = relativePath.Replace("\\", "/");

            TrakHoundVolumeOnChangeType changeType;
            switch (args.ChangeType)
            {
                case WatcherChangeTypes.Created: changeType = TrakHoundVolumeOnChangeType.Create; break;
                case WatcherChangeTypes.Deleted: changeType = TrakHoundVolumeOnChangeType.Delete; break;
                default: changeType = TrakHoundVolumeOnChangeType.Modify; break;
            }

            lock (_lock)
            {
                _changedItems.Remove(relativePath);
                _changedItems.Add(relativePath, new ChangeItem(relativePath, changeType));
            }

            _delayEvent.Set();
        }

        private void DelayEventElapsed(object sender, System.EventArgs e)
        {
            IEnumerable<ChangeItem> items;
            lock (_lock)
            {
                items = _changedItems.Values.ToList();
                _changedItems.Clear();
            }

            if (!items.IsNullOrEmpty())
            {
                foreach (var item in items)
                {
                    if (Changed != null) Changed.Invoke(item.Path, item.Type);
                }
            }
        }
    }
}
