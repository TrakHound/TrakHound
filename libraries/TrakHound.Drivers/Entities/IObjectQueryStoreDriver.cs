// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Entities;

namespace TrakHound.Drivers.Entities
{
    public interface IObjectQueryStoreDriver : ITrakHoundDriver
    {
        Task<TrakHoundResponse<bool>> StoreQuery(IEnumerable<ITrakHoundQueryRequestResult> queryResults);

        Task<TrakHoundResponse<bool>> StorePath(IEnumerable<ITrakHoundQueryRequestResult> queryResults);


        Task<TrakHoundResponse<bool>> StoreParents(IEnumerable<ITrakHoundObjectQueryResult> queryResults);

        Task<TrakHoundResponse<bool>> StoreChildren(IEnumerable<ITrakHoundObjectQueryResult> queryResults);


        Task<TrakHoundResponse<bool>> StoreChildrenByRoots(IEnumerable<ITrakHoundObjectQueryResult> queryResults);

        Task<TrakHoundResponse<bool>> StoreRootsByChildren(IEnumerable<ITrakHoundObjectQueryResult> queryResults);


        Task<TrakHoundResponse<bool>> StoreResults(IEnumerable<ITrakHoundObjectQueryResult> queryResults);

        Task<TrakHoundResponse<bool>> Invalidate(IEnumerable<string> objectUuids);
    }
}
