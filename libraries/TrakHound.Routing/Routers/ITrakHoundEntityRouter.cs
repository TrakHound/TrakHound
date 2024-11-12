// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Entities;

namespace TrakHound.Routing.Routers
{
    public interface ITrakHoundEntityRouter<TEntity> where TEntity : ITrakHoundEntity
    {
        TrakHoundRouter Router { get; }


        Task<TrakHoundResponse<TEntity>> Read(IEnumerable<string> uuids, string requestId = null);

        Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<TEntity>>>> Subscribe(string requestId = null);

        Task<TrakHoundResponse<TrakHoundPublishResult<TEntity>>> Publish(IEnumerable<TEntity> entities, TrakHoundOperationMode publishMode = TrakHoundOperationMode.Async, string requestId = null);

        Task<TrakHoundResponse<bool>> Delete(IEnumerable<EntityDeleteRequest> requests, TrakHoundOperationMode publishMode = TrakHoundOperationMode.Async, string requestId = null);

        Task<TrakHoundResponse<EntityDeleteResult>> Expire(IEnumerable<EntityDeleteRequest> requests, string requestId = null);

        Task<TrakHoundResponse<EntityDeleteResult>> ExpireByUpdate(IEnumerable<EntityDeleteRequest> requests, string requestId = null);

        Task<TrakHoundResponse<EntityDeleteResult>> ExpireByAccess(IEnumerable<EntityDeleteRequest> requests, string requestId = null);

    }
}
