// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Entities;

namespace TrakHound.Clients
{
    public partial interface ITrakHoundSystemDefinitionClient
    {

        #region "Query"

        Task<IEnumerable<ITrakHoundDefinitionEntity>> Query(string pattern, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null);


        Task<IEnumerable<ITrakHoundDefinitionEntity>> QueryByParentUuid(string parentUuid, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null);

        Task<IEnumerable<ITrakHoundDefinitionEntity>> QueryByParentUuid(IEnumerable<string> parentUuids, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null);


        Task<IEnumerable<ITrakHoundDefinitionEntity>> QueryByChildUuid(string childUuid, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null);

        Task<IEnumerable<ITrakHoundDefinitionEntity>> QueryByChildUuid(IEnumerable<string> childUuids, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null);


        Task<IEnumerable<ITrakHoundDefinitionEntity>> QueryChildrenByRootUuid(string rootUuid, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null);

        Task<IEnumerable<ITrakHoundDefinitionEntity>> QueryChildrenByRootUuid(IEnumerable<string> rootUuids, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null);


        Task<IEnumerable<ITrakHoundDefinitionEntity>> QueryRootByChildUuid(string childUuid, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null);

        Task<IEnumerable<ITrakHoundDefinitionEntity>> QueryRootByChildUuid(IEnumerable<string> childUuids, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null);

        #endregion

        #region "Query by Type"

        Task<IEnumerable<ITrakHoundDefinitionEntity>> QueryByType(string type, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null);
        Task<IEnumerable<ITrakHoundDefinitionEntity>> QueryByType(IEnumerable<string> types, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null);

        Task<IEnumerable<ITrakHoundDefinitionQueryResult>> QueryIdsByType(string type, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null);
        Task<IEnumerable<ITrakHoundDefinitionQueryResult>> QueryIdsByType(IEnumerable<string> types, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null);

        #endregion

    }
}
