// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Entities;

namespace TrakHound.Clients
{
    internal partial class TrakHoundInstanceSystemObjectLogClient
    {
        public async Task<IEnumerable<ITrakHoundObjectLogEntity>> QueryByObjectUuid(
           IEnumerable<string> objectUuids,
           TrakHoundLogLevel minLevel,
           long skip = 0,
           long take = 1000,
           SortOrder sortOrder = TrakHound.SortOrder.Ascending,
           string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router.Entities.Objects.Logs.QueryByObjectUuid(objectUuids, minLevel, skip, take, sortOrder);
                if (response.IsSuccess)
                {
                    return response.Content.ToDistinct();
                }
            }

            return default;
        }

        public async Task<IEnumerable<ITrakHoundObjectLogEntity>> QueryByRange(
           IEnumerable<TrakHoundRangeQuery> queries,
           TrakHoundLogLevel minLevel,
           long skip = 0,
           long take = 1000,
           SortOrder sortOrder = TrakHound.SortOrder.Ascending,
           string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router.Entities.Objects.Logs.QueryByRange(queries, minLevel, skip, take, sortOrder);
                if (response.IsSuccess)
                {
                    return response.Content.ToDistinct();
                }
            }

            return default;
        }
    }
}
