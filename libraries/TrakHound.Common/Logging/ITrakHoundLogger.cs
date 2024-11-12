// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;

namespace TrakHound.Logging
{
    public interface ITrakHoundLogger
    {
        /// <summary>
        /// The Name of the Logger
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Event for when a new Log Entry is received
        /// </summary>
        event EventHandler<TrakHoundLogItem> LogEntryReceived;


        void Log(TrakHoundLogItem logEntry);

        void Log(TrakHoundLogLevel logLevel, string message);

        void LogCritical(string message);

        void LogError(string message);

        void LogWarning(string message);

        void LogInformation(string message);

        void LogDebug(string message);

        void LogTrace(string message);
    }
}
