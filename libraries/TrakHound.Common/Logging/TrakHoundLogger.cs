// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;

namespace TrakHound.Logging
{
    public class TrakHoundLogger<T> : TrakHoundLogger
    {
        public Type LoggerType { get; set; }


        public TrakHoundLogger() : base(typeof(T).Name)
        {
            LoggerType = typeof(T);
        }

        public TrakHoundLogger(string name) : base(name)
        {
            LoggerType = typeof(T);
        }
    }

    public class TrakHoundLogger : ITrakHoundLogger
    {
        private readonly string _id;

        
        /// <summary>
        /// The Name of the Logger
        /// </summary>
        public string Id => _id;

        /// <summary>
        /// Event for when a new Log Entry is received
        /// </summary>
        public event TrakHoundLogEventHandler LogEntryReceived;


        public TrakHoundLogger(string id)
        {
            _id = id;
            //TrakHoundLogProvider.AddLogger(this);
        }


        public virtual void Log(TrakHoundLogItem logEntry)
        {
            Log(logEntry.LogLevel, logEntry.Message);
        }

        public virtual void Log(TrakHoundLogLevel logLevel, string message)
        {
            if (LogEntryReceived != null) LogEntryReceived.Invoke(this, new TrakHoundLogItem(_id, logLevel, message));
        }

        public void LogCritical(string message) => Log(TrakHoundLogLevel.Critical, message);

        public void LogError(string message) => Log(TrakHoundLogLevel.Error, message);

        public void LogWarning(string message) => Log(TrakHoundLogLevel.Warning, message);

        public void LogInformation(string message) => Log(TrakHoundLogLevel.Information, message);

        public void LogDebug(string message) => Log(TrakHoundLogLevel.Debug, message);

        public void LogTrace(string message) => Log(TrakHoundLogLevel.Trace, message);
    }
}
