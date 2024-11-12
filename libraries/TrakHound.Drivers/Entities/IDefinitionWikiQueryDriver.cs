// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Entities;

namespace TrakHound.Drivers.Entities
{
    /// <summary>
    /// Api Driver used to Query TrakHound Wiki Entities.
    /// </summary>
    public interface IDefinitionWikiQueryDriver : ITrakHoundDriver
    {
        /// <summary>
        /// Query Wiki Entities with the specified Entity UUID's
        /// </summary>
        Task<TrakHoundResponse<ITrakHoundDefinitionWikiEntity>> Query(IEnumerable<string> definitionUuids);
    }
}
