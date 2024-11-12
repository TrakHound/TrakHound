// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Blazor.Components;

namespace TrakHound.Blazor.Services
{
    public class NotificationService : IDisposable
    {
        private readonly List<Notifications.NotificationItem> _notifications = new List<Notifications.NotificationItem>();
        private System.Timers.Timer _notificationTimer;


        public IEnumerable<Notifications.NotificationItem> Notifications => _notifications;

        public event EventHandler Updated;


        public NotificationService() 
        {
            _notificationTimer = new System.Timers.Timer();
            _notificationTimer.Interval = 500;
            _notificationTimer.Elapsed += NotificationTimerElapsed;
            _notificationTimer.Start();
        }

        public void Dispose()
        {
            if (_notificationTimer != null) _notificationTimer.Dispose();
        }

        public void AddNotification(NotificationType type, string message, string details = null, int duration = 5000)
        {
            var notification = new Notifications.NotificationItem();
            notification.Type = type;
            notification.Message = message;
            notification.Details = details;
            notification.Timestamp = DateTime.Now;
            notification.TTL = TimeSpan.FromMilliseconds(duration);
            _notifications.Add(notification);

            if (Updated != null) Updated.Invoke(this, new EventArgs());
        }

        private void NotificationTimerElapsed(object sender, System.Timers.ElapsedEventArgs args)
        {
            if (!_notifications.IsNullOrEmpty())
            {
                var now = DateTime.Now;
                var ids = _notifications.Where(o => (now - o.Timestamp) > o.TTL).Select(o => o.Id).ToList();
                if (!ids.IsNullOrEmpty())
                {
                    foreach (var id in ids)
                    {
                        _notifications.RemoveAll(o => o.Id == id);
                    }

                    if (Updated != null) Updated.Invoke(this, new EventArgs());
                }
            }
        }
    }
}
