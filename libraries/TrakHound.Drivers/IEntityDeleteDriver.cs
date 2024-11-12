// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Entities;

namespace TrakHound.Drivers.Entities
{
    /// <summary>
    /// Entity Driver specifically for deleting the TrakHound Entities with the specified UUID(s).
    /// </summary>
    public interface IEntityDeleteDriver : ITrakHoundDriver { }


    /// <summary>
    /// Entity Driver specifically for deleting the TrakHound <typeparamref name="TEntity"/> Entities with the specified UUID(s).
    /// </summary>
    /// <typeparam name="TEntity">The Type of TrakHound Entity</typeparam>
    public interface IEntityDeleteDriver<TEntity> : IEntityDeleteDriver where TEntity : ITrakHoundEntity
    {
        Task<TrakHoundResponse<bool>> Delete(IEnumerable<EntityDeleteRequest> requests);
    }
}
