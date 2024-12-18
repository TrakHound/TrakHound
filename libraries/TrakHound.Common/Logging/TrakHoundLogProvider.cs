// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;

namespace TrakHound.Logging
{
    public abstract class TrakHoundLogProviderBase : ITrakHoundLogProvider
    {
        public readonly Dictionary<string, ITrakHoundLogger> _loggers = new Dictionary<string, ITrakHoundLogger>(); // LoggerId => Logger
        private readonly object _lock = new object();

        public static TrakHoundLogLevel MinimumLogLevel = TrakHoundLogLevel.Information;


        /// <summary>
        /// Event Handler for when a new Log Entry is received
        /// </summary>
        public event EventHandler<TrakHoundLogItem> LogEntryReceived;


        public virtual string GetLoggerId() => null;

        public virtual string GetLoggerId(string name) => name;


        public IEnumerable<string> GetLoggerIds()
        {
            lock (_lock)
            {
                return _loggers.Keys;
            }
        }

        public IEnumerable<ITrakHoundLogger> GetLoggers()
        {
            lock (_lock)
            {
                return _loggers.Values;
            }
        }

        public ITrakHoundLogger GetLogger(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                var loggerId = GetLoggerId(name);
                if (loggerId != null)
                {
                    lock (_lock)
                    {
                        var logger = _loggers.GetValueOrDefault(loggerId);
                        if (logger == null)
                        {
                            logger = new TrakHoundLogger(loggerId);
                            logger.LogEntryReceived += EntryReceived;
                            _loggers.Add(loggerId, logger);
                        }

                        return logger;
                    }
                }
            }

            return null;
        }

        public void AddLogger(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                var loggerId = GetLoggerId(name);
                if (loggerId != null)
                {
                    lock (_lock)
                    {
                        if (!_loggers.ContainsKey(loggerId))
                        {
                            var logger = new TrakHoundLogger(loggerId);
                            logger.LogEntryReceived += EntryReceived;
                            _loggers.Add(loggerId, logger);
                        }
                    }
                }
            }
        }

        private void EntryReceived(ITrakHoundLogger logger, TrakHoundLogItem item)
        {
            if (LogEntryReceived != null) LogEntryReceived.Invoke(logger, item);
            HandleEntryReceived(logger, item);
        }

        protected virtual void HandleEntryReceived(ITrakHoundLogger logger, TrakHoundLogItem item) { }


        public virtual IEnumerable<string> QueryLoggerIds(string pattern) => null;

        public virtual IEnumerable<string> QueryLoggerIds(IEnumerable<string> pattern) => null;


        public virtual IEnumerable<TrakHoundLogItem> QueryLogs(string id, DateTime from, DateTime to, int skip = 0, int take = 1000, SortOrder sortOrder = SortOrder.Ascending) => null;
        
        public virtual IEnumerable<TrakHoundLogItem> QueryLogsByMinimumLevel(string id, TrakHoundLogLevel minmimumLogLevel, DateTime from, DateTime to, int skip = 0, int take = 1000, SortOrder sortOrder = SortOrder.Ascending) => null;
        
        public virtual IEnumerable<TrakHoundLogItem> QueryLogsByLevel(string id, TrakHoundLogLevel logLevel, DateTime from, DateTime to, int skip = 0, int take = 1000, SortOrder sortOrder = SortOrder.Ascending) => null;

    }
}
