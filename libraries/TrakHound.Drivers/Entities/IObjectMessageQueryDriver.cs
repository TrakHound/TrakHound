// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Entities;

namespace TrakHound.Drivers.Entities
{
    /// <summary>
    /// Driver used to Query TrakHound Object Messages
    /// </summary>
    public interface IObjectMessageQueryDriver : ITrakHoundDriver
    {
        /// <summary>
        /// Query the Message Entities with the specified Object UUID's
        /// </summary>
        Task<TrakHoundResponse<ITrakHoundObjectMessageEntity>> Query(IEnumerable<string> objectUuids);
    }
}
