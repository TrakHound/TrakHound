// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Entities;

namespace TrakHound.Drivers.Entities
{
    /// <summary>
    /// Driver used to read TrakHound Reference Entities.
    /// </summary>
    public interface IObjectReferenceQueryDriver : ITrakHoundDriver
    {
        /// <summary>
        /// Query the Reference Entities with the specified Object UUID's
        /// </summary>
        Task<TrakHoundResponse<ITrakHoundObjectReferenceEntity>> Query(IEnumerable<string> objectUuids);
    }
}
