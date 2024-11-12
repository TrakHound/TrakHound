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
        Task<TrakHoundTimeRange> GetTimeRange(string objectPath, string routerId = null);

        Task<DateTime?> GetTimeRangeValue(string objectPath, string routerId = null);

        Task<IEnumerable<TrakHoundTimeRange>> GetTimeRanges(string objectPath, string routerId = null);

        Task<IEnumerable<TrakHoundTimeRange>> GetTimeRanges(IEnumerable<string> objectPaths, string routerId = null);


        Task<ITrakHoundConsumer<IEnumerable<TrakHoundTimeRange>>> SubscribeTimeRanges(string objectPath, string routerId = null);


        Task<bool> PublishTimeRange(string objectPath, long start, long end, bool async = false, string routerId = null);
        Task<bool> PublishTimeRange(string objectPath, string timeRangeExpression, bool async = false, string routerId = null);
        Task<bool> PublishTimeRange(string objectPath, DateTime start, DateTime end, bool async = false, string routerId = null);
    }
}
