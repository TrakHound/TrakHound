// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Entities;

namespace TrakHound.Clients
{
    public partial interface ITrakHoundSystemObjectObservationClient
    {
        Task<IEnumerable<ITrakHoundObjectObservationEntity>> QueryByRange(IEnumerable<TrakHoundRangeQuery> queries, long skip, long take, SortOrder sortOrder, string routerId = null);
    }
}
