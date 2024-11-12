// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Entities;

namespace TrakHound.Clients.Collections
{
    internal partial class TrakHoundCollectionObjectLogClient
    {
        public async Task<IEnumerable<ITrakHoundObjectLogEntity>> QueryByObjectUuid(
            IEnumerable<string> objectUuids,
            TrakHoundLogLevel minimumLevel,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null)
        {
            return default;
        }

        public async Task<IEnumerable<ITrakHoundObjectLogEntity>> QueryByRange(
            IEnumerable<TrakHoundRangeQuery> queries,
            TrakHoundLogLevel minimumLevel,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null)
        {
            return default;
        }
    }
}
