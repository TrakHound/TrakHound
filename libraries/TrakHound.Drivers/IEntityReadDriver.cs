// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Entities;

namespace TrakHound.Drivers.Entities
{
    /// <summary>
    /// Entity Driver specifically for reading the TrakHound Entities with the specified ID(s).
    /// </summary>
    public interface IEntityReadDriver : ITrakHoundDriver { }


    /// <summary>
    /// Entity Driver specifically for reading the TrakHound <typeparamref name="TEntity"/> Entities with the specified ID(s).
    /// </summary>
    /// <typeparam name="TEntity">The Type of TrakHound Entity</typeparam>
    public interface IEntityReadDriver<TEntity> : IEntityReadDriver where TEntity : ITrakHoundEntity
    {
        Task<TrakHoundResponse<TEntity>> Read(IEnumerable<string> uuids);
    }
}
