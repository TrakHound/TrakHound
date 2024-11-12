// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Blazor.Services
{
    public class TrakHoundDownloadService : IDisposable
    {
        private readonly Dictionary<string, TrakHoundDownloadItem> _imageItems = new Dictionary<string, TrakHoundDownloadItem>();
        private readonly Dictionary<string, DateTime> _imageRetentions = new Dictionary<string, DateTime>();
        private readonly object _lock = new object();
        private readonly System.Timers.Timer _retentionTimer;
        private readonly TimeSpan _retentionTime = TimeSpan.FromMinutes(5);


        public TrakHoundDownloadService()
        {
            _retentionTimer = new System.Timers.Timer();
            _retentionTimer.Interval = 5000;
            _retentionTimer.Elapsed += RetentionTimerElapsed;
            _retentionTimer.Start();
        }

        public void Dispose()
        {
            if (_retentionTimer != null) _retentionTimer.Dispose();
        }

        private void RetentionTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            var now = DateTime.UtcNow;

            lock (_lock)
            {
                if (!_imageRetentions.IsNullOrEmpty())
                {
                    foreach (var retention in _imageRetentions)
                    {
                        if ((now - retention.Value) > _retentionTime)
                        {
                            _imageRetentions.Remove(retention.Key);
                        }
                    }
                }
            }
        }

        public TrakHoundDownloadItem GetItem(string key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                lock (_lock) return _imageItems.GetValueOrDefault(key); 
            }

            return null;
        }

        public string AddItem(TrakHoundDownloadItem item)
        {
            if (item != null)
            {
                var key = Guid.NewGuid().ToString();
                lock (_lock)
                {
                    _imageItems.Add(key, item);
                    _imageRetentions.Add(key, DateTime.UtcNow);
                }
                return key;
            }

            return null;
        }
    }
}
