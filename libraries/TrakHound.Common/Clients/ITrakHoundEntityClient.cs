// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Entities;

namespace TrakHound.Clients
{
    public interface ITrakHoundEntityClient<TEntity> where TEntity : ITrakHoundEntity
    {
        Task<TEntity> ReadByUuid(string uuid, string routerId = null);

        Task<IEnumerable<TEntity>> ReadByUuid(IEnumerable<string> uuids, string routerId = null);


        Task<ITrakHoundConsumer<IEnumerable<TEntity>>> Subscribe(int interval = 0, int take = 1000, string consumerId = null, string routerId = null);


        Task<bool> Publish(TEntity entity, TrakHoundOperationMode mode = TrakHoundOperationMode.Async, string routerId = null);

        Task<bool> Publish(IEnumerable<TEntity> entities, TrakHoundOperationMode mode = TrakHoundOperationMode.Async, string routerId = null);


        Task<bool> Delete(string uuid, TrakHoundOperationMode mode = TrakHoundOperationMode.Async, string routerId = null);

        Task<bool> Delete(IEnumerable<string> uuids, TrakHoundOperationMode mode = TrakHoundOperationMode.Async, string routerId = null);


        Task<EntityDeleteResult> Expire(EntityDeleteRequest request, string routerId = null);

        Task<IEnumerable<EntityDeleteResult>> Expire(IEnumerable<EntityDeleteRequest> requests, string routerId = null);

        Task<EntityDeleteResult> ExpireByAccess(EntityDeleteRequest request, string routerId = null);

        Task<IEnumerable<EntityDeleteResult>> ExpireByAccess(IEnumerable<EntityDeleteRequest> requests, string routerId = null);

        Task<EntityDeleteResult> ExpireByUpdate(EntityDeleteRequest request, string routerId = null);

        Task<IEnumerable<EntityDeleteResult>> ExpireByUpdate(IEnumerable<EntityDeleteRequest> requests, string routerId = null);
    }
}
