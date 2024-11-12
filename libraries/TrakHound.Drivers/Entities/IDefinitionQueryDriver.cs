// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Entities;

namespace TrakHound.Drivers.Entities
{
    /// <summary>
    /// Driver used to Query TrakHound Definition IDs
    /// </summary>
    public interface IDefinitionQueryDriver : ITrakHoundDriver
    {
        /// <summary>
        /// Query the Driver for TrakHound Definition Entities matching the specified Pattern
        /// </summary>
        Task<TrakHoundResponse<string>> Query(string pattern, long skip = 0, long take = 0, SortOrder order = SortOrder.Ascending);

        /// <summary>
        /// Query the Driver for TrakHound Definitions matching the specified Types
        /// </summary>
        Task<TrakHoundResponse<ITrakHoundDefinitionQueryResult>> QueryByType(IEnumerable<string> types, long skip = 0, long take = 0, SortOrder order = SortOrder.Ascending);

        Task<TrakHoundResponse<ITrakHoundDefinitionQueryResult>> QueryByParentUuid(IEnumerable<string> parentUuids, long skip = 0, long take = 0, SortOrder order = SortOrder.Ascending);

        Task<TrakHoundResponse<ITrakHoundDefinitionQueryResult>> QueryByChildUuid(IEnumerable<string> childUuids, long skip = 0, long take = 0, SortOrder order = SortOrder.Ascending);
    }
}
