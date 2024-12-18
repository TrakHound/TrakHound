// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;

namespace TrakHound.Logging
{
    public interface ITrakHoundLogProvider
    {
        /// <summary>
        /// Event Handler for when a new Log Entry is received
        /// </summary>
        event EventHandler<TrakHoundLogItem> LogEntryReceived;


        string GetLoggerId();

        string GetLoggerId(string name);


        ITrakHoundLogger GetLogger(string name);

        void AddLogger(string name);


        IEnumerable<string> QueryLoggerIds(string pattern);

        IEnumerable<string> QueryLoggerIds(IEnumerable<string> patterns);


        IEnumerable<TrakHoundLogItem> QueryLogs(string name, DateTime from, DateTime to, int skip = 0, int take = 1000, SortOrder sortOrder = SortOrder.Ascending);
        
        IEnumerable<TrakHoundLogItem> QueryLogsByMinimumLevel(string name, TrakHoundLogLevel minimumLogLevel, DateTime from, DateTime to, int skip = 0, int take = 1000, SortOrder sortOrder = SortOrder.Ascending);
        
        IEnumerable<TrakHoundLogItem> QueryLogsByLevel(string name, TrakHoundLogLevel logLevel, DateTime from, DateTime to, int skip = 0, int take = 1000, SortOrder sortOrder = SortOrder.Ascending);
    }
}
