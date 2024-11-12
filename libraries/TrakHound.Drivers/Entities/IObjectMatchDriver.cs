// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Entities;

namespace TrakHound.Drivers.Entities
{
    /// <summary>
    /// Driver used to Match TrakHound Objects based on Patterns
    /// </summary>
    public interface IObjectMatchDriver : ITrakHoundDriver
    {
        Task<TrakHoundResponse<TrakHoundObjectMatchResult>> Match(IEnumerable<string> patterns, long skip = 0, long take = 0, SortOrder order = SortOrder.Ascending);
    }
}
