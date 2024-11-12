// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Entities;

namespace TrakHound.Drivers.Entities
{
    /// <summary>
    /// Driver used to read TrakHound Number Entities.
    /// </summary>
    public interface IObjectNumberQueryDriver : ITrakHoundDriver
    {
        /// <summary>
        /// Query the Number Entities with the specified Object UUID's
        /// </summary>
        Task<TrakHoundResponse<ITrakHoundObjectNumberEntity>> Query(IEnumerable<string> objectUuids);

        Task<TrakHoundResponse<ITrakHoundObjectNumberEntity>> Query(IEnumerable<string> objectUuids, double min, double max);
    }
}
