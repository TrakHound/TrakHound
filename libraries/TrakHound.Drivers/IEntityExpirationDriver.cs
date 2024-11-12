// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Entities;

namespace TrakHound.Drivers.Entities
{
    /// <summary>
    /// Entity Driver for Expiring TrakHound Entities.
    /// </summary>
    public interface IEntityExpirationDriver : ITrakHoundDriver
    {
        Task<TrakHoundResponse<EntityDeleteResult>> Expire(IEnumerable<EntityDeleteRequest> requests);

        Task<TrakHoundResponse<EntityDeleteResult>> Expire(long created);
    }

    /// <summary>
    /// Entity Driver for Expiring TrakHound Entities.
    /// </summary>
    public interface IEntityExpirationUpdateDriver : ITrakHoundDriver
    {
        Task<TrakHoundResponse<EntityDeleteResult>> ExpireByUpdate(IEnumerable<EntityDeleteRequest> requests);

        Task<TrakHoundResponse<EntityDeleteResult>> ExpireByUpdate(long lastUpdated);
    }

    /// <summary>
    /// Entity Driver for Expiring TrakHound Entities.
    /// </summary>
    public interface IEntityExpirationAccessDriver : ITrakHoundDriver
    {
        Task<TrakHoundResponse<EntityDeleteResult>> ExpireByAccess(IEnumerable<EntityDeleteRequest> requests);

        Task<TrakHoundResponse<EntityDeleteResult>> ExpireByAccess(long lastAccessed);
    }


    /// <summary>
    /// Entity Driver specifically for Expiring TrakHound Entities.
    /// </summary>
    public interface IEntityExpirationDriver<TEntity> : IEntityExpirationDriver where TEntity : ITrakHoundEntity { }

    /// <summary>
    /// Entity Driver specifically for Expiring TrakHound Entities.
    /// </summary>
    public interface IEntityExpirationUpdateDriver<TEntity> : IEntityExpirationUpdateDriver where TEntity : ITrakHoundEntity { }

    /// <summary>
    /// Entity Driver specifically for Expiring TrakHound Entities.
    /// </summary>
    public interface IEntityExpirationAccessDriver<TEntity> : IEntityExpirationAccessDriver where TEntity : ITrakHoundEntity { }
}
