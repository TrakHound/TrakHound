// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Entities;

namespace TrakHound.Drivers.Entities
{
    /// <summary>
    /// Api Driver used to read TrakHound Blob Entities.
    /// </summary>
    public interface IObjectBlobQueryDriver : ITrakHoundDriver
    {
        /// <summary>
        /// Query the Blob Entities with the specified Object UUID's
        /// </summary>
        Task<TrakHoundResponse<ITrakHoundObjectBlobEntity>> Query(IEnumerable<string> objectUuids);
    }
}
