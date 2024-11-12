// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;

namespace TrakHound.Drivers.Entities
{
    public interface IObservationAggregateDriver : ITrakHoundDriver
    {
        Task<TrakHoundResponse<double>> Aggregate(IEnumerable<string> objectUuids, long start, long stop, long skip = 0, long take = 0, SortOrder order = SortOrder.Ascending);
    }
}
