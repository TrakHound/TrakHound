// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Entities;

namespace TrakHound.Drivers.Entities
{
    public interface IEntityIndexQueryDriver : ITrakHoundDriver
    {
        Task<TrakHoundResponse<bool>> IndexExists(IEnumerable<string> targets);

        Task<TrakHoundResponse<string>> QueryIndex(IEnumerable<EntityIndexRequest> requests, long skip, long take, SortOrder sortOrder);
    }

    public interface IEntityIndexUpdateDriver : ITrakHoundDriver
    {
        Task<TrakHoundResponse<bool>> UpdateIndex(IEnumerable<EntityIndexPublishRequest> requests);
    }

    public interface IEntityIndexQueryDriver<TEntity> : IEntityIndexQueryDriver where TEntity : ITrakHoundEntity { }

    public interface IEntityIndexUpdateDriver<TEntity> : IEntityIndexUpdateDriver where TEntity : ITrakHoundEntity { }
}
