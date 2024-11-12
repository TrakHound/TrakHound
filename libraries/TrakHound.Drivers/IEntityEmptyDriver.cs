// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Entities;

namespace TrakHound.Drivers.Entities
{
    public interface IEntityEmptyDriver : ITrakHoundDriver { }


    /// <summary>
    /// Entity Driver specifically for Setting TrakHound <typeparamref name="TEntity"/> Entities to Empty.
    /// </summary>
    /// <typeparam name="TEntity">The Type of TrakHound Entity to Set to Empty</typeparam>
    public interface IEntityEmptyDriver<TEntity> : IEntityEmptyDriver where TEntity : ITrakHoundEntity
    {
        Task<TrakHoundResponse<bool>> Empty(IEnumerable<EntityEmptyRequest> requests);
    }
}
