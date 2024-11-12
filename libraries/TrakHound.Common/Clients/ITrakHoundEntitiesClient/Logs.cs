// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Requests;

namespace TrakHound.Clients
{
    public partial interface ITrakHoundEntitiesClient
    {
        Task<IEnumerable<TrakHoundLog>> GetLogs(string objectPath, TrakHoundLogLevel minimumLevel, int skip = _defaultSkip, int take = _defaultTake, SortOrder sortOrder = _defaultSortOrder, string routerId = null);

        Task<IEnumerable<TrakHoundLog>> GetLogs(string objectPath, TrakHoundLogLevel minimumLevel, DateTime start, DateTime stop, int skip = _defaultSkip, int take = _defaultTake, SortOrder sortOrder = _defaultSortOrder, string routerId = null);

        Task<IEnumerable<TrakHoundLog>> GetLogs(string objectPath, TrakHoundLogLevel minimumLevel, string startExpression, string stopExpression, int skip = _defaultSkip, int take = _defaultTake, SortOrder sortOrder = _defaultSortOrder, string routerId = null);


        Task<Dictionary<string, IEnumerable<TrakHoundLogValue>>> GetLogValues(string objectPath, TrakHoundLogLevel minimumLevel, DateTime start, DateTime stop, int skip = _defaultSkip, int take = _defaultTake, SortOrder sortOrder = _defaultSortOrder, string routerId = null);
        
        Task<Dictionary<string, IEnumerable<TrakHoundLogValue>>> GetLogValues(string objectPath, TrakHoundLogLevel minimumLevel, string startExpression, string stopExpression, int skip = _defaultSkip, int take = _defaultTake, SortOrder sortOrder = _defaultSortOrder, string routerId = null);


        Task<ITrakHoundConsumer<IEnumerable<TrakHoundLog>>> SubscribeLogs(string objectPath, TrakHoundLogLevel minimumLevel, string routerId = null);


        Task<bool> PublishLog(string objectPath, TrakHoundLogLevel minimumLevel, string message, string code = null, DateTime? timestamp = null, bool async = false, string routerId = null);

        Task<bool> PublishLogs(IEnumerable<TrakHoundLogEntry> entries, bool async = false, string routerId = null);
    }
}
