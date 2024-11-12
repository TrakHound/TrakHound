// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Entities;

namespace TrakHound.Clients
{
    public partial interface ITrakHoundSystemObjectLogClient
    {
        Task<IEnumerable<ITrakHoundObjectLogEntity>> QueryByObjectUuid(IEnumerable<string> objectUuids, TrakHoundLogLevel minimumLevel, long skip, long take, SortOrder sortOrder, string routerId = null);

        Task<IEnumerable<ITrakHoundObjectLogEntity>> QueryByRange(IEnumerable<TrakHoundRangeQuery> queries, TrakHoundLogLevel minimumLevel, long skip, long take, SortOrder sortOrder, string routerId = null);
    }
}
