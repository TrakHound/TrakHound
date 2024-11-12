// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Entities;

namespace TrakHound.Drivers.Entities
{
    public interface IObjectExpirationDriver : ITrakHoundDriver
    {
        Task<TrakHoundResponse<EntityDeleteResult>> Expire(IEnumerable<string> patterns, long created);
    }

    public interface IObjectExpirationUpdateDriver : ITrakHoundDriver
    {
        Task<TrakHoundResponse<EntityDeleteResult>> ExpireByUpdate(IEnumerable<string> patterns, long lastUpdated);
    }

    public interface IObjectExpirationAccessDriver : ITrakHoundDriver
    {
        Task<TrakHoundResponse<EntityDeleteResult>> ExpireByAccess(IEnumerable<string> patterns, long lastAccessed);
    }
}
