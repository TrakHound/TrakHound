// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Entities;

namespace TrakHound.Drivers.Entities
{
    /// <summary>
    /// Driver used to Query TrakHound Objects
    /// </summary>
    public interface IObjectQueryDriver : ITrakHoundDriver
    {
        Task<TrakHoundResponse<ITrakHoundObjectQueryResult>> Query(TrakHoundObjectQueryRequest queryRequest, long skip = 0, long take = 0, SortOrder order = SortOrder.Ascending);


        Task<TrakHoundResponse<string>> ListNamespaces(long skip = 0, long take = 0, SortOrder order = SortOrder.Ascending);

        Task<TrakHoundResponse<ITrakHoundObjectQueryResult>> QueryRoot(long skip = 0, long take = 0, SortOrder order = SortOrder.Ascending);

        Task<TrakHoundResponse<ITrakHoundObjectQueryResult>> QueryRoot(IEnumerable<string> namespaces, long skip = 0, long take = 0, SortOrder order = SortOrder.Ascending);


        Task<TrakHoundResponse<ITrakHoundObjectQueryResult>> QueryByParentUuid(IEnumerable<string> parentUuids, long skip = 0, long take = 0, SortOrder order = SortOrder.Ascending);

        Task<TrakHoundResponse<ITrakHoundObjectQueryResult>> QueryByChildUuid(IEnumerable<string> childUuids, long skip = 0, long take = 0, SortOrder order = SortOrder.Ascending);
        

        Task<TrakHoundResponse<ITrakHoundObjectQueryResult>> QueryChildrenByRootUuid(IEnumerable<string> rootUuids, long skip = 0, long take = 0, SortOrder order = SortOrder.Ascending);

        Task<TrakHoundResponse<ITrakHoundObjectQueryResult>> QueryRootByChildUuid(IEnumerable<string> childUuids, long skip = 0, long take = 0, SortOrder order = SortOrder.Ascending);


        Task<TrakHoundResponse<TrakHoundCount>> QueryChildCount(IEnumerable<string> uuids);
    }
}
