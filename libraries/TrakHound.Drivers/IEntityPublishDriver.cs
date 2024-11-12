// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Entities;

namespace TrakHound.Drivers.Entities
{
    public interface IEntityPublishDriver : ITrakHoundDriver { }


    /// <summary>
    /// Entity Driver specifically for Publishing TrakHound <typeparamref name="TEntity"/> Entities.
    /// </summary>
    /// <typeparam name="TEntity">The Type of TrakHound Entity to Publish</typeparam>
    public interface IEntityPublishDriver<TEntity> : IEntityPublishDriver where TEntity : ITrakHoundEntity
    {
        Task<TrakHoundResponse<TrakHoundPublishResult<TEntity>>> Publish(IEnumerable<TEntity> entities);
        //Task<TrakHoundResponse<TEntity>> Publish(IEnumerable<TEntity> entities);
    }
}
