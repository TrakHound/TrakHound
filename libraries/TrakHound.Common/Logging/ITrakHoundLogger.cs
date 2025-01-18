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
        string Id { get; }

        /// <summary>
        /// Event for when a new Log Entry is received
        /// </summary>
        event TrakHoundLogEventHandler LogEntryReceived;


        void Log(TrakHoundLogItem logEntry);

        void Log(TrakHoundLogLevel logLevel, string message);
        void Log(TrakHoundLogLevel logLevel, string name, string message);

        void LogCritical(string message);
        void LogCritical(string name, string message);

        void LogError(string message);
        void LogError(string name, string message);

        void LogWarning(string message);
        void LogWarning(string name, string message);

        void LogInformation(string message);
        void LogInformation(string name, string message);

        void LogDebug(string message);
        void LogDebug(string name, string message);

        void LogTrace(string message);
        void LogTrace(string name, string message);
    }
}
