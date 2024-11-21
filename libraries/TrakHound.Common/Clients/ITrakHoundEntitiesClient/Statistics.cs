// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Entities;
using TrakHound.Requests;

namespace TrakHound.Clients
{
    public partial interface ITrakHoundEntitiesClient
    {
        Task<IEnumerable<TrakHoundStatistic>> GetLatestStatistics(string objectPath, DateTime start, DateTime stop, TimeSpan span, int skip = _defaultSkip, int take = _defaultTake, SortOrder sortOrder = _defaultSortOrder, string routerId = null);

        Task<IEnumerable<TrakHoundStatistic>> GetLatestStatistics(string objectPath, string startExpression, string stopExpression, string spanExpression, int skip = _defaultSkip, int take = _defaultTake, SortOrder sortOrder = _defaultSortOrder, string routerId = null);


        Task<IEnumerable<TrakHoundStatistic>> GetStatistics(string objectPath, DateTime start, DateTime stop, TimeSpan span, int skip = _defaultSkip, int take = _defaultTake, SortOrder sortOrder = _defaultSortOrder, string routerId = null);

        Task<IEnumerable<TrakHoundStatistic>> GetStatistics(string objectPath, string startExpression, string stopExpression, string spanExpression, int skip = _defaultSkip, int take = _defaultTake, SortOrder sortOrder = _defaultSortOrder, string routerId = null);


        Task<ITrakHoundConsumer<IEnumerable<TrakHoundStatistic>>> SubscribeStatistics(string objectPath, string routerId = null);


        Task<bool> PublishStatistic(string objectPath, DateTime rangeStart, DateTime rangeEnd, object value, TrakHoundStatisticDataType? dataType = null, DateTime? timestamp = null, TrakHoundUpdateType aggregateType = TrakHoundUpdateType.Absolute, bool async = false, string routerId = null);

        Task<bool> PublishStatistic(string objectPath, string rangeStart, string rangeEnd, object value, TrakHoundStatisticDataType? dataType = null, DateTime? timestamp = null, TrakHoundUpdateType aggregateType = TrakHoundUpdateType.Absolute, bool async = false, string routerId = null);

        Task<bool> PublishStatistics(IEnumerable<TrakHoundStatisticEntry> entries, bool async = false, string routerId = null);
    }
}
