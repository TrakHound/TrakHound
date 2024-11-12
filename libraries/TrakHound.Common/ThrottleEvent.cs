// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;

namespace TrakHound
{
    public class ThrottleEvent : IDisposable
    {
        private readonly System.Timers.Timer _timer;
        private readonly object _lock = new object();
        private bool _set;
        private bool _sent;


        public event EventHandler Elapsed;


        public ThrottleEvent(int minimumInterval = 1000)
        {
            _timer = new System.Timers.Timer();
            _timer.Interval = minimumInterval;
            _timer.Elapsed += OnElapsed;
        }

        private void OnElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            bool sent;

            lock (_lock)
            {
                sent = _sent;
                _sent = true;
            }

            if (!sent)
            {
                if (Elapsed != null) Elapsed.Invoke(this, e);
            }
            else
            {
                lock (_lock) _set = false;
            }
        }

        public void Set()
        {
            bool send = false;

            try
            {
                lock (_lock)
                {
                    if (_set == false)
                    {
                        _set = true;
                        send = true;
                    }
                    else
                    {
                        send = false;
                    }

                    if (send) _sent = true;
                    else _sent = false;

                    if (_timer != null && !_timer.Enabled)
                    {
                        _timer.Start();
                    }
                }
            }
            catch { }            

            if (send)
            {       
                if (Elapsed != null) Elapsed.Invoke(this, new EventArgs());
            }
        }

        public void Dispose()
        {
            lock (_lock)
            {
                if (_timer != null) _timer.Dispose();
            }
        }
    }
}
