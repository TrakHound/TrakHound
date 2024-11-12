// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;

namespace TrakHound
{
    public class DelayEvent : IDisposable
    {
        private readonly System.Timers.Timer _timer;
        private readonly object _lock = new object();


        public event EventHandler Elapsed;


        public DelayEvent(int interval = 1000)
        {
            _timer = new System.Timers.Timer();
            _timer.Interval = interval;
            _timer.Elapsed += OnElapsed;
        }

        private void OnElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                lock (_lock) _timer.Stop();

                if (Elapsed != null) Elapsed.Invoke(this, e);
            }
            catch (ObjectDisposedException) { }
        }

        public void Set()
        {
            try
            {
                lock (_lock)
                {
                    if (_timer != null)
                    {
                        _timer.Stop();
                        _timer.Start();
                    }
                }
            }
            catch (ObjectDisposedException) { }
        }

        public void Dispose()
        {
            try
            {
                lock (_lock)
                {
                    if (_timer != null)
                    {
                        _timer.Stop();
                        _timer.Dispose();
                    }
                }
            }
            catch (ObjectDisposedException) { }
        }
    }
}
